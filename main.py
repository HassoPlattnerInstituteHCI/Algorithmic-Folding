# Docs are here
# https://openmesh-python.readthedocs.io/en/latest/iterators.html

import openmesh as om
import numpy as np
from math import sqrt
from queue import SimpleQueue

mesh = om.PolyMesh()

# add a a couple of vertices to the mesh
vh0 = mesh.add_vertex([0, 1, 1])
vh1 = mesh.add_vertex([1, 0, 1])
vh2 = mesh.add_vertex([2, 1, 2])
vh3 = mesh.add_vertex([0,-1, 1])
vh4 = mesh.add_vertex([2,-1, 1])

# add a couple of faces to the mesh
fh0 = mesh.add_face(vh0, vh1, vh2)
fh1 = mesh.add_face(vh1, vh3, vh4)
fh2 = mesh.add_face(vh0, vh3, vh1)

# add another face to the mesh, this time using a list
vh_list = [vh2, vh1, vh4]
fh3 = mesh.add_face(vh_list)

#  0 ==== 2
#  |\  0 /|
#  | \  / |
#  |2  1 3|
#  | /  \ |
#  |/  1 \|
#  3 ==== 4

# get the point with vertex handle vh0
point = mesh.point(vh0)

# get all points of the mesh
point_array = mesh.points()

# translate the mesh along the x-axis
point_array += np.array([1, 0, 0])

# write and read meshes
om.write_mesh('test.stl', mesh)
mesh = om.read_trimesh('test.stl')
mesh.update_normals()

# making it easier to read the OpenMesh API
get_adjacent_faces = lambda mesh, face: mesh.ff(face)
get_vertecies_by_face = lambda mesh, face: mesh.fv(face)


def bfs(mesh, start_face):
	visited_faces = set()
	queue = SimpleQueue()
	
	queue.put(start_face)
	visited_faces.add(start_face.idx())

	while not queue.empty():
		current_face = queue.get()

		for adjacent_face in get_adjacent_faces(mesh, current_face):
			if adjacent_face.idx() not in visited_faces:
				visited_faces.add(adjacent_face.idx())
				queue.put(adjacent_face)

		print(current_face.idx())
		# print(visited_faces)
		# print(list(queue.items))


def dfs(mesh, start_face):
	visited_faces = set()
	visited_faces.add(start_face.idx())
	current_face = start_face	

	while len(visited_faces) < len(mesh.faces()):
		for adjacent_face in get_adjacent_faces(mesh, current_face):
			if adjacent_face.idx() not in visited_faces:
				visited_faces.add(adjacent_face.idx())
				queue.put(adjacent_face)

		print(current_face.idx())
		# print(visited_faces)
		# print(list(queue.items))

# bfs(mesh, fh0)

# TODO model polygon cube

# TODO dfs
# TODO steepest edge
# TODO overlap checking
# TODO polygon 3D to local 2D coordinats
def get_2D_polygon(mesh, face):
	# this works by rotating the face such that the face normal vector matches the z-axis
	# https://math.stackexchange.com/questions/1956699/getting-a-transformation-matrix-from-a-normal-vector
	face_normal = mesh.normal(face)
	polygon_points_3d = [mesh.point(vertex_handle) for vertex_handle in get_vertecies_by_face(mesh, face)]

	nx, ny, nz = face_normal

	if nx == 0.0 and ny == 0.0:
		# points are already parallel to xy-plane. we just move them to z=0.
		return [np.array([point[0], point[2], 0.0]) for point in polygon_points_3d]

	rotation_matrix = np.array([
		[ny / sqrt(nx**2+ny**2), -nx / sqrt(nx**2 + ny**2), 0],
		[nx * ny / sqrt(nx**2+ny**2), ny * nz / sqrt(nx**2 + ny**2), -sqrt(nx**2 + ny**2)], 
		[nx, ny, nz]
	])
	
	return [rotation_matrix.dot(point) for point in polygon_points_3d]


print(get_2D_polygon(mesh, fh0))
# TODO align polygons in 2D
# TODO output visualization

# maybe TODO SVG export 
# maybe TODO kyub import