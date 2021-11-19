import networkx as nx
from shapely.geometry import Polygon
from copy import deepcopy
import matplotlib.pyplot as plt
import openmesh as om
import numpy as np
import drawSvg as draw
from math import pi, sqrt

##################################################################################################################################################################
#Magic created by lucas - altered by Ben to work with Bens spanning tree object

def get_rotation_matrix(axis, angle):
    from scipy.spatial.transform import Rotation
    return Rotation.from_rotvec(axis/np.linalg.norm(axis) * angle).as_matrix()

def get_edge_between_faces(mesh, face_a, face_b):
	for face_a_halfedge in mesh.fh(face_a):
		if mesh.face_handle(mesh.opposite_halfedge_handle(face_a_halfedge)).idx() == face_b.idx():
			return face_a_halfedge
	raise Error("there is no edge between these two faces")

def get_2d_projection(mesh, face):
	"""
	this works by rotating the face such that the face normal vector matches the z-axis
	source https://math.stackexchange.com/questions/1956699/getting-a-transformation-matrix-from-a-normal-vector
	"""
	nx, ny, nz = mesh.normal(face)
	if nx == 0.0 and ny == 0.0:
		# nothing to do: points are already parallel to xy-plane: matrix just gets rid of z-values
		return np.array([
			[1,0,0],
			[0,1,0]
		])
	else:
		return np.array([
			[ny / sqrt(nx**2+ny**2), -nx / sqrt(nx**2 + ny**2), 0],
			[nx * ny / sqrt(nx**2+ny**2), ny * nz / sqrt(nx**2 + ny**2), -sqrt(nx**2 + ny**2)]
		])

get_adjacent_faces = lambda mesh, face: list(mesh.ff(face))
get_vertecies_by_face = lambda mesh, face: list(mesh.fv(face))

def unfold(mesh, spanning_tree):
	polygons = []

	def get_unfolding_coordinate_mapping(node, parent):
		crease_halfedge = get_edge_between_faces(mesh,  mesh.face_handle(node.val),  mesh.face_handle(parent.val))
		crease_angle = mesh.calc_dihedral_angle(crease_halfedge)
		crease_vector = mesh.calc_edge_vector(crease_halfedge)
	
		
		# offset is introduces, since rotation is not occuring around the origin but along crease line
		offset = mesh.point(mesh.from_vertex_handle(crease_halfedge))

		return lambda points: get_rotation_matrix(crease_vector, crease_angle).dot(points - offset) + offset

	def unfolder_recursive_call(node, mapping_fn):
		polygon_3d = [mesh.point(vertex_handle) for vertex_handle in get_vertecies_by_face(mesh, mesh.face_handle(node.val))]
		# apply previous coordinate mapping function and store polygon
		polygons.append([mapping_fn(point_3d) for point_3d in polygon_3d])

		for child in node.get_children():
			# add a new mapping function to the 'stack' that maps the child's coordinate system to the parent's
			# coordinate system. (which in turn will be handed of to the rest of the function stack to end up with the final coordinates)
			new_mapping = get_unfolding_coordinate_mapping(child, node)
			unfolder_recursive_call(child, lambda points: mapping_fn(new_mapping(points)))

	map_to_2d = lambda points: get_2d_projection(mesh, mesh.face_handle(spanning_tree.val)).dot(points)
	unfolder_recursive_call(spanning_tree, map_to_2d)

	return polygons

##################################################################################################################################################################
#Magic from Stackoverflow
def _expand(G, explored_nodes, explored_edges):
    """
    Expand existing solution by a process akin to BFS.

    Arguments:
    ----------
    G: networkx.Graph() instance
        full graph

    explored_nodes: set of ints
        nodes visited

    explored_edges: set of 2-tuples
        edges visited

    Returns:
    --------

    solutions: list, where each entry in turns contains two sets corresponding to explored_nodes and explored_edges
        all possible expansions of explored_nodes and explored_edges

    """
    frontier_nodes = list()
    frontier_edges = list()
    for v in explored_nodes:
        for u in nx.neighbors(G,v):
            if not (u in explored_nodes):
                frontier_nodes.append(u)
                frontier_edges.append([(u,v), (v,u)])

    return zip([explored_nodes | frozenset([v]) for v in frontier_nodes], [explored_edges | frozenset(e) for e in frontier_edges])

#removes all duplicate edges
def filt(tree):
    rem = []
    for i, (parent, child) in enumerate(tree):
        for edge in tree[i:]:
            if edge == (child, parent):
                rem.append(edge)
    return [edge for edge in tree if edge not in rem]

