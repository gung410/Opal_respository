import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';
import {
  ApprovalTargetEnum,
  ChangeNominationStatusTargetEnum,
} from '../models/approval.enum';
import { ApprovalRequestTargetItem } from '../models/approval.model';

export class ApprovalConstants {
  static readonly ODP_APPROVAL_REQUEST_TARGET_ITEMS: ApprovalRequestTargetItem[] = [
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.LearningPlan,
      'ApprovalPage.LearningPlanTabName',
      [
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewLearningPlan',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.LearningDirection,
      'ApprovalPage.LearningDirectionTabName',
      [
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewLearningDirection',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.Nominations,
      'ApprovalPage.NominationsTabName',
      [
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewIndividualNomination',
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewGroup+OU+MassNomination',
      ]
    ),
  ];
  static readonly IDP_APPROVAL_REQUEST_TARGET_ITEMS: ApprovalRequestTargetItem[] = [
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.ClassRegistration,
      'ApprovalPage.ClassRegistrationTabName',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewClassRegistration+Withrawal+ChangeRequest',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.ClassWidthdrawal,
      'ApprovalPage.ClassWithdrawalTabName',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewClassRegistration+Withrawal+ChangeRequest',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.ClassChangeRequest,
      'ApprovalPage.ClassChangeRequestTabName',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewClassRegistration+Withrawal+ChangeRequest',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.AdhocNominations,
      'ApprovalPage.AdhocNominationsTabName',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForIndividual',
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForOthers',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.LNA,
      'ApprovalPage.LNATabName',
      ['CompetenceSpa.PendingRequests.StaffDevelopment.ReviewLNA+PDPlan']
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.PDPlan,
      'ApprovalPage.PDPlanTabName',
      ['CompetenceSpa.PendingRequests.StaffDevelopment.ReviewLNA+PDPlan']
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.SelfAssignPDO,
      'ApprovalPage.SelfAssignPDOTabName',
      ['CompetenceSpa.PendingRequests.StaffDevelopment.ReviewLNA+PDPlan']
    ),
  ];

  static readonly NOMINATION_APPROVAL_SUB_TARGET_ITEMS: ApprovalRequestTargetItem[] = [
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.NominationsLearner,
      'ApprovalPage.NominationLearner',
      [
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewIndividualNomination',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.NominationsGroup,
      'ApprovalPage.NominationGroup',
      [
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewGroup+OU+MassNomination',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.NominationDepartment,
      'ApprovalPage.NominationDepartment',
      [
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewGroup+OU+MassNomination',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.MassNomination,
      'ApprovalPage.MassNomination',
      [
        'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewGroup+OU+MassNomination',
      ]
    ),
  ];

  static readonly ADHOC_NOMINATION_APPROVAL_SUB_TARGET_ITEMS: ApprovalRequestTargetItem[] = [
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.AdhocNominationsLearner,
      'ApprovalPage.NominationLearner',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForIndividual',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.AdhocNominationsGroup,
      'ApprovalPage.NominationGroup',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForOthers',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.AdhocNominationDepartment,
      'ApprovalPage.NominationDepartment',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForOthers',
      ]
    ),
    new ApprovalRequestTargetItem(
      ApprovalTargetEnum.AdhocMassNomination,
      'ApprovalPage.MassNomination',
      [
        'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForOthers',
      ]
    ),
  ];

  static readonly NOMINATIONS_TARGET_ENUMS: ApprovalTargetEnum[] = [
    ApprovalTargetEnum.NominationsLearner,
    ApprovalTargetEnum.NominationsGroup,
    ApprovalTargetEnum.NominationDepartment,
    ApprovalTargetEnum.MassNomination,
  ];

  static readonly RESTRICT_SINGLE_SELECT_TARGET_ENUMS: ApprovalTargetEnum[] = [
    ApprovalTargetEnum.NominationsGroup,
    ApprovalTargetEnum.NominationDepartment,
    ApprovalTargetEnum.MassNomination,
    ApprovalTargetEnum.AdhocNominationsGroup,
    ApprovalTargetEnum.AdhocNominationDepartment,
    ApprovalTargetEnum.AdhocMassNomination,
  ];

  static readonly ADHOC_NOMINATIONS_TARGET_ENUMS: ApprovalTargetEnum[] = [
    ApprovalTargetEnum.AdhocNominationsLearner,
    ApprovalTargetEnum.AdhocNominationsGroup,
    ApprovalTargetEnum.AdhocNominationDepartment,
    ApprovalTargetEnum.AdhocMassNomination,
  ];

  static readonly CHANGE_NOMINATION_STATUS_TARGET_MAP: any = {
    [ChangeNominationStatusTargetEnum.Learner]:
      NominateStatusCodeEnum.PendingForApproval2nd,
    [ChangeNominationStatusTargetEnum.Group]: NominateStatusCodeEnum.Approved,
    [ChangeNominationStatusTargetEnum.Department]:
      NominateStatusCodeEnum.Approved,
    [ChangeNominationStatusTargetEnum.MassNomination]:
      NominateStatusCodeEnum.Approved,
    [ChangeNominationStatusTargetEnum.AdHocMassNomination]:
      NominateStatusCodeEnum.Approved,
  };

