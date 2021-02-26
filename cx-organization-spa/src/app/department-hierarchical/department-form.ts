import { AppConstant } from 'app/shared/app.constant';

// tslint:disable-next-line:variable-name
export const DepartmentFormJSON = {
  elements: [
    {
      type: 'dropdown',
      name: 'jsonDynamicAttributes.typeOfOrganizationUnits',
      title: 'Type of School / Organisation',
      optionsCaption: 'Select from the list',
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
      placeHolder: 'Type in organisation name',
      isRequired: true,
      requiredErrorText: 'Please fill in organisation unit name.',
      maxLength: 100
    },
    {
      type: 'text',
      name: 'organizationNumber',
      title: 'School Code',
      placeHolder: 'Type in school code',
      maxLength: 4
    },
    {
      type: 'text',
      name: 'identity.extId',
      title: 'Organisation Code',
      placeHolder: 'Type in organisation code',
      maxLength: 8
    },
    {
      type: 'text',
      name: 'jsonDynamicAttributes.clusterSuperintendent',
      title: 'Cluster Superintendent',
      placeHolder: 'Input name',
      visibleIf: '{isClusterDepartment} == true'
    },
    {
      type: 'text',
      name: 'jsonDynamicAttributes.zoneDirector',
      title: 'Zonal Director',
      placeHolder: 'Input name',
      visibleIf: '{isZoneDepartment} == true'
    },
    {
      type: 'cxtagbox',
      name: 'levels',
      storeWholeObject: true,
      keyName: 'identity.id',
      title: 'Level of education',
      placeholder: 'Input name',
      isRequired: true,
      requiredErrorText: 'Please select type of Level of education.',
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
      placeHolder: 'Type in address',
      isRequired: true,
      requiredErrorText: 'Please fill in Address information.',
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
