import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxInformationDialogService,
} from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { ErrorAPI } from 'app-models/auth.model';
import { environment } from 'environments/environment';
import { ToastrService } from 'ngx-toastr';
// tslint:disable-next-line:import-blacklist
import { Observable } from 'rxjs/Rx';
import * as Survey from 'survey-angular';
import { v4 as uuid } from 'uuid';
import { AppConstant, Constant, RouteConstant } from '../app.constant';

export enum HttpStatusCode {
  Unknown = 0,
  Unauthorized = 401,
  Forbidden = 403,
  NotFound = 404,
  RequestTimeout = 408,
  InternalServerError = 500,
  ServiceUnavailable = 503,
  GatewayTimeout = 504,
  HttpVersionNotSupported = 505,
}

@Injectable()
export class AuthHttpInterceptorService implements HttpInterceptor {
  cxToken: string;

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

    // interceptor adjust request url to support run behine virtual path
    const urlString = req.url;
    if (environment.VirtualPath) {
      if (
        req.method === 'GET' &&
        urlString.indexOf('https://') === -1 &&
        urlString.indexOf(environment.VirtualPath) === -1
      ) {
        const httpRequest = new HttpRequest(
          req.method,
          `/${environment.VirtualPath}${req.url}`
        );
        req = Object.assign(req, httpRequest);
      }
    }

    if (
      req.method === 'GET' ||
      req.method === 'POST' ||
      req.method === 'PUT' ||
      req.method === 'DELETE'
    ) {
      const headerContentType = req.headers.get('Content-type');
      const headerAcceptType = req.headers.get('Accept');
      const headerCorrelationId = req.headers.get('Correlation-Id');
      const headers = {
        cxToken: this.cxToken,
        Authorization: 'Bearer ' + accessToken,
        Accept: headerAcceptType
          ? headerAcceptType
          : 'application/json, text/plain, */*',
        'Access-Control-Expose-Headers': 'Correlation-Id',
        'Content-type': headerContentType
          ? headerContentType
          : 'application/json; charset=utf-8',
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Language': languageCode,
        'Accept-Language': languageCode,
        'Correlation-Id': headerCorrelationId ? headerCorrelationId : uuid(),
      };

      if (req.method === 'GET' || req.body instanceof FormData) {
        // In Angular 5+, including the header Content-Type can invalidate your request.
        delete headers['Content-type'];
      }

      authReq = req.clone({ setHeaders: headers });
    }

    const skipShowHTTPError =
      req.headers &&
      req.headers.has(AppConstant.httpRequestAvoidIntercepterCatchError);

    return next
      .handle(authReq)
      .catch((errorResponse: HttpErrorResponse) => {
        return this.handleError(errorResponse, skipShowHTTPError);
      })
      .timeout(Constant.REQUEST_TIME_OUT);
  }

  private handleSurveyjsRequest(): void {
    const authService: AuthService = this.injector.get(AuthService);
    let languageCode = localStorage.getItem('language-code');
    if (languageCode === null) {
      languageCode = environment.fallbackLanguage;
    }
    Survey.ChoicesRestfull.onBeforeSendRequest = (
      sender: any,
      options: any
    ) => {
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
      options.request.setRequestHeader('cxToken', this.cxToken);
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
    skipShowHTTPError: boolean
  ): Observable<any> {
    const cxGlobalLoaderService: CxGlobalLoaderService = this.injector.get(
      CxGlobalLoaderService
    );
    cxGlobalLoaderService.hideLoader();
    const siteEndpoint = `/${environment.apiGatewayResource.portal}/sites`;

    if (httpErrorResponse.url.includes(siteEndpoint)) {
      this.handleExceptionForSiteEndpoint(httpErrorResponse);

      return Observable.of(httpErrorResponse);
    }

    switch (httpErrorResponse.status) {
      case HttpStatusCode.Unauthorized:
        this.showErrorCodeMessage(HttpStatusCode.Unauthorized);
        return Observable.of(httpErrorResponse);

      case HttpStatusCode.Forbidden:
        this.handleForbiddenException(httpErrorResponse);
        return Observable.of(httpErrorResponse);

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
    // Prevent logout if current page is MPJ using at Learner Module
    const isHandledForIframe = this.checkAndHandleIfInAIframe();
    if (isHandledForIframe) {
      return;
    }

    const invalidSessionText = this.translate(
      'RequestErrorMessage.InvalidSession'
    );
    this.informationDialogService
      .error({ message: invalidSessionText })
      .then(() => this.logOut());
  }

  private logOut(): void {
    // Prevent logout if current page is MPJ using at Learner Module
    const isHandledForIframe = this.checkAndHandleIfInAIframe();
    if (isHandledForIframe) {
      return;
    }

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

  private checkAndHandleIfInAIframe(): boolean {
    if (!this.isInIframe) {
      return false;
    }
    this.navigateTo(RouteConstant.ERROR_COMMON);

    return true;
  }

  private get isInIframe(): boolean {
    try {
      return window.self !== window.top;
    } catch (e) {
      return true;
    }
  }

  private navigateToLearnerIfForbiddenUser(): void {
    location.href = environment.moduleLink.learner;
  }
}
