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

    def __build_non_triangular_mesh(self):
        normals = self.__get_face_normals()
        seen = set()
        adj = []
        faces = [[]]
        adjacend_faces, _ = igl.triangle_triangle_adjacency(self.igl_f)

        # for k, v in normals.items():
            # print(k, v)
        # print("\n\n")
        # for n in adjacend_faces:
            # print(n)
        # print("\n\n")

        def help(curr_igl_face, faces_index):
            print(curr_igl_face, faces_index)

            faces[faces_index].append(curr_igl_face)
            seen.add(curr_igl_face)

            for f in adjacend_faces[curr_igl_face]:
                if f in seen: 
                    continue
                if np.array_equal(normals[f], normals[curr_igl_face]):
                    # print(curr_igl_face, f, "equal")
                    seen.add(f)
                    faces[faces_index].append(f)
                    

            for f in adjacend_faces[curr_igl_face]:
                if f in seen: 
                    continue
                if np.array_equal(normals[f] * -1, normals[curr_igl_face]):
                    # print(curr_igl_face, f, "equal")
                    help(f, faces_index)
                else:
                    # print(curr_igl_face, f, "unequal")
                    faces.append([])
                    adj.append((faces_index, len(faces) - 1))
                    help(f, len(faces) - 1)

        help(0, 0)
        print(faces)
        print(adjacend_faces)
        return self.igl_v, self.igl_f

    def __init__(self, path):
        self.igl_v, self.igl_f = igl.read_triangle_mesh("./cube.stl")
        # TODO: what is h
        self.igl_v, self.igl_f, h = igl.remove_duplicates(self.igl_v, self.igl_f, 0.00001)
        self.v, self.f = self.__build_non_triangular_mesh()

    def draw(self):
        print("v")
        print(self.v)
        print("f")
        print(self.f)


m = Mesh("cube.stl")
# m.draw()
