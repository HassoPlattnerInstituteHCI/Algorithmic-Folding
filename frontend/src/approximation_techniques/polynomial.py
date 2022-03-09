from approximation_techniques.base_approximation import BaseApproximation
from scipy.optimize import curve_fit as cf
from types import FunctionType
import sympy as sy
from sympy.simplify.fu import TR5, TR8, TR0, TR7

class PolynomialApproximation(BaseApproximation):

    def get_function(self, n):
        func =  f'def approx(x, {", ".join(["a" + str(i) for i in range(n)])}):\n  return {" + ".join(["a" + str(i) + ("*x**") + str(i) for i in range(n)])}'
        f_code = compile(func, "<float>", "exec")
        f_func = FunctionType(f_code.co_consts[0], globals(), "approx")

        return f_func

    def set_parameters_and_approximate(self, parameter_map, xdata, ydata):
        n = int(parameter_map["N"]) + 1
        self.__func = self.get_function(n)
        popt, pcov = cf(self.__func, xdata, ydata)
        self.__popt = popt

    def get_approximated_function(self):
        n = len(self.__popt)
        func =  f'def approx(x):\n  return {" + ".join([str(self.__popt[i]) + ("*x**") + str(i) for i in range(n)])}'
        f_code = compile(func, "<float>", "exec")
        f_func = FunctionType(f_code.co_consts[0], globals(), "approx")
        return FunctionType(f_code.co_consts[0], globals(), "get_approx_value")

    def get_sympy_expression(self):
        alpha,beta,r,x,y = sy.symbols('alpha beta r x y')
        expr = 0
        for i in range(len(self.__popt)):
            expr += self.__popt[i]*x**i
        expr -= y
        expr = expr.subs(x, (r/2)*sy.cos(alpha) + (r/2)*sy.cos(beta))
        expr = expr.subs(y, (r/2)*sy.sin(alpha) + (r/2)*sy.sin(beta))
        result = (TR5(TR8(TR0(expr))).rewrite(sy.cos))
        return result