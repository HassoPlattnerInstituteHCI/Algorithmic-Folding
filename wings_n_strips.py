# TODO: Heuristics
# TODO: Refactor most of it in unified style
# from testnx import unfold, is_polygon_possible
import drawSvg as draw
import numpy as np
from Mesh import Mesh

#----------------------------------------------------------------------------------------------------------------------------------------------------------

get_adjacent_faces = lambda mesh, face: list(mesh.ff(face))

def draw_svg(polygons, file):
    scale = 1000
    d = draw.Drawing(scale, scale, origin='center', displayInline=False)
    for polygon in polygons:
        # polygon = [coords[0:2] for coords in polygon]
        d.append(draw.Lines(*np.array(polygon).flatten()*50, close=True, fill='#eeee00', stroke='#000', stroke_width=.1))
    d.saveSvg(file)

#Bens Spanning tree object
class node:
    def __init__(self, val):
        self.children = []
        self.val = val

    def get_children(self):
        return self.children

    def add_child(self, val):
        self.children.append(node(val))

    def add_child_node(self, val):
        self.children.append(val)

    def has(self, val):
        def iter(root, val):
            if root.val == val:
                return True
            for c in root.children:
                if iter(c, val):
                    return True
            return False
        return iter(self, val)

def is_normal(s1, s2):
    for c1, c2 in zip(s1, s2):
        if(abs(c1) != abs(c2)):
            return True
    return False

def dump_tree(root, dist = 0):
    print(dist * '\t', root.val)
    for c in root.get_children():
        dump_tree(c, dist + 1)

face_from_idx = lambda mesh, idx : list(mesh.faces())[idx]
get_adjacent_faces_idx = lambda mesh, idx : [f.idx() for f in get_adjacent_faces(mesh, face_from_idx(mesh, idx))]

def tree_rem_node(root, node):
    if node in root.get_children():
        root.children.remove(node)
        return
    for c in root.get_children():
        tree_rem_node(c, node)

def tree_insert_child(root, parent, child):
    if root.val == parent:
        root.add_child_node(child)
        return
    for c in root.get_children():
        tree_insert_child(c, parent, child)
#----------------------------------------------------------------------------------------------------------------------------------------------------------

get_vertecies_by_face = lambda mesh, face: list(mesh.fv(face))
# polygon_size = lambda mesh, face : Polygon([mesh.point(vertex_handle) for vertex_handle in get_vertecies_by_face(mesh, mesh.face_handle(face))]).area
from math import sqrt

# Calc Size of face not possible with Shapely bcs z cord
def size(mesh, face):
    points = lambda mesh, face : [mesh.point(vertex_handle) for vertex_handle in get_vertecies_by_face(mesh, mesh.face_handle(face))]
    vec_3d = lambda p1, p2 : (p1[0] - p2[0], p1[1] - p2[1], p1[2] - p2[2])
    vec_len = lambda v : sqrt(v[0]**2 + v[1]**2 + v[2]**2)

    f_p = points(mesh, face)
    vec1 = vec_3d(f_p[0], f_p[1])
    vec2 = vec_3d(f_p[2], f_p[3])
    return vec_len(vec1) * vec_len(vec2)

#returns id of biggest face
def best_root(mesh, strip):
    s = [(size(mesh, f), f) for f in strip]
    s.sort(key=lambda t : t[0])
    return s[-1][1]

def w_strip_2_tree(mesh, strip, trees = []):
    root_index = best_root(mesh, strip)
    root = node(strip.pop(root_index))
    curr = root
    remaining_strip = []
    while len(strip) > 0:
        children = [f for f in strip if f in get_adjacent_faces_idx(mesh, curr.val)]
        if children:
            child = children[0]
            strip.remove(child)
            child = node(child)
            curr.add_child_node(child)
            
            polygons = unfold(mesh, root)
            if not is_polygon_possible(polygons):
                remaining_strip.append(child.val)
                tree_rem_node(root, child)
            else:
                curr = child
        else:
            break
    if len(strip) > 0 or len(remaining_strip) > 0:
        remaining_strip += strip
        trees = w_strip_2_tree(mesh, remaining_strip, trees)

    trees.append(root)
    return trees

def w_attach_wings(mesh, trees, wings):
    remaining_wings = []
    def w_attach_wing(trees, wing):
        parent_face = (get_adjacent_faces_idx(mesh, wing))[0] #heuristic sort by something maybe check trees
        for t in trees: 
            if t.has(parent_face):
                tree_insert_child(t, parent_face, node(wing))
                if is_polygon_possible(unfold(mesh, t)):
                    return
        remaining_wings.append(wing)

    for wing in wings:
        w_attach_wing(trees, wing)

    if len(remaining_wings) > 0:
        trees = w_strip_2_tree(mesh, remaining_wings, trees) #heuristic ????
    return trees

def get_best_snw(mesh):

    # Get Three Faces normal to each other (one for each axis x, y, z)
    def three_normal(mesh):
        all_normals = list(mesh.face_normals())
        faces = []
        for f in mesh.faces():
            if not sum([not is_normal(all_normals[f.idx()], all_normals[ff.idx()]) for ff in faces]):
                faces.append(f)
            if len(faces) == 3:
                return faces

    # Create Strip and Wings list out of wing_face
    def strip_n_wings(face, mesh):
        norm = mesh.normal(face)
        all_normals = list(mesh.face_normals())
        strip = [f.idx() for f in mesh.faces() if is_normal(norm, all_normals[f.idx()])]
        wings = [f.idx() for f in mesh.faces() if f.idx() not in strip]
        return (strip, wings)

    # Create Three (Strips, Wings) tuples for every axis 
    snw_list = [strip_n_wings(f, mesh) for f in three_normal(mesh)]

    #sort them by length of strip
    snw_list.sort(key=lambda snw : len(snw[0]))

    #return the one with biggest strip
    return snw_list[-1]

#----------------------------------------------------------------------------------------------------------------------------------------------------------

if __name__ == '__main__':
    mesh = om.read_polymesh('Models/cube.obj')
    mesh.update_normals()
    strip, wings = get_best_snw(mesh)
    trees = w_strip_2_tree(mesh, strip)
    trees = w_attach_wings(mesh, trees, wings)
    for i, t in enumerate(trees):
        dump_tree(t)
        poly = unfold(mesh, t)
        draw_svg(poly, 'Unfolded_SVGs/ex' + str(i) + '.svg')