  static readonly APPROVAL_TARGET_ACTIONKEY_MAP: ApprovalTargetActionKeyMap = {
    [ApprovalTargetEnum.ClassRegistration]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewClassRegistration+Withrawal+ChangeRequest',
    [ApprovalTargetEnum.ClassWidthdrawal]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewClassRegistration+Withrawal+ChangeRequest',
    [ApprovalTargetEnum.ClassChangeRequest]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewClassRegistration+Withrawal+ChangeRequest',
    [ApprovalTargetEnum.LNA]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewLNA+PDPlan',
    [ApprovalTargetEnum.PDPlan]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewLNA+PDPlan',
    [ApprovalTargetEnum.SelfAssignPDO]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewLNA+PDPlan',

    // IDP Adhoc nomination
    [ApprovalTargetEnum.AdhocNominationsLearner]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForIndividual',
    [ApprovalTargetEnum.AdhocNominationsGroup]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForOthers',
    [ApprovalTargetEnum.AdhocNominationDepartment]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForOthers',
    [ApprovalTargetEnum.AdhocMassNomination]:
      'CompetenceSpa.PendingRequests.StaffDevelopment.ReviewAdhocNominationForOthers',

    //ODP
    [ApprovalTargetEnum.Nominations]:
      'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewIndividualNomination',
    [ApprovalTargetEnum.NominationsLearner]:
      'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewIndividualNomination',
    [ApprovalTargetEnum.NominationsGroup]:
      'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewGroup+OU+MassNomination',
    [ApprovalTargetEnum.NominationDepartment]:
      'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewGroup+OU+MassNomination',
    [ApprovalTargetEnum.MassNomination]:
      'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewGroup+OU+MassNomination',
    [ApprovalTargetEnum.LearningPlan]:
      'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewLearningPlan',
    [ApprovalTargetEnum.LearningDirection]:
      'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewLearningDirection',
  };
}

export class ApprovalConfirmMessage {
  // Approve
  static readonly APPROVE_REGISTRATION: string =
    'Approve selected class run registration(s)?';
  static readonly APPROVE_WITHDRAWAL: string =
    'Approve selected class run withdrawal(s)?';
  static readonly APPROVE_CHANGE_REQUEST: string =
    'Approve selected class run change request(s)?';
  static readonly APPROVE_NOMINATION: string =
    'Approve selected nomination request(s)?';
  static readonly APPROVE_AHOC_NOMINATION: string =
    'Approve selected nomination request(s)?';
  static readonly APPROVE_LEARNING_PLAN: string =
    'Approve selected learning plan request(s)?';
  static readonly APPROVE_LEARNING_DIRECTION: string =
    'Approve selected learning direction request(s)?';
  static readonly APPROVE_LEARNING_NEED: string =
    'Acknowledge selected learning need analysis request(s)?';
  static readonly APPROVE_PD_PLAN: string =
    'Acknowledge selected PD plan request(s)?';
  static readonly APPROVE_SELF_ASSIGN_PDO: string =
    'Acknowledge selected PD Opportunity request(s)?';

  // Reject
  static readonly REJECT_REGISTRATION: string =
    'Reason for rejecting selected class run registration(s)?';
  static readonly REJECT_WITHDRAWAL: string =
    'Reason for rejecting selected class run withdrawal(s)?';
  static readonly REJECT_CHANGE_REQUEST: string =
    'Reason for rejecting selected class run change request(s)?';
  static readonly REJECT_NOMINATION: string =
    'Reason for rejecting selected nomination request(s)?';
  static readonly REJECT_ADHOC_NOMINATION: string =
    'Reason for rejecting selected nomination request(s)?';
  static readonly REJECT_LEARNING_PLAN: string =
    'Reason for rejecting selected learning plan request(s)?';
  static readonly REJECT_LEARNING_DIRECTION: string =
    'Reason for rejecting selected learning direction request(s)?';
  static readonly REJECT_LEARNING_NEED: string =
    'Reason for rejecting selected learning need request(s)?';
  static readonly REJECT_PD_PLAN: string =
    'Reason for rejecting selected PD plan request(s)?';
  static readonly REJECT_SELF_ASSIGN_PDO: string =
    'Reason for rejecting selected PD Opportunity request(s)?';

