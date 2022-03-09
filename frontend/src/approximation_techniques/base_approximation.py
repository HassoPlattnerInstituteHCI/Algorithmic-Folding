from abc import abstractmethod

class BaseApproximation:

    @abstractmethod
    def set_parameters_and_approximate(self, parameter_map, xdata, ydata):
        pass

    def get_approximation(self, x_min, x_max):
        result = []
        function = self.get_approximated_function()
        for i in range(x_min, x_max+1):
            y = function(i)
            result.append([i, y])
        return result


    @abstractmethod
    def get_approximated_function(self):
        pass

    @abstractmethod
    def get_sympy_expression(self):
        pass
    