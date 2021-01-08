import {
  AssessmentStatusInfo,
  ObjectiveInfo,
} from 'app-models/assessment.model';
import { Identity } from 'app-models/common.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { ChangeNominationStatusTargetEnum } from 'app/approval-page/models/approval.enum';
import {
  ApprovalClassRunModel,
  ApprovalCourseModel,
  ApprovalDepartmentModel,
  ApprovalGroupModel,
  ApprovalLearnerModel,
  ApprovalMassNominationModel,
} from 'app/approval-page/models/class-registration.model';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';
import {
  AssignedPDOResultDTO,
  MassAssignedPDOResultDTO,
  MassAssignPDOResultAdditionalPropertiesDTO,
  PDOpportunityAnswerDTO,
} from './pdo-action-item.model';

export class AssignPDOpportunityPayload {
  identities: Identity[];
  courseId: string;
  classRunId?: string;
  klpExtId: string;
  departmentId: number;
  answer: PDOpportunityAnswerDTO;
  proceedAssign: boolean = false;
  exceptLearnerExtIds: string[];
  nominationApprovingOfficerExtId?: string;
  isRouteIndividualLearnerAOForApproval: boolean = false;
  isExternalPDO: boolean = false;
  isELearningPublicCourse: boolean = false;

  constructor(payload?: Partial<AssignPDOpportunityPayload>) {
    if (!payload) {
      return;
    }
    ObjectUtilities.copyPartialObject(payload, this);
  }
}

export class MassAssignPDOpportunityPayload {
  file: File;
  keyLearningProgramExtId: string;
  departmentId: number;
  proceedAssign: boolean = true;
  nominationApprovingOfficerExtId?: string;
  isRouteIndividualLearnerAOForApproval: boolean = false;
  sendEmail?: boolean = true;
  isAdhoc?: boolean = false;

  constructor(payload?: Partial<MassAssignPDOpportunityPayload>) {
    if (!payload) {
      return;
    }
    ObjectUtilities.copyPartialObject(payload, this);
  }
  toFormData(): FormData {
    // TODO: Refactor this for general usage.
    const formData = new FormData();
    formData.append('file', this.file, this.file.name);
    if (this.keyLearningProgramExtId) {
      formData.append('keyLearningProgramExtId', this.keyLearningProgramExtId);
    }
    if (this.departmentId) {
      formData.append('departmentId', `${this.departmentId}`);
    }
    if (this.proceedAssign) {
      formData.append('proceedAssign', `${this.proceedAssign}`);
    }
    if (this.nominationApprovingOfficerExtId) {
      formData.append(
        'nominationApprovingOfficerExtId',
        `${this.nominationApprovingOfficerExtId}`
      );
    }
    if (this.isRouteIndividualLearnerAOForApproval) {
      formData.append(
        'isRouteIndividualLearnerAOForApproval',
        `${this.isRouteIndividualLearnerAOForApproval}`
      );
    }
    if (this.sendEmail) {
      formData.append('sendEmail', `${this.sendEmail}`);
    }
    if (this.sendEmail) {
      formData.append('isAdhoc', `${this.isAdhoc}`);
    }

    return formData;
  }
}

export class GetAssignedPDOParams {
  courseId: string;
  classRunId: string;
  departmentId: number;
  pageIndex: number;
  pageSize: number;
  timestamp?: string;
  status?: NominateStatusCodeEnum;
  includeCourseInfo?: boolean;
  includeClassRunInfo?: boolean;
  getForApproval?: boolean;

  constructor(payload?: Partial<GetAssignedPDOParams>) {
    if (!payload) {
      return;
    }
    ObjectUtilities.copyPartialObject(payload, this);
  }
}

export class GetMassAssignedPDOParams extends GetAssignedPDOParams {
  klpExtId: string;
  constructor(payload?: Partial<GetMassAssignedPDOParams>) {
    super(payload);
    if (!payload) {
      return;
    }
    this.klpExtId = payload.klpExtId || '';
  }
}

