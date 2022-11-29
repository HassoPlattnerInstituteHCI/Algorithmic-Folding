import igl
import numpy as np

# prefixes:
#   - everything with tri_ is something related to triangular
#   - everything with ply_ is something related to polyhedral

def build_polyhedral_mesh(tri_vertices, tri_faces):

    # everything we need later
    tri_normals = igl.per_face_normals(tri_vertices, tri_faces, np.ones((1, 3)))
    tri_seen = set()
    tri_adjacency, _ = igl.triangle_triangle_adjacency(tri_faces)
    ply_adjacency = []
    tri_ply_mapping = {}
    ply_faces = [[]]
    ply_normals = []

    # checks if normals are equal according to certain tolerance 1e-5
    normal_vec_equal = lambda a, b: np.linalg.norm(np.cross(a, b)) < 1e-5


    # weird DFS too actualy detriangulate mesh
    def help(curr_tri_face, ply_faces_index):

        # later needed to create adjacencies efficiently without looping too
        # much
        tri_ply_mapping[curr_tri_face] = ply_faces_index

        # add current tri_face to the ply_faces
        ply_faces[ply_faces_index].append(curr_tri_face)

        # mark it as seen
        tri_seen.add(curr_tri_face)


        # loop over neighbors and check if normals are equal -> if so call again
        # with face otherwise ignore
        for f in tri_adjacency[curr_tri_face]:
            if f in tri_seen:
                continue
            if normal_vec_equal(tri_normals[f], tri_normals[curr_tri_face]):
                help(f, ply_faces_index)

        # loop over neighbors again all ignored but normals which are not equal
        # -> recurse for new face in ply_faces
        for f in tri_adjacency[curr_tri_face]:
            if f in tri_seen:
                continue
            ply_faces.append([])
            help(f, len(ply_faces) - 1)

        # they cannot be in the same loop otherwise some tri_faces end up in
        # wrong ply_faces bcs not equals will create new and then another new
        # for the face

    # start recusion
    help(0, 0)

    # create tuple list of ply_face adjacency:
    #   [(ply_face_a, ply_face_b), (ply_face_a, ply_face_c), ...]
    ply_adjacency_helper = []
    for a, adj_tri_faces in enumerate(tri_adjacency):
        for b in adj_tri_faces:
            ply_adjacency_helper.append((tri_ply_mapping[a], tri_ply_mapping[b]))

    # remove duplicates and self references in said tuple adjacency list
    ply_adjacency_helper = list(set(ply_adjacency_helper))
    ply_adjacency_helper = [t for t in ply_adjacency_helper if t[0] != t[1]]

    # loop over ply_face_ids as i
    for i in range(len(ply_faces)):

        # append normals on the way (loop over ply_face_ids anyway)
        ply_normals.append(tri_normals[ply_faces[i][0]])

        ply_adjacency.append([])

        # get every tuple with tuple[0] == i and add to real adjacency list
        for a in ply_adjacency_helper:
            if a[0] == i:
                ply_adjacency[i].append(a[1])
        
    return ply_faces, ply_adjacency, ply_normals
    

igl_v, igl_f = igl.read_triangle_mesh("./cube.stl")
igl_v, igl_f, _ = igl.remove_duplicates(igl_v, igl_f, 0.00001)

faces, adjacency, normals = build_polyhedral_mesh(igl_v, igl_f)

print("Faces")
print(faces)
print("Adjacency")
print(adjacency)
print("Normals")
print(normals)
