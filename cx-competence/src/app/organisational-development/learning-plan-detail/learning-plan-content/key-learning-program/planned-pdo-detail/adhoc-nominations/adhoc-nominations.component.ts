import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  ViewChild,
} from '@angular/core';
import { MatRadioChange } from '@angular/material/radio';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { ClassRunModel } from 'app-models/classrun.model';
import { Identity } from 'app-models/common.model';
import { IAdhocNominationPermission } from 'app-models/common/permission/adhoc-nomination-permission';
import {
  AssignedPDOResultModel,
  AssignPDOpportunityPayload,
  DepartmentAssignPDOResultModel,
  GroupAssignPDOResultModel,
  LearnerAssignPDOResultModel,
  MassAssignPDOpportunityPayload,
  MassAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import {
  AssignTargetObject,
  PDOpportunityAnswerDTO,
  PickApprovingOfficerTarget,
} from 'app-models/mpj/pdo-action-item.model';
import { MassNominationResultDto } from 'app-models/pdcatalog/pdcatalog.dto';
import { UserGroupModel } from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AdhocNominationService } from 'app-services/idp/assign-pdo/adhoc-nomination.service';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { PdPlannerService } from 'app-services/idp/pd-planner/pd-planner.service';
import { PDOpportunityStatusEnum } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { ImageHelpers } from 'app-utilities/image-helpers';
import { IApprovalUnitModel } from 'app/approval-page/models/class-registration.model';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';
import { AppConstant, Constant } from 'app/shared/app.constant';
import {
  CxSelectConfigModel,
  CxSelectItemModel,
} from 'app/shared/components/cx-select/cx-select.model';
import { AdhocNominationPermission } from 'app/shared/models/common/permission/adhoc-nomination-permission';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { clone, Dictionary, isEmpty } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { MassNominationResultsComponent } from '../adhoc-mass-nomination/mass-nomination-results/mass-nomination-results.component';
import { InvalidNominationRecordDialogComponent } from '../invalid-nomination-record-dialog/invalid-nomination-record-dialog.component';
import { AssignModeEnum } from '../planned-pdo-detail.model';

