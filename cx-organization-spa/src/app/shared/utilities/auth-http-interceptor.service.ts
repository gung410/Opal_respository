import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxInformationDialogService
} from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { environment } from 'environments/environment';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs/Rx';
import * as Survey from 'survey-angular';
import { v4 as uuid } from 'uuid';

import { ErrorAPI } from 'app-models/auth.model';
import { AppConstant, Constant, RouteConstant } from '../app.constant';
import { findIndexCommon } from '../constants/common.const';

export enum HttpStatusCode {
  Unknown = 0,
  Unauthorized = 401,
  Forbidden = 403,
  NotFound = 404,
  RequestTimeout = 408,
  InternalServerError = 500,
  ServiceUnavailable = 503,
  GatewayTimeout = 504,
  HttpVersionNotSupported = 505
}

// tslint:disable-next-line:import-blacklist
@Injectable()
export class AuthHttpInterceptorService implements HttpInterceptor {
  REQUEST_TIME_OUT: number = 60000;
  private cxToken: string;
  constructor(
    private injector: Injector,
    private router: Router,
    private informationDialogService: CxInformationDialogService
  ) {
    this.cxToken = `${environment.OwnerId}:${environment.CustomerId}`;
    this.handleSurveyjsRequest();
  }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const authService: AuthService = this.injector.get(AuthService);
    const accessToken = authService.getAccessToken();
    let languageCode = localStorage.getItem('language-code');
    if (languageCode === null) {
      languageCode = environment.fallbackLanguage;
    }
    // Clone the request to add the new header.
    let authReq: HttpRequest<any>;

    if (
      req.method === 'GET' ||
      req.method === 'POST' ||
      req.method === 'PUT' ||
      req.method === 'DELETE'
    ) {
      const headerContentType = req.headers.get('Content-type');
      const headers = {
        cxToken: this.cxToken,
        Authorization: 'Bearer ' + accessToken,
        Accept: req.headers.get('Accept')
          ? req.headers.get('Accept')
          : 'application/json, text/plain, */*',
        'Access-Control-Expose-Headers': 'Correlation-Id',
        'Content-type': headerContentType
          ? headerContentType
          : 'application/json; charset=utf-8',
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Language': languageCode,
        'Accept-Language': languageCode,
        'Correlation-Id': uuid()
      };

      if (req.method === 'GET' || req.body instanceof FormData) {
        delete headers['Content-type'];
      }

      // For now, don't send some headers to report module since they don't handle it.
      if (req.url.indexOf('checkFileExists') !== findIndexCommon.notFound) {
        delete headers.Authorization;
        delete headers['Access-Control-Expose-Headers'];
      }

      authReq = req.clone({ setHeaders: headers });
    }

    const skipShowHTTPError =
      req.headers &&
      req.headers.get(AppConstant.httpRequestAvoidIntercepterCatchError)
        ? JSON.parse(
            req.headers.get(AppConstant.httpRequestAvoidIntercepterCatchError)
          )
        : false;
    const skipHideLoaderWhenError =
      req.headers &&
      req.headers[AppConstant.httpRequestAvoidHideLoaderWhenError];

