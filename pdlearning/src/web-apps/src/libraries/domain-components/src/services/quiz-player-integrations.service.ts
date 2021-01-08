import { BehaviorSubject, Observable } from 'rxjs';
import { Injectable, NgZone } from '@angular/core';

import { AuthService } from '@opal20/authentication';
import { FormAnswerModel } from '@opal20/domain-api';

// @dynamic
@Injectable()
export class QuizPlayerIntegrationsService {
  public get formId$(): Observable<string> {
    return this._formIdSubject.asObservable().pipe();
  }

  public get myCourseId$(): Observable<string> {
    return this._myCourseIdSubject.asObservable().pipe();
  }

  public get resourceId$(): Observable<string> {
    return this._resourceIdSubject.asObservable().pipe();
  }

  public get classRunId$(): Observable<string> {
    return this._classRunIdSubject.asObservable().pipe();
  }

  public get assignmentId$(): Observable<string> {
    return this._assignmentIdSubject.asObservable().pipe();
  }

  public get isPassingMarkEnabled$(): Observable<boolean> {
    return this._isPassingMarkEnabledSubject.asObservable().pipe();
  }

  public get reviewOnly$(): Observable<boolean> {
    return this._reviewOnlySubject.asObservable().pipe();
  }

  public get formOriginalObjectId$(): Observable<string> {
    return this._formOriginalObjectIdSubject.asObservable().pipe();
  }

