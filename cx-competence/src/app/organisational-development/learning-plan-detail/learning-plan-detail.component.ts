import { Location } from '@angular/common';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxNode,
  CxNodeComponent,
  CxSurveyJsUtil,
  CxTreeIcon,
} from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import { User } from 'app-models/auth.model';
import { CxBreadCrumbItem } from 'app-models/breadcrumb.model';
import { Identity } from 'app-models/common.model';
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
import { DeactivateAssessmentParams } from 'app-models/deactivateAssessment.model';
import { PDPlanConfig, PDPlanDto, PdPlanType } from 'app-models/pdplan.model';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { CommentService } from 'app-services/comment.service';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { ResultHelper } from 'app-services/idp/result-helpers';
import { LocalScheduleService } from 'app-services/local-schedule.service';
import { KeyLearningProgramHelper } from 'app-services/odp/learning-plan-services/key-learning-program.helper';
import { BrowserIdleHandler } from 'app-utilities/browser-idle.handler';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { GroupStoreService } from 'app/core/store-services/group-store.service';
import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { SurveyjsMode } from 'app/shared/constants/surveyjs-mode.constant';
import { clone, cloneDeep, isEmpty, uniqueId } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs/operators';
import { LearningPlanContent } from '../models/learning-plan-content.model';
import { ChangePDPlanStatusDto, ODPFilterParams } from '../models/odp.models';
import { OdpService } from '../odp.service';
import { LearningPlanContentComponent } from './learning-plan-content/learning-plan-content.component';
import {
  NodeStatusType,
  OdpActivity,
  OdpActivityName,
  OdpStatusCode,
} from './odp.constant';

