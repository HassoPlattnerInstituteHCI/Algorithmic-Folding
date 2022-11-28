# Algorithmic Folding Lecture @ HPI

![2022_01_31 banner folding course_1](https://user-images.githubusercontent.com/1307670/151854735-180ea79b-f2d6-4b9a-852b-8bfbd941158f.png)

*please refer to the wiki page for logistics: https://hpi.de/baudisch/dokuwiki/algorithmic-folding-ws2023/start*

**objective**: implement and study folding algorithms applied to linkages (1D), origami (2D), and polyhedra (3D). We will go through the high level algorithms as well as practical in-class hacking assignments and learn about applied engineering examples. You will furthermore learn how to represent some intractable high dimensional folding problems in lower dimensions to then leverage universal and generic algorithms. You will learn how folding algorithms formed the backbone of several research projects at our lab in in the broader HCI and graphics field. And finally, you will learn how to extend what is known by running a small in-depth research project on one of the lectures, advised by the class instructors.

**description**: Every week you will code up a simple program in python through mostly in-class assignments, continued into homework assignments. The homework serves as a self-check and we provide full solutions in the code base. We are fully available to help with this as our objective is not to confirm that you are great students (we know that), but to help you get the most out of the learning experience. The in-depth assignment gives you an opportunity to do a little research beyond the paved roads of knowledge, every lecture contains an example in-depth assignment, but you are free to propose your own too. The instructors and TA are fully available to help you succeed at this. We are generally interested in the outcomes too! We can help code things up and create use cases etc and if you are excited to pursue the project beyond the lecture period, we will collaborate to turn this into an actual research paper or practical open source software tool!

**pre-requisites**: Master level course, heavily building on Maths 1+2 and Programming Technology 1+2

**language**: python

**dependenies**: (please install before the lectures to maximize flow, contact a TA should you have any problems) `pip install requirements.txt`
- jupyter notebook 
- scipy
- numpy
- sympy
- networkx
- shapely
- drawSVG
- conda
- igl `conda install -c conda-forge igl`
- meshplot `conda install meshplot`

**workload**: weekly 3hr time slot, in which the first slot is lecture + in-class coding and the second slot for homework with the instructors around. Some weeks (indicated in the schedule) are 3hr lecture only.

**further reading**: Geometric Folding Algorithms by Erik D. Demaine and Joseph o' Rourke is a fantastic standard work in this discipline and Origami Design Secrets, mathematical methods for an ancient art by Robert J. Lang takes a deep dive into the Tree Method and origami specific content

**authors**: 
- [Thijs Roumen](http://www.thijsroumen.com) thijs.roumen@hpi.de
- [Abdullah Muhammad](https://www.muhammad-abdullah.com): abdullah.muhammad@hpi.de
- [Ran Zhang](https://ran-zhang.com) (lecturer week 6,13): ran.zhang@hpi.de
- [Lukas Rambold](https://rambold.io): lukas.rambold@hpi.de

**coding logistics**: this repository contains the code base for all assignments in the course. Use the individual branches or the links in the schedule to be redirected to the repository of the week (`git checkout <branchname>`). Please pull the branch right before each lecture starts so we are all using the same up-to-date code base (`git pull`)
