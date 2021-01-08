import { clone, cloneDeep, keys, values } from "lodash";
import { Dictionary } from "../../typings";
import { any } from "./_common-functions";

// @dynamic
export class CxObjectUtil {
  public static keys(source: any, ignorePrivate: boolean = true): string[] {
    if (typeof source !== "object") {
      return [];
    }
    const result: string[] = [];
    for (const key in source) {
      if (
        typeof source[key] !== "function" &&
        (ignorePrivate === false || !key.startsWith("_"))
      ) {
        result.push(key);
      }
    }
    return result;
  }
  public static dictionaryMapTo<TSource, TTarget>(
    source: Dictionary<TSource>,
    mapCallback: (item: TSource) => TTarget
  ): Dictionary<TTarget> {
    const result: Dictionary<TTarget> = {};
    Object.keys(source).forEach(key => {
      result[key] = mapCallback(source[key]);
    });
    return result;
  }
  public static toJsonObj(source: any, ignorePrivate: boolean = true): any {
    if (typeof source !== "object") {
      return source;
    }
    if (source instanceof Array) {
      return source.map(p => CxObjectUtil.toJsonObj(p, ignorePrivate));
    }
    if (source instanceof Date) {
      return source;
    }
    const objResult = {};
    CxObjectUtil.keys(source, ignorePrivate).forEach(key => {
      objResult[key] = CxObjectUtil.toJsonObj(source[key], ignorePrivate);
    });
    return objResult;
  }
  public static clone<T>(
    value: T,
    updateClonedValueAction?: (clonedValue: T) => undefined | T
  ): T {
    if (value === undefined) {
      return value;
    }
    let clonedValue = clone(value);
    if (updateClonedValueAction !== undefined) {
      const updatedClonedValue = updateClonedValueAction(clonedValue);
      if (updatedClonedValue !== undefined) {
        clonedValue = updatedClonedValue;
      }
    }
    return clonedValue;
  }
  public static cloneWithNewValues<T extends object>(
    value: T,
    newValues: T | Partial<T>
  ): T {
    if (value === undefined) {
      return value;
    }
    const clonedValue = clone(value);
    Object.keys(newValues).forEach(newValueKey => {
      clonedValue[newValueKey] = newValues[newValueKey];
    });
    return clonedValue;
  }
  public static cloneDeep<T>(
    value: T,
    deepLevel?: number,
    updateClonedValueAction?: (clonedValue: T) => undefined | T | void
  ): T {
    if (value === undefined || typeof value !== "object") {
      return value;
    }

    let clonedValue = value;
    if (deepLevel === undefined) {
      clonedValue = cloneDeep(value);
    } else {
      cloneInsideRecursively(clonedValue, deepLevel);
    }

    if (updateClonedValueAction !== undefined) {
      const updatedClonedValue = updateClonedValueAction(clonedValue);
      if (updatedClonedValue !== undefined) {
        clonedValue = updatedClonedValue as T;
      }
    }

    return clonedValue;

    function cloneInsideRecursively(
      source: any,
      cloneDeepLevel: number,
      currentDeepLevel: number = 1
    ) {
      if (typeof source !== "object" || currentDeepLevel > cloneDeepLevel) {
        return;
      }
      CxObjectUtil.keys(source).map(key => {
        source[key] = clone(source[key]);
        cloneInsideRecursively(
          source[key],
          cloneDeepLevel,
          currentDeepLevel + 1
        );
      });
    }
  }

  public static getDictionaryKeys<T extends string | number>(
    object?: Dictionary<any>
  ): T[] {
    return keys(object).map(
      (key: any) => (!isNaN(key as any) ? parseInt(key, 0) : key) as T
    );
  }

  public static values<T>(
    object?: Dictionary<T> | ArrayLike<T> | undefined
  ): T[] {
    return values(object);
  }

