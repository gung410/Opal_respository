// tslint:disable:quotemark
// tslint:disable-next-line:no-unused-expression
// tslint:disable:object-literal-key-quotes
export const CACULATE_COST_FORM = {
  pages: [
    {
      name: 'Training Cost',
      elements: [
        {
          type: 'panel',
          name: 'pdoPlannedCost',
          elements: [
            {
              type: 'text',
              name: 'numOfOfficers_Planned',
              title: 'No. of Officers',
              inputType: 'cxinputmask',
              isRequired: true,
              config: {
                alias: 'decimal',
                allowMinus: false,
                digits: 2,
              },
              validators: [
                {
                  type: 'numeric',
                  text: 'The number of officers must be greater than 0.',
                  minValue: 1,
                },
              ],
            },
            {
              type: 'text',
              name: 'numOfHours_Planned',
              inputType: 'cxinputmask',
              title: 'No. of training hours per Pax',
              enableIf: '{isExternalPDO} == true',
              config: {
                alias: 'decimal',
                allowMinus: false,
                digits: 2,
              },
              validators: [
                {
                  type: 'numeric',
                  text: 'The number of training hours must be greater than 0.',
                  minValue: 1,
                },
              ],
            },
            {
              type: 'text',
              name: 'costPerPax_Planned',
              title: 'Cost per Pax',
              inputType: 'cxinputmask',
              enableIf: '{isExternalPDO} == true',
              isRequired: true,
              config: {
                alias: 'decimal',
                groupSeparator: ',',
                autoGroup: true,
                autoUnmask: true,
                allowMinus: false,
                digits: 2,
                digitsOptional: false,
                prefix: 'SGD ',
                placeholder: '0',
              },
              validators: [
                {
                  type: 'numeric',
                  text: 'The cost must equal or more than 0.',
                  minValue: 0,
                },
              ],
            },

            {
              type: 'expression',
              name: 'totalHours_Planned',
              title: 'Total training hours',
              expression: '{numOfHours_Planned} * {numOfOfficers_Planned}',
              displayStyle: 'decimal',
              currency: 'SGD',
              commentText: 'Other (describe)',
            },
            {
              type: 'expression',
              name: 'totalCost_Planned',
              title: 'Total Cost',
              expression: '{costPerPax_Planned} * {numOfOfficers_Planned}',
              displayStyle: 'currency',
              currency: 'SGD',
              commentText: 'Other (describe)',
            },
          ],
        },
        {
          type: 'panel',
          name: 'pdoActualCost',
          startWithNewLine: false,
          visibleIf: '{isCoursePadPDO} == true',
          elements: [
            {
              type: 'text',
              name: 'numOfOfficers_Actual',
              title: 'No. of Officers completed the course',
              inputType: 'cxinputmask',
              config: {
                alias: 'decimal',
                allowMinus: false,
                digits: 2,
              },
              readOnly: true,
              validators: [
                {
                  type: 'numeric',
                  text: 'The number of officers must be greater than 0.',
                  minValue: 1,
                },
              ],
            },
            {
              type: 'text',
              name: 'numOfHours_Actual',
              inputType: 'cxinputmask',
              title: 'No. of training hours per Pax',
              config: {
                alias: 'decimal',
                allowMinus: false,
                digits: 2,
              },
              readOnly: true,
              validators: [
                {
                  type: 'numeric',
                  text: 'The number of training hours must be greater than 0.',
                  minValue: 1,
                },
              ],
            },
            {
              type: 'text',
              name: 'costPerPax_Actual',
              title: 'Cost per Pax',
              inputType: 'cxinputmask',
              config: {
                alias: 'decimal',
                groupSeparator: ',',
                autoGroup: true,
                autoUnmask: true,
                allowMinus: false,
                digits: 2,
                digitsOptional: false,
                prefix: 'SGD ',
                placeholder: '0',
              },
              readOnly: true,
              validators: [
                {
                  type: 'numeric',
                  text: 'The cost must equal or more than 0.',
                  minValue: 0,
                },
              ],
            },
            {
              type: 'expression',
              name: 'totalHours_Actual',
              title: 'Total training hours',
              expression: '{numOfHours_Actual} * {numOfOfficers_Actual}',
              displayStyle: 'decimal',
              currency: 'SGD',
              commentText: 'Other (describe)',
            },
            {
              type: 'expression',
              name: 'totalCost_Actual',
              title: 'Total Cost',
              expression: '{costPerPax_Actual} * {numOfOfficers_Actual}',
              displayStyle: 'currency',
              currency: 'SGD',
              commentText: 'Other (describe)',
            },
          ],
        },
      ],
    },
  ],
  showQuestionNumbers: 'off',
  mode: 'display',
};
