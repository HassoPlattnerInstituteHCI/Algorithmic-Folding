import openmesh as om

class Mesh:
    def __init__(self, path):
        self.om_mesh = om.read_polymesh('Models/L.obj')
        self.om_mesh.update_normals()

    def faces(self):
        return self.om_mesh.faces()

    def face_normals(self):
        return list(self.om_mesh.face_normals())

    def get_adjacent_faces(self, face):
        return list(self.om_mesh.ff(face))

face_from_idx = lambda mesh, idx : list(mesh.faces())[idx]
get_adjacent_faces_idx = lambda mesh, idx : [f.idx() for f in get_adjacent_faces(mesh, face_from_idx(mesh, idx))]
#---------------------------------------------------------------------------------------------------------------
#Bens Spanning tree object
class node:
    def __init__(self, id):
        self.children = []
        self.id = id

    def get_children(self):
        return self.children

    def add_child(self, id):
        self.children.append(node(id))

    def has(self, id):
        def iter(root, id):
            if root.id == id:
                return True
            for c in root.children:
                if iter(c, id):
                    return True
            return False
        return iter(self, id)

    def dump_tree(self, dist = 0):
        print(dist * '\t', self.id)
        for c in self.get_children():
            c.dump_tree(dist + 1)

    def rem_node(self, node_id):
        if node in self.get_children():
            self.children.remove(node)
            return
        for c in self.get_children():
            c.rem_node(node)

    def insert_child(self, parent_id, child_id):
        if self.id == parent_id:
            self.add_child_node(child_id)
            return
        for c in self.get_children():
            c.insert_child(parent_id, child_id)

#---------------------------------------------------------------------------------------------------------------

#Puts every face:
#   - parallel to the wing_face in wing list
#   - normal to the wing_face in strip list
def strip_n_wings(mesh, wing_face_id):

    def is_normal(s1, s2):
        for c1, c2 in zip(s1, s2):
            if(abs(c1) != abs(c2)):
                return True
        return False

    all_normals = mesh.face_normals()
    norm = all_normals[wing_face_id]
    strip = [f.idx() for f in mesh.faces() if is_normal(norm, all_normals[f.idx()])]
    wings = [f.idx() for f in mesh.faces() if f.idx() not in strip]
    return strip, wings

def strip_2_tree(mesh, strip, trees = []):
    root = node(strip.pop(0))
    curr = root
    remaining_strip = []
    while len(strip) > 0:
        children_ids = [f for f in strip if f in mesh.get_adjacent_faces_idx(curr.id)]
        if children_ids:
            # TODO: Heuristic
            child_id = children[0]
            strip.remove(child_id)
            curr.add_child(node(child_id))
            polygons = root.unfold(mesh)
            if not is_polygon_possible(polygons):
                remaining_strip.append(child.id)
                tree_rem_node(root, child_id)
            else:
                curr = child
        else:
            break
    if len(strip) > 0 or len(remaining_strip) > 0:
        remaining_strip += strip
        trees = w_strip_2_tree(mesh, remaining_strip, trees)

    trees.append(root)
    return trees


if __name__ == '__main__':
    mesh = Mesh('Models/cube.obj')

    strip, wings = strip_n_wings(mesh, [f.idx() for f in mesh.faces()][0])
    print(strip)
    trees = strip_2_tree(mesh, strip)
    trees = w_attach_wings(mesh, trees, snw[1])
    for i, t in enumerate(trees):
        dump_tree(t)
        poly = unfold(mesh, t)
        draw_svg(poly, 'L_Shape' + str(i) + '.svg')