  /**
   * This function to support html page in a mobile web view can call a native code of mobile function.
   * We don't use it for now, but please don't remove it
   */
  private _formIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.quizPlayerIntegrations.currentFormId);
  private _resourceIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.quizPlayerIntegrations.currentResourceId);
  private _classRunIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.quizPlayerIntegrations.currentClassRunId);
  private _myCourseIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.quizPlayerIntegrations.currentMyCourseId);
  private _assignmentIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.quizPlayerIntegrations.currentAssignmentId);
  private _isPassingMarkEnabledSubject: BehaviorSubject<boolean> = new BehaviorSubject(
    AppGlobal.quizPlayerIntegrations.isPassingMarkEnabled
  );
  private _reviewOnlySubject: BehaviorSubject<boolean> = new BehaviorSubject(AppGlobal.quizPlayerIntegrations.reviewOnly);

  private _authTokenSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.quizPlayerIntegrations.currentAuthToken);

  private _formOriginalObjectIdSubject: BehaviorSubject<string> = new BehaviorSubject(
    AppGlobal.quizPlayerIntegrations.currentFormOriginalObjectId
  );

  constructor(private ngZone: NgZone, private authSvc: AuthService) {
    this.init();
  }

  public setup(options: {
    onQuizExited?: (formAnswerModel: FormAnswerModel) => void;
    onQuizFinished?: (formAnswerModel: FormAnswerModel) => void;
    onQuizFinishedForMobile?: () => void;
    onQuizInitiated?: () => void;
    onQuizSubmitted?: (formAnswerModel: FormAnswerModel) => void;
    onQuizTimeout?: (formAnswerModel: FormAnswerModel) => void;
  }): void {
    AppGlobal.quizPlayerIntegrations.onQuizFinished = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizFinished != null) {
          options.onQuizFinished(new FormAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
    AppGlobal.quizPlayerIntegrations.onQuizExited = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizExited != null) {
          options.onQuizExited(new FormAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
    AppGlobal.quizPlayerIntegrations.onQuizFinishedForMobile = () => {
      this.ngZone.run(() => {
        if (options.onQuizFinishedForMobile != null) {
          options.onQuizFinishedForMobile();
        }
      });
    };
    AppGlobal.quizPlayerIntegrations.onQuizInitiated = () => {
      this.ngZone.run(() => {
        if (options.onQuizInitiated != null) {
          options.onQuizInitiated();
        }
      });
    };
    AppGlobal.quizPlayerIntegrations.onQuizSubmitted = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizSubmitted != null) {
          options.onQuizSubmitted(new FormAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
    AppGlobal.quizPlayerIntegrations.onQuizTimeout = (formAnswerModelJsonString: string) => {
      this.ngZone.run(() => {
        if (options.onQuizTimeout != null) {
          options.onQuizTimeout(new FormAnswerModel(JSON.parse(formAnswerModelJsonString)));
        }
      });
    };
  }

  public init(): void {
    AppGlobal.quizPlayerIntegrations.setFormId = (formId: string) => {
      this.ngZone.run(() => {
        AppGlobal.quizPlayerIntegrations.currentFormId = formId;
        this._formIdSubject.next(formId);
      });
    };
    AppGlobal.quizPlayerIntegrations.setResourceId = (resourceId: string) => {
      this.ngZone.run(() => {
        AppGlobal.quizPlayerIntegrations.currentResourceId = resourceId;
        this._resourceIdSubject.next(resourceId);
      });
    };
    AppGlobal.quizPlayerIntegrations.setClassRunId = (classRunId: string) => {
      this.ngZone.run(() => {
        AppGlobal.quizPlayerIntegrations.currentClassRunId = classRunId;
        this._classRunIdSubject.next(classRunId);
      });
    };
    AppGlobal.quizPlayerIntegrations.setMyCourseId = (myCourseId: string) => {
      this.ngZone.run(() => {
        AppGlobal.quizPlayerIntegrations.currentMyCourseId = myCourseId;
        this._myCourseIdSubject.next(myCourseId);
      });
    };
    AppGlobal.quizPlayerIntegrations.setAssignmentId = (assignmentId: string) => {
      this.ngZone.run(() => {
        AppGlobal.quizPlayerIntegrations.currentAssignmentId = assignmentId;
        this._assignmentIdSubject.next(assignmentId);
      });
    };
    AppGlobal.quizPlayerIntegrations.setPassingRateEnableStringValue = (value: 'true' | 'false') => {
      this.ngZone.run(() => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        AppGlobal.quizPlayerIntegrations.isPassingMarkEnabled = enable;
        this._isPassingMarkEnabledSubject.next(enable);
      });
    };
    AppGlobal.quizPlayerIntegrations.setReviewOnlyStringValue = (value: 'true' | 'false') => {
      this.ngZone.run(() => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        AppGlobal.quizPlayerIntegrations.reviewOnly = enable;
        this._reviewOnlySubject.next(enable);
      });
    };
    AppGlobal.quizPlayerIntegrations.setAuthToken = (authToken: string) => {
      this.ngZone.run(() => {
        AppGlobal.quizPlayerIntegrations.currentAuthToken = authToken;
        this.authSvc.setAccessToken(authToken);
        this._authTokenSubject.next(authToken);
      });
    };
    AppGlobal.quizPlayerIntegrations.onQuizInitiated();
    if (AppGlobal.quizPlayerIntegrations.currentAuthToken !== undefined && AppGlobal.quizPlayerIntegrations.currentAuthToken !== '') {
      this.authSvc.setAccessToken(AppGlobal.quizPlayerIntegrations.currentAuthToken);
    }

    AppGlobal.quizPlayerIntegrations.setFormOriginalObjectId = (formOriginalObjectId: string) => {
      this.ngZone.run(() => {
        AppGlobal.quizPlayerIntegrations.currentFormOriginalObjectId = formOriginalObjectId;
        this._formOriginalObjectIdSubject.next(formOriginalObjectId);
      });
    };
  }

  public setFormId(formId: string): void {
    AppGlobal.quizPlayerIntegrations.setFormId(formId);
  }
  public setResourceId(resourceId: string): void {
    AppGlobal.quizPlayerIntegrations.setResourceId(resourceId);
  }
  public setClassRunId(classRunId: string): void {
    AppGlobal.quizPlayerIntegrations.setClassRunId(classRunId);
  }
  public setMyCourseId(myCourseId: string): void {
    AppGlobal.quizPlayerIntegrations.setMyCourseId(myCourseId);
  }
  public setAssignmentId(assignmentId: string): void {
    AppGlobal.quizPlayerIntegrations.setAssignmentId(assignmentId);
  }
  public setPassingRateEnable(enable: boolean): void {
    AppGlobal.quizPlayerIntegrations.setPassingRateEnableStringValue(enable ? 'true' : 'false');
  }
  public setReviewOnly(enable: boolean): void {
    AppGlobal.quizPlayerIntegrations.setReviewOnlyStringValue(enable ? 'true' : 'false');
  }
  public notifyQuizFinished(formAnswerModel: FormAnswerModel): void {
    AppGlobal.quizPlayerIntegrations.onQuizFinished(JSON.stringify(formAnswerModel));
    AppGlobal.quizPlayerIntegrations.onQuizFinishedForMobile();
  }
  public notifyQuizExit(formAnswerModel: FormAnswerModel): void {
    AppGlobal.quizPlayerIntegrations.onQuizExited(JSON.stringify(formAnswerModel));
  }
  public notifyQuizSubmitted(formAnswerModel: FormAnswerModel): void {
    AppGlobal.quizPlayerIntegrations.onQuizSubmitted(JSON.stringify(formAnswerModel));
  }
  public notifyQuizInitiated(): void {
    AppGlobal.quizPlayerIntegrations.onQuizInitiated();
  }
  public notifyQuizTimeout(formAnswerModel: FormAnswerModel): void {
    AppGlobal.quizPlayerIntegrations.onQuizTimeout(JSON.stringify(formAnswerModel));
  }

  public setFormOriginalObjectId(formOriginalObjectId: string): void {
    AppGlobal.quizPlayerIntegrations.setFormOriginalObjectId(formOriginalObjectId);
  }
}
