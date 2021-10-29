# Docs are here
# https://openmesh-python.readthedocs.io/en/latest/iterators.html

import openmesh as om
import numpy as np
from math import sqrt
from queue import SimpleQueue

mesh = om.read_trimesh('stanford_bunny.stl')
mesh.update_normals()

# making it easier to read the OpenMesh API
get_adjacent_faces = lambda mesh, face: mesh.ff(face)
get_vertecies_by_face = lambda mesh, face: mesh.fv(face)


def bfs(mesh, start_face):
	visited_faces = set()
	queue = SimpleQueue()
	tour = []
	
	queue.put(start_face)
	visited_faces.add(start_face.idx())

	while not queue.empty():
		current_face = queue.get()
		tour.append(current_face.idx())

		for adjacent_face in get_adjacent_faces(mesh, current_face):
			if adjacent_face.idx() not in visited_faces:
				visited_faces.add(adjacent_face.idx())
				queue.put(adjacent_face)
	
	return tour


# TODO model polygon cube

# TODO dfs
# TODO steepest edge


def is_polygon_overlapping(polygon_a, polygon_b):
	from shapely.geometry import Polygon
	return Polygon(polygon_a).intersects(Polygon(polygon_b))

# TODO polygon 3D to local 2D coordinats
def get_2D_polygon(mesh, face):
	# this works by rotating the face such that the face normal vector matches the z-axis
	# https://math.stackexchange.com/questions/1956699/getting-a-transformation-matrix-from-a-normal-vector
	face_normal = mesh.normal(face)
	polygon_points_3d = [mesh.point(vertex_handle) for vertex_handle in get_vertecies_by_face(mesh, face)]

	nx, ny, nz = face_normal

	if nx == 0.0 and ny == 0.0:
		# nothing to do: points are already parallel to xy-plane
		return [point[0:2] for point in polygon_points_3d]

	rotation_matrix = np.array([
		[ny / sqrt(nx**2+ny**2), -nx / sqrt(nx**2 + ny**2), 0],
		[nx * ny / sqrt(nx**2+ny**2), ny * nz / sqrt(nx**2 + ny**2), -sqrt(nx**2 + ny**2)], 
		[nx, ny, nz]
	])

	return [rotation_matrix.dot(point)[0:2] for point in polygon_points_3d]


polygon = get_2D_polygon(mesh, fh0)

polygon2 = get_2D_polygon(mesh, fh1)


print(polygon)
print(polygon2)

print(is_polygon_overlapping(polygon, polygon2))
# TODO align polygons in 2D

# TODO output visualization

# maybe TODO SVG export 
# maybe TODO kyub import