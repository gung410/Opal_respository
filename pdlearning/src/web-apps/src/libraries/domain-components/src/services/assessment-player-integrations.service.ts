import { BehaviorSubject, Observable } from 'rxjs';
import { Injectable, NgZone } from '@angular/core';

import { AssessmentAnswer } from '@opal20/domain-api';
import { AuthService } from '@opal20/authentication';

// @dynamic
@Injectable()
export class AssessmentPlayerIntegrationsService {
  public get assessmentId$(): Observable<string> {
    return this._assessmentIdSubject.asObservable().pipe();
  }

  public get assessmentAnswerId$(): Observable<string> {
    return this._assessmentAnswerIdSubject.asObservable().pipe();
  }

  public get participantAssignmentTrackId$(): Observable<string> {
    return this._participantAssignmentTrackIdSubject.asObservable().pipe();
  }

  public get userId$(): Observable<string> {
    return this._userIdSubject.asObservable().pipe();
  }

  /**
   * This function to support html page in a mobile web view can call a native code of mobile function.
   * We don't use it for now, but please don't remove it
   */
  private _assessmentIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.assessmentPlayerIntegrations.currentAssessmentId);
  private _assessmentAnswerIdSubject: BehaviorSubject<string> = new BehaviorSubject(
    AppGlobal.assessmentPlayerIntegrations.currentAssessmentAnswerId
  );
  private _participantAssignmentTrackIdSubject: BehaviorSubject<string> = new BehaviorSubject(
    AppGlobal.assessmentPlayerIntegrations.currentParticipantAssignmentTrackId
  );
  private _userIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.assessmentPlayerIntegrations.currentUserId);

  private _authTokenSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.assessmentPlayerIntegrations.currentAuthToken);

  constructor(private ngZone: NgZone, private authSvc: AuthService) {
    this.init();
  }

  public setup(options: {
    onAssessmentInitiated?: () => void;
    onAssessmentSaved?: (answer: AssessmentAnswer) => void;
    onAssessmentSubmitted?: (answer: AssessmentAnswer) => void;
    onAssessmentBack?: () => void;
  }): void {
    AppGlobal.assessmentPlayerIntegrations.onAssessmentInitiated = () => {
      this.ngZone.run(() => {
        if (options.onAssessmentInitiated != null) {
          options.onAssessmentInitiated();
        }
      });
    };
    AppGlobal.assessmentPlayerIntegrations.onAssessmentSaved = (jsonString: string) => {
      this.ngZone.run(() => {
        if (options.onAssessmentSaved != null) {
          options.onAssessmentSaved(new AssessmentAnswer(JSON.parse(jsonString)));
        }
      });
    };
    AppGlobal.assessmentPlayerIntegrations.onAssessmentSubmitted = (jsonString: string) => {
      this.ngZone.run(() => {
        if (options.onAssessmentSubmitted != null) {
          options.onAssessmentSubmitted(new AssessmentAnswer(JSON.parse(jsonString)));
        }
      });
    };
    AppGlobal.assessmentPlayerIntegrations.onAssessmentBack = () => {
      this.ngZone.run(() => {
        if (options.onAssessmentBack != null) {
          options.onAssessmentBack();
        }
      });
    };
  }

  public init(): void {
    AppGlobal.assessmentPlayerIntegrations.setAssessmentId = (assessmentId: string) => {
      this.ngZone.run(() => {
        AppGlobal.assessmentPlayerIntegrations.currentAssessmentId = assessmentId;
        this._assessmentIdSubject.next(assessmentId);
      });
    };
    AppGlobal.assessmentPlayerIntegrations.setAssessmentAnswerId = (assessmentAnswerId: string) => {
      this.ngZone.run(() => {
        AppGlobal.assessmentPlayerIntegrations.currentAssessmentAnswerId = assessmentAnswerId;
        this._assessmentAnswerIdSubject.next(assessmentAnswerId);
      });
    };
    AppGlobal.assessmentPlayerIntegrations.setParticipantAssignmentTrackId = (participantAssignmentTrackId: string) => {
      this.ngZone.run(() => {
        AppGlobal.assessmentPlayerIntegrations.currentParticipantAssignmentTrackId = participantAssignmentTrackId;
        this._participantAssignmentTrackIdSubject.next(participantAssignmentTrackId);
      });
    };
    AppGlobal.assessmentPlayerIntegrations.setUserId = (userId: string) => {
      this.ngZone.run(() => {
        AppGlobal.assessmentPlayerIntegrations.currentUserId = userId;
        this._userIdSubject.next(userId);
      });
    };
    AppGlobal.assessmentPlayerIntegrations.setAuthToken = (authToken: string) => {
      this.ngZone.run(() => {
        AppGlobal.assessmentPlayerIntegrations.currentAuthToken = authToken;
        this.authSvc.setAccessToken(authToken);
        this._authTokenSubject.next(authToken);
      });
    };
    AppGlobal.assessmentPlayerIntegrations.onAssessmentInitiated();
    if (
      AppGlobal.assessmentPlayerIntegrations.currentAuthToken !== undefined &&
      AppGlobal.assessmentPlayerIntegrations.currentAuthToken !== ''
    ) {
      this.authSvc.setAccessToken(AppGlobal.assessmentPlayerIntegrations.currentAuthToken);
    }
  }
  public setAssessmentId(assessmentId: string): void {
    AppGlobal.assessmentPlayerIntegrations.setAssessmentId(assessmentId);
  }
  public setAssessmentAnswerId(assessmentAnswerId: string): void {
    AppGlobal.assessmentPlayerIntegrations.setAssessmentAnswerId(assessmentAnswerId);
  }
  public setParticipantAssignmentTrackId(participantAssignmentTrackId: string): void {
    AppGlobal.assessmentPlayerIntegrations.setParticipantAssignmentTrackId(participantAssignmentTrackId);
  }
  public setUserId(userId: string): void {
    AppGlobal.assessmentPlayerIntegrations.setUserId(userId);
  }

  public notifyAssessmentSaved(answer: AssessmentAnswer): void {
    AppGlobal.assessmentPlayerIntegrations.onAssessmentSaved(JSON.stringify(answer));
  }
  public notifyAssessmentSubmitted(answer: AssessmentAnswer): void {
    AppGlobal.assessmentPlayerIntegrations.onAssessmentSubmitted(JSON.stringify(answer));
  }
  public notifyAssessmentInitiated(): void {
    AppGlobal.assessmentPlayerIntegrations.onAssessmentInitiated();
  }

  public notifyAssessmentBack(): void {
    AppGlobal.assessmentPlayerIntegrations.onAssessmentBack();
  }
}