  public static isDifferent<T>(
    value1: T,
    value2: T,
    shallowCheckFirstLevel: boolean = false
  ) {
    if (value1 === undefined && value2 === undefined) {
      return false;
    }
    if (value1 === undefined && value2 !== undefined) {
      return true;
    }
    if (value1 !== undefined && value2 === undefined) {
      return true;
    }
    if (typeof value1 !== "object" && typeof value2 !== "object") {
      return value1 !== value2;
    }
    if (value1 instanceof Array && value2 instanceof Array) {
      if (value1.length !== value2.length) {
        return true;
      }
    }
    const value1Keys = Object.keys(value1);
    const value2Keys = Object.keys(value2);
    if (value1Keys.length !== value2Keys.length) {
      return true;
    }
    if (shallowCheckFirstLevel) {
      return any(value1Keys, value1Key => {
        if (value1[value1Key] === value2[value1Key]) {
          return false;
        }
        if (
          typeof value1[value1Key] !== "object" &&
          typeof value2[value1Key] !== "object"
        ) {
          return true;
        }
        return (
          JSON.stringify(value1[value1Key]) !==
          JSON.stringify(value2[value1Key])
        );
      });
    }
    return JSON.stringify(value1) !== JSON.stringify(value2);
  }

  public static boxingFn<T>(fn?: (...args: any[]) => T, ...fnArgs: any[]) {
    return () => {
      return fn !== undefined ? fn(fnArgs) : undefined;
    };
  }

  public static assign<T extends object>(target: T, ...sources: Partial<T>[]) {
    sources.forEach(source => {
      CxObjectUtil.keys(source).forEach(sourceKey => {
        if (source[sourceKey] !== undefined) {
          target[sourceKey] = source[sourceKey];
        }
      });
    });

    return target;
  }

  public static extend<T extends object>(
    target: T,
    ...sources: Partial<T>[]
  ): T {
    sources.forEach(source => {
      CxObjectUtil.keys(source).forEach(sourceKey => {
        if (
          target[sourceKey] === undefined &&
          source[sourceKey] !== undefined
        ) {
          target[sourceKey] = source[sourceKey];
        }
      });
    });

    return target;
  }

  public static assignDeep<T extends object>(
    target: T,
    source: T,
    cloneSource: boolean = false
  ): T {
    return mapObject(target, source, cloneSource);
  }

  public static setDeep<T extends object>(
    target: T,
    source: T,
    cloneSource: boolean = false
  ): T {
    return mapObject(target, source, cloneSource, true);
  }

  public static getCurrentMissingItems<T>(
    prevValue: Dictionary<T>,
    currentValue: Dictionary<T>
  ): T[] {
    return CxObjectUtil.keys(prevValue)
      .filter(key => {
        return prevValue[key] !== undefined && currentValue[key] === undefined;
      })
      .map(key => prevValue[key]);
  }

  public static removeProps(obj: any, filterProp: (propValue: any) => boolean) {
    const result = Object.assign({}, obj);
    CxObjectUtil.keys(obj).forEach(key => {
      if (filterProp(obj[key])) {
        delete result[key];
      }
    });
    return result;
  }

  /*
   * Property route have template like: 'prop.childProp'
   */
  public static getPropertyValue(object: any, propertyRoute: string) {
    if (typeof object !== "object" || typeof propertyRoute !== "string") {
      return undefined;
    }
    const nestedProperties = propertyRoute.split(".");
    let finalValue;
    nestedProperties.forEach((prop, index) => {
      if (index === 0) {
        finalValue = object[prop];
      } else {
        finalValue = finalValue[prop];
      }
    });
    return finalValue;
  }

  public static getUniqueId(object: any, propertyRoutes: string[]) {
    if (typeof object !== "object") {
      return undefined;
    }
    const propertiesValue = [];
    propertyRoutes.forEach(propertyRoute => {
      propertiesValue.push(this.getPropertyValue(object, propertyRoute));
    });
    return propertiesValue.join("_");
  }

  public static setPropertyValue(
    object: any,
    propertyRoute: string,
    value: any
  ) {
    if (typeof object !== "object" || typeof propertyRoute !== "string") {
      return undefined;
    }
    const dotIndex = propertyRoute.indexOf(".");
    if (dotIndex >= 0) {
      const nestedObject = object[propertyRoute.substring(0, dotIndex)];
      const remainingPropertyRoute = propertyRoute.substr(dotIndex + 1);
      return this.setPropertyValue(nestedObject, remainingPropertyRoute, value);
    }
    object[propertyRoute] = value;
  }

