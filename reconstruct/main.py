class Vec:
    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z

    def len(self):
        return sqrt(self.x**2 + self.y**2 + self.z**2)

    def normalize(self):
        l = self.len()
        return Vec(self.x/l, self.y/l, self.z/l)

    def __add__(self, v):
        return Vec(self.x + v.x, self.y + v.y, self.z + v.z)

    def __mul__(self, s):
        return Vec(self.x * s, self.y * s, self.z * s)

    def __sub__(self, v):
        return Vec(self.x - v.x, self.y - v.y, self.z - v.z)

class Vertex:
    def __init__(self, id, pos_vec):
        self.id = id
        self.pos_vec = pos_vec

class Edge:
    def __init__(self, id, Vertex_a, Vertex_b):
        self.id = id
        self.Vertex_a = Vertex_a
        self.Vertex_b = Vertex_b

class Face:
    def __init__(self, id, edges):
        self.id = id
        self.edges = edges

class Mesh:
    def __init(self, id, faces):
        self.id = id
        self.faces = faces
