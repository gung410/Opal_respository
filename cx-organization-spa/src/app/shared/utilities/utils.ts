import {
  assignIn,
  clone,
  cloneDeep,
  difference,
  differenceWith,
  escape,
  flatMap,
  flow,
  isEmpty,
  isEqual,
  keyBy,
  keys,
  orderBy,
  remove,
  uniq
} from 'lodash';
import {
  interval,
  MonoTypeOperatorFunction,
  Observable,
  of,
  pipe,
  Subscription
} from 'rxjs';
import { delay, takeUntil } from 'rxjs/operators';

import { v4 as uuid } from 'uuid';

type List<T> = ArrayLike<T>;

export declare interface Dictionary<T> {
  [index: string]: T;
}

export declare interface NumericDictionary<T> {
  [index: number]: T;
}
/**
 * Utilities for the whole application.
 * The reason why we have this class:
 *  1. We want to leverage lodash APIs (don't reinvent the wheel)!
 *  2. We want to control exactly which APIs of lodash is allowed to use
 *  3. Don't import the whole lodash library (for tre-shaking purpose)
 * If you need any new methods from lodash, please ask teamlead before adding it.
 */

// @dynamic
export class Utils {
  /**
   * Check the value is defined, only comparing with undefined or null
   */
  static isDefined<T>(value: T): boolean {
    return value != null && value !== null;
  }

  /**
   * Checks if value is an empty object, collection, map, or set.
   * Refer https://lodash.com/docs/4.17.10#isEmpty
   */
  static isEmpty<T>(value: T): boolean {
    return isEmpty(value);
  }

  /**
   * This method is like _.assign except that it iterates over own and inherited source properties.
   * https://lodash.com/docs/4.17.10#assignIn
   */
  static assignIn<U, V>(object: U, other: V): U & V {
    return assignIn(object, other) as U & V;
  }

  /**
   * Base64 encoding
   */
  static encodeBase64(value: string): string {
    return window.btoa(value);
  }

  /**
   * Base64 decoding
   */
  static decodeBase64(value: string): string {
    return window.atob(value);
  }

  static serializeJSON<T>(value: T): string {
    return JSON.stringify(value);
  }

  static deserializeJSON<T>(input: string): T {
    return JSON.parse(input) as T;
  }

  /**
   * Creates a shallow clone of value.
   *
   * Note: This method is loosely based on the structured clone algorithm and supports cloning arrays, array buffers, booleans, date
   * objects, maps, numbers, Object objects, regexes, sets, strings, symbols, and typed arrays. The own enumerable properties of arguments
   * objects are cloned as plain objects. An empty object is returned for uncloneable values such as error objects, functions, DOM nodes,
   * and WeakMaps.
   * https://lodash.com/docs/4.17.15#clone
   */
  static clone<T>(
    value: T,
    updateClonedValueFn?: (clonedValue: T) => undefined | T | void
  ): T {
    if (value == null) {
      return value;
    }
    let clonedValue: T = clone(value);
    if (updateClonedValueFn != null) {
      const updatedClonedValue: undefined | T | void = updateClonedValueFn(
        clonedValue
      );
      if (updatedClonedValue != null) {
        clonedValue = updatedClonedValue as T;
      }
    }
    return clonedValue;
  }

  /**
   * This method is like _.clone except that it recursively clones value.
   * https://lodash.com/docs/4.17.15#cloneDeep
   */
  static cloneDeep<T>(value: T): T {
    return value == null ? null : cloneDeep<T>(value);
  }

  /**
   * Removes all elements from array that predicate returns truthy for and returns an array of the removed elements. The predicate is
   * invoked with three arguments: (value, index, array).
   *
   * Note: Unlike _.filter, this method mutates array.
   * https://lodash.com/docs/4.17.15#remove
   */
  static remove<T>(
    array: ArrayLike<T>,
    predicate?: (value: T, index: number, collection: ArrayLike<T>) => boolean
  ): T[] {
    return remove<T>(array, predicate);
  }

  /**
   * Create an array of number from start to end (including)
   */
  static range(start: number, end: number): number[] {
    const result: number[] = [];
    for (let i: number = start; i <= end; i++) {
      result.push(i);
    }
    return result;
  }

