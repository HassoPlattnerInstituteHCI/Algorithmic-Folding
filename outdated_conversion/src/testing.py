from utils import *
from linkage_geometry import *

r = 1
p1 = Point(0, 0)
p2 = Point(-1, 1)
p3 = Point(0, 2)
p4 = Point(1, 1)
p5 = Point(-1.5, 0.5)
p6 = Point(-0.5, 1)
p7 = Point(-2, 0)
p8 = Point(-2, 2)
p9 = Point(2 / 3, 8 / 3)
p10 = Point(5, 6)
p11 = Point(6, 4)


def create_rhombus_expression():
    base = Rhombus(p1, p2, p3, p4)
    print("--- Rhombus ---")
    print(geometry_to_expression(base))


def create_counterparallelogram_expression():
    cpara = CounterParallelogram(p1, p2, p3, p4)
    print("--- Counter Parallelogram ---")
    print(geometry_to_expression(cpara))


def create_multiplicator_expression():
    multi = Multiplicator(p1, p2, p3, p4, p5, p6)
    print("--- Multiplicator [combined]---")
    print(global_to_expression())

    # for debugging purposes:
    # print("--- Multiplicator [individual part 1]---")
    # print(geometry_to_expression(multi.geos[0]))
    # print("--- Multiplicator [individual part 2]---")
    # print(geometry_to_expression(multi.geos[1]))


def create_additor_expression():
    addi = Additor(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11)
    # print(global_to_expression())
    print("--- Additor [combined]---")

    expression = "M["
    for geos in addi.geos:
        for geo in geos.geos:
            expression += dp_to_expression(geo.points)
    expression += "\n]"
    print(expression)
    # todo: fix the link and point creation, there are 15 instead of 11 points and incorrect links


# for debugging:
"""# Generate by Pyslvs 21.12.0
# Example Additor (slightly incorrect point placement)"
M[
    J[R, color[Blue], P[-0.0013, 0.0272], L[ground, link_1, link_5, link_6, link_7, link_11]],
    J[R, color[Green], P[-1.0424, 1.7395], L[link_7, link_9]],
    J[R, color[Green], P[-0.0013, 0.8628], L[link_1, link_2]],
    J[R, color[Green], P[0.985, 0.9039], L[link_3, link_6]],
    J[R, color[Green], P[1.5192, 0.6847], L[link_2, link_3]],
    J[R, color[Green], P[2.7658, 0.0683], L[link_3, link_4]],
    J[R, color[Green], P[2.5603, 1.1094], L[link_4, link_5]],
    J[R, color[Green], P[2.4233, 2.356], L[link_6, link_8]],
    J[R, color[Green], P[4.8754, -0.9454], L[link_8, link_10]],
    J[R, color[Green], P[5.6699, 0.0272], L[link_10, link_11]],
    J[R, color[Green], P[2.9987, 1.671], L[link_8, link_9]]
]"""

# this generated a valid Mechanism-Expression that can be imported into PYSLVS through "Mechanism -> PASTE"
if __name__ == '__main__':
    # create_rhombus_expression()
    # create_counterparallelogram_expression()
    # create_multiplicator_expression()
    create_additor_expression()
    # import_test(1000)
