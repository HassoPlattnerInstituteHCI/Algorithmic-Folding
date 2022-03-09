# Stroke-based-Kempe-Linkage
Develoment out of interest and based on an assignment in Algorithmic Folding WS2021.

## Description

### Requirements
Make sure the following dependencies are installed
* Python 3
* SolveSpace API
* Pyslvs>=21.12.0 | https://pyslvs-ui.readthedocs.io/en/stable/
* pysimplegui>=4.55.1
* scipy>=1.7.3
* sympy>=1.9

### Usage
This project can be used through a pysimplegui. To open the GUI run `main_window.py` in the `/frontend` directory.
Within in the interface you are able to select an approximation method. For now only _"Polynomial approximation to a degree of N"_ is implemented.
After selecting the method for approximation and entering the degree in the field on the left for _N_ you are able to draw the desired curve on the canvas.
Pressing **Approximate** will display you the calculated approximation right in the canvas. <br/> <br/>
![Unable to load gif](https://media0.giphy.com/media/DkgjjF0q90zIMrQlHC/giphy.gif) <br/> <br/>

If you are happy with the approximation you can export the result via a press of the **Export** button.
You will be prompted a dialogue on the **CLI** to configure the initial values of alpha and beta (not displayed in GIF below). We can comptutaionally infer a valid configuration, but with the libraries used, this takes forever except for linear function. If you are only interested in the linkage system and do not want it to be correctly restricted by a peaucellier linkage, you can enter arbitrary values within the first quadrant. If you entered both values (or left them empty) the export starts.
It will display an image of the calculated linkage system and print out the _mechanism expression_ that can then be imported into [pyslvs](https://github.com/KmolYuan/Pyslvs-UI) for simulation. 

![Unable to load gif](https://media1.giphy.com/media/43QvhD7KjjUSh5Lu4O/giphy.gif) <br/> <br/>


Importing a _mechanism expression_ in pyslvs is done through the usage of the **mechanism** -> **paste** option. <br/>
![Unable to load gif](https://media4.giphy.com/media/hosZqf5Ues8V7vkfNF/giphy.gif) <br/>
Hint: for the sake of clarity it is recommended to disable _point marks_ and reduce the width of all lines through the settings (F2)!
<br/> <br/>
The linakge system movement can be simulated by telling pyslvs what points should act as _base points_ and which should act as _driver points_ in the **inputs** menu.
The correct choice for these can be checked by looking the green & blue colored areas of the displayed linkage image from the earlier steps.


## Limitations
This chapter will list the currently known limitations of this program
* As of now the program is **work-in-progress** and might not function as intended
* There is only one approximation technique currently implemented (Polynomial approximation to the degree of N)
* A valid approximation can only be reached when choosing a degree of equal or less than 3 as the code currently doesn't handle squares of cos
* The initial configuration of alpha and beta must be within the first quadrant, otherwise inconsistencies may occure. This does not apply for any outputs of the linkage, only for the initial angles within the rhombus. There surely is a simple mathematical error somewhere, but we were not able to find it.
* Pyslvs is a qt-ui with underlying usage of a reduced solvespace-api, a lot of functions are unavailable and the physics simulation is bare bones
* Without switching to a different visualization / simulation tool the complete path of the output point will not be traced
* The output area (2*R) is not considered in pyslvs so we will most likely see the linkage breaking at some point
* ... probably a lot more we are not even thinking of at the moment

## Pre-view
Here is what a partially linear simulation might look like.
Keep in mind this is still only partially correct.
<br/> <br/>
![Unable to load gif](https://media1.giphy.com/media/UOakG4pqyalqeR8QvF/giphy.gif) <br/>
