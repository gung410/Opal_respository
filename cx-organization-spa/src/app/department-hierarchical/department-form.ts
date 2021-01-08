import { AppConstant } from 'app/shared/app.constant';

// tslint:disable-next-line:variable-name
export const DepartmentFormJSON = {
  elements: [
    {
      type: 'dropdown',
      name: 'jsonDynamicAttributes.typeOfOrganizationUnits',
      title: 'Type of School / Organisation',
      isRequired: true,
      requiredErrorText: 'Please select type of school / organisation.',
      choicesByUrl: {
        url: '{learningCatalogApi_catalogentries_explorer_url}/OU-TYPES',
        titleName: 'displayText',
        valueName: 'id'
      }
    },
    {
      type: 'text',
      name: 'name',
      title: 'Organisation Unit Name',
      isRequired: true,
      requiredErrorText: 'Please fill in organisation unit name.',
      maxLength: 100
    },
    {
      type: 'text',
      name: 'organizationNumber',
      title: 'School Code',
      maxLength: 4
    },
    {
      type: 'text',
      name: 'identity.extId',
      title: 'Organisation Code',
      maxLength: 8
    },
    {
      type: 'text',
      name: 'jsonDynamicAttributes.clusterSuperintendent',
      title: 'Cluster Superintendent',
      visibleIf: '{isClusterDepartment} == true'
    },
    {
      type: 'text',
      name: 'jsonDynamicAttributes.zoneDirector',
      title: 'Zonal Director',
      visibleIf: '{isZoneDepartment} == true'
    },
    {
      type: 'cxtagbox',
      name: 'levels',
      storeWholeObject: true,
      keyName: 'identity.id',
      title: 'Level of education',
      choicesByUrl: {
        url: `${AppConstant.api.organization}/departmenttypes?archetypeEnums=6&includeLocalizedData=true`,
        titleName: 'localizedData.0.fields.0.localizedText'
      },
      visibleIf: '{isSchoolDepartment} == true',
      choicesEnableIf: '{isSchoolDepartment} == true'
    },
    {
      type: 'text',
      name: 'address',
      title: 'Address information',
      maxLength: 500
    },
    {
      type: 'checkbox',
      name: 'jsonDynamicAttributes.showInSignUpForm',
      titleLocation: 'hidden',
      choices: [
        {
          value: true,
          text: 'Show in sign up form'
        }
      ],
      visibleIf: '{isBranchDepartment} == true or {isSchoolDepartment} == true'
    }
  ]
};
