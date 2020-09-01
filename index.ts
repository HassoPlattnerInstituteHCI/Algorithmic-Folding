import { importPlates } from "./Util/PlateImporter";
import { DfsEdgeUnfolder, BfsEdgeUnfolder, BruteForceEdgeUnfolder, SteepestEdgeUnfolder } from "./Nesting/EdgeUnfolder";

// TODO: 
// - import models (done)
// - create plate graph (done)
// - create data structure to do joint-placement with (done)
// - choose any spanning tree
// - unfold (i.e. place plates joint-adjacent)
// - check for overlaps (done)
// - export an array of polygons as svg (done)

// import plates
console.log("Starting now");
const plates = importPlates('imports/chair.json');

// unfold
// new DfsEdgeUnfolder().nest(plates).saveSvg('dfs.svg');
// new BfsEdgeUnfolder().nest(plates).saveSvg('bfs.svg');
// new BruteForceEdgeUnfolder().nest(plates).saveSvg('bruteForce.svg');
new SteepestEdgeUnfolder().nest(plates).saveSvg('steepest.svg');

console.log("Finished successfully");