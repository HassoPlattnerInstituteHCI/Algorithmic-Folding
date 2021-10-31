import openmesh as om
# OpenMesh docs are here
# https://openmesh-python.readthedocs.io/en/latest/iterators.html

import numpy as np
from numpy.linalg import inv

from math import sqrt, pi
from queue import SimpleQueue
from collections import defaultdict

import drawSvg as draw

mesh = om.read_trimesh('stanford_bunny.stl')
mesh.update_normals()

# making it easier to read the OpenMesh API
get_adjacent_faces = lambda mesh, face: mesh.ff(face)
get_vertecies_by_face = lambda mesh, face: mesh.fv(face)


d = draw.Drawing(10, 10, origin='center', displayInline=False)

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
	# TODO make this work with off-center axis
	from scipy.spatial.transform import Rotation
	return Rotation.from_rotvec(axis/np.linalg.norm(axis) * angle).as_matrix()


def unfold(mesh, spanning_tree):
	root = spanning_tree[-1][0]
	polygons = []

	def get_mapping(node, parent):
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

		for child in spanning_tree[node]:
			# add a new mapping function to the 'stack' that maps the child's coordinate system to the parent's
			# coordinate system. (which in turn will be handed of to the rest of the function stack to end up with the final coordinates)
			new_mapping = lambda points: mapping_fn(get_mapping(child, node)(points))
			unfolder_recursive_call(child, new_mapping)

	map_to_2d = lambda points: get_2d_projection(mesh, mesh.face_handle(root)).dot(points)
	unfolder_recursive_call(root, map_to_2d)

	return polygons


spanning_tree = bfs(mesh, mesh.face_handle(0))
print(unfold(mesh, spanning_tree))

for polygon in unfold(mesh, spanning_tree):
	# polygon = [coords[0:2] for coords in polygon]
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