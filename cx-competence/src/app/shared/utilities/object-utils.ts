import { environment } from 'app-environments/environment';
import * as _ from 'lodash';
import { isEmpty, isEqual } from 'lodash';

export class ObjectUtilities {
  public static copy(obj, include = [], exclude = []) {
    return Object.keys(obj).reduce((target, k) => {
      if (exclude.length) {
        if (exclude.indexOf(k) < 0) {
          target[k] = obj[k];
        }
      } else if (include.indexOf(k) > -1) {
        target[k] = obj[k];
      }
      return target;
    }, {});
  }

  public static isEmpty(obj) {
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        return false;
      }
    }
    return true;
  }

  public static getDiffs(objA, objB, fields: string[]) {
    if (!objA || !objB) {
      return;
    }
    const temp = {};
    if (!fields || !fields.length) {
      return;
    }
    Object.keys(objA).forEach((key) => {
      const nestedA = objA[key];
      const nestedB = objB[key];
      if (!fields.includes(key) || (!nestedA && !nestedB)) {
        return;
      }
      if (Array.isArray(nestedA) || Array.isArray(nestedB)) {
        if (!isEqual(nestedA && nestedA.sort(), nestedB && nestedB.sort())) {
          temp[key] = { previous: nestedA, current: nestedB };
          return;
        }
      }
      if (typeof nestedA === 'object' && typeof nestedB === 'object') {
        const nestedDiff = this.getDiffs(nestedA, nestedB, fields);
        if (!ObjectUtilities.isEmpty(nestedDiff)) {
          temp[key] = nestedDiff;
        }
        return;
      }
      if (nestedA === nestedB) {
        return;
      }
      temp[key] = { previous: nestedA, current: nestedB };
    });
    return temp;
  }

  public static getPropLocalizedData(
    localizedDatas: any,
    propName: string,
    languageCode: string
  ): string {
    const localizedDataItem = localizedDatas.find(
      (x) => x.languageCode === languageCode
    );
    const fields =
      localizedDataItem !== undefined
        ? localizedDataItem.fields
        : localizedDatas.find(
            (x) => x.languageCode === environment.fallbackLanguage
          );
    for (const field of fields) {
      if (field.name === propName) {
        return field.localizedText;
      }
    }

    return undefined;
  }

  // On srcObject field name under format like: {'extensions.name': 'John'}
  // this function will process to {'extensions': {'name': 'John'}}
  static fieldWithDotToObject(srcObject: object): any {
    const targetObject = {};
    const allPropertiesInJson = Object.getOwnPropertyNames(srcObject);
    allPropertiesInJson.forEach((property) => {
      const value = srcObject[property];
      ObjectUtilities.setPropertyValue(targetObject, property, value);
    });

    return targetObject;
  }

  // On srcObject field name under format like: {'extensions': {'name': 'John'}}
  // this function will process to {'extensions.name': 'John'}
  static objectToFieldWithDot(srcObject: object, processKeys?: string[]): any {
    const targetObject = {};
    ObjectUtilities.objectToFieldWithDotRecursive(
      srcObject,
      targetObject,
      processKeys
    );

    return targetObject;
  }

  static objectToFieldWithDotRecursive(
    srcObject: object,
    targetObject: object,
    processKeys?: string[],
    parentProperty?: string
  ): any {
    for (const key in srcObject) {
      if (!srcObject.hasOwnProperty(key)) {
        continue;
      }
      const notObject = typeof srcObject[key] !== 'object';
      const notProcessKey = !processKeys || !processKeys.includes(key);
      const isArray = Array.isArray(srcObject[key]);
      if (notObject || isArray || notProcessKey) {
        if (parentProperty) {
          targetObject[parentProperty + '.' + key] = srcObject[key];
        } else {
          targetObject[key] = srcObject[key];
        }
      } else {
        const newPath = parentProperty ? parentProperty + '.' + key : key;
        ObjectUtilities.objectToFieldWithDotRecursive(
          srcObject[key],
          targetObject,
          processKeys,
          newPath
        );
      }
    }
  }

  // Set new field and valu for object
  public static setPropertyValue(
    obj: object,
    propertyRoute: string,
    value: any
  ): void {
    if (typeof obj !== 'object' || typeof propertyRoute !== 'string') {
      return;
    }
    const dotIndex = propertyRoute.indexOf('.');
    if (dotIndex >= 0) {
      const fieldName = propertyRoute.substring(0, dotIndex);
      if (!obj[fieldName]) {
        obj[fieldName] = {};
      }
      const remainingPropertyRoute = propertyRoute.substr(dotIndex + 1);

      return this.setPropertyValue(
        // tslint:disable-next-line: no-unsafe-any
        obj[fieldName],
        remainingPropertyRoute,
        value
      );
    }
    obj[propertyRoute] = value;
  }

  public static removeUndefinedFields<T>(object: T): T {
    for (const key in object) {
      if (object.hasOwnProperty(key)) {
        const value = object[key];
        if (value === undefined) {
          delete object[key];
        }
      }
    }

    return object;
  }

  public static clone<T>(object: T): T {
    return JSON.parse(JSON.stringify(object));
  }

  static copyPartialObject<T>(src: Partial<T>, target: T): T {
    for (const key in src) {
      if (src.hasOwnProperty(key)) {
        if (src[key] !== undefined) {
          target[key] = src[key];
        }
      }
    }

    return target;
  }

  static flattenArray<T>(
    array: T[],
    childrenFieldName: string = 'children',
    skipParent: boolean = false,
    flattenArrayParam?: T[]
  ): T[] {
    const flattenArray: T[] = flattenArrayParam ?? [];
    for (const item of array) {
      const itemClone = _.clone(item);
      const childrenField = _.clone(itemClone[childrenFieldName]);
      const hasChildren = !isEmpty(itemClone[childrenFieldName]);

      if (!skipParent || !hasChildren) {
        delete itemClone[childrenFieldName];
        flattenArray.push(itemClone);
      }

      if (hasChildren) {
        ObjectUtilities.flattenArray(
          childrenField,
          childrenFieldName,
          skipParent,
          flattenArray
        );
      }
    }

    return flattenArray;
  }
}
