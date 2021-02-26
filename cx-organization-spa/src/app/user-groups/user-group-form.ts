export const UserGroupFormJSON = {
  elements: [
    {
      type: 'panel',
      name: 'basic',
      visibleIf: "{formDisplayTabs} contains 'basic'",
      elements: [
        {
          type: 'text',
          name: 'name',
          title: 'Name',
          isRequired: true,
          requiredErrorText: 'Please fill in the name of the user group.',
          maxLength: 128
        },
        {
          type: 'comment',
          name: 'description',
          title: 'Description',
          maxLength: 256,
          validators: [
            {
              type: 'text',
              text: 'Description must equal or less than 256 character',
              maxLength: 256
            }
          ]
        }
      ]
    },
    {
      type: 'paneldynamic',
      renderMode: 'list',
      visibleIf: "{formDisplayMode} == 'edit'",
      allowAddPanel: false,
      allowRemovePanel: true,
      confirmDelete: true,
      confirmDeleteText: 'Do you want to remove this user?',
      name: 'arrray_user_info',
      title: 'User accounts',
      valueName: 'users',
      panelRemoveText: 'Remove',
      templateElements: [
        {
          type: 'html',
          name: 'userInfo',
          html: `
                    <div class="user-info-container">
                    <div class="avatar"><img class="avatar--circle" src="{panel.avatarUrl}"/></div>
                    <div class="info">
                    <span class="info--name">{panel.displayName}</span>
                    <span class="info--email">{panel.email}</span>
                    </div>
                    </div>`
        }
      ]
    },
    {
      type: 'html',
      name: 'noMembersText',
      title: 'User accounts',
      html: 'No members',
      visibleIf: "{formDisplayMode} == 'edit' and {users} empty"
    }
  ]
};

export const AddMemberToGroupFormJSON = {
  elements: [
    {
      type: 'dropdown',
      // renderAs: 'select2',
      name: 'userGroup',
      title: 'User Group',
      isRequired: true,
      requiredErrorText: 'Please select a user group.',
      choicesByUrl: {
        url:
          '{organizationApi_BaseUrl}/userpools?departmentIds={currentDepartment_id}&countActiveMembers' +
          '=false&orderBy=name&timestamp={replaceTS}',
        path: 'items',
        titleName: 'name'
      }
    },
    {
      type: 'paneldynamic',
      renderMode: 'list',
      allowAddPanel: false,
      allowRemovePanel: true,
      name: 'arrray_user_info',
      title: 'Selected user account(s)',
      description:
        'Selected user account(s) will be added to the selected group above.',
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
