export const FilterFormJSON = {
  elements: [
    {
      type: 'panel',
      name: 'panelFilterOptions',
      elements: [
        {
          type: 'dropdown',
          name: 'filterOptions',
          width: '250px',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Filter Criteria is required',
          title: 'Filter Criteria',
          choicesOrder: 'asc',
          choices: [
            {
              value: {
                data: 'STATUS',
                text: 'Account Status',
              },
              text: 'Account Status',
            },
            {
              value: {
                data: 'ACCOUNT_TYPE',
                text: 'Account Type',
              },
              text: 'Account Type',
            },
            {
              value: {
                data: 'AGE_GROUP',
                text: 'Age Group',
              },
              text: 'Age Group',
            },
            {
              value: {
                data: 'APPROVAL_GROUP',
                text: 'Approving Officer',
              },
              text: 'Approving Officer',
            },
            {
              value: {
                data: 'DESIGNATION',
                text: 'Designation',
              },
              text: 'Designation',
            },
            {
              value: {
                data: 'DEVELOPMENTAL_ROLE',
                text: 'Developmental Role',
              },
              text: 'Developmental Role',
            },
            {
              value: {
                data: 'JOB_FAMILY',
                text: 'Job Family',
              },
              text: 'Job Family',
            },
            {
              value: {
                data: 'LNA_STATUS',
                text: 'LNA Status',
              },
              text: 'LNA Status',
            },
            {
              value: {
                data: 'PDP_STATUS',
                text: 'PD Plan Status',
              },
              text: 'PD Plan Status',
            },
            {
              value: {
                data: 'SERVICE_SCHEME',
                text: 'Service Scheme',
              },
              text: 'Service Scheme',
            },
            {
              value: {
                data: 'TEACHING_SUBJECTS',
                text: 'Teaching Subjects',
              },
              text: 'Teaching Subjects',
            },
            {
              value: {
                data: 'USER_GROUP',
                text: 'User Group',
              },
              text: 'User Group',
            },
            {
              value: {
                data: 'LNA_ACKNOWLEDGED_PERIOD',
                text: 'LNA Acknowledged Period',
              },
              text: 'LNA Acknowledged Period',
            },
            {
              value: {
                data: 'PD_PLAN_ACKNOWLEDGED_PERIOD',
                text: 'PD Plan Acknowledged Period',
              },
              text: 'PD Plan Acknowledged Period',
            },
          ],
        },
        {
          type: 'cxtagbox',
          name: 'status',
          visibleIf: '{filterOptions.data} = "STATUS"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'User Status is required',
          title: 'User Status',
          otherPlaceHolder: 'please choose account status',
          choices: [
            {
              value: 'Active',
              text: 'Active',
            },
            {
              value: 'New',
              text: 'New',
            },
            {
              value: 'Deactive',
              text: 'Deleted',
            },
            {
              value: 'Inactive',
              text: 'Suspended',
            },
            {
              value: 'IdentityServerLocked',
              text: 'Locked',
            },
            {
              value: 'Archived',
              text: 'Archived',
            },
          ],
        },
        {
          type: 'cxtagbox',
          name: 'ageGroup',
          visibleIf: '{filterOptions.data} = "AGE_GROUP"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Age Group is required',
          title: 'Age Group',
          otherPlaceHolder: 'Please choose to filter',
          choices: [
            {
              value: '0',
              text: '0-19',
            },
            {
              value: '20',
              text: '20-29',
            },
            {
              value: '30',
              text: '30-39',
            },
            {
              value: '40',
              text: '40-49',
            },
            {
              value: '50',
              text: '> 50',
            },
          ],
        },
        {
          type: 'cxtagbox',
          name: 'accountType',
          visibleIf: '{filterOptions.data} = "ACCOUNT_TYPE"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Account Type is required',
          title: 'Account Type',
          choices: [
            {
              value: 1,
              text: 'External user',
            },
            {
              value: 2,
              text: 'MOE user',
            },
          ],
        },
        {
          type: 'dropdown',
          name: 'personnelGroups',
          visibleIf: '{filterOptions.data} = "SERVICE_SCHEME"',
          title: 'Service Scheme',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Service Scheme is required',
          choicesByUrl: {
            url:
              '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{PDCatalogue_SERVICESCHEMES}',
            titleName: 'displayText',
          },
        },
        {
          type: 'cxtagbox',
          name: 'teachingSubjects',
          title: 'Teaching Subjects',
          storeWholeObject: true,
          keyName: 'id',
          width: '400px',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Teaching Subjects is required',
          choicesByUrl: {
            url:
              '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{PDCatalogue_TEACHING_SUBJECTS}',
            titleName: 'displayText',
          },
          choicesEnableIf: '{filterOptions.data} = "TEACHING_SUBJECTS"',
          visibleIf: '{filterOptions.data} = "TEACHING_SUBJECTS"',
        },
        {
          type: 'cxtagbox',
          name: 'jobFamily',
          title: 'Job Family',
          storeWholeObject: true,
          keyName: 'id',
          width: '400px',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Job Family is required',
          choicesByUrl: {
            url:
              `{learningCatalogApi_BaseUrl}/catalogentries/explorer/` +
              `{PDCatalogue_JOB_FAMILIES}?relatedTo=SERVICESCHEMES_EAS`,
            titleName: 'displayText',
          },
          choicesEnableIf: '{filterOptions.data} = "JOB_FAMILY"',
          visibleIf: '{filterOptions.data} = "JOB_FAMILY"',
        },
        {
          type: 'dropdown',
          name: 'developmentalRole',
          startWithNewLine: false,
          width: 'unset',
          isRequired: true,
          requiredErrorText: 'Developmental Role is required',
          title: 'Developmental Role',
          choicesByUrl: {
            url:
              '{learningCatalogApi_BaseUrl}/catalogentries/explorer/{PDCatalogue_DEVROLES}',
            titleName: 'displayText',
          },
          visibleIf: '{filterOptions.data} = "DEVELOPMENTAL_ROLE"',
        },
        {
          type: 'text',
          name: 'designation',
          visibleIf: '{filterOptions.data} = "DESIGNATION"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Designation is required',
          title: 'Designation',
        },
        {
          type: 'cxtagbox',
          name: 'lnaStatus',
          visibleIf: '{filterOptions.data} = "LNA_STATUS"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'LNA Status is required',
          title: 'LNA Status',
          choices: [],
          storeWholeObject: false,
          keyName: 'value',
        },
        {
          type: 'cxtagbox',
          name: 'pdpStatus',
          visibleIf: '{filterOptions.data} = "PDP_STATUS"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'PD Plan Status is required',
          title: 'PD Plan Status',
          choices: [],
          storeWholeObject: false,
          keyName: 'value',
        },
        {
          type: 'cxtagbox',
          name: 'userGroup',
          visibleIf: '{filterOptions.data} = "USER_GROUP"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'User Group is required',
          title: 'User Group',
          choices: [],
          storeWholeObject: false,
          keyName: 'value',
        },
        {
          type: 'cxtagbox',
          name: 'approvalGroup',
          visibleIf: `{filterOptions.data} = "APPROVAL_GROUP"`,
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Approving Officer is required',
          title: 'Approving Officer',
          choices: [],
          storeWholeObject: false,
          keyName: 'value',
        },
        {
          type: 'text',
          name: 'acknowledgedLNADateFrom',
          title: 'From Date',
          inputType: 'cxinputmask',
          isRequired: true,
          requiredErrorText: 'From Date is required',
          visibleIf: `{filterOptions.data} = "LNA_ACKNOWLEDGED_PERIOD"`,
          config: {
            alias: 'datetime',
            inputFormat: 'dd/mm/yyyy',
            placeholder: 'dd/mm/yyyy',
          },
          datePickerConfig: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-10:c+20',
            dateFormat: 'dd/mm/yy',
          },
        },
        {
          type: 'text',
          name: 'acknowledgedLNADateTo',
          title: 'To Date',
          inputType: 'cxinputmask',
          isRequired: true,
          startWithNewLine: false,
          requiredErrorText: 'From Date is required',
          visibleIf: `{filterOptions.data} = "LNA_ACKNOWLEDGED_PERIOD"`,
          config: {
            alias: 'datetime',
            inputFormat: 'dd/mm/yyyy',
            placeholder: 'dd/mm/yyyy',
          },
          datePickerConfig: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-10:c+20',
            dateFormat: 'dd/mm/yy',
          },
          validators: [
            {
              type: 'expression',
              text: 'To date should be after from date.',
              expression:
                '{acknowledgedLNADateFrom} notempty and compareDates({acknowledgedLNADateTo},{acknowledgedLNADateFrom})',
            },
          ],
        },
        {
          type: 'text',
          name: 'acknowledgedPDPlanDateFrom',
          title: 'From Date',
          inputType: 'cxinputmask',
          isRequired: true,
          requiredErrorText: 'From Date is required',
          visibleIf: `{filterOptions.data} = "PD_PLAN_ACKNOWLEDGED_PERIOD"`,
          config: {
            alias: 'datetime',
            inputFormat: 'dd/mm/yyyy',
            placeholder: 'dd/mm/yyyy',
          },
          datePickerConfig: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-10:c+20',
            dateFormat: 'dd/mm/yy',
          },
        },
        {
          type: 'text',
          name: 'acknowledgedPDPlanDateTo',
          title: 'To Date',
          startWithNewLine: false,
          isRequired: true,
          inputType: 'cxinputmask',
          requiredErrorText: 'To Date is required',
          visibleIf: `{filterOptions.data} = "PD_PLAN_ACKNOWLEDGED_PERIOD"`,
          config: {
            alias: 'datetime',
            inputFormat: 'dd/mm/yyyy',
            placeholder: 'dd/mm/yyyy',
          },
          datePickerConfig: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-10:c+20',
            dateFormat: 'dd/mm/yy',
          },
          validators: [
            {
              type: 'expression',
              text: 'To date should be after from date.',
              expression:
                '{acknowledgedPDPlanDateFrom} notempty and compareDates({acknowledgedPDPlanDateTo},{acknowledgedPDPlanDateFrom})',
            },
          ],
        },
      ],
    },
  ],
};

// tslint:disable-next-line:variable-name
export const AppliedFilterFormJSON = {
  elements: [
    {
      type: 'paneldynamic',
      name: 'arrayAppliedFilter',
      title: 'Current Filters',
      valueName: 'appliedFilter',
      allowAddPanel: false,
      renderMode: 'list',
      templateElements: [
        {
          type: 'html',
          name: 'info',
          html: `<div style="display: flex; padding-right: 100px;">
                                <div style="width: 40%">{panel.filterOptions.text}</div>
                                <div style="width: 10%; text-align: center;">is</div>
                                <div style="width: 40%; text-align: right;">
                                    {panel.data.text}
                                </div>
                            </div>`,
        },
      ],
    },
  ],
};
