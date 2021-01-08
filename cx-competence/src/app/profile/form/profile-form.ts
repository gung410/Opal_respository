import { ArcheTypeEnum } from 'app-enums/user-type.enum';
import { Designations } from '../data/designation.data';
import { TeachingCourse } from '../data/teaching-course.data';
import { TeachingLevels } from '../data/teaching-level.data';

export const ProfileFormJSON = {
  pages: [
    {
      name: 'page1',
      elements: [
        {
          type: 'panel',
          innerIndent: 1,
          name: 'basicInfo',
          title: 'Basic info',
          elements: [
            {
              type: 'dropdown',
              name: 'titleSalutation',
              title: 'Title (Salutation)',
              maxLength: 66,
              startWithNewLine: false,
              width: '50%',
              choices: ['Ms', 'Miss', 'Mrs', 'Mr', 'Mdm', 'Dr'],
            },
            {
              type: 'text',
              name: 'firstName',
              title: 'Name',
              isRequired: true,
              requiredErrorText: 'Name is required',
              maxLength: 66,
              startWithNewLine: false,
              width: '50%',
            },
            {
              type: 'text',
              name: 'emailAddress',
              title: 'Official Email',
              isRequired: true,
              maxLength: 320,
              startWithNewLine: false,
              width: '50%',
              requiredErrorText: 'Official Email is required',
              validators: [
                {
                  type: 'email',
                  text: 'Invalid email',
                },
              ],
            },
            {
              type: 'text',
              name: 'ssn',
              title: 'UIN/FIN',
              startWithNewLine: false,
              width: '50%',
              validators: [
                {
                  type: 'regex',
                  text: 'Invalid NRIC.',
                  regex: '^[STFG]\\d{7}[A-Z]$',
                },
              ],
            },
            {
              type: 'text',
              name: 'dateOfBirth',
              title: 'Date of birth',
              inputType: 'cxinputmask',
              startWithNewLine: true,
              width: '50%',
              config: {
                alias: 'datetime',
                inputFormat: 'dd/mm/yyyy',
                placeholder: 'dd/mm/yyyy',
              },
              datePickerConfig: {
                changeMonth: true,
                changeYear: true,
                yearRange: 'c-50:c',
                maxDate: '+0d',
                dateFormat: 'dd/mm/yy',
              },
            },
            {
              type: 'radiogroup',
              name: 'gender',
              title: 'Gender',
              defaultValue: 0,
              width: '50%',
              startWithNewLine: false,
              choices: [
                { value: 0, text: 'Male' },
                { value: 1, text: 'Female' },
              ],
            },
            {
              type: 'text',
              name: 'created',
              title: 'Date Account is Created',
              inputType: 'cxinputmask',
              startWithNewLine: true,
              width: '50%',
              config: {
                alias: 'datetime',
                inputFormat: 'dd/mm/yyyy',
                placeholder: 'dd/mm/yyyy',
              },
              enableIf: 'false',
            },
          ],
        },
        {
          type: 'panel',
          innerIndent: 1,
          name: 'organisationInfo',
          title: 'Organisation info',
          elements: [
            {
              type: 'text',
              name: 'dateJoinedMinistry',
              title: 'Date joined Ministry',
              inputType: 'cxinputmask',
              startWithNewLine: false,
              maxLength: 66,
              width: '50%',
              config: {
                alias: 'datetime',
                inputFormat: 'dd/mm/yyyy',
                placeholder: 'dd/mm/yyyy',
              },
              enableIf: 'false',
            },
            {
              type: 'text',
              name: 'expirationDate',
              title: 'Date of Expiry of Account',
              inputType: 'cxinputmask',
              startWithNewLine: false,
              maxLength: 66,
              width: '50%',
              config: {
                alias: 'datetime',
                inputFormat: 'dd/mm/yyyy',
                placeholder: 'dd/mm/yyyy',
              },
              enableIf: 'false',
            },
            {
              type: 'text',
              name: 'departmentName',
              title: 'Place of Work',
              startWithNewLine: true,
              width: '50%',
              enableIf: 'false',
            },
            {
              type: 'text',
              name: 'departmentAddress',
              title: 'Organisation Address',
              startWithNewLine: false,
              width: '50%',
              enableIf: 'false',
            },
            {
              type: 'text',
              name: 'departmentType',
              title: 'Type of School /Organisation',
              startWithNewLine: false,
              width: '50%',
              enableIf: 'false',
            },
            {
              type: 'text',
              name: 'zone',
              title: 'Zone (for schools)',
              startWithNewLine: false,
              width: '50%',
              enableIf: 'false',
            },
          ],
        },
        {
          type: 'panel',
          innerIndent: 1,
          name: 'advancedInfo',
          title: 'Advanced info',
          elements: [
            {
              type: 'dropdown',
              name: 'serviceScheme',
              title: 'Service Scheme',
              startWithNewLine: true,
              width: '50%',
              choicesByUrl: {
                url: `{organizationApi_BaseUrl}/usertypes?archetypeEnums=${ArcheTypeEnum.PersonnelGroup}&includeLocalizedData=true`,
                valueName: 'identity.id',
                titleName: 'localizedData.0.fields.0.localizedText',
              },
            },
            {
              type: 'dropdown',
              name: 'developmentalRole',
              title: 'Developmental Role',
              startWithNewLine: false,
              width: '50%',
              choicesByUrl: {
                url: `{organizationApi_BaseUrl}/usertypes?archetypeEnums=${ArcheTypeEnum.DevelopmentalRole}&includeLocalizedData=true`,
                valueName: 'identity.id',
                titleName: 'localizedData.0.fields.0.localizedText',
              },
            },
            {
              type: 'cxtagbox',
              name: 'careerPath',
              title: 'Career Track(s)',
              startWithNewLine: false,
              width: '100%',
              choicesByUrl: {
                url: `{organizationApi_BaseUrl}/usertypes?archetypeEnums=${ArcheTypeEnum.CareerPath}&includeLocalizedData=true`,
                valueName: 'identity.id',
                titleName: 'localizedData.0.fields.0.localizedText',
              },
            },
          ],
        },
        {
          type: 'panel',
          innerIndent: 1,
          name: 'approvalInfo',
          title: 'Approval info',
          elements: [
            {
              type: 'text',
              name: 'approvingOfficer',
              title: 'Approving Officer',
              startWithNewLine: false,
              width: '50%',
              enableIf: 'false',
            },
            {
              type: 'text',
              name: 'alternateApprovingOfficer',
              title: 'Alternate Approving Officer',
              startWithNewLine: false,
              width: '50%',
              enableIf: 'false',
            },
          ],
        },
        {
          type: 'panel',
          innerIndent: 1,
          name: 'professionalInfo',
          title: 'Professional info',
          elements: [
            {
              type: 'dropdown',
              name: 'designation',
              title: 'Designation',
              choices: Designations,
              startWithNewLine: true,
              width: '100%',
            },
            {
              type: 'cxtagbox',
              name: 'teachingSubjects',
              title: 'Teaching Subjects',
              startWithNewLine: false,
              width: '100%',
              choicesByUrl: {
                url: `{assessmentApi_BaseUrl}/xcategories?types={xCategoryTypeId}&includeLocalizedData=true`,
                valueName: 'identity.id',
                titleName: 'localizedData.0.fields.0.localizedText',
              },
            },
            {
              type: 'cxtagbox',
              name: 'teachingLevel',
              title: 'Teaching Level(s)',
              startWithNewLine: false,
              width: '100%',
              choices: TeachingLevels,
            },
            {
              type: 'cxtagbox',
              name: 'teachingCourse',
              title: 'Teaching Course of Study(s)',
              startWithNewLine: false,
              width: '100%',
              choices: TeachingCourse,
            },
            {
              type: 'cxtagbox',
              name: 'learningFramework',
              title: 'Learning Framework(s)',
              startWithNewLine: false,
              width: '100%',
              choicesByUrl: {
                url: `{competenceApi_BaseUrl}/learningframeworks`,
                valueName: 'identity.id',
                titleName: 'name',
              },
            },
          ],
        },
        {
          type: 'panel',
          innerIndent: 1,
          name: 'systemInfo',
          title: 'System info',
          elements: [
            {
              name: 'notificationPreference',
              type: 'checkbox',
              title: 'Notification Preference',
              startWithNewLine: true,
              width: '100%',
              choices: [
                {
                  value: 'email',
                  text: 'Email',
                },
                {
                  value: 'system',
                  text: 'System',
                },
              ],
            },
          ],
        },
      ],
    },
  ],
};
