import SvgCreator from "../Util/SvgCreator";
import Polygon from "../Model/Polygon";
import Unfolding from "./Unfolding";
import * as THREE from "three";
import * as fs from 'fs';


export default class MultiUnfolding {

    // how much space to leave between two unfoldings and to the page outline
    private readonly gap = 25;

    private readonly height: number = 0;
    private readonly width: number = 0;

    // the "flattened" collection of polygons from the unfoldings
    private polygons: Polygon[] = [];

    constructor(unfoldings: Unfolding[]) {
        let currentX = this.gap;

        for (const unfolding of unfoldings) {
            // how much to move each point
            const center = unfolding.getCenter();
            const height = unfolding.getHeight();
            const width = unfolding.getWidth();

            const newPos = new THREE.Vector2(currentX + (width / 2), this.gap + (height / 2));
            const disp = newPos.clone().sub(center);

            // create a clone of the polygons and move them by disp
            const polys = unfolding.getPolygons().map(poly => {
                const points = poly.getPoints().map(p => (new THREE.Vector2(p.x, p.y)).add(disp));
                const holes = poly.getHoles().map(hole => hole.map(p => (new THREE.Vector2(p.x, p.y)).add(disp)));
                return new Polygon(points, holes);
            });

            // then add them to the list and change the x position
            this.polygons.push(...polys);
            currentX += width + this.gap;

            // finally, set the new height (if bigger)
            this.height = Math.max(this.height, height + 2 * this.gap);
        }

        this.width = currentX;
    }

    // export to svg
    public saveSvg(fileName: string) {

        const svgText = SvgCreator.getSvg(this.polygons, this.width, this.height);
        fs.writeFileSync(fileName, svgText);
    }
}