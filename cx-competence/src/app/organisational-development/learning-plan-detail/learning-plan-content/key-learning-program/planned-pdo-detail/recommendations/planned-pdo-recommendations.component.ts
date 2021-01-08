import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
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
import { Identity } from 'app-models/common.model';
import {
  IKeyLearningProgrammePermission,
  KeyLearningProgrammePermission,
} from 'app-models/common/permission/key-learning-programme-permission';
import {
  AssignedPDOResultModel,
  DepartmentAssignPDOResultModel,
  GroupAssignPDOResultModel,
  LearnerAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import {
  AssignTargetObject,
  PDOpportunityAnswerDTO,
} from 'app-models/mpj/pdo-action-item.model';
import { UserGroupModel } from 'app-models/user-group.model';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { RecommendationService } from 'app-services/idp/assign-pdo/recommendation.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { ResultHelper } from 'app-services/idp/result-helpers';
import {
  PDOpportunityDetailModel,
  PDOpportunityStatusEnum,
} from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { ImageHelpers } from 'app-utilities/image-helpers';
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
  selector: 'planned-pdo-recommendations',
  templateUrl: './planned-pdo-recommendations.component.html',
  styleUrls: ['./planned-pdo-recommendations.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannedPDORecommendationsComponent
  implements AfterViewInit, IKeyLearningProgrammePermission {
  @Input() pdoAnswer: PDOpportunityAnswerDTO;
  @Input() klpDto: IdpDto;
  @Input() pdoDetail: PDOpportunityDetailModel;
  @Input() allowManagePDO: boolean = false;

  // Observable variables
  learnerObs: (searchKey?: string) => Observable<CxSelectItemModel<Staff>[]>;
  userGroupObs: (
    pageIndex?: number,
    pageSize?: number
  ) => Observable<CxSelectItemModel<UserGroupModel>[]>;

  // Selector configs
  learnerSelectorConfig: CxSelectConfigModel;
  userGroupSelectorConfig: CxSelectConfigModel;

  selectedLearnerSelectItems: CxSelectItemModel<Staff>[];
  selectedUserGroupSelectItems: CxSelectItemModel<UserGroupModel>;

  selectedUserGroup: UserGroupModel;

  learnerRecommendationItems: LearnerAssignPDOResultModel[];
  groupRecommendationItems: GroupAssignPDOResultModel[];
  departmentRecommendationItems: DepartmentAssignPDOResultModel[];

  recommendationMode: AssignTargetObject = AssignTargetObject.Learner;

  // Flag variables
  validToRecommend: boolean = false;
  published: boolean;
  loadedData: boolean = false;

  // Secondary variables
  defaultAvatar: string = AppConstant.defaultAvatar;

  // Permission
  currentUser: User;
  keyLearningProgrammePermission: KeyLearningProgrammePermission;

  private courseId: string;
  private klpExtId: string;
  private klpTimestamp: string;
  private departmentId: number;

  constructor(
    protected authService: AuthService,
    private recommendationService: RecommendationService,
    private translateService: TranslateService,
    private changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    private globalLoader: CxGlobalLoaderService,
    private toastrService: ToastrService
  ) {}

  initKeyLearningProgrammePermission(loginUser: User): void {
    this.keyLearningProgrammePermission = new KeyLearningProgrammePermission(
      loginUser
    );
  }

  ngAfterViewInit(): void {
    this.currentUser = this.authService.userData().getValue();
    this.initKeyLearningProgrammePermission(this.currentUser);
    this.initData();
  }

  onClickedRemoveSelectedLearner(learner: CxSelectItemModel<Staff>): void {
    this.removeSelectedLearner(learner);
  }

  onClickedRecommend(): void {
    this.showConfirmRecommendDialog();
  }

  onClickedRemoveRecommendationItem(resultModel: AssignedPDOResultModel): void {
    this.showConfirmRemoveRecommendedResultDialog(resultModel);
  }

  onSelectUserGroup(
    selectedUserGroupItem: CxSelectItemModel<UserGroupModel>
  ): void {
    this.selectedUserGroup = selectedUserGroupItem
      ? selectedUserGroupItem.dataObject
      : undefined;
  }

  onChangeRecommendationMode(change: MatRadioChange): void {
    if (!change) {
      return;
    }
    this.recommendationMode = change.value;
  }

  onClickGroupRecommendItem(resultModel: GroupAssignPDOResultModel): void {
    this.loadMemberForGroup(resultModel);
  }

  onClickDepartmentRecommendItem(
    resultModel: DepartmentAssignPDOResultModel
  ): void {
    this.loadMemberForDepartment(resultModel);
  }

  getAvatar(email: string): string {
    return ImageHelpers.getAvatarFromEmail(email);
  }

  getDateTimeString(date: string): string {
    return DateTimeUtil.toDateString(date, AppConstant.backendDateTimeFormat);
  }

  isAssignedByCurrentKLP(result: AssignedPDOResultModel): boolean {
    return result.createdByResultExtId === this.klpExtId;
  }

  get isValidToRecommend(): boolean {
    switch (this.recommendationMode) {
      case AssignTargetObject.Learner:
        return !isEmpty(this.selectedLearnerSelectItems);
      case AssignTargetObject.Group:
        return this.selectedUserGroup !== undefined;
      case AssignTargetObject.Department:
        return true;
      default:
        break;
    }

    return false;
  }

  get isRecommendationLearnerMode(): boolean {
    return this.recommendationMode === AssignTargetObject.Learner;
  }

  get isRecommendationGroupMode(): boolean {
    return this.recommendationMode === AssignTargetObject.Group;
  }

  get isRecommendationDepartmentMode(): boolean {
    return this.recommendationMode === AssignTargetObject.Department;
  }

  private async initData(): Promise<void> {
    if (!this.processAndValidateData()) {
      console.error('Invalid input data for this component');

      return;
    }

    this.loadedData = false;
    this.globalLoader.showLoader();
    this.selectedLearnerSelectItems = [];
    this.initSelectorConfig();

    this.learnerObs = this.getLearnerObs;
    this.userGroupObs = this.getUserGroupObs;

    await this.getRecommendedItems();
    this.loadedData = true;
    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  private processAndValidateData(): boolean {
    this.klpExtId = ResultHelper.getResultExtId(this.klpDto);
    this.klpTimestamp = this.klpDto && this.klpDto.timestamp;
    this.departmentId = ResultHelper.getObjectiveId(this.klpDto);
    this.courseId = PDPlannerHelpers.getCourseIdFromPDOAnswer(this.pdoAnswer);
    this.published = this.pdoDetail
      ? this.pdoDetail.status === PDOpportunityStatusEnum.Published
      : false;

    return (
      !!this.klpExtId &&
      !!this.klpTimestamp &&
      !!this.departmentId &&
      !!this.courseId
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
      disableItemText: 'Recommended',
    });
    this.userGroupSelectorConfig = new CxSelectConfigModel({
      placeholder: this.translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.UserGroupSelectPlaceholder'
      ),
      searchable: false,
      multiple: false,
      hideSelected: false,
      clearable: true,
      disableItemText: 'Recommended',
    });
  }

  private showConfirmRemoveRecommendedResultDialog(
    resultModel: AssignedPDOResultModel
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
      'Odp.LearningPlan.PlannedPDODetail.RemoveRecommendationResult'
    );
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      this.removeRecommendedResult(resultModel);
      modalRef.close();
    });
  }

  private showConfirmRecommendDialog(): void {
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
    switch (this.recommendationMode) {
      case AssignTargetObject.Learner:
        const selectedLearnerCount = this.selectedLearnerSelectItems.length;
        modalComponent.content =
          selectedLearnerCount +
          ' learner(s) will be recommended for the selected PD opportunity';
        break;
      case AssignTargetObject.Group:
        modalComponent.content =
          'The user group(s) will be recommended for the selected PD opportunity';
        break;
      case AssignTargetObject.Department:
        modalComponent.content =
          'The Organisation Unit will be recommended for the selected PD opportunity';
        break;
      default:
        break;
    }

    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      modalRef.close();
      this.executeRecommend();
    });
  }

  private async executeRecommend(): Promise<void> {
    this.globalLoader.showLoader();
    let targetIdentities: Identity[];
    switch (this.recommendationMode) {
      case AssignTargetObject.Learner:
        targetIdentities = AssignPDOHelper.getLearnerIdentities(
          this.selectedLearnerSelectItems
        );
        break;
      case AssignTargetObject.Group:
        targetIdentities = [this.selectedUserGroup.identity];
        break;
      case AssignTargetObject.Department:
        targetIdentities = [this.klpDto.objectiveInfo.identity];
        break;
      default:
        break;
    }

    const payload = AssignPDOHelper.pdoAssignRecommendationDTOBuilder(
      targetIdentities,
      this.pdoAnswer,
      this.klpExtId,
      this.departmentId
    );

    await this.recommendationService.recommendPDOAsync(payload);

    this.getRecommendedItems();
    this.clearRecommendFormData();
    this.globalLoader.hideLoader();
  }

  private clearRecommendFormData(): void {
    this.selectedLearnerSelectItems = [];
    this.selectedUserGroupSelectItems = undefined;
    this.selectedUserGroup = undefined;
  }

  private removeSelectedLearner(learner: CxSelectItemModel<Staff>): void {
    const index = this.selectedLearnerSelectItems.indexOf(learner);
    this.selectedLearnerSelectItems.splice(index, 1);
    this.selectedLearnerSelectItems = clone(this.selectedLearnerSelectItems);
  }

  private async getRecommendedItems(): Promise<void> {
    const promise1 = this.getRecommendedLearnerResults();
    const promise2 = this.getRecommendedGroupResults();
    const promise3 = this.getRecommendedDepartmentResults();
    await Promise.all([promise1, promise2, promise3]);
    this.changeDetectorRef.detectChanges();
  }

  private async getRecommendedLearnerResults(): Promise<void> {
    const pagingIndividualItems = await this.recommendationService.getIndividualRecommendationAsync(
      this.courseId,
      this.departmentId,
      this.klpTimestamp
    );
    this.learnerRecommendationItems =
      pagingIndividualItems && !isEmpty(pagingIndividualItems.items)
        ? pagingIndividualItems.items
        : [];
  }

  private async getRecommendedGroupResults(): Promise<void> {
    this.groupRecommendationItems = await this.recommendationService.getGroupRecommendationAsync(
      this.courseId,
      this.departmentId,
      this.klpTimestamp
    );
  }

  private async getRecommendedDepartmentResults(): Promise<void> {
    this.departmentRecommendationItems = await this.recommendationService.getDepartmentRecommendationAsync(
      this.courseId,
      this.departmentId,
      this.klpTimestamp
    );
  }

  private async removeRecommendedResult(
    resultModel: AssignedPDOResultModel
  ): Promise<void> {
    if (!resultModel) {
      return;
    }
    this.globalLoader.showLoader();
    const result = await this.recommendationService.removeRecommendedItemAsync(
      resultModel.identity
    );
    this.getRecommendedItems();
    result
      ? this.toastrService.success('Remove success')
      : this.toastrService.error('Remove fail');
    this.globalLoader.hideLoader();
  }

  private getLearnerObs = (
    searchKey: string
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.recommendationService.getLearnersObs(
      searchKey,
      this.departmentId
    );
  };

  private getUserGroupObs = (): Observable<
    CxSelectItemModel<UserGroupModel>[]
  > => {
    return this.recommendationService.getUserGroupObs(this.departmentId);
  };

  private loadMemberForGroup = async (
    resultModel: GroupAssignPDOResultModel
  ): Promise<void> => {
    if (
      !resultModel ||
      (resultModel.pagingMemberAssignedItems &&
        !resultModel.pagingMemberAssignedItems.hasMoreData)
    ) {
      return;
    }

    this.globalLoader.showLoader();
    if (!resultModel.pagingMemberAssignedItems) {
      // Get member for group
      resultModel.pagingMemberAssignedItems = await this.recommendationService.getLearnerRecommendationOfGroupAsync(
        this.courseId,
        this.departmentId,
        resultModel.group.id,
        this.klpTimestamp
      );
    } else {
      // TODO: Get more member for group
    }
    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  };

  private loadMemberForDepartment = async (
    resultModel: DepartmentAssignPDOResultModel
  ): Promise<void> => {
    if (
      !resultModel ||
      (resultModel.pagingMemberAssignedItems &&
        !resultModel.pagingMemberAssignedItems.hasMoreData)
    ) {
      return;
    }

    this.globalLoader.showLoader();
    if (!resultModel.pagingMemberAssignedItems) {
      // Get member for department
      resultModel.pagingMemberAssignedItems = await this.recommendationService.getLearnerRecommendationOfDepartmentAsync(
        this.departmentId,
        this.klpTimestamp,
        this.courseId
      );
    } else {
      // TODO: Get more member for department
    }
    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  };
}
