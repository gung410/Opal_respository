import {
  Assessment,
  AssessmentAnswer,
  AssessmentAnswerRepository,
  AssessmentRepository,
  ISaveAssessmentAnswerRequest,
  UserRepository
} from '@opal20/domain-api';
import { AssessmentAnswerDetailViewModel, AssessmentCriteriaComponent, IAssessmentPlayerInput } from '@opal20/domain-components';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, HostBinding, Input, QueryList, ViewChildren } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Component({
  selector: 'assessment-answer-editor',
  templateUrl: './assessment-answer-editor.component.html'
})
export class AssessmentAnswerEditorComponent extends BaseFormComponent {
  @ViewChildren(AssessmentCriteriaComponent) public criteria: QueryList<AssessmentCriteriaComponent>;
  @HostBinding('class.-preview-mobile') public get previewMobileHostBinding(): boolean {
    return this.isMobileMode;
  }

  @Input() public isMobileMode: boolean = false;

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
  public assessmentAnswerVm: AssessmentAnswerDetailViewModel = new AssessmentAnswerDetailViewModel();
  private _loadDataSub: Subscription = new Subscription();
  private _assessment: Assessment = new Assessment();
  private _input: IAssessmentPlayerInput;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private assessmentAnswerRepository: AssessmentAnswerRepository,
    private assessmentRepository: AssessmentRepository,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
  }

  public get disableSubmitButton(): boolean {
    const criteriaDontHaveAnswer = this.assessmentAnswerVm.criteriaAnswers.filter(x => !x.hasScale());
    return criteriaDontHaveAnswer.length > 0;
  }

  public onSubmit(): void {
    this.validateAndSaveAnswerForAssessment(true).subscribe(_ => {
      this.showNotification(this.translate('Assessment submitted successfully'));
    });
  }

  public onSave(): void {
    from(this.saveAnswerForAssessment(false)).subscribe(_ => {
      this.showNotification(this.translate('Assessment saved successfully'));
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

  public saveAnswerForAssessment(isSubmit: boolean): Promise<AssessmentAnswer> {
    return new Promise((resolve, reject) => {
      const request: ISaveAssessmentAnswerRequest = {
        id: this.assessmentAnswerVm.id,
        criteriaAnswers: this.assessmentAnswerVm.criteriaAnswers.filter(x => x.hasScale()),
        isSubmit: isSubmit
      };

      this.assessmentAnswerRepository
        .saveAssessmentAnswer(request)
        .pipe(
          switchMap(assessmentAnswer => {
            const userObs = assessmentAnswer.isAssessentForFacilitator()
              ? this.userRepository.loadPublicUserInfoList({ userIds: [assessmentAnswer.changedBy] })
              : of(null);

            return userObs.pipe(
              map(users => {
                this.assessmentAnswerVm = new AssessmentAnswerDetailViewModel(
                  assessmentAnswer,
                  this._assessment,
                  users == null ? null : users[0]
                );
                return assessmentAnswer;
              })
            );
          }),
          this.untilDestroy()
        )
        .subscribe(_ => {
          resolve(_);
        }, reject);
    });
  }

  public get isAssessentForFacilitator(): boolean {
    return this.assessmentAnswerVm.assessmentAnswerData.isAssessentForFacilitator();
  }

  public get showWarningMessage(): boolean {
    return !this.isAssessentForFacilitator && this.assessmentAnswerVm.submittedDate == null;
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
    const assessmentAnswerObs: Observable<AssessmentAnswer> =
      this.input.assessmentAnswerId == null && this.input.participantAssignmentTrackId == null && this.input.userId == null
        ? of(null)
        : this.assessmentAnswerRepository.loadAssessmentAnswerById(
            this.input.assessmentAnswerId,
            this.input.participantAssignmentTrackId,
            this.input.userId
          );
    const assessmentObs =
      this.input.assessmentId == null ? of(new Assessment()) : this.assessmentRepository.loadAssessmentById(this.input.assessmentId);
    this._loadDataSub = combineLatest(assessmentAnswerObs, assessmentObs)
      .pipe(
        this.untilDestroy(),
        switchMap(([assessmentAnswer, assessment]) => {
          const userObs =
            assessmentAnswer.userId === AssessmentAnswer.assessmentForFacilitator
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
        this._assessment = data.assessment;
        this.assessmentAnswerVm = new AssessmentAnswerDetailViewModel(
          data.assessmentAnswer,
          data.assessment,
          data.users == null ? null : data.users[0]
        );
      });
  }
}
