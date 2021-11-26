import openmesh as om
import drawSvg as draw
from math import sqrt
import numpy as np

class Mesh:
    def __init__(self, path):
        self.model = om.read_polymesh(path)
        self.model.update_normals()

    def face_normals(self):
        return self.model.face_normals()

    def get_vertecies_by_face(self, face):
        return list(self.model.fv(face))

    def get_adjacent_faces(self, face):
        return list(self.model.ff(face))

    def faces(self):
        return self.model.faces()

    def normal(self, face):
        return self.model.normal(face)

    def get_edge_between_faces(self, face_a, face_b):
        for face_a_halfedge in self.model.fh(face_a):
            if self.model.face_handle(self.model.opposite_halfedge_handle(face_a_halfedge)).idx() == face_b.idx():
                return face_a_halfedge
        raise Error("there is no edge between these two faces")

    def face_handle(self, face):
        return self.model.face_handle(face)

    def fv(self, face):
        return self.model.fv(face)

    def point(self, vertex_handle):
        return self.model.point(vertex_handle)

    def ff(self, face):
        return self.model.ff(face)

    def calc_dihedral_angle(self, crease_halfedge):
        return self.model.calc_dihedral_angle(crease_halfedge)

    def calc_edge_vector(self, crease_halfedge):
        return self.model.calc_edge_vector(crease_halfedge)

    def from_vertex_handle(self, crease_halfedge):
        return self.model.from_vertex_handle(crease_halfedge)

    def get_2d_projection(self, face):
        """
        this works by rotating the face such that the face normal vector matches the z-axis
        source https://math.stackexchange.com/questions/1956699/getting-a-transformation-matrix-from-a-normal-vector
        """
        nx, ny, nz = self.normal(face)
        if nx == 0.0 and ny == 0.0:
            # nothing to do: points are already parallel to xy-plane: matrix just gets rid of z-values
            return np.array([
                [1,0,0],
                [0,1,0]
            ])
        else:
            return np.array([
                [ny / sqrt(nx**2+ny**2), -nx / sqrt(nx**2 + ny**2), 0],
                [nx * ny / sqrt(nx**2+ny**2), ny * nz / sqrt(nx**2 + ny**2), -sqrt(nx**2 + ny**2)]
            ])

    def face_from_idx(self, id):
        return list(self.faces())[id]

    def get_adjacent_faces_idx(self, id):
        return [f.idx() for f in self.get_adjacent_faces(self.face_from_idx(id))]

    #TODO: let students implement that 
    def face_size(self, face):
        points = lambda mesh, face : [mesh.point(vertex_handle) for vertex_handle in mesh.get_vertecies_by_face(mesh.face_handle(face))]
        vec_3d = lambda p1, p2 : (p1[0] - p2[0], p1[1] - p2[1], p1[2] - p2[2])
        vec_len = lambda v : sqrt(v[0]**2 + v[1]**2 + v[2]**2)

        f_p = points(self, face)
        vec1 = vec_3d(f_p[0], f_p[1])
        vec2 = vec_3d(f_p[2], f_p[3])
        return vec_len(vec1) * vec_len(vec2)

class node:
    def __init__(self, val):
        self.children = []
        self.val = val

    def get_children(self):
        return self.children

    def add_child(self, val):
        self.children.append(node(val))

    def add_child_node(self, child_node):
        self.children.append(child_node)

    def has(self, val):
        def iter(root, val):
            if root.val == val:
                return True
            for c in root.children:
                if iter(c, val):
                    return True
            return False
        return iter(self, val)

    def dump_tree(self, dist = 0):
        print(dist * '\t', self.val)
        for c in self.get_children():
            c.dump_tree(dist + 1)

    def rem_child(self, val):
        self.children = [c for c in self.children if c.val != val]
        for c in self.get_children():
            c.rem_child(val)

    def rem_child_node(self, child_node):
        self.children = [c for c in self.children if c.val != child_node.val]
        for c in self.get_children():
            c.rem_child_node(child_node)

    def insert_child(self, parent, child):
        if self.val == parent:
            self.add_child_node(child)
            return
        for c in self.get_children():
            c.insert_child(parent, child)


    def unfold(self, mesh):
        def get_rotation_matrix(axis, angle):
            from scipy.spatial.transform import Rotation
            return Rotation.from_rotvec(axis/np.linalg.norm(axis) * angle).as_matrix()

        polygons = []

        def get_unfolding_coordinate_mapping(node, parent):
            crease_halfedge = mesh.get_edge_between_faces(mesh.face_handle(node.val),  mesh.face_handle(parent.val))
            crease_angle = mesh.calc_dihedral_angle(crease_halfedge)
            crease_vector = mesh.calc_edge_vector(crease_halfedge)


            # offset is introduces, since rotation is not occuring around the origin but along crease line
            offset = mesh.point(mesh.from_vertex_handle(crease_halfedge))

            return lambda points: get_rotation_matrix(crease_vector, crease_angle).dot(points - offset) + offset

        def unfolder_recursive_call(node, mapping_fn):
            polygon_3d = [mesh.point(vertex_handle) for vertex_handle in mesh.get_vertecies_by_face(mesh.face_handle(node.val))]
            # apply previous coordinate mapping function and store polygon
            polygons.append([mapping_fn(point_3d) for point_3d in polygon_3d])

            for child in node.get_children():
                # add a new mapping function to the 'stack' that maps the child's coordinate system to the parent's
                # coordinate system. (which in turn will be handed of to the rest of the function stack to end up with the final coordinates)
                new_mapping = get_unfolding_coordinate_mapping(child, node)
                unfolder_recursive_call(child, lambda points: mapping_fn(new_mapping(points)))

        map_to_2d = lambda points: mesh.get_2d_projection(mesh.face_handle(self.val)).dot(points)
        unfolder_recursive_call(self, map_to_2d)

        return polygons

def is_unfolding_overlapping(polygons):

    def is_polygon_overlapping_fixed(polygon_a, polygon_b):
        from shapely.geometry import Polygon
        return Polygon(polygon_a).intersection(Polygon(polygon_b)).area

    for i in range(len(polygons)):
        for j in range(i+1, len(polygons)):
            if is_polygon_overlapping_fixed(polygons[i], polygons[j]) != 0:
                return True
    return False

def draw_svg(polygons, file):
    scale = 1000
    d = draw.Drawing(scale, scale, origin='center', displayInline=False)
    for polygon in polygons:
        # polygon = [coords[0:2] for coords in polygon]
        d.append(draw.Lines(*np.array(polygon).flatten()*50, close=True, fill='#eeee00', stroke='#000', stroke_width=.1))
    d.saveSvg(file)