@Component({
  selector: 'learning-plan-detail',
  templateUrl: './learning-plan-detail.component.html',
  styleUrls: ['./learning-plan-detail.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LearningPlanDetailComponent
  extends BaseScreenComponent
  implements
    OnInit,
    ILearningPlanPermission,
    ILearningDirectionPermission,
    IKeyLearningProgrammePermission {
  @ViewChild('cxPlanTree') cxPlanTreeComponent: CxNodeComponent;
  @ViewChild('planContent') planContentComponent: LearningPlanContentComponent;
  @ViewChild('viewLeftSide') viewLeftSide: ElementRef;
  @ViewChild('viewDivider') viewDivider: ElementRef;
  @ViewChild('viewToggler') viewToggler: ElementRef;
  @ViewChild('viewRightSide') viewRightSide: ElementRef;
  @ViewChild('cxSplitView') cxSplitView: ElementRef;

  activePdplan: LearningPlanContent;
  activeParentPdplanDTO: PDPlanDto;

  planFormJSON: any;
  directionFormJSON: any;
  programmeFormJSON: any;
  successMessage: any = {};
  odpContents: LearningPlanContent[];

  odpActivityName: any = OdpActivityName;
  odpTreeIcon: CxTreeIcon = new CxTreeIcon({
    add: 'material-icons add',
    collapse: 'material-icons arrow-drop-down',
    expand: 'material-icons arrow-right',
  });
  itemTrackByFn: (index: number, item: any) => {};

  planCxNode: CxNode;
  selectedNodeComponent: CxNodeComponent;
  isCreating: boolean = false;
  currentBreadcrumb: CxBreadCrumbItem[] = []; // Breadcrumb data

  hasRightCreateLearningDirection: boolean = false;
  hasRightCreateKeyLearningProgram: boolean = false;
  showAddLearningDirectionButton: boolean = false;

  // Permission
  learningPlanPermission: LearningPlanPermission;
  learningDirectionPermission: LearningDirectionPermission;
  keyLearningProgrammePermission: KeyLearningProgrammePermission;

  // Split view
  private hasMouseDownDivider: boolean = false;
  private flipClass: string = 'cx-flip';
  private pointerResize: string = 'pointer-resize';
  private transitionClass: string = 'cx-light-transition';
  private hiddentOverflowYClass: string = 'hidden-overflow-y';
  private disableSelectText: string = 'disable-select-text';
  private currentNodeId: string;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    authService: AuthService,
    private toastrService: ToastrService,
    private odpService: OdpService,
    private route: ActivatedRoute,
    private router: Router,
    private breadcrumbSettingService: BreadcrumbSettingService,
    private commentService: CommentService,
    private locationService: Location,
    private translateService: TranslateService,
    private globalLoader: CxGlobalLoaderService,
    public browserIdleHandler: BrowserIdleHandler,
    private keyLearningProgramHelper: KeyLearningProgramHelper,
    private groupStoreService: GroupStoreService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService
  ) {
    super(changeDetectorRef, authService);
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
    this.initLearningPlanPermissionn(this.currentUser);
    this.initLearningDirectionPermission(this.currentUser);
    this.initKeyLearningProgrammePermission(this.currentUser);
    this.initData();
    this.currentNodeId = this.route.snapshot.queryParams.node;
    this.getLearningPlanFormAPI(this.route.snapshot.params.extId);
  }

  initData(): void {
    this.hasRightCreateLearningDirection = this.learningDirectionPermission.allowCreate;
    this.hasRightCreateKeyLearningProgram = this.keyLearningProgrammePermission.allowCreate;
  }

  async getLearningPlanFormAPI(extID: string): Promise<void> {
    this.globalLoader.showLoader();
    // Get ODP config
    const odpConfigs: PDPlanConfig[] = await this.getODPConfig();
    if (!odpConfigs) {
      this.globalLoader.hideLoader();
      this.toastrService.error(`Can't get Organisational PD Journey configs!`);
      this.changeDetectorRef.detectChanges();

      return;
    }

    // Get learning Plan result tree
    const planTreeArray: PDPlanDto[] = await this.getLearningPlanResultTree(
      extID
    );
    if (!planTreeArray || planTreeArray.length === 0) {
      this.globalLoader.hideLoader();
      this.toastrService.error(
        this.translateService.instant('Odp.LearningPlan.FailedToRetrieveData')
      );
      this.router.navigate([`/odp`]);
      this.changeDetectorRef.detectChanges();

      return;
    }

    const planTree = planTreeArray[0];
    await this.initOdpForms(odpConfigs, planTree);
    this.updateBreadcrumbWhenReload(planTree.answer.Title);
    this.convertDatetimeToSurveyFormat(planTree);
    this.planCxNode = this.toCxNode(planTree);
    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  }

  async onSave(data: {
    newPlan: PDPlanDto;
    oldPlan: PDPlanDto;
    surveyFormJSON: any;
    isSilent: boolean;
  }): Promise<void> {
    if (!data.isSilent) {
      this.globalLoader.showLoader();
    }
    const pdPlanActivity = data.oldPlan.pdPlanActivity;
    const oldPlan = data.oldPlan;
    const surveyFormJSON = data.surveyFormJSON;
    const isCreating = !oldPlan.resultIdentity.id;
    let newPlan = cloneDeep(data.newPlan);

    if (isCreating) {
      // Case new plan
      const plan = new PDPlanDto();
      plan.objectiveInfo = newPlan.objectiveInfo;
      plan.answer = newPlan.answer;
      plan.parentResultExtId = newPlan.parentResultExtId;
      plan.timestamp = newPlan.timestamp;
      newPlan = plan;
    }
    newPlan.forceCreateResult = isCreating; // Creating new -> Set flag forceCreateResult to "true"

    this.successMessage[OdpActivity.Plan] = this.translateService.instant(
      'Odp.StartNewPlan.SubmitSuccess'
    );
    this.successMessage[OdpActivity.Direction] =
      'Saved Learning Direction successfully!';
    this.successMessage[OdpActivity.Programme] =
      'Saved Key Learning Programme successfully!';
    this.convertDatetimeToISOFormat(newPlan);
    const responePlan = await this.saveOrCreateLearningPlan(
      newPlan,
      pdPlanActivity
    );
    if (responePlan) {
      this.convertDatetimeToSurveyFormat(responePlan);
      this.updateSavedOdp(
        responePlan,
        surveyFormJSON,
        isCreating,
        data.isSilent
      );
      if (!data.isSilent) {
        this.toastrService.success(this.successMessage[pdPlanActivity]);
      }
    } else {
      this.toastrService.error('Save unsuccessfully!');
    }
    if (!data.isSilent) {
      this.globalLoader.hideLoader();
    }
    this.changeDetectorRef.detectChanges();
  }

  onApprove(param: any): void {
    this.onApproveOrReject(param);
  }

  onReject(param: any): void {
    this.onApproveOrReject(param);
  }

  async updateBreadcrumbWhenReload(planTitle: string): Promise<void> {
    const newBreadcrumb = new CxBreadCrumbItem(planTitle, '');
    const breadcrumbMaxLength = 3;
    const currentBreadcrumb = this.breadcrumbSettingService.currentBreadcrumb;
    if (currentBreadcrumb.length === breadcrumbMaxLength) {
      currentBreadcrumb.pop();
    }
    currentBreadcrumb.push(newBreadcrumb);
    this.addItemToBreadcrumb(currentBreadcrumb);
  }

  updateComponentWhenChangeStatus(planDTO: PDPlanDto): void {
    this.updateCxNodeInfo(planDTO, this.selectedNodeComponent.node);
    this.planContentComponent.refresh();
    this.selectedNodeComponent.refresh();
    this.showChangeStatusSuccessMessage(planDTO);
  }

  updateChangeStatus(
    odpContent: LearningPlanContent,
    newOdpStatus: ChangePDPlanStatusDto
  ): void {
    const odpContentIndex = this.odpContents.findIndex(
      (odp) =>
        odp.pdplanDto.resultIdentity.id ===
        odpContent.pdplanDto.resultIdentity.id
    );
    const newpdplanDto = cloneDeep(odpContent.pdplanDto);
    newpdplanDto.assessmentStatusInfo = newOdpStatus.targetStatusType;
    this.odpContents[odpContentIndex].pdplanDto = newpdplanDto;
    this.showChangeStatusSuccessMessage(newpdplanDto);
    this.changeDetectorRef.detectChanges();
  }

  showChangeStatusSuccessMessage(newpdplanDto: PDPlanDto): void {
    let message = '';
    if (newpdplanDto.assessmentStatusInfo) {
      if (
        newpdplanDto.assessmentStatusInfo.assessmentStatusCode ===
        OdpStatusCode.PendingForApproval
      ) {
        message = 'Submitted successfully!';
      }
      if (
        newpdplanDto.assessmentStatusInfo.assessmentStatusCode ===
        OdpStatusCode.Approved
      ) {
        message = 'Approved successfully!';
      }
      if (
        newpdplanDto.assessmentStatusInfo.assessmentStatusCode ===
        OdpStatusCode.Rejected
      ) {
        message = 'Rejected successfully!';
      }
    }
    if (message.length > 0) {
      this.toastrService.success(message);
    } else {
      this.toastrService.error('Error happen!');
    }
  }

  getAddOdpItemToolTip(odpContent: LearningPlanContent): string {
    switch (odpContent.pdplanDto.pdPlanActivity) {
      case OdpActivity.Plan:
        return 'Add Learning Direction';
      case OdpActivity.Direction:
        return 'Add Learning Programme';
      default:
        return '';
    }
  }

  onCancelCreateNode(): void {
    this.isCreating = false;
    this.nodeClickedHandler(this.selectedNodeComponent);
  }

  onDelete(deactivateAssessmentParams: DeactivateAssessmentParams): void {
    const pdplanDto = deactivateAssessmentParams.pdPlan;
    if (
      !pdplanDto ||
      !pdplanDto.resultIdentity.id ||
      !pdplanDto.assessmentStatusInfo
    ) {
      return;
    }
    if (deactivateAssessmentParams.deactivateAllVersion === true) {
      this.handleDeletingAllVersions(deactivateAssessmentParams);
    } else {
      this.handleDeletingDraftVersion(deactivateAssessmentParams);
    }
  }

  onMouseDown(event: any): void {
    if (!this.cxSplitView) {
      return;
    }
    const DIVIDER_ID = 'divider';
    if (event.target && event.target.id === DIVIDER_ID) {
      if (
        this.cxSplitView &&
        !this.cxSplitView.nativeElement.classList.contains(
          this.disableSelectText
        )
      ) {
        this.cxSplitView.nativeElement.classList.add(this.disableSelectText);
      }
      this.hasMouseDownDivider = true;
    }
  }

  @HostListener('mouseup', ['$event'])
  onMouseUp(): void {
    if (!this.cxSplitView) {
      return;
    }
    this.hasMouseDownDivider = false;
    this.cxSplitView.nativeElement.classList.remove(this.disableSelectText);
  }

  @HostListener('mousemove', ['$event'])
  onMouseMove(event: any): void {
    if (!this.viewToggler || !this.viewLeftSide || !this.viewDivider) {
      return;
    }

    const viewToggler = this.viewToggler.nativeElement;
    const leftSide = this.viewLeftSide.nativeElement;

    if (this.hasMouseDownDivider && this.cxSplitView) {
      if (viewToggler.classList.contains(this.flipClass)) {
        return;
      }

      const leftSideWidth = this.calculateLeftSideWidth(event);
      leftSide.classList.remove(this.transitionClass);
      leftSide.style.width = leftSideWidth + 'px';
    }
  }

  onClicktoggleButton(): void {
    if (!this.viewLeftSide || !this.viewToggler || !this.viewDivider) {
      return;
    }

    const leftSide = this.viewLeftSide.nativeElement;
    const viewToggler = this.viewToggler.nativeElement;
    const divider = this.viewDivider.nativeElement;
    const minWidth = 300;

    if (!leftSide.classList.contains(this.transitionClass)) {
      leftSide.classList.add(this.transitionClass);
    }

    divider.classList.toggle(this.pointerResize);
    viewToggler.classList.toggle(this.flipClass);
    leftSide.classList.toggle(this.hiddentOverflowYClass);
    leftSide.style.width = viewToggler.classList.contains(this.flipClass)
      ? 0 + 'px'
      : minWidth + 'px';
  }

  calculateLeftSideWidth(eventData: any): number {
    if (!this.cxSplitView || !this.viewLeftSide) {
      return;
    }

    const splitViewWidth = this.cxSplitView.nativeElement.offsetWidth;
    const splitViewPadding = 200;
    const minWidth = 300;
    const maxWidth = splitViewWidth - splitViewPadding;
    const leftSide = this.viewLeftSide.nativeElement;
    const boxRectangle = leftSide.getBoundingClientRect();
    let width = eventData.pageX - boxRectangle.left;

    width = width < minWidth ? minWidth : width;
    width = width > maxWidth ? maxWidth : width;

    return width;
  }

  /*
        CxTree Function & Event handler
    */
  nodeClickedHandler(cxNodeComponent: CxNodeComponent): void {
    const currentNode = cxNodeComponent.node;
    if (!cxNodeComponent || !currentNode) {
      return;
    }
    const localScheduleService = new LocalScheduleService(
      this.browserIdleHandler
    );
    window.scroll(0, 0);
    this.isCreating = false;
    this.selectedNodeComponent = cxNodeComponent;
    this.activeParentPdplanDTO =
      currentNode.parent && currentNode.parent.dataObject
        ? currentNode.parent.dataObject.pdplanDto
        : undefined;
    this.changeActivePdplan(currentNode.dataObject);
    this.updateRoute(currentNode);
    this.changeDetectorRef.detectChanges();
  }

  createNodeClickedHandler(nodeComponent: CxNodeComponent): void {
    if (this.planContentComponent && this.planContentComponent.isDirty) {
      window.scroll(0, 0);
      this.toastrService.warning('Please complete current form first');

      return;
    }

    this.selectedNodeComponent = nodeComponent; // Update current selected node
    this.updateRoute(nodeComponent.node); // Update route for current node

    const parentPlanContent = nodeComponent.node.dataObject;
    const parentPlanDTO = parentPlanContent.pdplanDto;
    const newPdplanDto = new PDPlanDto();

    this.isCreating = true;
    newPdplanDto.pdPlanType = PdPlanType.Odp;
    newPdplanDto.answer = {};
    newPdplanDto.resultIdentity = new Identity();
    newPdplanDto.resultIdentity.extId = (-uniqueId()).toString();
    newPdplanDto.objectiveInfo = parentPlanDTO.objectiveInfo;
    newPdplanDto.parentResultExtId = parentPlanDTO.resultIdentity.extId;
    newPdplanDto.timestamp = parentPlanDTO.timestamp;

    let newFormJSON;

    switch (parentPlanDTO.pdPlanActivity) {
      case OdpActivity.Plan:
        newFormJSON = cloneDeep(this.directionFormJSON);
        newPdplanDto.pdPlanActivity = OdpActivity.Direction;
        break;
      case OdpActivity.Direction:
        newFormJSON = cloneDeep(this.programmeFormJSON);
        newPdplanDto.pdPlanActivity = OdpActivity.Programme;
        break;
      default:
        break;
    }

    newFormJSON.mode = SurveyjsMode.edit;
    const pdplanContent = new LearningPlanContent({
      formJSON: newFormJSON,
      pdplanDto: newPdplanDto,
      id: uniqueId(),
    });
    window.scroll(0, 0);
    this.activeParentPdplanDTO = parentPlanDTO;
    this.changeActivePdplan(pdplanContent);
  }

  newNodeCreatedHandler(component: CxNodeComponent): void {
    if (
      !component ||
      !component.node ||
      !component.node.dataObject ||
      !this.isCreating
    ) {
      return;
    }
    this.isCreating = false;
    const planContent: LearningPlanContent = component.node.dataObject;
    const waitTime = 200;
    if (planContent.pdplanDto.pdPlanActivity === OdpActivity.Direction) {
      setTimeout(() => {
        this.scrollToEndTree();
      }, waitTime);
    }
    this.nodeClickedHandler(component);
  }

  createLearningDirectionClickedHandler(): void {
    if (!this.cxPlanTreeComponent) {
      return;
    }
    this.createNodeClickedHandler(this.cxPlanTreeComponent);
  }

  onClickBreadcrumbItem(route: any): void {
    if (route && this.currentUser) {
      if (route && route.value) {
        this.router.navigate([route.value]);
      } else {
        this.router.navigate([route]);
      }

      const itemBreadcrumbs = this.breadcrumbSettingService.mapRouteToBreadcrumb(
        this.currentUser.headerData.menus,
        route
      );
      this.addItemToBreadcrumb(itemBreadcrumbs);
    }
  }

  onCxNodeReady(rootComponent: CxNodeComponent): void {
    if (!rootComponent) {
      return;
    }
    let targetNode = rootComponent;

    if (this.currentNodeId) {
      const resultNode = rootComponent.findComponentById(this.currentNodeId);
      if (resultNode) {
        targetNode = resultNode;
      }
    }

    this.nodeClickedHandler(targetNode);
    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
    this.showAddLearningDirectionButton = true;
  }

  onClicMassNominate(): void {
    this.planContentComponent.massNominate();
  }

  // Actions button events
  onClickOverallView(): void {
    this.router.navigate([
      `/odp/overall-org-plan/${this.activePdplan.pdplanDto.resultIdentity.extId}`,
    ]);
  }

  onClickEdit(): void {
    this.planContentComponent.switchToEditMode();
  }

  onClickSubmit(): void {
    this.submitPlan();
  }

  onClickApprove(): void {
    this.planContentComponent.approveContent();
  }

  onClickReject(): void {
    this.planContentComponent.rejectContent();
  }

  onClickDeleteDraft(): void {
    this.planContentComponent.removeDraftVersion();
  }

  onClickDeleteAllVersions(): void {
    this.planContentComponent.removeAllVersions();
  }

  onClickCancelEdited(): void {
    this.planContentComponent.onCancel();
  }

  onClickSaveEdtied(): void {
    this.planContentComponent.doSubmitSurveyForm();
  }

  onClickDuplicateLearningDirection(): void {
    this.planContentComponent.duplicateLearningDirection();
  }

  onupdateFormAction(): void {
    this.changeDetectorRef.detectChanges();
  }

  get currentActivePlanTypeName(): string {
    if (!this.activePdplan || !this.activePdplan.pdplanDto) {
      return '';
    }

    return OdpActivityName[this.activePdplan.pdplanDto.pdPlanActivity];
  }

  get isEditingContent(): boolean {
    return this.planContentComponent && this.planContentComponent.isEditing;
  }

  get canEditContent(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.canEdit &&
      (this.currentActivePlanTypeName === OdpActivityName.LearningPlan
        ? this.learningPlanPermission.allowEdit
        : this.currentActivePlanTypeName === OdpActivityName.LearningDirection
        ? this.learningDirectionPermission.allowEdit
        : this.keyLearningProgrammePermission.allowEdit)
    );
  }

  get canSubmitContent(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.canSubmit &&
      (this.currentActivePlanTypeName === OdpActivityName.LearningPlan
        ? this.learningPlanPermission.allowSubmit
        : this.learningDirectionPermission.allowSubmit)
    );
  }

  get canApproveRejectContent(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.canApproveReject &&
      (this.currentActivePlanTypeName === OdpActivityName.LearningPlan
        ? this.learningPlanPermission.allowApprove ||
          this.learningPlanPermission.allowReject
        : this.learningDirectionPermission.allowApprove ||
          this.learningDirectionPermission.allowReject)
    );
  }

  get canDeleteDraft(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.canDeleteDraft &&
      this.learningPlanPermission.allowRestore
    );
  }

  get canDeleteAllVersions(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.canDeleteAllVersions &&
      (this.currentActivePlanTypeName === OdpActivityName.LearningPlan
        ? this.learningPlanPermission.allowDelete
        : this.learningDirectionPermission.allowDelete)
    );
  }

  get canDuplicateLearningDirection(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.canDuplicateLearningDirection &&
      this.learningDirectionPermission.allowDuplicate
    );
  }

  get canMassNominate(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.canMassNominate &&
      !this.isCreating &&
      (this.activeParentPdplanDTO &&
      this.activeParentPdplanDTO.assessmentStatusInfo
        ? this.activeParentPdplanDTO.assessmentStatusInfo
            .assessmentStatusCode === IdpStatusCodeEnum.Approved
        : false) &&
      this.keyLearningProgrammePermission.allowCreate
    );
  }

  get disableEditOnMassNominationMode(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.disableEditOnMassNominationMode
    );
  }

  get showOverallViewButton(): boolean {
    return (
      this.planContentComponent &&
      this.planContentComponent.showOverallViewButton &&
      this.learningPlanPermission.allowViewOverall
    );
  }

  private convertDatetimeToISOFormat(pdplan: PDPlanDto): void {
    if (pdplan.pdPlanActivity !== OdpActivity.Plan) {
      return;
    }
    if (pdplan.answer.startDate) {
      pdplan.answer.startDate = DateTimeUtil.surveyToDateLocalTime(
        pdplan.answer.startDate
      ).toISOString();
    }
    if (pdplan.answer.endDate) {
      pdplan.answer.endDate = DateTimeUtil.surveyToDateLocalTime(
        pdplan.answer.endDate
      ).toISOString();
    }
  }

  private convertDatetimeToSurveyFormat(pdplan: PDPlanDto): void {
    if (pdplan.answer.startDate) {
      pdplan.answer.startDate = DateTimeUtil.toSurveyFormat(
        pdplan.answer.startDate
      );
    }
    if (pdplan.answer.endDate) {
      pdplan.answer.endDate = DateTimeUtil.toSurveyFormat(
        pdplan.answer.endDate
      );
    }
  }

  private updateSavedOdp(
    planRespone: PDPlanDto,
    surveyFormJSON: any,
    isCreating: boolean,
    isSilent: boolean
  ): void {
    if (isCreating) {
      if (this.selectedNodeComponent && this.selectedNodeComponent.node) {
        if (!this.selectedNodeComponent.node.children) {
          this.selectedNodeComponent.node.children = [];
        }
        const parentNode = this.selectedNodeComponent.node;
        const newCxNode: CxNode = this.convertNodeInfo(
          planRespone,
          parentNode,
          isSilent
        );
        this.selectedNodeComponent.node.children.push(newCxNode);
      }
    } else {
      this.checkUpdateBreadCrumb(planRespone);
      this.updateCxNodeInfo(planRespone, this.selectedNodeComponent.node);
    }
    if (!isSilent) {
      surveyFormJSON.mode = SurveyjsMode.display;
    }
    this.selectedNodeComponent.refresh();
  }

  private async onApproveOrReject(eventData: any): Promise<void> {
    if (eventData && eventData.pdPlan && eventData.assessmentStatusInfo) {
      this.globalLoader.showLoader();
      const pdPlanDto = eventData.pdPlan;
      const pdPlanId = ResultHelper.getResultId(pdPlanDto);
      const pdEvaluationModel = eventData.pdEvaluationModel;

      const changeStatusResponse = await this.odpService
        .changeStatusPlan(
          pdPlanId,
          new ChangePDPlanStatusDto({
            targetStatusType: eventData.assessmentStatusInfo,
          }),
          pdPlanDto.pdPlanActivity
        )
        .toPromise();

      if (changeStatusResponse && changeStatusResponse.targetStatusType) {
        // Update the existing result/answer (inject the comment) and change the status of the ODP.
        pdPlanDto.assessmentStatusInfo = changeStatusResponse.targetStatusType;
        // Since the API change status doesn't return the last update info,
        // so we need to build it in the client side in order to get the UI updated.
        // TODO: Remove when the API supports responding this info.
        pdPlanDto.lastUpdated = new Date().toISOString();
        pdPlanDto.lastUpdatedBy = {
          identity: this.currentUser.identity,
          email: this.currentUser.emails,
          name: this.currentUser.fullName,
        };

        this.updateComponentWhenChangeStatus(pdPlanDto);
        if (pdEvaluationModel) {
          const planId = pdPlanDto.resultIdentity.extId;
          const eventEntity = this.keyLearningProgramHelper.getCommentEventEntity(
            pdPlanDto
          );
          const result = await this.commentService.saveCommentAsync(
            eventEntity,
            planId,
            pdEvaluationModel
          );
          if (result) {
            this.planContentComponent.updateCommentData();
          }
        }
      } else {
        this.toastrService.warning(
          'Oops, Something went wrong, please try again.'
        );
      }
      this.globalLoader.hideLoader();
    }
  }

  private changeActivePdplan(newActivePdplan: LearningPlanContent): void {
    this.activePdplan = newActivePdplan;
    if (this.planContentComponent) {
      this.planContentComponent.refresh();
    }
  }

  private toCxNode(planNode: PDPlanDto, parent?: CxNode): CxNode {
    const cxNode: CxNode = this.convertNodeInfo(planNode, parent);
    const parentNode =
      planNode.pdPlanActivity === OdpActivity.Direction ? cxNode : undefined;
    if (planNode.children && planNode.children.length > 0) {
      cxNode.children = [];
      planNode.children.forEach((node) => {
        cxNode.children.push(this.toCxNode(node, parentNode));
      });
    }

    return cxNode;
  }

  private convertNodeInfo(
    planNode: PDPlanDto,
    parent?: CxNode,
    isSilent: boolean = false
  ): CxNode {
    const activity = planNode.pdPlanActivity;
    const nodeId = planNode.resultIdentity.extId;
    const nodeName =
      planNode.answer && planNode.answer.Title ? planNode.answer.Title : '';
    const nodeDataObject = new LearningPlanContent({
      pdplanDto: planNode,
      formJSON: this.getFormJSON(activity),
      id: uniqueId(),
    });

    if (isSilent) {
      nodeDataObject.formJSON.mode = this.activePdplan.formJSON.mode;
    }

    const cxNode: CxNode = {
      id: nodeId,
      name: nodeName,
      dataObject: nodeDataObject,
      hideChildren: false,
    };

    if (activity) {
      const statusCode =
        planNode.assessmentStatusInfo &&
        planNode.assessmentStatusInfo.assessmentStatusCode
          ? planNode.assessmentStatusInfo.assessmentStatusCode
          : undefined;
      const cxNodeStatus = this.convertAssessmentStatusToNodeStatus(statusCode);
      const cxNodeStatusText = this.buildNodeStatusText(
        planNode.assessmentStatusInfo
      );
      switch (activity) {
        case OdpActivity.Plan:
          cxNode.status = {
            shortName: 'LP',
            type: cxNodeStatus,
            text: cxNodeStatusText,
          };
          cxNode.iconClass = 'icon-opj-lp';
          break;
        case OdpActivity.Direction:
          cxNode.status = {
            shortName: 'LD',
            type: cxNodeStatus,
            text: cxNodeStatusText,
          };
          cxNode.iconClass = 'icon-opj-ld';

          if (this.hasRightCreateKeyLearningProgram) {
            cxNode.canCreateChildren = true;
            cxNode.createChildrenIcon = 'icon-add';
            cxNode.addNodeText = this.translateService.instant(
              'Odp.Button.AddKLP'
            );
          }
          break;
        case OdpActivity.Programme:
          cxNode.parent = parent ? parent : undefined;
          cxNode.iconClass = 'icon-opj-klp';
          cxNode.subName =
            planNode.answer && planNode.answer.targetAudienceSummary
              ? planNode.answer.targetAudienceSummary
              : undefined;
          break;
        default:
          break;
      }
    }

    return cxNode;
  }

  private convertAssessmentStatusToNodeStatus(
    assessmentStatusCode: any
  ): NodeStatusType {
    switch (assessmentStatusCode) {
      case OdpStatusCode.Started:
        return NodeStatusType.Draft;
      case OdpStatusCode.PendingForApproval:
        return NodeStatusType.Pending;
      case OdpStatusCode.Approved:
        return NodeStatusType.Approved;
      case OdpStatusCode.Rejected:
        return NodeStatusType.Rejected;
      default:
        return;
    }
  }

  private buildNodeStatusText(
    assessmentStatusInfo: AssessmentStatusInfo
  ): string {
    if (assessmentStatusInfo && assessmentStatusInfo.assessmentStatusName) {
      return `${assessmentStatusInfo.assessmentStatusName}`;
    }

    return '';
  }

  private scrollToEndTree(): void {
    const tree = document.getElementById('cxPlanTree');
    if (!tree || !tree.firstChild || !tree.firstChild.lastChild) {
      return;
    }
    const childrenScroll: any = tree.firstChild.lastChild;
    childrenScroll.scrollTop = childrenScroll.scrollHeight;
  }

  private updateCxNodeInfo(planNode: PDPlanDto, cxNode: CxNode): void {
    const activity = planNode.pdPlanActivity;
    cxNode.name =
      planNode.answer && planNode.answer.Title ? planNode.answer.Title : '';
    cxNode.dataObject.pdplanDto = planNode;

    if (activity === OdpActivity.Plan || activity === OdpActivity.Direction) {
      const statusCode =
        planNode.assessmentStatusInfo &&
        planNode.assessmentStatusInfo.assessmentStatusCode
          ? planNode.assessmentStatusInfo.assessmentStatusCode
          : undefined;
      cxNode.status.type = this.convertAssessmentStatusToNodeStatus(statusCode);
      cxNode.status.text = this.buildNodeStatusText(
        planNode.assessmentStatusInfo
      );
    }

    if (activity === OdpActivity.Programme) {
      cxNode.subName =
        planNode.answer && planNode.answer.targetAudienceSummary
          ? planNode.answer.targetAudienceSummary
          : undefined;
    }
  }

  private addItemToBreadcrumb(items: any): void {
    this.currentBreadcrumb = items;
    this.changeDetectorRef.detectChanges();
  }

  private async getODPConfig(): Promise<PDPlanConfig[]> {
    try {
      return await this.odpService.getOdpConfigs().toPromise();
    } catch (error) {
      return;
    }
  }

  private async getLearningPlanResultTree(extID: string): Promise<PDPlanDto[]> {
    try {
      return await this.odpService
        .getLearningPlanResultByExtID(extID)
        .toPromise();
    } catch (error) {
      return;
    }
  }

  private async saveOrCreateLearningPlan(
    pdPlan: PDPlanDto,
    activity: any
  ): Promise<PDPlanDto> {
    try {
      return await this.odpService.savePlan(pdPlan, activity).toPromise();
    } catch (error) {
      return;
    }
  }
  private setSurveyjsDisplayMode(): void {
    this.planFormJSON.mode = SurveyjsMode.display;
    this.directionFormJSON.mode = SurveyjsMode.display;
    this.programmeFormJSON.mode = SurveyjsMode.display;
  }

  private getFormJSON(pdplanType: string): void {
    switch (pdplanType) {
      case OdpActivity.Plan:
        return this.planFormJSON;
      case OdpActivity.Direction:
        return this.directionFormJSON;
      case OdpActivity.Programme:
        return this.programmeFormJSON;
      default:
        break;
    }
  }

  private updateRoute(currentNode: CxNode): void {
    const pdPlanDto: PDPlanDto = this.getPlanDto(currentNode);
    if (!pdPlanDto) {
      return;
    }

    const planExtId = this.getExtId(pdPlanDto);
    if (!planExtId) {
      return;
    }

    const url = this.buildNewRoute(planExtId);

    this.locationService.go(url);
  }

  private buildNewRoute(planExtId: string): string {
    const nodeParamQueryTemplate = '?node=';
    let currentRoute: string = this.router.url;
    const indexOfNodeParam = currentRoute.indexOf(nodeParamQueryTemplate);
    if (indexOfNodeParam > -1) {
      currentRoute = currentRoute.substring(0, indexOfNodeParam);
    }

    return `${currentRoute}${nodeParamQueryTemplate}${planExtId}`;
  }

  private getExtId(pdPlanDto: PDPlanDto): string {
    if (!pdPlanDto || !pdPlanDto.resultIdentity) {
      return;
    }

    return pdPlanDto.resultIdentity.extId;
  }

  private getPlanDto(cxNode: CxNode): PDPlanDto {
    if (!cxNode || !cxNode.dataObject) {
      return;
    }

    return cxNode.dataObject.pdplanDto;
  }

  private getSubmitDto(): ChangePDPlanStatusDto {
    return new ChangePDPlanStatusDto({
      targetStatusType: {
        assessmentStatusCode: IdpStatusCodeEnum.PendingForApproval,
      },
      autoMapTargetStatusType: false,
    });
  }

  private submitPlan(): void {
    if (!this.activePdplan) {
      return;
    }
    if (!ResultHelper.hasValidResultIdentity(this.activePdplan.pdplanDto)) {
      return;
    }

    this.globalLoader.showLoader();
    const plan = this.activePdplan.pdplanDto;
    const planStatusChanged = this.getSubmitDto();
    const planId = ResultHelper.getResultId(plan);
    const planActivity = plan.pdPlanActivity;

    this.odpService
      .changeStatusPlan(planId, planStatusChanged, planActivity)
      .subscribe(
        (responeStatus) => {
          if (responeStatus && responeStatus.targetStatusType) {
            plan.assessmentStatusInfo = responeStatus.targetStatusType;
            // Since the API change status doesn't return the last update info,
            // so we need to build it in the client side in order to get the UI updated.
            // TODO: Remove when the API supports responding this info.
            plan.lastUpdated = new Date().toISOString();
            plan.lastUpdatedBy = {
              identity: this.currentUser.identity,
              email: this.currentUser.emails,
              name: this.currentUser.fullName,
            };
          }
          this.updateComponentWhenChangeStatus(plan);
          this.globalLoader.hideLoader();
        },
        () => {
          this.toastrService.warning('Submit unsuccessfully, please try later');
          this.globalLoader.hideLoader();
        }
      );
  }

  private checkUpdateBreadCrumb(planRespone: PDPlanDto): void {
    if (
      !this.currentBreadcrumb ||
      !planRespone ||
      planRespone.pdPlanActivity !== OdpActivity.Plan ||
      !planRespone.answer
    ) {
      return;
    }
    const breadcrumbPlanIndex = 2;
    const planNameBreadCrumbItem = this.currentBreadcrumb[breadcrumbPlanIndex];
    // tslint:disable-next-line: no-unsafe-any
    const planName: string = planRespone.answer.Title;
    if (planNameBreadCrumbItem && !isEmpty(planName)) {
      planNameBreadCrumbItem.name = planName;
      this.currentBreadcrumb = clone(this.currentBreadcrumb);
    }
  }

  private handleDeletingAllVersions(
    deactivateAssessmentParams: DeactivateAssessmentParams
  ): void {
    const pdplanDto = deactivateAssessmentParams.pdPlan;
    this.globalLoader.showLoader();

    this.odpService.deactivateODP(deactivateAssessmentParams).subscribe(() => {
      this.globalLoader.hideLoader();
      if (pdplanDto.pdPlanActivity === OdpActivity.Plan) {
        this.showDeleteSuccess(
          OdpActivityName.LearningPlan,
          this.selectedNodeComponent.node.name
        );
        this.router.navigate([AppConstant.siteURL.menus.odp]);
      } else {
        // Handle remove learning direction and children KLPs.
        this.showDeleteSuccess(
          OdpActivityName.LearningDirection,
          this.selectedNodeComponent.node.name
        );
        this.planCxNode.children = this.planCxNode.children.filter(
          (child) => child.id !== this.selectedNodeComponent.node.id
        );
        // Refresh the left side.
        this.cxPlanTreeComponent.refresh();

        // Focus to the root node.
        const rootNode = this.cxPlanTreeComponent.findComponentById(
          this.planCxNode.id
        );
        if (rootNode) {
          this.nodeClickedHandler(rootNode);
        }
      }
    }, this.handleError());
  }

  private showDeleteSuccess(
    odpActivityName: OdpActivityName,
    nodeName: string
  ): void {
    this.toastrService.success(`Deleted ${odpActivityName} '${nodeName}'.`);
  }

  private handleDeletingDraftVersion(
    deactivateAssessmentParams: DeactivateAssessmentParams
  ): void {
    const pdplanDto = deactivateAssessmentParams.pdPlan;
    this.globalLoader.showLoader();
    // Call API to deactivate the existing one.
    this.odpService.deactivateODP(deactivateAssessmentParams).subscribe(() => {
      // Call API again in order to get the last submitted version.
      const filterParams = new ODPFilterParams();
      filterParams.pdplanActivities = [pdplanDto.pdPlanActivity];
      filterParams.resultIds = [pdplanDto.previousResultIdentity.id];
      this.odpService.getOdpResults(filterParams).subscribe(
        (odpResults) => {
          if (!odpResults || !odpResults[0]) {
            this.toastrService.error(
              'Something went wrong.\nPlease refresh the page and try again!'
            );
            this.changeDetectorRef.detectChanges();

            return;
          }
          const previousPlan = odpResults[0];
          this.convertDatetimeToSurveyFormat(previousPlan);
          this.updateCxNodeInfo(previousPlan, this.selectedNodeComponent.node);
          this.selectedNodeComponent.refresh();
          this.toastrService.success('Successfully delete the draft version.');
          this.globalLoader.hideLoader();
        },
        () => {
          this.toastrService.error(
            'Something went wrong.\nPlease refresh the page and try again!'
          );
          this.globalLoader.hideLoader();
        }
      );
    }, this.handleError());
  }

  private handleError(): (error: any) => void {
    return () => {
      this.toastrService.error(
        'Something went wrong.\nPlease refresh the page and try again!'
      );
      this.globalLoader.hideLoader();
    };
  }

  private async initOdpForms(
    odpConfigs: PDPlanConfig[],
    planTree: PDPlanDto
  ): Promise<void> {
    this.planFormJSON = odpConfigs.find(
      (config) => config.pdPlanActivity === OdpActivity.Plan
    ).configuration;
    this.directionFormJSON = odpConfigs.find(
      (config) => config.pdPlanActivity === OdpActivity.Direction
    ).configuration;
    await this.initProgrammeFormJSON(planTree, odpConfigs);

    this.setSurveyjsDisplayMode();
  }

  /**
   * Initializes the Key Learning Programme form
   *  which the user groups fields should be reloaded
   *  every time access the learning plan detail.
   * @param rootPDPlanDto The root of the ODP hierarchy which is the learning plan.
   * @param odpConfigs All ODP configurations.
   */
  private async initProgrammeFormJSON(
    rootPDPlanDto: PDPlanDto,
    odpConfigs: PDPlanConfig[]
  ): Promise<void> {
    const departmentId = rootPDPlanDto.objectiveInfo.identity.id;
    const userGroupChoices = await this.buildUserGroupChoices(departmentId);
    const form = odpConfigs.find(
      (config) => config.pdPlanActivity === OdpActivity.Programme
    ).configuration;
    const userGroupQuestionName = 'userGroups';
    this.cxSurveyjsExtendedService.removeSurveyFormProperty(
      form,
      userGroupQuestionName,
      'choicesByUrl'
    );
    CxSurveyJsUtil.addProperty(
      form,
      userGroupQuestionName,
      'choices',
      userGroupChoices
    );
    this.programmeFormJSON = form;
  }

  private async buildUserGroupChoices(departmentId: number): Promise<any[]> {
    return await this.groupStoreService
      .getUserGroupsByDepartmentId(departmentId)
      .pipe(
        map((paging) =>
          paging.items.map((item) => ({
            value: item.identity.id,
            text: item.name,
          }))
        )
      )
      .toPromise();
  }
}
