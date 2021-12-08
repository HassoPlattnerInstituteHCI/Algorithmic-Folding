import networkx as nx
import openmesh as om

if __name__ == "__main__":
    mesh = om.read_polymesh('cube.obj')
    mesh.update_normals()

    faces = mesh.faces()
    Graph = nx.Graph()

    for f in faces:
        for af in mesh.ff(f):
            edge = (f.idx(), af.idx())
            if not Graph.has_edge(*edge):
                Graph.add_edge(*edge)

    print(Graph)

    Graph = nx.algorithms.minimum_spanning_tree(Graph, algorithm='kruskal', weight='weight', ignore_nan=False)

    print(Graph)

    print('Hello World')
