// tslint:disable:max-line-length
export const LEARNING_AREA_SELECTOR_FORM = {
  pages: [
    {
      elements: [
        {
          type: 'panel',
          visibleIf: '{parentObject} notempty',
          elements: [
            {
              type: 'dropdown',
              name: 'learningFrameWorkByServiceSchemes',
              title: 'Learning Framework',
              isRequired: true,
              requiredErrorText: 'Learning Framework is required',
              choicesOrder: 'asc',
              choicesByUrl: {
                url:
                  '{learningCatalogApi_BaseUrl}/catalogentries/explorer/1822d1fa-b42c-11e9-b3e6-0242ac120004?relatedToStr={parentObject.personnelGroupIds}',
                titleName: 'displayText',
              },
            },
            {
              type: 'dropdown',
              name: 'learningDimensionByServiceSchemes',
              title: 'Learning Dimension',
              isRequired: true,
              requiredErrorText: 'Learning Dimension is required',
              visibleIf: '{learningFrameWorkByServiceSchemes} notempty',
              choicesOrder: 'asc',
              choicesByUrl: {
                url:
                  '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{learningFrameWorkByServiceSchemes.id}?entryTypes=Learning%20Dimension',
                titleName: 'displayText',
              },
            },
            {
              type: 'checkbox',
              name: 'listLearningAreaByServiceSchemes',
              valueName: 'listLearningArea',
              title: 'Learning Areas',
              isRequired: true,
              requiredErrorText: 'Learning Area is required',
              visibleIf:
                '{learningDimensionByServiceSchemes} notempty and {learningFrameWorkByServiceSchemes} notempty',
              choicesOrder: 'asc',
              choicesByUrl: {
                url:
                  '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{learningDimensionByServiceSchemes.id}',
                titleName: 'displayText',
              },
            },
          ],
        },
        {
          type: 'panel',
          visibleIf: '{parentObject} empty',
          elements: [
            {
              type: 'dropdown',
              name: 'allLearningFrameWork',
              title: 'Learning Framework',
              isRequired: true,
              requiredErrorText: 'Learning Framework is required',
              choicesOrder: 'asc',
              choicesByUrl: {
                url:
                  '{learningCatalogApi_BaseUrl}/catalogentries/explorer/1822d1fa-b42c-11e9-b3e6-0242ac120004',
                titleName: 'displayText',
              },
            },
            {
              type: 'dropdown',
              name: 'learningDimensionByAllLearningFramework',
              title: 'Learning Dimension',
              isRequired: true,
              requiredErrorText: 'Learning Dimension is required',
              visibleIf: '{allLearningFrameWork} notempty',
              choicesOrder: 'asc',
              choicesByUrl: {
                url:
                  '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{allLearningFrameWork.id}?entryTypes=Learning%20Dimension',
                titleName: 'displayText',
              },
            },
            {
              type: 'checkbox',
              name: 'listLearningAreaByAllLearningFramework',
              valueName: 'listLearningArea',
              title: 'Learning Areas',
              isRequired: true,
              requiredErrorText: 'Learning Area is required',
              visibleIf:
                '{learningDimensionByAllLearningFramework} notempty and {allLearningFrameWork} notempty',
              choicesOrder: 'asc',
              choicesByUrl: {
                url:
                  '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{learningDimensionByAllLearningFramework.id}',
                titleName: 'displayText',
              },
            },
          ],
        },
      ],
    },
  ],
  showQuestionNumbers: 'off',
};
