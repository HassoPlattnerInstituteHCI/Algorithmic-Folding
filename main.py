import openmesh as om
import matplotlib.pyplot as plt
# OpenMesh docs are here
# https://openmesh-python.readthedocs.io/en/latest/iterators.html

import numpy as np
from numpy.linalg import inv
import networkx as nx

from math import sqrt, pi
from queue import SimpleQueue
from collections import defaultdict

import drawSvg as draw

def set_visited(mesh, face):
	mesh.set_face_property("visited", face, True)

def is_visited(mesh, face):
	return mesh.face_property("visited", face) == True

def clear_visited(mesh):
	for face in mesh.faces():
		mesh.set_face_property("visited", face, False)

mesh = om.read_polymesh('Models/cube.obj')
mesh.update_normals()

# making it easier to read the OpenMesh API
get_adjacent_faces = lambda mesh, face: list(mesh.ff(face))
get_vertecies_by_face = lambda mesh, face: list(mesh.fv(face))

def get_graph(mesh):
    faces = mesh.faces()
    Graph = nx.Graph()
    for f in faces:
        for af in get_adjacent_faces(mesh, f):
            edge = (f.idx(), af.idx())
            if not Graph.has_edge(*edge):
                Graph.add_edge(*edge)
    return Graph

class Tree:
	adjacency_lists = defaultdict(list)

	def get_root(self):
		return self.adjacency_lists[-1]

	def set_root(self, node_id):
		self.adjacency_lists[-1] = node_id

	def insert(self, node_id, parent_id):
		self.adjacency_lists[parent_id].append(node_id)

	def get_children(self, node_id):
		return self.adjacency_lists[node_id]

d = draw.Drawing(10, 10, origin='center', displayInline=False)


def bfs(mesh, start_face):
	queue = SimpleQueue()
	spanning_tree = Tree()
	clear_visited(mesh)
	
	queue.put(start_face)
	set_visited(mesh, start_face)
	spanning_tree.set_root(start_face.idx())

	while not queue.empty():
		current_face = queue.get()
		for adjacent_face in get_adjacent_faces(mesh, current_face):
			if not is_visited(mesh, adjacent_face):
				queue.put(adjacent_face)
				set_visited(mesh, adjacent_face)	
				spanning_tree.insert(node_id=adjacent_face.idx(), parent_id=current_face.idx())

	return spanning_tree


def is_polygon_overlapping(polygon_a, polygon_b):
	from shapely.geometry import Polygon
	return Polygon(polygon_a).intersects(Polygon(polygon_b))

def is_polygon_overlapping_fixed(polygon_a, polygon_b):
	from shapely.geometry import Polygon
	return Polygon(polygon_a).intersection(Polygon(polygon_b)).area

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

def get_edge_between_faces(mesh, face_a, face_b):
	for face_a_halfedge in mesh.fh(face_a):
		if mesh.face_handle(mesh.opposite_halfedge_handle(face_a_halfedge)).idx() == face_b.idx():
			return face_a_halfedge
	raise Error("there is no edge between these two faces")

def get_rotation_matrix(axis, angle):
    from scipy.spatial.transform import Rotation
    return Rotation.from_rotvec(axis/np.linalg.norm(axis) * angle).as_matrix()

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

		for child in spanning_tree.get_children(node):
			# add a new mapping function to the 'stack' that maps the child's coordinate system to the parent's
			# coordinate system. (which in turn will be handed of to the rest of the function stack to end up with the final coordinates)
			new_mapping = get_unfolding_coordinate_mapping(child, node)
			unfolder_recursive_call(child, lambda points: mapping_fn(new_mapping(points)))

	map_to_2d = lambda points: get_2d_projection(mesh, mesh.face_handle(spanning_tree.get_root())).dot(points)
	unfolder_recursive_call(spanning_tree.get_root(), map_to_2d)

	return polygons

def is_result_overlapping(polygons):
	for i in range(len(polygons)):
		for j in range(i+1, len(polygons)):
			if is_polygon_overlapping(polygons[i], polygons[j]):
				return True
	return False

def is_result_overlapping_fixed(polygons):
    for i in range(len(polygons)):
        for j in range(i+1, len(polygons)):
            if not is_polygon_overlapping_fixed(polygons[i], polygons[j]):
                return True
    return False

spanning_tree = bfs(mesh, mesh.face_handle(0))
polygons = unfold(mesh, spanning_tree)

# Graph = get_graph(mesh)
# print(len(list(spanning_tree_list)))

# print(spanning_tree.adjacency_lists)
# polygons = unfold(mesh, spanning_tree)
# print(polygons)

print(is_result_overlapping_fixed(polygons))

# for polygon in polygons:
    # # polygon = [coords[0:2] for coords in polygon]
    # d.append(draw.Lines(*np.array(polygon).flatten(), closed=True,
                # fill='#eeee00'))

# d.saveSvg('example.svg')

# TODO steepest edge unfolding
