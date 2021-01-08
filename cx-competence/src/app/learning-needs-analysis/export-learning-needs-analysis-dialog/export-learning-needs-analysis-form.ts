export const exportLearningNeedsAnalysisFormJSON = {
  elements: [
    {
      type: 'panel',
      name: 'shortListPanel',
      visibleIf: '{isLongList} = false',
      elements: [
        {
          type: 'html',
          name: 'askingConfirmation',
          html: 'It will take a moment to generate. Would you like to wait?',
        },
        {
          type: 'radiogroup',
          name: 'receiveMethod',
          titleLocation: 'hidden',
          isRequired: true,
          defaultValue: 'yes',
          choices: [
            {
              value: 'yes',
              text: 'Yes, let me download right away.',
            },
            {
              value: 'no',
              text: 'No, send me an email.',
            },
          ],
        },
      ],
    },
    {
      type: 'panel',
      name: 'longListPanel',
      visibleIf: '{isLongList} = true',
      elements: [
        {
          type: 'html',
          name: 'guidanceForLongList',
          html:
            'It will take some minutes to generate. You will receive email when the download file is ready. Do you want to continue?',
        },
      ],
    },
  ],
};
