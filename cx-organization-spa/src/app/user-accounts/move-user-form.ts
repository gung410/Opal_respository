export const moveUserAccountFormJSON = {
  elements: [
    {
      type: 'dropdown',
      name: 'departmentId',
      title: 'Target Place of Work',
      renderAs: 'select2',
      requiredErrorText: 'Please fill in the new Place of Work.',
      isRequired: true,
      select2Config: {
        placeholder: 'Please select the target place of work'
      },
      choicesByUrl: {
        url: `{organizationApi_BaseUrl}/departments/{fromDepartmentId}/hierarchydepartmentinfos?includeParent=false&includeChildren=true&includeDepartmentType=false&countChildren=false&getParentNode=false&countUser=false`,
        valueName: 'identity.id',
        titleName: 'name'
      },
      choicesOrder: 'asc'
    },
    {
      type: 'paneldynamic',
      renderMode: 'list',
      allowAddPanel: false,
      allowRemovePanel: true,
      name: 'arrray_user_info',
      title: 'Selected user account(s)',
      valueName: 'users',
      templateElements: [
        {
          type: 'html',
          name: 'userInfo',
          html: `
                    <div class="user-info-container">
                    <div class="avatar"><img class="avatar--circle" src="{panel.avatarUrl}"/></div>
                    <div class="info">
                    <span class="info--name">{panel.firstName}</span>
                    <span class="info--email">{panel.emailAddress}</span>
                    </div>
                    </div>
                    `
        }
      ]
    }
  ]
};
