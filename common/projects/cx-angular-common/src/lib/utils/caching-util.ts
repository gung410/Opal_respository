import { CxDateUtil } from './date-util';
import { CxStringUtil } from './string.util';

export class CxCachingUtil {
  public static getString(key: string): string | undefined {
    return localStorage.getItem(key) as string | undefined;
  }

  public static getDate(key: string): Date | undefined {
    const stringValue = localStorage.getItem(key);
    if (stringValue === undefined) {
      {
        return undefined;
      }
    }
    return CxDateUtil.parseDate(stringValue);
  }

  public static getBoolean(key: string): boolean | undefined {
    const stringValue = localStorage.getItem(key);
    if (stringValue === undefined) {
      return undefined;
    }
    return CxStringUtil.toBoolean(stringValue);
  }

  public static setString(key: string, value: string) {
    localStorage.setItem(key, value);
  }

  public static setJson(key: string, value: object) {
    localStorage.setItem(key, JSON.stringify(value));
  }

  public static getNumber(key: string): number | undefined {
    const stringValue = localStorage.getItem(key);
    if (stringValue === undefined) {
      return undefined;
    }
    return Number(stringValue);
  }

  public static getJson(key: string): any {
    const stringValue = localStorage.getItem(key);
    if (stringValue === undefined) {
      return undefined;
    }
    return JSON.parse(stringValue);
  }

  public static removeItem(key: string) {
    localStorage.removeItem(key);
  }
}
