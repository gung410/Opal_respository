import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
  CxInformationDialogService,
  CxSurveyjsComponent,
  CxSurveyjsEventModel,
  CxSurveyJsModeEnum,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { environment } from 'app-environments/environment';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import { LearningNeedsCompletionRate } from 'app-models/mpj/idp.model';
import { IdpConfigParams } from 'app-models/pdplan.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { IdpService } from 'app-services/idp.service';
import { LearningNeedHelpers } from 'app-services/idp/learning-need/learning-needs-helpers';
import { PdPlannerService } from 'app-services/idp/pd-planner/pd-planner.service';
import { ResultHelper } from 'app-services/idp/result-helpers';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import {
  IDPMode,
  IdpStatusCodeEnum,
  IdpStatusEnum,
} from 'app/individual-development/idp.constant';
import { SubmittedLNAEventData } from 'app/individual-development/models/pd-evaluation.model';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { Constant } from 'app/shared/app.constant';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { cloneDeep, isEmpty, isEqual } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { PageModel, SurveyModel } from 'survey-angular';
import { isNullOrUndefined } from 'util';
import { LearningNeedsPreviewAnswerComponent } from '../learning-needs-preview-answer/learning-needs-preview-answer.component';
import { LearningNeedsPreviewComponent } from '../learning-needs-preview/learning-needs-preview.component';

const COMPLETED_STATUSES: IdpStatusCodeEnum[] = [
  IdpStatusCodeEnum.Completed,
  IdpStatusCodeEnum.PendingForApproval,
  IdpStatusCodeEnum.Approved,
  IdpStatusCodeEnum.Rejected,
];

const EDITABLE_STATUSES: IdpStatusCodeEnum[] = [
  IdpStatusCodeEnum.Rejected,
  IdpStatusCodeEnum.NotStarted,
  IdpStatusCodeEnum.Started,
];

const SUBMITTABLE_STATUSES: IdpStatusCodeEnum[] = [
  IdpStatusCodeEnum.NotStarted,
  IdpStatusCodeEnum.Started,
];

const LNA_PAGE_NAME = {
  EasLearningAreas: 'Key Learning Areas',
  OtherLearningAreas: 'Learning Areas',
  Competencies: 'Competencies',
};

const NUMBER_OF_PREVIOUS_LNA_RESULTS = 3;

