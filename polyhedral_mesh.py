import igl
import numpy as np
import networkx as nx
import matplotlib.pyplot as plt

# prefixes:
#   - everything with tri_ is something related to triangular
#   - everything with ply_ is something related to polyhedral
normal_vec_equal = lambda a, b: np.linalg.norm(np.cross(a, b)) < 1e-5

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

def strip_unfold(faces, adjacency, normals):
    unfolding = nx.Graph()

    # find suitable strip faces
    all_faces = list(range(len(faces)))

    ignored_normals = [
        np.array([1, 0, 0]),
        np.array([0, 1, 0]),
        np.array([0, 0, 1]),
    ]
    
    # create a filter prefix for all ignored normals
    filter_prefixs = [lambda f: not normal_vec_equal(normals[f], i) for i in
        ignored_normals]

    # find best strip
    # strip faces are all_faces except with certain normal
    # create all possible strips by filtering all_faces to all filter prefixs
    pos_strips = [set(filter(prefix, all_faces)) for prefix in filter_prefixs]

    # use the best (= longest) strip in further code
    strip_faces = max(pos_strips, key=lambda a: len(a))

    # wing faces are all_faces which are not wing_faces
    wing_faces = set(all_faces).difference(strip_faces)

    # build a 'circle' out of the strip_faces
    for f in strip_faces:
        for adj_f in adjacency[f]:
            if adj_f in strip_faces:
                unfolding.add_edge(f, adj_f)

    # minimum_spanning_tree will disconnect one of the connections and build a
    # strip
    unfolding = nx.minimum_spanning_tree(unfolding)

    unattachable = []
    # add wing faces on first adjacency found
    for f in wing_faces:
        adj_faces = set(adjacency[f])
        # all possible positions where the face could attach 
        pos_positions = list(adj_faces.intersection(strip_faces))

        # check if wing is connectable
        if len(pos_positions) == 0:
            # if not add it to unattachable array to build subassembly later
            unattachable.append(f)
        else:
            # if so add_edge to unfolding itsef and to the strip so later a wing
            # could be attached to the newly attached wing f

            # currently choosing just the first found position to attach the
            # wing, here optimizations could be done
            unfolding.add_edge(f, pos_positions[0])
            strip_faces.add(f)


    return unfolding, unattachable

unfolding, _ = strip_unfold(faces, adjacency, normals)
print(unfolding.edges())
nx.draw(unfolding)
plt.show()
