import openmesh as om
import networkx as nx
import numpy as np
import drawSvg as draw

# TODO: DFS Unfolding ?? 


get_adjacent_faces = lambda mesh, face : [f.idx() for f in mesh.faces() if f in mesh.ff(list(mesh.faces())[face])]
get_vertecies_by_face = lambda mesh, face: list(mesh.fv(face))

nxroot = lambda G : list(nx.topological_sort(G))[0]
# def bfs(mesh, start_face):

    # # TODO: is a lookup table cleaner/simpler ???
    # set_visited = lambda mesh, face, val=1 : mesh.set_face_property("visited", face, val)
    # clear_visited = lambda mesh : [set_visited(mesh, face, 0) for face in mesh.faces()]
    # is_visited = lambda mesh, face : mesh.face_property("visited", face)

    # queue = []
    # clear_visited(mesh)
    # Tree = nx.DiGraph()

    # queue.append(start_face)
    # set_visited(mesh, start_face)
    # Tree.add_node(start_face.idx())

    # while queue:
        # current_face = queue.pop(-1)
        # for af in get_adjacent_faces(mesh, current_face):
            # if not is_visited(mesh, af):
                # queue.append(af)
                # set_visited(mesh, af)
                # Tree.add_edge(current_face.idx(), af.idx())

    # return Tree

# TODO: Comments
# def bfs(mesh, start_face):
    # queue = [start_face]
    # visited = [start_face]
    # Tree = nx.DiGraph()
    # Tree.add_node(start_face.idx())

    # while queue:
        # current_face = queue.pop(-1)
        # work = lambda af : (queue.append(af), visited.append(af), Tree.add_edge(current_face.idx(), af.idx()))
        # [work(af) for af in get_adjacent_faces(mesh, current_face) if af not in visited]
        # # naf = [af for af in get_adjacent_faces(mesh, current_face) if af not in visited]
        # # queue += naf
        # # visited += naf
        # # [Tree.add_edge(current_face.idx(), af.idx()) for af in naf]
    # return Tree

# def bfs(mesh, start_face):
    # Tree = nx.DiGraph()

    # def helper(queue, visited):
        # if queue:
            # current_face = queue.pop(-1)
            # naf = [af for af in get_adjacent_faces(mesh, current_face) if af not in visited]
            # Tree.add_edges_from( map(lambda af: (current_face, af), naf) )
            # helper(queue + naf, visited + naf)

    # helper([start_face], [start_face])
    # return Tree

# def bfs(mesh, start_face):
    # Tree = nx.DiGraph()
    # queue = [start_face]
    # visited = [start_face]

    # while queue:
        # current_face = queue.pop(-1)
        # naf = [af for af in get_adjacent_faces(mesh, current_face) if af not in visited]
        # Tree.add_edges_from( map(lambda af: (current_face, af), naf) )
        # queue += naf
        # visited += naf

    # return Tree

def bfs(mesh, start_face):
    Tree = nx.DiGraph()
    queue = [start_face]
    visited = [start_face]

    for current_face in queue:
        naf = [af for af in get_adjacent_faces(mesh, current_face) if af not in visited]
        Tree.add_edges_from( map(lambda af: (current_face, af), naf) )
        queue += naf
        visited += naf

    return Tree

# def bfs(mesh, start_face):
    # queue = [start_face]
    # visited = []

    # while queue:
        # cf = queue.pop(-1)
        # naf = [(cf, af) for af in get_adjacent_faces(mesh, cf) if not any([af in e for e in visited])]
        # queue += map(lambda x: x[1], naf)
        # visited += naf

    # Tree = nx.DiGraph()
    # Tree.add_edges_from(visited)
    # return Tree

def get_edge_between_faces(mesh, face_a, face_b):
	for face_a_halfedge in mesh.fh(face_a):
		if mesh.face_handle(mesh.opposite_halfedge_handle(face_a_halfedge)).idx() == face_b.idx():
			return face_a_halfedge
	raise Error("there is no edge between these two faces")

def get_rotation_matrix(axis, angle):
    from scipy.spatial.transform import Rotation
    return Rotation.from_rotvec(axis/np.linalg.norm(axis) * angle).as_matrix()

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

def unfold(mesh, spanning_tree):

    polygons = []

    def get_unfolding_coordinate_mapping(node, parent):
        crease_halfedge = get_edge_between_faces(mesh,  mesh.face_handle(node),  mesh.face_handle(parent))
        crease_angle = mesh.calc_dihedral_angle(crease_halfedge)
        crease_vector = mesh.calc_edge_vector(crease_halfedge)

        # offset is introduces, since rotation is not occuring around the origin but along crease line
        offset = mesh.point(mesh.from_vertex_handle(crease_halfedge))

        return lambda points: get_rotation_matrix(crease_vector, crease_angle).dot(points - offset) + offset

    def unfolder_recursive_call(node, mapping_fn):
        polygon_3d = [mesh.point(vertex_handle) for vertex_handle in get_vertecies_by_face(mesh, mesh.face_handle(node))]
        # apply previous coordinate mapping function and store polygon
        polygons.append([mapping_fn(point_3d) for point_3d in polygon_3d])

        # for child in spanning_tree.get_children(node):
        for child in spanning_tree.successors(node):
            # add a new mapping function to the 'stack' that maps the child's coordinate system to the parent's
            # coordinate system. (which in turn will be handed of to the rest of the function stack to end up with the final coordinates)
            new_mapping = get_unfolding_coordinate_mapping(child, node)
            unfolder_recursive_call(child, lambda points: mapping_fn(new_mapping(points)))

    map_to_2d = lambda points: get_2d_projection(mesh, mesh.face_handle(nxroot(spanning_tree))).dot(points)
    unfolder_recursive_call(nxroot(spanning_tree), map_to_2d)
    return polygons

def draw_svg(polygons, file):
    scale = 1000
    d = draw.Drawing(scale, scale, origin='center', displayInline=False)
    for polygon in polygons:
        # polygon = [coords[0:2] for coords in polygon]
        d.append(draw.Lines(*np.array(polygon).flatten()*50, close=True, fill='#eeee00', stroke='#000', stroke_width=.1))
    d.saveSvg(file)

if __name__ == '__main__':
    mesh = om.read_polymesh('Models/cube.obj')
    mesh.update_normals()
    start_face = list(mesh.faces())[0].idx()
    Tree = bfs(mesh, start_face)
    polygons = unfold(mesh, Tree)
    draw_svg(polygons, 'SVG/test.svg')
    print("Succ")
