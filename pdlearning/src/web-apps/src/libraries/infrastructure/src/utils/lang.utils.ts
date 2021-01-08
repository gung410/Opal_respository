import { Observable } from 'rxjs';
/**
 * @license
 * Copyright Google Inc. All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

// tslint:disable:no-any

export class LangUtils {
  /**
   * Determine if the argument is shaped like a Promise
   */
  public static isPromise(obj: any): obj is Promise<any> {
    // Allow any Promise/A+ compliant thenable.
    // It's up to the caller to ensure that obj.then conforms to the spec
    return Boolean(obj) && typeof obj.then === 'function';
  }

  /**
   * Determine if the argument is an Observable
   */
  public static isObservable(obj: any | Observable<any>): obj is Observable<any> {
    // TODO use Symbol.observable when https://github.com/ReactiveX/rxjs/issues/2415 will be resolved
    return Boolean(obj) && typeof obj.subscribe === 'function';
  }

  /**
   * Convert the argument is a Promise
   */
  public static resolvedAsPromise(obj: void | Promise<any>): Promise<any> {
    if (LangUtils.isPromise(obj)) {
      return obj;
    } else {
      return Promise.resolve(obj);
    }
  }
}