    return next
      .handle(authReq)
      .catch((errorResponse: HttpErrorResponse) => {
        return this.handleError(
          errorResponse,
          skipShowHTTPError,
          skipHideLoaderWhenError
        );
      })
      .timeout(this.REQUEST_TIME_OUT);
  }

  private handleSurveyjsRequest(): void {
    const authService: AuthService = this.injector.get(AuthService);
    let languageCode = localStorage.getItem('language-code');
    if (languageCode === null) {
      languageCode = environment.fallbackLanguage;
    }
    Survey.ChoicesRestfull.onBeforeSendRequest = (sender: any, options) => {
      // System is using a variable for the api url putted into the choicesByUrl config
      //  but the variable in the survey js variable is encoded
      //  so we need to decode this api url.
      const decodePathURI = (requestUrl: string): string => {
        const queryStringSeparator = '?';
        const segments = requestUrl.split(queryStringSeparator); // split the query string parameters and the url path.
        const result = [];
        for (let index = 0; index < segments.length; index++) {
          if (index === 0) {
            result.push(decodeURIComponent(segments[index]));
          } else {
            result.push(segments[index]);
          }
        }

        return result.join(queryStringSeparator);
      };
      // Set the request with the api url that has been fixed (decoded).
      options.request.open('GET', decodePathURI(sender.processedUrl));
      // Set request headers.
      options.request.setRequestHeader(
        'cxToken',
        `${AppConstant.ownerId}:${AppConstant.customerId}`
      );
      options.request.setRequestHeader(
        'Authorization',
        'Bearer ' + authService.getAccessToken()
      );
      options.request.setRequestHeader(
        'Accept',
        'application/json, text/plain, */*'
      );
      options.request.setRequestHeader(
        'Access-Control-Expose-Headers',
        'Correlation-Id'
      );
      options.request.setRequestHeader('Content-Language', languageCode);
      options.request.setRequestHeader('Accept-Language', languageCode);
      options.request.setRequestHeader('Correlation-Id', uuid());
    };
  }

  private handleError(
    httpErrorResponse: HttpErrorResponse,
    skipShowHTTPError: boolean,
    skipHideLoaderWhenError: boolean
  ): Observable<any> {
    const cxGlobalLoaderService: CxGlobalLoaderService = this.injector.get(
      CxGlobalLoaderService
    );
    if (!skipHideLoaderWhenError) {
      cxGlobalLoaderService.hideLoader();
    }
    const siteEndpoint = `/${environment.apiGatewayResource.portal}/sites`;

    if (httpErrorResponse.url.includes(siteEndpoint)) {
      this.handleExceptionForSiteEndpoint(httpErrorResponse);

      return Observable.throwError(httpErrorResponse);
    }

    switch (httpErrorResponse.status) {
      case HttpStatusCode.Unauthorized:
        this.showErrorCodeMessage(HttpStatusCode.Unauthorized);
        break;
      case HttpStatusCode.Forbidden:
        this.handleForbiddenException(httpErrorResponse);
        break;
      case HttpStatusCode.NotFound:
      case HttpStatusCode.RequestTimeout:
      case HttpStatusCode.InternalServerError:
      case HttpStatusCode.ServiceUnavailable:
      case HttpStatusCode.GatewayTimeout:
        if (!skipShowHTTPError) {
          this.showErrorCodeMessage(httpErrorResponse.status);
        }
        break;
      default:
        if (!skipShowHTTPError) {
          this.showErrorCodeMessage();
        }
        break;
    }

    return Observable.throwError(httpErrorResponse);
  }

  private handleExceptionForSiteEndpoint(
    httpErrorResponse: HttpErrorResponse
  ): void {
    if (httpErrorResponse.status === HttpStatusCode.Unauthorized) {
      this.logOut();

      return;
    }

    if (
      httpErrorResponse.status === HttpStatusCode.Forbidden &&
      httpErrorResponse.error
    ) {
      const currentError = httpErrorResponse.error as ErrorAPI;
      const isNotAuthorized =
        currentError.error.includes(Constant.KEY_NOT_AUTHORISE_ERROR_401) ||
        currentError.error.includes(Constant.KEY_NOT_AUTHORISE_ERROR_401);

      // Check valid valid token
      if (isNotAuthorized) {
        this.logOut();

        return;
      }

      if (currentError.errorCode === Constant.FORBBIDEN_USER_ERROR_CODE) {
        this.navigateToLearnerIfForbiddenUser();

        return;
      }

      if (currentError.errorCode === Constant.PENDING_USER_ERROR_CODE) {
        this.navigateTo(RouteConstant.ERROR_PENDING_USER);

        return;
      }
    }

    // Case can't detect error
    this.navigateTo(RouteConstant.ERROR_COMMON);
  }

  private handleForbiddenException(httpErrorResponse: HttpErrorResponse): void {
    if (httpErrorResponse.error) {
      const currentError = httpErrorResponse.error as ErrorAPI;

      if (currentError.errorCode === Constant.FORBBIDEN_USER_ERROR_CODE) {
        this.navigateToLearnerIfForbiddenUser();

        return;
      }

      if (currentError.errorCode === Constant.PENDING_USER_ERROR_CODE) {
        this.navigateTo(RouteConstant.ERROR_PENDING_USER);

        return;
      }

      // Case key invalid session
      const isInvalidSession = currentError.error.includes(
        Constant.INVALID_SESSION_ERROR_403
      );
      if (isInvalidSession) {
        this.showInvalidSessionPopup();

        return;
      }
    }

    // Case default
    this.showErrorCodeMessage(HttpStatusCode.Forbidden);
  }

  private showErrorCodeMessage(errorCode?: number): void {
    const toastrService: ToastrService = this.injector.get(ToastrService);

    let msg: string;
    if (errorCode) {
      msg =
        'Error ' +
        errorCode +
        ': ' +
        this.translate(`RequestErrorMessage.${errorCode}`);
    } else {
      msg = this.translate(`RequestErrorMessage.Common`);
    }

    toastrService.error(msg);
  }

  private showInvalidSessionPopup(): void {
    const invalidSessionText = this.translate(
      'RequestErrorMessage.InvalidSession'
    );
    this.informationDialogService
      .error({ message: invalidSessionText })
      .then(() => this.logOut());
  }

  private logOut(): void {
    const authService: AuthService = this.injector.get(AuthService);
    authService.logout();
  }

  private navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  private translate(path: string): string {
    const translateService: TranslateService = this.injector.get(
      TranslateService
    );

    return translateService.instant(path) as string;
  }

  private navigateToLearnerIfForbiddenUser(): void {
    location.href = AppConstant.moduleLink.LearnerWeb;
  }
}
