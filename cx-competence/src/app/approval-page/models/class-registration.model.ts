import {
  ClassRunDetailDTO,
  CourseDetailDTO,
  MassAssignPDOIdentityDTO,
} from 'app-models/mpj/pdo-action-item.model';
import {
  ApprovalDepartmentDTO,
  ApprovalGroupDTO,
  ApprovalLearnerDTO,
  ApprovalMassNominationDTO,
  RegistrationDTO,
} from './class-registration.dto';

export enum ClassRegistrationStatusEnum {
  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ConfirmedByCA = 'ConfirmedByCA',
  RejectedByCA = 'RejectedByCA',
  WaitlistPendingApprovalByLearner = 'WaitlistPendingApprovalByLearner',
  WaitlistConfirmed = 'WaitlistConfirmed',
  WaitlistRejected = 'WaitlistRejected',
  OfferPendingApprovalByLearner = 'OfferPendingApprovalByLearner',
  OfferRejected = 'OfferRejected',
  OfferConfirmed = 'OfferConfirmed',
  AddedByCAConflict = 'AddedByCAConflict',
  AddedByCAClassfull = 'AddedByCAClassfull',
  OfferExpired = 'OfferExpired',
}

export enum WithrawalStatusEnum {
  PendingConfirmation = 'PendingConfirmation',
  Rejected = 'Rejected',
  Approved = 'Approved',
  Withdrawn = 'Withdrawn',
  RejectedByCA = 'RejectedByCA',
}

export enum ClassRunChangeStatusEnum {
  PendingConfirmation = 'PendingConfirmation',
  Rejected = 'Rejected',
  Approved = 'Approved',
  ConfirmedByCA = 'ConfirmedByCA',
  RejectedByCA = 'RejectedByCA',
}

export enum ClassRegistrationModeEnum {
  CLASS_REGISTRATION = 'class-registration',
  CLASS_WITHDRAWAL = 'class-withdrawal',
  CLASS_CHANGE_REQUEST = 'class-change-request',
}

export class ApprovalClassRunModel {
  id: string;
  code: string;
  name: string;
  startDate: string;
  endDate: string;
  applicationStartDate: string;
  constructor(classRunDTO?: Partial<ClassRunDetailDTO>) {
    if (!classRunDTO) {
      return;
    }
    this.id = classRunDTO.classRunId;
    this.code = classRunDTO.classRunCode;
    this.name = classRunDTO.classTitle;
    this.startDate = classRunDTO.startDate;
    this.endDate = classRunDTO.endDate;
    this.applicationStartDate = classRunDTO.applicationStartDate;
  }
}

export class ApprovalCourseModel {
  id: string;
  name: string;
  thumbnail: string;
  description: string;
  duration: number;
  isExternalPDO: boolean;

  constructor(courseDTO?: Partial<CourseDetailDTO>) {
    if (!courseDTO) {
      return;
    }
    this.id = courseDTO.courseId;
    this.name = courseDTO.courseName;
    this.thumbnail = courseDTO.thumbnailUrl;
    this.description = courseDTO.description;
    this.duration = courseDTO.durationHours;
    this.isExternalPDO = courseDTO.isExternalPDO;
  }
}

export class ApprovalLearnerModel {
  id: number;
  name: string;
  email: string;
  avatar: string;

  constructor(learnerDTO?: Partial<ApprovalLearnerDTO>) {
    if (!learnerDTO) {
      return;
    }
    this.id = learnerDTO.userId;
    this.name = learnerDTO.fullName;
    this.email = learnerDTO.email;
    this.avatar = learnerDTO.avatarUrl;
  }
}

export interface IApprovalUnitModel {
  id: number;
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

  totalNotNominated: number;
}

export class ApprovalGroupModel implements IApprovalUnitModel {
  id: number;
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

  public get totalNotNominated(): number {
    return (
      (this.totalLearner || 0) -
      (this.totalApproved || 0) -
      (this.totalPendingLv1 || 0) -
      (this.totalPendingLv2 || 0) -
      (this.totalPendingLv3 || 0) -
      (this.totalRejectedLv1 || 0) -
      (this.totalRejectedLv2 || 0) -
      (this.totalRejectedLv3 || 0) -
      (this.totalRejectedLv4 || 0) -
      (this.totalRejectedLv5 || 0)
    );
  }

  constructor(dto?: Partial<ApprovalGroupDTO>) {
    if (!dto) {
      return;
    }
    this.id = dto.groupId;
    this.name = dto.name;
    this.totalLearner = dto.totalLearner;
    if (dto.numberLearnerByStatusType) {
      this.totalApproved = dto.numberLearnerByStatusType.Approved;
      this.totalPendingLv1 = dto.numberLearnerByStatusType.PendingForApproval;
      this.totalPendingLv2 =
        dto.numberLearnerByStatusType.PendingForApproval2nd;
      this.totalPendingLv3 =
        dto.numberLearnerByStatusType.PendingForApproval3rd;
      this.totalRejectedLv1 = dto.numberLearnerByStatusType.Rejected;
      this.totalRejectedLv2 = dto.numberLearnerByStatusType.Rejected2nd;
      this.totalRejectedLv3 = dto.numberLearnerByStatusType.Rejected3nd;
      this.totalRejectedLv4 = dto.numberLearnerByStatusType.Rejected4th;
      this.totalRejectedLv5 = dto.numberLearnerByStatusType.Rejected5th;
    }
  }
}

