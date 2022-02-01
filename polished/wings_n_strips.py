import numpy as np
from unfolding_helper import *

# TODO: GJK Algorithm
#----------------------------------------------------------------------------------------------------------------------------------------------------------
#returns id of biggest face
def best_root(mesh, strip):
    s = [(mesh.face_size(f), i) for i, f in enumerate(strip)]
    s.sort(key=lambda t : t[0])
    return s[-1][1]

#simpler version without checks works only on cube
def cube_strip_2_tree(mesh, strip):

    root = node(strip.pop(0))
    curr = root
    while strip:
        #find all adjacent_faces to current face curr 
        children = [f for f in strip if f in mesh.get_adjacent_faces_idx(curr.val)]

        child_id = children[0] # TODO: heuristic
        strip.remove(child_id)
        child_node = node(child_id)

        curr.add_child_node(child_node)
        curr = child_node
    return [root]

def strip_2_tree(mesh, strip, trees = []):

    root_index = best_root(mesh, strip)
    root = node(strip.pop(root_index))

    curr = root
    remaining_strip = []

    while strip:
        #find all adjacent_faces to current face curr 
        children = [f for f in strip if f in mesh.get_adjacent_faces_idx(curr.val)]

        #break if there are no adjecent faces to the current face left in the strip
        if not children:
            break

        child_id = children[0] # TODO: heuristic
        strip.remove(child_id)
        child_node = node(child_id)
        curr.add_child_node(child_node)
        
        if is_unfolding_overlapping(root.unfold(mesh)):
            remaining_strip.append(child_node.val)
            root.rem_child_node(child_node)
        else:
            curr = child_node

    if len(strip) > 0 or len(remaining_strip) > 0:
        remaining_strip += strip
        trees = strip_2_tree(mesh, remaining_strip, trees)

    trees.append(root)
    return trees

# def cube_attach_wings(mesh, tree, wings):
    # def attach_wing(tree, wing):
        # for parent_face in mesh.get_adjacent_faces_idx(wing): 
            # if tree.has(parent_face):
                # tree.insert_child(parent_face, node(wing))
                # return

    # for wing in wings:
        # attach_wing(tree, wing)
    # return [tree]

def attach_wings(mesh, trees, wings):
    # TODO: Implement Heuristics
    remaining_wings = []
    def attach_wing(trees, wing):
        for t in trees: 
            for parent_face in (mesh.get_adjacent_faces_idx(wing)): #heuristic sort by something maybe check trees
                if t.has(parent_face):
                    t.insert_child(parent_face, node(wing))
                    if not is_unfolding_overlapping(t.unfold(mesh)):
                        return
                    else:
                        t.rem_child(wing)
        remaining_wings.append(wing)


    for wing in wings:
        attach_wing(trees, wing)

    if remaining_wings:
        trees = strip_2_tree(mesh, remaining_wings, trees) #heuristic ????
    return trees

def get_best_snw(mesh):

    #checks if to faces are normal to each other
    is_normal = lambda s1, s2 : sum([abs(c1) != abs(c2) for c1, c2 in zip(s1, s2)]) != 0

    # Create Strip and Wings list out of wing_face
    def strip_n_wings(face_normals, mesh):
        all_normals = list(mesh.face_normals())
        strip = [f.idx() for f in mesh.faces() if is_normal(face_normals, all_normals[f.idx()])]
        wings = [f.idx() for f in mesh.faces() if f.idx() not in strip]
        return (strip, wings)

    # Create Three (Strips, Wings) tuples for every axis 
    # TODO: non orthoganal faces
    snw_list = [strip_n_wings(f, mesh) for f in [[0, 0, 1], [0, 1, 0], [1, 0, 0]]]

    #sort them by length of strip
    snw_list.sort(key=lambda snw : len(snw[0]))

    #return the one with biggest strip
    return snw_list[-1]
#----------------------------------------------------------------------------------------------------------------------------------------------------------
if __name__ == '__main__':
    mesh = Mesh('../Models/L2.obj')
    strip, wings = get_best_snw(mesh)
    trees = strip_2_tree(mesh, strip)
    trees = attach_wings(mesh, trees, wings)
    for i, t in enumerate(trees):
        t.dump_tree()
        poly = t.unfold(mesh)
        draw_svg(poly, '../Unfolded_SVGs/ex' + str(i) + '.svg')
