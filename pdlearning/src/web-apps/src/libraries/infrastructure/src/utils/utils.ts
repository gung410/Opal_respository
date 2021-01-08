import { MonoTypeOperatorFunction, Observable, Subscription, interval, of, pipe } from 'rxjs';
import { assignIn, clone, cloneDeep, difference, escape, flatMap, flow, isEmpty, keyBy, keys, orderBy, remove, uniq } from 'lodash-es';
import { delay, takeUntil } from 'rxjs/operators';

import { v4 as uuid } from 'uuid';

type List<T> = ArrayLike<T>;

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
  public static isDefined<T>(value: T): boolean {
    return value != null && value !== null;
  }

  /**
   * Checks if value is an empty object, collection, map, or set.
   * Refer https://lodash.com/docs/4.17.10#isEmpty
   */
  public static isEmpty<T>(value: T): boolean {
    return isEmpty(value);
  }

  /**
   * This method is like _.assign except that it iterates over own and inherited source properties.
   * https://lodash.com/docs/4.17.10#assignIn
   */
  public static assignIn<U, V>(object: U, other: V): U & V {
    return <U & V>assignIn(object, other);
  }

  /**
   * Base64 encoding
   */
  public static encodeBase64(value: string): string {
    return window.btoa(value);
  }

  // Support characters outside Latin1
  public static encodeBase64_2(value: string): string {
    return window.btoa(unescape(encodeURIComponent(value)));
  }

  /**
   * Base64 decoding
   */
  public static decodeBase64(value: string): string {
    return window.atob(value);
  }

  // Support characters outside Latin1
  public static decodeBase64_2(value: string): string {
    return decodeURIComponent(escape(window.atob(value)));
  }

  public static serializeJSON<T>(value: T): string {
    return JSON.stringify(value);
  }

  public static deserializeJSON<T>(input: string): T {
    return <T>JSON.parse(input);
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
  public static clone<T>(value: T, updateClonedValueFn?: (clonedValue: T) => undefined | T | void): T {
    if (value == null) {
      return value;
    }
    let clonedValue: T = clone(value);
    if (updateClonedValueFn != null) {
      const updatedClonedValue: undefined | T | void = updateClonedValueFn(clonedValue);
      if (updatedClonedValue != null) {
        clonedValue = <T>updatedClonedValue;
      }
    }
    return clonedValue;
  }

  /**
   * This method is like _.clone except that it recursively clones value.
   * https://lodash.com/docs/4.17.15#cloneDeep
   */
  public static cloneDeep<T>(value: T): T {
    return value == null ? null : cloneDeep<T>(value);
  }

  /**
   * Removes all elements from array that predicate returns truthy for and returns an array of the removed elements. The predicate is
   * invoked with three arguments: (value, index, array).
   *
   * Note: Unlike _.filter, this method mutates array.
   * https://lodash.com/docs/4.17.15#remove
   */
  public static remove<T>(array: ArrayLike<T>, predicate?: (value: T, index: number, collection: ArrayLike<T>) => boolean): T[] {
    return remove<T>(array, predicate);
  }

  /**
   * Create an array of number from start to end (including)
   */
  public static range(start: number, end: number): number[] {
    const result: number[] = [];
    for (let i: number = start; i <= end; i++) {
      result.push(i);
    }
    return result;
  }

  public static move<T>(
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

  public static orderBy<T>(
    collection: T[],
    iteratees: (value: T, index: number, collection: ArrayLike<T>) => number | string | object,
    desc: boolean = false
  ): T[] {
    return orderBy(collection, iteratees, desc ? 'desc' : 'asc');
  }

  public static flatMap<T>(collection: Dictionary<T[]> | NumericDictionary<T[]> | null | undefined): T[] {
    return flatMap(collection);
  }

  public static uniq<T>(array: List<T> | null | undefined): T[] {
    return uniq(array);
  }

  // tslint:disable-next-line:no-any
  public static flow<A extends any[], R1, R2>(f1: (...args: A) => R1, f2: (a: R1) => R2): (...args: A) => R2 {
    return flow(
      f1,
      f2
    );
  }

  public static debounce(func: (...args: unknown[]) => void, wait: number): (...args: unknown[]) => void {
    if (wait <= 0) {
      return func;
    }

    let timeout: number;
    return (...args: unknown[]) => {
      const context = this;

      const executeFunction = () => {
        func.apply(context, args);
      };

      clearTimeout(timeout);
      timeout = <number>(<unknown>setTimeout(executeFunction, wait));
    };
  }

  public static delay(callback: () => void, delayTime?: number, cancelOn$?: Observable<unknown>): Subscription {
    if (typeof delayTime === 'number' && delayTime === 0) {
      callback();
      return new Subscription();
    } else {
      const delayObs = pipe(
        cancelOn$ != null ? takeUntil(cancelOn$) : (obs: Observable<unknown>) => obs,
        delay(delayTime == null ? 10 : delayTime)
      );
      return delayObs(of(undefined)).subscribe(() => callback());
    }
  }
  /**
   * This function create a new temporary new UUID. This is not guarantee unique. Use it only for front-end.
   */
  public static createUUID(): string {
    return uuid();
  }

  public static toDictionary<T>(collection: ArrayLike<T> | undefined, dictionaryKeySelector?: (item: T) => string | number): Dictionary<T> {
    const defaultKeySelector: (item: T) => string | number = p => p.toString();
    return keyBy(collection, dictionaryKeySelector != null ? dictionaryKeySelector : defaultKeySelector);
  }

  public static toDictionarySelect<T, TValueSelect>(
    collection: ArrayLike<T> | undefined,
    dictionaryKeySelector: (item: T) => string | number,
    dictionaryValueSelector: (item: T) => TValueSelect
  ): Dictionary<TValueSelect> {
    const result: Dictionary<TValueSelect> = {};
    for (let i = 0; i < collection.length; i++) {
      result[dictionaryKeySelector(collection[i])] = dictionaryValueSelector(collection[i]);
    }
    return result;
  }

  public static isDifferent<T>(value1: T, value2: T): boolean {
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

  public static isEqual<T>(value1: T, value2: T): boolean {
    return !Utils.isDifferent(value1, value2);
  }

  /**
   * Replace the item, return new updated list.
   */
  public static replaceOne<T>(collection: T[], replaceItem: T, condition: (item: T) => boolean): T[] {
    const clonedCollection = Utils.clone(collection);
    for (let i = 0; i < clonedCollection.length; i++) {
      if (condition(clonedCollection[i])) {
        clonedCollection[i] = replaceItem;
        return clonedCollection;
      }
    }
    return collection;
  }

  public static addOrReplace<T>(collection: T[] | undefined, item: T, replaceCondition: (item: T) => boolean): T[] | undefined {
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

  public static includesAll<T>(superset: T[], subset: T[]): boolean {
    return difference(subset, superset).length === 0;
  }

  public static includesAny<T>(superset: T[], subset: T[]): boolean {
    for (let i = 0; i < subset.length; i++) {
      const subsetItem = subset[i];
      if (superset.indexOf(subsetItem) >= 0) {
        return true;
      }
    }
    return false;
  }

  public static mapToArray<T>(map: Map<string, T>): T[] {
    return Array.from(map.values());
  }

  public static hasDuplicatedItems<T, TCheckBy>(collection: T[], checkBySelector: (item: T) => TCheckBy): boolean {
    for (let i = 0; i < collection.length; i++) {
      const item = collection[i];
      if (
        collection
          .slice(i + 1)
          .map(checkBySelector)
          .findIndex(p => p === checkBySelector(item)) >= 0
      ) {
        return true;
      }
    }
    return false;
  }

  public static toDictionaryGroupBy<T>(
    collection: ArrayLike<T> | undefined,
    dictionaryGroupByKeySelector: (item: T) => string | number,
    mapFn?: (items: T[]) => T[]
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

    if (mapFn != null) {
      Object.keys(result).forEach(key => {
        result[key] = mapFn(result[key]);
      });
    }

    return result;
  }

  public static any<T>(collection: ArrayLike<T> | undefined, predicate: (item: T) => boolean): boolean {
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

  public static selectMany<T, S>(collection: T[] | undefined, selectCallback: (item: T) => S[]): S[] {
    if (collection == null || collection.length === 0) {
      return [];
    }
    const listOfChildList = collection.map(selectCallback);
    return listOfChildList.reduce((prevValue, currentValue) => prevValue.concat(currentValue));
  }

  public static isAbsoluteUrl(url: string): boolean {
    // ref to: https://stackoverflow.com/questions/10687099/how-to-test-if-a-url-string-is-absolute-or-relative
    const r = new RegExp('^(?:[a-z]+:)?//', 'i');
    return r.test(url);
  }

  public static getHrefFromAnchor(aTag: HTMLAnchorElement): string {
    const html = aTag.outerHTML;
    const matches = /href=[\'"]?([^\'" >]+)/.exec(html);
    if (matches.length === 2) {
      // get url in href
      const href = matches[1];
      return href;
    }

    return '';
  }

  public static replaceColumnResizeHtml(originHtml: string): string {
    return originHtml.replace(/data-colwidth=[\\'"]?([^\\'" >]+)[\\'"]?/gi, 'width="$1" data-colwidth="$1"');
  }

  public static isInternalUrl(targetUrl: string): boolean {
    const urlTemp = document.createElement('a');
    urlTemp.href = targetUrl;
    return this.getPureDomain(urlTemp.host) === this.getPureDomain(location.host);
  }

  public static getPureDomain(url: string): string {
    return url.startsWith('www.') ? url.slice(4) : url;
  }

  public static escapeHtml(htmlString: string): string {
    return escape(htmlString);
  }

  public static extracUrlfromHtml(htmlString: string): string[] {
    const finalResult: string[] = [];
    const pattern = /<(a(?:\s+|\s.+\s)href|img(?:\s+|\s.+\s)src)="(?<url>.+?)"/gim;
    let hrefMatches = pattern.exec(htmlString);
    while (hrefMatches != null) {
      finalResult.push(hrefMatches[2]);
      hrefMatches = pattern.exec(htmlString);
    }
    return finalResult;
  }

  // tslint:disable-next-line:no-any
  public static getDictionaryKeys<T extends string | number>(object?: Dictionary<any>): T[] {
    // tslint:disable-next-line:no-any
    return keys(object).map((key: any) => <T>(!isNaN(<any>key) ? parseInt(key, undefined) : key));
  }

  public static assign<T extends object>(target: T, ...sources: Partial<T>[]): T {
    sources.forEach(source => {
      Utils.keys(source).forEach(sourceKey => {
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
  public static keys(source: any, ignorePrivate: boolean = true, ignoreNull: boolean = false): string[] {
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

  public static toPureObject<TObject, TObjectInterface>(source: TObject): TObjectInterface {
    const result = {};
    Utils.keys(source).forEach(key => {
      result[key] = source[key];
    });
    return <TObjectInterface>result;
  }

  public static upsertDic<T>(
    currentData: Dictionary<T>,
    newData: Dictionary<Partial<T>> | Partial<T>[],
    getItemKey: (item: T | Partial<T>) => string | number,
    // tslint:disable-next-line:no-any
    initItem: (data: any) => T,
    removeNotExistedItems?: boolean,
    removeNotExistedItemsFilter?: (item: Partial<T>) => boolean,
    replaceEachItem?: boolean,
    // tslint:disable-next-line:no-any
    onHasNewStateDifferent?: (newState: Dictionary<T>) => any,
    optionalProps: (keyof T)[] = []
  ): Dictionary<T> {
    return modifyDic(currentData, newState => {
      const newDataDic = newData instanceof Array ? Utils.toDictionary(newData, x => getItemKey(x)) : newData;
      if (removeNotExistedItems) {
        removeNotExistedItemsInNewData(newState, newDataDic);
      }

      Utils.getDictionaryKeys(newDataDic).forEach(id => {
        if (newState[id] == null || newDataDic[id] == null || typeof newDataDic[id] !== 'object' || typeof newState[id] !== 'object') {
          newState[id] = initItem(newDataDic[id]);
        } else {
          const prevNewStateItem = newState[id];
          const newStateItemData = replaceEachItem ? newDataDic[id] : Utils.assign<Partial<T>>(Utils.clone(newState[id]), newDataDic[id]);
          if (optionalProps.length > 0) {
            optionalProps.forEach(optionalProp => {
              if (prevNewStateItem[optionalProp] != null && newStateItemData[optionalProp] == null) {
                newStateItemData[optionalProp] = prevNewStateItem[optionalProp];
              }
            });
          }
          newState[id] = initItem(newStateItemData);
        }
      });
    });

    function removeNotExistedItemsInNewData(state: Dictionary<Partial<T>>, newDataDic: Dictionary<Partial<T>>): void {
      const removeItemIds = Utils.getDictionaryKeys(state).filter(
        id => newDataDic[id] == null && (removeNotExistedItemsFilter == null || removeNotExistedItemsFilter(state[id]))
      );
      removeItemIds.forEach(id => {
        delete state[id];
      });
    }

    function modifyDic(state: Dictionary<T>, modifyDicAction: (state: Dictionary<T>) => void | Dictionary<T>): Dictionary<T> {
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

  public static isNullOrEmpty(value: unknown | unknown[]): boolean {
    if (value instanceof Array) {
      return value.length === 0;
    }
    return value == null || value.toString().trim() === '';
  }

  public static isNullOrUndefined(value: unknown): boolean {
    return value == null || value === undefined;
  }

  public static doInterval(
    callback: (intervalSubscriber: Subscription) => unknown,
    ms: number,
    maximumCount?: number,
    // tslint:disable-next-line:no-any
    ...pipeOps: MonoTypeOperatorFunction<any>[]
  ): Subscription {
    const intervalSubscriber = Utils.interval(callback, ms, maximumCount, ...pipeOps);
    callback(intervalSubscriber);
    return intervalSubscriber;
  }

  public static interval(
    callback: (intervalSubscriber: Subscription) => unknown,
    ms: number,
    maximumCount?: number,
    // tslint:disable-next-line:no-any
    ...pipeOps: MonoTypeOperatorFunction<any>[]
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

  public static rightJoin<T extends string | number>(left: T[], right: T[]): T[] {
    const rightDic = Utils.toDictionary(right, p => p);
    return left ? left.filter(p => rightDic[p] != null) : [];
  }

  public static rightJoinSingle<T extends string | number>(left: T, right: T[]): T | undefined {
    const rightDic = Utils.toDictionary(right, p => p);
    return rightDic[left] != null ? left : undefined;
  }

  public static rightJoinBy<TLeft, TRight>(
    left: TLeft[],
    right: TRight[],
    leftBy: (item: TLeft) => string | number,
    rightBy: (item: TRight) => string | number
  ): TLeft[] {
    const rightDic = Utils.toDictionary(right, rightBy);
    return left ? left.filter(p => rightDic[leftBy(p)] != null) : [];
  }

  public static rightJoinSingleBy<TLeft, TRight>(
    left: TLeft,
    right: TRight[],
    leftBy: (item: TLeft) => string | number,
    rightBy: (item: TRight) => string | number
  ): TLeft | undefined {
    const rightDic = Utils.toDictionary(right, rightBy);
    return rightDic[leftBy(left)] != null ? left : undefined;
  }

  public static defaultIfNull<T>(value: T | null | undefined, defaultValue: T): T {
    return value == null ? defaultValue : value;
  }

  public static flatTwoDimensionsArray<T>(value: T[][]): T[] {
    let result: T[] = [];
    value.forEach(x => {
      result = result.concat(x);
    });
    return result;
  }

  public static getDistinctValues<T>(collection: T[]): T[] {
    return Array.from(new Set(collection));
  }

  public static distinctBy<T>(collection: T[], comparator: (item: T) => string | number): T[] {
    const usedComparatorValues = new Set();
    const result: T[] = [];
    collection.forEach(a => {
      if (!usedComparatorValues.has(comparator(a))) {
        result.push(a);
        usedComparatorValues.add(comparator(a));
      }
    });
    return result;
  }

  public static distinct<T>(collection: T[]): T[] {
    return uniq(collection);
  }

  public static concatList<T>(...lists: (T[] | null)[]): T[] {
    let result = [];
    lists
      .map(p => (p == null ? [] : p))
      .forEach(p => {
        result = result.concat(p);
      });
    return result;
  }

  public static removeFirst(collection: (string | number | unknown)[], item: string | number | unknown): (string | number | unknown)[] {
    return Utils.clone(collection, p => {
      const index = p.indexOf(item);
      if (index !== -1) {
        p.splice(index, 1);
      }
      return p;
    });
  }

  public static removeAll(collection: (string | number)[], item: string | number): (string | number)[] {
    return collection.filter(ele => ele !== item);
  }

  public static removeAlls<T extends string | number>(collection: T[], toRemoveItems: T[]): T[] {
    const toRemiveItemsDic = Utils.toDictionary(toRemoveItems);

    return collection.filter(ele => toRemiveItemsDic[ele] == null);
  }

  public static moveItemsToTheEndOfList<T>(collection: T[], itemsConditionFn: (item: T) => boolean): T[] {
    return collection.filter(x => !itemsConditionFn(x)).concat(collection.filter(itemsConditionFn));
  }

  public static getFileNameFromPath(path: string): string {
    return path.split('/').pop();
  }

  public static countDictionaryKey(dictionary: Dictionary<unknown>): number {
    return Object.keys(dictionary).length;
  }

  public static countDictionaryValue(dictionary: Dictionary<unknown>): number {
    return Object.keys(dictionary).filter(key => dictionary[key] !== undefined && dictionary[key] !== null).length;
  }

  /**
   * Return new list include orgiginalItems and newItems which is not existed in original items.
   * Compare item is equal by the return value from comparator
   */
  public static addIfNotExist<T>(originalItems: T[], newItems: T[], comparator: (item: T) => string | number): T[] {
    const originalItemsDicByComparator = Utils.toDictionary(originalItems, comparator);
    return originalItems.concat(newItems.filter(p => originalItemsDicByComparator[comparator(p)] == null));
  }

  public static round(value: number, factionDigits: number = 0): number {
    const floatPointMovingValue = Math.pow(10, factionDigits) * 1.0;
    return Math.round(value * floatPointMovingValue) / floatPointMovingValue;
  }

  public static downloadFile(fileContent: Blob, fileName: string): void {
    const blobURL = window.URL.createObjectURL(fileContent);
    const downloadAnchor = document.createElement('a');
    downloadAnchor.download = fileName;
    downloadAnchor.href = blobURL;
    downloadAnchor.click();
  }

  /**
   * To download file on iOS
   * Currently, cannot set the name of file when download.
   */
  public static downloadFileByFileReader(fileContent: Blob, fileName: string): void {
    const reader = new FileReader();
    reader.addEventListener('load', e => {
      window.open(reader.result.toString(), '_blank');
    });
    reader.readAsDataURL(fileContent);
  }

  public static setValueAllProperties<T>(object: T, value: unknown): void {
    Object.getOwnPropertyNames(object).forEach(property => {
      object[property] = value;
    });
  }

  public static checkAllPropertiesHasValue<T>(object: T): boolean {
    if (Utils.isNullOrEmpty(object)) {
      return false;
    }
    let result = false;
    const properties = Object.getOwnPropertyNames(object);
    for (let i = 0; i < properties.length; i++) {
      const property = properties[i];
      const value = object[property];

      if (!Utils.isNullOrEmpty(value)) {
        result = true;
        break;
      }
    }

    return result;
  }

  public static checkXssScript(content: unknown): RegExpMatchArray | null {
    // tslint:disable-next-line: max-line-length
    const xssRegex = /<[^\w<>]*(?:[^<>"'\s]*:)?[^\w<>]*(?:\W*s\W*c\W*r\W*i\W*p\W*t|\W*f\W*o\W*r\W*m|\W*s\W*t\W*y\W*l\W*e|\W*s\W*v\W*g|\W*m\W*a\W*r\W*q\W*u\W*e\W*e|(?:\W*l\W*i\W*n\W*k|\W*o\W*b\W*j\W*e\W*c\W*t|\W*e\W*m\W*b\W*e\W*d|\W*a\W*p\W*p\W*l\W*e\W*t|\W*p\W*a\W*r\W*a\W*m|\W*i?\W*f\W*r\W*a\W*m\W*e|\W*b\W*a\W*s\W*e|\W*b\W*o\W*d\W*y|\W*m\W*e\W*t\W*a|\W*i\W*m\W*a?\W*g\W*e?|\W*v\W*i\W*d\W*e\W*o|\W*a\W*u\W*d\W*i\W*o|\W*b\W*i\W*n\W*d\W*i\W*n\W*g\W*s|\W*s\W*e\W*t|\W*i\W*s\W*i\W*n\W*d\W*e\W*x|\W*a\W*n\W*i\W*m\W*a\W*t\W*e)[^>\w])|(?:<\w[\s\S]*[\s\0\/]|['"])(?:formaction|style|background|src|lowsrc|ping|on(?:d(?:e(?:vice(?:(?:orienta|mo)tion|proximity|found|light)|livery(?:success|error)|activate)|r(?:ag(?:e(?:n(?:ter|d)|xit)|(?:gestur|leav)e|start|drop|over)?|op)|i(?:s(?:c(?:hargingtimechange|onnect(?:ing|ed))|abled)|aling)|ata(?:setc(?:omplete|hanged)|(?:availabl|chang)e|error)|urationchange|ownloading|blclick)|Moz(?:M(?:agnifyGesture(?:Update|Start)?|ouse(?:PixelScroll|Hittest))|S(?:wipeGesture(?:Update|Start|End)?|crolledAreaChanged)|(?:(?:Press)?TapGestur|BeforeResiz)e|EdgeUI(?:C(?:omplet|ancel)|Start)ed|RotateGesture(?:Update|Start)?|A(?:udioAvailable|fterPaint))|c(?:o(?:m(?:p(?:osition(?:update|start|end)|lete)|mand(?:update)?)|n(?:t(?:rolselect|extmenu)|nect(?:ing|ed))|py)|a(?:(?:llschang|ch)ed|nplay(?:through)?|rdstatechange)|h(?:(?:arging(?:time)?ch)?ange|ecking)|(?:fstate|ell)change|u(?:echange|t)|l(?:ick|ose))|m(?:o(?:z(?:pointerlock(?:change|error)|(?:orientation|time)change|fullscreen(?:change|error)|network(?:down|up)load)|use(?:(?:lea|mo)ve|o(?:ver|ut)|enter|wheel|down|up)|ve(?:start|end)?)|essage|ark)|s(?:t(?:a(?:t(?:uschanged|echange)|lled|rt)|k(?:sessione|comma)nd|op)|e(?:ek(?:complete|ing|ed)|(?:lec(?:tstar)?)?t|n(?:ding|t))|u(?:ccess|spend|bmit)|peech(?:start|end)|ound(?:start|end)|croll|how)|b(?:e(?:for(?:e(?:(?:scriptexecu|activa)te|u(?:nload|pdate)|p(?:aste|rint)|c(?:opy|ut)|editfocus)|deactivate)|gin(?:Event)?)|oun(?:dary|ce)|l(?:ocked|ur)|roadcast|usy)|a(?:n(?:imation(?:iteration|start|end)|tennastatechange)|fter(?:(?:scriptexecu|upda)te|print)|udio(?:process|start|end)|d(?:apteradded|dtrack)|ctivate|lerting|bort)|DOM(?:Node(?:Inserted(?:IntoDocument)?|Removed(?:FromDocument)?)|(?:CharacterData|Subtree)Modified|A(?:ttrModified|ctivate)|Focus(?:Out|In)|MouseScroll)|r(?:e(?:s(?:u(?:m(?:ing|e)|lt)|ize|et)|adystatechange|pea(?:tEven)?t|movetrack|trieving|ceived)|ow(?:s(?:inserted|delete)|e(?:nter|xit))|atechange)|p(?:op(?:up(?:hid(?:den|ing)|show(?:ing|n))|state)|a(?:ge(?:hide|show)|(?:st|us)e|int)|ro(?:pertychange|gress)|lay(?:ing)?)|t(?:ouch(?:(?:lea|mo)ve|en(?:ter|d)|cancel|start)|ime(?:update|out)|ransitionend|ext)|u(?:s(?:erproximity|sdreceived)|p(?:gradeneeded|dateready)|n(?:derflow|load))|f(?:o(?:rm(?:change|input)|cus(?:out|in)?)|i(?:lterchange|nish)|ailed)|l(?:o(?:ad(?:e(?:d(?:meta)?data|nd)|start)?|secapture)|evelchange|y)|g(?:amepad(?:(?:dis)?connected|button(?:down|up)|axismove)|et)|e(?:n(?:d(?:Event|ed)?|abled|ter)|rror(?:update)?|mptied|xit)|i(?:cc(?:cardlockerror|infochange)|n(?:coming|valid|put))|o(?:(?:(?:ff|n)lin|bsolet)e|verflow(?:changed)?|pen)|SVG(?:(?:Unl|L)oad|Resize|Scroll|Abort|Error|Zoom)|h(?:e(?:adphoneschange|l[dp])|ashchange|olding)|v(?:o(?:lum|ic)e|ersion)change|w(?:a(?:it|rn)ing|heel)|key(?:press|down|up)|(?:AppComman|Loa)d|no(?:update|match)|Request|zoom))[\s\0]*=/gm;
    const contentString = JSON.stringify(content);
    return contentString.match(xssRegex);
  }

  public static removeNullProps<T>(obj: T): T {
    if (obj == null || typeof obj !== 'object') {
      return obj;
    }
    const objKeys = Object.keys(obj);
    for (let i = 0; i < objKeys.length; i++) {
      const key = objKeys[i];
      if (obj[key] == null) {
        delete obj[key];
      }
    }
    return obj;
  }

  public static downloadPdfFromBase64(base64Value: string, fileName: string): void {
    const linkSource = `data:application/pdf;base64,${base64Value}`;
    const downloadLink = document.createElement('a');

    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.click();
  }

  public static toPureObj<T>(source: T, ignorePrivate: boolean = true): T {
    if (source == null) {
      return null;
    }
    if (typeof source !== 'object') {
      return source;
    }
    if (source instanceof Array) {
      return <T>(<unknown>source.map(p => Utils.toPureObj(p, ignorePrivate)));
    }
    if (source instanceof Date) {
      return source;
    }
    const objResult = {};
    Utils.keys(source, ignorePrivate).forEach(key => {
      objResult[key] = Utils.toPureObj(source[key], ignorePrivate);
    });
    return <T>objResult;
  }
}
