import drawSvg as draw
from enum import Enum
import math
strip =[]
stripwidth = 10
striplength=0
def render (strip, striplength, stripwidth):
    d = draw.Drawing(1000,stripwidth)
    for element in strip:
        if type(element) is Plain:
            striplength += element.length
    d.append(draw.Rectangle(0, 0, striplength, stripwidth,stroke='red', fill='none'))
    d.saveSvg('test.svg')
    d.rasterize()
    return d