def find_all_spanning_trees(G, root=0):
    """
    Find all spanning trees of a Graph.

    Arguments:
    ----------
    G: networkx.Graph() instance
        full graph

    Returns:
    ST: list of networkx.Graph() instances
        list of all spanning trees

    """

    # initialise solution
    explored_nodes = frozenset([root])
    explored_edges = frozenset([])
    solutions = [(explored_nodes, explored_edges)]
    # we need to expand solutions number_of_nodes-1 times
    for ii in range(G.number_of_nodes()-1):
        # get all new solutions
        solutions = [_expand(G, nodes, edges) for (nodes, edges) in solutions]
        # flatten nested structure and get unique expansions
        solutions = set([item for sublist in solutions for item in sublist])

    trees = [tuple(edges )for (nodes, edges) in solutions]
    # return [filt(t) for t in trees] #need filtered version here
    return trees

##################################################################################################################################################################
#Bens Spanning tree object
class node:
    def __init__(self, val):
        self.children = []
        self.val = val

    def get_children(self):
        return self.children

    def add_child(self, val):
        self.children.append(node(val))

def create_tree(tree_list):

    def find_children(root, tree_list):
        rem = []
        for edge in tree_list:
            if root.val in edge:
                child = edge[0] if root.val == edge[1] else edge[1]
                root.add_child(child)
                rem.append(edge)
        tree_list = [edge for edge in tree_list if edge not in rem]

        for c in root.get_children():
            c, tree_list = find_children(c, tree_list)

        return root, tree_list

    root = node(tree_list[0][0])
    root.add_child(tree_list[0][1])
    find_children(root, tree_list[1:])
    return root

def dump_tree(root, dist = 0):
    print(dist * '\t', root.val)
    for c in root.get_children():
        dump_tree(c, dist + 1)

##################################################################################################################################################################
#Utils

#create nx Graph of faces from mesh
def get_graph(mesh):
    faces = mesh.faces()
    Graph = nx.Graph()
    for f in faces:
        for af in get_adjacent_faces(mesh, f):
            edge = (f.idx(), af.idx())
            if not Graph.has_edge(*edge):
                Graph.add_edge(*edge)
    return Graph

def draw_svg(polygons, file):
    d = draw.Drawing(100, 100, origin='center', displayInline=False)
    for polygon in polygons:
        # polygon = [coords[0:2] for coords in polygon]
        d.append(draw.Lines(*np.array(polygon).flatten(), closed=True,
                    fill='#eeee00'))
    d.saveSvg(file)

# TODO: fix this
#always returns 0.0
def overlapping_area(polygons):

    def is_polygon_overlapping_fixed(polygon_a, polygon_b):
        from shapely.geometry import Polygon
        return Polygon(polygon_a).intersection(Polygon(polygon_b)).area

    l = []
    for i in range(len(polygons)):
        for j in range(i+1, len(polygons)):
            l.append(is_polygon_overlapping_fixed(polygons[i], polygons[j]))
    return sum(l)

create_vec = lambda p1, p2 : (p1[0] - p2[0], p1[1] - p2[1])
len_vec = lambda vec : sqrt(abs(vec[0]**2 + vec[1]**2))
vecs_point_to_polygon = lambda point, polygon : [create_vec(point, p) for p in polygon]

def vecs_in_polygon(polygon):
    l = []
    for p in polygon:
        l += vecs_point_to_polygon(p, polygon)
    return l

def is_point_in_polygon(point, polygon):
    longest_vec_in_polygon = max([len_vec(v) for v in vecs_in_polygon(polygon)])
    longest_vec_p2p = max([len_vec(v) for v in vecs_point_to_polygon(point, polygon)])
    return longest_vec_in_polygon > longest_vec_p2p

def is_overlapping(polygon_a, polygon_b):
    for point in polygon_a:
        if is_point_in_polygon(point, polygon_b):
            return True
    return False

nparray_to_tuple = lambda arr : tuple(map(tuple, arr))

def is_same_polygon(polygon_a, polygon_b):
    polygon_a = [tuple(p) for p in polygon_a]
    polygon_b = [tuple(p) for p in polygon_b]
    swap = lambda vec : (vec[1], vec[0])
    return sum([p in polygon_b or swap(p) in polygon_b for p in polygon_a]) == len(polygon_b)
        
def is_polygon_possible(polygons):
    for i, polygon_a in enumerate(polygons):
        for  polygon_b in polygons[i + 1:]:
            if is_overlapping(polygon_a, polygon_b):
                return False
            if is_same_polygon(polygon_a, polygon_b):
                return False
    return True

##################################################################################################################################################################

if __name__ == '__main__':
    mesh = om.read_polymesh('Models/L.obj')
    mesh.update_normals()

    G = get_graph(mesh)
    print(G)

    trees = find_all_spanning_trees(G) #returns list of nx.Graphs
    print(len(trees))
    trees = [create_tree(t) for t in trees] #turns nx.Graphs to custom spanning tree object
    print(len(trees))
    trees = [unfold(deepcopy(mesh), t) for t in trees] #unfolds spanning tree objects
    print(len(trees))
    # for i, t in enumerate(trees):
        # draw_svg(t, 'Unfolded_SVGs/ex_' + str(i) + '.svg')