export class GetLearnerAssignedPDOParams extends GetAssignedPDOParams {
  relatedObjectId?: number;
  relatedObjectArchetype?: string;
}

export class GetAdhocMassNominationLearnerParams extends GetAssignedPDOParams {
  // The assignment Id of ad-hoc mass-nomination id
  resultId: string | number;

  constructor(payload?: Partial<GetAdhocMassNominationLearnerParams>) {
    super(payload);
    if (!payload) {
      return;
    }
  }
}

export interface AssignedLearnerResult {
  isSuccess?: boolean;
  messageCode?: string;
  message?: string;
  resultId?: number;
  identity?: Identity;
}

export interface ChangeStatusNominateRequestPayload {
  resultIds: number[];
  target: ChangeNominationStatusTargetEnum;
  changePDOpportunityStatus: NominateStatusCodeEnum;
  reason?: string;
  proceedAssign?: boolean;
  exceptResultIds?: number[];
}

export interface AssignPDOpportunityResponse {
  isSuccess?: boolean;
  message?: string;
  messageCode?: string;
  assignedLearnerResults?: AssignedLearnerResult[];
  totalLearner?: number;
}

export class AssignedPDOResultModel {
  id: number;
  identity: Identity;
  course: ApprovalCourseModel;
  classRun: ApprovalClassRunModel;
  status: AssessmentStatusInfo;
  createdAt: string;
  createdBy: ObjectiveInfo;
  createdByResultExtId: string;
  additionalProperties: any;
  timestamp: string;

  constructor(dto?: AssignedPDOResultDTO) {
    if (!dto) {
      return;
    }

    dto.resultIdentity = dto.resultIdentity ? dto.resultIdentity : {};

    this.id = dto.resultIdentity.id;
    this.identity = dto.resultIdentity;
    this.status = dto.assessmentStatusInfo;
    this.createdBy = dto.createdBy;
    this.createdAt = dto.created;

    if (!dto.additionalProperties) {
      return;
    }

    this.createdAt = dto.additionalProperties.assignedDate || this.createdAt;
    this.course = new ApprovalCourseModel(
      dto.additionalProperties.courseDetail
    );

    const courseUri = dto.additionalProperties.learningOpportunityUri;

    if (PDPlannerHelpers.isExternalPDOUri(courseUri)) {
      this.classRun = new ApprovalClassRunModel(
        dto.additionalProperties.classRunDetail
      );
    } else {
      this.classRun = new ApprovalClassRunModel({
        classTitle: 'External PD Opportunity',
      });
    }

    this.createdByResultExtId = dto.additionalProperties.fromResultExtId;
    this.timestamp = dto.timestamp;
  }
}

export class LearnerAssignPDOResultModel extends AssignedPDOResultModel {
  learner: ApprovalLearnerModel;
  constructor(nominateDTO?: AssignedPDOResultDTO) {
    super(nominateDTO);
    if (!nominateDTO) {
      return;
    }
    let user = nominateDTO.additionalProperties
      ? nominateDTO.additionalProperties.userDetail
      : null;
    if (!user && nominateDTO.objectiveInfo) {
      user = {
        avatarUrl: '',
        email: nominateDTO.objectiveInfo.email,
        fullName: nominateDTO.objectiveInfo.name,
        userId: nominateDTO.objectiveInfo.identity
          ? nominateDTO.objectiveInfo.identity.id
          : null,
      };
    }
    this.learner = new ApprovalLearnerModel(user);
  }
}

export class GroupAssignPDOResultModel extends AssignedPDOResultModel {
  group: ApprovalGroupModel;
  pagingMemberAssignedItems?: PagingResponseModel<LearnerAssignPDOResultModel>;
  constructor(nominateDTO?: AssignedPDOResultDTO) {
    super(nominateDTO);
    if (!nominateDTO) {
      return;
    }
    this.group = new ApprovalGroupModel(
      nominateDTO.additionalProperties.groupDetail
    );
    this.additionalProperties = nominateDTO.additionalProperties;
  }
}

