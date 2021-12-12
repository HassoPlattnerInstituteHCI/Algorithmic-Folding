from shapely.geometry import Polygon, LineString
import drawSvg as draw

circle_color = "#DEB887"
river_color = "blue"
crease_color = "red"
guide_color = "lightgrey"
polygon_color = "black"

line_thickness = 2
viewport = 700

def max_val (points,x=0, y=0):
    for point in points: 
        if point.x > x: x = point.x
        if point.y > y: y = point.y
    return float(x),float(y)

def draw_geom(p,d,col,fill='none'):
    path = draw.Path(stroke=col, stroke_width=d.width/viewport*line_thickness, fill=fill)
    path.M(p[0][0], p[0][1]) #point is a tuple (x,y)
    [path.L(v[0],v[1]) for v in p]
    d.append(path)
    
def render(points,circles,creases,guides,polys,rivers):
    w,h = max_val(points)
    d = draw.Drawing (w,h)
    [draw_geom(c.exterior.coords,d,polygon_color,circle_color) for c in circles]
    [draw_geom(p.exterior.coords,d,polygon_color) for p in polys]
    [draw_geom(c.coords,d,crease_color) for c in creases]
    [draw_geom(g.coords,d,guide_color) for g in guides]
    [draw_geom(r.coords,d,river_color) for r in rivers if type(r) == LineString]
    d.setRenderSize(viewport,viewport*h/w)
    d.saveSvg("export.svg")
    return d