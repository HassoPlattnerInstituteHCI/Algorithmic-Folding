import random
import string
import linkage_geometry

from pyslvs import VPoint, VJoint, VLink
from enum import Enum


class Point:
    def __init__(self, x, y):
        self.x = x
        self.y = y


def rhombus_link(points):
    links = []
    link_amount = 4
    counter = 0
    while counter < link_amount:
        links.append(VLink(generate_name("Link", counter), "blue", list()))
        counter += 1
    points[0]['vpoint'].set_links(['ground', links[0].name, links[3].name])
    points[3]['vpoint'].set_links([links[3].name, links[2].name])
    points[1]['vpoint'].set_links([links[0].name, links[1].name])
    points[2]['vpoint'].set_links([links[1].name, links[2].name])
    return links


def counter_parallelogram_link(points):
    links = []
    link_amount = 4
    counter = 0
    while counter < link_amount:
        links.append(VLink(generate_name("Link", counter), "blue", list()))
        counter += 1
    points[0]['vpoint'].set_links(['ground', links[0].name, links[2].name])
    points[3]['vpoint'].set_links([links[3].name, links[1].name])
    points[1]['vpoint'].set_links([links[0].name, links[1].name])
    points[2]['vpoint'].set_links([links[3].name, links[2].name])
    return links


# required to avoid collisions, increase K for more options -> 26^k
def generate_name(base, counter):
    return base + "_" + ''.join(random.choices(string.ascii_uppercase + string.digits, k=3)) + "_" + str(counter)


# example: VPoint(('ground', 'L1'), 0, 0.0, [[-67.38, 36.13], [0.0, 0.0]])
def point_to_vpoint(point, *args):
    """ Return a valid vpoint object that can be used to greate mechanism expressions later on """
    # todo: differentiate between R / P / RP Joints
    color = "BLUE"
    if len(args) > 0:
        color = args[0]
    point = VPoint('', VJoint.R, 0, color, point.x, point.y)
    print()
    return point


def vpoint_to_detailed_point(name, vpoint, role):
    return {'name': name, 'vpoint': vpoint, 'role': role}


def geometry_to_expression(geo):
    """ Return a valid Mechanism-Expression based on the VPoints in a Geometry object
    that can then be imported into pyslvs """
    expression = "M["
    expression += dp_to_expression(geo.points)
    expression += "\n]"
    return expression


def geometries_to_expression(geos):
    """ Return a valid Mechanism-Expression based on the VPoints in multiple Geometry objects
        that can then be imported into pyslvs """
    expression = "M["
    for geo in geos:
        expression += dp_to_expression(geo.points)
    expression += "\n]"
    return expression


def global_to_expression():
    global_points = linkage_geometry.get_global_points()
    expression = "M["
    for dp in global_points:
        vp = global_points[dp].vPoint
        expression += "\n" + vp.expr() + ","
    expression += "\n]"
    return expression


def dp_to_expression(dps):
    local_expression = ""
    global_points = linkage_geometry.get_global_points()
    for dp in dps:
        vp = global_points[dp].vPoint
        local_expression += "\n" + vp.expr() + ","
    return local_expression


def combine_link(*args):
    new_link = VLink(generate_name("Link", 8), "blue", list())
    for point in args:
        vpoint = point["vpoint"]
        links = vpoint.links
        new_links = links + (new_link.name,)
        vpoint.set_links(new_links)
# todo: delete previous sub-links that 2+ of the points to combine had in common


class Role(Enum):
    INPUT_ALPHA = "INPUT_ALPHA"
    INPUT_BETA = "INPUT_BETA"
    OUTPUT = "OUTPUT"
    INPUT = "INPUT"