  static move<T>(
    arr: T[],
    itemPredicate: (value: T, index: number, collection: T[]) => boolean,
    newIndex: (itemIndex: number) => number,
    circleMoving: boolean = false
  ): void {
    const itemIndex = arr.findIndex(itemPredicate);
    if (itemIndex < 0) {
      return;
    }

    let newIndexValue = newIndex(itemIndex);
    if (newIndexValue < 0) {
      newIndexValue = circleMoving ? arr.length - 1 : 0;
    } else if (newIndexValue >= arr.length) {
      newIndexValue = circleMoving ? 0 : arr.length - 1;
    }
    arr.splice(newIndexValue, 0, arr.splice(itemIndex, 1)[0]);
  }

  static orderBy<T>(
    collection: T[],
    iteratees: (
      value: T,
      index: number,
      collection: ArrayLike<T>
    ) => number | string | object,
    desc: boolean = false
  ): T[] {
    return orderBy(collection, iteratees, desc ? 'desc' : 'asc');
  }

  static flatMap<T>(
    collection: Dictionary<T[]> | NumericDictionary<T[]> | null | undefined
  ): T[] {
    return flatMap(collection);
  }

  static uniq<T>(array: List<T> | null | undefined): T[] {
    return uniq(array);
  }

  // tslint:disable-next-line:no-any
  static flow<A extends any[], R1, R2>(
    f1: (...args: A) => R1,
    f2: (a: R1) => R2
  ): (...args: A) => R2 {
    return flow(f1, f2);
  }

  static debounce(
    func: (...args: Array<unknown>) => void,
    wait: number
  ): (...args: Array<unknown>) => void {
    if (wait <= 0) {
      return func;
    }

    let timeout: number;
    return (...args: Array<unknown>) => {
      const context = this;

      const executeFunction = () => {
        func.apply(context, args);
      };

      clearTimeout(timeout);
      timeout = (setTimeout(executeFunction, wait) as unknown) as number;
    };
  }

  static differenceWith<T>(array: T[], comparedArray: T[]): T[] {
    return differenceWith(array, comparedArray, isEqual);
  }

  static delay(
    callback: () => void,
    delayTime?: number,
    cancelOn$?: Observable<unknown>
  ): Subscription {
    if (typeof delayTime === 'number' && delayTime === 0) {
      callback();
      return new Subscription();
    } else {
      const delayObs = pipe(
        cancelOn$ != null
          ? takeUntil(cancelOn$)
          : (obs: Observable<unknown>) => obs,
        delay(delayTime == null ? 10 : delayTime)
      );
      return delayObs(of(undefined)).subscribe(() => callback());
    }
  }
  /**
   * This function create a new temporary new UUID. This is not guarantee unique. Use it only for front-end.
   */
  static createUUID(): string {
    return uuid();
  }

  static toDictionary<T>(
    collection: ArrayLike<T> | undefined,
    dictionaryKeySelector?: (item: T) => string | number
  ): Dictionary<T> {
    const defaultKeySelector: (item: T) => string | number = (p) =>
      p.toString();
    return keyBy(
      collection,
      dictionaryKeySelector != null ? dictionaryKeySelector : defaultKeySelector
    );
  }

  static toDictionarySelect<T, TValueSelect>(
    collection: ArrayLike<T> | undefined,
    dictionaryKeySelector: (item: T) => string | number,
    dictionaryValueSelector: (item: T) => TValueSelect
  ): Dictionary<TValueSelect> {
    const result: Dictionary<TValueSelect> = {};
    for (let i = 0; i < collection.length; i++) {
      result[dictionaryKeySelector(collection[i])] = dictionaryValueSelector(
        collection[i]
      );
    }
    return result;
  }

  static isDifferent<T>(value1: T, value2: T): boolean {
    if (value1 == null && value2 == null) {
      return false;
    }
    if (value1 == null && value2 != null) {
      return true;
    }
    if (value1 != null && value2 == null) {
      return true;
    }
    if (typeof value1 !== 'object' && typeof value2 !== 'object') {
      return value1 !== value2;
    }
    if (value1 instanceof Array && value2 instanceof Array) {
      if (value1.length !== value2.length) {
        return true;
      }
    }
    return JSON.stringify(value1) !== JSON.stringify(value2);
  }

  static isEqual<T>(value1: T, value2: T): boolean {
    return !Utils.isDifferent(value1, value2);
  }

  /**
   * Replace the item, return new updated list.
   */
  static replaceOne<T>(
    collection: T[],
    replaceItem: T,
    condition: (item: T) => boolean
  ): T[] {
    const clonedCollection = Utils.clone(collection);
    for (let i = 0; i < clonedCollection.length; i++) {
      if (condition(clonedCollection[i])) {
        clonedCollection[i] = replaceItem;
        return clonedCollection;
      }
    }
    return collection;
  }

