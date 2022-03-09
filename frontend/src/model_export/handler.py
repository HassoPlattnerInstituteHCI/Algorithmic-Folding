import sympy as sy
from sympy.solvers import solve
import matplotlib.pyplot as plt
from model_export.model import Model
from model_export.converter import convert_geometry_to_mechanism

class function_exporter:

    def __init__(self, function: sy.core.add.Add) -> None:
        self.__function = function
        self.__model = self.__build_model()

    def export_model_to_external(self) -> None:
        self.draw_linkage()
        print("Please copy the following model string to pyslvs\n\n")
        print(convert_geometry_to_mechanism(self.__model))
    
    def __build_model(self) -> Model:
        r = sy.symbols('r')
        self.__function = self.__function.subs(r, 1)
        print(self.__function)
        initial_a, initial_b = self.__get_initial_alpha_beta()
        print(f"initial alpha is {initial_a} and initial beta is {initial_b}")
        linkages = []
        model = Model(initial_angles={"alpha": initial_a, "beta": initial_b})
        for key, value in self.__function.as_coefficients_dict().items():
            if key == 1:
                continue
            linkages.append(model.lengthen_or_shorten_linkage_to_length(self.__get_linkage_for_component(key, model), value))
        final_node = model.add_up_linkages_to_final_result(linkages)
        model.add_peaucellier_linkage(final_node)
        model.sanity_check()
        return model

    def __get_initial_alpha_beta(self, a = sy.pi/8):
        alpha, beta = sy.symbols('alpha beta')
        solve_func = 0
        for key, value in self.__function.as_coefficients_dict().items():
            if key == 1:
                continue
            solve_func = solve_func + value * key

        solve_func = solve_func.simplify()
        print(f"We need an initial configuration for alpha and beta that solves the following expression to zero: \n{solve_func}\n\n" +
        "We can try to computationally solve that, but this may take a while. If you wish, provide two angles (in radians) now or press enter to skip and infer them computationally.\n")

        alph = input("alpha: ")
        bet = input("beta: ")

        if alph != "" and bet != "":
            return float(alph), float(bet)
        else:
            res = solve(solve_func, beta, dict=True)
            b = res[0][beta].subs(alpha, a)
            return a.evalf(), b.evalf()


    def __get_linkage_for_component(self, component, model: Model):
        sub_components = component.args[0].as_coefficients_dict().items()
        alpha, beta = sy.symbols('alpha beta')
        alpha_linkage = None
        beta_linkage = None
        add_pi = 0
        for angle, factor in sub_components:
            if angle == alpha:
                alpha_linkage = model.create_and_get_multiplicator_of_factor(factor, "alpha")
            elif angle == beta:
                beta_linkage = model.create_and_get_multiplicator_of_factor(factor, "beta")
            else:
                add_pi = factor
        result_linkage = alpha_linkage if beta_linkage is None else beta_linkage
        result_linkage = model.add_angles(alpha_linkage, beta_linkage) if ((alpha_linkage is not None) and (beta_linkage is not None)) else result_linkage
        return model.add_or_substract_half_pi_to_linkage_angle(result_linkage, True if add_pi > 0 else False) if add_pi != 0 else result_linkage


    def draw_saxena(self) -> None:
        linkages = []
        model = Model()
        linkages.append(model.lengthen_or_shorten_linkage_to_length(model.create_and_get_multiplicator_of_factor(1, "alpha"), 0.353553390593274))
        linkages.append(model.lengthen_or_shorten_linkage_to_length(model.create_and_get_multiplicator_of_factor(2, "alpha"), 0.25))
        linkages.append(model.lengthen_or_shorten_linkage_to_length(model.create_and_get_multiplicator_of_factor(1, "beta"), 0.353553390593274))
        linkages.append(model.lengthen_or_shorten_linkage_to_length(model.create_and_get_multiplicator_of_factor(2, "beta"), 0.25))
        linkages.append(model.lengthen_or_shorten_linkage_to_length(model.add_or_substract_half_pi_to_linkage_angle(model.create_and_get_multiplicator_of_factor(1, "alpha"), False), -0.353553390593274))
        linkages.append(model.lengthen_or_shorten_linkage_to_length(model.add_or_substract_half_pi_to_linkage_angle(model.create_and_get_multiplicator_of_factor(1, "beta"), False), -0.353553390593274))
        linkages.append(model.lengthen_or_shorten_linkage_to_length(model.add_angles(model.create_and_get_multiplicator_of_factor(1, "alpha"), model.create_and_get_multiplicator_of_factor(1, "beta")), 0.5))
        model.add_up_linkages_to_final_result(linkages)
        model.sanity_check()
        model.draw_linkage()
    
    def draw_linkage(self, text: str = None) -> None:
        for geom in self.__model.get_geometry():
            color = "black"
            if geom.has_tag("rhombus"):
                color = "red"
            elif geom.has_tag("alpha"):
                color = "green"
            elif geom.has_tag("beta"):
                color = "blue"
            elif geom.has_tag("combination"):
                color = "purple"
            elif geom.has_tag("helper"):
                color = "grey"
            if geom.has_tag("linkage"):
                x1,y1 = geom.get_nodes()[0].get_xy()
                x2,y2 = geom.get_nodes()[1].get_xy()
                plt.plot([x1, x2], [y1, y2], color = color)
            else:
                x,y = geom.get_xy()
                if geom.has_tag("final"):
                    plt.plot(x, y, marker='o', color="yellow", markersize=10)
                else:
                    plt.plot(x, y, marker='o', color=color)
        plt.show()
