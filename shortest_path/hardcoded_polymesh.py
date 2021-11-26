from openmesh import *
mesh = PolyMesh()

A = (1, 0, 0)
B = (1, 1, 0)
C = (0, 1, 0)
D = (0, 0, 0)

#top point
E = (1, 0, 1)
F = (1, 1, 1)
G = (0, 1, 1)
H = (0, 0, 1)

A = mesh.add_vertex(A)
B = mesh.add_vertex(B)
C = mesh.add_vertex(C)
D = mesh.add_vertex(D)
E = mesh.add_vertex(E)
F = mesh.add_vertex(F)
G = mesh.add_vertex(G)
H = mesh.add_vertex(H)

#Order in which vertices are inserted is imported --> clockwise
bottom = mesh.add_face(A, B, C, D)
front = mesh.add_face(A, E, F, B)
right = mesh.add_face(B, F, G, C)
back = mesh.add_face(D, C, G, H)
left = mesh.add_face(A, D, H, E)
top = mesh.add_face(E, H, G, F)

write_mesh('test.obj', mesh)
