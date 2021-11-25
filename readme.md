# AF-Developable-surface

Algorithmic folding course - Developable surface
=======

This course is based on efficient C++ libraries binded to python.
The main philosophy is to use `NumPy` arrays as a common interface, making them highly composable with each-other as well as existing scientific computing packages.

## Installation

The easiest way to install the libraries is trough the [conda](https://anaconda.org/) or [miniconda](https://docs.conda.io/en/latest/miniconda.html) python package manager.

All libraries are part of the channel [conda forge](https://conda-forge.org/), which we advise to add to your conda channels by:
```bash
conda config --add channels conda-forge
```
This step allows to install any conda forge package simply with `conda install <package>`.

To install all our packages just run:
```bash
conda install meshplot
conda install igl
```

**Note 1**: that you can install only the ones you need.

**Note 2**: in case of problem we advise to create a new conda environment `conda create -n <name>`.

**Note 3**: if problem persist or your want you feature please post issues on the github bugtracker of each library or [here](https://github.com/geometryprocessing/geometric-computing-python/issues).

### Packages Description

The two packages have specific functionalities and own website.

- [Meshplot](https://skoch9.github.io/meshplot/): fast 2d and 3d mesh viewer based on `pythreejs`.
- [igl](https://libigl.github.io/): swiss-army-knife of geometric processing functions ([python documentation](https://libigl.github.io/libigl-python-bindings/))

## Experiment 1 - Gauss map

See the jupyter notebook [gauss_map.ipynb](https://github.com/origamidance/AF-Developable-surface/blob/main/gauss_map.ipynb).