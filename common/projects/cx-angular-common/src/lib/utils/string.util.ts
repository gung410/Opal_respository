import { Dictionary } from '../../typings';

// @dynamic
export class CxStringUtil {
  private static readonly registeredUniqueStrings: string[] = [];
  public static registerUnique(value: string) {
    if (CxStringUtil.registeredUniqueStrings.indexOf(value) < 0) {
      CxStringUtil.registeredUniqueStrings.push(value);
    } else {
      throw new Error(`Value ${value} is registered.`);
    }
    return value;
  }

  public static substrBeforeFirst(originalStr: string, beforeStr: string) {
    const beforeStrIndex = originalStr.indexOf(beforeStr);
    return beforeStrIndex > -1
      ? originalStr.substring(0, beforeStrIndex)
      : originalStr;
  }

  public static isNullOrEmpty(value: string | number | undefined | null) {
    return value === undefined || value.toString().trim() === '';
  }
  public static isNumber(
    value: string | undefined | number | undefined
  ): boolean {
    if (value === undefined) {
      return false;
    }
    return new RegExp('^-?[0-9]\\d*(\\.\\d+)?$').test(value.toString());
  }

  public static toUppercaseSplitByUppercaseWithUnderscore(value: string) {
    return value
      .replace(
        new RegExp('\\.?([A-Z]|\\d+)', 'g'),
        (value1: string, value2: string) => {
          return '_' + value2;
        }
      )
      .replace(new RegExp('^_'), '')
      .toUpperCase();
  }

  public static toSplitByUppercaseWithSpace(value: string) {
    return value
      .replace(
        new RegExp('\\.?([A-Z]|\\d+)', 'g'),
        (value1: string, value2: string) => {
          return ' ' + value2;
        }
      )
      .replace(new RegExp('^\\s'), '');
  }

  public static toLowercaseSplitByUppercaseWithHyphen(value: string) {
    return value
      .replace(
        new RegExp('\\.?([A-Z]|\\d+)', 'g'),
        (value1: string, value2: string) => {
          return '-' + value2.toLowerCase();
        }
      )
      .replace(new RegExp('^-'), '');
  }

  public static toBoolean(value: string | undefined) {
    if (value === undefined) {
      return false;
    }
    if (value === '0' || value.toLowerCase() === 'false') {
      return false;
    }
    if (value === '1' || value.toLowerCase() === 'true') {
      return true;
    }
    throw new Error(`${value} can't be parsed to boolean`);
  }

  public static stringUrlToQueryDic(url: string, urlOrigin?: string) {
    let urlWithoutOrigin = url.replace(
      urlOrigin !== undefined ? urlOrigin : location.origin,
      ''
    );
    const querySperatorIndex = urlWithoutOrigin.indexOf('?');
    if (
      querySperatorIndex >= 0 &&
      querySperatorIndex < urlWithoutOrigin.length - 1
    ) {
      urlWithoutOrigin = urlWithoutOrigin.substr(
        urlWithoutOrigin.indexOf('?') + 1
      );
    } else {
      return {};
    }

    const dictionary: Dictionary<any> = {};
    const parts = urlWithoutOrigin.split('&');
    parts.forEach(item => {
      const keyValuePair = item.split('=');

      const key = keyValuePair[0];
      let value = keyValuePair[1];

      value = decodeURIComponent(value);
      value = value.replace(/\+/g, ' ');

      dictionary[key] = value;
    });

    return dictionary;
  }

  public static camelize(str: string): string {
    const firstReplacedString = str.replace(
      /(?:^\w|[A-Z]|\b\w)/g,
      (letter, index) => {
        return index === 0 ? letter.toLowerCase() : letter.toUpperCase();
      }
    );
    return firstReplacedString.replace(/\s+/g, '');
  }

  public static truncateWordByWord(
    value: string,
    maxLength: number,
    overflowSuffix: string
  ) {
    if (value.length <= maxLength) {
      return value;
    }
    const strAry = value.split(' ');
    let retLen = strAry[0].length;
    let i = 1;
    for (i = 1; i < strAry.length; i++) {
      if (retLen === maxLength || retLen + strAry[i].length + 1 > maxLength) {
        break;
      }
      retLen += strAry[i].length + 1;
    }
    return strAry.slice(0, i).join(' ') + (overflowSuffix || '');
  }
}
