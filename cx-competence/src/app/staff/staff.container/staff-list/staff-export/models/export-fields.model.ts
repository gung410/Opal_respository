export const ExportFormJSON = {
  elements: [
    {
      type: 'checkbox',
      name: 'fieldsExport',
      title: 'Export Fields',
      isRequired: true,
      hasSelectAll: true,
      selectAllText: 'Select all',
      requiredErrorText: 'Export Fields is required',
      defaultValue: [
        'FirstName',
        'Department.name',
        'PersonnelGroups[*].name',
        'AssessmentInfos.LearningNeed.completionRate',
        'AssessmentInfos.LearningNeed.statusInfo.assessmentStatusName',
        'AssessmentInfos.LearningPlan.statusInfo.assessmentStatusName',
        'EntityStatus.statusId',
        'UserGroupInfos[*].name',
      ],
      choices: [
        {
          value: 'FirstName',
          text: 'Full Name',
        },
        {
          value: 'Department.name',
          text: 'Organisation Unit',
        },
        {
          value: 'PersonnelGroups[*].name',
          text: 'Service Scheme',
        },
        {
          value: 'AssessmentInfos.LearningNeed.completionRate',
          text: 'LNA Completion Rate',
        },
        {
          value: 'AssessmentInfos.LearningNeed.statusInfo.assessmentStatusName',
          text: 'LNA Status',
        },
        {
          value: 'AssessmentInfos.LearningPlan.statusInfo.assessmentStatusName',
          text: 'PD Plan Status',
        },
        {
          value: 'EntityStatus.statusId',
          text: 'Account Status',
        },
        {
          value: 'UserGroupInfos[*].name',
          text: 'Approving Officers',
        },
      ],
    },
    {
      type: 'html',
      name: 'warning',
      html: 'You are selected <b>{selectedUserCount}</b> user(s)',
      visibleIf: '{selectedUserCount} notempty',
    },
    {
      type: 'checkbox',
      name: 'includeSubDepartment',
      titleLocation: 'hidden',
      choices: [
        {
          value: true,
          text: 'Include sub organisation unit',
        },
      ],
      visibleIf: '{selectedUserCount} empty',
    },
  ],
};

export const ExportFields = [
  {
    value: 'FirstName',
    text: 'Full Name',
  },
  {
    value: 'Department.name',
    text: 'Organisation Unit',
  },
  {
    value: 'PersonnelGroups[*].name',
    text: 'Service Scheme',
  },
  {
    value: 'AssessmentInfos.LearningNeed.completionRate',
    text: 'LNA Completion Rate',
  },
  {
    value: 'AssessmentInfos.LearningNeed.statusInfo.assessmentStatusName',
    text: 'LNA Status',
  },
  {
    value: 'AssessmentInfos.LearningPlan.statusInfo.assessmentStatusName',
    text: 'PD Plan Status',
  },
  {
    value: 'EntityStatus.statusId',
    text: 'Account Status',
  },
  {
    value: 'UserGroupInfos[*].name',
    text: 'Approving Officers',
  },
];
