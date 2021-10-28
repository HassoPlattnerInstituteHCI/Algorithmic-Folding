# Docs are here
# https://openmesh-python.readthedocs.io/en/latest/iterators.html

import openmesh as om
import numpy as np

mesh = om.PolyMesh()

# add a a couple of vertices to the mesh
vh0 = mesh.add_vertex([0, 1, 0])
vh1 = mesh.add_vertex([1, 0, 0])
vh2 = mesh.add_vertex([2, 1, 0])
vh3 = mesh.add_vertex([0,-1, 0])
vh4 = mesh.add_vertex([2,-1, 0])

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
# om.write_mesh('test.stl', mesh)
# mesh_2 = om.read_trimesh('test.off')

# making it easier to read the OpenMesh API
get_adjacent_faces = lambda mesh, face: mesh.ff(face)

from queue import SimpleQueue

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


bfs(mesh, fh0)
# for fh in get_adjacent_faces(fh0):
#     print(fh.idx())

