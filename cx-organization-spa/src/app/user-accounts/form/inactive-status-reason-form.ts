// tslint:disable-next-line:variable-name
export const SuspensionReasonFormJSON = {
  elements: [
    {
      type: 'radiogroup',
      name: 'suspensionReason',
      defaultValue: 0,
      width: '100%',
      title: 'Please select one option below',
      isRequired: true,
      requiredErrorText: 'Please select a reason for suspension.',
      choices: [
        {
          value: 'Inactive_Manually_Absence',
          text: 'On leave of absence of 90 days or more'
        },
        { value: 'Inactive_Manually_Retirement', text: 'Retirement' },
        { value: 'Inactive_Manually_Resignation', text: 'Resignation' },
        { value: 'Inactive_Manually_Termination', text: 'Termination' },
        {
          value: 'Inactive_Manually_LeftWithoutNotice',
          text: 'The staff has left the service without advance notice'
        }
      ],
      colCount: ''
    },
    {
      type: 'html',
      name: 'info',
      html:
        'You are about to suspend <b>{selectedUserCount}</b> user(s) with reason <b>{suspensionReason}</b>',
      visibleIf: '{suspensionReason} notempty'
    }
  ]
};
