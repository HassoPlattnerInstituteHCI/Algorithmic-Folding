from model_export.geometry import Geometry
from model_export.linkage import Linkage
from model_export.node import Node
from model_export.geometry import Geometry
import math

class Model:

    def __init__(self, scale_factor=256, initial_angles={"alpha": 1/8*math.pi, "beta": 3/8*math.pi}) -> None:
        self.__scale_factor = scale_factor
        self.__initial_angles = initial_angles
        self.__all_geometry: list(Geometry) = []
        self.__maximum_multiplicator= {"alpha": 1, "beta": 1}
        self.__minimum_multiplicator= {"alpha": 1, "beta": 1}
        a, b, c, d = self.__make_rhombus_nodes()
        self.__make_rhombus_linkages(a,b,c,d)
        self.__add_initial_counterparallelograms()

    def __pythagoras2(self, a: float, b: float) -> float:
        return math.sqrt(a**2 + b**2)

    def __make_rhombus_nodes(self):
        a = Node(["rhombus", "origin"], True, (0,0))
        b_x, b_y = self.__get_x_y_for_angle_and_length(self.__initial_angles["alpha"], self.__scale_factor)
        c_x, c_y = self.__get_x_y_for_angle_and_length(self.__initial_angles["beta"], self.__scale_factor)
        b = Node(["rhombus", "alpha", "1"], False, (b_x, b_y))
        c = Node(["rhombus", "beta", "1"], False, (c_x, c_y))
        d = Node(["rhombus", "result"], False, (b_x + c_x, b_y + c_y))
        self.__all_geometry.extend([a,b,c,d])
        self.__origin = a
        self.__alpha_node = b
        self.__beta_node = c
        return a,b,c,d

    def __calculate_position_of_lower_multiplicator_node(self, angle, short_edge_length) -> tuple[float, float]:
        x_angle, y_angle = self.__calculate_other_multiplicator_node_angles(angle, short_edge_length)
        return (2 * short_edge_length - short_edge_length * math.sin(x_angle)), -short_edge_length * math.sin(y_angle)

    def __calculate_position_of_upper_multiplicator_node(self, angle, short_edge_length) -> tuple[float, float]:
        x_angle, y_angle = self.__calculate_other_multiplicator_node_angles(angle, short_edge_length)
        return (short_edge_length - 2 * short_edge_length * math.sin(x_angle)), 2 * short_edge_length * math.sin(y_angle)

    def __calculate_other_multiplicator_node_angles(self, angle, short_edge_length) -> tuple[float, float]:
        length_a = math.sqrt(5*short_edge_length**2-4*short_edge_length**2*math.cos(angle))
        upper_angle = math.acos((length_a**2 + short_edge_length**2 - (2*short_edge_length)**2)/(2*length_a*short_edge_length))
        y_angle = upper_angle - (math.pi - angle - upper_angle)
        x_angle = 1/2 * math.pi - y_angle
        return x_angle, y_angle

    def __make_rhombus_linkages(self, a: Node, b: Node, c: Node, d: Node) -> None:
        self.__all_geometry.append(Linkage(["rhombus", "alpha", "1", "short"], a, b, self.__scale_factor))
        self.__all_geometry.append(Linkage(["rhombus", "beta", "1", "short"], a, c, self.__scale_factor))
        self.__all_geometry.append(Linkage(["rhombus"], b, d, self.__scale_factor))
        self.__all_geometry.append(Linkage(["rhombus"], c, d, self.__scale_factor))

    def __add_initial_counterparallelograms(self):
        outer = Node(["base"], True, (2 * self.__scale_factor, 0))
        self.__all_geometry.append(Linkage(["base"], self.__origin, outer, self.__scale_factor*2, True))

        for (angle, rhombus_node) in [("alpha", self.__alpha_node), ("beta", self.__beta_node)]:
            x,y = self.__calculate_position_of_lower_multiplicator_node(abs(self.__initial_angles[angle]), self.__scale_factor)
            x = x * (1 if self.__initial_angles[angle] > -math.pi/2 and self.__initial_angles[angle] < math.pi/2 else -1)
            y = y * (1 if self.__initial_angles[angle] > 0 else -1)
            lower = Node([angle], False, (x, y))
            self.__all_geometry.append(Linkage([angle, "1", "long"], rhombus_node, lower, self.__scale_factor*2))
            self.__all_geometry.append(Linkage([angle], outer, lower, self.__scale_factor))
            self.__all_geometry.extend([outer, lower])

    def __add_initial_negative_counterparallelogram(self, angle):
        outer = Node([angle, "0"], True, (1/2 * self.__scale_factor, 0))
        self.__all_geometry.append(Linkage([angle, "0", "short"], self.__origin, outer, self.__scale_factor/2, True))

        node = self.__alpha_node if angle == "alpha" else self.__beta_node
        x,y = self.__calculate_position_of_upper_multiplicator_node(abs(self.__initial_angles[angle]), self.__scale_factor/2)
        x = x * (1 if self.__initial_angles[angle] > -math.pi/2 and self.__initial_angles[angle] < math.pi/2 else -1)
        y = y * (1 if self.__initial_angles[angle] > 0 else -1)
        upper = Node([angle], False, (x, y))
        self.__all_geometry.append(Linkage([angle, "0", "long"], node, upper, self.__scale_factor/2))
        self.__all_geometry.append(Linkage([angle, "0"], outer, upper, self.__scale_factor))
        self.__all_geometry.extend([outer, upper])

    def create_and_get_multiplicator_of_factor(self, factor: int, angle: str) -> Linkage:
        if factor > 0:
            for i in range(self.__maximum_multiplicator[angle] + 1, factor + 1):
                self.__append_multiplicator(i, angle)
                self.__maximum_multiplicator[angle] = i
        else:
            for i in range(self.__minimum_multiplicator[angle] - 1, factor - 1, -1):
                if i == 0:
                    self.__add_initial_negative_counterparallelogram(angle)
                else:
                    self.__append_negative_multiplicator(i, angle)
                self.__minimum_multiplicator[angle] = i

        for geom in self.__all_geometry:
            if geom.has_tag("linkage") and geom.has_tag("short") and geom.has_tag(str(factor)) and geom.has_tag(angle):
                return geom
        
    def __append_multiplicator(self, level: int, angle: str):
        new_length = self.__scale_factor/2**(level-1)
        short, long = self.__get_short_and_long_linkage_of_previous_mulitplicator(level-1, angle)
        handle_node = short.get_nodes()[0] if short.get_nodes()[0] in long.get_nodes() else short.get_nodes()[1]
        distant_node = long.get_nodes()[0] if long.get_nodes()[1] == handle_node else long.get_nodes()[1]
        helper_node = self.__place_helper_node_for_multiplicator([angle], [angle, str(level-1), "helper"], new_length, handle_node, distant_node)
        new_angle = level * self.__initial_angles[angle]
        new_x = math.cos(new_angle) * new_length
        new_y = math.sin(new_angle) * new_length
        new_node = Node([angle, str(level)], False, (new_x, new_y))
        self.__all_geometry.append(new_node)
        self.__all_geometry.append(Linkage([angle, str(level), "short"], self.__origin, new_node, new_length))
        self.__all_geometry.append(Linkage([angle, str(level), "long"], new_node, helper_node, 2 * new_length))

    def __append_negative_multiplicator(self, level: int, angle: str):
        new_length = self.__scale_factor/2**(abs(level))
        short, long = self.__get_short_and_long_linkage_of_previous_mulitplicator(level+1, angle)
        handle_node = short.get_nodes()[0] if short.get_nodes()[0] in long.get_nodes() else short.get_nodes()[1]
        distant_node = long.get_nodes()[0] if long.get_nodes()[1] == handle_node else long.get_nodes()[1]
        helper_node = self.__place_helper_node_for_multiplicator([angle], [angle, str(level-1), "helper"], new_length / 2, handle_node, distant_node)
        new_angle = level * self.__initial_angles[angle]
        new_x = math.cos(new_angle) * new_length / 2
        new_y = math.sin(new_angle) * new_length / 2
        new_node = Node([angle, str(level)], False, (new_x, new_y))
        self.__all_geometry.append(new_node)
        self.__all_geometry.append(Linkage([angle, str(level), "short"], self.__origin, new_node, new_length / 2))
        self.__all_geometry.append(Linkage([angle, str(level), "long"], new_node, helper_node, new_length))

    def __place_helper_node_for_multiplicator(self, node_tags, linkage_tags, new_short_length, handle_node, distant_node):
        helper_x = handle_node.get_x() + (distant_node.get_x() - handle_node.get_x()) / 4
        helper_y = handle_node.get_y() + (distant_node.get_y() - handle_node.get_y()) / 4
        helper_node = Node(node_tags, False, (helper_x, helper_y))
        self.__all_geometry.append(helper_node)
        self.__all_geometry.append(Linkage(linkage_tags, handle_node, helper_node, new_short_length))
        self.__all_geometry.append(Linkage(linkage_tags, helper_node, distant_node, new_short_length * 3))
        return helper_node

    def __get_short_and_long_linkage_of_previous_mulitplicator(self, level, angle) -> tuple[Linkage, Linkage]:
        short = None
        long = None
        for geom in self.__all_geometry:
            if geom.has_tag(angle) and geom.has_tag(str(level)):
                if geom.has_tag("short"):
                    short = geom
                if geom.has_tag("long"):
                    long = geom
        return short, long

    def sanity_check(self, threshold = 1):
        consitant = True
        for geom in self.__all_geometry:
            if geom.has_tag("linkage"):
                dist = self.__pythagoras2(geom.get_nodes()[0].get_x()-geom.get_nodes()[1].get_x(), geom.get_nodes()[0].get_y()-geom.get_nodes()[1].get_y())
                if abs(dist - geom.get_length()) > threshold and geom.get_nodes()[0] in self.__all_geometry and geom.get_nodes()[1] in self.__all_geometry:
                    consitant = False
                    print("Inconsistancy detected")
                    print(f"A linkage of length {geom.get_length()} is impossible between nodes {geom.get_nodes()[0].get_xy()} and {geom.get_nodes()[1].get_xy()}")
                    print(f"Tags are: Linkage {geom.get_tags()}, Node 1 {geom.get_nodes()[0].get_tags()}, Node 2 {geom.get_nodes()[1].get_tags()}")

        if consitant:
            print("The model is consistant")

    def add_angles(self, linkage_a: Linkage, linkage_b: Linkage) -> Linkage:
        short_edge, long_edge = self.__get_short_edge_long_edge(linkage_a, linkage_b)
        short_outer = self.__get_outer_node(short_edge)
        long_outer = self.__get_outer_node(long_edge)
        new_node = self.__get_new_node(short_edge, long_edge, short_outer)
        small_angle = self.__get_angle_of_node(long_outer)
        large_angle = self.__get_angle_of_node(short_outer)
        sum_angle = small_angle + large_angle
        x_half,y_half = self.__get_x_y_for_angle_and_length(sum_angle/2, long_edge.get_length()/2)
        x_full, y_full = self.__get_x_y_for_angle_and_length(sum_angle, long_edge.get_length()/4)
        half_node = Node(["helper", "additor"], False, (x_half,y_half))
        full_node = Node(["additor"], False, (x_full, y_full))
        self.__all_geometry.append(half_node)
        self.__all_geometry.append(full_node)
        self.__all_geometry.append(Linkage(["helper", "additor"], self.__origin, half_node, long_edge.get_length()/2))
        result_handle = Linkage(["additor"], self.__origin, full_node, long_edge.get_length()/4)
        self.__all_geometry.append(result_handle)
        outer_multiplicator_node = Node(["additor", "helper"], True, (long_edge.get_length(), 0))
        self.__all_geometry.append(outer_multiplicator_node)
        self.__all_geometry.append(Linkage(["additor", "helper"], self.__origin, outer_multiplicator_node, long_edge.get_length(), True))
        lower_multiplicator_node_x, lower_multiplicator_node_y = self.__calculate_position_of_lower_multiplicator_node(sum_angle/2, long_edge.get_length()/2)
        lower_multiplicator_node = Node(["additor", "helper"], False, (lower_multiplicator_node_x, lower_multiplicator_node_y))
        self.__all_geometry.append(lower_multiplicator_node)
        self.__all_geometry.append(Linkage(["additor", "helper"], lower_multiplicator_node, outer_multiplicator_node, long_edge.get_length()/2))
        self.__all_geometry.append(Linkage(["additor", "helper"], half_node, lower_multiplicator_node, long_edge.get_length()))
        multiplicator_helper_node = self.__place_helper_node_for_multiplicator(["additor", "helper"],["additor", "helper"], long_edge.get_length()/4, half_node, lower_multiplicator_node)
        self.__all_geometry.append(Linkage(["additor", "helper"], multiplicator_helper_node, full_node, long_edge.get_length()/2))
        lower_additor_node_x, lower_additor_node_y = self.__calculate_position_of_lower_multiplicator_node(sum_angle/2 - small_angle, long_edge.get_length()/2)
        lower_additor_node_x_rotated = math.cos(small_angle) * lower_additor_node_x - math.sin(small_angle) * lower_additor_node_y
        lower_additor_node_y_rotated = math.sin(small_angle) * lower_additor_node_x + math.cos(small_angle) * lower_additor_node_y
        lower_additor_node = Node(["additor", "helper"], False, (lower_additor_node_x_rotated, lower_additor_node_y_rotated))
        self.__all_geometry.append(lower_additor_node)
        self.__all_geometry.append(Linkage(["additor", "helper"], lower_additor_node, long_outer, long_edge.get_length()/2))
        self.__all_geometry.append(Linkage(["additor", "helper"], lower_additor_node, half_node, long_edge.get_length()))
        additor_helper_node = self.__place_helper_node_for_multiplicator(["additor", "helper"], ["additor", "helper"], long_edge.get_length()/4, half_node, lower_additor_node)
        self.__all_geometry.append(Linkage(["additor", "helper"], new_node, additor_helper_node, long_edge.get_length()/2))
        return result_handle


    def __get_short_edge_long_edge(self, linkage_a: Linkage, linkage_b: Linkage) -> tuple[Linkage, Linkage]:
        outer_node_a = self.__get_outer_node(linkage_a)
        outer_node_b = self.__get_outer_node(linkage_b)
        angle_a = self.__get_angle_of_node(outer_node_a)
        angle_b = self.__get_angle_of_node(outer_node_b)
        return (linkage_a, linkage_b) if angle_a > angle_b else (linkage_b, linkage_a)

    def __get_angle_of_node(self, node: Node) -> float:
        x,y = node.get_xy()
        hyp = self.__pythagoras2(x, y)
        if y > 0:
            return math.acos(x/hyp)
        else:
            return math.pi + (math.pi - math.acos(x/hyp))
        
    def __get_x_y_for_angle_and_length(self, angle: float, length: float) -> tuple[float, float]:
        y = length * math.sin(angle)
        x = length * math.cos(angle)
        return x,y

    def __get_new_node(self, short_edge, long_edge, reference_node):
        new_node = None
        if long_edge.get_length() != short_edge.get_length() * 4:
            new_x = reference_node.get_x() * long_edge.get_length() / 4 / short_edge.get_length() 
            new_y = reference_node.get_y() * long_edge.get_length() / 4 / short_edge.get_length()
            new_node = Node(["helper", "additor"], False, (new_x, new_y))
            self.__all_geometry.append(new_node)
            self.__all_geometry.append(Linkage(["helper", "additor"], new_node, self.__origin, long_edge.get_length()/4))
            self.__all_geometry.append(Linkage(["helper", "additor"], new_node, reference_node, abs(long_edge.get_length()/4 - short_edge.get_length())))
        else:
            new_node = reference_node
        return new_node

    def __get_outer_node(self, linkage: Linkage) -> Node:
        return linkage.get_nodes()[0] if linkage.get_nodes()[0] != self.__origin else linkage.get_nodes()[1]

    def add_or_substract_half_pi_to_linkage_angle(self, linkage: Linkage, add: bool = True) -> Linkage:
        outer_node = self.__get_outer_node(linkage)
        new_angle = self.__get_angle_of_node(outer_node) + (math.pi/2 * (1 if add else -1))
        x,y = self.__get_x_y_for_angle_and_length(new_angle, linkage.get_length())
        new_node = Node(["pi/2"], False, (x,y))
        self.__all_geometry.append(new_node)
        new_handle = Linkage(["pi/2"], self.__origin, new_node, linkage.get_length())
        self.__all_geometry.append(new_handle)
        self.__all_geometry.append(Linkage(["pi/2", "helper"], outer_node, new_node, self.__pythagoras2(linkage.get_length(), linkage.get_length())))
        return new_handle

    def lengthen_or_shorten_linkage_to_length(self, linkage: Linkage, length: float) -> Linkage:
        length = length * self.__scale_factor
        if linkage.get_length() == length:
            return linkage
        outer_node = self.__get_outer_node(linkage)
        x = outer_node.get_x() * length / linkage.get_length()
        y = outer_node.get_y() * length / linkage.get_length()
        new_node = Node(["length_change"], False, (x,y))
        self.__all_geometry.append(new_node)
        result_linkage = Linkage(["length_change"], self.__origin, new_node, abs(length))
        self.__all_geometry.append(result_linkage)
        self.__all_geometry.append(Linkage(["length_change"], outer_node, new_node, abs(length - linkage.get_length())))
        return result_linkage

    def add_up_linkages_to_final_result(self, linkages: list[Linkage], inner_node: Node = None) -> Node:
        if inner_node is None:
            inner_node = self.__origin
    
        new_linkages = []
        inner_a, outer_a = self.__get_inner_and_outer_node(linkages[0], inner_node)
        final_node = None
        for linkage in linkages[1:]:
            inner_b, outer_b = self.__get_inner_and_outer_node(linkage, inner_node)
            new_node = Node(["combination"], False, (outer_a.get_x() + (outer_b.get_x() - inner_b.get_x()), outer_a.get_y() + (outer_b.get_y() - inner_b.get_y())))
            new_linkage = Linkage(["combination"], outer_a, new_node, linkage.get_length())
            new_linkages.append(new_linkage)
            self.__all_geometry.append(new_linkage)
            self.__all_geometry.append(Linkage(["combination"], outer_b, new_node, linkages[0].get_length()))
            self.__all_geometry.append(new_node)
            final_node = new_node
        if len(new_linkages) > 1:
           return self.add_up_linkages_to_final_result(new_linkages, outer_a)
        else:
            final_node.append_tag("final")
            return final_node
        

    def __get_inner_and_outer_node(self, linkage: Linkage, inner_node: Node):
        if linkage.get_nodes()[0] == inner_node:
            return linkage.get_nodes()[0], linkage.get_nodes()[1]
        else: 
            return linkage.get_nodes()[1], linkage.get_nodes()[0]

    def add_peaucellier_linkage(self, node: Node):
        x,y = node.get_xy()
        inner_distance = math.sqrt((self.__scale_factor * 2)**2/2)
        upper_node = Node(["peaucellier"], False, (x-inner_distance, y+inner_distance))
        lower_node = Node(["peaucellier"], False, (x-inner_distance, y-inner_distance))
        left_node = Node(["peaucellier"], False, (x-2*inner_distance, y))
        inner_anchor_node = Node(["peaucellier"], True, (x-2*inner_distance-2*self.__scale_factor, y))
        outer_anchor_node = Node(["peaucellier"], True, (x-2*inner_distance-4*self.__scale_factor, y))
        outer_linkage_length = math.sqrt(inner_distance**2 + (4*self.__scale_factor+inner_distance)**2)
        self.__all_geometry.append(upper_node)
        self.__all_geometry.append(lower_node)
        self.__all_geometry.append(left_node)
        self.__all_geometry.append(inner_anchor_node)
        self.__all_geometry.append(outer_anchor_node)
        self.__all_geometry.append(Linkage(["peaucellier"], node, upper_node, 2*self.__scale_factor))
        self.__all_geometry.append(Linkage(["peaucellier"], node, lower_node, 2*self.__scale_factor))
        self.__all_geometry.append(Linkage(["peaucellier"], left_node, upper_node, 2*self.__scale_factor))
        self.__all_geometry.append(Linkage(["peaucellier"], left_node, lower_node, 2*self.__scale_factor))
        self.__all_geometry.append(Linkage(["peaucellier"], left_node, inner_anchor_node, 2*self.__scale_factor))
        self.__all_geometry.append(Linkage(["peaucellier"], outer_anchor_node, upper_node, outer_linkage_length))
        self.__all_geometry.append(Linkage(["peaucellier"], outer_anchor_node, lower_node, outer_linkage_length))

    def get_geometry(self):
        return self.__all_geometry
