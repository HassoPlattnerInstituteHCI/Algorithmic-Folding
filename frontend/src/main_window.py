
import PySimpleGUI as sg
import math

from approximation_techniques.polynomial import PolynomialApproximation
from model_export.converter import convert_geometry_to_mechanism

from model_export.model import Model

from window_layout import WindowLayout
from model_export.handler import function_exporter
import sympy as sy
from sympy.simplify.fu import TR5, TR8, TR0

STROKE_WIDTH = 2
POINT_DISTANCE_THRESHOLD = 40
points = []
def paint(event):
    w = window["CANVAS"].tk_canvas
    if len(points) > 0 and math.sqrt((event.x - points[-1][0])**2 + (event.y - points[-1][1])**2) < POINT_DISTANCE_THRESHOLD:
        w.create_line(event.x,event.y,points[-1][0],points[-1][1], fill="blue", width=STROKE_WIDTH)
    points.append([event.x, event.y])

def init_canvas():
    window["CANVAS"].tk_canvas.bind("<Button1-Motion>", paint)
    w = window["CANVAS"].tk_canvas
    w.create_line(0, layout.CANVAS_SIZE_Y//2, layout.CANVAS_SIZE_X, layout.CANVAS_SIZE_Y//2)
    w.create_line(layout.CANVAS_SIZE_X//2, 0, layout.CANVAS_SIZE_X/2, layout.CANVAS_SIZE_Y)

def get_xy_data(raw_points):
    xdata = []
    ydata = []
    for x,y in raw_points:
        xdata.append(x-layout.CANVAS_SIZE_X//2)
        ydata.append((y-layout.CANVAS_SIZE_Y//2) * -1)

    return xdata, ydata

def draw_approximated(func):
    points_n = []
    for i in range(-layout.CANVAS_SIZE_X//2,layout.CANVAS_SIZE_X//2):
        y = func(i)
        points_n.append([i+layout.CANVAS_SIZE_X//2, (y*-1) + layout.CANVAS_SIZE_Y//2])

    w = window["CANVAS"].tk_canvas
    for i in range(len(points_n)-2):
        w.create_line(points_n[i][0], points_n[i][1], points_n[i+1][0], points_n[i+1][1], fill="purple", width=STROKE_WIDTH)

def clear():
    layout.set_export_button(window, True)
    points.clear()
    window["CANVAS"].tk_canvas.delete('all')
    init_canvas()

layout = WindowLayout()
window = sg.Window(title="Kempe Linkage", layout=layout.get_layout(), margins=(10, 10), finalize=True)
init_canvas()

while True:
    event, values = window.read()
    if event == "Exit" or event == sg.WIN_CLOSED:
        break
    if event == "METHOD_SELECTION":
        layout.switch_parameter_group(window)
    if event == "BUTTON":
        method, parameters = layout.get_selected_method(window)
        approx = method["approximation_class"]()
        xdata, ydata = get_xy_data(points)
        approx.set_parameters_and_approximate(parameters, xdata, ydata)
        func = approx.get_approximated_function()
        draw_approximated(func)
        layout.set_export_button(window, False)
    if event == "EXPORT":
        syfunc = approx.get_sympy_expression()
        exporter = function_exporter(syfunc)
        exporter.export_model_to_external()
        # exporter.draw_saxena()
    if event == "CLEAR":
        clear()

window.close()