  static readonly APPROVE_CONFIRM_MESSAGE_MAP: ApprovalConfirmMessageMap = {
    // IDP
    [ApprovalTargetEnum.ClassRegistration]:
      ApprovalConfirmMessage.APPROVE_REGISTRATION,
    [ApprovalTargetEnum.ClassWidthdrawal]:
      ApprovalConfirmMessage.APPROVE_WITHDRAWAL,
    [ApprovalTargetEnum.ClassChangeRequest]:
      ApprovalConfirmMessage.APPROVE_CHANGE_REQUEST,

    // IDP Adhoc nomination
    [ApprovalTargetEnum.AdhocNominationsLearner]:
      ApprovalConfirmMessage.APPROVE_AHOC_NOMINATION,
    [ApprovalTargetEnum.AdhocNominationsGroup]:
      ApprovalConfirmMessage.APPROVE_AHOC_NOMINATION,
    [ApprovalTargetEnum.AdhocNominationDepartment]:
      ApprovalConfirmMessage.APPROVE_AHOC_NOMINATION,
    [ApprovalTargetEnum.AdhocMassNomination]:
      ApprovalConfirmMessage.APPROVE_AHOC_NOMINATION,
    [ApprovalTargetEnum.LNA]: ApprovalConfirmMessage.APPROVE_LEARNING_NEED,
    [ApprovalTargetEnum.PDPlan]: ApprovalConfirmMessage.APPROVE_PD_PLAN,
    [ApprovalTargetEnum.SelfAssignPDO]:
      ApprovalConfirmMessage.APPROVE_SELF_ASSIGN_PDO,

    //ODP
    [ApprovalTargetEnum.Nominations]: ApprovalConfirmMessage.APPROVE_NOMINATION,

    [ApprovalTargetEnum.NominationsLearner]:
      ApprovalConfirmMessage.APPROVE_NOMINATION,
    [ApprovalTargetEnum.NominationsGroup]:
      ApprovalConfirmMessage.APPROVE_NOMINATION,
    [ApprovalTargetEnum.NominationDepartment]:
      ApprovalConfirmMessage.APPROVE_NOMINATION,
    [ApprovalTargetEnum.MassNomination]:
      ApprovalConfirmMessage.APPROVE_NOMINATION,

    [ApprovalTargetEnum.LearningPlan]:
      ApprovalConfirmMessage.APPROVE_LEARNING_PLAN,
    [ApprovalTargetEnum.LearningDirection]:
      ApprovalConfirmMessage.APPROVE_LEARNING_DIRECTION,
  };

  static readonly REJECT_CONFIRM_MESSAGE_MAP: ApprovalConfirmMessageMap = {
    // IDP
    [ApprovalTargetEnum.ClassRegistration]:
      ApprovalConfirmMessage.REJECT_REGISTRATION,
    [ApprovalTargetEnum.ClassWidthdrawal]:
      ApprovalConfirmMessage.REJECT_WITHDRAWAL,
    [ApprovalTargetEnum.ClassChangeRequest]:
      ApprovalConfirmMessage.REJECT_CHANGE_REQUEST,
    [ApprovalTargetEnum.LNA]: ApprovalConfirmMessage.REJECT_LEARNING_NEED,
    [ApprovalTargetEnum.PDPlan]: ApprovalConfirmMessage.REJECT_PD_PLAN,
    [ApprovalTargetEnum.SelfAssignPDO]:
      ApprovalConfirmMessage.REJECT_SELF_ASSIGN_PDO,

    // IDP Adhoc nomination
    [ApprovalTargetEnum.AdhocNominationsLearner]:
      ApprovalConfirmMessage.REJECT_ADHOC_NOMINATION,
    [ApprovalTargetEnum.AdhocNominationsGroup]:
      ApprovalConfirmMessage.REJECT_ADHOC_NOMINATION,
    [ApprovalTargetEnum.AdhocNominationDepartment]:
      ApprovalConfirmMessage.REJECT_ADHOC_NOMINATION,
    [ApprovalTargetEnum.AdhocMassNomination]:
      ApprovalConfirmMessage.REJECT_ADHOC_NOMINATION,

    //ODP
    [ApprovalTargetEnum.Nominations]: ApprovalConfirmMessage.REJECT_NOMINATION,
    [ApprovalTargetEnum.NominationsLearner]:
      ApprovalConfirmMessage.REJECT_NOMINATION,
    [ApprovalTargetEnum.NominationsGroup]:
      ApprovalConfirmMessage.REJECT_NOMINATION,
    [ApprovalTargetEnum.NominationDepartment]:
      ApprovalConfirmMessage.REJECT_NOMINATION,
    [ApprovalTargetEnum.MassNomination]:
      ApprovalConfirmMessage.REJECT_NOMINATION,
    [ApprovalTargetEnum.LearningPlan]:
      ApprovalConfirmMessage.REJECT_LEARNING_PLAN,
    [ApprovalTargetEnum.LearningDirection]:
      ApprovalConfirmMessage.REJECT_LEARNING_DIRECTION,
  };
}

type ApprovalConfirmMessageMap = Partial<
  {
    [key in ApprovalTargetEnum]: string;
  }
>;

type ApprovalTargetActionKeyMap = Partial<
  {
    [key in ApprovalTargetEnum]: string;
  }
>;
