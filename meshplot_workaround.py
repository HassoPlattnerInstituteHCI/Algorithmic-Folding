from IPython.core.display import display, HTML

def plot_mesh(v, f):
  display(HTML(mp.plot(v, f).to_html()))

plot_mesh(vertices, faces)
