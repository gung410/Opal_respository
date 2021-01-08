import {
  ActionToolbarModel,
  CxTreeIcon,
  CxTreeText,
  DepartmentHierarchiesModel,
} from '@conexus/cx-angular-common';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';

export function initUniversalToolbar(
  translateAdapterService: TranslateAdapterService
): DepartmentHierarchiesModel {
  const departmentModel = new DepartmentHierarchiesModel({
    idFieldRoute: 'identity.id',
    parentIdFieldRoute: 'parentDepartmentId',
    icon: new CxTreeIcon(),
    text: new CxTreeText(),
    isViewMode: true,
    enableSearch: true,
    treeHeader: 'Organisation Unit',
    havingExtensiveArea: false,
    noResultFoundMessage: translateAdapterService.getValueImmediately(
      'Staff.Search_Result_Dialog.No_Result_Message'
    ),
    isDisplayOrganisationNavigation: true,
    departments: [],
  });

  return departmentModel;
}

export function initStaffActions(
  translateAdapterService: TranslateAdapterService
): ActionToolbarModel {
  return new ActionToolbarModel({
    listEssentialActions: [
      {
        text: translateAdapterService.getValueImmediately(
          `Common.Action.AssignLearningNeeds`
        ),
        actionType: StatusActionTypeEnum.AssignLearningNeeds,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
        disable: true,
      },
      {
        text: translateAdapterService.getValueImmediately(
          `Common.Action.Reminder`
        ),
        actionType: StatusActionTypeEnum.Reminder,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
        disable: true,
      },
      {
        text: translateAdapterService.getValueImmediately(
          `Common.Action.Export_Selected`
        ),
        actionType: StatusActionTypeEnum.Export,
        allowActionSingle: false,
        icon: null,
        messageConfirm: '',
        disable: true,
      },
    ],
  });
}
