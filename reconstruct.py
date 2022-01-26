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

    def __iter__(self):
        yield self.x
        yield self.y
        yield self.z

def is_onedge(edge, point):
    def same(l):
        l = [e for e in l if e != None]
        return None if len(l) <= 0 or sum([e == l[0] for e in l]) != len(l) else l[0]

    ea = Vec(edge[0][0], edge[0][1], edge[0][2])
    eb = Vec(edge[1][0], edge[1][1], edge[1][2])
    p = Vec(point[0], point[1], point[2])

    support = tuple(eb - ea)
    t = tuple(p - ea)

    # None shinanigans necessary to deal with division by 0
    # --> put None for every division by zero
    # --> filter out all None later in same(...)
    div = lambda a, b : a / b if b != 0 else None
    t = [div(a, b) for a, b in zip(t, support)]
    t = same(t)

    if t:
        return 0 < same(t) < 1
    return False

def insert_vertex(face, vertex):
    for i, v in enumerate(face):
        edge = (face[i - 1], v)
        if is_onedge(edge, vertex):
            face.insert(i, vertex)
            return face
    return face

def cut_face(face, s_line):
    edges = [(face[i - 1], v) for i, v in enumerate(face)]
    fa = [(s_line)]
    fb = []
    appending = False
    for e in edges:
        if e[0] == s_line[1]:
            appending = True

        if appending:
            fa.append(e)
        else:
            fb.append(e)

        if e[1] == s_line[0]:
            appending = False

    fb.append(s_line[::-1])
    collapse = lambda face : [e[0] for e in face]
    return collapse(fa), collapse(fb)

def print_edges(face):
    for i, v in enumerate(face):
        print(face[i - 1], v)

if __name__ == '__main__':
    face = [ (0, 1, 0), (1, 1, 0), (1, 0, 0), (0, 0, 0) ]
    face = insert_vertex(face, (0.25, 1, 0))
    fa, fb = cut_face(face, ((0, 0, 0), (0.25, 1, 0)))
    print(fa)
    print(fb)
