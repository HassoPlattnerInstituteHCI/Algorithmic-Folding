from math import sqrt
import openmesh as om

class Vec:
    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z

    def len(self):
        return sqrt(self.x**2 + self.y**2 + self.z**2)

    def normal(self):
        l = self.len()
        return Vec(self.x/l, self.y/l, self.z/l)

    def add(self, v):
        return Vec(self.x + v.x, self.y + v.y, self.z + v.z)

    def s_mult(self, s):
        return Vec(self.x * s, self.y * s, self.z * s)

    def dif(self, v):
        return Vec(self.x - v.x, self.y - v.y, self.z - v.z)

class Insertion_Vertex:
    def __init__(self, edge_id, dist):
        self.edge_id = edge_id
        self.dist = dist


    def calc_pos(self, edge):
        #BA/len(BA) * dist + A = point between B and A dist away from A
        return (edge[1].dif(edge[0])).normal().s_mult(self.dist).add(edge[0])

class Vertex:
    def __init__(self, id, pos):
        self.id = id
        self.pos = pos

class Edge:
    def __init__(self, id, vertices):
        self.id = id
        self.vertices = vertices

class Face:
    def __init__(self, id, edges):
        self.id = id
        self.edges = edges

class Object:
    def __init__(self, id, faces):
        self.id = id
        self.faces = faces

def mesh_2_Object(mesh):
    face_ids = [f.idx() for f in mesh.faces()]
    vertices = [Vertex(i, Vec(*v)) for i, v in enumerate(mesh.points())]
    for v in vertices:
        print(v.id, ':', (v.pos.x, v.pos.y, v.pos.z))

mesh = om.read_polymesh('test.obj')
mesh.update_normals()

mesh_2_Object(mesh)