  static addOrReplace<T>(
    collection: T[] | undefined,
    item: T,
    replaceCondition: (item: T) => boolean
  ): T[] | undefined {
    if (collection == null) {
      return collection;
    }
    const clonedCollection = Utils.clone(collection);
    for (let i = 0; i < clonedCollection.length; i++) {
      if (replaceCondition(clonedCollection[i])) {
        clonedCollection[i] = item;
        return clonedCollection;
      }
    }

    clonedCollection.push(item);
    return clonedCollection;
  }

  static includesAll<T>(superset: T[], subset: T[]): boolean {
    return difference(subset, superset).length === 0;
  }

  static includesAny<T>(superset: T[], subset: T[]): boolean {
    for (let i = 0; i < subset.length; i++) {
      const subsetItem = subset[i];
      if (superset.indexOf(subsetItem) >= 0) {
        return true;
      }
    }
    return false;
  }

  static mapToArray<T>(map: Map<string, T>): T[] {
    return Array.from(map.values());
  }

  static hasDuplicatedItems<T, TCheckBy>(
    collection: T[],
    checkBySelector: (item: T) => TCheckBy
  ): boolean {
    for (let i = 0; i < collection.length; i++) {
      const item = collection[i];
      if (
        collection
          .slice(i + 1)
          .map(checkBySelector)
          .findIndex((p) => p === checkBySelector(item)) >= 0
      ) {
        return true;
      }
    }
    return false;
  }

  static toDictionaryGroupBy<T>(
    collection: ArrayLike<T> | undefined,
    dictionaryGroupByKeySelector: (item: T) => string | number
  ): Dictionary<T[]> {
    if (collection == null) {
      return {};
    }
    const result: Dictionary<T[]> = {};
    for (let i = 0; i < collection.length; i++) {
      const item = collection[i];
      if (result[dictionaryGroupByKeySelector(item)] == null) {
        result[dictionaryGroupByKeySelector(item)] = [];
      }
      result[dictionaryGroupByKeySelector(item)].push(item);
    }
    return result;
  }

  static any<T>(
    collection: ArrayLike<T> | undefined,
    predicate: (item: T) => boolean
  ): boolean {
    if (collection == null) {
      return false;
    }
    for (let i = 0; i < collection.length; i++) {
      const element = collection[i];
      if (predicate(element)) {
        return true;
      }
    }
    return false;
  }

  static selectMany<T, S>(
    collection: T[] | undefined,
    selectCallback: (item: T) => S[]
  ): S[] {
    if (collection == null || collection.length === 0) {
      return [];
    }
    const listOfChildList = collection.map(selectCallback);
    return listOfChildList.reduce((prevValue, currentValue) =>
      prevValue.concat(currentValue)
    );
  }

  static isAbsoluteUrl(url: string): boolean {
    // ref to: https://stackoverflow.com/questions/10687099/how-to-test-if-a-url-string-is-absolute-or-relative
    const r = new RegExp('^(?:[a-z]+:)?//', 'i');
    return r.test(url);
  }

