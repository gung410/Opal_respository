import {
  organizationUnitLevelIdsConst,
  organizationUnitLevelsConst
} from 'app/department-hierarchical/models/department-level.model';
import { AppConstant } from 'app/shared/app.constant';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';

// tslint:disable:variable-name
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
          requiredErrorText: 'Please select a filter criteria.',
          title: 'Filter Criteria',
          choices: [
            {
              value: {
                data: 'STATUS',
                text: 'Account Status'
              },
              text: 'Account Status'
            },
            {
              value: {
                data: 'ACCOUNT_TYPE',
                text: 'Account Type'
              },
              text: 'Account Type'
            },
            {
              value: {
                data: 'AGE_GROUP',
                text: 'Age Group'
              },
              text: 'Age Group'
            },
            {
              value: {
                data: 'CREATION_DATE',
                text: 'Creation Date'
              },
              text: 'Creation Date'
            },
            {
              value: {
                data: 'DESIGNATION',
                text: 'Designation'
              },
              text: 'Designation'
            },
            {
              value: {
                data: 'DEVELOPMENTAL_ROLE',
                text: 'Developmental Role'
              },
              text: 'Developmental Role'
            },
            {
              value: {
                data: 'EXPIRATION_DATE',
                text: 'Expiration Date'
              },
              text: 'Expiration Date'
            },
            {
              value: {
                data: 'JOB_FAMILY',
                text: 'Job Family'
              },
              text: 'Job Family'
            },
            {
              value: {
                data: 'SERVICE_SCHEME',
                text: 'Service Scheme'
              },
              text: 'Service Scheme'
            },
            {
              value: {
                data: 'ROLE',
                text: 'System Role'
              },
              text: 'System Role'
            },
            {
              value: {
                data: 'TEACHING_SUBJECTS',
                text: 'Teaching Subjects'
              },
              text: 'Teaching Subjects'
            },
            {
              value: {
                data: 'ORGANISATION_UNIT',
                text: 'Place of Work'
              },
              text: 'Place of Work',
              visibleIf: `{currentUser_departmentTypes} notcontains ${organizationUnitLevelsConst.School}
                or ({currentUser_isOverallSystemAdministrator}==true or {currentUser_isUserAccountAdministrator}==true)`
            },
            {
              value: {
                data: 'USER_GROUP',
                text: 'User Group'
              },
              text: 'User Group'
            }
          ]
        },
        {
          type: 'cxtagbox',
          name: 'status',
          visibleIf: '{filterOptions.data} = "STATUS"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select account status.',
          title: 'User Status',
          otherPlaceHolder: 'Please select account status',
          choices: [
            {
              value: 'Active',
              text: 'Active'
            },
            {
              value: 'New',
              text: 'New'
            },
            {
              value: 'Rejected',
              text: 'Rejected'
            },
            {
              value: 'Deactive',
              text: 'Deleted'
            },
            {
              value: 'Inactive',
              text: 'Suspended'
            },
            {
              value: 'IdentityServerLocked',
              text: 'Locked'
            },
            {
              value: 'Archived',
              text: 'Archived'
            }
          ]
        },
        {
          type: 'cxtagbox',
          name: 'userGroup',
          visibleIf: '{filterOptions.data} = "USER_GROUP"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'User Group is required',
          title: 'User Group',
          otherPlaceHolder: 'User Group is required',
          choices: [],
          storeWholeObject: false,
          keyName: 'value'
        },
        {
          type: 'cxtagbox',
          name: 'ageGroup',
          visibleIf: '{filterOptions.data} = "AGE_GROUP"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select age group.',
          title: 'Age Group',
          otherPlaceHolder: 'Please select to filter',
          choices: [
            {
              value: '0',
              text: '0-19'
            },
            {
              value: '20',
              text: '20-29'
            },
            {
              value: '30',
              text: '30-39'
            },
            {
              value: '40',
              text: '40-49'
            },
            {
              value: '50',
              text: '> 50'
            }
          ]
        },
        {
          type: 'cxtagbox',
          name: 'systemRole',
          visibleIf: '{filterOptions.data} = "ROLE"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select system role.',
          title: 'System Role',
          otherPlaceHolder: 'Please select system role',
          choicesByUrl: {
            url: `${AppConstant.api.organization}/usertypes?archetypeEnums=SystemRole&includeLocalizedData=true`,
            valueName: 'identity.extId',
            titleName: 'localizedData.0.fields.0.localizedText'
          },
          choicesVisibleIf: `{currentUser_assignRolePermission} contains {item}`
        },
        {
          type: 'cxtagbox',
          name: 'accountType',
          visibleIf: '{filterOptions.data} = "ACCOUNT_TYPE"',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select account type.',
          title: 'Account Type',
          choices: [
            {
              value: 1,
              text: 'External user'
            },
            {
              value: 2,
              text: 'MOE user'
            }
          ]
        },
        {
          type: 'datepicker',
          name: 'creationDateFrom',
          visibleIf: '{filterOptions.data} = "CREATION_DATE"',
          width: '200px',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select start date.',
          title: 'From',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-20:c+20'
          }
        },
        {
          type: 'datepicker',
          name: 'creationDateTo',
          visibleIf: '{filterOptions.data} = "CREATION_DATE"',
          width: '200px',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select end date.',
          title: 'To',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          validators: [
            {
              type: 'expression',
              text: 'End date should be after start date.',
              expression:
                '{creationDateFrom} notempty and compareDates({creationDateTo},{creationDateFrom})'
            }
          ],
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-20:c+20'
          }
        },
        {
          type: 'datepicker',
          name: 'expirationDateFrom',
          useDisplayValuesInTitle: false,
          visibleIf: '{filterOptions.data} = "EXPIRATION_DATE"',
          width: '200px',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select start date.',
          title: 'From',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-20:c+20'
          }
        },
        {
          type: 'datepicker',
          name: 'expirationDateTo',
          useDisplayValuesInTitle: false,
          visibleIf: '{filterOptions.data} = "EXPIRATION_DATE"',
          width: '200px',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select end date.',
          title: 'To',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          validators: [
            {
              type: 'expression',
              text: 'End date should be after start date.',
              expression:
                '{expirationDateFrom} notempty and compareDates({expirationDateTo},{expirationDateFrom})'
            }
          ],
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-20:c+20'
          }
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
              '{learningCatalogApi_catalogentries_explorer_url}/{learningCatalog_serviceSchemes}',
            titleName: 'displayText'
          }
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
              '{learningCatalogApi_catalogentries_explorer_url}/{learningCatalog_teachingSubjects}',
            titleName: 'displayText'
          },
          choicesEnableIf: '{filterOptions.data} = "TEACHING_SUBJECTS"',
          visibleIf: '{filterOptions.data} = "TEACHING_SUBJECTS"'
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
              `{learningCatalogApi_catalogentries_explorer_url}/` +
              `{learningCatalog_jobFamily}?relatedTo=4a4bcf3a-9e31-11e9-9939-0242ac120003`,
            titleName: 'displayText'
          },
          choicesEnableIf: '{filterOptions.data} = "JOB_FAMILY"',
          visibleIf: '{filterOptions.data} = "JOB_FAMILY"'
        },
        {
          type: 'dropdown',
          name: 'developmentalRole',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Developmental Role is required',
          title: 'Developmental Role',
          choicesByUrl: {
            url:
              '{learningCatalogApi_catalogentries_explorer_url}/{learningCatalog_developmentalRoles}',
            titleName: 'displayText'
          },
          visibleIf: '{filterOptions.data} = "DEVELOPMENTAL_ROLE"'
        },
        {
          type: 'cxtagbox',
          name: 'designation',
          visibleIf: '{filterOptions.data} = "DESIGNATION"',
          storeWholeObject: true,
          keyName: 'id',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please select designation.',
          title: 'Designation',
          width: '50%',
          choicesByUrl: {
            url:
              '{learningCatalogApi_catalogentries_explorer_url}/' +
              `{learningCatalog_designation}?relatedToStr={currentDepartment_typesQueryParam}`,
            titleName: 'displayText'
          }
        },
        {
          type: 'cxtagbox',
          name: 'organisationUnit',
          visibleIf: '{filterOptions.data} = "ORGANISATION_UNIT"',
          startWithNewLine: false,
          storeWholeObject: true,
          keyName: 'identity.id',
          choicesByUrl: {
            // tslint:disable-next-line:max-line-length
            url: `${AppConstant.api.organization}/departments/{fromDepartmentId}/hierarchydepartmentinfos?includeParent=false&includeChildren=true&countChildren=false&departmentTypeIds=${organizationUnitLevelIdsConst.Branch}&departmentTypeIds=${organizationUnitLevelIdsConst.Cluster}&departmentTypeIds=${organizationUnitLevelIdsConst.Division}&departmentTypeIds=${organizationUnitLevelIdsConst.Ministry}&departmentTypeIds=${organizationUnitLevelIdsConst.OrganizationUnit}&departmentTypeIds=${organizationUnitLevelIdsConst.Wing}`,
            titleName: 'name',
            valueName: 'identity.id'
          },
          isRequired: true,
          requiredErrorText: 'Place of Work is required',
          title: 'Place of Work',
          choicesOrder: 'asc'
        }
      ]
    }
  ]
};

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
          html: `<div style="display: flex">
          <div style=" min-width: 11em">{panel.filterOptions.text}</div>
          <div style="width: 20%; padding-left:1em;max-width:5em">is</div>
                                <div style="width: 40%">
                                    {panel.data.text}
                                </div>
                            </div>`
        }
      ]
    }
  ]
};
