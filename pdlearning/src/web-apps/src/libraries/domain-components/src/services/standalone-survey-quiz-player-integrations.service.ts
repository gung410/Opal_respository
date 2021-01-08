import { BehaviorSubject, Observable } from 'rxjs';
import { Injectable, NgZone } from '@angular/core';

import { AuthService } from '@opal20/authentication';
import { SurveyAnswerModel } from '@opal20/domain-api';

// @dynamic
@Injectable()
export class StandaloneSurveyQuizPlayerIntegrationsService {
  public get formId$(): Observable<string> {
    return this._formIdSubject.asObservable().pipe();
  }

  public get resourceId$(): Observable<string> {
    return this._resourceIdSubject.asObservable().pipe();
  }

  public get isPassingMarkEnabled$(): Observable<boolean> {
    return this._isPassingMarkEnabledSubject.asObservable().pipe();
  }

  public get reviewOnly$(): Observable<boolean> {
    return this._reviewOnlySubject.asObservable().pipe();
  }

  private _formIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.standaloneSurveyIntegration.currentFormId);
  private _resourceIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.standaloneSurveyIntegration.currentResourceId);
  private _isPassingMarkEnabledSubject: BehaviorSubject<boolean> = new BehaviorSubject(
    AppGlobal.standaloneSurveyIntegration.isPassingMarkEnabled
  );
  private _reviewOnlySubject: BehaviorSubject<boolean> = new BehaviorSubject(AppGlobal.standaloneSurveyIntegration.reviewOnly);

  private _authTokenSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.standaloneSurveyIntegration.currentAuthToken);

  constructor(private ngZone: NgZone, private authSvc: AuthService) {
    this.init();
  }

  public setup(options: {
    onQuizExited?: (formAnswerModel: SurveyAnswerModel) => void;
    onQuizFinished?: (formAnswerModel: SurveyAnswerModel) => void;
    onQuizFinishedForMobile?: () => void;
    onQuizInitiated?: () => void;
    onQuizSubmitted?: (formAnswerModel: SurveyAnswerModel) => void;
    onQuizTimeout?: (formAnswerModel: SurveyAnswerModel) => void;
  }): void {
    AppGlobal.standaloneSurveyIntegration.onQuizFinished = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizFinished != null) {
          options.onQuizFinished(new SurveyAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
    AppGlobal.standaloneSurveyIntegration.onQuizExited = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizExited != null) {
          options.onQuizExited(new SurveyAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
    AppGlobal.standaloneSurveyIntegration.onQuizFinishedForMobile = () => {
      this.ngZone.run(() => {
        if (options.onQuizFinishedForMobile != null) {
          options.onQuizFinishedForMobile();
        }
      });
    };
    AppGlobal.standaloneSurveyIntegration.onQuizInitiated = () => {
      this.ngZone.run(() => {
        if (options.onQuizInitiated != null) {
          options.onQuizInitiated();
        }
      });
    };
    AppGlobal.standaloneSurveyIntegration.onQuizSubmitted = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizSubmitted != null) {
          options.onQuizSubmitted(new SurveyAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
    AppGlobal.standaloneSurveyIntegration.onQuizTimeout = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizTimeout != null) {
          options.onQuizTimeout(new SurveyAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
  }

  public init(): void {
    AppGlobal.standaloneSurveyIntegration.setFormId = (formId: string) => {
      this.ngZone.run(() => {
        AppGlobal.standaloneSurveyIntegration.currentFormId = formId;
        this._formIdSubject.next(formId);
      });
    };
    AppGlobal.standaloneSurveyIntegration.setResourceId = (resourceId: string) => {
      this.ngZone.run(() => {
        AppGlobal.standaloneSurveyIntegration.currentResourceId = resourceId;
        this._resourceIdSubject.next(resourceId);
      });
    };
    AppGlobal.standaloneSurveyIntegration.setPassingRateEnableStringValue = (value: 'true' | 'false') => {
      this.ngZone.run(() => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        AppGlobal.standaloneSurveyIntegration.isPassingMarkEnabled = enable;
        this._isPassingMarkEnabledSubject.next(enable);
      });
    };
    AppGlobal.standaloneSurveyIntegration.setReviewOnlyStringValue = (value: 'true' | 'false') => {
      this.ngZone.run(() => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        AppGlobal.standaloneSurveyIntegration.reviewOnly = enable;
        this._reviewOnlySubject.next(enable);
      });
    };
    AppGlobal.standaloneSurveyIntegration.setAuthToken = (authToken: string) => {
      this.ngZone.run(() => {
        AppGlobal.standaloneSurveyIntegration.currentAuthToken = authToken;
        this.authSvc.setAccessToken(authToken);
        this._authTokenSubject.next(authToken);
      });
    };
    AppGlobal.standaloneSurveyIntegration.onQuizInitiated();
    if (
      AppGlobal.standaloneSurveyIntegration.currentAuthToken !== undefined &&
      AppGlobal.standaloneSurveyIntegration.currentAuthToken !== ''
    ) {
      this.authSvc.setAccessToken(AppGlobal.standaloneSurveyIntegration.currentAuthToken);
    }
  }

  public setFormId(formId: string): void {
    AppGlobal.standaloneSurveyIntegration.setFormId(formId);
  }
  public setResourceId(resourceId: string): void {
    AppGlobal.standaloneSurveyIntegration.setResourceId(resourceId);
  }
  public setPassingRateEnable(enable: boolean): void {
    AppGlobal.standaloneSurveyIntegration.setPassingRateEnableStringValue(enable ? 'true' : 'false');
  }
  public setReviewOnly(enable: boolean): void {
    AppGlobal.standaloneSurveyIntegration.setReviewOnlyStringValue(enable ? 'true' : 'false');
  }
  public notifyQuizFinished(formAnswerModel: SurveyAnswerModel): void {
    AppGlobal.standaloneSurveyIntegration.onQuizFinished(JSON.stringify(formAnswerModel));
    AppGlobal.standaloneSurveyIntegration.onQuizFinishedForMobile();
  }
  public notifyQuizExit(formAnswerModel: SurveyAnswerModel): void {
    AppGlobal.standaloneSurveyIntegration.onQuizExited(JSON.stringify(formAnswerModel));
  }
  public notifyQuizSubmitted(formAnswerModel: SurveyAnswerModel): void {
    AppGlobal.standaloneSurveyIntegration.onQuizSubmitted(JSON.stringify(formAnswerModel));
  }
  public notifyQuizInitiated(): void {
    AppGlobal.standaloneSurveyIntegration.onQuizInitiated();
  }
  public notifyQuizTimeout(formAnswerModel: SurveyAnswerModel): void {
    AppGlobal.standaloneSurveyIntegration.onQuizTimeout(JSON.stringify(formAnswerModel));
  }
}
