import sympy as sy
from sympy.simplify.fu import TR5, TR8, TR0
alpha,beta,r,x,y = sy.symbols('a b r x y')
expr = x**3*y - 5*x*y**2
expr = expr.subs(x, (r/2)*sy.cos(alpha) + (r/2)*sy.cos(beta))
expr = expr.subs(y, (r/2)*sy.sin(alpha) + (r/2)*sy.sin(beta))
sy.pprint ((TR5(TR8(TR0(expr))).rewrite(sy.cos)))
