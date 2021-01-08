import {
  difference,
  filter,
  find,
  keyBy,
  last,
  max,
  maxBy,
  orderBy,
  remove,
  uniq,
  uniqBy
} from 'lodash';

import { Dictionary } from '../../typings';
import { any } from './_common-functions';
import { CxObjectUtil } from './object.util';

// @dynamic
export class CxArrayUtil {
  public static find<T>(
    collection: T[] | undefined,
    predicate: (item: T) => boolean,
    fromIndex?: number
  ): T | undefined {
    return find(collection, predicate, fromIndex);
  }

  public static get<T>(
    collection: T[] | undefined,
    predicate: (item: T) => boolean,
    fromIndex?: number
  ): T {
    const result = find(collection, predicate, fromIndex);
    if (result === undefined) {
      throw new Error(`Can't find any item`);
    }
    return result;
  }

  public static findSelect<T, TSelect>(
    collection: T[] | undefined,
    predicate: (item: T) => boolean,
    selectCallback: (item: T) => TSelect,
    fromIndex?: number
  ): TSelect | undefined {
    const item = find(collection, predicate, fromIndex);
    return item !== undefined ? selectCallback(item) : undefined;
  }

  public static filter<T>(
    collection: T[] | undefined,
    predicate: (item: T) => boolean
  ): T[] {
    return filter(collection, predicate);
  }

  public static selectMany<T, S>(
    collection: T[] | undefined,
    selectCallback: (item: T) => S[]
  ): S[] {
    if (collection === undefined || collection.length === 0) {
      return [];
    }
    const listOfChildList = collection.map(selectCallback);
    return listOfChildList.reduce((prevValue, currentValue) =>
      prevValue.concat(currentValue)
    );
  }

  public static max<T>(collection: ArrayLike<T> | undefined): T | undefined {
    return max(collection);
  }

  public static maxBy<T>(
    collection: ArrayLike<T> | undefined,
    iteratee: (item: T) => number
  ): T | undefined {
    return maxBy(collection, iteratee);
  }

  public static toDictionary<T>(
    collection: ArrayLike<T> | undefined,
    dictionaryKeySelector: (item: T) => string | number
  ): Dictionary<T> {
    return keyBy(collection, dictionaryKeySelector);
  }

  public static keyBySelect<T, TSelect>(
    collection: T[] | undefined,
    dictionaryKeySelector: (item: T) => string | number,
    dictionaryValueSelector: (item: T) => TSelect
  ): Dictionary<TSelect> {
    if (collection === undefined) {
      return {};
    }

    const result: Dictionary<TSelect> = {};
    collection.forEach(item => {
      result[dictionaryKeySelector(item)] = dictionaryValueSelector(item);
    });
    return result;
  }

  public static includesAll<T>(superSet: T[], subset: T[]): boolean {
    return difference(subset, superSet).length === 0;
  }

  public static includesAny<T>(superSet: T[], subset: T[]): boolean {
    for (const sub of subset) {
      const subsetItem = sub;
      if (superSet.indexOf(subsetItem) >= 0) {
        return true;
      }
    }
    return false;
  }

  public static all<T>(
    collection: ArrayLike<T> | undefined,
    predicate: (item: T) => boolean
  ): boolean {
    return CxArrayUtil.any(collection, (item: T) => !predicate(item))
      ? false
      : true;
  }

  public static any<T>(
    collection: ArrayLike<T> | undefined,
    predicate: (item: T) => boolean
  ): boolean {
    return any(collection, predicate);
  }

  public static remove<T>(
    collection: T[],
    predicate: (item: T) => boolean
  ): T[] {
    return remove(collection, predicate);
  }

  public static removeFirst<T>(
    collection: T[],
    predicate: (item: T) => boolean
  ): T | undefined {
    let removedItem: T | undefined;
    for (let i = 0; i < collection.length; i++) {
      if (predicate(collection[i])) {
        removedItem = collection.splice(i, 1)[0];
      }
    }
    return removedItem;
  }

  public static removeMissedItems<T>(
    collection: T[],
    newCollection: T[],
    equalCallback: (item: T, newItem: T) => boolean
  ) {
    return CxArrayUtil.remove(collection, item => {
      return (
        CxArrayUtil.find(newCollection, newItem =>
          equalCallback(item, newItem)
        ) === undefined
      );
    });
  }

