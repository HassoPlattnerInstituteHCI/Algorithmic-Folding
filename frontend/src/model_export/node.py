from model_export.geometry import Geometry

class Node(Geometry):
    
    def __init__(self, tags: list[str], is_fixed: bool, location: tuple[float, float]=None) -> None:
        tags.append("node")
        super().__init__(tags)
        
        self.__linkages = []
        if location is not None:
            self.__x = location[0]
            self.__y = location[1]

        self.__is_fixed = is_fixed

    def __str__(self):
        return "Node [" + str(self.__x) + " | " + str(self.__y) + "]"

    def add_linkage(self, linkage) -> None:
        self.__linkages.append(linkage)
    
    def get_linkages(self):
        return self.__linkages

    def get_x(self) -> float:
        return self.__x

    def get_y(self) -> float:
        return self.__y

    def get_xy(self) -> tuple[float, float]:
        return self.__x, self.__y

    def is_fixed(self) -> bool:
        return self.__is_fixed
