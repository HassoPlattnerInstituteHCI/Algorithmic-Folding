import random
import string
from pyslvs import VPoint, VJoint
from model_export.linkage import Linkage
from model_export.node import Node

# global var for storing vpoints in a dictionary with the node obj id as their key
all_points = {}


def extract_shared_links(links):
    """ Extracts common elements that exist at least twice in the links collection """
    lists = list()
    for element in links:
        for entry in element:
            entry_list = list(entry)
            lists.append(set(entry_list))
    return set.intersection(*lists)


def link(node_a, node_b):
    """ Link two nodes, avoiding the creation of new links if nodes already share a common link """
    vpoint_a = all_points[id(node_a)]
    vpoint_b = all_points[id(node_b)]
    existing_links = [[vpoint_a.links], [vpoint_b.links]]
    shared_links = extract_shared_links(existing_links)
    if len(shared_links) == 0:
        link_name = "Link_" + ''.join(random.choices(string.ascii_uppercase + string.digits, k=8))
        vpoint_a.set_links(vpoint_a.links + (link_name,))
        vpoint_b.set_links(vpoint_b.links + (link_name,))
    else:
        #print("[DEBUG] Nodes already share a common link")
        pass


def convert_geometry_to_mechanism(geometry):
    """ Convert a custom geometry to a mechanism expression that can be imported in pyslvs """
    __all_links = []
    for element in geometry.get_geometry():
        if isinstance(element, Node):
            if element.is_fixed():
                all_points[id(element)] = VPoint(['ground'], VJoint.R, 0, "BLUE", element.get_x(), element.get_y())
            else:
                all_points[id(element)] = VPoint('', VJoint.R, 0, "BLUE", element.get_x(), element.get_y())
        elif isinstance(element, Linkage):
            __all_links.append(element)
    for current_link in __all_links:
        node_a, node_b = current_link.get_nodes()
        link(node_a, node_b)
    expression = "M["
    for entry in all_points:
        expression += "\n" + all_points[entry].expr() + ","
    expression += "\n]"
    return expression
