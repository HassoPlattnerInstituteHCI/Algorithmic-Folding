import networkx as nx
from sympy import Point
from itertools import count
from enum import Enum

EdgeType = Enum('EdgeType','STRUT BAR CABLE')
DIM = 2
# if config is True it returns the configuration, otherwise just a graph, bracing adds a diagonal
def parallel_4_bar(braced=False):
    graph = nx.Graph()
    set_id=count()
    p1 = Point(0,0) 
    p2 = Point(5,0) 
    p3 = Point(5,5) 
    p4 = Point(0,5) 
    graph.add_edges_from([(p1,p2),(p2,p3),(p3,p4),(p4,p1)])
    if braced: graph.add_edge(p4,p2)
    graph.set_edge_attributes(graph,)
    return graph

# if config is True it returns the configuration, otherwise just a graph, bracing adds a bracing bar
def jansen_walker(braced=False):
    graph = nx.Graph()
    set_id=count()
    p1 = Point(0,6)
    p2 = Point(4,10)
    p3 = Point(9,6)
    p4 = Point(5,3)
    p5 = Point(3,0)
    p6 = Point(1,3)
    p7 = Point(4,6)
    graph.add_edges_from([(p1,p2),(p2,p3),(p3,p4),(p4,p5),(p5,p6),(p6,p1),(p6,p4),(p4,p7),(p1,p7),(p2,p7)])
    if braced: graph.add_edge(p7,p3)
    return graph