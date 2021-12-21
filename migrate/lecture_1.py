from lecture_0 import *
from shapely.geometry import Polygon
from itertools import combinations

def is_unfolding_overlapping(polygons):
    polygons = [Polygon(p) for p in polygons]
    return any([pa.intersection(pb).area for pa, pb in combinations(polygons, 2)])

# def strip_2_tree(mesh, strip, trees = []):

    # #returns id of biggest face
    # # def best_root(mesh, strip):
        # # TODO:  pyny3d Polygon ??? 
        # # s = [(mesh.face_size(f), i) for i, f in enumerate(strip)]
        # # s.sort(key=lambda t : t[0])
        # # return s[-1][1]

    # # root_index = best_root(mesh, strip)
    # root_index = 0
    # Tree = nx.DiGraph()
    # curr = strip.pop(root_index)
    
    # remaining_strip = []

    # while strip:
        # #find all adjacent_faces to current face curr 
        # children = [f for f in strip if f in get_adjacent_faces(mesh, curr)]

        # #break if there are no adjecent faces to the current face left in the strip
        # if not children:
            # break

        # child = children[0] # TODO: heuristic
        # strip.remove(child)
        # Tree.add_edge(curr, child)
        # print((curr, child))
        
        # if is_unfolding_overlapping(unfold(mesh, Tree)):
            # remaining_strip.append(child)
            # Tree.remove_edge(curr, child)
        # else:
            # curr = child

    # if strip or remaining_strip:
        # remaining_strip += strip
        # trees = strip_2_tree(mesh, remaining_strip, trees)

    # trees.append(Tree)
    # return trees

def strip_2_tree(mesh, strip, trees = []):
    Tree = nx.DiGraph()

    curr = 0
    get_child = lambda : next((c for c in get_adjacent_faces(mesh, curr) if c in strip), None)

    child = get_child()
    while child:
        Tree.add_edge(curr, child)
        if is_unfolding_overlapping(unfold(mesh, Tree)):
            Tree.remove_edge(curr, child)
        else:
            strip.remove(curr)
            curr = child
        child = get_child()

    strip.remove(curr)
    if strip:
        trees = strip_2_tree(mesh, strip, trees)

    trees.append(Tree)
    return trees
# def attach_wings(mesh, trees, wings):
    # #Heuristic: Sort Spanning Trees by their size
    # trees.sort(key=lambda t : t.number_of_nodes, reverse=True)

    # remaining_wings = []
    # def attach_wing(trees, wing):
        # for t in trees: 

            # # All adjacent faces to wing currently in the tree
            # parent_faces = [f for f in get_adjacent_faces(mesh, wing) if t.has_node(f)]

            # #Heuristic: Sort parent faces by number of edges
            # #TODO: sort number of total edges minus number of occupied edges
            # parent_faces.sort(key=lambda f : len(mesh.face_edge_indices()[f]), reverse=True)

            # for parent_face in parent_faces:
                # t.add_edge(parent_face, wing)
                # if not is_unfolding_overlapping(unfold(mesh, t)):
                    # return
                # t.remove_edge(parent_face, wing)

        # remaining_wings.append(wing)

    # for wing in wings:
        # attach_wing(trees, wing)

    # if remaining_wings:
        # trees = strip_2_tree(mesh, remaining_wings, trees) #heuristic ????

    # return trees

def attach_wings(mesh, trees, wings, remaining_wings = []):
    # Exit Condition
    if not wings:
        if remaining_wings:
            trees = strip_2_tree(mesh, remaining_wings, trees) #heuristic ????
        return trees

    #Heuristic: Sort Spanning Trees by their size
    trees.sort(key=lambda t : t.number_of_nodes, reverse=True)

    wing = wings.pop(0)
    
    for t in trees: 
        # All adjacent faces to wing currently in the tree
        #Heuristic: Sort parent faces by number of edges
        parent_faces = [f for f in get_adjacent_faces(mesh, wing) if t.has_node(f)]
        parent_faces.sort(key=lambda f : len(mesh.face_edge_indices()[f]), reverse=True)

        for parent_face in parent_faces:
            t.add_edge(parent_face, wing)
            if not is_unfolding_overlapping(unfold(mesh, t)):
                return attach_wings(mesh, trees, wings, remaining_wings)
            t.remove_edge(parent_face, wing)

    return attach_wings(mesh, trees, wings, remaining_wings)

def dump_tree(Tree, node, dist = 0):
    print(dist * '\t', node)
    for c in Tree.successors(node):
        dump_tree(Tree, c, dist + 1)

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

    # TODO: Comments from notebooks

if __name__ == '__main__':
    mesh = om.read_polymesh('Models/cube.obj')
    mesh.update_normals()
    strip, wings = get_best_snw(mesh)
    trees = strip_2_tree(mesh, strip)
    trees = attach_wings(mesh, trees, wings)
    for i, t in enumerate(trees):
        poly = unfold(mesh, t)
        dump_tree(t, nxroot(t))
        draw_svg(poly, 'SVG/test' + str(i) + '.svg')
    print('Succ')
