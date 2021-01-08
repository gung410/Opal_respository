import { Utils } from './utils';

/**
 * Local storage APIs
 * By default data will be serialized and encoded
 */
export class LocalStorageUtils {
  public static get<T>(key: string, prop?: string): T | null {
    const dataJson: string = localStorage.getItem(key);

    if (!dataJson) {
      return null;
    }

    const collection: { [key: string]: T } | null = Utils.deserializeJSON(Utils.decodeBase64(dataJson));

    return collection && prop && collection[prop];
  }

  public static getIfNot<T>(key: string, obj: {}): T {
    const data: string = localStorage.getItem(key);

    if (data) {
      const decodedValue: string = Utils.decodeBase64(data);
      const deserializedValue: T = Utils.deserializeJSON<T>(decodedValue);

      return deserializedValue;
    }

    return Object.create(obj);
  }

  public static set<T>(key: string, value: T, extendable: boolean = false): void {
    let data: T;

    if (extendable) {
      data = Utils.assignIn(LocalStorageUtils.get<T>(key), value);
    } else {
      data = value;
    }

    const serializedValue: string = Utils.serializeJSON(data);
    const encodedValue: string = Utils.encodeBase64(serializedValue);

    localStorage.setItem(key, encodedValue);
  }

  public static setIfNot<T>(key: string, value: T): void {
    if (!LocalStorageUtils.get<T>(key)) {
      LocalStorageUtils.set(key, value);
    }
  }

  public static remove(key: string): void {
    localStorage.removeItem(key);
  }
}

/**
 * Session storage APIs
 * By default data will be serialized and encoded
 */
export class SessionStorageUtils {
  public static get<T>(key: string, prop?: string): T | null {
    const dataJson: string = sessionStorage.getItem(key);
    const collection: { [key: string]: T } | null = Utils.deserializeJSON(dataJson);

    return collection && prop && collection[prop];
  }

  public static getIfNot<T>(key: string, obj: {}): T {
    const data: string = sessionStorage.getItem(key);

    if (data) {
      return Utils.deserializeJSON(data);
    }

    return Object.create(obj);
  }

  public static set<T>(key: string, value: T, extendable: boolean = false): void {
    let data: T;

    if (extendable) {
      data = Utils.assignIn(LocalStorageUtils.get<T>(key), value);
    } else {
      data = value;
    }

    return sessionStorage.setItem(key, Utils.serializeJSON(data));
  }

  public static setIfNot<T>(key: string, value: T): void {
    if (!SessionStorageUtils.get<T>(key)) {
      SessionStorageUtils.set(key, value);
    }
  }

  public static remove(key: string): void {
    sessionStorage.removeItem(key);
  }
}
