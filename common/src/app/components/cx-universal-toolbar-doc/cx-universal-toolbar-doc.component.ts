import { Component, OnInit } from '@angular/core';

import { ActionsModel, ActionToolbarModel } from 'projects/cx-angular-common/src';

@Component({
  selector: 'cx-universal-toolbar-doc',
  templateUrl: './cx-universal-toolbar-doc.component.html',
  styleUrls: ['./cx-universal-toolbar-doc.component.scss'],
})
export class CxUniversalToolbarDocComponent implements OnInit {
  breadCrumbNavigation = [
    {
      name: 'Division 28',
      identity: {
        extId: '',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'OrganizationalUnit',
        id: 20581,
      },
    },
  ];
  departmentModel = {
    departments: [
      {
        parentDepartmentId: 15817,
        departmentName: 'Division 24',
        departmentDescription: '',
        organizationNumber: '24',
        address: '',
        postalCode: '',
        city: '',
        tag: '',
        languageId: 2,
        countryCode: 65,
        path: '\\2\\15130\\15131\\20115\\',
        pathName:
          'Ministry Of Education, SG\\Professional Wing (updated-02)\\Division 24',
        childrenCount: 1,
        isCurrentDepartment: true,
        identity: {
          extId: '',
          ownerId: 3001,
          customerId: 2052,
          archetype: 'OrganizationalUnit',
          id: 20577,
        },
        entityStatus: {
          externallyMastered: false,
          lastExternallySynchronized: '2019-09-05T09:06:54.9066667Z',
          entityVersion: 'AAAAAAARDcU=',
          lastUpdated: '2019-09-05T09:06:54.9066667Z',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false,
        },
        enableAddBtn: true,
        enableEditBtn: true,
        enableMoveBtn: true,
        enableRemoveBtn: true,
      },
      {
        parentDepartmentId: 20577,
        departmentName: 'Branch 24',
        departmentDescription: '',
        organizationNumber: '24',
        address: '',
        postalCode: '',
        city: '',
        tag: '',
        languageId: 2,
        countryCode: 65,
        path: '\\2\\15130\\15131\\20115\\20135\\',
        pathName:
          'Ministry Of Education, SG\\Professional Wing (updated-02)\\Division 24\\Branch 24',
        childrenCount: 1,
        identity: {
          extId: '',
          ownerId: 3001,
          customerId: 2052,
          archetype: 'OrganizationalUnit',
          id: 20597,
        },
        entityStatus: {
          externallyMastered: false,
          lastExternallySynchronized: '2019-09-05T09:06:54.92Z',
          entityVersion: 'AAAAAAARDdk=',
          lastUpdated: '2019-09-05T09:06:54.92Z',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false,
        },
        enableAddBtn: true,
        enableEditBtn: true,
        enableMoveBtn: true,
        enableRemoveBtn: true,
      },
    ],
    icon: {
      add: 'add-icon',
      edit: 'edit-icon',
      move: 'move-icon',
      remove: 'remove-icon',
      save: 'save-icon',
      close: 'close-icon',
      expand: 'expand-icon',
      collapse: 'collapse-icon',
      root: 'root-icon',
      node: 'node-icon',
      emptyNode: 'empty-node-icon',
      indeterminate: 'indeterminate-icon',
    },
    text: {
      header: 'Name',
      moveBtn: 'Move',
      removeBtn: 'Remove',
      addTooltip: 'Add',
      editTooltip: 'Edit',
      moveTooltip: 'Move',
      removeTooltip: 'Remove',
      saveTooltip: 'Save',
      closeTooltip: 'Close',
    },
    noResultFoundMessage: 'We can’t find any staffs that match to your search',
    isDetectExpandTree: false,
    isDisplayOrganisationNavigation: true,
    havingExtensiveArea: false,
    currentDepartmentId: 20577,
    idFieldRoute: 'identity.id',
    parentIdFieldRoute: 'parentDepartmentId',
    enableSearch: true,
    treeHeader: 'Organisation Unit',
    isViewMode: true,
  };

  userActions = new ActionToolbarModel({
    listEssentialActions: [
      {
        text: `Action 1`,
        actionType: 1,
        allowActionSingle: false,
        icon: 'icon-filter',
        messageConfirm: '',
        disable: true,
      },
      {
        text: `Action 2`,
        actionType: 1,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
      },
      {
        text: `Action 3`,
        actionType: 3,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
      },
    ],
    listSpecifyActions: [
      {
        text: `Action 1`,
        actionType: 1,
        allowActionSingle: false,
        icon: 'icon-filter',
        messageConfirm: '',
        disable: false,
      },
      {
        text: `Action 2`,
        actionType: 2,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
      },
      {
        text: `Action 2`,
        actionType: 2,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
      },

    ],
    listNonEssentialActions: [
      {
        text: `Action 1`,
        actionType: 1,
        allowActionSingle: false,
        icon: 'icon-filter',
        messageConfirm: '',
        disable: true,
      },
      {
        text: `Action 2`,
        actionType: 2,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
      },
      {
        text: `Action 3`,
        actionType: 3,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
      },
    ],
  });
  isVerticalToShowMenuAction = true;

  constructor() {}
  onClickButtonGroupAction($event: ActionsModel) {}
  ngOnInit() {}
}
