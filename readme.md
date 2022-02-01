# Algorithmic-Folding (WS2021/22) by Thijs Roumen and Abdullah Muhammad

![2022_01_31 banner folding course_1](https://user-images.githubusercontent.com/1307670/151854735-180ea79b-f2d6-4b9a-852b-8bfbd941158f.png)

**objective**: implement and study folding algorithms applied to linkages (1D), origami (2D), and polyhedra (3D). We will go through the high level algorithms as well as practical in-class hacking assignments and learn about applied engineering examples. You will furthermore learn how to represent some intractable high dimensional folding problems in lower dimensions to then leverage universal and generic algorithms. You will learn how folding algorithms formed the backbone of several research projects at our lab in in the broader HCI and graphics field. And finally, you will learn how to extend what is known by running a small in-depth research project on one of the lectures, advised by the class instructors.

**description**: Every week you will code up a simple program in python through mostly in-class assignments, continued into homework assignments. The homework serves as a self-check and we provide full solutions in the code base. We are fully available to help with this as our objective is not to confirm that you are great students (we know that), but to help you get the most out of the learning experience. The in-depth assignment gives you an opportunity to do a little research beyond the paved roads of knowledge, every lecture contains an example in-depth assignment, but you are free to propose your own too. The instructors and TA are fully available to help you succeed at this. We are generally interested in the outcomes too! We can help code things up and create use cases etc and if you are excited to pursue the project beyond the lecture period, we will collaborate to turn this into an actual research paper or practical open source software tool!

**mailing list**: please sign up to [this mailing list](https://myhpi.de/lists/p2EiNpHZWMBnltwycLTcw4kFapI01t8Rq49zRw49Q)

**pre-requisites**: Master level course, heavily building on Maths 1+2 and Programming Technology 1+2

**language**: python

**dependenies**: (please install before the lectures to maximize flow, contact a TA should you have any problems)
- jupyter notebook 
- scipy
- numpy
- sympy
- networkx
- itertools
- shapely
- openmesh
- drawSVG
- igl
- meshplot

**deliverables and grading**: weekly homework assignment (ungraded, solution provided) a mid-term (30%) and final exam (70%) and in in-depth project in which you dive deeper into any of the lecture's topics. The in-depth assignment is evaluated through a question about your approach and findings as part of the final exam

**workload**: weekly 3hr time slot, in which the first slot is lecture + in-class coding and the second slot for homework with the instructors around. Some weeks (indicated in the schedule) are 3hr lecture only.

**further reading**: Geometric Folding Algorithms by Erik D. Demaine and Joseph o' Rourke is a fantastic standard work in this discipline and Origami Design Secrets, mathematical methods for an ancient art by Robert J. Lang takes a deep dive into the Tree Method and origami specific content

**contact information**: no official office hours--contact by email, we are happy to set up extra zoom calls for support anytime
- Thijs Roumen (instructor) : thijs.roumen@hpi.de
- Abdullah Muhammas (co-instructor): abdullah.muhammad@hpi.de
- Lukas Rambold (TA): lukas.rambold@hpi.de

**coding logistics**: this repository contains the code base for all assignments in the course. Use the individual branches or the links in the schedule to be redirected to the repository of the week (`git checkout <branchname>`). Please pull the branch right before each lecture starts so we are all using the same up-to-date code base (`git pull`)

**schedule, slides and code repositories**:
[COVID]
unfortunately from December on, our lecture takes place via zoom:
Meeting ID: 910 1917 9704
Passcode: 56942491

| week  | date |     content     |  code branch | notes |
|---|----------|---|--------|---|
| 1  | Mon 01.11.21 | [vertex unfolding.ppt](https://www.dropbox.com/s/aowysjp3rvkdwjm/05%20AF-Unfolding%20polyhedra%20%28vertex%29%20%28intro%20version%29.pptx?dl=0) + homework |  [code (soon) in repo](https://docs.google.com/document/d/1pCiY1Nrs-NpOeYKXIFngJyGDFIbKDWrsfiCv5-pspqY/edit)      | swap with lecture 2 again next year  |
| 2  | Mon 08.11.21 | [strip folding.ppt](https://www.dropbox.com/s/yfzdljgw7a90f00/16%20Algorithmic%20Folding%20%5B120min%5D.pptx?dl=0) + homework | [folding/01](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/Folding/01)   |   |
| 3  | Mon 15.11.21 | [edge unfolding.ppt](https://www.dropbox.com/s/5zli3li184w394l/13%20AF-Unfolding%20polyhedra%20%28120%20min%29.pptx?dl=0) + homework | [unfolding/01](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/Unfolding/01)   |   |
| 4  | Mon 22.11.21 | [unfolding+lasercutting.ppt](https://www.dropbox.com/s/uakfww4knxg0xfk/08%20AF-Unfolding%20polyhedra%20%28application%29%2873%20min%29.pptx?dl=0) + homework | [unfolding/02](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/Unfolding/02)   |   |
| 5  | Mon 29.11.21 | [general unfolding.ppt](https://www.dropbox.com/s/w6s583px4sn7wcg/05%20AF-Unfolding%20polyhedra%20%28general%29%20%2860%20min%29.pptx?dl=0) + homework | [unfolding/03](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/Unfolding/03)   |   |
| 6  | Mon 06.12.21 | [developable surfaces.ppt](https://www.dropbox.com/s/l9kp2feqclq5fu5/Ran%20Zhang-Developable%20surface%20modeling.pptx?dl=0) double slot | [developable_surface](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/developable_surface)   |   |
| 7  | Mon 13.12.21 | [efficient origami (tree method).ppt](https://www.dropbox.com/s/lgsazzs65ejmzfa/11%20AF-TreeMethod.pptx?dl=0) + homework  | [folding/02](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/Folding/02)   |   |
| 8  | Mon 20.12.21 | mid-term exam  |   |   |
| 9  | Mon 10.01.22 | [linkage folding (kempe's universality).ppt](https://www.dropbox.com/s/izky9yvkxipzzgc/16%20AF-Linkage%20Folding%20%20%5B150min%5D.pptx?dl=0) + homework | [linkages_kempe](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/linkages-kempe) |   |
| 10  | Mon 17.01.22 | [rigidity theory.ppt](https://www.dropbox.com/s/wmtjtmcobv97zpm/05%20AF-rigidity%20%5B180min%5D.pptx?dl=0) double slot | [rigidity/01](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/Rigidity/01)  |   |
| 12  | Mon 24.01.22 | [tensegrities.ppt](https://www.dropbox.com/s/ub4krn4sz2nffty/12%20AF-tensegrities.pptx?dl=0) + in-depth assignment | [rigidity/02-tensegrities](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/Rigidity/02-tensegrities)  |   |
| 13  | Mon 31.01.22 | [multistability and simulation(coming).ppt]() double slot | [multistable](https://github.com/HassoPlattnerInstituteHCI/Algorithmic-Folding/tree/multistable)  |  slides coming soon |
| 14  | Mon 07.02.22 | Guest lecture by [Yasaman Tahouni](https://www.yasamantahouni.com) (IDC (University of Stuttgart), MIT) Programming Shape-Change: From Bioinspiration to Autonomous Origami Structures) |   |   |
| 15  | Mon 14.02.22 | Final Exam  |   |   |
| 16  | Mon 28.02.22 | in-depth projects due (demo session)  |   |   |
