import { HttpErrorResponse, HttpEvent, HttpHandler, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable, throwError } from 'rxjs';

import { APP_BASE_HREF } from '@angular/common';
import { BaseInterceptor } from '../base-interceptor';
import { GlobalTranslatorService } from '../../translation/global-translator.service';
import { HttpErrorCode } from '../models/http-error-code.enum';
import { InterceptorType } from '../interceptor-registry';
import { ModalService } from '../../services/modal.service';
import { TranslationMessage } from '../../translation/translation.models';
import { Utils } from '../../utils/utils';
import { WindowContainerService } from '@progress/kendo-angular-dialog';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HttpResponseInterceptor extends BaseInterceptor {
  protected key: string = InterceptorType.HttpResponse;

  constructor(protected injector: Injector, private modalService: ModalService) {
    super(injector);
  }

  public handle(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const invalidDataMessage = 'The data that you have entered is not allowed.';
    const found = req.body != null ? Utils.checkXssScript(req.body) : null;
    if (found && found.length > 0) {
      this.showError(invalidDataMessage);
      return throwError(invalidDataMessage);
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        this.handleError(error);

        return throwError(error);
      })
    );
  }

  protected handleConnectionError(errorMessage: string): void {
    this.showError(errorMessage);
  }

  private handleError(response: HttpErrorResponse): void {
    const genericErrorMessage: string = 'An error has occurred. Please contact the Helpdesk if the issue persists.';

    switch (response.status) {
      case 401:
      case 403:
        if (this.isSingleSessionError(response)) {
          const moduleId: string = AppGlobal.getModuleIdFromUrl(this.injector.get(APP_BASE_HREF, '/'));
          const containerService: WindowContainerService = this.injector.get(WindowContainerService, null);
          const callback: () => void = () => {
            localStorage.setItem('opal20_redirect_module_id', moduleId);
            AppGlobal.logoutFn();
          };

          if (containerService.container) {
            const loggedOutMessage =
              // tslint:disable-next-line:max-line-length
              'You have been logged out of the OPAL2.0 session as your account has been logged in from another device/location.\nIf you have not authorised the login, please reset your password and/or contact your administrator.';
            this.modalService.showErrorMessage(loggedOutMessage, callback, callback);
          } else if (AppGlobal.logoutFn) {
            callback();
          }
          return;
        } else if (
          AppGlobal.quizPlayerIntegrations.onAuthTokenError !== undefined ||
          AppGlobal.standaloneSurveyIntegration.onAuthTokenError !== undefined ||
          AppGlobal.scormPlayerIntegrations.onAuthTokenError !== undefined ||
          AppGlobal.digitalContentPlayerIntergrations.onAuthTokenError !== undefined ||
          AppGlobal.assignmentPlayerIntegrations.onAuthTokenError !== undefined
        ) {
          if (AppGlobal.quizPlayerIntegrations.onAuthTokenError !== undefined) {
            AppGlobal.quizPlayerIntegrations.onAuthTokenError(JSON.stringify(response));
          }
          if (AppGlobal.standaloneSurveyIntegration.onAuthTokenError !== undefined) {
            AppGlobal.standaloneSurveyIntegration.onAuthTokenError(JSON.stringify(response));
          }
          if (AppGlobal.scormPlayerIntegrations.onAuthTokenError !== undefined) {
            AppGlobal.scormPlayerIntegrations.onAuthTokenError(JSON.stringify(response));
          }
          if (AppGlobal.digitalContentPlayerIntergrations.onAuthTokenError !== undefined) {
            AppGlobal.digitalContentPlayerIntergrations.onAuthTokenError(JSON.stringify(response));
          }
          if (AppGlobal.assignmentPlayerIntegrations.onAuthTokenError !== undefined) {
            AppGlobal.assignmentPlayerIntegrations.onAuthTokenError(JSON.stringify(response));
          }
        } else {
          this.showError('The data that you have entered is not allowed.');
        }
        break;
      case 400:
        const httpError = response.error;
        // Could not detect detail error => show a generic error message
        if (!httpError || !httpError.error) {
          this.showError(genericErrorMessage);
          return;
        }

        const errorResponse = httpError.error;
        const errorCode: string = errorResponse.code;
        const errorMessage: string = errorResponse.message || genericErrorMessage;

        if (errorCode === HttpErrorCode.DataValidationException) {
          this.showError('Data is invalid. Please try again.');
          return;
        }

        if (errorCode === HttpErrorCode.BusinessLogicException) {
          this.showBusinessError(errorMessage);
          return;
        }

        this.showError(errorMessage);
        break;
      case 0:
        this.handleConnectionError(genericErrorMessage);
        break;
      default:
        this.showError(genericErrorMessage);
        return;
    }
  }
  private showError(message: string): void {
    const translator: GlobalTranslatorService = this.injector.get(GlobalTranslatorService);
    this.modalService.showErrorMessage(new TranslationMessage(translator, message));
  }

  private showBusinessError(message: string): void {
    const translator: GlobalTranslatorService = this.injector.get(GlobalTranslatorService);
    this.modalService.showWarningMessage(new TranslationMessage(translator, message));
  }

  private isSingleSessionError(response: HttpErrorResponse): boolean {
    return (
      response &&
      response.error &&
      response.error.error &&
      (response.error.error.code === HttpErrorCode.AuthenticationException ||
        (response.error.error.indexOf != null &&
          (response.error.error.indexOf('Key not authorised') > -1 || response.error.error.indexOf('Key not authorized') > -1)))
    );
  }
}
