import { MassAssignPDOIdentityDTO } from 'app-models/mpj/pdo-action-item.model';
import {
  ClassRegistrationStatusEnum,
  ClassRunChangeStatusEnum,
  WithrawalStatusEnum,
} from './class-registration.model';

export interface RegistrationDTO {
  registrationId: string;
  registrationStatus: ClassRegistrationStatusEnum;
  withdrawalStatus: WithrawalStatusEnum;
  classRunChangeStatus: ClassRunChangeStatusEnum;
  registrationDate: string;
  classRunChangeRequestDate: string;
  withdrawalRequestDate: string;
  reason: string;
  classRun: RegistationClassRunDTO;
  classRunChange: RegistationClassRunDTO;
  course: RegistationCourseDTO;
  user: ApprovalLearnerDTO;
}

export interface RegistationClassRunDTO {
  classRunId: string;
  classTitle: string;
  startDate: string;
  endDate: string;
  applicationStartDate: string;
}

export interface RegistationCourseDTO {
  courseId: string;
  courseName: string;
  thumbnailUrl: string;
  description: string;
  durationHours: number;
  durationMinutes: number;
}

export interface ApprovalLearnerDTO {
  avatarUrl: string;
  userId: number;
  fullName: string;
  email: string;
}

export interface ApprovalGroupDTO {
  groupId: number;
  name: string;
  totalLearner: number;
  numberLearnerByStatusType: GroupNominationStatusSummary;
}

export interface ApprovalDepartmentDTO {
  departmentId: number;
  name: string;
  totalLearner: number;
  numberLearnerByStatusType: GroupNominationStatusSummary;
}

export interface GroupNominationStatusSummary {
  Approved: number;
  NotNominated: number;
  PendingForApproval: number;
  PendingForApproval2nd: number;
  PendingForApproval3rd: number;
  Rejected: number;
  Rejected2nd: number;
  Rejected3nd: number;
  Rejected4th: number;
  Rejected5th: number;
}

export interface ApprovalMassNominationDTO {
  massNominationIdentity: MassAssignPDOIdentityDTO;
  name: string;
  totalLearner: number;
  totalApproved: number;
  totalPendingLv1: number;
  totalPendingLv2: number;
  totalPendingLv3: number;
  totalRejectedLv1: number;
  totalRejectedLv2: number;
  totalRejectedLv3: number;
  totalRejectedLv4: number;
  totalRejectedLv5: number;
}

export interface GetClassRegistrationDTO {
  pageIndex: number;
  pageSize: number;
  registrationFilterType: RegistrationFilterTypeEnum;
}

export interface ChangeStatusClassRegistrationDTO {
  ids: string[];
  status: ClassRegistrationStatusEnum;
  comment?: string;
}

export interface ChangeStatusClassWithdrawalDTO {
  ids: string[];
  withdrawalStatus: ClassRegistrationStatusEnum;
  comment?: string;
}

export interface ChangeStatusClassChangeRequestDTO {
  ids: string[];
  classRunChangeStatus: ClassRegistrationStatusEnum;
  comment?: string;
}

export enum RegistrationFilterTypeEnum {
  Registration = 'Registration',
  Withdraw = 'Withdraw',
  ClassRunChangeRequest = 'ClassRunChange',
}
