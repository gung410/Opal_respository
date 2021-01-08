import { BehaviorSubject, Observable } from 'rxjs';
import { Injectable, NgZone } from '@angular/core';
import { ParticipantAssignmentTrack, QuizAssignmentFormQuestion } from '@opal20/domain-api';

import { AuthService } from '@opal20/authentication';

// @dynamic
@Injectable()
export class AssignmentPlayerIntegrationsService {
  public get assignmentId$(): Observable<string> {
    return this._assignmentIdSubject.asObservable().pipe();
  }

  public get participantAssignmentTrackId$(): Observable<string> {
    return this._participantAssignmentTrackIdSubject.asObservable().pipe();
  }

  /**
   * This function to support html page in a mobile web view can call a native code of mobile function.
   * We don't use it for now, but please don't remove it
   */
  private _participantAssignmentTrackIdSubject: BehaviorSubject<string> = new BehaviorSubject(
    AppGlobal.assignmentPlayerIntegrations.currentParticipantAssignmentTrackId
  );
  private _assignmentIdSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.assignmentPlayerIntegrations.currentAssignmentId);
  private _authTokenSubject: BehaviorSubject<string> = new BehaviorSubject(AppGlobal.assignmentPlayerIntegrations.currentAuthToken);

  constructor(private ngZone: NgZone, private authSvc: AuthService) {
    this.init();
  }

  public setup(options: {
    onAssignmentInitiated?: () => void;
    onAssignmentSaved?: (answer: ParticipantAssignmentTrack) => void;
    onAssignmentSubmitted?: (answer: ParticipantAssignmentTrack) => void;
    onAssignmentBack?: () => void;
    onAssignmentQuestionChanged?: (question: QuizAssignmentFormQuestion) => void;
  }): void {
    AppGlobal.assignmentPlayerIntegrations.onAssignmentInitiated = () => {
      this.ngZone.run(() => {
        if (options.onAssignmentInitiated != null) {
          options.onAssignmentInitiated();
        }
      });
    };
    AppGlobal.assignmentPlayerIntegrations.onAssignmentSaved = (jsonString: string) => {
      this.ngZone.run(() => {
        if (options.onAssignmentSaved != null) {
          options.onAssignmentSaved(new ParticipantAssignmentTrack(JSON.parse(jsonString)));
        }
      });
    };
    AppGlobal.assignmentPlayerIntegrations.onAssignmentSubmitted = (jsonString: string) => {
      this.ngZone.run(() => {
        if (options.onAssignmentSubmitted != null) {
          options.onAssignmentSubmitted(new ParticipantAssignmentTrack(JSON.parse(jsonString)));
        }
      });
    };
    AppGlobal.assignmentPlayerIntegrations.onAssignmentBack = () => {
      this.ngZone.run(() => {
        if (options.onAssignmentBack != null) {
          options.onAssignmentBack();
        }
      });
    };
    AppGlobal.assignmentPlayerIntegrations.onAssignmentQuestionChanged = (jsonString: string) => {
      this.ngZone.run(() => {
        if (options.onAssignmentQuestionChanged != null) {
          options.onAssignmentQuestionChanged(new QuizAssignmentFormQuestion(JSON.parse(jsonString)));
        }
      });
    };
  }

  public init(): void {
    AppGlobal.assignmentPlayerIntegrations.setParticipantAssignmentTrackId = (participantAssignmentTrackId: string) => {
      this.ngZone.run(() => {
        AppGlobal.assignmentPlayerIntegrations.currentParticipantAssignmentTrackId = participantAssignmentTrackId;
        this._participantAssignmentTrackIdSubject.next(participantAssignmentTrackId);
      });
    };
    AppGlobal.assignmentPlayerIntegrations.setAssignmentId = (assignmentId: string) => {
      this.ngZone.run(() => {
        AppGlobal.assignmentPlayerIntegrations.currentAssignmentId = assignmentId;
        this._assignmentIdSubject.next(assignmentId);
      });
    };
    AppGlobal.assignmentPlayerIntegrations.setAuthToken = (authToken: string) => {
      this.ngZone.run(() => {
        AppGlobal.assignmentPlayerIntegrations.currentAuthToken = authToken;
        this.authSvc.setAccessToken(authToken);
        this._authTokenSubject.next(authToken);
      });
    };
    AppGlobal.assignmentPlayerIntegrations.onAssignmentInitiated();
    if (
      AppGlobal.assignmentPlayerIntegrations.currentAuthToken !== undefined &&
      AppGlobal.assignmentPlayerIntegrations.currentAuthToken !== ''
    ) {
      this.authSvc.setAccessToken(AppGlobal.assignmentPlayerIntegrations.currentAuthToken);
    }
  }
  public setParticipantAssignmentTrackId(participantAssignmentTrackId: string): void {
    AppGlobal.assignmentPlayerIntegrations.setParticipantAssignmentTrackId(participantAssignmentTrackId);
  }
  public setAssignmentId(assignmentId: string): void {
    AppGlobal.assignmentPlayerIntegrations.setAssignmentId(assignmentId);
  }

  public notifyAssignmentSaved(answer: ParticipantAssignmentTrack): void {
    AppGlobal.assignmentPlayerIntegrations.onAssignmentSaved(JSON.stringify(answer));
  }
  public notifyAssignmentSubmitted(answer: ParticipantAssignmentTrack): void {
    AppGlobal.assignmentPlayerIntegrations.onAssignmentSubmitted(JSON.stringify(answer));
  }
  public notifyAssignmentInitiated(): void {
    AppGlobal.assignmentPlayerIntegrations.onAssignmentInitiated();
  }

  public notifyAssignmentBack(): void {
    AppGlobal.assignmentPlayerIntegrations.onAssignmentBack();
  }

  public notifyAssignmentQuestionChanged(question: QuizAssignmentFormQuestion): void {
    AppGlobal.assignmentPlayerIntegrations.onAssignmentQuestionChanged(JSON.stringify(question));
  }
}