@Component({
  selector: 'adhoc-nominations',
  templateUrl: './adhoc-nominations.component.html',
  styleUrls: ['./adhoc-nominations.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdhocNominationsComponent
  implements AfterViewInit, IAdhocNominationPermission {
  @ViewChild('massNominationResult')
  massNominationResultComponent: MassNominationResultsComponent;

  assignPDOType: AssignModeEnum = AssignModeEnum.AdhocNominate;
  @Input() allowManagePDO: boolean = true;
  currentUser: User;
  nominationFile: File = undefined;
  // Main variables
  courseId: string;
  selectedPDOAnswer: PDOpportunityAnswerDTO;

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

  massNominationItems: MassAssignPDOResultModel[] = [];
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
  loadedData: boolean = false;
  isELearningPublicCourse: boolean = false;
  showCatalogue: boolean = false;

  // Secondary variables
  defaultAvatar: string = AppConstant.defaultAvatar;
  catalogueTitle: string = 'Select a PD Opportunity';
  selectCourseText: string = 'Select';

  // Permission
  adhocNominationPermission: AdhocNominationPermission;

  private departmentId: number;
  private currentYearTimestamp: string;

  constructor(
    private authService: AuthService,
    private pdOpportunityService: PDOpportunityService,
    private adhocNominationService: AdhocNominationService,
    private translateService: TranslateService,
    private pdPlannerService: PdPlannerService,
    private changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    private globalLoader: CxGlobalLoaderService,
    private toastrService: ToastrService
  ) {}

  initAdhocNominationPermission(loginUser: User): void {
    this.adhocNominationPermission = new AdhocNominationPermission(loginUser);
  }

  ngAfterViewInit(): void {
    this.initData();
  }

  loadReportFileResults(): void {
    this.massNominationResultComponent.loadFileResults();
  }

  onClickedRemoveSelectedLearner(learner: CxSelectItemModel<Staff>): void {
    this.removeSelectedLearner(learner);
  }

  onClickedNominate(): void {
    if (this.nominationMode === AssignTargetObject.MassNomination) {
      this.executeNominate();
    } else {
      this.showConfirmNominateDialog();
    }
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
    this.approvingOfficerMode = change.value as PickApprovingOfficerTarget;
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
    this.nominationMode = change.value as AssignTargetObject;
    this.changeDetectorRef.detectChanges();
  }

  onClickGroupNominateItem(resultModel: GroupAssignPDOResultModel): void {
    const status = this.getDefaultStatusTab(resultModel.group);

    this.loadGroupLearner(resultModel, status);
    this.setActiveGroupTab(resultModel, status);
  }

  onClickMassNominateResultItem(
    resultModel: DepartmentAssignPDOResultModel
  ): void {
    //TODO: download mass nomination result file
  }

  onOpenPDCatalogue(): void {
    this.showCatalogue = true;
  }

  onClickRemovePDO(): void {
    this.clearData();
  }

  getAvatar(email: string): string {
    return ImageHelpers.getAvatarFromEmail(email);
  }

  getDateTimeString(date: string): string {
    return DateTimeUtil.toDateString(date, AppConstant.backendDateTimeFormat);
  }

  handleFileInput(event: any): void {
    // tslint:disable-next-line: no-unsafe-any
    if (!event || !event.target || !event.target.files) {
      return;
    }

    // tslint:disable-next-line: no-unsafe-any
    const fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      this.nominationFile = fileList[0];
    }
  }
  openDetailNominateStatus(
    resultModel: GroupAssignPDOResultModel,
    status: NominateStatusCodeEnum
  ): void {
    this.loadGroupLearner(resultModel, status);
    this.setActiveGroupTab(resultModel, status);
  }

  openDetailNominateDepartmentStatus(
    departmentResultModel: DepartmentAssignPDOResultModel,
    status: NominateStatusCodeEnum = NominateStatusCodeEnum.PendingForApproval
  ): void {
    status = this.getDefaultStatusTab(departmentResultModel.department, status);

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

  onSelectPDOFromCatalogue(opportunity: PDCatalogCourseModel): void {
    this.onSelectPDO(opportunity);
    this.showCatalogue = false;
  }

  onClickBackFromCatalogue(): void {
    this.showCatalogue = false;
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
    activeGroupData.data = await this.adhocNominationService.getLearnerAdhocNominationOfGroupAsync(
      this.departmentId,
      resultModel.group.id,
      this.courseId,
      resultModel.classRun.id,
      status,
      this.currentYearTimestamp,
      pageIndex,
      pageSize
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

    activeGroupData.data = await this.adhocNominationService.getLearnerAdhocNominationOfDepartmentAsync(
      this.departmentId,
      this.courseId,
      resultModel.classRun.id,
      status,
      this.currentYearTimestamp,
      pageIndex,
      pageSize
    );

    this.nominationDepartmentDataById[
      resultModel.department.id + '-' + resultModel.classRun.id
    ] = activeGroupData;

    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  get isValidToNominate(): boolean {
    if (!this.selectedClassRun) {
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

  get allowNominate(): boolean {
    return (
      this.adhocNominationPermission.allowIndividualNominate ||
      this.adhocNominationPermission.allowGroupNominate ||
      this.adhocNominationPermission.allowCurrentOrganisationUnitNominate ||
      this.adhocNominationPermission.allowMassNominate
    );
  }

  get isMassNominationMode(): boolean {
    return this.nominationMode === AssignTargetObject.MassNomination;
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
      (this.isNominationGroupMode ||
        this.isNominationDepartmentMode ||
        (this.isMassNominationMode && this.nominationFile)) &&
      this.selectedPDOAnswer &&
      this.published &&
      this.hasValidClassRunToNominate
    );
  }

  private getDefaultStatusTab(
    group: IApprovalUnitModel,
    status: NominateStatusCodeEnum = null
  ): NominateStatusCodeEnum {
    if (status === null) {
      if (group.totalNotNominated > 0) {
        status = NominateStatusCodeEnum.NotNominated;
      }
      if (group.totalRejectedLv2 > 0) {
        status = NominateStatusCodeEnum.Rejected2nd;
      }
      if (group.totalRejectedLv1 > 0) {
        status = NominateStatusCodeEnum.Rejected;
      }
      if (group.totalRejectedLv4 > 0) {
        status = NominateStatusCodeEnum.Rejected4th;
      }
      if (group.totalPendingLv2 > 0) {
        status = NominateStatusCodeEnum.PendingForApproval2nd;
      }
      if (group.totalPendingLv3 > 0) {
        status = NominateStatusCodeEnum.PendingForApproval3rd;
      }
      if (group.totalPendingLv1 > 0) {
        status = NominateStatusCodeEnum.PendingForApproval;
      }
      if (group.totalApproved > 0) {
        status = NominateStatusCodeEnum.Approved;
      }
    }

    return status;
  }

  private async initData(): Promise<void> {
    this.departmentId = this.authService.userDepartmentId;
    if (!this.departmentId) {
      throw new Error('Cannot get department Id');
    }

    this.loadedData = false;

    // Get current year timestamp
    const currentYear = new Date().getFullYear();
    this.currentYearTimestamp = new Date(
      Date.UTC(currentYear, Constant.MIDDLE_MONTH_OF_YEAR_VALUE)
    ).toISOString();

    this.globalLoader.showLoader();
    this.currentUser = this.authService.userData().getValue();
    this.initAdhocNominationPermission(this.currentUser);
    this.initSelectorConfig();
    this.learnerObs = this.getLearnerObs;
    this.approvingAdminObs = this.getApprovingAdminObs;
    this.classRunObs = this.getClassRunObs;
    this.userGroupObs = this.getUserGroupObs;
    this.loadedData = true;
    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  private initSelectorConfig(): void {
    this.learnerSelectorConfig = new CxSelectConfigModel({
      placeholder:
        '' +
        this.translateService.instant(
          'Odp.LearningPlan.PlannedPDODetail.LearnerSelectPlaceholder'
        ),
      searchText:
        '' +
        this.translateService.instant(
          'Odp.LearningPlan.PlannedPDODetail.LearnerSearchText'
        ),
      disableItemText: 'Nominated',
    });
    this.adminSelectorConfig = new CxSelectConfigModel({
      placeholder:
        '' +
        this.translateService.instant(
          'Odp.LearningPlan.PlannedPDODetail.AdminSelectPlaceholder'
        ),
      searchable: false,
      multiple: false,
      hideSelected: false,
      clearable: true,
    });
    this.classrunSelectorConfig = new CxSelectConfigModel({
      placeholder:
        '' +
        this.translateService.instant(
          'Odp.LearningPlan.PlannedPDODetail.ClassRunSelectPlaceholder'
        ),
      searchable: false,
      multiple: false,
      hideSelected: false,
      clearable: true,
    });
    this.userGroupSelectorConfig = new CxSelectConfigModel({
      placeholder:
        '' +
        this.translateService.instant(
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
    modalComponent.content =
      '' +
      this.translateService.instant(
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
    if (this.nominationMode === AssignTargetObject.MassNomination) {
      return this.MassNominate();
    }

    const classRunExtId = this.selectedClassRun.id;
    let approvingOfficerExtId: string;
    let targetIdentities: Identity[];
    let payload: AssignPDOpportunityPayload;
    switch (this.nominationMode) {
      case AssignTargetObject.Learner:
        targetIdentities = AssignPDOHelper.getLearnerIdentities(
          this.selectedLearnerSelectItems
        );
        payload = AssignPDOHelper.pdoAssignAdhocNominationDTOBuilder(
          targetIdentities,
          this.selectedPDOAnswer,
          this.departmentId,
          undefined,
          classRunExtId,
          false,
          this.isELearningPublicCourse
        );
        break;
      case AssignTargetObject.Group:
        targetIdentities = [this.selectedUserGroup.identity];
        approvingOfficerExtId = this.isApprovingAdminMode
          ? this.getUserExtId(this.selectedApprovingOfficerSelectItem)
          : undefined;
        const isRouteForIndividualAOForGroup = this.isApprovingOfficerMode;
        payload = AssignPDOHelper.pdoAssignAdhocNominationDTOBuilder(
          targetIdentities,
          this.selectedPDOAnswer,
          this.departmentId,
          approvingOfficerExtId,
          classRunExtId,
          isRouteForIndividualAOForGroup,
          this.isELearningPublicCourse
        );
        break;
      case AssignTargetObject.Department:
        targetIdentities = [this.getNewDepartmentIdentity(this.departmentId)];
        approvingOfficerExtId = this.isApprovingAdminMode
          ? this.getUserExtId(this.selectedApprovingOfficerSelectItem)
          : undefined;
        const isRouteForIndividualAOForDepartment = this.isApprovingOfficerMode;
        payload = AssignPDOHelper.pdoAssignAdhocNominationDTOBuilder(
          targetIdentities,
          this.selectedPDOAnswer,
          this.departmentId,
          approvingOfficerExtId,
          classRunExtId,
          isRouteForIndividualAOForDepartment,
          this.isELearningPublicCourse
        );
        break;
      default:
        break;
    }

    const isSuccess = await this.adhocNominationService.adhocNominatePDOAsync(
      payload
    );
    if (isSuccess) {
      this.getNominatedItems();
      this.clearNominateFormData();
    }

    this.globalLoader.hideLoader();
  }

  private async MassNominate(): Promise<void> {
    this.toastrService.warning('Feature disable for now');
    // TODO: Update for new code flow
    // this.globalLoader.showLoader();
    // const massAssignPDOPayload = this.buildMassAssignPDOPayload();
    // const massNominationFileResult = await this.aDdhocNominationService.validateAdhocMassNominationFileAsync(
    //   massAssignPDOPayload
    // );
    // this.globalLoader.hideLoader();
    // this.clearNominateFormData();
    // if (massNominationFileResult) {
    //   this.showNominationWarning(massNominationFileResult);
    // } else {
    //   this.showAsyncNominationProccessingMessage();
    //   await this.adhocNominationService.adhocMassNominatePDOAsync(
    //     massAssignPDOPayload
    //   );
    // }
  }

  private showNominationWarning(
    invalidNominatingResults: MassNominationResultDto[]
  ): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Action.Close'
    ) as string;
    modalComponent.confirmButtonText = this.translateService.instant(
      'MassNomination.Warning.ConfirmButton'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'MassNomination.Warning.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'MassNomination.Warning.Content'
    ) as string;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      this.showInvalidNominationRecords(invalidNominatingResults);
      modalRef.close();
    });
  }

  private showInvalidNominationRecords(
    invalidNominatingResults: MassNominationResultDto[]
  ): void {
    const modalRef = this.ngbModal.open(
      InvalidNominationRecordDialogComponent,
      {
        size: 'lg',
        centered: true,
      }
    );
    const modalComponent = modalRef.componentInstance as InvalidNominationRecordDialogComponent;
    modalComponent.invalidNominatingResults = invalidNominatingResults;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
  }

  private showAsyncNominationProccessingMessage(): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Action.Close'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'MassNomination.AsyncProcessing.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'MassNomination.AsyncProcessing.Content'
    ) as string;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      modalRef.close();
    });
  }

  private buildMassAssignPDOPayload(): MassAssignPDOpportunityPayload {
    const approvingOfficerExtId = this.isApprovingOfficerMode
      ? this.getUserExtId(this.selectedApprovingOfficerSelectItem)
      : undefined;
    const isRouteForIndividualAOForGroup = this.isApprovingAdminMode;

    return new MassAssignPDOpportunityPayload({
      file: this.nominationFile,
      keyLearningProgramExtId: undefined,
      departmentId: this.departmentId,
      proceedAssign: true,
      nominationApprovingOfficerExtId: approvingOfficerExtId,
      isRouteIndividualLearnerAOForApproval: isRouteForIndividualAOForGroup,
      isAdhoc: true,
    });
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
    const promise4 = this.getNominatedFileResults();
    await Promise.all([promise1, promise2, promise3, promise4]);
    this.changeDetectorRef.detectChanges();
  }

  private async getNominatedFileResults(): Promise<void> {
    // TODO: Fix bug get mass nomination item
    // const pagingMasssNominationItems = await this.adhocNominationService.getAdhocMassNominationAssignedPDOAsync(
    //   this.departmentId
    // );
    // // Check empty and set value
    // const massNominationItemsNotEmpty =
    //   pagingMasssNominationItems && !isEmpty(pagingMasssNominationItems.items);
    // this.massNominationItems = massNominationItemsNotEmpty
    //   ? pagingMasssNominationItems.items
    //   : [];
  }

  private async getNominatedLearnerResults(): Promise<void> {
    const pagingIndividualItems = await this.adhocNominationService.getIndividualAdhocNominationAsync(
      this.departmentId,
      this.courseId,
      this.currentYearTimestamp
    );

    // Check empty and set value
    const individualItemsNotEmpty =
      pagingIndividualItems && !isEmpty(pagingIndividualItems.items);
    this.learnerNominationItems = individualItemsNotEmpty
      ? pagingIndividualItems.items
      : [];
  }

  private async getNominatedGroupResults(): Promise<void> {
    this.groupNominationItems = await this.adhocNominationService.getGroupAdhocNominationAsync(
      this.departmentId,
      this.courseId,
      this.currentYearTimestamp
    );
  }

  private async getNominatedDepartmentResults(): Promise<void> {
    this.departmentNominationItems = await this.adhocNominationService.getDepartmentAdhocNominationAsync(
      this.departmentId,
      this.courseId,
      this.currentYearTimestamp
    );
  }

  private async removeNominatedResult(
    resultModel: AssignedPDOResultModel
  ): Promise<void> {
    if (!resultModel) {
      return;
    }
    this.globalLoader.showLoader();
    const result = await this.adhocNominationService.removeAdhocNominatedItemAsync(
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
    return this.adhocNominationService.getAvailableLearnersObs(
      searchKey,
      this.departmentId
    );
  };

  private getApprovingAdminObs = (
    searchKey: string
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.adhocNominationService.getAdminObs(
      searchKey,
      this.departmentId
    );
  };

  private getUserGroupObs = (): Observable<
    CxSelectItemModel<UserGroupModel>[]
  > => {
    return this.adhocNominationService.getUserGroupObs(this.departmentId);
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
      resultModel.department.id + '-' + resultModel.classRun.id
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

  private onSelectPDO = async (opportunity: PDCatalogCourseModel) => {
    if (opportunity && opportunity.course) {
      this.clearData();
      this.selectedPDOAnswer = PDPlannerHelpers.toPDOpportunityAnswerDTO(
        opportunity
      );
      this.selectedPDOAnswer.learningOpportunity = PDPlannerHelpers.updateCoursePadPDOInfo(
        opportunity.course,
        this.selectedPDOAnswer
      );
      this.courseId = opportunity.course.id;
      this.published =
        opportunity.course.status === PDOpportunityStatusEnum.Published;
      this.hasValidClassRunToNominate = await this.pdOpportunityService.checkCourseHaveAnyClassRunAsync(
        opportunity.course.id
      );
      this.isELearningPublicCourse = AssignPDOHelper.isElearningPublicCourse(
        opportunity.course
      );
      await this.getNominatedItems();
      this.changeDetectorRef.detectChanges();
    }
  };

  private getNewDepartmentIdentity(id: number): Identity {
    return {
      id,
      ownerId: 3001,
      customerId: 2052,
      archetype: 'OrganizationalUnit',
    };
  }

  private clearData(): void {
    this.selectedPDOAnswer = undefined;
    this.published = undefined;
    this.courseId = undefined;
    this.hasValidClassRunToNominate = undefined;
    this.learnerNominationItems = undefined;
    this.groupNominationItems = undefined;
    this.departmentNominationItems = undefined;
    this.validToNominate = undefined;
    this.selectedLearnerSelectItems = undefined;
    this.selectedClassRunSelectItem = undefined;
    this.selectedUserGroupSelectItems = undefined;
    this.selectedApprovingOfficerSelectItem = undefined;
    this.selectedClassRun = undefined;
    this.selectedUserGroup = undefined;
  }
}