export class ApprovalDepartmentModel implements IApprovalUnitModel {
  id: number;
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

  public get totalNotNominated(): number {
    return (
      (this.totalLearner || 0) -
      (this.totalApproved || 0) -
      (this.totalPendingLv1 || 0) -
      (this.totalPendingLv2 || 0) -
      (this.totalPendingLv3 || 0) -
      (this.totalRejectedLv1 || 0) -
      (this.totalRejectedLv2 || 0) -
      (this.totalRejectedLv3 || 0) -
      (this.totalRejectedLv4 || 0) -
      (this.totalRejectedLv5 || 0)
    );
  }

  constructor(dto?: Partial<ApprovalDepartmentDTO>) {
    if (!dto) {
      return;
    }
    this.id = dto.departmentId;
    this.name = dto.name;
    this.totalLearner = dto.totalLearner;
    if (dto.numberLearnerByStatusType) {
      this.totalApproved = dto.numberLearnerByStatusType.Approved;
      this.totalPendingLv1 = dto.numberLearnerByStatusType.PendingForApproval;
      this.totalPendingLv2 =
        dto.numberLearnerByStatusType.PendingForApproval2nd;
      this.totalPendingLv3 =
        dto.numberLearnerByStatusType.PendingForApproval3rd;
      this.totalRejectedLv1 = dto.numberLearnerByStatusType.Rejected;
      this.totalRejectedLv2 = dto.numberLearnerByStatusType.Rejected2nd;
      this.totalRejectedLv3 = dto.numberLearnerByStatusType.Rejected3nd;
      this.totalRejectedLv4 = dto.numberLearnerByStatusType.Rejected4th;
      this.totalRejectedLv5 = dto.numberLearnerByStatusType.Rejected5th;
    }
  }
}

export class ApprovalMassNominationModel implements IApprovalUnitModel {
  massNominationIdentity: MassAssignPDOIdentityDTO;
  id: number;
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

  public get totalNotNominated(): number {
    return (
      (this.totalLearner || 0) -
      (this.totalApproved || 0) -
      (this.totalPendingLv1 || 0) -
      (this.totalPendingLv2 || 0) -
      (this.totalPendingLv3 || 0) -
      (this.totalRejectedLv1 || 0) -
      (this.totalRejectedLv2 || 0) -
      (this.totalRejectedLv3 || 0) -
      (this.totalRejectedLv4 || 0) -
      (this.totalRejectedLv5 || 0)
    );
  }

  constructor(dto?: Partial<ApprovalMassNominationDTO>) {
    if (!dto) {
      return;
    }
    this.massNominationIdentity = dto.massNominationIdentity;
    this.name = dto.name;
    this.totalLearner = dto.totalLearner;
    this.totalApproved = dto.totalApproved;
    this.totalPendingLv1 = dto.totalPendingLv1;
    this.totalPendingLv2 = dto.totalPendingLv2;
    this.totalPendingLv3 = dto.totalPendingLv3;
    this.totalRejectedLv1 = dto.totalRejectedLv1;
    this.totalRejectedLv2 = dto.totalRejectedLv2;
    this.totalRejectedLv3 = dto.totalRejectedLv3;
    this.totalRejectedLv4 = dto.totalRejectedLv4;
    this.totalRejectedLv5 = dto.totalRejectedLv5;
  }
}

export class RegistrationModel {
  id: string;
  registrationStatus: ClassRegistrationStatusEnum;
  withdrawalStatus: WithrawalStatusEnum;
  classRunChangeStatus: ClassRunChangeStatusEnum;

  registrationDate: string;
  classRunChangeRequestDate: string;
  withdrawalRequestDate: string;
  reason: string;
  classRun: ApprovalClassRunModel;
  classRunChange: ApprovalClassRunModel;
  course: ApprovalCourseModel;
  learner: ApprovalLearnerModel;

  constructor(registrationDTO?: Partial<RegistrationDTO>) {
    if (!registrationDTO) {
      return;
    }
    this.id = registrationDTO.registrationId;

    this.registrationStatus = registrationDTO.registrationStatus;
    this.withdrawalStatus = registrationDTO.withdrawalStatus;
    this.classRunChangeStatus = registrationDTO.classRunChangeStatus;

    this.registrationDate = registrationDTO.registrationDate;

    this.classRunChangeRequestDate = registrationDTO.classRunChangeRequestDate;
    this.withdrawalRequestDate = registrationDTO.withdrawalRequestDate;

    this.reason = registrationDTO.reason;

    this.classRun = new ApprovalClassRunModel(registrationDTO.classRun);
    this.classRunChange = new ApprovalClassRunModel(
      registrationDTO.classRunChange
    );

    this.course = new ApprovalCourseModel(registrationDTO.course);
    this.learner = new ApprovalLearnerModel(registrationDTO.user);
  }
}
