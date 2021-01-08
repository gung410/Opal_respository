import { AssessmentAnswer, AssessmentAnswerRepository, ICreateAssessmentAnswerRequest, PublicUserInfo } from '@opal20/domain-api';
import {
  BaseFormComponent,
  IFormBuilderDefinition,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IAssessmentPlayerInput, ParticipantAssignmentTrackViewModel } from '@opal20/domain-components';
import { Observable, from } from 'rxjs';

import { PeerAssessmentPageComponentService } from '../services/peer-assessment-page-component.service';
import { PeerAssessmentViewModel } from '../view-models/peer-assessment-view.model';
import { map } from 'rxjs/operators';
import { requiredForListValidator } from '@opal20/common-components';

@Component({
  selector: 'peer-assessor-selection',
  templateUrl: './peer-assessor-selection.component.html'
})
export class PeerAssessorSelectionComponent extends BaseFormComponent {
  public get peerAssessmentVm(): PeerAssessmentViewModel {
    return this._peerAssessmentVm;
  }

  @Input()
  public set peerAssessmentVm(v: PeerAssessmentViewModel) {
    if (Utils.isDifferent(this._peerAssessmentVm, v)) {
      this._peerAssessmentVm = v;
    }
  }
  public get courseId(): string | undefined {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string | undefined) {
    if (Utils.isDifferent(this._courseId, v)) {
      this._courseId = v;
    }
  }

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
    }
  }

  public get assignmentId(): string | undefined {
    return this._assignmentId;
  }

  @Input()
  public set assignmentId(v: string | undefined) {
    if (Utils.isDifferent(this._assignmentId, v)) {
      this._assignmentId = v;
    }
  }

  public get input(): IAssessmentPlayerInput {
    return this._input;
  }
  @Input()
  public set input(v: IAssessmentPlayerInput) {
    if (Utils.isDifferent(this._input, v) && v != null) {
      this._input = v;
    }
  }

  @Output('peerAssessmentSelected') public peerAssessmentSelectedEvent: EventEmitter<AssessmentAnswer> = new EventEmitter<
    AssessmentAnswer
  >();

  public fetchUserInfoItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<PublicUserInfo[]>;
  public peerUserInfo: PublicUserInfo[] = [];

  private _courseId: string;
  private _classRunId: string;
  private _assignmentId: string;
  private _peerAssessmentVm: PeerAssessmentViewModel;
  private _input: IAssessmentPlayerInput;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private assessmentAnswerRepository: AssessmentAnswerRepository,
    private peerAssessmentPageComponentService: PeerAssessmentPageComponentService
  ) {
    super(moduleFacadeService);
    this.fetchUserInfoItemsFn = this._createFetchUserSelectItemFn(p => {
      return p
        .map(x => x.user)
        .filter(u => u.id !== this.input.userId && !this.peerAssessmentVm.peerAssessors.some(assessor => u.id === assessor.userId));
    });
  }

  public canRemoveAssessorFn: (item: AssessmentAnswer) => boolean = item => item.submittedDate == null;

  public validateAndAddPeerAssessor(): Observable<AssessmentAnswer> {
    return from(
      new Promise<AssessmentAnswer>((resolve, reject) => {
        this.validate().then(val => {
          if (val) {
            this.addPeerAssessor().then(_ => {
              resolve(_);
            }, reject);
          } else {
            reject('validation error');
          }
        }, reject);
      })
    );
  }

  public onPeerAssessorClicked(value: AssessmentAnswer): void {
    this.peerAssessmentSelectedEvent.emit(value);
  }

  public onAddButtonClicked(): void {
    if (this.peerAssessmentVm.isSelectedToAddPeerAssessor()) {
      this.validateAndAddPeerAssessor().subscribe();
      this.resetForm();
    }
  }

  public addPeerAssessor(): Promise<AssessmentAnswer> {
    const request: ICreateAssessmentAnswerRequest = {
      data: {
        assessmentId: this.input.assessmentId,
        participantAssignmentTrackId: this.input.participantAssignmentTrackId,
        userId: this.peerAssessmentVm.selectedToAddPeerAssessorId
      }
    };
    return this.assessmentAnswerRepository
      .createAssessmentAnswer(request)
      .toPromise()
      .then(data => {
        this.showNotification(this.translate('Peer Assessor added successfully'));
        return data;
      });
  }

  public canRemovePeerAssessment(assesmentAnswer: AssessmentAnswer): boolean {
    return assesmentAnswer.submittedDate ? false : true;
  }

  public onRemoveButtonClicked(assesmentAnswer: AssessmentAnswer): void {
    if (!assesmentAnswer.submittedDate) {
      this.assessmentAnswerRepository.deleteAssessmentAnswer(assesmentAnswer.id, assesmentAnswer.participantAssignmentTrackId).then(() => {
        const deletedPeerAssessmentUserName = this.peerAssessmentVm.displayPeerAssessorsDic[assesmentAnswer.userId].fullName;
        this.showNotification(`Peer assessment of ${deletedPeerAssessmentUserName} is successfully deleted`, NotificationType.Success);
      });
    } else {
      this.showNotification(this.translate('The completed assessment answer can not be deleted!'), NotificationType.Warning);
    }
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        selectedToAddPeerAssessor: {
          defaultValue: null,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Please select a assessor')
            }
          ]
        }
      }
    };
  }

  private _createFetchUserSelectItemFn(
    mapFn: (data: ParticipantAssignmentTrackViewModel[]) => PublicUserInfo[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<PublicUserInfo[]> {
    return (searchText, skipCount, maxResultCount) => {
      return this.peerAssessmentPageComponentService
        .loadParticipantAssignmentTracks(
          this.courseId,
          this.classRunId,
          this.assignmentId,
          searchText,
          null,
          null,
          skipCount,
          maxResultCount
        )
        .pipe(
          map(p => {
            return mapFn(p.data);
          })
        );
    };
  }
}
