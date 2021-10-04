#!/bin/python3

from Util.importer import import_plates
from edge_unfolder import SteepestEdgeUnfolder

plates = import_plates("imports/boxel.json")
	
	# DfsEdgeUnfolder.nest(plates).saveSvg('dfs.svg');
# // new BfsEdgeUnfolder().nest(plates).saveSvg('bfs.svg');
# // new BruteForceEdgeUnfolder().nest(plates).saveSvg('bruteForce.svg');

# TODO check what's up with plates
SteepestEdgeUnfolder.nest(list(plates)).save_svg("steepest.svg");

print("Finished successfully")