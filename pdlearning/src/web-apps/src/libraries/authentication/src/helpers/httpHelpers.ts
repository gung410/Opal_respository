import { HttpClient, HttpParams } from '@angular/common/http';

import { AppConstant } from '../app.constant';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
// tslint:disable:all

/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
class HttpHelperOptions {
  headers?: any;
  observe?: string;
  reportProgress?: boolean;
  responseType?: string;
  withCredentials?: boolean;
  avoidIntercepterCatchError?: boolean;
}

/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
@Injectable({ providedIn: 'root' })
export class HttpHelpers {
  constructor(private http: HttpClient) {}

  get<T>(path: string, objparams?: any, helperOptions?: HttpHelperOptions): Observable<T> {
    const options = this.getHttpOptions(helperOptions, objparams);

    return this.http.get<T>(path, options);
  }

  post<T>(path: string, body?: any, objparams?: any, helperOptions?: HttpHelperOptions): Observable<T> {
    const payLoad = this.cloneBody(body);
    const options = this.getHttpOptions(helperOptions, objparams);

    return this.http.post<T>(path, payLoad, options);
  }

  put<T>(path: string, body?: any, objparams?: any, helperOptions?: HttpHelperOptions): Observable<T> {
    const payLoad = this.cloneBody(body);
    const options = this.getHttpOptions(helperOptions, objparams);

    return this.http.put<T>(path, payLoad, options);
  }

  delete<T>(path: string, body?: any, objparams?: any, helperOptions?: HttpHelperOptions): Observable<T> {
    const payLoad = this.cloneBody(body);
    const options = {
      ...this.getHttpOptions(helperOptions, objparams),
      body: payLoad
    };

    return this.http.request<T>('delete', path, options);
  }

  async getAsync<T>(path: string, objparams?: any, helperOptions?: HttpHelperOptions): Promise<any> {
    const getPromise = this.get<T>(path, objparams, helperOptions).toPromise();

    return await toCxAsync(getPromise);
  }

  async postAsync<T>(path: string, body?: any, objparams?: any, helperOptions?: HttpHelperOptions): Promise<any> {
    const postPromise = this.post<T>(path, body, objparams, helperOptions).toPromise();

    return await toCxAsync(postPromise);
  }

  async putAsync<T>(path: string, body?: any, objparams?: any, helperOptions?: HttpHelperOptions): Promise<any> {
    const putPromise = this.put<T>(path, body, objparams, helperOptions).toPromise();

    return await toCxAsync(putPromise);
  }

  async deleteAsync<T>(path: string, body?: any, objparams?: any, helperOptions?: HttpHelperOptions): Promise<any> {
    const deletePromise = this.delete<T>(path, body, objparams, helperOptions).toPromise();

    return await toCxAsync(deletePromise);
  }

  // tslint:disable: no-unsafe-any
  private convertObjectToHttpParams(params: any): HttpParams {
    let paramsClone = new HttpParams();
    if (params !== null && params !== undefined) {
      for (const key of Object.keys(params)) {
        if (params[key] === null || params[key] === undefined || params[key] === '') {
          continue;
        }
        if (Array.isArray(params[key])) {
          params[key].forEach(item => {
            paramsClone = paramsClone.append(key, item);
          });
        } else {
          paramsClone = paramsClone.append(key, params[key]);
        }
      }
    }

    return paramsClone;
  }

  private cloneBody(body: any): string {
    const bodyObj = JSON.parse(JSON.stringify(body));
    if (!!bodyObj) {
      for (const propName in bodyObj) {
        if (bodyObj[propName] === null || bodyObj[propName] === undefined) {
          delete bodyObj[propName];
        }
      }
    }

    return JSON.stringify(bodyObj);
  }

  private getHttpOptions(options: HttpHelperOptions, objparams?: any): {} {
    const httpOptions: any = options ? options : {};
    if (!httpOptions.headers) {
      // TODO: set default headers
      httpOptions.headers = {};
    }

    httpOptions.params = this.convertObjectToHttpParams(objparams);

    if (httpOptions.avoidIntercepterCatchError) {
      httpOptions.headers[AppConstant.httpRequestAvoidIntercepterCatchError] = 'true';
    }

    return httpOptions;
  }
}

export function toCxAsync<T>(promise: Promise<T>): Promise<any> {
  return promise
    .then(data => {
      return new AsyncResponse(data, null);
    })
    .catch((err: Error) => {
      return new AsyncResponse(undefined, err);
    });
}

export class AsyncResponse<T> {
  data: T;
  error: Error;
  constructor(res: T, err: Error) {
    this.data = res;
    this.error = err;
  }
}
