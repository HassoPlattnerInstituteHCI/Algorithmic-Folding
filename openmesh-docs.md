

# unofficial OpenMesh docs
Lukas Rambold

OpenMesh is cool because it handles TriMeshes and PolyMeshes (polygon-based meshes) in the same API. It's implemented in C++ and builds on Numpy. It seems to be built for research purposes and therefore lacks a nice documentation for its python wrapper (the [C++ docs](https://www.graphics.rwth-aachen.de/media/openmesh_static/Documentations/OpenMesh-6.1-Documentation/index.html) are good, tho and share the same concepts), so here is quick overview:

## The data structure: HalfEdgeMesh
*(taken from here: https://www.graphics.rwth-aachen.de/media/openmesh_static/Documentations/OpenMesh-6.1-Documentation/a00016.html)*

![](./halfedge_structure.png)

- Each vertex references one outgoing halfedge, i.e. a halfedge that starts at this vertex (1).
- Each face references one of the halfedges bounding it (2).
- Each halfedge provides a handle to
	+ the vertex it points to (3),
	+ the face it belongs to (4)
	+ the next halfedge inside the face (ordered counter-clockwise) (5),
	+ the opposite halfedge (6),
	+ (optionally: the previous halfedge in the face (7)).


## Importing a mesh
```py
mesh = om.read_trimesh('cube1.ply')
mesh.update_normals()
```
it supports .off, .obj, .stl and .om (and apperently .ply).

## Introducing: handles
Handles are pointers from python into the C++ implementatipon of openmesh. 
there are handles for every pieces of data in the halfedgemesh: 
- `FaceHandle`
- `HalfEdgeHandle`
- `EdgeHandle`
- `VertexHandle`

every of these types has incrementing ids for a given mesh. 

One can get the id by calling .idx() on the handle.
eg. `some_vertex_id = vertex_handle.idx()`.

To get back from the id to the handle instance, you can use the main function for the specific type on the mesh instance: 

```py
original_vertex_handle = mesh.vertex_handle(some_vertex_id)
```


## getting around the halfedgemesh datastructure
Where things are uniquly defined, the main getter function for that type of datum works. 
eg. `mesh.face_handle(halfEdgeHandle)` gives you the face that is linked to the specific HalfEdge.

There are special functions for some cases, like `mesh.opposite_halfedge_handle(halfEdgeHandle)` or `opposite_face_handle()`.

Also you might find things in the list below.

## Iterating over the Mesh

Now that we have added a couple of vertices to the mesh, we can iterate over them and print out their indices:
```py
for vh in mesh.vertices():
    print vh.idx()
```

We can also iterate over halfedges, edges and faces by calling mesh.halfedges(), mesh.edges() and mesh.faces() respectively:
```py
# iterate over all halfedges
for heh in mesh.halfedges():
    print heh.idx()
# iterate over all edges
for eh in mesh.edges():
    print eh.idx()
# iterate over all faces
for fh in mesh.faces():
    print fh.idx()
```
To iterate over the items adjacent to another item we can use one of the circulator functions. For example, to iterate over the vertices adjacent to another vertex we can call mesh.vv() and pass the handle of the center vertex:

```py
for vh in mesh.vv(vh1):
    print vh.idx()
```

We can also iterate over the adjacent halfedges, edges and faces:
```py
# iterate over all incoming halfedges
for heh in mesh.vih(vh1):
    print heh.idx()
# iterate over all outgoing halfedges
for heh in mesh.voh(vh1):
    print heh.idx()
# iterate over all adjacent edges
for eh in mesh.ve(vh1):
    print eh.idx()
# iterate over all adjacent faces
for fh in mesh.vf(vh1):
    print fh.idx()
```
To iterate over the items adjacent to a face we can use the following functions:
```py
# iterate over the face's vertices
for vh in mesh.fv(fh0):
    print vh.idx()
# iterate over the face's halfedges
for heh in mesh.fh(fh0):
    print heh.idx()
# iterate over the face's edges
for eh in mesh.fe(fh0):
    print eh.idx()
# iterate over all edge-neighboring faces
for fh in mesh.ff(fh0):
    print fh.idx()
```

## Dealing with Normals and Points
`mesh.point(vertexHandle)` gives you the coordinate as a numpy array.

`mesh.points()` gets you all the coordinates.

`mesh.normal(faceHandle)` gives you the face normal. Be sure to have called `mesh.update_normals()` if you alterd or imported the mesh before.

More metrics can be obtained using these functions: 
-`mesh.calc_dihedral_angle`
-`mesh.calc_dihedral_angle_fast`
-`mesh.calc_edge_length`
-`mesh.calc_edge_sqr_length`
-`mesh.calc_edge_vector`
-`mesh.calc_face_centroid`
-`mesh.calc_face_normal`
-`mesh.calc_halfedge_normal`
-`mesh.calc_sector_angle`
-`mesh.calc_sector_area`
-`mesh.calc_sector_normal`
-`mesh.calc_sector_vectors`
-`mesh.calc_vertex_normal`
-`mesh.calc_vertex_normal_correct`
-`mesh.calc_vertex_normal_fast`
-`mesh.calc_vertex_normal_loop`

## Properties
one can attach data to all types. For the FaceHandler, it looks like this:
```py
# getter
mesh.face_property("visited", face)
# setter
mesh.set_face_property("visited", face, True)
```

## For Completness, Here Is a Full List of the Functions that a Mesh Offers
```
mesh.add_face
mesh.add_faces
mesh.add_vertex
mesh.add_vertices
mesh.adjust_outgoing_halfedge
mesh.assign_connectivity
mesh.calc_dihedral_angle
mesh.calc_dihedral_angle_fast
mesh.calc_edge_length
mesh.calc_edge_sqr_length
mesh.calc_edge_vector
mesh.calc_face_centroid
mesh.calc_face_normal
mesh.calc_halfedge_normal
mesh.calc_sector_angle
mesh.calc_sector_area
mesh.calc_sector_normal
mesh.calc_sector_vectors
mesh.calc_vertex_normal
mesh.calc_vertex_normal_correct
mesh.calc_vertex_normal_fast
mesh.calc_vertex_normal_loop
mesh.ccw_rotated_halfedge_handle
mesh.clean
mesh.clear
mesh.collapse
mesh.color
mesh.copy_all_properties
mesh.copy_property
mesh.cw_rotated_halfedge_handle
mesh.delete_edge
mesh.delete_face
mesh.delete_isolated_vertices
mesh.delete_vertex
mesh.edge_colors
mesh.edge_face_indices
mesh.edge_halfedge_indices
mesh.edge_handle
mesh.edge_property
mesh.edge_property_array
mesh.edge_vertex_indices
mesh.edges
mesh.edges_empty
mesh.ef_indices
mesh.eh_indices
mesh.ev_indices
mesh.face_colors
mesh.face_edge_indices
mesh.face_face_indices
mesh.face_halfedge_indices
mesh.face_handle
mesh.face_normals
mesh.face_property
mesh.face_property_array
mesh.face_vertex_indices
mesh.faces
mesh.faces_empty
mesh.fe
mesh.fe_indices
mesh.ff
mesh.ff_indices
mesh.fh
mesh.fh_indices
mesh.find_feature_edges
mesh.find_halfedge
mesh.flip
mesh.from_vertex_handle
mesh.fv
mesh.fv_indices
mesh.garbage_collection
mesh.halfedge_colors
mesh.halfedge_edge_indices
mesh.halfedge_face_indices
mesh.halfedge_from_vertex_indices
mesh.halfedge_handle
mesh.halfedge_normals
mesh.halfedge_property
mesh.halfedge_property_array
mesh.halfedge_texcoords1D
mesh.halfedge_texcoords2D
mesh.halfedge_texcoords3D
mesh.halfedge_to_vertex_indices
mesh.halfedge_vertex_indices
mesh.halfedges
mesh.halfedges_empty
mesh.has_edge_colors
mesh.has_edge_property
mesh.has_face_colors
mesh.has_face_normals
mesh.has_face_property
mesh.has_face_texture_index
mesh.has_halfedge_colors
mesh.has_halfedge_normals
mesh.has_halfedge_property
mesh.has_halfedge_texcoords1D
mesh.has_halfedge_texcoords2D
mesh.has_halfedge_texcoords3D
mesh.has_vertex_colors
mesh.has_vertex_normals
mesh.has_vertex_property
mesh.has_vertex_texcoords1D
mesh.has_vertex_texcoords2D
mesh.has_vertex_texcoords3D
mesh.he_indices
mesh.hf_indices
mesh.hfv_indices
mesh.hl
mesh.htv_indices
mesh.hv_indices
mesh.is_boundary
mesh.is_collapse_ok
mesh.is_deleted
mesh.is_estimated_feature_edge
mesh.is_flip_ok
mesh.is_manifold
mesh.is_polymesh
mesh.is_simple_link
mesh.is_simply_connected
mesh.is_triangles
mesh.is_trimesh
mesh.is_valid_handle
mesh.n_edges
mesh.n_faces
mesh.n_halfedges
mesh.n_vertices
mesh.new_edge
mesh.new_face
mesh.new_vertex
mesh.next_halfedge_handle
mesh.normal
mesh.opposite_face_handle
mesh.opposite_halfedge_handle
mesh.opposite_he_opposite_vh
mesh.opposite_vh
mesh.point
mesh.points
mesh.prev_halfedge_handle
mesh.reinsert_edge
mesh.release_edge_colors
mesh.release_face_colors
mesh.release_face_normals
mesh.release_face_texture_index
mesh.release_halfedge_colors
mesh.release_halfedge_normals
mesh.release_halfedge_texcoords1D
mesh.release_halfedge_texcoords2D
mesh.release_halfedge_texcoords3D
mesh.release_vertex_colors
mesh.release_vertex_normals
mesh.release_vertex_texcoords1D
mesh.release_vertex_texcoords2D
mesh.release_vertex_texcoords3D
mesh.remove_edge
mesh.remove_edge_property
mesh.remove_face_property
mesh.remove_halfedge_property
mesh.remove_vertex_property
mesh.request_edge_colors
mesh.request_face_colors
mesh.request_face_normals
mesh.request_face_texture_index
mesh.request_halfedge_colors
mesh.request_halfedge_normals
mesh.request_halfedge_texcoords1D
mesh.request_halfedge_texcoords2D
mesh.request_halfedge_texcoords3D
mesh.request_vertex_colors
mesh.request_vertex_normals
mesh.request_vertex_texcoords1D
mesh.request_vertex_texcoords2D
mesh.request_vertex_texcoords3D
mesh.reserve
mesh.resize_points
mesh.sedges
mesh.set_color
mesh.set_deleted
mesh.set_edge_property
mesh.set_edge_property_array
mesh.set_face_handle
mesh.set_face_property
mesh.set_face_property_array
mesh.set_halfedge_handle
mesh.set_halfedge_property
mesh.set_halfedge_property_array
mesh.set_next_halfedge_handle
mesh.set_normal
mesh.set_point
mesh.set_texcoord1D
mesh.set_texcoord2D
mesh.set_texcoord3D
mesh.set_texture_index
mesh.set_vertex_handle
mesh.set_vertex_property
mesh.set_vertex_property_array
mesh.sfaces
mesh.shalfedges
mesh.split
mesh.split_copy
mesh.split_edge
mesh.split_edge_copy
mesh.svertices
mesh.texcoord1D
mesh.texcoord2D
mesh.texcoord3D
mesh.texture_index
mesh.texture_name
mesh.to_vertex_handle
mesh.triangulate
mesh.update_face_normals
mesh.update_halfedge_normals
mesh.update_normal
mesh.update_normals
mesh.update_vertex_normals
mesh.valence
mesh.ve
mesh.ve_indices
mesh.vertex_colors
mesh.vertex_edge_indices
mesh.vertex_face_indices
mesh.vertex_handle
mesh.vertex_incoming_halfedge_indices
mesh.vertex_normals
mesh.vertex_outgoing_halfedge_indices
mesh.vertex_property
mesh.vertex_property_array
mesh.vertex_split
mesh.vertex_texcoords1D
mesh.vertex_texcoords2D
mesh.vertex_texcoords3D
mesh.vertex_vertex_indices
mesh.vertices
mesh.vertices_empty
mesh.vf
mesh.vf_indices
mesh.vih
mesh.vih_indices
mesh.voh
mesh.voh_indices
mesh.vv
mesh.vv_indices
```
