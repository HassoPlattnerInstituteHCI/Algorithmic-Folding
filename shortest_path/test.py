from shapely.geometry import *
from unfolding_helper import *
import openmesh as om

class Vector:
    def __init__(self, x, y):
        self.x = x
        self.y = y

def cut_edges(start, end, lines):
    main_line = LineString([start, end])
    end = []
    inter_point = lambda l : list(main_line.intersection(l).coords)[0]
    for l in lines:
        p = inter_point(l)
        end.append((list(l.coords)[0], p))
        end.append((list(l.coords)[1], p))
    return end

edges_2_lines = lambda edges : [LineString(e) for e in edges]

edges = [
        ((1, -2), (3, 3)), 
        ((3, 1), (3, -2)),
        ((3, -2), (5, 2))
        ]

def strip_n_wings(face_normals, mesh):
    is_normal = lambda s1, s2 : sum([abs(c1) != abs(c2) for c1, c2 in zip(s1, s2)]) != 0
    all_normals = list(mesh.face_normals())
    strip = [f.idx() for f in mesh.faces() if is_normal(face_normals, all_normals[f.idx()])]
    wings = [f.idx() for f in mesh.faces() if f.idx() not in strip]
    return (strip, wings)

def strip_2_tree(mesh, strip):
    root = node(strip.pop(0))
    curr = root
    while strip:
        #find all adjacent_faces to current face curr 
        children = [f for f in strip if f in mesh.get_adjacent_faces_idx(curr.val)]

        child_id = children[0]
        strip.remove(child_id)
        child_node = node(child_id)
        curr.add_child_node(child_node)
        curr = child_node

    return root

# Hardcoded Part:

#bottom points:
A = (1, 0, 0)
B = (1, 1, 0)
C = (0, 1, 0)
D = (0, 0, 0)

#top point
E = (1, 0, 1)
F = (1, 1, 1)
G = (0, 1, 1)
H = (0, 0, 1)
mesh = PolyMesh()

def add_new_vertices(vertecies, edges):
    if not vertecies:
        return edges
    # TODO: pop point from verticies
    # TODO: find edge which is interesected by point
    # TODO: rem edge from edges
    # TODO: create two new egdges (edge[0], point) (edge[1], point)
    return add_new_vertices(vertecies, edges)

def recon_mesh(edges):
    for e in edges:
        pass
        # TODO: mesh.add_edge(e) ??

if __name__ == '__main__':
    # print('Hello World')
    # mesh = Mesh('../Models/cube.obj')
    # strip = strip_n_wings([0, 0, 1], mesh)[0]
    # strip = strip_2_tree(mesh, strip)
    # strip.dump_tree()
    # strip = strip.unfold(mesh)
    # draw_svg(strip, 'test.svg')


    # edges = edges_2_lines(edges)
    # edges = cut_edges((1, 1), (4, -2), edges)
    # for e in edges:
        # print(e)