  static getHrefFromAnchor(aTag: HTMLAnchorElement): string {
    const html = aTag.outerHTML;
    const matches = /href=[\'"]?([^\'" >]+)/.exec(html);
    if (matches.length === 2) {
      // get url in href
      const href = matches[1];
      return href;
    }

    return '';
  }

  static isInternalUrl(targetUrl: string): boolean {
    const urlTemp = document.createElement('a');
    urlTemp.href = targetUrl;
    return (
      this.getPureDomain(urlTemp.host) === this.getPureDomain(location.host)
    );
  }

  static getPureDomain(url: string): string {
    return url.startsWith('www.') ? url.slice(4) : url;
  }

  static escapeHtml(htmlString: string): string {
    return escape(htmlString);
  }

  // tslint:disable-next-line:no-any
  static getDictionaryKeys<T extends string | number>(
    object?: Dictionary<any>
  ): T[] {
    // tslint:disable-next-line:no-any
    return keys(object).map(
      (key: any) => (!isNaN(key as any) ? parseInt(key, undefined) : key) as T
    );
  }

  static assign<T extends object>(target: T, ...sources: Array<Partial<T>>): T {
    sources.forEach((source) => {
      Utils.keys(source).forEach((sourceKey) => {
        if (source[sourceKey] != null) {
          // Catch this to prevent can not set get only prop
          try {
            target[sourceKey] = source[sourceKey];
          } catch (error) {
            // Not throw error
          }
        }
      });
    });

    return target;
  }

  // tslint:disable-next-line:no-any
  static keys(
    source: any,
    ignorePrivate: boolean = true,
    ignoreNull: boolean = false
  ): string[] {
    if (typeof source !== 'object') {
      return [];
    }
    const result: string[] = [];
    for (const key in source) {
      if (
        typeof source[key] !== 'function' &&
        (ignorePrivate === false || !key.startsWith('_')) &&
        (ignoreNull === false || source[key] != null)
      ) {
        result.push(key);
      }
    }
    return result;
  }

  static upsertDic<T>(
    currentData: Dictionary<T>,
    newData: Dictionary<Partial<T>> | Array<Partial<T>>,
    getItemKey: (item: T | Partial<T>) => string | number,
    // tslint:disable-next-line:no-any
    initItem: (data: any) => T,
    removeNotExistedItems?: boolean,
    removeNotExistedItemsFilter?: (item: Partial<T>) => boolean,
    replaceEachItem?: boolean,
    // tslint:disable-next-line:no-any
    onHasNewStateDifferent?: (newState: Dictionary<T>) => any,
    optionalProps: Array<keyof T> = []
  ): Dictionary<T> {
    return modifyDic(currentData, (newState) => {
      const newDataDic =
        newData instanceof Array
          ? Utils.toDictionary(newData, (x) => getItemKey(x))
          : newData;
      if (removeNotExistedItems) {
        removeNotExistedItemsInNewData(newState, newDataDic);
      }

      Utils.getDictionaryKeys(newDataDic).forEach((id) => {
        if (
          newState[id] == null ||
          newDataDic[id] == null ||
          typeof newDataDic[id] !== 'object' ||
          typeof newState[id] !== 'object'
        ) {
          newState[id] = initItem(newDataDic[id]);
        } else {
          const prevNewStateItem = newState[id];
          const newStateItemData = replaceEachItem
            ? newDataDic[id]
            : Utils.assign<Partial<T>>(
                Utils.clone(newState[id]),
                newDataDic[id]
              );
          if (optionalProps.length > 0) {
            optionalProps.forEach((optionalProp) => {
              if (
                prevNewStateItem[optionalProp] != null &&
                newStateItemData[optionalProp] == null
              ) {
                newStateItemData[optionalProp] = prevNewStateItem[optionalProp];
              }
            });
          }
          newState[id] = initItem(newStateItemData);
        }
      });
    });

    function removeNotExistedItemsInNewData(
      state: Dictionary<Partial<T>>,
      newDataDic: Dictionary<Partial<T>>
    ): void {
      const removeItemIds = Utils.getDictionaryKeys(state).filter(
        (id) =>
          newDataDic[id] == null &&
          (removeNotExistedItemsFilter == null ||
            removeNotExistedItemsFilter(state[id]))
      );
      removeItemIds.forEach((id) => {
        delete state[id];
      });
    }

    function modifyDic(
      state: Dictionary<T>,
      modifyDicAction: (state: Dictionary<T>) => void | Dictionary<T>
    ): Dictionary<T> {
      const newState = Utils.clone(state);
      const modifiedState = modifyDicAction(newState);
      if (modifiedState === state) {
        return state;
      }
      if (Utils.isDifferent(state, newState)) {
        if (onHasNewStateDifferent != null) {
          onHasNewStateDifferent(newState);
        }
        return newState;
      }
      return state;
    }
  }

  static isNullOrEmpty(value: any | any[]): boolean {
    if (value instanceof Array) {
      return value.length === 0;
    }

    return value == null || value.toString().trim() === '';
  }

  static doInterval(
    callback: (intervalSubscriber: Subscription) => unknown,
    ms: number,
    maximumCount?: number,
    // tslint:disable-next-line:no-any
    ...pipeOps: Array<MonoTypeOperatorFunction<any>>
  ): Subscription {
    const intervalSubscriber = Utils.interval(
      callback,
      ms,
      maximumCount,
      ...pipeOps
    );
    callback(intervalSubscriber);
    return intervalSubscriber;
  }

  static interval(
    callback: (intervalSubscriber: Subscription) => unknown,
    ms: number,
    maximumCount?: number,
    // tslint:disable-next-line:no-any
    ...pipeOps: Array<MonoTypeOperatorFunction<any>>
  ): Subscription {
    let count = 1;
    let intervalObs = interval(ms);
    if (pipeOps != null) {
      intervalObs = pipeOps.reduce((obs, currentPipeOp) => {
        return obs.pipe(currentPipeOp);
      }, intervalObs);
    }
    const intervalSubscriber = intervalObs.subscribe(() => {
      callback(intervalSubscriber);
      if (maximumCount != null && maximumCount <= count) {
        intervalSubscriber.unsubscribe();
      }
      count++;
    });
    return intervalSubscriber;
  }

  static rightJoin<T extends string | number>(left: T[], right: T[]): T[] {
    const rightDic = Utils.toDictionary(right, (p) => p);
    return left ? left.filter((p) => rightDic[p] != null) : [];
  }

  static rightJoinSingle<T extends string | number>(
    left: T,
    right: T[]
  ): T | undefined {
    const rightDic = Utils.toDictionary(right, (p) => p);
    return rightDic[left] != null ? left : undefined;
  }

  static rightJoinBy<TLeft, TRight>(
    left: TLeft[],
    right: TRight[],
    leftBy: (item: TLeft) => string | number,
    rightBy: (item: TRight) => string | number
  ): TLeft[] {
    const rightDic = Utils.toDictionary(right, rightBy);
    return left ? left.filter((p) => rightDic[leftBy(p)] != null) : [];
  }

  static rightJoinSingleBy<TLeft, TRight>(
    left: TLeft,
    right: TRight[],
    leftBy: (item: TLeft) => string | number,
    rightBy: (item: TRight) => string | number
  ): TLeft | undefined {
    const rightDic = Utils.toDictionary(right, rightBy);
    return rightDic[leftBy(left)] != null ? left : undefined;
  }

  static defaultIfNull<T>(value: T | null | undefined, defaultValue: T): T {
    return value == null ? defaultValue : value;
  }

  static flatTwoDimensionsArray<T>(value: T[][]): T[] {
    let result: T[] = [];
    value.forEach((x) => {
      result = result.concat(x);
    });
    return result;
  }

  static getDistinctValues<T>(collection: T[]): T[] {
    return Array.from(new Set(collection));
  }

  static distinctBy<T>(
    collection: T[],
    comparator: (item: T) => string | number
  ): T[] {
    const usedComparatorValues = new Set();
    const result: T[] = [];
    collection.forEach((a) => {
      if (!usedComparatorValues.has(comparator(a))) {
        result.push(a);
        usedComparatorValues.add(comparator(a));
      }
    });
    return result;
  }

  static distinct<T>(collection: T[]): T[] {
    return uniq(collection);
  }

  static concatList<T>(...lists: Array<T[] | null>): T[] {
    let result = [];
    lists
      .map((p) => (p == null ? [] : p))
      .forEach((p) => {
        result = result.concat(p);
      });
    return result;
  }

  static removeAlls<T extends string | number>(
    collection: T[],
    toRemoveItems: T[]
  ): T[] {
    const toRemiveItemsDic = Utils.toDictionary(toRemoveItems);

    return collection.filter((ele) => toRemiveItemsDic[ele] == null);
  }

  static removeFirst(
    collection: Array<string | number | unknown>,
    item: string | number | unknown
  ): Array<string | number | unknown> {
    return Utils.clone(collection, (p) => {
      const index = p.indexOf(item);
      if (index !== -1) {
        p.splice(index, 1);
      }
      return p;
    });
  }

  static removeAll(
    collection: Array<string | number>,
    item: string | number
  ): Array<string | number> {
    return collection.filter((ele) => ele !== item);
  }

  static moveItemsToTheEndOfList<T>(
    collection: T[],
    itemsConditionFn: (item: T) => boolean
  ): T[] {
    return collection
      .filter((x) => !itemsConditionFn(x))
      .concat(collection.filter(itemsConditionFn));
  }

  static getFileNameFromPath(path: string): string {
    return path.split('/').pop();
  }

  static countDictionaryKey(dictionary: Dictionary<unknown>): number {
    return Object.keys(dictionary).length;
  }

  static countDictionaryValue(dictionary: Dictionary<unknown>): number {
    return Object.keys(dictionary).filter(
      (key) => dictionary[key] !== undefined && dictionary[key] !== null
    ).length;
  }
}
