import { BaseBackendService, IGetParams, IHttpOptions } from '@opal20/infrastructure';

import { HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

export class BaseStandaloneSurveyService extends BaseBackendService {
  protected subModule: 'csl' | 'lna' = 'lna';
  protected communityId?: string;

  public initService(param: { type: 'csl' | 'lna'; communityId?: string }): void {
    if (param.type === 'lna') {
      this.subModule = 'lna';
    } else {
      this.subModule = 'csl';
      this.communityId = this.communityId;
    }
  }

  public get<TResponse>(url: string, params?: IGetParams, showSpinner: boolean = true): Observable<TResponse> {
    if (params) {
      params.communityId = this.communityId ? this.communityId : null; // null value will be automatically removed.
      params.subModule = this.subModule;
    }
    return super.get(url, params, showSpinner);
  }

  public post<TBody, TResponse>(
    url: string,
    body: TBody,
    showSpinner: boolean = true,
    additionalHttpOptions?: Partial<IHttpOptions>
  ): Observable<TResponse> {
    body = this.preprocessBody<TBody>(body);
    return super.post(url, body, showSpinner, additionalHttpOptions);
  }

  public postDownloadFile<TBody>(
    url: string,
    body: TBody,
    showSpinner: boolean = true,
    isIOSDevice: boolean = false
  ): Observable<HttpResponse<Blob>> {
    body = this.preprocessBody<TBody>(body);
    return super.postDownloadFile(url, body, showSpinner, isIOSDevice);
  }

  public put<TBody, TResponse>(url: string, body: TBody, showSpinner: boolean = true): Observable<TResponse> {
    body = this.preprocessBody<TBody>(body);
    return super.put(url, body, showSpinner);
  }

  public delete<TResponse>(url: string, showSpinner: boolean = true): Observable<TResponse> {
    return super.delete(url, showSpinner);
  }

  private preprocessBody<TBody>(body: TBody): TBody {
    if (this.subModule === 'csl') {
      body = body ? body : <TBody>{};
      // tslint:disable-next-line:no-string-literal
      body['communityId'] = this.communityId;
      // tslint:disable-next-line:no-string-literal
      body['subModule'] = this.subModule;
    }

    return body;
  }
}
