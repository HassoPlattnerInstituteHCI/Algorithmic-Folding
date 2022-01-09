import networkx as nx
from itertools import count
from shapely.geometry import Point
SCALE_FACTOR = 100

def tree_distances(tree): return {x[0]:x[1] for x in nx.all_pairs_dijkstra_path_length(tree)}

def otherLizardTree(T=nx.Graph(),node_map={},scale_factor=SCALE_FACTOR):
    set_id=count()
    #TODO: do the actual NLopt for circle packing
    raw_points = Point(0,0.507), Point(1.938,0), Point(3.876,0.507),Point(0,3.507), Point(1.938,4.000), Point(3.876,3.507), Point(1.938,2.000)
    pts = [Point(p.x*scale_factor,p.y*scale_factor) for p in raw_points]
    for p in pts:
        node_map[p.coords[0]] = next(set_id)
        T.add_node(node_map[p.coords[0]], pos=p)
    inner_node1 = next(set_id)
    inner_node2 = next(set_id)
    inner_node3 = next(set_id)   
    T.add_node(inner_node1)
    T.add_node(inner_node2)
    T.add_node(inner_node3)
    set_id = count()
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node2,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node2,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node2,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node3,weight=0.5*scale_factor)
    T.add_edge(inner_node2,inner_node3,weight=0.5*scale_factor)
    T.add_edge(inner_node1,inner_node3,weight=0.5*scale_factor)
    return pts,node_map,T, tree_distances(T)

def lizardTree(T=nx.Graph(),node_map={},scale_factor=SCALE_FACTOR):
    set_id=count()
    #TODO: do the actual NLopt for circle packing
    raw_points = Point(0,0.6), Point(0,3.6), Point(1.908,0),Point(1.908,3), Point(3.816,0.6), Point(3.816,3.600)
    pts = [Point(p.x*scale_factor,p.y*scale_factor) for p in raw_points]
    for p in pts:
        node_map[p.coords[0]] = next(set_id)
        T.add_node(node_map[p.coords[0]], pos=p)
    inner_node1 = next(set_id)
    inner_node2 = next(set_id)    
    T.add_node(inner_node1)
    T.add_node(inner_node2)
    set_id = count()
    T.add_edge(next(set_id),inner_node2,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node2,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node2,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(inner_node1,inner_node2,weight=1*scale_factor)
    return pts,node_map,T, tree_distances(T)

def threeNodesTree(T=nx.Graph(),node_map={}, scale_factor=SCALE_FACTOR):
    set_id=count()
    #TODO: do the actual NLopt for circle packing
    raw_points = Point(2,0), Point(0,0), Point(1,1.74)
    pts = [Point(p.x*scale_factor,p.y*scale_factor) for p in raw_points]
    for p in pts:
        node_map[p.coords[0]] = next(set_id)
        T.add_node(node_map[p.coords[0]], pos=pts)
    inner_node1 = next(set_id)
    set_id = count()
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    return pts,node_map,T, tree_distances(T)

def antennaBeetleTree(T=nx.Graph(),node_map={},scale_factor=SCALE_FACTOR):
    #TODO: do the actual NLopt for circle packing
    set_id=count()
    raw_points = Point(6,2), Point(6,0), Point(4,0), Point(3,2.5), Point(6,6.12), Point(2,0), Point(0,0), Point(0,2), Point(0,6.12)
    pts = [Point(p.x*scale_factor,p.y*scale_factor) for p in raw_points]
    for p in pts:
        node_map[p.coords[0]] = next(set_id)
        T.add_node(node_map[p.coords[0]], pos=pts)
    inner_node1 = next(set_id)
    set_id = count()
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1.69*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=3*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=3*scale_factor)
    return pts,node_map,T, tree_distances(T)

def beetleTree(T=nx.Graph(),node_map={},scale_factor=SCALE_FACTOR):
    set_id=count()
    #TODO: do the actual NLopt for circle packing

    raw_points = Point(9.63,29.26), Point(0,26.56), Point(0,14.56), Point(6.63,0), Point(14.63,11.71), Point(14.63,21.94), Point(14.63,25.94), Point(19.63,29.26), Point(29.26,26.56), Point(29.26,14.56), Point(22.63,0),Point(14.63,29.26)
    pts = [Point(p.x*scale_factor,p.y*scale_factor) for p in raw_points]
    for p in pts:
        node_map[p.coords[0]] = next(set_id)
        T.add_node(node_map[p.coords[0]], pos=p)
    inner_node1 = next(set_id)
    inner_node2 = next(set_id)
    inner_node3 = next(set_id)
    inner_node4 = next(set_id)
    inner_node5 = next(set_id)
    inner_node6 = next(set_id) 
    
    set_id = count()
    T.add_edge(next(set_id),inner_node1,weight=4*scale_factor)
    T.add_edge(next(set_id),inner_node3,weight=4*scale_factor)
    T.add_edge(next(set_id),inner_node5,weight=6*scale_factor)  
    T.add_edge(next(set_id),inner_node6,weight=8*scale_factor)
    T.add_edge(next(set_id),inner_node6,weight=6.2*scale_factor)
    T.add_edge(next(set_id),inner_node4,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node2,weight=1*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=4*scale_factor)
    T.add_edge(next(set_id),inner_node3,weight=4*scale_factor)
    T.add_edge(next(set_id),inner_node5,weight=6*scale_factor)
    T.add_edge(next(set_id),inner_node6,weight=8*scale_factor)
    T.add_edge(next(set_id),inner_node1,weight=1*scale_factor)
    T.add_edge(inner_node5,inner_node6,weight=2*scale_factor) 
    T.add_edge(inner_node3,inner_node4,weight=1*scale_factor)
    T.add_edge(inner_node4,inner_node5,weight=1*scale_factor)
    T.add_edge(inner_node1,inner_node2,weight=1*scale_factor)
    T.add_edge(inner_node2,inner_node3,weight=1*scale_factor)
    return pts,node_map,T, tree_distances(T)