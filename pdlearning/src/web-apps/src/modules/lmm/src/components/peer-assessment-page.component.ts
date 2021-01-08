import {
  AssessmentAnswer,
  AssessmentAnswerRepository,
  PublicUserInfo,
  SearchAssessmentAnswerResult,
  UserRepository
} from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Observable, Subscription, of } from 'rxjs';

import { IAssessmentPlayerInput } from '@opal20/domain-components';
import { PeerAssessmentViewModel } from '../view-models/peer-assessment-view.model';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'peer-assessment-page',
  templateUrl: './peer-assessment-page.component.html'
})
export class PeerAssessmentPageComponent extends BaseFormComponent {
  public get input(): IAssessmentPlayerInput {
    return this._input;
  }
  @Input()
  public set input(v: IAssessmentPlayerInput) {
    if (Utils.isDifferent(this._input, v) && v != null) {
      this._input = v;
      this.loadData();
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

  public get currentSeletedAssessmentAnswer(): AssessmentAnswer | undefined {
    return this._currentSeletedAssessmentAnswer;
  }

  @Input()
  public set currentSeletedAssessmentAnswer(v: AssessmentAnswer | undefined) {
    if (Utils.isDifferent(this._currentSeletedAssessmentAnswer, v)) {
      this._currentSeletedAssessmentAnswer = v;
    }
  }

  public get assessmentInput(): IAssessmentPlayerInput {
    return {
      assessmentId: this.currentSeletedAssessmentAnswer ? this.currentSeletedAssessmentAnswer.assessmentId : '',
      participantAssignmentTrackId: this.currentSeletedAssessmentAnswer
        ? this.currentSeletedAssessmentAnswer.participantAssignmentTrackId
        : '',
      userId: this.currentSeletedAssessmentAnswer ? this.currentSeletedAssessmentAnswer.userId : ''
    };
  }

  public peerAssessmentVm: PeerAssessmentViewModel = new PeerAssessmentViewModel();

  private _loadDataSub: Subscription = new Subscription();
  private _input: IAssessmentPlayerInput;
  private _courseId: string;
  private _classRunId: string;
  private _assignmentId: string;
  private _currentSeletedAssessmentAnswer: AssessmentAnswer | null = null;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private assessmentAnswerRepository: AssessmentAnswerRepository,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
  }

  public onSelectedAssessmentAnswer(value: AssessmentAnswer): void {
    this.currentSeletedAssessmentAnswer = value;
  }
  public loadPeerAssessorInfo(peerAssessors: AssessmentAnswer[]): Observable<PublicUserInfo[]> {
    return this.userRepository.loadPublicUserInfoList({ userIds: peerAssessors.map(p => p.userId) }).pipe(this.untilDestroy());
  }

  protected onInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();

    const assessmentAnswerObs: Observable<SearchAssessmentAnswerResult | null> =
      this.input.participantAssignmentTrackId == null
        ? of(null)
        : this.assessmentAnswerRepository.loadSearchAssessmentAnswer(
            this.input.participantAssignmentTrackId,
            null,
            '',
            null,
            null,
            null,
            true
          );

    this._loadDataSub = assessmentAnswerObs
      .pipe(
        switchMap(searchAssessmentAnswerResult => {
          return PeerAssessmentViewModel.create(searchAssessmentAnswerResult.items, a => this.loadPeerAssessorInfo(a));
        })
      )
      .subscribe(assessmentAnswerVM => {
        this.peerAssessmentVm = assessmentAnswerVM;
      });
  }
}
