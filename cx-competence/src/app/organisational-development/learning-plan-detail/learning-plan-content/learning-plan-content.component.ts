import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import {
  CxConfirmationDialogComponent,
  CxFormModal,
  CxGlobalLoaderService,
  CxSurveyjsComponent,
  CxSurveyjsEventModel,
  CxSurveyjsService,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { ArchetypeEnum } from 'app-enums/archetypeEnum';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import { User } from 'app-models/auth.model';
import {
  IKeyLearningProgrammePermission,
  KeyLearningProgrammePermission,
} from 'app-models/common/permission/key-learning-programme-permission';
import {
  ILearningDirectionPermission,
  LearningDirectionPermission,
} from 'app-models/common/permission/learning-direction-permission';
import {
  ILearningPlanPermission,
  LearningPlanPermission,
} from 'app-models/common/permission/learning-plan-permission';
import {
  SurveySubmitEventData,
  SurveySubmitEventData as SurveySubmittingEventData,
} from 'app-models/common/surveyjs.model';
import { DeactivateAssessmentParams } from 'app-models/deactivateAssessment.model';
import {
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
} from 'app-models/mpj/pdo-action-item.model';
import {
  KLPPlannedAreaModel,
  LearningAreaFormSelectorResultModel,
  MetadataObject,
} from 'app-models/opj/klp-planned-areas.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { ScheduledTask } from 'app-models/scheduled-task.model';
import { CommentService } from 'app-services/comment.service';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { PdPlannerService } from 'app-services/idp/pd-planner/pd-planner.service';
import { ResultHelper } from 'app-services/idp/result-helpers';
import { LocalScheduleService } from 'app-services/local-schedule.service';
import { KeyLearningProgramHelper } from 'app-services/odp/learning-plan-services/key-learning-program.helper';
import { KeyLearningProgramService } from 'app-services/odp/learning-plan-services/key-learning-program.service';
import { BrowserIdleHandler } from 'app-utilities/browser-idle.handler';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { StartingHierarchyDepartment } from 'app/cx-people-picker/cx-people-picker-dialog/starting-hierarchy-department.enum';
import { CxPeoplePickerService } from 'app/cx-people-picker/cx-people-picker.service';
import {
  CommentChangeData,
  CommentData,
} from 'app/individual-development/cx-comment/comment.model';
import { CxCommentComponent } from 'app/individual-development/cx-comment/cx-comment.component';
import {
  EvaluationTypeToIdpStatusCode,
  PDEvaluationType,
} from 'app/individual-development/idp.constant';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
import { PDEvaluationModel } from 'app/individual-development/models/pd-evaluation.model';
import { PdEvaluationDialogComponent } from 'app/individual-development/shared/pd-evalution-dialog/pd-evaluation-dialog.component';
import { IdleConfig } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { SurveyjsMode } from 'app/shared/constants/surveyjs-mode.constant';
import { cloneDeep, isEmpty } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { DuplicateLearningDirectionDialogComponent } from '../duplicate-learning-direction-dialog/duplicate-learning-direction-dialog.component';
import {
  OdpActivity,
  OdpActivityName,
  OdpStatusCode,
  OdpStatusEnum,
} from '../odp.constant';

@Component({
  selector: 'learning-plan-content',
  templateUrl: './learning-plan-content.component.html',
  styleUrls: ['./learning-plan-content.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LearningPlanContentComponent
  extends BaseScreenComponent
  implements
    OnInit,
    OnChanges,
    ILearningPlanPermission,
    ILearningDirectionPermission,
    IKeyLearningProgrammePermission {
  @Input() formJSON: any;
  @Input() pdplanDto: PDPlanDto;
  @Input() parentDTO: PDPlanDto;
  @Input() isActive: boolean = true;
  @Input() successMessage: any = {};
  @Output() save: EventEmitter<any> = new EventEmitter();
  @Output() submit: EventEmitter<any> = new EventEmitter();
  @Output() approve: EventEmitter<any> = new EventEmitter();
  @Output() reject: EventEmitter<any> = new EventEmitter();
  @Output()
  delete: EventEmitter<DeactivateAssessmentParams> = new EventEmitter();
  @Output() cancelCreateNode: EventEmitter<PDPlanDto> = new EventEmitter();
  @Output() updateFormAction: EventEmitter<void> = new EventEmitter();

  title: string;
  currentUserRoles: any[] = [];
  formVariables: CxSurveyjsVariable[];
  comments: CommentData[];
  currentPlannedPDO: PDOpportunityAnswerDTO; //PDOpportunityAnswerDTO

  // flags
  isOnMassNominationMode: boolean = false;
  allowManagePDO: boolean = false;
  isAutoSave: boolean = false;
  isPauseAutoSave: boolean = false;
  showCommentSection: boolean;
  autoSaveMessage: string = 'Autosave';
  clonedPdPlanDto?: PDPlanDto = null;

  // Catalogue variables
  catalogTagIds: string[];
  catalogPersonnelGroupsIdsForExternalPDOUsage: string[];
  catalogSelectedCourse: string[];
  showCatalogue: boolean = false;

  // permission
  learningPlanPermission: LearningPlanPermission;
  learningDirectionPermission: LearningDirectionPermission;
  keyLearningProgrammePermission: KeyLearningProgrammePermission;

  private _isdirty: boolean = false;
  get isDirty(): boolean {
    return this._isdirty;
  }
  set isDirty(value: boolean) {
    this._isdirty = value;
    if (this._isdirty) {
      this.unregisterPDAutoSaveScheduledTask(
        IdleConfig.AUTOSAVE_PD_CONTENT_TASK
      );
      this.registerPDAutoSaveTask(IdleConfig.AUTOSAVE_PD_CONTENT_TASK);
    } else {
      this.unregisterPDAutoSaveScheduledTask(
        IdleConfig.AUTOSAVE_PD_CONTENT_TASK
      );
    }
  }

  private learningPlanCyclePeriod: string = 'learningPlanCyclePeriod';
  private backupAnswer: any;
  private evaluationDialogHeader: any = {
    [PDEvaluationType.Approve]: 'Common.Action.Approve',
    [PDEvaluationType.Reject]: 'Common.Action.Reject',
  };

  @ViewChild('commentComponent') private commentComponent: CxCommentComponent;
  @ViewChild('surveyjsContent') private cxSurveyComponent: CxSurveyjsComponent;

  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService,
    private ngbModal: NgbModal,
    private translateService: TranslateService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private cxSurveyjsService: CxSurveyjsService,
    private keyLearningProgramHelper: KeyLearningProgramHelper,
    private commentService: CommentService,
    private pdPlannerService: PdPlannerService,
    private klpSerivce: KeyLearningProgramService,
    private toastrService: ToastrService,
    private globalLoader: CxGlobalLoaderService,
    private peoplePickerService: CxPeoplePickerService,
    public browserIdleHandler: BrowserIdleHandler,
    public localScheduleService: LocalScheduleService,
    private router: Router
  ) {
    super(changeDetectorRef, authService);
    this.formVariables = [];
  }

  initLearningPlanPermissionn(loginUser: User): void {
    this.learningPlanPermission = new LearningPlanPermission(loginUser);
  }

  initLearningDirectionPermission(loginUser: User): void {
    this.learningDirectionPermission = new LearningDirectionPermission(
      loginUser
    );
  }

  initKeyLearningProgrammePermission(loginUser: User): void {
    this.keyLearningProgrammePermission = new KeyLearningProgrammePermission(
      loginUser
    );
  }

  ngOnInit(): void {
    this.currentUserRoles =
      this.currentUser.systemRoles &&
      this.currentUser.systemRoles.map((role) => role.identity.extId)
        ? this.currentUser.systemRoles.map((role) => role.identity.extId)
        : [];
    this.initLearningPlanPermissionn(this.currentUser);
    this.initLearningDirectionPermission(this.currentUser);
    this.initKeyLearningProgrammePermission(this.currentUser);
    this.initData();
    this.initPdPlanDto();
    this.changeDetectorRef.detectChanges();
  }

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    if (this.pdplanDto.pdPlanActivity !== OdpActivity.Programme) {
      this.turnOffMassNominationMode();
    }
    if (changes && (changes.formJSON || changes.pdplanDto)) {
      this.initData();
      this.initPdPlanDto();
      const currentSurveyObjId = this.cxSurveyjsExtendedService.getVariable(
        SurveyVariableEnum.currentObject_id
      );
      this.cxSurveyjsExtendedService.setCurrentObjectVariables(this.pdplanDto);
      if (this.isEditMode(currentSurveyObjId)) {
        this.changeMode(SurveyjsMode.edit);
        this.isAutoSave = false;
      } else if (this.isViewMode(currentSurveyObjId)) {
        this.changeMode(SurveyjsMode.display);
      }
      this.updateCommentData();

      if (this.isKeyLearningProgram) {
        this.checkManagePDORight();
      }
    } else {
      this.isDirty = false;
    }

    if (this.pdplanDto.pdPlanActivity === OdpActivity.Plan) {
      const isAlreadyHasVariable = this.formVariables.some(
        (variable) => variable.name === this.learningPlanCyclePeriod
      );
      if (!isAlreadyHasVariable && this.pdplanDto.answer.CyclePeriod) {
        const learningPlanCyclePeriod = new Date(
          this.pdplanDto.answer.CyclePeriod
        );
        // tslint:disable-next-line:no-magic-numbers
        learningPlanCyclePeriod.setUTCHours(23, 59, 59);
        this.formVariables.push(
          new CxSurveyjsVariable({
            name: this.learningPlanCyclePeriod,
            value: learningPlanCyclePeriod.toISOString(),
          })
        );
        this.formVariables = cloneDeep(this.formVariables);
      }
    }

    this.changeDetectorRef.detectChanges();
  }

  isEditMode(surveyObjId: string): boolean {
    return (
      this.isNewPlan() ||
      (surveyObjId === undefined &&
        this.pdplanDto.resultIdentity.id === undefined)
    );
  }

  isViewMode(surveyObjId: string): boolean {
    return (
      !this.isAutoSave &&
      surveyObjId !== undefined &&
      this.pdplanDto.resultIdentity.id.toString() !== surveyObjId.toString()
    );
  }

  afterQuestionsRendered(event: CxSurveyjsEventModel): void {
    const question = event.options.question;
    const htmlElement = event.options.htmlElement;
    const questionType = question.getType();
    if (questionType === 'cxpeoplepicker') {
      const klpDepartmentId =
        this.pdplanDto &&
        this.pdplanDto.objectiveInfo &&
        this.pdplanDto.objectiveInfo.identity
          ? this.pdplanDto.objectiveInfo.identity.id
          : 0;
      this.peoplePickerService.initQuestionPeoplePicker({
        currentUser: this.currentUser,
        question,
        htmlElement,
        startingHierarchyDepartment:
          StartingHierarchyDepartment.SpecifiedDepartment,
        specificDepartmentId: klpDepartmentId,
        windowClass: 'add-individual-learners-klp-modal',
      });
    }
  }

  onSubmitting(eventData: SurveySubmittingEventData): void {
    if (!eventData.options.allowComplete) {
      return;
    }
    if (!this.isDirty) {
      this.toastrService.success(
        this.successMessage[this.pdplanDto.pdPlanActivity]
      );

      return this.changeMode(SurveyjsMode.display);
    }
    eventData.options.allowComplete = false;
    const surveyResult: any = eventData.survey.data;
    this.updateCustomKLPInfo(surveyResult);
    this.onSave(surveyResult);
  }

  switchToEditMode(): void {
    this.backupAnswer = ObjectUtilities.clone(this.pdplanDto.answer);
    this.changeMode(SurveyjsMode.edit);
  }

  onCancel(): void {
    if (this.isDirty) {
      const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
        size: 'lg',
        centered: true,
      });
      const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
      modalComponent.cancelButtonText = this.translateService.instant(
        'Odp.ConfirmationDialog.No'
      );
      modalComponent.confirmButtonText = this.translateService.instant(
        'Odp.ConfirmationDialog.Confirm'
      );
      modalComponent.header = this.translateService.instant(
        'Odp.ConfirmationDialog.Header'
      );
      modalComponent.content = this.translateService.instant(
        'Odp.ConfirmationDialog.Content'
      );
      modalComponent.cancel.subscribe(() => {
        modalRef.close();
      });
      modalComponent.confirm.subscribe(() => {
        modalRef.close();
        this.cancelEditing();
        this.isDirty = false;
        this.showCatalogue = false;
      });
    } else {
      this.cancelEditing();
      this.isDirty = false;
      this.showCatalogue = false;
    }
    this.changeDetectorRef.detectChanges();
  }

  cancelEditing(): void {
    if (!this.pdplanDto.resultIdentity.id) {
      this.cancelCreateNode.emit();
    } else {
      this.changeMode(SurveyjsMode.display);
      if (this.isDirty) {
        this.pdplanDto = {
          ...this.pdplanDto,
          answer: { ...this.backupAnswer },
        };
        this.changeDetectorRef.detectChanges();
      }
    }
  }

  changeMode(modeParam: string): void {
    const variables = {};
    variables[SurveyVariableEnum.formDisplayMode] = modeParam;
    this.cxSurveyjsService.setVariables(variables);
    this.formJSON = {
      ...this.formJSON,
      mode: modeParam,
    };
    if (this.isKeyLearningProgram) {
      this.checkManagePDORight();
    }
    this.updateFormAction.emit();
    this.changeDetectorRef.detectChanges();
  }

  onChangeValue(eventData: any): void {
    if (this.formJSON.mode === SurveyjsMode.display) {
      return;
    }

    if (eventData.options.value !== eventData.options.oldValue) {
      this.isDirty = true;
    }
  }

  approveContent(): void {
    this.evaluate(PDEvaluationType.Approve);
  }

  rejectContent(): void {
    this.evaluate(PDEvaluationType.Reject);
  }

  doSubmitSurveyForm(): void {
    if (!this.cxSurveyComponent) {
      return;
    }
    this.cxSurveyComponent.doComplete();
  }

  onOpenPDCatalogue(): void {
    this.pauseAutoSaveScheduledTask();
    this.openPDCatalogDialog();
  }

  async onClickAddAreas(): Promise<void> {
    this.pauseAutoSaveScheduledTask();
    await this.showLearningAreasSelector();
  }

  onKLPRemovePDO(pdoURI: string): void {
    if (!this.allowManagePDO) {
      return;
    }
    this.showModalConfirmRemovePDO(pdoURI);
  }

  onKLPRemoveArea(areaId: string): void {
    this.removePlannedArea(areaId);
  }

  onClickPlannedPDO(pdo: PDOpportunityAnswerDTO): void {
    if (this.isEditing) {
      this.toastrService.warning('Please complete editing the form first');

      return;
    }
    this.currentPlannedPDO = pdo;
    window.scrollTo(0, 0);
    this.changeDetectorRef.detectChanges();
  }
  performAutoSavePD(): void {
    if (this.isDirty && !this.isPauseAutoSave) {
      if (!this.cxSurveyComponent.surveyModel.isCurrentPageHasErrors) {
        this.autoSave(this.cxSurveyComponent);
      }
    }
  }

  autoSave(eventData: any): void {
    this.isAutoSave = true;
    const surveyResult: any = eventData.survey.data;
    this.updateCustomKLPInfo(surveyResult);
    this.onSave(surveyResult, true);
    // Temporary hide
    // this.toastrService.success(this.successMessage[this.autoSaveMessage]);
    this.backupAnswer = ObjectUtilities.clone(this.pdplanDto.answer);
  }

  pauseAutoSaveScheduledTask(): void {
    this.isPauseAutoSave = true;
    this.unregisterPDAutoSaveScheduledTask(IdleConfig.AUTOSAVE_PD_CONTENT_TASK);
  }

  resumeAutoSaveScheduledTask(): void {
    this.isPauseAutoSave = false;
    if (this.isDirty) {
      this.registerPDAutoSaveTask(IdleConfig.AUTOSAVE_PD_CONTENT_TASK);
    }
  }

  registerPDAutoSaveTask(taskId: string): void {
    const autoSaveDigitalContentTask = new ScheduledTask(
      taskId,
      IdleConfig.AUTOSAVE_INTERVALTIME,
      () => this.performAutoSavePD(),
      false
    );
    if (!this.localScheduleService) {
      this.localScheduleService = new LocalScheduleService(
        this.browserIdleHandler
      );
    }
    this.localScheduleService.register(autoSaveDigitalContentTask);
  }

  unregisterPDAutoSaveScheduledTask(taskId: string): void {
    if (this.localScheduleService) {
      this.localScheduleService.unregister(taskId);
    }
  }

  duplicateLearningDirection(): void {
    const modalRef = this.ngbModal.open(
      DuplicateLearningDirectionDialogComponent,
      { centered: true, size: 'sm' }
    );
    const modalComponent = modalRef.componentInstance as DuplicateLearningDirectionDialogComponent;
    modalComponent.currentPDPlanDto = this.pdplanDto;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.success.subscribe((responseExtId) => {
      if (responseExtId) {
        const url = `/odp/plan-detail/${responseExtId.planExtId}`;
        this.router.navigate([url], {
          queryParams: { node: responseExtId.directionExtId },
        });
      }
      modalRef.close();
    });
  }

  onBackFromPlannedPDODetail(event: PDOpportunityAnswerDTO): void {
    if (event && event.learningOpportunity.source === 'custom-pdo') {
      this.checkIfUpdateExternalPDO(event);
    }
    this.turnOffMassNominationMode();
    this.currentPlannedPDO = undefined;
    this.changeDetectorRef.detectChanges();
  }

  onAddPDOFromCatalogue(pdCatalogCourse: PDCatalogCourseModel): void {
    this.onAddPDOToKLP(pdCatalogCourse);
    this.showCatalogue = false;
    this.changeDetectorRef.detectChanges();
    this.scrollToBottom();
    this.resumeAutoSaveScheduledTask();
  }

  onAddExternalPDOFromCatalogue(pdoDTO: PDOpportunityDTO): void {
    this.onAddExternalPDOToKLP(pdoDTO);
    this.showCatalogue = false;
    this.changeDetectorRef.detectChanges();
    this.scrollToBottom();
    this.resumeAutoSaveScheduledTask();
  }

  onClickBackOnCatalogue(): void {
    this.showCatalogue = false;
    this.resumeAutoSaveScheduledTask();
  }

  refresh(): void {
    this.ngOnChanges(undefined);
  }

  removeDraftVersion(): void {
    const deactivateAssessmentParams: DeactivateAssessmentParams = {
      pdPlan: this.pdplanDto,
      deactivateAllVersion: false,
      deactivateAllDescendants: false,
    };
    this.confirmRemoving(deactivateAssessmentParams);
  }

  removeAllVersions(): void {
    const deactivateAssessmentParams: DeactivateAssessmentParams = {
      pdPlan: this.pdplanDto,
      deactivateAllVersion: true,
      deactivateAllDescendants: true,
    };

    this.confirmRemoving(deactivateAssessmentParams);
  }

  massNominate(): void {
    this.turnOnMassNominationMode();
    this.changeDetectorRef.detectChanges();
  }

  get canEdit(): boolean {
    // tslint:disable-next-line: no-unsafe-any
    const isDisplayMode =
      this.formJSON && this.formJSON.mode === SurveyjsMode.display;
    const isSubmitting = this.checkPlanStatus(
      this.pdplanDto,
      OdpStatusCode.PendingForApproval
    );

    return (
      isDisplayMode &&
      !isSubmitting &&
      !this.currentPlannedPDO &&
      !this.isPlanOfPreviousYears(this.pdplanDto)
    );
  }

  get isEditing(): boolean {
    // tslint:disable-next-line: no-unsafe-any
    const isEditMode =
      this.formJSON && this.formJSON.mode === SurveyjsMode.edit;

    return isEditMode && !this.currentPlannedPDO;
  }

  get canSubmit(): boolean {
    // tslint:disable-next-line: no-unsafe-any
    const isDisplayMode =
      this.formJSON && this.formJSON.mode === SurveyjsMode.display;
    const isDraft = this.checkPlanStatus(this.pdplanDto, OdpStatusCode.Started);

    return isDisplayMode && isDraft && !this.currentPlannedPDO;
  }

  get canApproveReject(): boolean {
    // tslint:disable-next-line: no-unsafe-any
    const isDisplayMode =
      this.formJSON && this.formJSON.mode === SurveyjsMode.display;
    const isSubmitting = this.checkPlanStatus(
      this.pdplanDto,
      OdpStatusCode.PendingForApproval
    );

    return isDisplayMode && isSubmitting && !this.currentPlannedPDO;
  }

  get canDeleteDraft(): boolean {
    // tslint:disable-next-line: no-unsafe-any
    const isDisplayMode =
      this.formJSON && this.formJSON.mode === SurveyjsMode.display;
    const isDraft = this.checkPlanStatus(this.pdplanDto, OdpStatusCode.Started);
    const hasPreviousResult = this.pdplanDto.previousResultIdentity;
    const hasValidResultIdentity = ResultHelper.hasValidResultIdentity(
      this.pdplanDto
    );

    return (
      isDraft &&
      isDisplayMode &&
      hasPreviousResult &&
      hasValidResultIdentity &&
      !this.currentPlannedPDO &&
      !this.isPlanOfPreviousYears(this.pdplanDto)
    );
  }

  get canDeleteAllVersions(): boolean {
    const isDisplayMode =
      this.formJSON && this.formJSON.mode === SurveyjsMode.display;
    const isDraft = this.checkPlanStatus(this.pdplanDto, OdpStatusCode.Started);
    const hasValidResultIdentity = ResultHelper.hasValidResultIdentity(
      this.pdplanDto
    );

    return (
      isDraft &&
      isDisplayMode &&
      hasValidResultIdentity &&
      !this.currentPlannedPDO &&
      !this.isPlanOfPreviousYears(this.pdplanDto)
    );
  }

  get canDuplicateLearningDirection(): boolean {
    const isDisplayMode =
      this.formJSON && this.formJSON.mode === SurveyjsMode.display;
    const isAtLearningDirectionNode =
      this.pdplanDto.pdPlanActivity === OdpActivity.Direction;

    return isDisplayMode && isAtLearningDirectionNode;
  }

  get canMassNominate(): boolean {
    return (
      !this.isOnMassNominationMode &&
      this.pdplanDto.pdPlanActivity === OdpActivity.Programme &&
      this.formJSON.mode !== SurveyjsMode.edit &&
      !this.currentPlannedPDO
    );
  }

  get haveCommentPermission(): boolean {
    return this.pdplanDto.pdPlanActivity === OdpActivity.Plan
      ? this.learningPlanPermission.allowComment
      : this.learningDirectionPermission.allowComment;
  }

  get showOverallViewButton(): boolean {
    return this.pdplanDto.pdPlanActivity === OdpActivity.Plan;
  }

  get disableEditOnMassNominationMode(): boolean {
    return this.isOnMassNominationMode;
  }

  get planTitle(): string {
    if (this.clonedPdPlanDto && this.clonedPdPlanDto.answer) {
      return this.clonedPdPlanDto.answer.Title;
    }

    return 'N/A';
  }

  turnOnMassNominationMode(): void {
    this.isOnMassNominationMode = true;
  }

  turnOffMassNominationMode(): void {
    this.isOnMassNominationMode = false;
  }

  onChangeComment(changeData: CommentChangeData): void {
    const planId = this.pdplanDto.resultIdentity.extId;
    this.commentService
      .saveComment(
        this.keyLearningProgramHelper.getCommentEventEntity(this.pdplanDto),
        planId,
        changeData
      )
      .subscribe(
        (newCommentItem) => {
          changeData.commentItem = newCommentItem;
          changeData.changeResult = true;
          this.commentComponent.changeCommentResult(changeData);
        },
        (error) => {
          console.error(error);
        }
      );
  }

  async updateCommentData(): Promise<void> {
    const isValidActivity = this.isLearningDirection || this.isLearningPlan;
    const isNewPLan = this.isNewPlan();
    if (!isValidActivity || isNewPLan) {
      this.showCommentSection = false;

      return;
    }
    this.showCommentSection = true;
    const planId = this.pdplanDto.resultIdentity.extId;
    this.comments = await this.commentService.getCommentsAsync(
      this.keyLearningProgramHelper.getCommentEventEntity(this.pdplanDto),
      planId
    );
    this.changeDetectorRef.detectChanges();
  }

  get isKeyLearningProgram(): boolean {
    return this.pdplanDto.pdPlanActivity === OdpActivity.Programme;
  }

  get isLearningDirection(): boolean {
    return this.pdplanDto.pdPlanActivity === OdpActivity.Direction;
  }

  get isLearningPlan(): boolean {
    return this.pdplanDto.pdPlanActivity === OdpActivity.Plan;
  }

  get isPlanHasValidResult(): boolean {
    return ResultHelper.hasValidResultIdentity(this.pdplanDto);
  }

  // Check learning plan, learning direction is have this status code
  private checkPlanStatus(plan: PDPlanDto, statusCode: string): boolean {
    if (this.isLearningDirection || this.isLearningPlan) {
      if (
        plan.assessmentStatusInfo &&
        plan.assessmentStatusInfo.assessmentStatusCode === statusCode
      ) {
        return true;
      }
    }

    return false;
  }

  /**
   * Check whether it is the plan of previous years or not.
   * @param plan The PD Plan dto.
   */
  private isPlanOfPreviousYears(plan: PDPlanDto): boolean {
    return (
      plan &&
      plan.surveyInfo &&
      plan.surveyInfo.endDate < new Date().toISOString()
    );
  }

  private isNewPlan(): boolean {
    return (
      !this.pdplanDto.resultIdentity || !(this.pdplanDto.resultIdentity.id > 0)
    );
  }

  private initData(): void {
    this.title = OdpActivityName[this.pdplanDto.pdPlanActivity];
    this.allowManagePDO = false;
    this.currentPlannedPDO = undefined;
    this.successMessage[OdpActivity.Plan] = this.translateService.instant(
      'Odp.StartNewPlan.SubmitSuccess'
    );
    this.successMessage[OdpActivity.Direction] =
      'Saved Learning Direction successfully!';
    this.successMessage[OdpActivity.Programme] =
      'Saved Key Learning Programme successfully!';
    this.successMessage[this.autoSaveMessage] = 'Changes are saved!';
    this.isOnMassNominationMode = false;
    this.showCatalogue = false;
    this.catalogTagIds = undefined;
    this.catalogPersonnelGroupsIdsForExternalPDOUsage = undefined;
    this.catalogSelectedCourse = undefined;
  }

  private initPdPlanDto(): void {
    this.isAutoSave =
      this.clonedPdPlanDto !== null &&
      this.clonedPdPlanDto.resultIdentity.extId ===
        this.pdplanDto.resultIdentity.extId
        ? this.isAutoSave
        : false;

    this.clonedPdPlanDto =
      this.clonedPdPlanDto !== null &&
      this.clonedPdPlanDto.resultIdentity.id ===
        this.pdplanDto.resultIdentity.id
        ? this.clonedPdPlanDto
        : cloneDeep(this.pdplanDto);
  }

  private evaluate(type: PDEvaluationType): void {
    if (this.ngbModal.hasOpenModals()) {
      this.ngbModal.dismissAll();
    }

    const modalRef = this.ngbModal.open(PdEvaluationDialogComponent, {
      centered: true,
      size: 'lg',
    });
    const modalRefComponentInstance = modalRef.componentInstance as PdEvaluationDialogComponent;
    modalRefComponentInstance.header = this.translateService.instant(
      this.evaluationDialogHeader[type]
    );
    modalRefComponentInstance.doneButtonText = this.translateService.instant(
      this.evaluationDialogHeader[type]
    );
    modalRefComponentInstance.done.subscribe((reason) => {
      this.onEvaluated(new PDEvaluationModel({ type, reason }));
      modalRef.close();
    });
    modalRefComponentInstance.cancel.subscribe(() => modalRef.close());
  }

  private onEvaluated(result: PDEvaluationModel): void {
    // Emit approve/reject event.
    const statusInfo = new AssessmentStatusInfo();
    statusInfo.assessmentStatusCode =
      EvaluationTypeToIdpStatusCode[result.type];
    const eventData = {
      pdPlan: this.pdplanDto,
      assessmentStatusInfo: statusInfo,
      pdEvaluationModel: result,
    };
    if (result.type === PDEvaluationType.Approve) {
      this.approve.emit(eventData);
    }

    if (result.type === PDEvaluationType.Reject) {
      this.reject.emit(eventData);
    }
  }

  private onSave(surveyResult: any, isSilent: boolean = false): void {
    if (!this.isAutoSave) {
      window.scroll(0, 0);
    }
    const newPdplanDto = cloneDeep(this.pdplanDto);
    newPdplanDto.answer = surveyResult;
    if (newPdplanDto.assessmentStatusInfo) {
      newPdplanDto.assessmentStatusInfo.assessmentStatusId =
        OdpStatusEnum.Started;
      newPdplanDto.assessmentStatusInfo.assessmentStatusCode =
        OdpStatusCode.Started;
    }
    if (this.isKeyLearningProgram) {
      this.keyLearningProgramHelper.initDataForKLPWhenCreate(newPdplanDto);
    }
    const eventData = {
      newPlan: newPdplanDto,
      oldPlan: this.pdplanDto,
      surveyFormJSON: this.formJSON,
      isSilent,
    };

    //If surveyResult has data from learning direction
    if (surveyResult && surveyResult.relatedStrategicThrusts) {
      newPdplanDto.additionalProperties = {
        objectiveResultExtId: surveyResult.relatedStrategicThrusts,
      };
    }

    this.clonedPdPlanDto = newPdplanDto;
    this.save.emit(eventData);
    this.isDirty = false;
  }

  private async openPDCatalogDialog(): Promise<void> {
    window.scroll(0, 0);
    const formData = this.getCurrentSurveyData();
    const taggingByTargetAudience = await this.klpSerivce.getTaggingByTargetAudience(
      formData,
      [ArchetypeEnum.PersonnelGroup, ArchetypeEnum.DevelopmentalRole],
      true
    );
    this.catalogPersonnelGroupsIdsForExternalPDOUsage =
      taggingByTargetAudience.PersonnelGroup;
    this.catalogTagIds = this.klpSerivce.buildGroupTagFromKLPData(
      formData,
      taggingByTargetAudience,
      this.pdplanDto.answer.listLearningArea
    );
    this.catalogSelectedCourse = this.klpSerivce.getSelectedCourseIdsOnKLP(
      formData
    );
    this.showCatalogue = true;
  }

  private onAddPDOToKLP = (pdCatalogCourse: PDCatalogCourseModel): void => {
    if (
      !pdCatalogCourse ||
      !pdCatalogCourse.course ||
      !this.pdplanDto.answer ||
      !this.isKeyLearningProgram
    ) {
      return;
    }

    if (!this.pdplanDto.answer.listLearningOpportunity) {
      this.pdplanDto.answer.listLearningOpportunity = [];
    }

    const pdos: PDOpportunityAnswerDTO[] = this.pdplanDto.answer
      .listLearningOpportunity;
    const isExisted = this.klpSerivce.checkPDOExisted(
      pdCatalogCourse.course.id,
      pdos
    );

    if (isExisted) {
      this.toastrService.info(
        'This course existed on your Key Learning Programme.'
      );

      return;
    }

    this.isDirty = true;
    pdCatalogCourse.isSelected = true;

    // Update KLP data
    const pdoAnswer = PDPlannerHelpers.toPDOpportunityAnswerDTO(
      pdCatalogCourse
    );

    pdos.push(pdoAnswer);

    this.scrollToBottom();
    this.pdplanDto = { ...this.pdplanDto };
    this.changeDetectorRef.detectChanges();
  };

  private onAddExternalPDOToKLP = (pdoDTO: PDOpportunityDTO): void => {
    if (!pdoDTO || !this.isKeyLearningProgram) {
      return;
    }
    this.isDirty = true;

    if (!this.pdplanDto.answer.listLearningOpportunity) {
      this.pdplanDto.answer.listLearningOpportunity = [];
    }

    // Update KLP data
    const pdos = this.pdplanDto.answer.listLearningOpportunity;
    const pdoAnswer: PDOpportunityAnswerDTO = {
      learningOpportunity: pdoDTO,
    };
    pdos.push(pdoAnswer);

    this.scrollToBottom();
    this.pdplanDto = { ...this.pdplanDto };
    this.changeDetectorRef.detectChanges();
  };

  private klpRemovePDOByUri(pdoURI: string): void {
    if (!pdoURI || !this.pdplanDto) {
      return;
    }

    const pdos: PDOpportunityAnswerDTO[] = this.pdplanDto.answer
      .listLearningOpportunity;
    const index = pdos.findIndex(
      (pdo: PDOpportunityAnswerDTO) => pdo.learningOpportunity.uri === pdoURI
    );

    this.isDirty = true;

    pdos.splice(index, 1);

    this.pdplanDto = { ...this.pdplanDto };

    this.changeDetectorRef.detectChanges();
  }

  private removePlannedArea(areaId: string): void {
    if (!areaId) {
      return;
    }
    const areas: KLPPlannedAreaModel[] = this.pdplanDto.answer.listLearningArea;
    const index = areas.findIndex(
      (plannedArea: KLPPlannedAreaModel) =>
        plannedArea.area && plannedArea.area.id === areaId
    );
    this.isDirty = true;

    areas.splice(index, 1);

    this.pdplanDto = { ...this.pdplanDto };

    this.changeDetectorRef.detectChanges();
  }

  private checkManagePDORight(): void {
    const validKLP =
      this.isKeyLearningProgram &&
      this.keyLearningProgramHelper.hasParentLearningDirectionApproved(
        this.parentDTO
      );
    const hasRightToEdit = this.keyLearningProgrammePermission.allowEdit;

    this.allowManagePDO = hasRightToEdit && validKLP;
  }

  private async showLearningAreasSelector(): Promise<void> {
    const formData = this.getCurrentSurveyData();
    const taggingByTargetAudience = await this.klpSerivce.getTaggingByTargetAudience(
      formData,
      [ArchetypeEnum.PersonnelGroup],
      true
    );
    const modalRef = this.klpSerivce.showLearningAreaSelectorPopup(
      taggingByTargetAudience.PersonnelGroup
    );
    if (!modalRef) {
      return;
    }

    const modalRefComponent = modalRef.componentInstance as CxFormModal;
    modalRefComponent.changeValue.subscribe(() => {});
    modalRefComponent.submitting.subscribe((event: SurveySubmitEventData) => {
      const result = event.survey.data as LearningAreaFormSelectorResultModel;
      this.addLearningAreas(result);
      modalRef.close();
      this.resumeAutoSaveScheduledTask();
    });
    modalRef.componentInstance.cancel.subscribe(() => {
      this.resumeAutoSaveScheduledTask();
    });
  }

  private addLearningAreas(result: LearningAreaFormSelectorResultModel): void {
    if (!result || isEmpty(result.listLearningArea)) {
      return;
    }
    if (!this.pdplanDto || !this.pdplanDto.answer) {
      return;
    }

    const framework: MetadataObject =
      result.allLearningFrameWork || result.learningFrameWorkByServiceSchemes;
    const dimension: MetadataObject =
      result.learningDimensionByAllLearningFramework ||
      result.learningDimensionByServiceSchemes;
    const ares: MetadataObject[] = result.listLearningArea;
    let listAreaModels = ares.map(
      (area) => new KLPPlannedAreaModel(framework, dimension, area)
    );
    this.pdplanDto.answer.listLearningArea =
      this.pdplanDto.answer.listLearningArea || [];

    // Check exist
    const existId: string[] = this.pdplanDto.answer.listLearningArea.map(
      (areaModel) => areaModel.area && areaModel.area.id
    );
    listAreaModels = listAreaModels.filter(
      (areaModel) => !existId.includes(areaModel.area.id)
    );
    if (isEmpty(listAreaModels)) {
      return;
    }

    this.isDirty = true;
    this.pdplanDto.answer.listLearningArea = this.pdplanDto.answer.listLearningArea.concat(
      listAreaModels
    );

    this.pdplanDto = { ...this.pdplanDto };
    this.changeDetectorRef.detectChanges();
  }

  // Update learning areas and learning opportunity info into survey form data.
  private updateCustomKLPInfo(surveyResult: any): void {
    if (
      this.isKeyLearningProgram &&
      !!this.pdplanDto &&
      !!this.pdplanDto.answer
    ) {
      surveyResult.listLearningOpportunity = this.pdplanDto.answer.listLearningOpportunity;
      surveyResult.listLearningArea = this.pdplanDto.answer.listLearningArea;
    }
  }

  private getCurrentSurveyData(): any {
    if (
      !this.cxSurveyComponent ||
      !this.cxSurveyComponent.surveyModel ||
      !this.cxSurveyComponent.surveyModel.data
    ) {
      return undefined;
    }

    return this.cxSurveyComponent.surveyModel.data;
  }

  private scrollToBottom(): void {
    window.scrollTo(0, document.body.scrollHeight);
  }

  private confirmRemoving(
    deactivateAssessmentParams: DeactivateAssessmentParams
  ): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'lg',
      centered: true,
    });
    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.No'
    );
    modalComponent.confirmButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.Confirm'
    );
    modalComponent.header = this.translateService.instant(
      'Odp.ConfirmationDialog.Header'
    );
    const confirmationContentResourceKey =
      deactivateAssessmentParams.deactivateAllDescendants === true
        ? 'Odp.ConfirmationDialog.DeleteAllVersionConfirmation'
        : 'Odp.ConfirmationDialog.DeleteDraftWithPreviousSavedVersion';
    this.translateService
      .get(confirmationContentResourceKey, {
        odpActivityName: (deactivateAssessmentParams.pdPlan.pdPlanActivity ===
        OdpActivity.Plan
          ? OdpActivityName.LearningPlan
          : OdpActivityName.LearningDirection
        ).toLowerCase(),
        odpTitle: deactivateAssessmentParams.pdPlan.answer.Title,
      })
      .subscribe((res: string) => {
        modalComponent.content = res;
      });
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      modalRef.close();
      this.delete.emit(deactivateAssessmentParams);
    });
  }

  private checkIfUpdateExternalPDO(pdoAnswer: PDOpportunityAnswerDTO): void {
    Object.assign(
      this.pdplanDto.answer.listLearningOpportunity,
      this.pdplanDto.answer.listLearningOpportunity.map((pdo) =>
        pdo.learningOpportunity.uri === pdoAnswer.learningOpportunity.uri
          ? pdoAnswer
          : pdo
      )
    );
    this.pdplanDto = { ...this.pdplanDto };
  }

  private async showModalConfirmRemovePDO(pdoURI: string): Promise<void> {
    if (!pdoURI || !this.pdplanDto) {
      return;
    }

    if (this.ngbModal.hasOpenModals()) {
      this.ngbModal.dismissAll();
    }

    const confirmationText = await this.getRemovePDOConfirmationText(pdoURI);

    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.Cancel'
    );
    modalComponent.confirmButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.ConfirmOK'
    );
    modalComponent.header = this.translateService.instant(
      'Odp.ConfirmationDialog.Header'
    );
    modalComponent.content = confirmationText;

    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });

    modalComponent.confirm.subscribe((rejectReason?: string) => {
      this.klpRemovePDOByUri(pdoURI);
      modalRef.close();
    });
  }

  private async getRemovePDOConfirmationText(pdoURI: string): Promise<string> {
    const pdos: PDOpportunityAnswerDTO[] = this.pdplanDto.answer
      .listLearningOpportunity;

    if (isEmpty(pdos)) {
      return;
    }

    const index = pdos.findIndex(
      (pdo: PDOpportunityAnswerDTO) => pdo.learningOpportunity.uri === pdoURI
    );

    const pdoAnswer = pdos[index];

    if (isEmpty(pdoAnswer)) {
      return;
    }

    const confirmationTranslatePath =
      'Odp.ConfirmationDialog.DeletePDOOnKLPConfirmation';

    const pdoTitle =
      pdoAnswer.learningOpportunity && pdoAnswer.learningOpportunity.name;

    const confirmationText = await this.translateService
      .get(confirmationTranslatePath, {
        klpTitle: this.planTitle,
        pdoTitle,
      })
      .toPromise();

    return confirmationText || 'N/A';
  }
}