  public static hasAnyProperty(object: any) {
    for (const prop in object) {
      if (Object.prototype.hasOwnProperty.call(object, prop)) {
        return true;
      }
    }
    return false;
  }
}

function mapObject<T extends object>(
  target: T,
  source: T,
  cloneSource: boolean = false,
  makeTargetValuesSameSourceValues: boolean = false
) {
  if (makeTargetValuesSameSourceValues) {
    removeTargetKeysNotInSource(target, source);
  }

  const sourceKeys = Object.keys(source);
  sourceKeys.forEach(key => {
    if (CxObjectUtil.isDifferent(target[key], source[key])) {
      if (mapObjectCheckTwoValueCanSetDirectly(target[key], source[key])) {
        target[key] = cloneSource
          ? CxObjectUtil.cloneDeep(source[key])
          : source[key];
      } else {
        target[key] = CxObjectUtil.clone(target[key]);

        if (target[key] instanceof Array && source[key] instanceof Array) {
          mapArray(
            target[key],
            source[key],
            cloneSource,
            makeTargetValuesSameSourceValues
          );
        } else {
          mapObject(
            target[key],
            source[key],
            cloneSource,
            makeTargetValuesSameSourceValues
          );
        }
      }
    }
  });

  return target;

  function mapObjectCheckTwoValueCanSetDirectly(
    targetValue: any,
    sourceValue: any
  ) {
    const isTargetArray = targetValue instanceof Array;
    const isSourceArray = sourceValue instanceof Array;
    if (
      targetValue === undefined ||
      sourceValue === undefined ||
      typeof targetValue !== "object" ||
      typeof sourceValue !== "object" ||
      (isTargetArray && !isSourceArray) ||
      (!isTargetArray && isSourceArray)
    ) {
      return true;
    }
    {
      return false;
    }
  }

  function mapArray(
    targetArray: any[],
    sourceArray: any[],
    cloneSourceArray = false,
    makeTargetValuesSameSource = false
  ) {
    if (targetArray.length > sourceArray.length && makeTargetValuesSameSource) {
      targetArray.splice(sourceArray.length);
    }

    for (let i = 0; i < sourceArray.length; i++) {
      if (makeTargetValuesSameSource) {
        if (CxObjectUtil.isDifferent(targetArray[i], sourceArray[i])) {
          targetArray[i] = cloneSourceArray
            ? CxObjectUtil.cloneDeep(sourceArray[i])
            : sourceArray[i];
        }
      } else {
        if (
          mapObjectCheckTwoValueCanSetDirectly(targetArray[i], sourceArray[i])
        ) {
          targetArray[i] = cloneSourceArray
            ? CxObjectUtil.cloneDeep(sourceArray[i])
            : sourceArray[i];
        } else {
          targetArray[i] = CxObjectUtil.clone(
            targetArray[i],
            newTargetArrayItem => {
              mapObject(
                newTargetArrayItem,
                sourceArray[i],
                cloneSourceArray,
                makeTargetValuesSameSource
              );
            }
          );
        }
      }
    }
  }
}

function removeTargetKeysNotInSource<T extends object>(target: T, source: T) {
  if (target === undefined || source === undefined) {
    return;
  }

  const targetKeys = CxObjectUtil.keys(target);
  const sourceKeys = CxObjectUtil.keys(source);

  targetKeys.forEach(key => {
    if (sourceKeys.indexOf(key) < 0) {
      delete target[key];
    }
  });
}

export class ValueWrapper<TValue> {
  constructor(public value: TValue) {}

  public map<TMapValue>(
    func: (value: TValue) => TMapValue | ValueWrapper<TMapValue>
  ): ValueWrapper<TMapValue> {
    const funcValue = func(this.value);
    if (funcValue instanceof ValueWrapper) {
      return new ValueWrapper(funcValue.value);
    }
    return new ValueWrapper(funcValue);
  }
}
