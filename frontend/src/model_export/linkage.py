from model_export.node import Node
from model_export.geometry import Geometry

class Linkage(Geometry):
    
    def __init__(self, tags: list[str], node_a: Node, node_b: Node, length: float, horizontal_contraint: bool=False) -> None:
        tags.append("linkage")
        super().__init__(tags)

        self.__node_a = node_a
        self.__node_b = node_b
        self.__length = length
        self.__constraint = horizontal_contraint
        node_a.add_linkage(self)
        node_b.add_linkage(self)

    def __str__(self):
        return "Link [" + str(self.__node_a) + " & " + str(self.__node_b) + "]"

    def get_nodes(self) -> tuple[Node, Node]:
        return self.__node_a, self.__node_b

    def get_length(self) -> float:
        return self.__length

    def is_constrained_horizontically(self) -> bool:
        return self.__constraint