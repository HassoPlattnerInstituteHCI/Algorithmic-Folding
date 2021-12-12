import networkx as nx
from shapely.geometry import Polygon, Point, MultiPoint, LineString, MultiLineString
from shapely.affinity import rotate, translate, scale
from shapely.ops import linemerge, nearest_points
from shapely import wkt
import numpy as np

EPSILON = 3
ROUND_PRECISION = 6

# tree properties
def nodes_with_attribute(tree,att): return [n for n,d in tree.nodes(data=True) if d==att]
def interior_nodes(tree): return nodes_with_attribute(tree,{})
def tree_distance(n1,n2,d): return d[n1][n2]
def get_edge_weight (tree,node): return[(c["weight"]) for (a, b, c) in tree.edges(node,data=True)][0]
def path_to_leaf(node,tree,path = []):
    for n in tree.neighbors(node):
        if tree.degree(n) == 1: return path+[n] #found a leaf!
    path_to_leaf(n,tree,path+[n]) 
    
# affine transforms
def line_move_to (line,point): return translate(line, point.x- start_point(line).x,point.y - start_point(line).y)
def line_orthogonal(line): return rotate(line,90,origin=start_point(line))
def line_stretch(line,size): return scale(line,xfact=size/line.length,yfact=size/line.length,origin=start_point(line))
def line_flip(line): return rotate(line,180,origin=start_point(line))

# geometric calculations
def are_colinear(p1,p2,p3): return p2.intersects(line_from_points(p1,p3)) and not (p1.equals(p2) or p1.equals(p3) or p2.equals(p3))
def find_colinear_points(points): return [points[i-1] for i in range(len(points)) if are_colinear(points[i-2],points[i-1],points[i])]
def azimuth(point1, point2): return np.degrees(np.arctan2(point2.x - point1.x, point2.y - point1.y))
def parallel_line(line,offset,side): return line.parallel_offset(offset,side,join_style=2)
def buffer_line_symmetric(line_segments,offset): return [parallel_line(line_segments,offset,'left'),parallel_line(line_segments,offset,'right')]
def tiny_segment(line,p): return line_from_points(Point(coords(line.intersection(p.buffer(0.1)))),p)
def connect_points(points): return [line_from_points(points.centroid,p) for p in get_vertices(points)]
def angular_bisectors(polygon): return [line_stretch(line_from_points(v,vertex_at_index(i,polygon.buffer(-1))),polygon.length) for i,v in enumerate(get_vertices(polygon))]
def mirror_line(source_line,through_line,at_point):
    angle = azimuth(*get_vertices(source_line)) - azimuth(*get_vertices(tiny_segment(through_line,at_point)))
    return line_move_to(rotate(source_line,180+2*angle,origin=at_point),at_point)

def first_intersection (line,other_lines,ref = float('inf'),intersection_point = None,intersecting_line = None):
    for other_line in other_lines:
        intersection = line.intersection(other_line) 
        if not intersection.is_empty:
            for point in [intersection]:
                distance = start_point(line).distance(point)
                if EPSILON < distance < ref: 
                    intersection_point,intersecting_line,ref = point,other_line,distance
    return intersection_point, intersecting_line

# shapely wrappers
def convex_polygon(points): return Polygon(points).convex_hull
def nearest_vertex(point,geom): return nearest_points(point,geom)[1]
def start_point(geom): return get_vertices(geom)[0]
def end_point(geom): return get_vertices(geom)[-1]
def line_from_points(point1,point2): return LineString([point1,point2])
def approx(geom): return wkt.loads(wkt.dumps(geom,rounding_precision=ROUND_PRECISION))
def is_on_polygon(geom,collection): return geom.relate(collection) == 'F1FF0F212'
def not_in_collection(geom,collection): return geom.relate(collection) == 'FF100F102'
def get_vertices(geom): return MultiPoint(coords(geom)[:-1]) if type(geom) == Polygon else MultiPoint(coords(geom))
def coords (geom): 
    if type(geom) == Point: return geom.coords[0]
    if type(geom) == LineString: return geom.coords[:]
    if type(geom) == Polygon: return geom.exterior.coords[:]
    if type(geom) == list or tuple: return [coords(g) for g in geom]
    
# node <-> geometry maps    
def vertex_from_node(node,node_map): return Point(next((point for point, i in node_map.items() if i == node), None))
def vertices_in_origin(vertices,node_map_origin,node_map): return [vertex_from_node(get_nodes(vertex,node_map),node_map_origin) for vertex in vertices]
def line_in_origin(line,node_map_origin,node_map): return line_from_points(*vertices_in_origin(get_vertices(line),node_map_origin,node_map))
def line_from_nodes(node1,node2,node_map): return line_from_points(vertex_from_node(node1,node_map),vertex_from_node(node2,node_map))
def get_nodes(geom,node_map): return node_map[coords(geom)] if type(geom) == Point else [node_map[g] for g in coords(geom)] 

# vertices in polygons
def index_from_vertex(vertex,polygon): return coords(polygon).index(coords(vertex))
def vertex_at_index(index,polygon): return get_vertices(polygon)[index]
def edge_from_indices(index1,index2,polygon): return line_from_points(coords(polygon)[index1],coords(polygon)[index2])
def vertex_in_polygon(vertex,polygons): return [p for p in polygons if coords(vertex) in coords(p)]
def adjacent_edges(vertex,polygon):
    index = index_from_vertex(vertex,polygon)
    return [edge_from_indices(index,index+1,polygon), edge_from_indices(index-1,index,polygon)]

# other helper functions
def query_matrix(m,val):
    res = np.where(m==val)
    return list(zip(res[0],res[1]))

def extend_lines(lines,extension):
    for i,line in enumerate(lines):
        if approx(extension).touches(approx(line)):
            merge = linemerge(MultiLineString([extension,line]))
            if type(merge) == LineString: lines[i] = merge
            return lines
    lines += [extension]
    return lines