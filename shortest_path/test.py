from shapely.geometry import *

class Vector:
    def __init__(self, x, y):
        self.x = x
        self.y = y

def cut_edges(start, end, lines):
    main_line = LineString([start, end])
    return [main_line.intersection(l) for l in lines]

edges = [
        ((1, -2), (3, 3)), 
        ((3, 1), (3, -2)),
        ((3, -2), (5, 2))
        ]

edges = [LineString(e) for e in edges]
points = cut_edges((1, 1), (4, -2), edges)
print(points)
