// tslint:disable:variable-name
export const ExportFormJSON = {
  elements: [
    {
      type: 'checkbox',
      name: 'fieldsExport',
      title: 'EXPORT FIELDS',
      isRequired: true,
      hasSelectAll: true,
      selectAllText: 'Select all',
      requiredErrorText: 'Please select at least one criterion',
      defaultValue: [
        'FirstName',
        'SystemRoles',
        'statusId',
        'Gender',
        'EmailAddress',
        'PersonnelGroups',
        'LearningFramework',
        'designation',
        'ExpirationDate',
        'Created',
        'organizationhierarchy',
        'organizationtypes'
      ],
      choices: [
        {
          value: 'FirstName',
          text: 'Full Name'
        },
        {
          value: 'SystemRoles',
          text: 'System Role'
        },
        {
          value: 'statusId',
          text: 'Account Application Report'
        },
        {
          value: 'Gender',
          text: 'Gender'
        },
        {
          value: 'EmailAddress',
          text: 'Email Address'
        },
        {
          value: 'PersonnelGroups',
          text: 'Service Scheme'
        },
        {
          value: 'LearningFramework',
          text: 'Learning Framework'
        },
        {
          value: 'designation',
          text: 'Designation'
        },
        {
          value: 'ExpirationDate',
          text: 'Expiration Date'
        },
        {
          value: 'Created',
          text: 'Created Date'
        },
        {
          value: 'organizationhierarchy',
          text: 'Organisation Hierarchy'
        },
        {
          value: 'organizationtypes',
          text: 'Type of Organisation Unit'
        }
      ]
    },
    {
      type: 'html',
      name: 'warning',
      html: 'You have selected <b>{selectedUserCount}</b> user(s)',
      visibleIf: '{selectedUserCount} notempty'
    },
    {
      type: 'checkbox',
      name: 'includeSubDepartment',
      titleLocation: 'hidden',
      choices: [
        {
          value: 'true',
          text: 'Include sub department'
        }
      ],
      visibleIf: '{selectedUserCount} empty'
    }
  ]
};

export const ExportFields = [
  {
    value: 'FirstName',
    text: 'Full Name'
  },
  {
    value: 'SystemRoles',
    text: 'System Role'
  },
  {
    value: 'statusId',
    text: 'Account Application Report'
  },
  {
    value: 'Gender',
    text: 'Gender'
  },
  {
    value: 'EmailAddress',
    text: 'Email Address'
  },
  {
    value: 'PersonnelGroups',
    text: 'Service Scheme'
  },
  {
    value: 'LearningFramework',
    text: 'Learning Framework'
  },
  {
    value: 'designation',
    text: 'Designation'
  },
  {
    value: 'ExpirationDate',
    text: 'Expiration Date'
  },
  {
    value: 'Created',
    text: 'Created Date'
  },
  {
    value: 'organizationhierarchy',
    text: 'Organisation Hierarchy'
  },
  {
    value: 'organizationtypes',
    text: 'Type of Organisation Unit'
  }
];