export class DepartmentAssignPDOResultModel extends AssignedPDOResultModel {
  department: ApprovalDepartmentModel;
  pagingMemberAssignedItems?: PagingResponseModel<LearnerAssignPDOResultModel>;
  constructor(nominateDTO?: AssignedPDOResultDTO) {
    super(nominateDTO);
    if (!nominateDTO) {
      return;
    }
    this.department = new ApprovalDepartmentModel(
      nominateDTO.additionalProperties.departmentDetail
    );
    this.additionalProperties = nominateDTO.additionalProperties;
  }
}

export class MassAssignPDOResultBaseModel {
  id: number;
  created: string;
  createdBy: ObjectiveInfo;
  assessmentStatusInfo?: AssessmentStatusInfo;
  constructor(nominateDTO?: MassAssignedPDOResultDTO) {
    if (!nominateDTO) {
      return;
    }
    this.id = nominateDTO.resultIdentity.id;
    this.createdBy = nominateDTO.createdBy;
    this.created = nominateDTO.created;
    this.assessmentStatusInfo = nominateDTO.assessmentStatusInfo;
    if (!nominateDTO.additionalProperties) {
      return;
    }
  }
}

export class MassAssignPDOResultModel extends MassAssignPDOResultBaseModel {
  filePath: string;
  fileName: string;
  fileId: string;
  originalFileName: string;
  resultIdentity: Identity;
  objectiveInfo?: ObjectiveInfo;
  additionalProperties: MassAssignPDOResultAdditionalPropertiesDTO;
  timestamp: string;

  displayFileName: string;
  email: string;
  assessmentStatusName: string;
  numberOfRegisters: number;
  numberOfPendingApprovals: number;
  massNominate: ApprovalMassNominationModel;
  constructor(nominateDTO?: MassAssignedPDOResultDTO) {
    super(nominateDTO);
    if (!nominateDTO) {
      return;
    }
    this.email = this.createdBy ? this.createdBy.email : 'N/A';
    this.assessmentStatusName =
      nominateDTO.assessmentStatusInfo.assessmentStatusName;
    if (!nominateDTO.additionalProperties) {
      return;
    }
    if (nominateDTO.additionalProperties.massNominationDetail) {
      this.numberOfRegisters =
        nominateDTO.additionalProperties.massNominationDetail.totalLearner || 0;
      this.numberOfPendingApprovals =
        (nominateDTO.additionalProperties.massNominationDetail
          .totalPendingLv1 || 0) +
        (nominateDTO.additionalProperties.massNominationDetail
          .totalPendingLv2 || 0);
      this.fileId =
        nominateDTO.additionalProperties.massNominationDetail
          .massNominationIdentity.fileId || '';
      this.filePath =
        nominateDTO.additionalProperties.massNominationDetail
          .massNominationIdentity.filePath || '';
      this.fileName =
        nominateDTO.additionalProperties.massNominationDetail
          .massNominationIdentity.fileName || '';
      this.originalFileName =
        nominateDTO.additionalProperties.massNominationDetail
          .massNominationIdentity.originalFileName || '';
    }

    this.resultIdentity = nominateDTO.resultIdentity;
    this.timestamp = nominateDTO.timestamp;
    this.additionalProperties = nominateDTO.additionalProperties;
    this.objectiveInfo = nominateDTO.objectiveInfo;
    this.massNominate = new ApprovalMassNominationModel(
      nominateDTO.additionalProperties.massNominationDetail
    );
  }
}

export class DownloadMassAssignedPDOReportFileModel {
  fileName: string;
  filePath: string;

  constructor(payload?: Partial<DownloadMassAssignedPDOReportFileModel>) {
    if (!payload) {
      return;
    }
    ObjectUtilities.copyPartialObject(payload, this);
  }
}
