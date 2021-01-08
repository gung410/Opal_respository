import {
  Assessment,
  AssessmentAnswer,
  AssessmentAnswerRepository,
  AssessmentRepository,
  ISaveAssessmentAnswerRequest,
  UserRepository
} from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, Input, QueryList, ViewChildren } from '@angular/core';
import { DialogAction, OpalDialogService } from '@opal20/common-components';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { AssessmentAnswerDetailViewModel } from './../../view-models/assessment-answer-detail-view.model';
import { AssessmentCriteriaComponent } from './assessment-criteria.component';
import { AssessmentPlayerIntegrationsService } from './../../services/assessment-player-integrations.service';

@Component({
  selector: 'main-assessment-player',
  templateUrl: './main-assessment-player.component.html'
})
export class MainAssessmentPlayerComponent extends BaseFormComponent {
  @ViewChildren(AssessmentCriteriaComponent) public criteria: QueryList<AssessmentCriteriaComponent>;
  @HostBinding('class.-preview-mobile') public get previewMobileHostBinding(): boolean {
    return this.isMobileMode;
  }

  @Input() public isMobileMode: boolean = false;
  @Input() public assessmentId: string;
  @Input() public forPreview: boolean = false;
  public assessmentAnswerVm: AssessmentAnswerDetailViewModel = new AssessmentAnswerDetailViewModel();
  public assessment: Assessment = new Assessment();
  private _loadDataSub: Subscription = new Subscription();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private assessmentAnswerRepository: AssessmentAnswerRepository,
    private assessmentRepository: AssessmentRepository,
    private userRepository: UserRepository,
    private assessmentPlayerIntegrationsService: AssessmentPlayerIntegrationsService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public get disableSubmitButton(): boolean {
    const criteriaDontHaveAnswer = this.assessmentAnswerVm.criteriaAnswers.filter(x => !x.hasScale());
    return criteriaDontHaveAnswer.length > 0;
  }

  public onSubmit(): void {
    this.validateAndSaveAnswerForAssessment(true).subscribe(_ => {
      this.assessmentPlayerIntegrationsService.notifyAssessmentSubmitted(_);
    });
  }

  public onSave(): void {
    from(this.saveAnswerForAssessment(false)).subscribe(_ => {
      this.assessmentPlayerIntegrationsService.notifyAssessmentSaved(_);
    });
  }

  public onBack(): void {
    from(this.saveAnswerForAssessment(false, false)).subscribe(_ => {
      this.assessmentPlayerIntegrationsService.notifyAssessmentSaved(_);
      this.assessmentPlayerIntegrationsService.notifyAssessmentBack();
    });
  }

  public validateAndSaveAnswerForAssessment(isSubmit: boolean): Observable<AssessmentAnswer> {
    return from(
      new Promise<AssessmentAnswer>((resolve, reject) => {
        this.validate().then(val => {
          if (val) {
            this.saveAnswerForAssessment(isSubmit).then(_ => {
              resolve(_);
            }, reject);
          } else {
            reject('validation error');
          }
        }, reject);
      })
    );
  }

  public saveAnswerForAssessment(isSubmit: boolean, showMessage: boolean = true): Promise<AssessmentAnswer> {
    const request: ISaveAssessmentAnswerRequest = {
      id: this.assessmentAnswerVm.id,
      criteriaAnswers: this.assessmentAnswerVm.criteriaAnswers.filter(x => x.hasScale()),
      isSubmit: isSubmit
    };
    return this.assessmentAnswerRepository
      .saveAssessmentAnswer(request)
      .pipe(
        switchMap(assessmentAnswer => {
          const userObs = assessmentAnswer.isAssessentForFacilitator()
            ? this.userRepository.loadPublicUserInfoList({ userIds: [assessmentAnswer.changedBy] })
            : of(null);

          return userObs.pipe(
            map(users => {
              return { assessmentAnswer, users };
            })
          );
        })
      )
      .toPromise()
      .then(data => {
        this.assessmentAnswerVm = new AssessmentAnswerDetailViewModel(
          data.assessmentAnswer,
          this.assessment,
          data.users == null ? null : data.users[0]
        );
        if (!AppGlobal.assignmentPlayerIntegrations.isMobile && showMessage) {
          this.showNotification(
            isSubmit ? this.translate('Assessment submitted successfully') : this.translate('Assessment saved successfully')
          );
        }
        return data.assessmentAnswer;
      });
  }

  public showBackConfirmationDialog(): void {
    if (!this.dataHasChanged()) {
      this.assessmentPlayerIntegrationsService.notifyAssessmentBack();
      return;
    }

    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'You have unsaved changes, would you like to save it now?',
        hideNoBtn: false,
        yesBtnText: 'Yes'
      })
      .subscribe(action => {
        if (action === DialogAction.Cancel) {
          this.assessmentPlayerIntegrationsService.notifyAssessmentBack();
        } else if (action === DialogAction.OK) {
          this.onBack();
        }
      });
  }

  public get viewMode(): boolean {
    return this.assessmentAnswerVm.submittedDate != null;
  }

  protected onInit(): void {
    this.loadData();
  }

  protected additionalCanSaveCheck(): Promise<boolean> {
    return Promise.all(
      this.criteria
        .toArray()
        .reverse()
        .map(p => p.validate())
    ).then(finalResult => {
      return !finalResult.includes(false);
    });
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = combineLatest(
      this.assessmentId != null ? of(this.assessmentId) : this.assessmentPlayerIntegrationsService.assessmentId$,
      this.assessmentPlayerIntegrationsService.assessmentAnswerId$,
      this.assessmentPlayerIntegrationsService.participantAssignmentTrackId$,
      this.assessmentPlayerIntegrationsService.userId$
    )
      .pipe(
        this.untilDestroy(),
        switchMap(([assessmentId, assessmentAnswerId, participantAssignmentTrackId, userId]) => {
          const assessmentAnswerObs =
            assessmentAnswerId == null && participantAssignmentTrackId == null && userId == null
              ? of(null)
              : this.assessmentAnswerRepository.loadAssessmentAnswerById(assessmentAnswerId, participantAssignmentTrackId, userId);
          const assessmentObs = assessmentId == null ? of(new Assessment()) : this.assessmentRepository.loadAssessmentById(assessmentId);
          return combineLatest(assessmentAnswerObs, assessmentObs);
        }),
        switchMap(([assessmentAnswer, assessment]) => {
          const userObs =
            assessmentAnswer && assessmentAnswer.isAssessentForFacilitator()
              ? this.userRepository.loadPublicUserInfoList({ userIds: [assessmentAnswer.changedBy] })
              : of(null);
          return userObs.pipe(
            map(users => {
              return { assessmentAnswer, assessment, users };
            })
          );
        })
      )
      .subscribe(data => {
        this.assessment = data.assessment;
        this.assessmentAnswerVm = new AssessmentAnswerDetailViewModel(
          data.assessmentAnswer,
          data.assessment,
          data.users == null ? null : data.users[0]
        );
      });
  }

  private dataHasChanged(): boolean {
    return this.assessmentAnswerVm && this.assessmentAnswerVm.dataHasChanged();
  }
}
