import {importPlates} from "./Util/PlateImporter";
import {BfsEdgeUnfolder, BruteForceEdgeUnfolder, DfsEdgeUnfolder, SteepestEdgeUnfolder} from "./Nesting/EdgeUnfolder";


// import plates
console.log("Starting now");
const plates = importPlates('imports/chair.json');

// unfold
new DfsEdgeUnfolder().nest(plates).saveSvg('dfs.svg');
new BfsEdgeUnfolder().nest(plates).saveSvg('bfs.svg');
new BruteForceEdgeUnfolder().nest(plates).saveSvg('bruteForce.svg');
new SteepestEdgeUnfolder().nest(plates).saveSvg('steepest.svg');

console.log("Finished successfully");