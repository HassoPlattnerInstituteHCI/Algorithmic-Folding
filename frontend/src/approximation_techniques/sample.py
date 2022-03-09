from approximation_techniques.base_approximation import BaseApproximation

def y_is_x(x):
        return x

        
class PolynomialApproximation(BaseApproximation):
    def set_parameters_and_approximate(self, parameter_map, xdata, ydata):
        pass


    def get_approximated_function(self):
        return y_is_x

    def get_sympy_expression(self):
        return None