# TODO: Heuristics
# TODO: Refactor most of it in unified style
# TODO: Not linked list like strips

import openmesh as om
from testnx import unfold, is_polygon_possible
import drawSvg as draw
import numpy as np

#----------------------------------------------------------------------------------------------------------------------------------------------------------

get_adjacent_faces = lambda mesh, face: list(mesh.ff(face))

def draw_svg(polygons, file):
    scale = 10000 * 10
    d = draw.Drawing(scale, scale, origin='center', displayInline=False)
    for polygon in polygons:
        # polygon = [coords[0:2] for coords in polygon]
        # d.append(draw.Lines(*np.array(polygon).flatten(), closed=True,
                    # fill='#eeee00'))
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

def strip_2_tree(strip, root, mesh):
    if len(strip) == 0:
        return root
    # TODO: heuristic needed currently taking first child
    child = [f for f in strip if f in get_adjacent_faces_idx(mesh, root.val)][0] 

    strip.remove(child)
    child = node(child)
    child = strip_2_tree(strip, child, mesh)
    root.add_child_node(child)
    return root

def w_strip_2_tree(mesh, strip, trees = []):
    print(strip)
    root = node(strip.pop(0))
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



def attach_wings(strip_root, wings):
    def attach_wing(strip_root, wing):
        parent_face = (get_adjacent_faces_idx(mesh, wing))[0]
        tree_insert_child(strip_root, parent_face, node(wing))

    for w in wings:
        attach_wing(strip_root, w)
    return strip_root

def strip_n_wings(face, mesh):
    norm = mesh.normal(face)
    all_normals = list(mesh.face_normals())
    strip = [f.idx() for f in mesh.faces() if is_normal(norm, all_normals[f.idx()])]
    wings = [f.idx() for f in mesh.faces() if f.idx() not in strip]
    return (strip, wings)
#----------------------------------------------------------------------------------------------------------------------------------------------------------

if __name__ == '__main__':
    mesh = om.read_polymesh('Models/L2.obj')
    mesh.update_normals()

    snw = strip_n_wings(list(mesh.faces())[0], mesh)
    print(snw)

    strip = list(snw[0])
    print(strip)
    # strip = [f for f in snw[0]]
    # print(strip)
    # root = node(strip.pop(0))
    # print(strip.pop(0))
    trees = w_strip_2_tree(mesh, strip)
    trees = w_attach_wings(mesh, trees, snw[1])
    # dump_tree(trees[0])
    for i, t in enumerate(trees):
        dump_tree(t)
        poly = unfold(mesh, t)
        draw_svg(poly, 'L_Shape' + str(i) + '.svg')
