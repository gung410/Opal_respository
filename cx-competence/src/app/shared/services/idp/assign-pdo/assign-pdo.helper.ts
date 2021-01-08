import { TranslateService } from '@ngx-translate/core';
import { ClassRunDTO, ClassRunModel } from 'app-models/classrun.model';
import { Identity } from 'app-models/common.model';
import { LearnerInfoDTO } from 'app-models/common/learner-info.model';
import {
  AssignedLearnerResult,
  AssignPDOpportunityPayload,
  AssignPDOpportunityResponse,
  DepartmentAssignPDOResultModel,
  GroupAssignPDOResultModel,
  LearnerAssignPDOResultModel,
  MassAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import {
  AssignedPDOResultDTO,
  MassAssignedPDOResultDTO,
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';
import { UserGroupDTO, UserGroupModel } from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import {
  CourseRegistrationMethod,
  PDOpportunityDetailModel,
} from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { ImageHelpers } from 'app-utilities/image-helpers';
import { ApprovalLearnerModel } from 'app/approval-page/models/class-registration.model';
import { LearningNeedGridRowModel } from 'app/approval-page/models/learning-need-grid-row.model';
import { PdPlanGridRowModel } from 'app/approval-page/models/pd-plan-grid-row.model';
import { AssignResultModel } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/assign-pdo-result-dialog/assign-pdo-result-dialog.model';
import { AssignModeEnum } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/planned-pdo-detail.model';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { Constant } from 'app/shared/app.constant';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import {
  PagedStaffsList,
  Staff,
} from 'app/staff/staff.container/staff-list/models/staff.model';
import { isEmpty } from 'lodash';
import { PDPlannerHelpers } from '../pd-planner/pd-planner-helpers';

export class AssignPDOHelper {
  static readonly assignedResultMessageMapping: any = {
    // Genaral
    NO_VALID_LEARNER_TO_NOMINATE:
      'Nomination for class run has not been completed. There are no valid learners to nominate.',
    NO_VALID_LEARNER_TO_RECOMMEND:
      'Recommendation for PD Opportunity has not been completed. There no valid learners to recommend',
    INVALID_TOTAL_MEMBER:
      'Nominate classrun has not been completed, only support nominate equal or less than 1000 learners at a time',
    INVALID_COURSE_UNPUBLISHED:
      'Request has not been completed, course has been unpublished',
    INVALID_CLASSRUN_UNPUBLISHED:
      'Request has not been completed, class run has been unpublished',
    // Individual
    NOT_FOUND_ANY_AO: 'No approving officer, please assign AO to proceed',
    INACTIVE_LEARNER: 'Learner is not active',
    NOMINATED_LEARNER: 'Learner has been nominated to this course',
    NOMINATED_LEARNER_ON_CAM: 'Learner has been nominated to this course',
    RECOMMENDED_LEARNER:
      'Learner has been nominated/recommended/self registered to this course',
    MAX_RELEARNING_TIMES: 'Learner has reached maximum learning times',
    COURSE_UNPUBLISHED: 'Course has been unpublished',
    CLASSRUN_UNPUBLISHED: 'Class run has been unpublished',
    CLASSRUN_APPLICATION_DATE_EXPIRED:
      'Application date of class run has expired',
    CLASSRUN_APPLICATION_START_DATE_NOT_REACH:
      'Application date of class run is not met',

    // Group
    ALREADY_NOMINATED_GROUP:
      'Nominate classrun has not been completed, this group already nominated',
    ALREADY_RECOMMENDED_GROUP:
      'Recommend PD Opportunity has not been completely, this group already has this PD Opportunity',

    // Department
    ALREADY_NOMIANTED_DEPARTMENT:
      'Nominate classrun has not been completed, this department already nominated',
    ALREADY_RECOMMENDED_DEPARTMENT:
      'Recommend PD Opportunity has not been completely, this department has this PD Opportunity',
  };

  static filterParamLearnerBuilder(
    searchKey: string,
    departmentId: number,
    pageIndex: number = 1
  ): FilterParamModel {
    const pageSize = 10;
    const filterParams = new FilterParamModel();
    filterParams.pageSize = pageSize;
    filterParams.pageIndex = pageIndex;
    filterParams.idpEmployeeSearchKey = searchKey || undefined;
    filterParams.departmentIds =
      departmentId !== undefined ? [departmentId] : undefined;
    filterParams.entityStatuses = [
      StatusTypeEnum.Active.code,
      StatusTypeEnum.Inactive.code,
      StatusTypeEnum.New.code,
      StatusTypeEnum.IdentityServerLocked.code,
      StatusTypeEnum.PendingApproval1st.code,
      StatusTypeEnum.PendingApproval2nd.code,
      StatusTypeEnum.PendingApproval3rd.code,
    ];
    filterParams.includeFilterOptions = {
      statusTypeInfo: true,
      userGroupInfo: true,
      approvalGroupInfo: true,
    };

    return filterParams;
  }

  static filterParamApprovingOfficerBuilder(
    searchKey: string,
    departmentId: number
  ): FilterParamModel {
    const pageSize = 10;
    const courseApprovingOfficerId = 98;
    const filterParams = new FilterParamModel();
    filterParams.pageSize = pageSize;
    filterParams.pageIndex = 1;
    filterParams.idpEmployeeSearchKey = searchKey || undefined;
    filterParams.departmentIds = [departmentId];
    filterParams.multiUserTypeIds[0] = [courseApprovingOfficerId];
    filterParams.entityStatuses = [
      StatusTypeEnum.Active.code,
      StatusTypeEnum.New.code,
    ];

    return filterParams;
  }

  static filterParamAdminBuilder(
    searchKey: string,
    departmentId: number
  ): FilterParamModel {
    const pageSize = 10;
    const schoolAdminId = 89;
    const branchAdminId = 88;
    const divisonAdminId = 87;
    const filterParams = new FilterParamModel();
    filterParams.pageSize = 0;
    filterParams.pageIndex = 0;
    filterParams.idpEmployeeSearchKey = searchKey || undefined;
    filterParams.departmentIds = [departmentId];
    filterParams.multiUserTypeIds[0] = [
      schoolAdminId,
      divisonAdminId,
      branchAdminId,
    ];
    filterParams.entityStatuses = [
      StatusTypeEnum.Active.code,
      StatusTypeEnum.New.code,
    ];

    return filterParams;
  }

  static getLearnerIdentities(
    learners: CxSelectItemModel<Staff>[]
  ): Identity[] {
    if (isEmpty(learners)) {
      return [];
    }

    const learnerIdentities = learners
      .filter((learner) => learner.dataObject && learner.dataObject.identity)
      .map((learner) => learner.dataObject.identity);

    return learnerIdentities;
  }

  static mapPagedStaffsToCxSelectItems = (
    pagedStaffsList: PagedStaffsList
  ): CxSelectItemModel<Staff>[] => {
    if (!pagedStaffsList || isEmpty(pagedStaffsList.items)) {
      return [];
    }

    const staffs = pagedStaffsList.items;
    const cxSelectItems = staffs.map(
      (staff) =>
        new CxSelectItemModel({
          id: staff.identity.id.toString(),
          avatar: ImageHelpers.getAvatarFromEmail(staff.email),
          dataObject: staff,
          primaryField: staff.fullName,
          secondaryField: staff.email,
        })
    );

    return cxSelectItems;
  };

  static mapPagedClassRunsToCxSelectItems = (
    pagedClassRunDTOs: PagingResponseModel<ClassRunDTO>
  ): CxSelectItemModel<ClassRunModel>[] => {
    if (!pagedClassRunDTOs || isEmpty(pagedClassRunDTOs.items)) {
      return [];
    }

    return pagedClassRunDTOs.items.map(
      AssignPDOHelper.classRunDTOToCxSelectItem
    );
  };

  static mapPagedUserGroupDTOToCxSelectItems = (
    pagedUserGroupDTOs: PagingResponseModel<UserGroupDTO>
  ): CxSelectItemModel<UserGroupModel>[] => {
    if (!pagedUserGroupDTOs || isEmpty(pagedUserGroupDTOs.items)) {
      return [];
    }

    const userGroupDTOs = pagedUserGroupDTOs.items;
    const userGroups = userGroupDTOs.map((dto) => new UserGroupModel(dto));
    const cxSelectItems = userGroups.map(
      (userGroup) =>
        new CxSelectItemModel({
          id: userGroup.identity.id.toString(),
          dataObject: userGroup,
          primaryField: userGroup.name,
          secondaryField:
            (userGroup.memberCount > 0 ? userGroup.memberCount : '0') +
            ' members',
        })
    );

    return cxSelectItems;
  };

  static pdoAssignRecommendationDTOBuilder(
    identities: Identity[],
    pdoAnswer: PDOpportunityAnswerDTO,
    resultExtId: string,
    departmentId: number
  ): AssignPDOpportunityPayload {
    if (isEmpty(identities) || !pdoAnswer) {
      return;
    }

    const courseId = PDPlannerHelpers.getCourseIdFromURI(
      pdoAnswer.learningOpportunity.uri
    );
    const answer = AssignPDOHelper.pdoAnswerBuilder(pdoAnswer);

    return new AssignPDOpportunityPayload({
      identities,
      answer,
      courseId,
      departmentId,
      klpExtId: resultExtId,
    });
  }

  static pdoAssignNominationDTOBuilder(
    identities: Identity[],
    pdoAnswer: PDOpportunityAnswerDTO,
    klpExtId: string,
    departmentId: number,
    nominationApprovingOfficerExtId: string,
    classRunId: string,
    isRouteIndividualLearnerAOForApproval: boolean = false,
    isExternalPDO: boolean = false,
    isELearningPublicCourse: boolean = false
  ): AssignPDOpportunityPayload {
    if (isEmpty(identities) || !pdoAnswer) {
      return;
    }

    const courseId = PDPlannerHelpers.getCourseIdFromURI(
      pdoAnswer.learningOpportunity.uri
    );
    const answer = AssignPDOHelper.pdoAnswerBuilder(pdoAnswer, classRunId);

    return new AssignPDOpportunityPayload({
      identities,
      answer,
      courseId,
      departmentId,
      classRunId,
      klpExtId,
      nominationApprovingOfficerExtId,
      isRouteIndividualLearnerAOForApproval,
      isExternalPDO,
      isELearningPublicCourse,
    });
  }

  static pdoAssignAdhocNominationDTOBuilder(
    identities: Identity[],
    pdoAnswer: PDOpportunityAnswerDTO,
    departmentId: number,
    nominationApprovingOfficerExtId: string,
    classRunId: string,
    isRouteIndividualLearnerAOForApproval: boolean = false,
    isELearningPublicCourse: boolean = false
  ): AssignPDOpportunityPayload {
    if (isEmpty(identities) || !pdoAnswer) {
      return;
    }

    const courseId = PDPlannerHelpers.getCourseIdFromURI(
      pdoAnswer.learningOpportunity.uri
    );
    const answer = AssignPDOHelper.pdoAnswerBuilder(pdoAnswer, classRunId);

    return new AssignPDOpportunityPayload({
      identities,
      answer,
      courseId,
      departmentId,
      classRunId,
      klpExtId: null,
      nominationApprovingOfficerExtId,
      isRouteIndividualLearnerAOForApproval,
      isELearningPublicCourse,
    });
  }

  static pdoAnswerBuilder(
    pdoAnswer: PDOpportunityAnswerDTO,
    classRunId?: string
  ): PDOpportunityAnswerDTO {
    const pdoDTO = pdoAnswer.learningOpportunity;

    const learningOpportunity = AssignPDOHelper.pdoDTOBuilder(pdoDTO);

    const answer: PDOpportunityAnswerDTO = {
      learningOpportunity,
      classRunId,
    };

    return answer;
  }

  static pdoDTOBuilder(pdoDTO: PDOpportunityDTO): PDOpportunityDTO {
    if (pdoDTO.source === PDOSource.CoursePadPDO) {
      return {
        uri: pdoDTO.uri,
        source: PDOSource.CoursePadPDO,
      };
    }

    return pdoDTO;
  }

  static filterValidClassRun(classRunDTO: ClassRunDTO): boolean {
    if (!classRunDTO || !classRunDTO.applicationEndDate) {
      return false;
    }
    const currentEndDate = new Date(classRunDTO.applicationEndDate);
    const currentDate = new Date();

    return currentEndDate >= currentDate;
  }

  static classRunDTOToCxSelectItem(
    classRunDTO: ClassRunDTO
  ): CxSelectItemModel<ClassRunModel> {
    const classRun = new ClassRunModel(classRunDTO);

    return new CxSelectItemModel({
      id: classRun.id,
      dataObject: classRun,
      primaryField: classRun.classTitle,
      secondaryField: classRun.code,
    });
  }

  static toPagingLearningNeedRequest(
    idpDtoResults: IdpDto[]
  ): PagingResponseModel<LearningNeedGridRowModel> {
    if (!idpDtoResults) {
      return;
    }

    // TODO: Remove fake PagingResponseModel<IdpDto> model
    return new PagingResponseModel<LearningNeedGridRowModel>({
      totalItems: idpDtoResults.length,
      pageIndex: 1,
      pageSize: idpDtoResults.length,
      items: AssignPDOHelper.toLearningNeedModels(idpDtoResults),
      hasMoreData: false,
    });
  }

  static toPagingPdPlanRequest(
    idpDtoResults: IdpDto[]
  ): PagingResponseModel<PdPlanGridRowModel> {
    if (!idpDtoResults) {
      return;
    }

    // TODO: Remove fake PagingResponseModel<IdpDto> model
    return new PagingResponseModel<PdPlanGridRowModel>({
      totalItems: idpDtoResults.length,
      pageIndex: 1,
      pageSize: idpDtoResults.length,
      items: AssignPDOHelper.toPdPlanRowModels(idpDtoResults),
      hasMoreData: false,
    });
  }

  static toPagingNominateLearnerRequest(
    pagingNominateDTO: PagingResponseModel<AssignedPDOResultDTO>
  ): PagingResponseModel<LearnerAssignPDOResultModel> {
    if (!pagingNominateDTO) {
      return;
    }

    return new PagingResponseModel<LearnerAssignPDOResultModel>({
      totalItems: pagingNominateDTO.totalItems,
      pageIndex: pagingNominateDTO.pageIndex,
      pageSize: pagingNominateDTO.pageSize,
      items: AssignPDOHelper.toNominateLearnerModels(pagingNominateDTO.items),
      hasMoreData: pagingNominateDTO.hasMoreData,
    });
  }

  static toPagingNominateGroupRequest(
    pagingNominateDTO: PagingResponseModel<AssignedPDOResultDTO>
  ): PagingResponseModel<GroupAssignPDOResultModel> {
    if (!pagingNominateDTO) {
      return;
    }

    return new PagingResponseModel<GroupAssignPDOResultModel>({
      totalItems: pagingNominateDTO.totalItems,
      pageIndex: pagingNominateDTO.pageIndex,
      pageSize: pagingNominateDTO.pageSize,
      items: AssignPDOHelper.toNominateGroupModels(pagingNominateDTO.items),
      hasMoreData: pagingNominateDTO.hasMoreData,
    });
  }

  static toPagingNominateDepartmentRequest(
    pagingNominateDTO: PagingResponseModel<AssignedPDOResultDTO>
  ): PagingResponseModel<DepartmentAssignPDOResultModel> {
    if (!pagingNominateDTO) {
      return;
    }

    return new PagingResponseModel<DepartmentAssignPDOResultModel>({
      totalItems: pagingNominateDTO.totalItems,
      pageIndex: pagingNominateDTO.pageIndex,
      pageSize: pagingNominateDTO.pageSize,
      items: AssignPDOHelper.toNominateDepartmentModels(
        pagingNominateDTO.items
      ),
      hasMoreData: pagingNominateDTO.hasMoreData,
    });
  }

  static toPagingMassNominationRequest(
    pagingNominateDTO: PagingResponseModel<MassAssignedPDOResultDTO>
  ): PagingResponseModel<MassAssignPDOResultModel> {
    if (!pagingNominateDTO) {
      return;
    }

    return new PagingResponseModel<MassAssignPDOResultModel>({
      totalItems: pagingNominateDTO.totalItems,
      pageIndex: pagingNominateDTO.pageIndex,
      pageSize: pagingNominateDTO.pageSize,
      items: AssignPDOHelper.toMassNominationModels(pagingNominateDTO.items),
      hasMoreData: pagingNominateDTO.hasMoreData,
    });
  }
  static toNominateLearnerModels(
    nominateRequestDTOs: AssignedPDOResultDTO[]
  ): LearnerAssignPDOResultModel[] {
    if (isEmpty(nominateRequestDTOs)) {
      return [];
    }

    return nominateRequestDTOs.map(
      (dto) => new LearnerAssignPDOResultModel(dto)
    );
  }
  static toLearningNeedModels(idpDtos: IdpDto[]): LearningNeedGridRowModel[] {
    if (isEmpty(idpDtos)) {
      return [];
    }

    return idpDtos.map((dto) => {
      return {
        id: dto.resultIdentity ? dto.resultIdentity.id : null,
        identity: dto.resultIdentity ? dto.resultIdentity : {},
        learner: {
          id: dto.objectiveInfo.identity.id,
          userId: dto.objectiveInfo.identity.id,
          email: dto.objectiveInfo.email,
          avatar: '',
          name: dto.objectiveInfo.name,
        } as ApprovalLearnerModel,
        LNAStatus: {
          userId: dto.objectiveInfo.identity.id,
          assessmentInfo: {
            dueDate: dto.dueDate ? new Date(dto.dueDate) : null,
            identity: dto.resultIdentity,
            statusInfo: dto.assessmentStatusInfo,
          },
        },
        learnerDetailUrl: `/employee/detail/${dto.objectiveInfo.identity.id}`,
      } as LearningNeedGridRowModel;
    });
  }

  static toPdPlanRowModels(idpDtos: IdpDto[]): PdPlanGridRowModel[] {
    if (isEmpty(idpDtos)) {
      return [];
    }

    return idpDtos.map((dto) => {
      return {
        id: dto.resultIdentity ? dto.resultIdentity.id : null,
        identity: dto.resultIdentity ? dto.resultIdentity : {},
        learner: {
          id: dto.objectiveInfo.identity.id,
          userId: dto.objectiveInfo.identity.id,
          email: dto.objectiveInfo.email,
          avatar: '',
          name: dto.objectiveInfo.name,
        } as ApprovalLearnerModel,
        PdPlanStatus: {
          userId: dto.objectiveInfo.identity.id,
          assessmentInfo: {
            dueDate: dto.dueDate ? new Date(dto.dueDate) : null,
            identity: dto.resultIdentity,
            statusInfo: dto.assessmentStatusInfo,
          },
        },
        learnerDetailUrl: `/employee/detail/${dto.objectiveInfo.identity.id}`,
      } as PdPlanGridRowModel;
    });
  }

  static toNominateGroupModels(
    nominateRequestDTOs: AssignedPDOResultDTO[]
  ): GroupAssignPDOResultModel[] {
    if (isEmpty(nominateRequestDTOs)) {
      return [];
    }

    return nominateRequestDTOs.map((dto) => new GroupAssignPDOResultModel(dto));
  }

  static toNominateDepartmentModels(
    nominateRequestDTOs: AssignedPDOResultDTO[]
  ): DepartmentAssignPDOResultModel[] {
    if (isEmpty(nominateRequestDTOs)) {
      return [];
    }

    return nominateRequestDTOs.map(
      (dto) => new DepartmentAssignPDOResultModel(dto)
    );
  }

  static toMassNominationModels(
    nominateRequestDTOs: MassAssignedPDOResultDTO[]
  ): MassAssignPDOResultModel[] {
    if (isEmpty(nominateRequestDTOs)) {
      return [];
    }

    return nominateRequestDTOs.map((dto) => new MassAssignPDOResultModel(dto));
  }
  static getAssignPDOMessage(
    assignResult: AssignPDOpportunityResponse,
    assignPDOMode: AssignModeEnum
  ): string {
    const nominateSuccessMessage = (x: number) =>
      `Your nomination request is successful. ${x} learner(s) have been submitted for approval`;
    const recommendSuccessMessage = 'Your recommendation request is successful';
    const messageCode = AssignPDOHelper.getAssignPDOMessageCode(
      assignResult,
      assignPDOMode
    );
    const errorMessage = AssignPDOHelper.getAssignPDOErrorMessage(
      assignPDOMode
    );

    const defaultMessage = assignResult.isSuccess
      ? assignPDOMode === AssignModeEnum.Nominate ||
        assignPDOMode === AssignModeEnum.AdhocNominate
        ? nominateSuccessMessage(assignResult.totalLearner)
        : recommendSuccessMessage
      : errorMessage;

    return (
      (AssignPDOHelper.assignedResultMessageMapping[messageCode] as string) ||
      defaultMessage
    );
  }

  static getChangeNominateStatusMessage(
    assignResult: AssignPDOpportunityResponse,
    isApproveMode: boolean
  ): string {
    const successLearnerCount =
      (assignResult.totalLearner || 0) -
      (assignResult.assignedLearnerResults
        ? assignResult.assignedLearnerResults.length
        : 0);
    const successMessage = isApproveMode
      ? `You have approved successfully for ${successLearnerCount} learner(s).`
      : 'Reject successfully';
    const messageCode =
      assignResult.messageCode === 'NO_VALID_LEARNER'
        ? 'NO_VALID_LEARNER_TO_NOMINATE'
        : assignResult.messageCode;

    const errorMessage = isApproveMode
      ? 'Approval nomination request has not been completed, please try later'
      : 'Rejection nomination request has not been completed, please try later';

    const defaultMessage = assignResult.isSuccess
      ? successMessage
      : errorMessage;

    return (
      (AssignPDOHelper.assignedResultMessageMapping[messageCode] as string) ||
      defaultMessage
    );
  }

  static getAssignPDOMessageCode(
    assignResult: AssignPDOpportunityResponse,
    assignPDOMode: AssignModeEnum
  ): string {
    return assignResult.messageCode === 'NO_VALID_LEARNER'
      ? assignPDOMode === AssignModeEnum.Nominate ||
        assignPDOMode === AssignModeEnum.AdhocNominate
        ? 'NO_VALID_LEARNER_TO_NOMINATE'
        : 'NO_VALID_LEARNER_TO_RECOMMEND'
      : assignResult.messageCode;
  }

  static getAssignPDOErrorMessage(assignPDOMode: AssignModeEnum): string {
    return assignPDOMode === AssignModeEnum.Nominate ||
      assignPDOMode === AssignModeEnum.AdhocNominate
      ? 'Nominate classrun has not been completed, please try later'
      : 'Recommend PD Opportunity has not been completed, please try later';
  }

  static mapToAssignResultModel = (
    result: AssignedLearnerResult,
    learnerInfos: LearnerInfoDTO[]
  ): AssignResultModel => {
    const isSuccess = result.isSuccess;
    const reason =
      (AssignPDOHelper.assignedResultMessageMapping[
        result.messageCode
      ] as string) || '';
    const learnerInfo = learnerInfos
      ? learnerInfos.find(
          (learner) => learner.userCxId === result.identity.extId
        )
      : undefined;
    const name = learnerInfo ? learnerInfo.fullName : 'N/A';
    const email = learnerInfo ? learnerInfo.emailAddress : 'N/A';
    const avatar = ImageHelpers.getAvatarFromEmail(
      learnerInfo ? learnerInfo.emailAddress : undefined
    );

    return new AssignResultModel({
      reason,
      name,
      email,
      avatar,
      isSuccess,
    });
  };

  static isApproveMode(status: NominateStatusCodeEnum): boolean {
    return (
      status === NominateStatusCodeEnum.PendingForApproval2nd ||
      status === NominateStatusCodeEnum.Approved
    );
  }

  static isElearningPublicCourse(course: PDOpportunityDetailModel): boolean {
    if (!course) {
      return false;
    }

    const isELearningCourse =
      course.learningMode === Constant.E_LEARNING_COURSE_TAG_ID;
    const isPublicCourse =
      course.registrationMethod === CourseRegistrationMethod.Public;

    return isELearningCourse && isPublicCourse;
  }

  static getNominateIndividualConfirmationMessage(
    selectedLearners: number,
    isELearningPublicCourse: boolean,
    translateService: TranslateService
  ): string {
    let confirmationMessage: string = translateService.instant(
      'Odp.LearningPlan.PlannedPDODetail.NominateIndividualConfirmation',
      {
        selectedLearners,
        pluralCharacter: selectedLearners > 1 ? 's' : '',
      }
    ) as string;

    if (isELearningPublicCourse) {
      const nominateELearningPublicCourseConfirmationMessage = translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.NominateELearningPublicCourseConfirmation'
      ) as string;
      confirmationMessage += ` ${nominateELearningPublicCourseConfirmationMessage}`;
    }

    return confirmationMessage;
  }

  static getNominateGroupConfirmationMessage(
    isELearningPublicCourse: boolean,
    translateService: TranslateService
  ): string {
    let confirmationMessage: string = translateService.instant(
      'Odp.LearningPlan.PlannedPDODetail.NominateGroupConfirmation'
    ) as string;

    if (isELearningPublicCourse) {
      const nominateELearningPublicCourseConfirmationMessage = translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.NominateELearningPublicCourseConfirmation'
      ) as string;
      confirmationMessage += ` ${nominateELearningPublicCourseConfirmationMessage}`;
    }

    return confirmationMessage;
  }

  static getNominateDepartmentConfirmationMessage(
    isELearningPublicCourse: boolean,
    translateService: TranslateService
  ): string {
    let confirmationMessage: string = translateService.instant(
      'Odp.LearningPlan.PlannedPDODetail.NominateDepartmentConfirmation'
    ) as string;

    if (isELearningPublicCourse) {
      const nominateELearningPublicCourseConfirmationMessage = translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.NominateELearningPublicCourseConfirmation'
      ) as string;
      confirmationMessage += ` ${nominateELearningPublicCourseConfirmationMessage}`;
    }

    return confirmationMessage;
  }
}
