# Goal
generate a long strip to fold any given STL file

# Install Dependencies

You'll need the python [libigl package](https://libigl.github.io/libigl-python-bindings/), a geometry processing library by [Alec Jacbson](http://www.cs.toronto.edu/~jacobson/), that is popular in computer graphics research. The package that include the python bindings wants to be installed with the Anaconda package manager, hence we recommend to use Anaconda for this course.

Once you have conda installed, open the project in a conda-enabled shell and run

```sh
conda install -c conda-forge igl
```
(This will also install things like Numpy and SciPy as well.)


Furthermore, we need a way of dealing with vector graphics. We use drawSvg for this, which in turn uses the [Cairo Graphics library](https://www.cairographics.org/).

First, make sure that you have Cairo installed on your platform: https://www.cairographics.org/download/

Secondly, install drawSvg:
```sh
python -m pip install drawSvg
```

Once you set all of this up, you just need to open the project using Jupyter Notebook. You can install it using
```sh
conda install jupyter
```

and open it with
```sh
jupyter notebook
```
or use Visual Studio Code with the respective Jupyter Notebook extensions.


If you deviate from this setup due to personal taste, make sure that pip, conda and jupyter all point to the same environment to avoid "package not found"-errors.