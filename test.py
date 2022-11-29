import igl
import numpy as np

class Mesh:

    def __get_face_normals(self):
        map = {}
        for face_id, face in enumerate(self.igl_f):
            a = self.igl_v[face[1]] - self.igl_v[face[0]]
            b = self.igl_v[face[2]] - self.igl_v[face[0]]
            normal = np.cross(a, b)
            normal = normal / np.linalg.norm(normal)
            map[face_id] = normal

        return map;

    def __build_polyhedral_mesh(self):
        normals = self.__get_face_normals()
        normals_polyhedral = []
        seen = set()
        adj = []
        faces = [[]]
        adjacend_faces, _ = igl.triangle_triangle_adjacency(self.igl_f)
        face_mapping = {}

        normal_vec_equal = lambda a, b: np.array_equal(a, b) or np.array_equal(a * -1, b)

        def help(curr_igl_face, faces_index, normal_vec):
            face_mapping[curr_igl_face] = faces_index

            if faces_index >= len(normals_polyhedral):
                normals_polyhedral.append(normal_vec)

            faces[faces_index].append(curr_igl_face)
            seen.add(curr_igl_face)

            for f in adjacend_faces[curr_igl_face]:
                if f in seen:
                    continue
                if normal_vec_equal(normals[f], normals[curr_igl_face]):
                    help(f, faces_index, None)

            for f in adjacend_faces[curr_igl_face]:
                if f in seen:
                    continue
                if not normal_vec_equal(normals[f], normals[curr_igl_face]):
                    faces.append([])
                    help(f, len(faces) - 1, normals[f])

        help(0, 0, normals[0])
        for igl_f, af in enumerate(adjacend_faces):
            for f in af:
                adj.append((face_mapping[igl_f], face_mapping[f]))

        # TODO: remove duplicates in adj
            
        return faces, adj, normals_polyhedral

    def __init__(self, path):
        self.igl_v, self.igl_f = igl.read_triangle_mesh("./cube.stl")
        self.igl_v, self.igl_f, _ = igl.remove_duplicates(self.igl_v, self.igl_f, 0.00001)
        self.v = self.igl_v
        self.faces, self.adjacend_faces, self.normals = self.__build_polyhedral_mesh()

        print("Faces")
        print(self.faces)
        print("Adjecend Faces")
        print(self.adjacend_faces)
        print("NOrmals")
        print(self.normals)

    def draw(self):
        print("v")
        print(self.v)
        print("f")
        print(self.f)


m = Mesh("cube.stl")
# m.draw()
