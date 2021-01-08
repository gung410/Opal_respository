import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
} from '@angular/core';
import { MatRadioChange } from '@angular/material/radio';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
} from '@conexus/cx-angular-common';
import { Dictionary } from '@conexus/cx-angular-common/typings';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { ClassRunModel } from 'app-models/classrun.model';
import { Identity } from 'app-models/common.model';
import {
  IKeyLearningProgrammePermission,
  KeyLearningProgrammePermission,
} from 'app-models/common/permission/key-learning-programme-permission';
import {
  AssignedPDOResultModel,
  AssignPDOpportunityPayload,
  DepartmentAssignPDOResultModel,
  GroupAssignPDOResultModel,
  LearnerAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import {
  AssignTargetObject,
  PDOpportunityAnswerDTO,
  PickApprovingOfficerTarget,
} from 'app-models/mpj/pdo-action-item.model';
import { UserGroupModel } from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { KlpNominationService } from 'app-services/idp/assign-pdo/klp-nomination.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { ResultHelper } from 'app-services/idp/result-helpers';
import {
  PDOpportunityDetailModel,
  PDOpportunityStatusEnum,
} from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { ImageHelpers } from 'app-utilities/image-helpers';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { AppConstant } from 'app/shared/app.constant';
import {
  CxSelectConfigModel,
  CxSelectItemModel,
} from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { clone, isEmpty } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';

@Component({
  selector: 'planned-pdo-nominations',
  templateUrl: './planned-pdo-nominations.component.html',
  styleUrls: ['./planned-pdo-nominations.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannedPDONominationsComponent
  implements OnInit, AfterViewInit, IKeyLearningProgrammePermission {
  @Input() pdoAnswer: PDOpportunityAnswerDTO;
  @Input() klpDto: IdpDto;
  @Input() pdoDetail: PDOpportunityDetailModel;
  @Input() allowManagePDO: boolean = false;

  // Main variables
  courseId: string;

  // Observable variables
  learnerObs: (searchKey?: string) => Observable<CxSelectItemModel<Staff>[]>;
  approvingAdminObs: (
    searchKey?: string
  ) => Observable<CxSelectItemModel<Staff>[]>;
  classRunObs: (
    pageIndex?: number,
    pageSize?: number
  ) => Observable<CxSelectItemModel<ClassRunModel>[]>;
  userGroupObs: (
    pageIndex?: number,
    pageSize?: number
  ) => Observable<CxSelectItemModel<UserGroupModel>[]>;

  // Selector configs
  learnerSelectorConfig: CxSelectConfigModel;
  adminSelectorConfig: CxSelectConfigModel;
  classrunSelectorConfig: CxSelectConfigModel;
  userGroupSelectorConfig: CxSelectConfigModel;

  selectedLearnerSelectItems: CxSelectItemModel<Staff>[];
  selectedClassRunSelectItem: CxSelectItemModel<ClassRunModel>;
  selectedUserGroupSelectItems: CxSelectItemModel<UserGroupModel>;
  selectedApprovingOfficerSelectItem: CxSelectItemModel<Staff>;

  selectedClassRun: ClassRunModel;
  selectedUserGroup: UserGroupModel;

  learnerNominationItems: LearnerAssignPDOResultModel[];
  groupNominationItems: GroupAssignPDOResultModel[];
  departmentNominationItems: DepartmentAssignPDOResultModel[];
  nominationMode: AssignTargetObject = AssignTargetObject.Learner;
  approvingOfficerMode: PickApprovingOfficerTarget =
    PickApprovingOfficerTarget.Admin;

  // Nomination group/department variables
  nominationGroupDataById: Dictionary<{
    status: NominateStatusCodeEnum;
    data: PagingResponseModel<LearnerAssignPDOResultModel>;
  }> = {};

  nominationDepartmentDataById: Dictionary<{
    status: NominateStatusCodeEnum;
    data: PagingResponseModel<LearnerAssignPDOResultModel>;
  }> = {};

  // Flag variables
  validToNominate: boolean = false;
  hasValidClassRunToNominate: boolean;
  published: boolean;
  isExternalPDO: boolean;
  loadedData: boolean = false;
  isELearningPublicCourse: boolean = false;

  // Secondary variables
  defaultAvatar: string = AppConstant.defaultAvatar;

  // helper variables
  nominateStatus: typeof NominateStatusCodeEnum = NominateStatusCodeEnum;

  totalItems: number = 0;
  currentPageIndex: number = 1;
  currentPageSize: number = AppConstant.ItemPerPageOnDialog;
  defaultPageSize: number = AppConstant.ItemPerPageOnDialog;

  // permission
  currentUser: User;
  keyLearningProgrammePermission: KeyLearningProgrammePermission;

  private klpExtId: string;
  private klpTimestamp: string;
  private departmentId: number;

  constructor(
    protected authService: AuthService,
    private pdOpportunityService: PDOpportunityService,
    private klpNominationService: KlpNominationService,
    private translateService: TranslateService,
    private changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    private globalLoader: CxGlobalLoaderService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.userData().getValue();
    this.initKeyLearningProgrammePermission(this.currentUser);
  }

  initKeyLearningProgrammePermission(loginUser: User): void {
    this.keyLearningProgrammePermission = new KeyLearningProgrammePermission(
      loginUser
    );
  }

  ngAfterViewInit(): void {
    this.initData();
  }

  onClickedRemoveSelectedLearner(learner: CxSelectItemModel<Staff>): void {
    this.removeSelectedLearner(learner);
  }

  onClickedNominate(): void {
    this.showConfirmNominateDialog();
  }

  onClickedRemoveNominationItem(
    learnerAssignedItem: AssignedPDOResultModel
  ): void {
    this.showConfirmRemoveNominatedResultDialog(learnerAssignedItem);
  }

  onSelectClassRunChange(
    selectedClassRunItem: CxSelectItemModel<ClassRunModel>
  ): void {
    this.selectedClassRun = selectedClassRunItem
      ? selectedClassRunItem.dataObject
      : undefined;
  }

  onChangeApprovingOfficerMode(change: MatRadioChange): void {
    if (!change) {
      return;
    }
    this.approvingOfficerMode = change.value;
    this.selectedApprovingOfficerSelectItem = undefined;
  }

  onSelectUserGroup(
    selectedUserGroupItem: CxSelectItemModel<UserGroupModel>
  ): void {
    this.selectedUserGroup = selectedUserGroupItem
      ? selectedUserGroupItem.dataObject
      : undefined;
  }

  onChangeNominationMode(change: MatRadioChange): void {
    if (!change) {
      return;
    }
    this.nominationMode = change.value;
  }

  onClickGroupNominateItem(resultModel: GroupAssignPDOResultModel): void {
    let status: NominateStatusCodeEnum;
    const group = resultModel.group;
    if (group.totalNotNominated > 0) {
      status = NominateStatusCodeEnum.NotNominated;
    }
    if (group.totalRejectedLv4 > 0) {
      status = NominateStatusCodeEnum.Rejected4th;
    }
    if (group.totalRejectedLv2 > 0) {
      status = NominateStatusCodeEnum.Rejected2nd;
    }
    if (group.totalRejectedLv1 > 0) {
      status = NominateStatusCodeEnum.Rejected;
    }
    if (group.totalPendingLv3 > 0) {
      status = NominateStatusCodeEnum.PendingForApproval3rd;
    }
    if (group.totalPendingLv2 > 0) {
      status = NominateStatusCodeEnum.PendingForApproval2nd;
    }
    if (group.totalPendingLv1 > 0) {
      status = NominateStatusCodeEnum.PendingForApproval;
    }
    if (group.totalApproved > 0) {
      status = NominateStatusCodeEnum.Approved;
    }

    if (this.isGroupTabChanged(resultModel, status)) {
      this.loadGroupLearner(resultModel, status);
    }

    this.setActiveGroupTab(resultModel, status);
  }

  onClickDepartmentNominateItem(
    resultModel: DepartmentAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): void {
    if (status === null) {
      const group = resultModel.department;
      if (group.totalNotNominated > 0) {
        status = NominateStatusCodeEnum.NotNominated;
      }
      if (group.totalRejectedLv4 > 0) {
        status = NominateStatusCodeEnum.Rejected4th;
      }
      if (group.totalRejectedLv2 > 0) {
        status = NominateStatusCodeEnum.Rejected2nd;
      }
      if (group.totalRejectedLv1 > 0) {
        status = NominateStatusCodeEnum.Rejected;
      }
      if (group.totalPendingLv3 > 0) {
        status = NominateStatusCodeEnum.PendingForApproval3rd;
      }
      if (group.totalPendingLv2 > 0) {
        status = NominateStatusCodeEnum.PendingForApproval2nd;
      }
      if (group.totalPendingLv1 > 0) {
        status = NominateStatusCodeEnum.PendingForApproval;
      }
      if (group.totalApproved > 0) {
        status = NominateStatusCodeEnum.Approved;
      }
    }

    const currentGroupData = this.nominationDepartmentDataById[
      resultModel.department.id + '-' + resultModel.classRun.id
    ];

    if (!currentGroupData || currentGroupData.status !== status) {
      this.loadDepartmentLearner(resultModel, status);
    }

    this.setActiveDepartmentTab(resultModel, NominateStatusCodeEnum.Approved);
  }

  getAvatar(email: string): string {
    return ImageHelpers.getAvatarFromEmail(email);
  }

  getDateTimeString(date: string): string {
    return DateTimeUtil.toDateString(date, AppConstant.backendDateTimeFormat);
  }

  openDetailNominateStatus(
    resultModel: GroupAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): void {
    if (this.isGroupTabChanged(resultModel, status)) {
      this.loadGroupLearner(resultModel, status);
    }

    this.setActiveGroupTab(resultModel, status);
  }

  openDetailNominateDepartmentStatus(
    departmentResultModel: DepartmentAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): void {
    if (this.isDepartmentTabChanged(departmentResultModel, status)) {
      this.loadDepartmentLearner(departmentResultModel, status);
    }

    this.setActiveDepartmentTab(departmentResultModel, status);
  }

  isGroupActiveTab(groupId: string, status: NominateStatusCodeEnum): boolean {
    return (
      (this.nominationGroupDataById[groupId] &&
        this.nominationGroupDataById[groupId].status) === status
    );
  }

  onPagingGroupChanged(
    resultModel: GroupAssignPDOResultModel,
    status: NominateStatusCodeEnum,
    pagingData: { pageIndex: number; pageSize: number }
  ): void {
    this.loadGroupLearner(
      resultModel,
      status,
      pagingData.pageIndex,
      pagingData.pageSize
    );
  }

  onPagingDepartmentChanged(
    resultModel: DepartmentAssignPDOResultModel,
    status: NominateStatusCodeEnum,
    pagingData: { pageIndex: number; pageSize: number }
  ): void {
    this.loadDepartmentLearner(
      resultModel,
      status,
      pagingData.pageIndex,
      pagingData.pageSize
    );
  }

  onCurrentPageChange(pageIndex: number): void {
    this.currentPageIndex = pageIndex;
    this.getNominatedLearnerResults();
  }

  onPageSizeChange(pageSize: number): void {
    this.currentPageIndex = 1;
    this.currentPageSize = +pageSize;
    this.getNominatedLearnerResults();
  }

  async loadGroupLearner(
    resultModel: GroupAssignPDOResultModel,
    status: NominateStatusCodeEnum,
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<void> {
    const activeGroupData = this.nominationGroupDataById[
      resultModel.group.id + '-' + resultModel.classRun.id
    ] || { status, data: null };

    this.globalLoader.showLoader();

    activeGroupData.data = await this.klpNominationService.getLearnerNominationOfGroupAsync(
      this.courseId,
      resultModel.classRun.id,
      this.departmentId,
      resultModel.group.id,
      status,
      this.klpTimestamp,
      pageIndex,
      pageSize,
      this.isExternalPDO
    );

    this.nominationGroupDataById[
      resultModel.group.id + '-' + resultModel.classRun.id
    ] = activeGroupData;

    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  async loadDepartmentLearner(
    resultModel: DepartmentAssignPDOResultModel,
    status: NominateStatusCodeEnum,
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<void> {
    const activeGroupData = this.nominationDepartmentDataById[
      resultModel.department.id + '-' + resultModel.classRun.id
    ] || { status, data: null };

    this.globalLoader.showLoader();

    activeGroupData.data = await this.klpNominationService.getLearnNominationOfDepartmentAsync(
      this.departmentId,
      this.klpTimestamp,
      this.courseId,
      resultModel.classRun.id,
      status,
      pageIndex,
      pageSize,
      this.isExternalPDO
    );

    this.nominationDepartmentDataById[
      resultModel.department.id + '-' + resultModel.classRun.id
    ] = activeGroupData;

    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  get isValidToNominate(): boolean {
    if (!this.selectedClassRun && !this.isExternalPDO) {
      return false;
    }

    switch (this.nominationMode) {
      case AssignTargetObject.Learner:
        return (
          this.selectedLearnerSelectItems &&
          this.selectedLearnerSelectItems.length > 0
        );
      case AssignTargetObject.Group:
        const validPickAdminForGroup =
          this.approvingOfficerMode === PickApprovingOfficerTarget.Admin &&
          !!this.selectedApprovingOfficerSelectItem;
        const validRouteToAOForGroup =
          this.approvingOfficerMode === PickApprovingOfficerTarget.CAO;
        const isValidSeledtedGroup = !isEmpty(this.selectedUserGroup);

        return (
          isValidSeledtedGroup &&
          (validPickAdminForGroup ||
            validRouteToAOForGroup ||
            this.isELearningPublicCourse)
        );
      case AssignTargetObject.Department:
        const validPickAdminForDepartment =
          this.approvingOfficerMode === PickApprovingOfficerTarget.Admin &&
          !!this.selectedApprovingOfficerSelectItem;
        const validRouteToAOForDepartment =
          this.approvingOfficerMode === PickApprovingOfficerTarget.CAO;

        return (
          validPickAdminForDepartment ||
          validRouteToAOForDepartment ||
          this.isELearningPublicCourse
        );
      default:
        break;
    }

    return false;
  }

  get isNoClassRunToNominate(): boolean {
    return (
      !this.hasValidClassRunToNominate && this.allowManagePDO && this.published
    );
  }

  get isNominationLearnerMode(): boolean {
    return this.nominationMode === AssignTargetObject.Learner;
  }

  get isNominationGroupMode(): boolean {
    return this.nominationMode === AssignTargetObject.Group;
  }

  get isNominationDepartmentMode(): boolean {
    return this.nominationMode === AssignTargetObject.Department;
  }

  get isApprovingOfficerMode(): boolean {
    return this.approvingOfficerMode === PickApprovingOfficerTarget.CAO;
  }

  get isApprovingAdminMode(): boolean {
    return this.approvingOfficerMode === PickApprovingOfficerTarget.Admin;
  }

  get showSetApprovingOfficerField(): boolean {
    return (
      !this.isELearningPublicCourse &&
      (this.isNominationGroupMode || this.isNominationDepartmentMode)
    );
  }

  isAssignedByCurrentKLP(result: AssignedPDOResultModel): boolean {
    return result.createdByResultExtId === this.klpExtId;
  }

  private async initData(): Promise<void> {
    if (!this.processAndValidateData()) {
      console.error('Invalid input data for this component');
      this.globalLoader.hideLoader();

      return;
    }
    this.loadedData = false;
    this.globalLoader.showLoader();

    this.hasValidClassRunToNominate = this.isExternalPDO
      ? true
      : await this.pdOpportunityService.checkCourseHaveAnyClassRunAsync(
          this.courseId
        );

    this.selectedLearnerSelectItems = [];
    this.initSelectorConfig();
    this.learnerObs = this.getLearnerObs;
    this.approvingAdminObs = this.getApprovingAdminObs;
    this.classRunObs = this.getClassRunObs;
    this.userGroupObs = this.getUserGroupObs;

    await this.getNominatedItems();
    this.loadedData = true;
    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  private processAndValidateData(): boolean {
    this.klpExtId = ResultHelper.getResultExtId(this.klpDto);
    this.klpTimestamp = this.klpDto && this.klpDto.timestamp;
    this.departmentId = ResultHelper.getObjectiveId(this.klpDto);
    this.courseId = PDPlannerHelpers.getCourseIdFromPDOAnswer(this.pdoAnswer);
    this.isExternalPDO = PDPlannerHelpers.isExternalPDOByAnswer(this.pdoAnswer);
    this.isELearningPublicCourse = AssignPDOHelper.isElearningPublicCourse(
      this.pdoDetail
    );
    this.published = this.isExternalPDO
      ? true
      : this.pdoDetail
      ? this.pdoDetail.status === PDOpportunityStatusEnum.Published
      : false;

    return (
      !!this.klpExtId &&
      !!this.klpTimestamp &&
      !!this.courseId &&
      !!this.departmentId
    );
  }

  private initSelectorConfig(): void {
    this.learnerSelectorConfig = new CxSelectConfigModel({
      placeholder: this.translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.LearnerSelectPlaceholder'
      ),
      searchText: this.translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.LearnerSearchText'
      ),
      disableItemText: 'Nominated',
    });
    this.adminSelectorConfig = new CxSelectConfigModel({
      placeholder: this.translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.AdminSelectPlaceholder'
      ),
      searchable: false,
      multiple: false,
      hideSelected: false,
      clearable: true,
    });
    this.classrunSelectorConfig = new CxSelectConfigModel({
      placeholder: this.translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.ClassRunSelectPlaceholder'
      ),
      searchable: false,
      multiple: false,
      hideSelected: false,
      clearable: true,
    });
    this.userGroupSelectorConfig = new CxSelectConfigModel({
      placeholder: this.translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.UserGroupSelectPlaceholder'
      ),
      searchable: false,
      multiple: false,
      hideSelected: false,
      clearable: true,
      disableItemText: 'Nominated',
    });
  }
  private showConfirmRemoveNominatedResultDialog(
    result: AssignedPDOResultModel
  ): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.Cancel'
    ) as string;
    modalComponent.confirmButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.ConfirmOK'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'Odp.ConfirmationDialog.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'Odp.LearningPlan.PlannedPDODetail.RemoveNominationResult'
    );
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      this.removeNominatedResult(result);
      modalRef.close();
    });
  }

  private showConfirmNominateDialog(): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });
    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.Cancel'
    ) as string;
    modalComponent.confirmButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.ConfirmOK'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'Odp.ConfirmationDialog.Header'
    ) as string;

    switch (this.nominationMode) {
      case AssignTargetObject.Learner:
        const selectedLearnerCount = this.selectedLearnerSelectItems.length;
        modalComponent.content = AssignPDOHelper.getNominateIndividualConfirmationMessage(
          selectedLearnerCount,
          this.isELearningPublicCourse,
          this.translateService
        );
        break;
      case AssignTargetObject.Group:
        modalComponent.content = AssignPDOHelper.getNominateGroupConfirmationMessage(
          this.isELearningPublicCourse,
          this.translateService
        );
        break;
      case AssignTargetObject.Department:
        modalComponent.content = AssignPDOHelper.getNominateDepartmentConfirmationMessage(
          this.isELearningPublicCourse,
          this.translateService
        );
        break;
      default:
        break;
    }

    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      modalRef.close();
      this.executeNominate();
    });
  }

  private async executeNominate(): Promise<void> {
    this.globalLoader.showLoader();

    const classRunExtId = this.isExternalPDO
      ? undefined
      : this.selectedClassRun.id;
    let approvingOfficerExtId: string;
    let targetIdentities: Identity[];
    let payload: AssignPDOpportunityPayload;

    switch (this.nominationMode) {
      case AssignTargetObject.Learner:
        targetIdentities = AssignPDOHelper.getLearnerIdentities(
          this.selectedLearnerSelectItems
        );
        payload = AssignPDOHelper.pdoAssignNominationDTOBuilder(
          targetIdentities,
          this.pdoAnswer,
          this.klpExtId,
          this.departmentId,
          undefined,
          classRunExtId,
          false,
          this.isExternalPDO,
          this.isELearningPublicCourse
        );
        break;

      case AssignTargetObject.Group:
        targetIdentities = [this.selectedUserGroup.identity];
        approvingOfficerExtId = this.isApprovingAdminMode
          ? this.getUserExtId(this.selectedApprovingOfficerSelectItem)
          : undefined;
        const isRouteForIndividualAOForGroup = this.isApprovingOfficerMode;
        payload = AssignPDOHelper.pdoAssignNominationDTOBuilder(
          targetIdentities,
          this.pdoAnswer,
          this.klpExtId,
          this.departmentId,
          approvingOfficerExtId,
          classRunExtId,
          isRouteForIndividualAOForGroup,
          this.isExternalPDO,
          this.isELearningPublicCourse
        );
        break;

      case AssignTargetObject.Department:
        targetIdentities = [this.klpDto.objectiveInfo.identity];
        approvingOfficerExtId = this.isApprovingAdminMode
          ? this.getUserExtId(this.selectedApprovingOfficerSelectItem)
          : undefined;
        const isRouteForIndividualAOForDepartment = this.isApprovingOfficerMode;
        payload = AssignPDOHelper.pdoAssignNominationDTOBuilder(
          targetIdentities,
          this.pdoAnswer,
          this.klpExtId,
          this.departmentId,
          approvingOfficerExtId,
          classRunExtId,
          isRouteForIndividualAOForDepartment,
          this.isExternalPDO,
          this.isELearningPublicCourse
        );
        break;

      default:
        break;
    }

    const isSuccess = await this.klpNominationService.nominatePDOAsync(payload);
    if (isSuccess) {
      this.getNominatedItems();
      this.clearNominateFormData();
    }

    this.globalLoader.hideLoader();
  }

  private clearNominateFormData(): void {
    this.selectedLearnerSelectItems = [];
    this.selectedClassRun = undefined;
    this.selectedUserGroup = undefined;
    this.selectedClassRunSelectItem = undefined;
    this.selectedUserGroupSelectItems = undefined;
    this.selectedApprovingOfficerSelectItem = undefined;
    this.changeDetectorRef.detectChanges();
  }

  private removeSelectedLearner(learner: CxSelectItemModel<Staff>): void {
    const index = this.selectedLearnerSelectItems.indexOf(learner);
    this.selectedLearnerSelectItems.splice(index, 1);
    this.selectedLearnerSelectItems = clone(this.selectedLearnerSelectItems);
  }

  private async getNominatedItems(): Promise<void> {
    const promise1 = this.getNominatedLearnerResults();
    const promise2 = this.getNominatedGroupResults();
    const promise3 = this.getNominatedDepartmentResults();
    await Promise.all([promise1, promise2, promise3]);
    this.changeDetectorRef.detectChanges();
  }

  private async getNominatedLearnerResults(): Promise<void> {
    const pagingIndividualItems = await this.klpNominationService.getIndividualNominationAsync(
      this.courseId,
      this.departmentId,
      this.klpTimestamp,
      this.currentPageIndex,
      this.currentPageSize,
      this.isExternalPDO
    );

    // Check empty and set value
    const individualItemsNotEmpty =
      pagingIndividualItems && !isEmpty(pagingIndividualItems.items);
    this.learnerNominationItems = individualItemsNotEmpty
      ? pagingIndividualItems.items
      : [];
    this.totalItems = individualItemsNotEmpty
      ? pagingIndividualItems.totalItems
      : 0;
    this.changeDetectorRef.detectChanges();
  }

  private async getNominatedGroupResults(): Promise<void> {
    this.groupNominationItems = await this.klpNominationService.getGroupNominationAsync(
      this.courseId,
      this.departmentId,
      this.klpTimestamp
    );
  }

  private async getNominatedDepartmentResults(): Promise<void> {
    this.departmentNominationItems = await this.klpNominationService.getDepartmentNominationAsync(
      this.courseId,
      this.departmentId,
      this.klpTimestamp
    );
  }

  private async removeNominatedResult(
    resultModel: AssignedPDOResultModel
  ): Promise<void> {
    if (!resultModel) {
      return;
    }
    this.globalLoader.showLoader();
    const result = await this.klpNominationService.removeNominatedItemAsync(
      resultModel.identity
    );
    this.getNominatedItems();
    result
      ? this.toastrService.success('Remove success')
      : this.toastrService.error('Remove fail');
    this.globalLoader.hideLoader();
  }

  private getClassRunObs = (): Observable<
    CxSelectItemModel<ClassRunModel>[]
  > => {
    return this.pdOpportunityService.getClassRunByCourseIdObs(this.courseId);
  };

  private getLearnerObs = (
    searchKey: string
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.klpNominationService.getLearnersObs(
      searchKey,
      this.departmentId
    );
  };

  private getApprovingAdminObs = (
    searchKey: string
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.klpNominationService.getAdminObs(searchKey, this.departmentId);
  };

  private getUserGroupObs = (): Observable<
    CxSelectItemModel<UserGroupModel>[]
  > => {
    return this.klpNominationService.getUserGroupObs(this.departmentId);
  };

  private setActiveGroupTab(
    resultModel: GroupAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): void {
    // Update status in order to chanage 'active' class for nominate-tabs.
    const currentGroupData = this.nominationGroupDataById[
      resultModel.group.id + '-' + resultModel.classRun.id
    ] || { status, data: null };
    currentGroupData.status = status;
    this.nominationGroupDataById[
      resultModel.group.id + '-' + resultModel.classRun.id
    ] = currentGroupData;
  }

  private setActiveDepartmentTab(
    resultModel: DepartmentAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): void {
    // Update status in order to chanage 'active' class for nominate-tabs.
    const currentDepartmentData = this.nominationDepartmentDataById[
      resultModel.department.id + '-' + resultModel.classRun.id
    ] || { status, data: null };
    currentDepartmentData.status = status;
    this.nominationGroupDataById[
      resultModel.department.id
    ] = currentDepartmentData;
  }

  private isGroupTabChanged(
    resultModel: GroupAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): boolean {
    const currentGroupData = this.nominationGroupDataById[
      resultModel.group.id + '-' + resultModel.classRun.id
    ];

    // If the group does not exist in the dictionary or the status difference with last value.
    // => it mean user click other tab.
    return !currentGroupData || currentGroupData.status !== status;
  }

  private isDepartmentTabChanged(
    resultModel: DepartmentAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): boolean {
    const currentDepartmentData = this.nominationDepartmentDataById[
      resultModel.department.id + '-' + resultModel.classRun.id
    ];

    // If the department does not exist in the dictionary or the status difference with last value.
    // => it mean user click other tab.
    return !currentDepartmentData || currentDepartmentData.status !== status;
  }

  private getUserExtId(userSelectItem: CxSelectItemModel<Staff>): string {
    if (!userSelectItem || !userSelectItem.dataObject) {
      return;
    }

    return userSelectItem.dataObject.identity.extId;
  }
}
