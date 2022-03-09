import random
import string

from pyslvs import VPoint, VJoint, VLink
from conversion.src.utils import *


class DetailedPoint:
    def __init__(self, x, y):
        self.vPoint = point_to_vpoint(Point(x, y))
        self.role = []

    def add_link(self, link_name):
        self.vPoint.set_links(self.vPoint.links + (link_name,))


class Point:
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def __hash__(self):
        return hash((self.x, self.y))

    def __eq__(self, other):
        return (self.x, self.y) == (other.x, other.y)

    def __ne__(self, other):
        return not(self == other)

    def __str__(self):
        return "Point[" + str(self.x) + "|" + str(self.y) + "]"


# this dict is used to avoid duplicate point creation
global_points = {Point(0, 0): DetailedPoint(0, 0), }


class Rhombus:
    # store points for later reference to global_points
    points = []

    def __init__(self, o, b, a, d):
        self.points = []
        create_new_points(o, b, a, d)

        # link points based on rhombus rules
        link(o, b)
        link(a, b)
        link(a, d)
        link(d, o)
        self.points.extend([o, b, a, d])


# todo: fix input roles for usage in multiplicator & additor
class CounterParallelogram:
    # store points for later reference to global_points
    points = []

    def __init__(self, o, i, p, b):
        # for some reason python requires this clearing of self data even though it should belong to the instance anyway
        self.points = []
        create_new_points(o, i, p, b)
        link(o, i)
        link(i, b)
        link(b, p)
        link(p, o)
        self.points.extend([o, b, i, p])


class Multiplicator:
    geos = []

    def __init__(self, p1, p2, p3, p4, p5, p6):
        self.geos = []
        create_new_points(p1, p2, p3, p4, p5, p6)
        cp1 = CounterParallelogram(p1, p2, p3, p4)
        self.geos.append(cp1)
        cp2 = CounterParallelogram(p1, p5, p2, p6)
        self.geos.append(cp2)
        link(p2, p4, p6)


# todo: fix point re-use & check why links are so extremely weird (had to remove 5 points and 3 links)
# todo: change to new structure
class Additor:
    geos = []

    def __init__(self, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11):
        self.geos = []
        create_new_points(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11)
        multi1 = Multiplicator(p1, p2, p3, p4, p5, p6)
        self.geos.append(multi1)
        multi2 = Multiplicator(p1, p8, p7, p9, p10, p11)
        self.geos.append(multi2)
        # link(p1, p3, p11)


def create_new_points(*args):
    """Adds only new points to global_points dict for later reference"""
    for point in args:
        if point not in global_points:
            # print("[DEBUG] Did not detect " + str(point) + " in global points")
            global_points[point] = DetailedPoint(point.x, point.y)


def link(*args):
    """Links Points if they don't already share a link"""
    existing_links = []
    for point in args:
        if isinstance(point, Point):
            existing_links.append([global_points[point].vPoint.links])
        else:
            print("[Error] Type of point in link method could not be determined")
    # feel free to improve this, I just suck at dealing with types in python :)
    shared_links = extract_shared_links(existing_links)
    if len(shared_links) == 0:
        link_name = generate_name("Link", int(random.random() * 100))
        for point in args:
            global_points[point].add_link(link_name)
    else:
        print("[Info] Points already all share a common link: " + shared_links.__str__())


# ignore please
# def import_test(amount):
#     vpoints_list = []
#     i = 0
#     while i < amount:
#         vpoints_list.append(point_to_vpoint(Point(random.random(), random.random())))
#         i += 1
#     expression = "M["
#     for p in vpoints_list:
#         expression += "\n" + p.expr() + ","
#     expression += "\n]"
#     print(expression)
def extract_shared_links(list_of_links):
    """This is just a helper function because handling tuples is annoying."""
    lists = list()
    for element in list_of_links:
        for entry in element:
            entry_list = list(entry)
            lists.append(set(entry_list))
    return set.intersection(*lists)


def get_global_points():
    return global_points
