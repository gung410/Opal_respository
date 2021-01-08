import { DEFAULT_INTERCEPTORS, InterceptorRegistry } from './interceptor-registry';
import { DownloadFileIHttpOptions, IHttpOptions } from './models/http-options';
import { FW_DISPLAY_SPINNER, FW_INTERCEPTOR_KEYS } from '../constants';
import { HttpParams, HttpResponse } from '@angular/common/http';

import { CommonFacadeService } from '../services/common-facade.service';
import { IGetParams } from './models/get-params';
import { Observable } from 'rxjs';
import { Utils } from '../utils/utils';
import { map } from 'rxjs/operators';

export abstract class BaseBackendService {
  protected get apiUrl(): string {
    return '';
  }

  private interceptorRegistry: InterceptorRegistry = new InterceptorRegistry(DEFAULT_INTERCEPTORS);

  constructor(protected commonFacadeService: CommonFacadeService) {}

  public get<TResponse>(url: string, params?: IGetParams, showSpinner: boolean = true): Observable<TResponse> {
    return this.commonFacadeService.http.get<TResponse>(
      `${this.apiUrl}${url}`,
      this.getHttpOptions(showSpinner, this.preprocessData(params))
    );
  }

  public post<TBody, TResponse>(
    url: string,
    body: TBody,
    showSpinner: boolean = true,
    additionalHttpOptions?: Partial<IHttpOptions>
  ): Observable<TResponse> {
    return this.commonFacadeService.http.post<TResponse>(`${this.apiUrl}${url}`, this.preprocessData(body), <IHttpOptions>{
      ...this.getHttpOptions(showSpinner),
      ...additionalHttpOptions
    });
  }

  public postDownloadFile<TBody>(
    url: string,
    body: TBody,
    showSpinner: boolean = true,
    isIOSDevice: boolean = false
  ): Observable<HttpResponse<Blob>> {
    return this.commonFacadeService.http
      .post(`${this.apiUrl}${url}`, this.preprocessData(body), this.getDownloadFileHttpOptions(showSpinner))
      .pipe(
        map(res => {
          isIOSDevice
            ? Utils.downloadFile(res.body, res.headers.get('Download-File-Name'))
            : Utils.downloadFileByFileReader(res.body, res.headers.get('Download-File-Name'));
          return res;
        })
      );
  }

  public put<TBody, TResponse>(url: string, body: TBody, showSpinner: boolean = true): Observable<TResponse> {
    return this.commonFacadeService.http.put<TResponse>(
      `${this.apiUrl}${url}`,
      this.preprocessData(body),
      this.getHttpOptions(showSpinner)
    );
  }

  public delete<TResponse>(url: string, showSpinner: boolean = true): Observable<TResponse> {
    return this.commonFacadeService.http.delete<TResponse>(`${this.apiUrl}${url}`, this.getHttpOptions(showSpinner));
  }

  /**
   * Override this method to set or replace interceptors for current service scope.
   */
  protected onFilterInterceptors(registry: InterceptorRegistry): InterceptorRegistry {
    return this.interceptorRegistry;
  }

  /**
   * This function will pass the internal headers for checking which interceptors will be run.
   * @see BaseInterceptor
   */
  protected getHttpOptions(showSpinner: boolean, params?: IGetParams): IHttpOptions {
    return {
      headers: {
        [FW_DISPLAY_SPINNER]: showSpinner.toString(),
        [FW_INTERCEPTOR_KEYS]: this.onFilterInterceptors(this.interceptorRegistry).toJSON()
      },
      params: this.parseHttpGetParam(params)
    };
  }

  protected getDownloadFileHttpOptions(showSpinner: boolean, params?: IGetParams): DownloadFileIHttpOptions {
    return {
      headers: {
        [FW_DISPLAY_SPINNER]: showSpinner.toString(),
        [FW_INTERCEPTOR_KEYS]: this.onFilterInterceptors(this.interceptorRegistry).toJSON()
      },
      params: this.parseHttpGetParam(params),
      responseType: 'blob',
      observe: 'response'
    };
  }

  /**
   * We remove all null props because it's not necessary. And in server dotnet core, if the data is nullable => default value is null
   * so that do not need to submit null. If data is not nullable, then if submit null can raise exception.
   */
  private preprocessData<T>(data: T): T {
    return Utils.toPureObj(Utils.removeNullProps(data));
  }

  private flattenHttpGetParam(inputParams?: IGetParams, returnParam: IGetParams = {}, prefix?: string): IGetParams {
    for (const paramKey in inputParams || {}) {
      const inputParamValue = inputParams[paramKey];
      const inputParamFinalKey = prefix ? `${prefix}.${paramKey}` : paramKey;
      if (inputParamValue instanceof Array) {
        returnParam[inputParamFinalKey] = inputParamValue;
      } else if (typeof inputParamValue === 'object') {
        this.flattenHttpGetParam(inputParamValue, returnParam, paramKey);
      } else if (inputParamValue != null) {
        returnParam[inputParamFinalKey] = inputParamValue.toString();
      }
    }

    return returnParam;
  }

  private parseHttpGetParam(inputParams?: IGetParams): HttpParams {
    let returnParam = new HttpParams();
    const flattenedInputParams = this.flattenHttpGetParam(inputParams);
    for (const paramKey in flattenedInputParams) {
      if (flattenedInputParams.hasOwnProperty(paramKey)) {
        const inputParamValue = flattenedInputParams[paramKey];
        if (inputParamValue instanceof Array) {
          inputParamValue.forEach(p => {
            returnParam = returnParam.append(paramKey, p);
          });
        } else {
          returnParam = returnParam.append(paramKey, inputParamValue.toString());
        }
      }
    }
    return returnParam;
  }
}
