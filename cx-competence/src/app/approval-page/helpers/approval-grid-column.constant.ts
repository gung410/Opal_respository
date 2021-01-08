import { ColDef } from '@ag-grid-community/core';
import { ApprovalTargetEnum } from '../models/approval.enum';

export class ApprovalColumnDefConstants {
  static readonly CLASS_REGISTRATION: ColDef[] = [
    {
      headerName: 'attendee',
      field: 'learner',
      cellRenderer: 'learnerCellRenderer',
      lockPosition: true,
      minWidth: 320,
    },
    {
      headerName: 'course',
      field: 'course',
      cellRenderer: 'courseCellRenderer',
      lockPosition: true,
      minWidth: 310,
    },
    {
      headerName: 'class run',
      field: 'classRun',
      cellRenderer: 'classRunCellRenderer',
      lockPosition: true,
      minWidth: 220,
    },
    {
      headerName: 'application date',
      field: 'registrationDate',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: 'status',
      field: 'registrationStatus',
      cellRenderer: 'statusCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
  ];

  static readonly CLASS_WITHDRAWAL: ColDef[] = [
    {
      headerName: 'attendee',
      field: 'learner',
      cellRenderer: 'learnerCellRenderer',
      lockPosition: true,
      minWidth: 320,
    },
    {
      headerName: 'course',
      field: 'course',
      cellRenderer: 'courseCellRenderer',
      lockPosition: true,
      minWidth: 310,
    },
    {
      headerName: 'class run',
      field: 'classRun',
      cellRenderer: 'classRunCellRenderer',
      lockPosition: true,
      minWidth: 220,
    },
    {
      headerName: 'submit date',
      field: 'withdrawalRequestDate',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: 'reason',
      field: 'reason',
      cellRenderer: 'reasonCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: 'status',
      field: 'withdrawalStatus',
      cellRenderer: 'statusCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
  ];

  static readonly CLASS_CHANGE_REQUEST: ColDef[] = [
    {
      headerName: 'attendee',
      field: 'learner',
      cellRenderer: 'learnerCellRenderer',
      lockPosition: true,
      minWidth: 320,
    },
    {
      headerName: 'course',
      field: 'course',
      cellRenderer: 'courseCellRenderer',
      lockPosition: true,
      minWidth: 310,
    },
    {
      headerName: 'old class run',
      field: 'classRun',
      cellRenderer: 'classRunCellRenderer',
      lockPosition: true,
      minWidth: 220,
    },

    {
      headerName: 'new class run',
      field: 'classRunChange',
      cellRenderer: 'classRunCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: 'submit date',
      field: 'classRunChangeRequestDate',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: 'reason',
      field: 'reason',
      cellRenderer: 'reasonCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: 'status',
      field: 'classRunChangeStatus',
      cellRenderer: 'statusCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
  ];

  static readonly NOMINATION_LEARNER: ColDef[] = [
    {
      headerName: 'attendee',
      field: 'learner',
      cellRenderer: 'learnerCellRenderer',
      lockPosition: true,
      minWidth: 320,
    },
    {
      headerName: 'course',
      field: 'course',
      cellRenderer: 'courseCellRenderer',
      lockPosition: true,
      minWidth: 310,
    },
    {
      headerName: 'class run',
      field: 'classRun',
      cellRenderer: 'classRunCellRenderer',
      lockPosition: true,
      minWidth: 220,
    },
    {
      headerName: 'nomination date',
      field: 'createdAt',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
  ];

  static readonly NOMINATION_GROUP: ColDef[] = [
    {
      headerName: 'group name',
      field: 'group',
      cellRenderer: 'groupCellRenderer',
      lockPosition: true,
      minWidth: 320,
    },
    {
      headerName: 'course',
      field: 'course',
      cellRenderer: 'courseCellRenderer',
      lockPosition: true,
      minWidth: 310,
    },
    {
      headerName: 'class run',
      field: 'classRun',
      cellRenderer: 'classRunCellRenderer',
      lockPosition: true,
      minWidth: 220,
    },
    {
      headerName: 'nomination date',
      field: 'createdAt',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: '',
      field: 'additionalProperties',
      cellRenderer: 'actionGroupRenderer',
      lockPosition: true,
      minWidth: 150,
    },
  ];

  static readonly NOMINATION_DEPARTMENT: ColDef[] = [
    {
      headerName: 'department name',
      field: 'department',
      cellRenderer: 'departmentCellRenderer',
      lockPosition: true,
      minWidth: 320,
    },
    {
      headerName: 'course',
      field: 'course',
      cellRenderer: 'courseCellRenderer',
      lockPosition: true,
      minWidth: 310,
    },
    {
      headerName: 'class run',
      field: 'classRun',
      cellRenderer: 'classRunCellRenderer',
      lockPosition: true,
      minWidth: 220,
    },
    {
      headerName: 'nomination date',
      field: 'createdAt',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: '',
      field: 'additionalProperties',
      cellRenderer: 'actionGroupRenderer',
      lockPosition: true,
      minWidth: 150,
    },
  ];

  static readonly MASS_NOMINATION: ColDef[] = [
    {
      headerName: 'file name',
      field: 'massNominate',
      cellRenderer: 'massNominationCellRenderer',
      minWidth: 400,
      lockPosition: true,
    },
    {
      headerName: 'nominated by',
      field: 'email',
      minWidth: 150,
      lockPosition: true,
    },
    {
      headerName: 'nomination date',
      field: 'created',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 100,
    },
    {
      headerName: 'no. of registers',
      field: 'numberOfRegisters',
      colId: 'numberOfRegisters',
      lockPosition: true,
      minWidth: 50,
      checkboxSelection: false,
    },
    {
      headerName: '',
      field: 'additionalProperties',
      cellRenderer: 'actionGroupRenderer',
      lockPosition: true,
      minWidth: 50,
    },
  ];

  static readonly LEARNING_NEED: ColDef[] = [
    {
      headerName: 'name',
      field: 'learner',
      cellRenderer: 'learnerCellRenderer',
      lockPosition: true,
      sortable: true,
      sort: 'asc',
      hide: false,
      width: 350,
      minWidth: 280,
      cellClass: 'text-overflow-overlay',
    },
    {
      headerName: 'LNA status',
      field: 'LNAStatus',
      cellRenderer: 'LNAStatusRenderer',
      lockPosition: true,
      colId: 'LearningNeedStatusType',
      minWidth: 220,
      maxWidth: 250,
      hide: false,
      sortable: true,
      headerClass: 'grid-header-centered',
      cellClass: 'grid-cell-centered',
    },
    {
      headerName: '',
      field: 'learnerDetailUrl',
      cellRenderer: 'linkViewButtonRenderer',
      lockPosition: true,
      colId: 'learnerDetailUrl',
      minWidth: 60,
      suppressSizeToFit: true,
      width: 100,
    },
  ];

  static readonly LEARNING_PD_PLAN: ColDef[] = [
    {
      headerName: 'name',
      field: 'learner',
      cellRenderer: 'learnerCellRenderer',
      lockPosition: true,
      sortable: true,
      sort: 'asc',
      hide: false,
      width: 350,
      minWidth: 280,
      cellClass: 'text-overflow-overlay',
    },
    {
      headerName: 'PD plan status',
      field: 'PdPlanStatus',
      cellRenderer: 'PDPStatusRenderer',
      lockPosition: true,
      colId: 'LearningPlanStatusType',
      minWidth: 220,
      maxWidth: 250,
      hide: false,
      sortable: true,
      headerClass: 'grid-header-centered',
      cellClass: 'grid-cell-centered',
    },
    {
      headerName: '',
      field: 'learnerDetailUrl',
      cellRenderer: 'linkViewButtonRenderer',
      lockPosition: true,
      colId: 'learnerDetailUrl',
      minWidth: 60,
      suppressSizeToFit: true,
      width: 100,
    },
  ];

  /// ODP
  static readonly LEARNING_PLAN: ColDef[] = [
    {
      headerName: 'title',
      field: 'name',
      cellRenderer: 'learningPlanNameRenderer',
      lockPosition: true,
      colId: 'name',
      minWidth: 500,
    },
    {
      headerName: 'period',
      field: 'period',
      colId: 'period',
      lockPosition: true,
      minWidth: 150,
    },
    {
      headerName: 'status',
      field: 'status',
      colId: 'status',
      lockPosition: true,
      minWidth: 150,
    },
  ];

  static readonly LEARNING_DIRECTON: ColDef[] = [
    {
      headerName: 'title',
      field: 'name',
      cellRenderer: 'learningPlanNameRenderer',
      lockPosition: true,
      colId: 'name',
      minWidth: 500,
    },
    {
      headerName: 'period',
      field: 'period',
      colId: 'period',
      lockPosition: true,
      minWidth: 150,
    },
    {
      headerName: 'status',
      field: 'status',
      colId: 'status',
      lockPosition: true,
      minWidth: 150,
    },
  ];

  static readonly SELF_ASSIGN_PDO: ColDef[] = [
    {
      headerName: 'attendee',
      field: 'learner',
      cellRenderer: 'learnerCellRenderer',
      lockPosition: true,
      minWidth: 320,
    },
    {
      headerName: 'external course',
      field: 'course',
      cellRenderer: 'courseCellRenderer',
      lockPosition: true,
      minWidth: 310,
    },
    {
      headerName: 'status',
      field: 'status',
      cellRenderer: 'assessmentStatusCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
    {
      headerName: 'added date',
      field: 'createdAt',
      cellRenderer: 'applicationDateCellRenderer',
      lockPosition: true,
      minWidth: 200,
    },
  ];

  static readonly COL_DEFS_MAP: GridColumsDefMap = {
    // IDP
    [ApprovalTargetEnum.ClassRegistration]:
      ApprovalColumnDefConstants.CLASS_REGISTRATION,
    [ApprovalTargetEnum.ClassWidthdrawal]:
      ApprovalColumnDefConstants.CLASS_WITHDRAWAL,
    [ApprovalTargetEnum.ClassChangeRequest]:
      ApprovalColumnDefConstants.CLASS_CHANGE_REQUEST,
    [ApprovalTargetEnum.AdhocNominations]:
      ApprovalColumnDefConstants.NOMINATION_LEARNER,
    [ApprovalTargetEnum.AdhocNominationsLearner]:
      ApprovalColumnDefConstants.NOMINATION_LEARNER,
    [ApprovalTargetEnum.AdhocNominationsGroup]:
      ApprovalColumnDefConstants.NOMINATION_GROUP,
    [ApprovalTargetEnum.AdhocNominationDepartment]:
      ApprovalColumnDefConstants.NOMINATION_DEPARTMENT,
    [ApprovalTargetEnum.AdhocMassNomination]:
      ApprovalColumnDefConstants.MASS_NOMINATION,
    [ApprovalTargetEnum.LNA]: ApprovalColumnDefConstants.LEARNING_NEED,
    [ApprovalTargetEnum.PDPlan]: ApprovalColumnDefConstants.LEARNING_PD_PLAN,
    [ApprovalTargetEnum.SelfAssignPDO]:
      ApprovalColumnDefConstants.SELF_ASSIGN_PDO,

    // ODP
    [ApprovalTargetEnum.Nominations]:
      ApprovalColumnDefConstants.NOMINATION_LEARNER,
    [ApprovalTargetEnum.NominationsLearner]:
      ApprovalColumnDefConstants.NOMINATION_LEARNER,
    [ApprovalTargetEnum.NominationsGroup]:
      ApprovalColumnDefConstants.NOMINATION_GROUP,
    [ApprovalTargetEnum.NominationDepartment]:
      ApprovalColumnDefConstants.NOMINATION_DEPARTMENT,
    [ApprovalTargetEnum.MassNomination]:
      ApprovalColumnDefConstants.MASS_NOMINATION,
    [ApprovalTargetEnum.LearningPlan]: ApprovalColumnDefConstants.LEARNING_PLAN,
    [ApprovalTargetEnum.LearningDirection]:
      ApprovalColumnDefConstants.LEARNING_DIRECTON,
  };
}

type GridColumsDefMap = Partial<
  {
    [key in ApprovalTargetEnum]: ColDef[];
  }
>;
