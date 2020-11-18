import {importPlates} from "./Util/PlateImporter";
import {BfsEdgeUnfolder, BruteForceEdgeUnfolder, DfsEdgeUnfolder, SteepestEdgeUnfolder} from "./Nesting/EdgeUnfolder";
import StripUnfolder from "./Nesting/StripUnfolder/StripUnfolder";


// import plates
console.log("Starting now");
const plates = importPlates('imports/boxel.json');

// unfold
new DfsEdgeUnfolder().nest(plates).saveSvg('dfs.svg');
new BfsEdgeUnfolder().nest(plates).saveSvg('bfs.svg');
new BruteForceEdgeUnfolder().nest(plates).saveSvg('bruteForce.svg');
new SteepestEdgeUnfolder().nest(plates).saveSvg('steepest.svg');
new StripUnfolder().nest(plates).saveSvg('strip.svg');

console.log("Finished successfully");