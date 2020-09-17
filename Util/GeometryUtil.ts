import Polygon from "../Model/Polygon";
import * as THREE from 'three';
import Util from "./Util";

export default class GeometryUtil {

  /** Computation methods */
  public static distanceOf2d(vec1: THREE.Vector2, vec2: THREE.Vector2): number {
    const a = vec1.x - vec2.x;
    const b = vec1.y - vec2.y;
    return Math.sqrt(a * a + b * b);
  }

  public static distanceOf3d(vec1: THREE.Vector3, vec2: THREE.Vector3): number {
    const a = vec1.x - vec2.x;
    const b = vec1.y - vec2.y;
    const c = vec1.z - vec2.z;
    return Math.sqrt(a * a + b * b + c * c);
  }

  // returns x in "vec = x * base"
  public static getNeededScalar(vec: THREE.Vector3, base: THREE.Vector3): number {
    const [x, y, z] = [Math.abs(base.x), Math.abs(base.y), Math.abs(base.z)];
    const max = Math.max(x, y, z);
    if (max === x) return vec.x / base.x;
    if (max === y) return vec.y / base.y;
    return vec.z / base.z;
  }

  // angle in degrees (0 - 359)
  public static angleBetween(vec1: THREE.Vector3, vec2: THREE.Vector3): number {
    const radians = vec1.angleTo(vec2);
    const degrees = radians * 180 / Math.PI;
    return (720 + degrees) % 360;
  }

  public static vectorsParallel(vec1: THREE.Vector3, vec2: THREE.Vector3): boolean {
    const angle = GeometryUtil.angleBetween(vec1, vec2);
    return Util.eq(angle % 180, 0);
  }

  /**
   * The following 2 methods help to create transformation matrices. Matrices
   * are combined in the order that we like to think about transformations.
   * I.e. calling makeRotationFromVectorsAroundNormal and then
   * makeTranslation will lead to a transformation matrix that first scales
   * and then translates, whereas the mathematically correct order would be
   * translationMatrix * scaleMatrix.
   */
  public static makeTranslation(vec: THREE.Vector2, matrix: THREE.Matrix4): void {
    const helperMatrix = new THREE.Matrix4().makeTranslation(vec.x, vec.y, 0);
    matrix.premultiply(helperMatrix);
  }

  public static makeRotationFromVectorsAroundNormal(source: THREE.Vector2, target: THREE.Vector2, normal: THREE.Vector3, matrix: THREE.Matrix4): void{

    const quaternion = GeometryUtil.fromVectorsAroundNormal(source, target, normal);
    const rotationMatrix = new THREE.Matrix4().makeRotationFromQuaternion(quaternion);
    matrix.premultiply(rotationMatrix);
  }

  private static fromVectorsAroundNormal(source2d: THREE.Vector2, target2d: THREE.Vector2, normal: THREE.Vector3): THREE.Quaternion {

    const quaternion = new THREE.Quaternion()
    const source = new THREE.Vector3(source2d.x, source2d.y, 0);
    const target = new THREE.Vector3(target2d.x, target2d.y, 0);

    if (Util.eq(source.manhattanLength(), 0) || Util.eq(target.manhattanLength(), 0)) {
      return new THREE.Quaternion();
    }

    normal = normal.normalize();
    source.normalize();
    target.normalize();

    const signedAngle = GeometryUtil.angleBetweenTwoVectors(source, target, normal);

    quaternion.setFromAxisAngle(normal, signedAngle);
    quaternion.normalize();

    return quaternion;
  }

  private static angleBetweenTwoVectors(a: THREE.Vector3, b: THREE.Vector3, direction: THREE.Vector3): number {
    return Math.atan2(a.clone().cross(b).dot(direction), a.dot(b));
  }


  /**
   * Returns a new Polygon object, the original one remains intact.
   */
  public static applyMatrix4ToPolygon(polygon: Polygon, matrix: THREE.Matrix4): Polygon {
    const points = polygon.getPoints().map(p => this.applyMatrix4(p, matrix));
    const holes = polygon.getHoles().map(hole => hole.map(p => GeometryUtil.applyMatrix4(p, matrix)));

    return new Polygon(points, holes);
  }


  /**
   * The following methods are used to apply Vector3-Methods to Vector2 objects
   *
   * Returns a new Vector2 object, the original one remains intact.
   */
  public static applyMatrix4(vec2: THREE.Vector2, matrix: THREE.Matrix4): THREE.Vector2 {
    return GeometryUtil.applyOperation(vec2, matrix, "applyMatrix4");
  }

  public static transformDirection(vec2: THREE.Vector2, matrix: THREE.Matrix4): THREE.Vector2 {
    return GeometryUtil.applyOperation(vec2, matrix, "transformDirection");
  }

  private static applyOperation(vec2: THREE.Vector2, matrix: THREE.Matrix4, functionName: string): THREE.Vector2 {
    const vec3 = new THREE.Vector3(vec2.x, vec2.y, 0);
    vec3[functionName](matrix);
    const newVec2 = new THREE.Vector2(vec3.x, vec3.y);

    if (!Util.eq(vec3.z, 0)) throw new Error("Should have empty z component");
    return newVec2;
  }
}