import {
  AssessmentStatusInfo,
  ObjectiveInfo,
  ResultIdentity,
} from 'app-models/assessment.model';
import { Identity } from 'app-models/common.model';
import { ApprovalTypeEnum } from 'app/approval-page/models/approval.enum';
import {
  ApprovalDepartmentDTO,
  ApprovalGroupDTO,
  ApprovalLearnerDTO,
} from 'app/approval-page/models/class-registration.dto';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { ApprovalMassNominationDTO } from './../../../approval-page/models/class-registration.dto';

export enum PDOSource {
  CoursePadPDO = 'coursepad-pdo',
  CustomPDO = 'custom-pdo',
}

export class ActionItemResult {
  public resultIdentity: ResultIdentity;
  public objectiveInfo: ObjectiveInfo;
  constructor(resultIdentity: ResultIdentity, objectiveInfo: ObjectiveInfo) {
    this.resultIdentity = resultIdentity;
    this.objectiveInfo = objectiveInfo;
  }
}

export enum PDOAddType {
  Nominated = 'nominated',
  CAMNominated = 'cam-nominated',
  AdhocNominated = 'adhoc-nominated',
  SelfRegistered = 'self-registered',
  Recommended = 'recommended',
  MassNominated = 'massnominated',
}

export class AdditionalPropertiesDTO {
  type?: PDOAddType;
  fromResultExtId?: string; // Store KLP extId
  relatedObjectIdentity?: Identity; // Store group/department relate nominated
  learningOpportunityUri?: string;
  courseId?: string;
  classRunId?: string;
  isCompleted?: boolean;
  unPublished?: boolean;
}

export class AssignPDOResultAdditionalPropertiesDTO extends AdditionalPropertiesDTO {
  courseDetail?: CourseDetailDTO;
  classRunDetail?: ClassRunDetailDTO;
  userDetail?: ApprovalLearnerDTO;
  groupDetail?: ApprovalGroupDTO;
  departmentDetail?: ApprovalDepartmentDTO;
  assignedDate?: string;
}

export class MassAssignPDOResultAdditionalPropertiesDTO extends AdditionalPropertiesDTO {
  massNominationDetail?: ApprovalMassNominationDTO;
}

export class MassAssignPDOIdentityDTO {
  filePath: string;
  fileName: string;
  fileId: string;
  originalFileName: string;
}
export class PDOpportunityExtensions {
  mode?: PDCatalogResourceItem; // Mode of PD Activity
  venue?: string; // Locality of PD Opportunity
  startDate?: string;
  endDate?: string;
  description?: string; // Objectives / Description of PD Activity
  pdOpportunityType?: PDCatalogResourceItem; // Type of PD Opportunity
  duration?: number;
  tags?: string[];
  courseCode?: string;
  enableExternalPDOApproval?: boolean;
  approvalType?: ApprovalTypeEnum;
  externalPDOApprovingOfficerExtId?: string;
}

export class ExternalPDOExtensions extends PDOpportunityExtensions {
  area?: PDCatalogResourceItem[]; // Connection To Learning Framework
  courseNature?: PDCatalogResourceItem; // Nature of PD Activity

  // Don't have on document, need confirm name
  cost?: number; // Cost of PD Opportunity
  capacity?: string; // Capacity of Attendee
  organiser?: string; // Name of Organiser
  isPublishToEPortfolio?: string; // Publish to e-Portfolio Learning Log
}

export class PDOpportunityDTO {
  uri?: string;
  name?: string;
  source?: PDOSource;
  thumbnailUrl?: string;
  subject?: PDCatalogResourceItem[];
  extensions?: ExternalPDOExtensions | PDOpportunityExtensions;
}

export class PDOpportunityAnswerDTO {
  learningOpportunity?: PDOpportunityDTO;
  startDateEdited?: string;
  endDateEdited?: string;
  classRunId?: string;
  tranningCost?: TranningCost;
}

export class PDOpportunityModel {
  answerDTO: PDOpportunityAnswerDTO;
  identityActionItemDTO: ResultIdentity;
  assessmentStatusInfo: AssessmentStatusInfo;
  createdBy: ObjectiveInfo;
  additionalProperties?: AdditionalPropertiesDTO;
  unPublished?: boolean;
  canDelete?: boolean;
}

export class PDOActionItemDTO extends IdpDto {
  answer?: PDOpportunityAnswerDTO;
  additionalProperties?: AdditionalPropertiesDTO;
  canDelete?: boolean;
}

export class AssignedPDOResultDTO extends IdpDto {
  additionalProperties?: AssignPDOResultAdditionalPropertiesDTO;
}

export class MassAssignedPDOResultDTO extends IdpDto {
  resultIdentity: Identity;
  additionalProperties?: MassAssignPDOResultAdditionalPropertiesDTO;
}
export class PDOpportunityInfo {
  uri: string;
  name: string;
  constructor(uri: string, name: string) {
    this.uri = uri;
    this.name = name;
  }
}
export class NominatedInfo {
  public learningOpportunityInfo: PDOpportunityDTO;
  public learnerInfos: ActionItemResult[];
}

export class PDCatalogResourceItem {
  id: string;
  displayText: string;
}

export enum AssignTargetObject {
  Learner = 'learner',
  Group = 'group',
  Department = 'department',
  MassNomination = 'massnomination',
}

export enum PickApprovingOfficerTarget {
  CAO = 'cao',
  Admin = 'admin',
}

export class TranningCost {
  numOfOfficersPlanned?: number;
  numOfHoursPlanned?: number;
  costPerPaxPlanned?: number;
}

export interface CourseDetailDTO {
  courseId: string;
  courseName: string;
  thumbnailUrl: string;
  description: string;
  durationHours: number;
  durationMinutes: number;
  isExternalPDO: boolean;
}

export interface ClassRunDetailDTO {
  classRunId: string;
  classRunCode: string;
  classTitle: string;
  startDate: string;
  endDate: string;
  applicationStartDate: string;
}
