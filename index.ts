import {importPlates} from "./Util/PlateImporter";
import {BfsEdgeUnfolder, BruteForceEdgeUnfolder, DfsEdgeUnfolder, SteepestEdgeUnfolder} from "./Nesting/EdgeUnfolder";
import {StarUnfolder} from "./Nesting/GeneralUnfolder";


// import plates
console.log("Starting now");
const plates = importPlates('imports/boxel.json');

// unfold
/* new DfsEdgeUnfolder().nest(plates).saveSvg('dfs.svg');
new BfsEdgeUnfolder().nest(plates).saveSvg('bfs.svg');
new BruteForceEdgeUnfolder().nest(plates).saveSvg('bruteForce.svg');
new SteepestEdgeUnfolder().nest(plates).saveSvg('steepest.svg'); */

// new SteepestEdgeUnfolder().nest(plates).saveSvg('steepest.svg');
new StarUnfolder().nest(plates).saveSvg('star.svg');

console.log("Finished successfully");