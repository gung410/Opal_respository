const maxNumberOfUsers = 10;

export const AssignLearningNeedsAnalysisForm = {
  elements: [
    {
      type: 'text',
      name: 'dueDate',
      title: 'Complete by',
      inputType: 'cxinputmask',
      width: '170',
      isRequired: true,
      useDisplayValuesInTitle: false,
      requiredErrorText: 'Complete by is required',
      config: {
        alias: 'datetime',
        inputFormat: 'dd/mm/yyyy',
        placeholder: 'dd/mm/yyyy',
      },
      datePickerConfig: {
        changeMonth: true,
        changeYear: true,
        yearRange: 'c:c+20',
        minDate: '+0d',
        dateFormat: 'dd/mm/yy',
      },
    },
    {
      type: 'paneldynamic',
      renderMode: 'list',
      allowAddPanel: false,
      allowRemovePanel: false,
      name: 'arrray_user_info',
      title: 'Assign to below employee(s)',
      valueName: 'unassignedLNAUsers',
      visibleIf: `{unassignedLNAUsers} notempty and {unassignedLNAUsersCount} <= ${maxNumberOfUsers}`,
      templateElements: [
        {
          type: 'html',
          html: `<div class="list-item">{panel.fullName}</div>`,
        },
      ],
    },
    {
      type: 'html',
      name: 'assignToManyUsers',
      html:
        '<h5>Assign Learning Needs Analysis to {unassignedLNAUsersCount} employees</h5>',
      visibleIf: `{unassignedLNAUsersCount} > ${maxNumberOfUsers}`,
    },
    {
      type: 'html',
      name: 'noUnassignedUserText',
      html: '<h5>No unassigned employees</h5>',
      visibleIf: '{unassignedLNAUsers} empty',
    },
    {
      type: 'paneldynamic',
      renderMode: 'list',
      allowAddPanel: false,
      allowRemovePanel: false,
      name: 'arrray_user_info',
      title: 'Below employee(s) will be set the new complete date',
      valueName: 'expiredLNAUsers',
      visibleIf: '{expiredLNAUsers} notempty',
      templateElements: [
        {
          type: 'html',
          html: `<div class="list-item">{panel.fullName}</div>`,
        },
      ],
    },
  ],
};