  public static add<T>(
    collection: T[] | undefined,
    addItem: T,
    condition?: (addItem: T, collection: ArrayLike<T> | undefined) => boolean
  ): T[] | undefined {
    if (collection === undefined) {
      return collection;
    }
    if (condition === undefined || condition(addItem, collection)) {
      collection.push(addItem);
      return collection;
    }
    return collection;
  }

  public static replaceOne<T>(
    collection: T[],
    replaceItem: T,
    condition: (item: T) => boolean
  ): T[] {
    for (let i = 0; i < collection.length; i++) {
      if (condition(collection[i])) {
        collection[i] = replaceItem;
        return collection;
      }
    }
    return collection;
  }

  public static replaceMany<T>(
    collection: T[],
    replaceItems: T[],
    condition: (item: T, replaceItem: T) => boolean
  ): T[] {
    const replacedItems: T[] = [];
    replaceItems = CxObjectUtil.clone(replaceItems);
    for (let i = 0; i < collection.length; i++) {
      for (let j = 0; j < replaceItems.length; j++) {
        if (condition(collection[i], replaceItems[j])) {
          collection[i] = replaceItems[j];
          replacedItems.push(replaceItems[j]);
          replaceItems.splice(j, 1);
          break;
        }
      }
    }
    return replacedItems;
  }

  public static addOrReplace<T>(
    collection: T[] | undefined,
    item: T,
    replaceCondition: (item: T) => boolean
  ): T[] | undefined {
    if (collection === undefined) {
      return collection;
    }
    for (let i = 0; i < collection.length; i++) {
      if (replaceCondition(collection[i])) {
        collection[i] = item;
        return collection;
      }
    }

    collection.push(item);
    return collection;
  }

  public static addIfNotExist<T>(
    collection: T[],
    addItems: T[],
    equalBy?: (item: T) => any
  ): T[] {
    addItems.forEach(addItem => {
      if (
        CxArrayUtil.find(collection, p =>
          equalBy !== undefined
            ? equalBy(p) === equalBy(addItem)
            : p === addItem
        ) === undefined
      ) {
        collection.push(addItem);
      }
    });
    return collection;
  }

  public static last<T>(collection: ArrayLike<T> | undefined): T | undefined {
    if (collection === undefined) {
      return undefined;
    }
    return last(collection);
  }

  public static distinct<T>(collection: ArrayLike<T>): T[] {
    return uniq(collection);
  }

  public static distinctBy<T>(
    collection: ArrayLike<T>,
    iterate: (value: T) => {} | undefined
  ): T[] {
    return uniqBy(collection, iterate);
  }

  public static orderBy<T>(
    collection: T[],
    iterates: (value: T, index: number, collection: ArrayLike<T>) => any,
    desc: boolean = false
  ): T[] {
    return orderBy(collection, iterates, desc ? 'desc' : 'asc');
  }

  public static concatAll<T>(...collection: T[][]): T[] {
    let result: T[] = [];
    collection.forEach(item => {
      result = result.concat(item);
    });
    return result;
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

  public static rightMerge<T>(
    currentCollection: T[],
    newCollection: T[],
    compareSelector: (item: T) => string | number
  ): T[] {
    if (currentCollection.length === 0) {
      return newCollection;
    }
    const currentCollectionDic = CxArrayUtil.toDictionary(
      currentCollection,
      compareSelector
    );
    const result: T[] = [];
    for (let i = 0; i < newCollection.length; i++) {
      result.push(newCollection[i]);

      const innerJoinItem = currentCollectionDic[compareSelector(result[i])];
      if (innerJoinItem !== undefined) {
        if (typeof innerJoinItem === 'object') {
          result[i] = CxObjectUtil.clone(result[i], newResultItemValue => {
            return CxObjectUtil.extend(
              newResultItemValue as any,
              currentCollectionDic[compareSelector(result[i]).toString()]
            );
          });
        }
      }
    }
    return result;
  }

  public static total<T>(
    collection: T[],
    valueSelector: (item: T) => number | undefined
  ): number {
    let result = 0;
    collection.forEach(p => {
      const currentItemValue = valueSelector(p);
      if (currentItemValue !== undefined) {
        result += currentItemValue;
      }
    });
    return result;
  }
}
