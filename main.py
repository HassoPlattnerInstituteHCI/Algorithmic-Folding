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
def get_world_to_face_plane_mapping_fn(mesh, face):
	# this works by rotating the face such that the face normal vector matches the z-axis
	# https://math.stackexchange.com/questions/1956699/getting-a-transformation-matrix-from-a-normal-vector
	nx, ny, nz = mesh.normal(face)

	if nx == 0.0 and ny == 0.0:
		# nothing to do: points are already parallel to xy-plane
		transform_matrix = np.array([
			[1,0,0],
			[0,1,0]
		])
	else:
		transform_matrix = np.array([
			[ny / sqrt(nx**2+ny**2), -nx / sqrt(nx**2 + ny**2), 0],
			[nx * ny / sqrt(nx**2+ny**2), ny * nz / sqrt(nx**2 + ny**2), -sqrt(nx**2 + ny**2)]
		])

	return lambda point: transform_matrix.dot(point)






print(bfs(mesh, mesh.face_handle(0)))

import drawSvg as draw

d = draw.Drawing(200, 100, origin='center', displayInline=False)


for face in mesh.faces():
	convert_to_2d = get_world_to_face_plane_mapping_fn(mesh, face)	
	# Draw an irregular polygon
	line = [convert_to_2d(mesh.point(vertex)) for vertex in get_vertecies_by_face(mesh, face)]
	d.append(draw.Lines(*np.array(line).flatten(), closed=True,
	            fill='#eeee00',
	            stroke='black'))


d.saveSvg('example.svg')
# print(is_polygon_overlapping(polygon, polygon2))

# TODO align polygons in 2D

# TODO output visualization

# maybe TODO SVG export 
# maybe TODO kyub import