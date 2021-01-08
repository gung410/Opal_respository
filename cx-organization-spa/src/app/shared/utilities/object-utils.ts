import * as _ from 'lodash';
import { findIndexCommon } from '../constants/common.const';

export class ObjectUtilities {
  static copy(obj: any, include: any = [], exclude: any = []): any {
    return Object.keys(obj).reduce((target, k) => {
      if (exclude.length) {
        if (exclude.indexOf(k) < 0) {
          target[k] = obj[k];
        }
      } else if (include.indexOf(k) > findIndexCommon.notFound) {
        target[k] = obj[k];
      }

      return target;
    }, {});
  }

  static isEmpty(obj: any): any {
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        return false;
      }
    }

    return true;
  }
}
