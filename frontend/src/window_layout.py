import PySimpleGUI as sg
from numpy import polynomial
from urllib3 import disable_warnings

from approximation_techniques.polynomial import PolynomialApproximation

class WindowLayout:

    CANVAS_SIZE_X = 700
    CANVAS_SIZE_Y = 200

    __methods = {
        "Polynomial of Degree N": {
            "short_name": "POLYNOMIAL",
            "parameters": [
                {"name": "N", "description_text": "n = ", "type": "free", "default": "2"}
            ],
            "approximation_class": PolynomialApproximation
        },
        "Some other test method that is not implemented": {
            "short_name": "FOO",
            "parameters" : [
                {"name": "A", "description_text": "a text field", "type": "free", "default": ""},
                {"name": "B", "description_text": "a selection field", "type": "selection", "values": ["Eins", "Zwei", "Drei"]}
            ]
        }
    }
        
    def get_layout(self):
        return [
            [
                self.__get_parameters_column(),
                sg.VSeperator(),
                sg.Column([
                    [sg.Text("Please select the desired approximation technique.")],
                    [sg.Combo(list(self.__methods.keys()), readonly=True, enable_events=True, key="METHOD_SELECTION")],
                    [sg.Canvas(size = (self.CANVAS_SIZE_X, self.CANVAS_SIZE_Y), background_color="white", key="CANVAS")],
                    [sg.Button("Clear", key="CLEAR"), sg.Button("Approximate", key="BUTTON", disabled=True), sg.Button("Export", key="EXPORT", disabled=True)]
                ], size = (800, 300))
            ]
        ]

    def __get_parameters_column(self):
        col = []
        for key, method in self.__methods.items():
            method_layout = []
            for param in method["parameters"]:
                row = []
                row.append(sg.Text(param["description_text"]))
                if param["type"] == "free":
                    row.append(sg.Input(default_text=param["default"], key="_".join([method["short_name"], param["name"]])))
                elif param["type"] == "selection":
                    row.append(sg.Combo(param["values"], default_value=param["values"][0], 
                        readonly=True, key="_".join([method["short_name"], param["name"]])))
                else:
                    raise ValueError("This parameter type is unknown")
                method_layout.append(row)

            col.append([sg.Frame(title=key, border_width=0, layout=method_layout, visible=False, key="_".join([method["short_name"], "FRAME"]))])

        return sg.Column(col, size = (400, None))

    def switch_parameter_group(self, window):
        for key, method in self.__methods.items():
            window["_".join([method["short_name"], "FRAME"])].update(visible=False)
        method = self.__methods[window["METHOD_SELECTION"].get()]
        window["_".join([method["short_name"], "FRAME"])].update(visible=True)
        window["BUTTON"].update(disabled=False)

    def get_selected_method(self, window):
        method = self.__methods[window["METHOD_SELECTION"].get()]
        parameters = {}
        for param in method["parameters"]:
            parameters[param["name"]] = window["_".join([method["short_name"], param["name"]])].get()

        return method, parameters

    def set_export_button(self, window, disabled):
        window["EXPORT"].update(disabled=disabled)