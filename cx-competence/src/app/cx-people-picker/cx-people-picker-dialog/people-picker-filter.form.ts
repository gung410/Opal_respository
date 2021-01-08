export const peoplePickerFilterFormJSON = {
  elements: [
    {
      type: 'text',
      name: 'searchKey',
      title: 'Learners',
      placeHolder: 'Search learners by name or email...',
    },
    {
      type: 'cxtagbox',
      name: 'serviceSchemes',
      visibleIf: '{supportFilterByServiceScheme} = true',
      title: 'Service Schemes',
      startWithNewLine: true,
      choicesByUrl: {
        url:
          '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{PDCatalogue_SERVICESCHEMES}',
        valueName: 'id',
        titleName: 'displayText',
      },
    },
  ],
};