@Component({
  selector: 'learning-needs-analysis-content',
  templateUrl: './learning-needs-analysis-content.component.html',
  styleUrls: ['./learning-needs-analysis-content.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class LearningNeedsAnalysisContentComponent
  extends BaseSmartComponent
  implements OnInit {
  @Input() user: Staff;
  @Input() learningNeeds: IdpDto;
  @Input() reviewMode: boolean = false;
  @Input() mode: IDPMode = IDPMode.Learner;
  _needsResults: IdpDto[];
  get needsResults(): IdpDto[] {
    return this._needsResults;
  }
  @Input() set needsResults(results: IdpDto[]) {
    if (!results) {
      this._needsResults = [];

      return;
    }
    const currentPeriod = new Date(
      this.learningNeeds.surveyInfo.startDate
    ).getFullYear();
    let previousResults = results.filter(
      (rs) => currentPeriod > new Date(rs.surveyInfo.startDate).getFullYear()
    );

    if (previousResults.length > NUMBER_OF_PREVIOUS_LNA_RESULTS) {
      previousResults = previousResults.filter(
        (result, index) => index < NUMBER_OF_PREVIOUS_LNA_RESULTS
      );
    }

    this._needsResults = previousResults;
  }

  @Output()
  submit: EventEmitter<SubmittedLNAEventData> = new EventEmitter<SubmittedLNAEventData>();
  @Output() needToReloadLNA: EventEmitter<void> = new EventEmitter<void>();
  @ViewChild(LearningNeedsPreviewComponent)
  learningNeedsPreviewComponent: LearningNeedsPreviewComponent;
  @ViewChild(LearningNeedsPreviewAnswerComponent)
  learningNeedsPreviewAnswerComponent: LearningNeedsPreviewAnswerComponent;

  allAnswers: any;
  learningNeedsConfig: any;
  surveyVariables: CxSurveyjsVariable[] = [];

  isLatestStep: boolean;
  /**
   * LNA was at preview step or the status of result is one of these
   * @see COMPLETED_STATUSES
   */
  isCompleted: boolean;
  isEnableSaveDraft: boolean;

  steps: string[] = [];
  currentStep: number = 0;
  currentCompletedStep: number = 0;

  surveySteps: number[] = [];
  /**
   * Distance from first step of LNA to the survey
   */
  firstSurveyStepDistance: number;
  reviewStep: number = 0;
  previewStep: number = 0;

  isEditable: boolean;
  isSubmittable: boolean;
  isCurrentYearAcknowledgeResult: boolean;
  canEditOnReviewMode: boolean;
  /**
   * When changing answer in Competencies step we need to set some default values for next step manually.
   * This map will store questions that were set manually to avoid re-checking save draft button.
   */
  private setDefaultValueMap: object = {};
  /**
   * When moving to preview step, we need to clear some answers of questions are invisible to show the chart correctly.
   * This map will store questions that were cleared manually to avoid re-checking save draft button.
   */
  private clearInvisibleValueMap: object = {};
  /**
   * When changing the answer by clicking the radio, it will emit an event but it later than the event of surveyjs.
   * So we have store these questions to avoid duplicated event.
   */
  private matrixSelectedRowsMap: object = {};
  private isCancelEditing: boolean;

  @ViewChild('survey') private surveyComp: CxSurveyjsComponent;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private idpService: IdpService,
    private pdPlannerService: PdPlannerService,
    private toastrService: ToastrService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private translateAdapterService: TranslateAdapterService,
    private informationDialogService: CxInformationDialogService,
    private ngbModal: NgbModal
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    this.cxGlobalLoaderService.showLoader();
    const params = new IdpConfigParams({
      resultId: this.learningNeeds.resultIdentity.id,
    });
    this.idpService.getLearningNeedConfig(params).subscribe((needsConfig) => {
      const surveyJSON = { ...needsConfig.configuration };
      this.initSurvey(surveyJSON);
      this.cxGlobalLoaderService.hideLoader();
      const answers = this.learningNeeds.answer
        ? { ...this.learningNeeds.answer }
        : {};
      this.allAnswers = answers;
      this.setSurveyJsObjectVariables(this.learningNeeds);
    });
  }

  onSubmit(surveyEvent: CxSurveyjsEventModel): void {
    if (!surveyEvent.options.allowComplete) {
      return;
    }
    surveyEvent.options.allowComplete = false;
    this.cxGlobalLoaderService.showLoader();

    const newNeedsResult = {
      ...this.learningNeeds,
    };
    newNeedsResult.answer = surveyEvent.survey.data;

    newNeedsResult.additionalProperties.completionRate =
      LearningNeedsCompletionRate.PrioritiesOfLearningAreas;
    this.idpService.saveNeedsResult(newNeedsResult).subscribe((response) => {
      this.changeLearningNeedsStatus(response, {
        assessmentStatusId: IdpStatusEnum.PendingForApproval,
        assessmentStatusCode: IdpStatusCodeEnum.PendingForApproval,
      });
    });
  }

  clickSubmit(): void {
    if (!this.user.approvalGroups || !this.user.approvalGroups.length) {
      const warningMessage: string = this.translateAdapterService.getValueImmediately(
        'MyPdJourney.Message.SubmitWithUnassignAO'
      );
      this.informationDialogService.warning({ message: warningMessage });

      return;
    }

    if (
      this.learningNeeds.dueDate &&
      DateTimeUtil.isInThePast(this.learningNeeds.dueDate)
    ) {
      const warningMessage: string = this.translateAdapterService.getValueImmediately(
        'MyPdJourney.Message.SubmitExpiredLNA'
      );
      this.informationDialogService.warning({
        message: warningMessage.replace(
          '{DueDate}',
          DateTimeUtil.toDateString(new Date(this.learningNeeds.dueDate))
        ),
      });

      return;
    }
    this.surveyComp.doComplete();
  }

  saveDraft(): void {
    this.cxGlobalLoaderService.showLoader();
    this.saveSurvey(
      this.surveyComp.surveyModel.data,
      this.currentStep,
      true,
      true
    )
      .then(() => {
        this.toastrService.success(
          'Your LNA progress has been saved successfully as a Draft version.'
        );
        this.isEnableSaveDraft = false;
      })
      .catch(() =>
        this.toastrService.error(
          'Unable to save Draft version for your LNA progress. Please try again!'
        )
      )
      .finally(() => this.cxGlobalLoaderService.hideLoader());
  }

  onSurveyDataChanging(event: CxSurveyjsEventModel): void {
    if (this.isCancelEditing) {
      this.isCancelEditing = false;

      return;
    }
    const oldValue: object = event.options.oldValue;
    const newValue: object = event.options.value;
    const question = event.options.question;
    if (!oldValue && !newValue) {
      return;
    }

    this.onChangingAnswer(question, oldValue, newValue);
  }

  async clickNext(currentStep: number): Promise<void> {
    this.cxGlobalLoaderService.showLoader();
    const isCurrentPageIsSurvey = this.surveySteps.includes(currentStep);
    const nextStep = currentStep + 1;

    if (isCurrentPageIsSurvey) {
      const isCurrentPageHasErrors = this.surveyComp.surveyModel
        .isCurrentPageHasErrors;
      if (isCurrentPageHasErrors) {
        return this.hideLoader();
      }

      if (this.surveyComp.surveyModel.isLastPage) {
        this.clearInvisibleQuestionValues();
      }

      if (this.isEditable) {
        await this.saveSurvey(
          this.surveyComp.surveyModel.data,
          nextStep,
          false
        );
      }
    }

    this.changeStep(nextStep);

    if (this.currentStep > this.currentCompletedStep) {
      this.currentCompletedStep = this.currentStep;
    }

    if (this.isEditable && this.isLatestStep) {
      if (this.learningNeedsPreviewComponent) {
        this.learningNeedsPreviewComponent.getLearningNeedsReportData();
      }
      if (this.learningNeedsPreviewAnswerComponent) {
        this.learningNeedsPreviewAnswerComponent.reloadSurvey();
      }
    }

    this.ScrollToTop();

    this.hideLoader();
  }

  onStepClicked(stepIndex: number): void {
    if (stepIndex > this.currentCompletedStep) {
      return;
    }

    this.cxGlobalLoaderService.showLoader();

    this.changeStep(stepIndex);

    this.hideLoader();
  }

  onAfterSurveyRendered(event: CxSurveyjsEventModel): void {
    event.survey.onAfterRenderQuestion.add(this.onAfterRenderQuestion);
    this.initSurveySteps();
    this.surveyComp.surveyModel.css.pageTitle = 'surveyjs-page-header';
    this.recheckStatus();
    const statusCode = this.learningNeeds.assessmentStatusInfo
      .assessmentStatusCode;

    if (COMPLETED_STATUSES.includes(statusCode as IdpStatusCodeEnum)) {
      this.currentCompletedStep = this.steps.length - 1;
    }

    this.autoNavigateToCurrentStep();

    if (this.learningNeedsPreviewComponent) {
      this.learningNeedsPreviewComponent.getLearningNeedsReportData();
    }
    if (this.learningNeedsPreviewAnswerComponent) {
      this.learningNeedsPreviewAnswerComponent.reloadSurvey();
    }
  }

  onEditPreviousStep(): {
    confirm: EventEmitter<any>;
    cancel: EventEmitter<any>;
    close: () => void;
  } {
    if (this.ngbModal.hasOpenModals()) {
      return;
    }
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      centered: true,
    });
    const componentInstance = modalRef.componentInstance as CxConfirmationDialogComponent;
    componentInstance.isDanger = true;
    componentInstance.cancelButtonText = this.translateAdapterService.getValueImmediately(
      'MyPdJourney.EditLNAConfirmation.Cancel'
    );
    componentInstance.confirmButtonText = this.translateAdapterService.getValueImmediately(
      'MyPdJourney.EditLNAConfirmation.Yes'
    );
    componentInstance.content = this.translateAdapterService.getValueImmediately(
      'MyPdJourney.EditLNAConfirmation.Content'
    );
    const closeModalRef = () => modalRef.close();

    return {
      confirm: componentInstance.confirm,
      cancel: componentInstance.cancel,
      close: closeModalRef,
    };
  }

  async onClickEditOnReviewMode(): Promise<void> {
    const result = await this.informationDialogService.question({
      message: this.translateAdapterService.getValueImmediately(
        'MyPdJourney.LearningNeeds.ConfirmEditingApprovedLNA'
      ),
      confirmButtonText: this.translateAdapterService.getValueImmediately(
        'Common.Confirmation.Yes'
      ),
      cancelButtonText: this.translateAdapterService.getValueImmediately(
        'Common.Confirmation.No'
      ),
    });
    if (result && result.value === true) {
      await this.retakeLNAAsync();
      this.needToReloadLNA.emit();
    }
  }

  onSubmittedLearningNeeds(eventData: SubmittedLNAEventData): void {
    this.submit.emit(eventData);
  }

  async retakeLNAAsync(): Promise<void> {
    const updatingLNA = { ...this.learningNeeds };
    const latestLNA = await this.getLatestLNA(updatingLNA);
    if (!latestLNA) {
      // Show error message if the latest LNA could not be found but it rarely happens in the real case.
      this.toastrService.error(
        `Could not find the latest Learning Need Analysis!`
      );

      return;
    }

    if (updatingLNA.resultIdentity.id !== latestLNA.resultIdentity.id) {
      // Conflict happens when the user tries to retake LNA but it is not the latest state of the LNA.
      this.toastrService.error(
        this.translateAdapterService.getValueImmediately(
          'MyPdJourney.LearningNeeds.ConflictEditingLNA'
        )
      );

      return;
    }

    const secondStep = 1;
    const draftLNA = await this.createDraftLNA(updatingLNA, secondStep);
    if (!draftLNA) {
      return;
    }

    this.learningNeeds = draftLNA;
    this.recheckStatus();
    this.surveyComp.changeMode(CxSurveyJsModeEnum.Edit);
    this.currentCompletedStep = secondStep;
    this.changeStep(secondStep);
  }

  private changeLearningNeedsStatus(
    result: IdpDto,
    newStatusInfo: AssessmentStatusInfo
  ) {
    this.idpService
      .changeStatusLearningNeeds({
        resultIdentities: [this.learningNeeds.resultIdentity],
        targetStatusType: {
          assessmentStatusCode: newStatusInfo.assessmentStatusCode,
          assessmentStatusId: newStatusInfo.assessmentStatusId,
        },
      })
      .subscribe(
        (resultResponse) => {
          this.cxGlobalLoaderService.hideLoader();
          if (!resultResponse) {
            return;
          }
          const changeStatusResult = resultResponse.find(
            (result) =>
              this.learningNeeds.resultIdentity.extId === result.identity.extId
          );
          this.learningNeeds = {
            ...result,
            assessmentStatusInfo: changeStatusResult.targetStatusType,
          };
          this.recheckStatus();
          this.isEnableSaveDraft = false;
          this.handleAfterSubmitLNA();
        },
        () => {
          this.cxGlobalLoaderService.hideLoader();
        }
      );
  }

  private async createDraftLNA(
    updatingLNA: IdpDto,
    activeStep: number
  ): Promise<IdpDto> {
    updatingLNA.assessmentStatusInfo = {
      assessmentStatusCode: IdpStatusCodeEnum.Started,
    };

    if (!updatingLNA.additionalProperties) {
      updatingLNA.additionalProperties = {};
    }

    updatingLNA.additionalProperties.currentStep = activeStep;

    updatingLNA.additionalProperties.completionRate = LearningNeedHelpers.ProcessCompletionRateByStep(
      activeStep
    );

    const response = await this.idpService.saveNeedsResultAsync(updatingLNA);
    if (response.error) {
      console.error(response.error);

      return;
    }

    return response.data;
  }

  private async getLatestLNA(currentLearningNeed: IdpDto): Promise<IdpDto> {
    const response = await this.idpService.getNeedsResultAsync({
      resultExtIds: [currentLearningNeed.resultIdentity.extId],
    });

    if (response.error) {
      return;
    }

    const relatedLearningNeeds: IdpDto[] = response.data;

    if (relatedLearningNeeds) {
      return relatedLearningNeeds.sort((result1, result2) => {
        const date1 = +new Date(result1.created);
        const date2 = +new Date(result2.created);

        return date2 - date1;
      })[0];
    }

    return;
  }

  private autoNavigateToCurrentStep(): void {
    const storedStep: number =
      this.learningNeeds.additionalProperties &&
      this.learningNeeds.additionalProperties.currentStep;
    const targetStep = this.reviewMode ? 0 : storedStep;
    if (!isNullOrUndefined(targetStep)) {
      this.changeStep(targetStep);

      if (!this.currentCompletedStep) {
        this.currentCompletedStep = targetStep;
      }
    }
  }

  private onChangingAnswer(
    question: any,
    oldValue: object,
    newValue: object
  ): void {
    this.setMatrixSelectedRow(question, oldValue, newValue);
    let isAutoChanged = false;

    if (this.setDefaultValueMap[question.name]) {
      this.setDefaultValueMap[question.name] = false;
      isAutoChanged = true;
    }
    if (this.clearInvisibleValueMap[question.name]) {
      this.clearInvisibleValueMap[question.name] = false;
      isAutoChanged = true;
    }
    if (isAutoChanged) {
      return;
    }

    this.evaluateChangingAnswer(question, oldValue, newValue);
  }

  private evaluateChangingAnswer(
    question: any,
    oldValue: object,
    newValue: object
  ): void {
    if (this.currentStep < this.currentCompletedStep) {
      // is editing previous step
      const editPreviousStepDialog = this.onEditPreviousStep();
      if (!editPreviousStepDialog) {
        return;
      }
      editPreviousStepDialog.confirm.subscribe(() => {
        this.checkSaveDraftButton(oldValue, newValue);
        this.currentCompletedStep = this.currentStep;
        editPreviousStepDialog.close();
      });
      editPreviousStepDialog.cancel.subscribe(() => {
        this.isCancelEditing = true;
        question.value = oldValue;
        editPreviousStepDialog.close();
      });

      return;
    }
    this.checkSaveDraftButton(oldValue, newValue);
  }

  private checkSaveDraftButton(oldValue: object, newValue: object): void {
    if (this.isEnableSaveDraft) {
      return;
    }
    if (this.currentStep === this.previewStep) {
      return;
    }
    this.isEnableSaveDraft = !isEqual(oldValue, newValue);
  }

  private suggestMoveToPdplan(): void {
    if (this.checkIsMobile()) {
      return;
    }
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      centered: true,
    });
    const componentInstance = modalRef.componentInstance as CxConfirmationDialogComponent;
    componentInstance.header = this.translateAdapterService.getValueImmediately(
      'MyPdJourney.SuggestNavigatingToPdplan.Header'
    );
    componentInstance.content = this.translateAdapterService.getValueImmediately(
      'MyPdJourney.SuggestNavigatingToPdplan.Content'
    );
    componentInstance.confirmButtonText = this.translateAdapterService.getValueImmediately(
      'Common.Confirmation.Yes'
    );
    componentInstance.cancelButtonText = this.translateAdapterService.getValueImmediately(
      'Common.Confirmation.No'
    );
    const submitEventData: SubmittedLNAEventData = {
      result: this.learningNeeds,
    };

    componentInstance.confirm.subscribe(() => {
      submitEventData.navigateToPDPlan = true;
      this.submit.emit(submitEventData);
      modalRef.close();
    });
    componentInstance.cancel.subscribe(() => {
      submitEventData.navigateToPDPlan = false;
      this.submit.emit(submitEventData);
      modalRef.close();
    });
  }

  private changeStep(stepIndex: number): void {
    this.currentStep = stepIndex;
    this.isLatestStep = this.currentStep + 1 === this.steps.length;

    const surveyPageIndex = this.currentStep - this.firstSurveyStepDistance;
    const currentSurveyPage = this.surveyComp.surveyModel.getPage(
      surveyPageIndex
    );
    if (
      currentSurveyPage &&
      (currentSurveyPage.name === LNA_PAGE_NAME.EasLearningAreas ||
        currentSurveyPage.name === LNA_PAGE_NAME.OtherLearningAreas) &&
      this.isEditable
    ) {
      this.updateDefaultValueForLearningAreas(currentSurveyPage);
    }

    const isCurrentPageIsSurvey = this.surveySteps.includes(this.currentStep);
    if (isCurrentPageIsSurvey) {
      this.changeSurveyPage();
    }
  }

  private initSurveySteps(): void {
    const reviewStep: string[] = ['Getting started'];
    const surveyStepTitles = this.surveyComp.getTitleList();
    const previewStep: string[] = ['Preview and submit LNA'];

    this.steps = [...reviewStep, ...surveyStepTitles, ...previewStep];

    this.reviewStep = 0; // first step
    this.previewStep = reviewStep.length + surveyStepTitles.length;

    const surveySteps = [];
    this.firstSurveyStepDistance = reviewStep.length;
    surveyStepTitles.forEach((step, index) => {
      surveySteps.push(index + this.firstSurveyStepDistance);
    });
    this.surveySteps = surveySteps;
  }

  private changeSurveyPage(): void {
    this.surveyComp.surveyModel.currentPageNo =
      this.currentStep - this.firstSurveyStepDistance;
    setTimeout(() => {
      this.surveyComp.surveyModel.focusFirstQuestion();
    });
  }

  /**
   * Triggers after initiating, saving or submitting the result.
   */
  private recheckStatus(): void {
    const statusCode = ResultHelper.getStatusCode(this.learningNeeds);
    this.isCurrentYearAcknowledgeResult = this.checkIsCurrentYearResultAcknowledged(
      this.learningNeeds
    );
    this.isSubmittable = SUBMITTABLE_STATUSES.includes(
      statusCode as IdpStatusCodeEnum
    );
    this.isEditable =
      EDITABLE_STATUSES.includes(statusCode as IdpStatusCodeEnum) &&
      !this.reviewMode;
    if (
      this.reviewMode &&
      this.isCurrentYearAcknowledgeResult &&
      this.mode === IDPMode.Learner
    ) {
      this.idpService
        .getLearningNeedsAnalysisStatusChangeHistory(
          this.learningNeeds.resultIdentity.extId
        )
        .subscribe((statusHistories) => {
          const approvedHistories = statusHistories.filter(
            (s) =>
              s.targetStatusType &&
              s.targetStatusType.assessmentStatusCode ===
                IdpStatusCodeEnum.Approved
          );
          this.canEditOnReviewMode =
            approvedHistories.length <=
            environment.lnaResult.allowToEditLNAApprovedXTimes;
        });
    }
    if (!this.isEditable) {
      this.surveyComp.changeMode(CxSurveyJsModeEnum.Display);
    }
  }

  private setSurveyJsObjectVariables(result: any): void {
    // Pass the user which the object belonging to so that it would be set into the default variables.
    result.currentObjectUser = this.user;
    this.cxSurveyjsExtendedService.setCurrentObjectVariables(result);
  }

  private saveSurvey(
    surveyData: any,
    newStep: number,
    catchError: boolean = true,
    isSaveDraft: boolean = false
  ): Promise<any> {
    const isUpdatingAnswer = !isEqual(this.learningNeeds.answer, surveyData);
    const oldCurrentStep =
      this.learningNeeds.additionalProperties &&
      this.learningNeeds.additionalProperties.currentStep;
    const isUpdatingStep = oldCurrentStep !== newStep;

    let updatedCompletionRateNeedResult = this.updateCompletionRate(
      isSaveDraft ? newStep + 1 : newStep
    );
    let newNeedsResult: IdpDto = { ...updatedCompletionRateNeedResult };
    if (isUpdatingAnswer || isUpdatingStep) {
      newNeedsResult = {
        ...newNeedsResult,
        ...this.generateNewNeedResult(surveyData, newStep),
      };
    }

    return this.idpService
      .saveNeedsResult(newNeedsResult, catchError)
      .toPromise()
      .then((response: IdpDto) => {
        this.learningNeeds = response;
        this.recheckStatus();
      });
  }

  private generateNewNeedResult(surveyData: any, newStep: number): IdpDto {
    const newNeedsResult = { ...this.learningNeeds, answer: surveyData };
    if (
      newNeedsResult.assessmentStatusInfo.assessmentStatusCode !==
      IdpStatusCodeEnum.Started
    ) {
      const newStatusInfo = new AssessmentStatusInfo();
      newStatusInfo.assessmentStatusId = IdpStatusEnum.Started;
      newStatusInfo.assessmentStatusCode = IdpStatusCodeEnum.Started;
      newNeedsResult.assessmentStatusInfo = newStatusInfo;
    }

    if (!newNeedsResult.additionalProperties) {
      newNeedsResult.additionalProperties = {};
    }

    if (!isNullOrUndefined(newStep)) {
      newNeedsResult.additionalProperties.currentStep = newStep;
    }
    return newNeedsResult;
  }

  private initSurvey(surveyJSON: any): void {
    surveyJSON.showNavigationButtons = 'none';
    this.surveyVariables.push(
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.formDisplayMode,
        value: 'edit',
      })
    );
    this.learningNeedsConfig = surveyJSON;
  }

  private updateCompletionRate(step: number): IdpDto {
    const newNeedsResult = { ...this.learningNeeds };
    if (!this.surveyComp.surveyModel.isCurrentPageHasErrors) {
      newNeedsResult.additionalProperties.completionRate = LearningNeedHelpers.ProcessCompletionRateByStep(
        step
      );
    }
    return newNeedsResult;
  }

  private updateDefaultValueForLearningAreas(surveyPage: PageModel): void {
    if (!surveyPage) {
      return;
    }
    surveyPage.questions
      .filter((question) => question.visible)
      .forEach((question) => {
        this.setDefaultValueMap[question.name] = true;
        if (isEmpty(question.value)) {
          question.value = question.defaultValue;
        } else {
          Object.keys(question.defaultValue).forEach((key) => {
            if (isEmpty(question.value[key])) {
              question.value[key] = question.defaultValue[key];
            }
          });
          question.value = { ...question.value };
        }
        // Clear the value that doesn't exists in the radio group.
        question.clearIncorrectValues();
      });
  }

  private onAfterRenderQuestion = (survey: SurveyModel, options: any): void => {
    if (options.question.name !== LNA_PAGE_NAME.Competencies) {
      return;
    }
    this.matrixSelectedRowsMap[options.question.name] = {};
    const questionName: string = options.question.name;
    const question = survey.getQuestionByName(questionName);
    // Have to use Array forEach call for IE11
    Array.prototype.forEach.call(
      options.htmlElement.querySelectorAll('input'),
      (input) => {
        input.onclick = this.onClickMatrixRadioButton.bind(null, question);
      }
    );
  };

  private onClickMatrixRadioButton = (question: any, event: any) => {
    const valueName = event.srcElement.name.split('_')[2];
    if (this.matrixSelectedRowsMap[question.name][valueName]) {
      this.matrixSelectedRowsMap[question.name][valueName] = false;

      return;
    }
    if (!question.value) {
      return;
    }
    if (!!question.value[valueName]) {
      const tempValue: object = cloneDeep(question.value);
      delete tempValue[valueName];
      question.value = tempValue;
    }
  };

  private setMatrixSelectedRow(
    question: any,
    oldValue: object,
    newValue: object
  ): void {
    if (!newValue || !this.matrixSelectedRowsMap[question.name]) {
      return;
    }
    const newValueRows = Object.keys(newValue);
    const newRowValue = oldValue
      ? newValueRows.find((k) => newValue[k] !== oldValue[k])
      : newValueRows[0];
    if (!newRowValue) {
      return;
    }
    this.matrixSelectedRowsMap[question.name][newRowValue] = true;
  }

  private clearInvisibleQuestionValues(): void {
    const questions = this.surveyComp.surveyModel.getAllQuestions();
    questions.forEach((question) => {
      if (question.isVisible) {
        return;
      }
      this.clearInvisibleValueMap[question.name] = true;
      question.clearValueIfInvisible();
    });
  }

  private checkIsCurrentYearResultAcknowledged(result: IdpDto): boolean {
    const isCompleted = ResultHelper.checkIsCompletedResult(result);
    const isCurrentYearResult = ResultHelper.checkIsCurrentYearResult(result);

    return isCompleted && isCurrentYearResult;
  }

  private checkIsMobile(): boolean {
    const minWebWidth: number = 768; // px

    return document.body.offsetWidth < minWebWidth;
  }

  /**
   * Scroll to the top of the container.
   */
  private ScrollToTop(): void {
    const inputQuery = '.needs-analysis-title';
    const titleElement: HTMLElement = document.querySelector(inputQuery);
    if (!titleElement) {
      return;
    }
    titleElement.scrollIntoView();
  }

  private hideLoader(): void {
    setTimeout(() => {
      this.cxGlobalLoaderService.hideLoader();
    }, Constant.DELAY_RENDER_DOM);
  }

  private async handleAfterSubmitLNA(): Promise<void> {
    const surveyLink = await this.pdPlannerService.getTodoLNASurvey();
    if (surveyLink) {
      const ngbModal = this.pdPlannerService.showLNASurveyDialog(surveyLink);
      ngbModal.clickStart.subscribe(() => this.suggestMoveToPdplan());
    } else {
      this.suggestMoveToPdplan();
    }
  }
}
