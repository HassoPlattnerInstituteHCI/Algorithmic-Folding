import openmesh as om

#loading stuff
# mesh = om.read_trimesh('cube.stl')
# mesh.update_normals()
mesh_obj = om.read_mesh(self.mesh, 'cube.obj', Null)
mesh_obj.update_normals()


print("\nobj:")
for v in mesh_obj.faces():
    print(v.idx())
