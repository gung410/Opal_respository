import * as fs from 'fs';
// tslint:disable:no-any

/**
 * @public
 */
export class Utilities {
  public static getFilePathIfExist(path: string): string | undefined {
    if (fs.existsSync(path)) {
      return path;
    }

    return undefined;
  }
  public static isNotNullOrUndefined(value: any): boolean {
    if (value === null || value === undefined) {
      return false;
    }

    return true;
  }
  public static isNonemptyString(value: string): boolean {
    if (Utilities.isNotNullOrUndefined(value) && value.length > 0) {
      return true;
    }

    return false;
  }
}
