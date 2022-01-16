import networkx as nx
from sympy import Point
from itertools import count

DIM = 2
# if config is True it returns the configuration, otherwise just a graph, bracing adds a diagonal
def parallel_4_bar(config=False,braced=False):
    graph = nx.Graph()
    set_id=count()
    p1 = Point(0,0) if config else next(set_id)
    p2 = Point(5,0) if config else next(set_id)
    p3 = Point(5,5) if config else next(set_id)
    p4 = Point(0,5) if config else next(set_id)  
    graph.add_edges_from([(p1,p2),(p2,p3),(p3,p4),(p4,p1)])
    if braced: graph.add_edge(p4,p2)
    return graph

# if config is True it returns the configuration, otherwise just a graph, bracing adds a bracing bar
def jansen_walker(config=False,braced=False):
    graph = nx.Graph()
    set_id=count()
    p1 = Point(0,6) if config else next(set_id)
    p2 = Point(4,10) if config else next(set_id)
    p3 = Point(9,6) if config else next(set_id)
    p4 = Point(5,3) if config else next(set_id)
    p5 = Point(3,0) if config else next(set_id)
    p6 = Point(1,3) if config else next(set_id)
    p7 = Point(4,6) if config else next(set_id)
    graph.add_edges_from([(p1,p2),(p2,p3),(p3,p4),(p4,p5),(p5,p6),(p6,p1),(p6,p4),(p4,p7),(p1,p7),(p2,p7)])
    if braced: graph.add_edge(p7,p3)
    return graph

