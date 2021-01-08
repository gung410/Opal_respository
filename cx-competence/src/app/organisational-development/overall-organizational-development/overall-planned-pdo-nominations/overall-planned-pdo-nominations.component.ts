import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
} from '@angular/core';
import { MatRadioChange } from '@angular/material/radio';
import { Dictionary } from '@conexus/cx-angular-common/typings';
import { TranslateService } from '@ngx-translate/core';
import { ClassRunModel } from 'app-models/classrun.model';
import {
  AssignedPDOResultModel,
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
import { isEmpty } from 'lodash';
import { Observable } from 'rxjs';

@Component({
  selector: 'overall-planned-pdo-nominations',
  templateUrl: './overall-planned-pdo-nominations.component.html',
  styleUrls: ['./overall-planned-pdo-nominations.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OverallPlannedPDONominationsComponent implements AfterViewInit {
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

  // Secondary variables
  defaultAvatar: string = AppConstant.defaultAvatar;

  // helper variables
  nominateStatus: typeof NominateStatusCodeEnum = NominateStatusCodeEnum;

  private klpExtId: string;
  private klpTimestamp: string;
  private departmentId: number;

  constructor(
    private pdOpportunityService: PDOpportunityService,
    private klpNominationService: KlpNominationService,
    private translateService: TranslateService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngAfterViewInit(): void {
    this.initData();
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

  async loadGroupLearner(
    resultModel: GroupAssignPDOResultModel,
    status: NominateStatusCodeEnum,
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<void> {
    const activeGroupData = this.nominationGroupDataById[
      resultModel.group.id + '-' + resultModel.classRun.id
    ] || { status, data: null };

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
          (validPickAdminForGroup || validRouteToAOForGroup)
        );
      case AssignTargetObject.Department:
        const validPickAdminForDepartment =
          this.approvingOfficerMode === PickApprovingOfficerTarget.Admin &&
          !!this.selectedApprovingOfficerSelectItem;
        const validRouteToAOForDepartment =
          this.approvingOfficerMode === PickApprovingOfficerTarget.CAO;

        return validPickAdminForDepartment || validRouteToAOForDepartment;
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

  isAssignedByCurrentKLP(result: AssignedPDOResultModel): boolean {
    return result.createdByResultExtId === this.klpExtId;
  }

  private async initData(): Promise<void> {
    if (!this.processAndValidateData()) {
      console.error('Invalid input data for this component');

      return;
    }
    this.loadedData = false;

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
  }

  private processAndValidateData(): boolean {
    this.klpExtId = ResultHelper.getResultExtId(this.klpDto);
    this.klpTimestamp = this.klpDto && this.klpDto.timestamp;
    this.departmentId = ResultHelper.getObjectiveId(this.klpDto);
    this.courseId = PDPlannerHelpers.getCourseIdFromPDOAnswer(this.pdoAnswer);
    this.isExternalPDO = PDPlannerHelpers.isExternalPDOByAnswer(this.pdoAnswer);
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
      undefined,
      undefined,
      this.isExternalPDO
    );

    // Check empty and set value
    const individualItemsNotEmpty =
      pagingIndividualItems && !isEmpty(pagingIndividualItems.items);
    this.learnerNominationItems = individualItemsNotEmpty
      ? pagingIndividualItems.items
      : [];
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
}
