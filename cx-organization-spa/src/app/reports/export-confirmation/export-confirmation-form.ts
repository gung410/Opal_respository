export const exportConfirmationFormJSON = {
  elements: [
    {
      type: 'panel',
      name: 'processQuicklyPanel',
      visibleIf: '{isLongProcess} = false',
      elements: [
        {
          type: 'html',
          name: 'askingConfirmation',
          html: 'It will take a moment to generate. Would you like to wait?'
        },
        {
          type: 'radiogroup',
          name: 'downloadDirectly',
          titleLocation: 'hidden',
          isRequired: true,
          defaultValue: 'yes',
          choices: [
            {
              value: 'yes',
              text: 'Yes, let me download right away.'
            },
            {
              value: 'no',
              text: 'No, send me an email.'
            }
          ]
        }
      ]
    },
    {
      type: 'panel',
      name: 'processLongPanel',
      visibleIf: '{isLongProcess} = true',
      elements: [
        {
          type: 'html',
          name: 'guidanceForLongProcess',
          html:
            'It will take some minutes to generate. You will receive email when the download file is ready. Do you want to continue?'
        }
      ]
    }
  ]
};
