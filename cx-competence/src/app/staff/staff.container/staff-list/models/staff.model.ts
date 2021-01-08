import { ApprovalGroup } from 'app-models/approval-group.model';
import { AssessmentInfo } from 'app-models/assessment-info.model';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import { Identity } from 'app-models/common.model';
import { UserStatusTypeEnum } from 'app/shared/constants/user-status-type.enum';

export class Department {
  description: string;
  identity: any;
  name: string;
}

export class UserType {
  description: string;
  identity: Identity;
  name: string;
}

export class Staff {
  firstName: string;
  lastName: string;
  fullName: string;
  assessmentInfo: AssessmentInfo;
  department: Department;
  email: string;
  identity: Identity;
  entityStatus: StaffEntityStatus;
  lastActive: Date;
  systemRoleInfos: Array<UserType>;
  userGroupInfos: Array<UserType>;
  personnelGroups: Array<UserType>; // or Service Scheme.
  developmentalRoles: Array<UserType>;
  careerPaths: Array<UserType>; // or Career Tracks
  experienceCategories: Array<UserType>;
  learningFrameworks: Array<UserType>;
  approvalGroups: Array<ApprovalGroup>;
  assessmentInfos: AssessmentInfos;
  avatarUrl: string;
}

export class AssessmentInfos {
  // tslint:disable-next-line:variable-name
  LearningNeed: IdpAssessmentInfo;
  // tslint:disable-next-line:variable-name
  LearningPlan: IdpAssessmentInfo;
}

export class IdpAssessmentInfo {
  statusInfo: AssessmentStatusInfo;
  identity?: Identity;
  dueDate?: Date;
}

export class StaffEntityStatus {
  externallyMastered: boolean;
  lastExternallySynchronized: string;
  lastUpdated: string;
  statusId: UserStatusTypeEnum;
}

export class PagedStaffsList {
  totalItems: number;
  pageIndex: number;
  pageSize: number;
  items: Staff[];
}
