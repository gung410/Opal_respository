import {
  DesignationEnum,
  TeachingLevelEnum,
  TrackEnum
} from 'app/shared/constants/pd-catalogue.enum';
import { ServiceSchemeCodeEnum } from 'app/shared/constants/service-scheme.enum';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';

import { AppConstant } from 'app/shared/app.constant';
import { JobKeys } from './constants/job-key.constant';
import { PDCatalogueConstant } from './constants/pd-catalogue.enum';

export const userFormJSON = {
  focusFirstQuestionAutomatic: false,
  showNavigationButtons: 'none',
  pages: [
    {
      name: 'basic',
      elements: [
        {
          type: 'html',
          name: 'avatar',
          visibleIf: "{formDisplayMode}=='edit'"
        },
        {
          name: 'titleSalutation',
          type: 'dropdown',
          title: 'Salutation',
          isRequired: true,
          requiredErrorText: 'Please select salutation.',
          width: '30%',
          choices: [
            { value: 'Dr', text: 'Dr' },
            { value: 'Mdm', text: 'Mdm' },
            { value: 'Miss', text: 'Miss' },
            { value: 'Mr', text: 'Mr' },
            { value: 'Mrs', text: 'Mrs' },
            { value: 'Ms', text: 'Ms' }
          ],
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'text',
          name: 'firstName',
          title: 'Name',
          width: '70%',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Please fill in your full name.',
          maxLength: 66,
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`,
          validators: [
            {
              type: 'regex',
              text:
                'The following characters are not supported: &, <, >, \\, ", \'',
              regex: /^((?![&<>\\“”‘’'"])[\s\S])*$/
            }
          ]
        },
        {
          name: 'identityType',
          type: 'dropdown',
          title: 'Identity Type',
          isRequired: true,
          visible: false,
          requiredErrorText: 'Please select identity type.',
          showOptionsCaption: false,
          width: '20%',
          choices: ['NRIC', 'FIN', 'Other'],
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'text',
          name: 'ssn',
          title: 'Identity No',
          width: '30%',
          isRequired: true,
          visible: false,
          startWithNewLine: false,
          requiredErrorText: 'Please fill in NRIC.',
          popoverText:
            `NRIC:- ANNNNNNNC <br /> ` +
            `(i) A is the century prefix as follows:-<br />` +
            `For year of birth 1900-1999: 'S'.<br />` +
            `For year of birth 2000-2099: 'T'.<br />` +
            `(ii) NNNNNNN is a seven-digit assigned number.<br />` +
            `(iii) C is the check digit.<br />`,
          validators: [
            {
              type: 'regex',
              text: `Please enter a valid NRIC.`,
              regex: '(^[STst]\\d{7}[A-Z,a-z]$)'
            }
          ],
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'text',
          name: 'ssn',
          title: 'Identity No',
          width: '30%',
          isRequired: true,
          visible: false,
          startWithNewLine: false,
          requiredErrorText: 'Please fill in FIN number.',
          popoverText:
            `FIN:- ANNNNNNNC <br />` +
            `(i) A is the century prefix as follows:-<br />` +
            `For year 1900-1999: 'F'.<br />` +
            `For year 2000-2099: 'G'.<br />` +
            `(ii) NNNNNNN is an assigned number.<br />` +
            `(iii) C is the check digit.`,
          validators: [
            {
              type: 'regex',
              text: `Please enter a valid FIN.`,
              regex: '(^[FGfg]\\d{7}[A-Z,a-z]$)'
            }
          ],
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'text',
          name: 'ssn',
          title: 'Identity No',
          width: '30%',
          isRequired: true,
          visible: false,
          startWithNewLine: false,
          requiredErrorText: 'Identity No is required',
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'text',
          name: 'emailAddress',
          title: 'Official Email',
          isRequired: true,
          maxLength: 320,
          startWithNewLine: false,
          requiredErrorText: 'Please fill in an official email address.',
          validators: [
            {
              type: 'email',
              text: 'Please enter a valid official email address.'
            }
          ],
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          name: 'dateOfBirth',
          type: 'datepicker',
          inputType: 'date',
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Date of Birth is required',
          title: 'Date of Birth',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-50:c',
            maxDate: '+0d'
          },
          width: '50%',
          visibleIf: `false`,
          readOnly: true
        },
        {
          name: 'expirationDate',
          type: 'datepicker',
          inputType: 'date',
          title: 'Date of Expiry of Account',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          startWithNewLine: true,
          isRequired: true,
          requiredErrorText: 'Date of Expiry of Account is required',
          width: '50%',
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit'
              or ({formDisplayMode}=='edit'
                and ({currentUser_hasPermissionToEdit}==true or {currentUser_isDivisionLearningCoordinatorOrSchoolStaffDeveloper}==true)))`,
          validators: [
            {
              type: 'expression',
              text:
                'Account expiry date should be after account creation date.',
              expression:
                '{activeDate} empty or compareDates({expirationDate},{activeDate})'
            },
            {
              type: 'expression',
              text: 'Please select current or future date.',
              expression: 'compareDates({expirationDate},{currentDate})'
            }
          ]
        },
        {
          type: 'radiogroup',
          name: 'gender',
          title: 'Gender',
          defaultValue: 0,
          startWithNewLine: false,
          isRequired: true,
          requiredErrorText: 'Gender is required',
          width: '50%',
          choices: [
            {
              value: 0,
              text: 'Male',
              enableIf: `{titleSalutation} empty
            or ['Dr', 'Mr'] contains {titleSalutation}
            or {currentUser_hasPermissionToEdit}==true`
            },
            {
              value: 1,
              text: 'Female',
              enableIf: `{titleSalutation} empty
            or ['Dr', 'Mdm', 'Miss', 'Mrs', 'Ms'] contains {titleSalutation}
            or {currentUser_hasPermissionToEdit}==true`
            }
          ],
          enableIf: `{currentObject_isExternallyMastered}==false
              and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          name: 'activeDate',
          type: 'datepicker',
          inputType: 'date',
          title: 'Account Active From',
          placeHolder: 'dd/mm/yyyy',
          dateFormat: 'dd/mm/yy',
          startWithNewLine: false,
          width: '50%',
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`,
          visibleIf: '{isVisibleActivateDate}==true'
        },
        {
          name: 'teachingRecords',
          type: 'datepicker',
          inputType: 'date',
          title: 'Date joined Ministry',
          placeHolder: 'dd/mm/yyyy',
          startWithNewLine: false,
          dateFormat: 'dd/mm/yy',
          config: {
            changeMonth: true,
            changeYear: true,
            yearRange: 'c:c+50',
            minDate: new Date(),
            firstDay: 1
          },
          width: '50%',
          readOnly: true,
          visibleIf:
            "{formDisplayMode}=='edit' && {currentObject_isExternallyMastered}==true",
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          name: 'entityStatus',
          type: 'text',
          title: 'Account Status',
          startWithNewLine: false,
          width: '50%',
          readOnly: true,
          visibleIf: "{formDisplayMode}=='edit'"
        },
        {
          name: 'departmentName',
          type: 'text',
          title: 'Place of Work',
          startWithNewLine: false,
          width: '50%',
          readOnly: true,
          visibleIf: "{formDisplayMode}=='edit'"
        },
        {
          type: 'dropdown',
          name: 'personnelGroups',
          title: 'Service Scheme',
          startWithNewLine: false,
          width: '50%',
          requiredErrorText: 'Please select your service scheme.',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.ServiceSchemes.id}`,
            titleName: 'displayText'
          },
          visibleIf: "{formDisplayMode}=='edit' and {isPendingUser}==false",
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          name: 'personalStorageSize',
          type: 'text',
          inputType: 'number',
          title: 'Personal Space Limitation',
          startWithNewLine: false,
          width: '50%',
          isRequired: true,
          maxLength: 20,
          minValue: 1,
          requiredErrorText: 'This field is required.',
          validators: [
            {
              type: 'expression',
              text: 'The storage size is limited between 1 to 100 GBs.',
              expression: `
              {personalStorageSize} >= 1
               && ({currentObject_systemRoles} notcontains ${UserRoleEnum.OverallSystemAdministrator}
                    && {personalStorageSize} <= 100)
               || {currentObject_systemRoles} contains ${UserRoleEnum.OverallSystemAdministrator}`
            }
          ],
          visibleIf: `{formDisplayMode}=='edit' || {formDisplayMode}=='create'`,
          enableIf: `(({formDisplayMode}=='edit'
          && {currentUser_systemRoles_ExId} anyof [${UserRoleEnum.OverallSystemAdministrator}]
          && {currentObject_systemRoles} notcontains ${UserRoleEnum.OverallSystemAdministrator})
            || ({currentObject_extId} == {currentUser_extId}))
          || ({formDisplayMode}=='create' && {currentUser_systemRoles_ExId} anyof [${UserRoleEnum.OverallSystemAdministrator}])`
        },
        {
          type: 'dropdown',
          name: 'departmentId',
          title: 'Place of Work',
          renderAs: 'select2',
          startWithNewLine: false,
          width: '50%',
          requiredErrorText: 'Please select the Place of Work.',
          isRequired: true,
          choicesByUrl: {
            url:
              `{organizationApi_BaseUrl}/departments/{fromDepartmentId}/hierarchydepartmentinfos` +
              `?includeParent=false&includeChildren={includeChildrenDepartments}` +
              `&includeDepartmentType=false&countChildren=false&getParentNode=false&countUser=false`,
            valueName: 'identity.id',
            titleName: 'name'
          },
          choicesOrder: 'asc',
          visibleIf: `{formDisplayMode}=='create'`,
          enableIf: `
              {formDisplayMode}=='create'
          and (
              {currentObject_isExternallyMastered}==false
          and ({currentUser_systemRoles_ExId} anyof [${UserRoleEnum.OverallSystemAdministrator}, ${UserRoleEnum.UserAccountAdministrator}])
          )`
        },
        {
          name: 'jobKey',
          type: 'dropdown',
          hasOther: true,
          title: 'Job Key',
          width: '50%',
          choices: JobKeys,
          startWithNewLine: false,
          enableIf: `{currentObject_isExternallyMastered}==false
            and ({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`,
          storeOthersAsComment: false,
          visible: false
        },
        {
          type: 'comment',
          name: 'signupReason',
          title: 'Reason for User Account Request',
          maxLength: 1000,
          placeHolder:
            'The number of limited character is 1000 characters. State the activities that user will be doing in OPAL2.0 e.g., Conduct a course, participate in professional development activity.',
          requiredErrorText: 'Reason for User Account Request is required.',
          startWithNewLine: true,
          width: '100%',
          isRequired: true,
          visibleIf: "{formDisplayMode}!='edit' or {isPendingUser}==true",
          validators: [
            {
              type: 'text',
              text: 'Reason must equal or less than 1000 character',
              maxLength: 1000
            },
            {
              type: 'regex',
              text:
                'The following characters are not supported: &, <, >, \\, ", \'',
              regex: /^((?![&<>\\“”‘’'"])[\s\S])*$/
            }
          ]
        }
      ]
    },
    {
      name: 'advanced',
      elements: [
        {
          type: 'dropdown',
          name: 'careerPathsDropdown',
          title: 'Track',
          requiredErrorText: 'Track is required',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.Tracks.id}?relatedTo={personnelGroups.code}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and (
                    {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.AED}'
                or  {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.MKE}'
                or  ({personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EAS}' and {currentObject_isExternallyMastered}==false)
                )
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'tagbox',
          name: 'careerPathsTagbox',
          title: 'Track',
          requiredErrorText: 'Track is required',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.Tracks.id}?relatedTo={personnelGroups.code}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'expression',
          name: 'careerPaths',
          expression:
            'getArrayFromDropdownAndTagbox({careerPathsDropdown}, {careerPathsTagbox})',
          visible: false
        },
        {
          type: 'tagbox',
          name: 'teachingLevels',
          title: 'Teaching Level(s)',
          requiredErrorText: 'Teaching Level is required',
          choicesByUrl: {
            url:
              `${AppConstant.api.learningCatalog}/catalogentries/explorer/` +
              `${PDCatalogueConstant.TeachingLevels.id}?relatedTo={personnelGroups.code}`,
            valueName: 'id',
            titleName: 'displayText'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and (
                    {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
                or  {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.MKE}'
                or  (
                        {personnelGroups.codingScheme}='${ServiceSchemeCodeEnum.AED}'
                    and ({careerPaths} contains '${TrackEnum.Teaching_and_Learning}'
                    or {careerPaths} contains '${TrackEnum.Outdoor_Adventure_Educator}')
                    )
                )
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        // {
        //   type: 'dropdown',
        //   name: 'teachingCourseOfStudy',
        //   title: 'Teaching Course of Study',
        //   isRequired: true,
        //   requiredErrorText: 'Teaching Course of Study is required',
        //   choicesByUrl: {
        //     url:
        //       `${AppConstant.api.learningCatalog}/catalogentries/explorer/` +
        //       `${PDCatalogueConstant.teachingCourseOfStudy.id}?relatedTo={personnelGroups.id}`,
        //     valueName: 'id',
        //     titleName: 'displayText'
        //   },
        //   visibleIf:
        //     `
        //         {formDisplayMode}=='edit'
        //     and {isPendingUser}==false
        //     and {personnelGroups.codingScheme}!='${ServiceSchemeCodeEnum.NA}'
        //     and (
        //             {personnelGroups.codingScheme}!='${ServiceSchemeCodeEnum.EO}'
        //         and (
        //                 {careerPaths} notcontains '${TrackEnum.Teaching_and_Learning}'
        //             and {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.AED}'
        //             )
        //         )
        //     `
        // },
        {
          name: 'designation',
          type: 'dropdown',
          title: 'Designation',
          startWithNewLine: true,
          requiredErrorText: 'Designation is required',
          width: '100%',
          choicesByUrl: {
            url:
              `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.Designation.id}?` +
              `relatedTo={personnelGroups.code}&relatedTo={currentOrganisationUnit_types}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
              {formDisplayMode}=='edit'
          and {isPendingUser}==false
          and {personnelGroups} notempty
          and {currentObject_isExternallyMastered}==false
          and {personnelGroups.codingScheme}!='${ServiceSchemeCodeEnum.NA}'
          `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'text',
          name: 'jobTitle',
          title: 'Designation',
          startWithNewLine: false,
          readOnly: true,
          visibleIf: `
              {formDisplayMode}=='edit'
          and {isPendingUser}==false
          and {currentObject_isExternallyMastered}==true
          and {personnelGroups} notempty
          and {personnelGroups.codingScheme}!='${ServiceSchemeCodeEnum.NA}'`
        },
        // TODO: using for Release 2
        // {
        //   type: 'expression',
        //   name: 'designationQueryParams',
        //   expression: 'getDesignationQueryParams({careerPaths}, {personnelGroups.id}, {currentObject_departmentTypes})',
        //   visible: false
        // },
        {
          type: 'tagbox',
          name: 'portfolio',
          title: 'Portfolio',
          isRequired: true,
          requiredErrorText: 'Portfolio is required',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.Portfolio.id}?relatedToStr={teachingLevels}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {teachingLevels} notempty
            and {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
            and (
                    {designation}=='${DesignationEnum.Senior_Teacher}'
                or  {designation}=='${DesignationEnum.Lead_Teacher}'
                or  {designation}=='${DesignationEnum.Level_Head}'
                or  {designation}=='${DesignationEnum.Subject_Head}'
                or  {designation}=='${DesignationEnum.Head_of_Department}'
                or  {designation}=='${DesignationEnum.Principal_Master_Teacher}'
                or  {designation}=='${DesignationEnum.Master_Teacher}'
                )
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'tagbox',
          name: 'teachingCourseOfStudy',
          title: 'Teaching Course of Study',
          requiredErrorText: 'Teaching Course of Study is required',
          choicesByUrl: {
            url:
              `${AppConstant.api.learningCatalog}/catalogentries/explorer/` +
              `${PDCatalogueConstant.TeachingCourseOfStudy.id}?relatedTo={personnelGroups.code}`,
            valueName: 'id',
            titleName: 'displayText'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and (
                    {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
                or  (
                        {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.AED}'
                    and ({careerPaths} contains '${TrackEnum.Teaching_and_Learning}'
                    or {careerPaths} contains '${TrackEnum.Outdoor_Adventure_Educator}')
                    )
                )
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'checkbox',
          name: 'systemRoles',
          title: 'System Roles',
          isRequired: true,
          hasSelectAll: true,
          requiredErrorText:
            'Please select the appropriate System Role(s) for this user.',
          choicesByUrl: {
            url:
              '{organizationApi_BaseUrl}/usertypes?archetypeEnums=SystemRole&includeLocalizedData=true&timestamp={replaceTS}',
            titleName: 'localizedData.0.fields.0.localizedText'
          },
          keepIncorrectValues: true,
          choicesEnableIf: `
            (
              (
                {currentObject_extId} != {currentUser_extId} &&
                {currentUser_noPermissionRoles} notcontains {item.identity.id}) &&
                {currentUser_fullGrantedRoles} contains {item.identity.id} ||
              (
                {currentObject_extId} == {currentUser_extId} &&
                {currentUser_noPermissionRoles} notcontains {item.identity.id} &&
                {currentUser_fullGrantedRoles} contains {item.identity.id}
                &&
                (
                  (
                    {currentUser_systemRoles_ExId} contains ${UserRoleEnum.OverallSystemAdministrator} and
                    {item.identity.extId} notcontains ${UserRoleEnum.OverallSystemAdministrator}
                  )
                  ||
                  (
                    {currentUser_systemRoles_ExId} notcontains ${UserRoleEnum.OverallSystemAdministrator}
                    &&
                    (
                      (
                        {currentUser_systemRoles_ExId} contains ${UserRoleEnum.DivisionAdmin} and
                        {item.identity.extId} notcontains ${UserRoleEnum.DivisionAdmin}
                      ) or
                      (
                        {currentUser_systemRoles_ExId} contains ${UserRoleEnum.BranchAdmin} and
                        {item.identity.extId} notcontains ${UserRoleEnum.BranchAdmin}
                      ) or
                      (
                        {currentUser_systemRoles_ExId} contains ${UserRoleEnum.SchoolAdmin} and
                        {item.identity.extId} notcontains ${UserRoleEnum.SchoolAdmin}
                      )
                    )
                  )
                )
              )
            )
            and ({formDisplayMode}!='edit'
              or ({formDisplayMode}=='edit'
                and ({currentUser_hasPermissionToEdit}==true or {currentUser_isDivisionLearningCoordinatorOrSchoolStaffDeveloper}==true)))
          `,
          choicesVisibleIf: `   {currentUser_visibleRolePermission} contains {item.identity.id}
            or  (
                {systemRoleIdsOfSelectedUser} contains {item.identity.id}
              )
            `
        },
        {
          type: 'tagbox',
          name: 'teachingSubjects',
          title: 'Teaching Subject(s)',
          requiredErrorText: 'Teaching Subject is required',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.TeachingSubjects.id}?relatedToStr={teachingLevels}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {teachingLevels} notempty
            and (
                    ({teachingLevels} notcontains '${TeachingLevelEnum.NotApplicable}' and {teachingLevels.length} == 1)
                or  ({teachingLevels.length} > 1)
                )
            and (
                    {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
                or  {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.AED}'
                )
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'tagbox',
          name: 'cocurricularActivities',
          title: 'Co-curricular Activity',
          requiredErrorText: 'Co-curricular Activity is required',
          choicesByUrl: {
            url:
              `${AppConstant.api.learningCatalog}/catalogentries/explorer/` +
              `${PDCatalogueConstant.CoCurricularActivity.id}?relatedToStr={teachingLevels}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {teachingLevels} notempty
            and (
                    {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
                or  (
                        ({personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.AED}'
                        and (
                                {careerPaths} contains '${TrackEnum.Teaching_and_Learning}'
                            or  {careerPaths} contains '${TrackEnum.Outdoor_Adventure_Educator}'
                            )
                    )
                )
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          name: 'roleSpecificProficiencies',
          type: 'tagbox',
          title: 'Role-Specific Proficiency',
          requiredErrorText: 'Role-Specific Proficiency is required',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.RoleSpecificProficiencies.id}`,
            valueName: 'id',
            titleName: 'displayText'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
            and {currentObject_isExternallyMastered}==true
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'tagbox',
          name: 'jobFamily',
          title: 'Job Family',
          requiredErrorText: 'Job Family is required',
          choicesByUrl: {
            url:
              `${AppConstant.api.learningCatalog}/catalogentries/explorer/` +
              `${PDCatalogueConstant.JobFamily.id}?relatedTo={personnelGroups.code}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
              {formDisplayMode}=='edit'
          and {isPendingUser}==false
          and {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EAS}'
          `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        }
      ]
    },
    {
      name: 'professionalDevelopment',
      elements: [
        {
          type: 'dropdown',
          name: 'developmentalRoles',
          title: 'Developmental Role',
          requiredErrorText: 'Developmental Role is required',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.DevelopmentalRoles.id}?relatedTo={personnelGroups.code}`,
            titleName: 'displayText'
          },
          visibleIf: `
              {formDisplayMode}=='edit'
          and {isPendingUser}==false
          and {personnelGroups} notempty
          and {personnelGroups.codingScheme}!='${ServiceSchemeCodeEnum.NA}'
          `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'dropdown',
          name: 'learningFrameworksDropdown',
          title: 'Learning Frameworks',
          requiredErrorText: 'Learning Frameworks is required',
          choicesByUrl: {
            url:
              `${AppConstant.api.learningCatalog}/catalogentries/explorer/` +
              `${PDCatalogueConstant.LearningFrameworks.id}?relatedTo={developmentalRoles.id}`,
            valueName: 'id',
            titleName: 'displayText'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {developmentalRoles} notempty
            and {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.AED}'
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'tagbox',
          name: 'learningFrameworksTagbox',
          title: 'Learning Frameworks',
          requiredErrorText: 'Learning Frameworks is required',
          choicesByUrl: {
            url:
              `${AppConstant.api.learningCatalog}/catalogentries/explorer/` +
              `${PDCatalogueConstant.LearningFrameworks.id}?relatedTo={developmentalRoles.id}`,
            valueName: 'id',
            titleName: 'displayText'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {developmentalRoles} notempty
            and (
                    {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EO}'
                or  {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.EAS}'
                or  {personnelGroups.codingScheme}=='${ServiceSchemeCodeEnum.MKE}'
            )
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        },
        {
          type: 'expression',
          name: 'learningFrameworks',
          expression: `getArrayFromDropdownAndTagbox({learningFrameworksDropdown}, {learningFrameworksTagbox})`,
          visible: false
        },
        {
          type: 'tagbox',
          name: 'professionalInterestArea',
          title: 'Areas of Professional Interest',
          requiredErrorText: 'Areas of Professional Interest is required',
          choicesByUrl: {
            url: `${AppConstant.api.learningCatalog}/catalogentries/explorer/${PDCatalogueConstant.AreasOfProfessionalInterest.id}?relatedTo={personnelGroups.code}`,
            titleName: 'displayText',
            valueName: 'id'
          },
          visibleIf: `
                {formDisplayMode}=='edit'
            and {isPendingUser}==false
            and {personnelGroups} notempty
            and {personnelGroups.codingScheme}!='${ServiceSchemeCodeEnum.NA}'
            `,
          enableIf: `({formDisplayMode}!='edit' or ({formDisplayMode}=='edit' and {currentUser_hasPermissionToEdit}==true))`
        }
      ]
    }
  ]
};
