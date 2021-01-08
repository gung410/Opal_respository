export const approveUserFormJSON = {
  focusFirstQuestionAutomatic: false,
  showNavigationButtons: 'none',
  pages: [
    {
      name: 'basic',
      elements: [
        {
          name: 'activeDate',
          type: 'datepicker',
          inputType: 'date',
          title: 'Account Active From *',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c:c+20',
            minDate: '+0d'
          },
          width: '50%'
        },
        {
          name: 'expirationDate',
          type: 'datepicker',
          inputType: 'date',
          title: 'Date of Expiry of Account *',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          startWithNewLine: false,
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-50:c+50',
            minDate: '+0d'
          },
          width: '50%',
          validators: [
            {
              type: 'expression',
              text:
                'Date of Expiry of Account should be after Account Active From date.',
              expression:
                '{activeDate} empty or compareDates({expirationDate},{activeDate})'
            }
          ]
        },
        {
          type: 'radiogroup',
          name: 'setDateOption',
          title:
            'Would you like to set above information for selected user account(s)?',
          defaultValue: 'forSelectedUsers',
          visibleIf: `{isProcessingSingleUser} !== true`,
          choices: [
            {
              value: 'forSelectedUsers',
              text: 'Yes, set for selected user account(s).'
            },
            {
              value: 'forMissingInformation',
              text: 'No, only set for those not having information.'
            }
          ]
        }
      ]
    }
  ]
};
