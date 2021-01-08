import { HttpHeaders, HttpParams } from '@angular/common/http';

export interface IHttpOptions {
  headers?:
    | HttpHeaders
    | {
        [header: string]: string | string[];
      };
  params?:
    | HttpParams
    | {
        [param: string]: string | string[];
      };
  observe?: 'body';
  responseType?: 'json';
}

export interface DownloadFileIHttpOptions {
  headers?:
    | HttpHeaders
    | {
        [header: string]: string | string[];
      };
  observe: 'response';
  params?:
    | HttpParams
    | {
        [param: string]: string | string[];
      };
  reportProgress?: boolean;
  responseType: 'blob';
  withCredentials?: boolean;
}
