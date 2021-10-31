import openmesh as om
# OpenMesh docs are here
# https://openmesh-python.readthedocs.io/en/latest/iterators.html

import numpy as np
from math import sqrt, pi
from queue import SimpleQueue
from collections import defaultdict

mesh = om.read_trimesh('cube.ply')
mesh.update_normals()

# making it easier to read the OpenMesh API
get_adjacent_faces = lambda mesh, face: mesh.ff(face)
get_vertecies_by_face = lambda mesh, face: mesh.fv(face)


def bfs(mesh, start_face):
	visited_faces = set()
	queue = SimpleQueue()
	
	spanning_tree = defaultdict(list)
	insert_to_spanningtree = lambda node_id, parent_id: spanning_tree[parent_id].append(node_id)
	
	queue.put(start_face)
	visited_faces.add(start_face.idx())
	# -1 is pointer for root
	insert_to_spanningtree(node_id=start_face.idx(), parent_id=-1)

	while not queue.empty():
		current_face = queue.get()

		for adjacent_face in get_adjacent_faces(mesh, current_face):
			if adjacent_face.idx() not in visited_faces:
				queue.put(adjacent_face)
				visited_faces.add(adjacent_face.idx())
				insert_to_spanningtree(node_id=adjacent_face.idx(), parent_id=current_face.idx())
				
	return spanning_tree

def is_polygon_overlapping(polygon_a, polygon_b):
	from shapely.geometry import Polygon
	return Polygon(polygon_a).intersects(Polygon(polygon_b))

def rotation_matrix(axis, theta):
    """
    Return the rotation matrix associated with counterclockwise rotation about
    the given axis by theta radians.
    source: https://stackoverflow.com/questions/6802577/rotation-of-3d-vector
    """
    axis = np.asarray(axis)
    axis = axis / math.sqrt(np.dot(axis, axis))
    a = math.cos(theta / 2.0)
    b, c, d = -axis * math.sin(theta / 2.0)
    aa, bb, cc, dd = a * a, b * b, c * c, d * d
    bc, ad, ac, ab, bd, cd = b * c, a * d, a * c, a * b, b * d, c * d
    return np.array([[aa + bb - cc - dd, 2 * (bc + ad), 2 * (bd - ac)],
                     [2 * (bc - ad), aa + cc - bb - dd, 2 * (cd + ab)],
                     [2 * (bd + ac), 2 * (cd - ab), aa + dd - bb - cc]])


def get_2d_projection(mesh, face):
	"""
	this works by rotating the face such that the face normal vector matches the z-axis
	source https://math.stackexchange.com/questions/1956699/getting-a-transformation-matrix-from-a-normal-vector
	"""
	nx, ny, nz = mesh.normal(face)

	if nx == 0.0 and ny == 0.0:
		# nothing to do: points are already parallel to xy-plane
		return np.array([
			[1,0,0],
			[0,1,0]
		])
	else:
		return np.array([
			[ny / sqrt(nx**2+ny**2), -nx / sqrt(nx**2 + ny**2), 0],
			[nx * ny / sqrt(nx**2+ny**2), ny * nz / sqrt(nx**2 + ny**2), -sqrt(nx**2 + ny**2)]
		])

def get_world_to_face_plane_mapping_fn(mesh, face):
	return lambda point: get_2d_projection(mesh, face).dot(point)

def get_edge_between_faces(mesh, face_a, face_b):
	for face_a_halfedge in mesh.fh(face_a):
		if mesh.face_handle(mesh.opposite_halfedge_handle(face_a_halfedge)).idx() == face_b.idx():
			return face_a_halfedge
	raise Error("there is no edge between these two faces")

def get_rotation_matrix(axis, angle):
	from scipy.spatial.transform import Rotation
	return Rotation.from_rotvec(angle * axis).as_matrix()

def unfold(mesh, spanning_tree):
	root = spanning_tree[-1][0]
	polygons = []

	def get_relative_rotation(node, parent):
		node_face = mesh.face_handle(node)
		parent_face = mesh.face_handle(parent)
		node_normal = mesh.normal(node_face)
		parent_normal = mesh.normal(parent_face)

		angle_between_faces = np.arccos(node_normal.dot(parent_normal)) + pi/2
		rotation_axis = mesh.normal(get_edge_between_faces(mesh, node_face, parent_face))
		# print(rotation_axis)
		return get_rotation_matrix(angle_between_faces, rotation_axis)

	def unfolder_recursive_call(node, transform_matrix_heritage):
		polygon_3d = [mesh.point(vertex_handle) for vertex_handle in get_vertecies_by_face(mesh, mesh.face_handle(node))]
		polygons.append([transform_matrix_heritage.dot(point_3d) for point_3d in polygon_3d])

		for child in spanning_tree[node]:
			# TODO check order of dot op
			new_transformation = transform_matrix_heritage.dot(get_relative_rotation(node, child))
			unfolder_recursive_call(child, new_transformation)

	unfolder_recursive_call(root, get_2d_projection(mesh, mesh.face_handle(root)))

	return polygons


spanning_tree = bfs(mesh, mesh.face_handle(0))
import drawSvg as draw
print(unfold(mesh, spanning_tree))

d = draw.Drawing(200, 100, origin='center', displayInline=False)

for polygon in unfold(mesh, spanning_tree):
	d.append(draw.Lines(*np.array(polygon).flatten(), closed=True,
	            fill='#eeee00'))

d.saveSvg('example.svg')


# TODO model polygon cube
# TODO dfs
# TODO steepest edge

# kyub_to_openmesh("imports/boxel.json")
# TODO align polygons in 2D

# TODO output visualization

# maybe TODO SVG export 
# maybe TODO kyub import