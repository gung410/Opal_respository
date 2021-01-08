import { BasePageComponent, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { CCPM_PERMISSIONS, ContextMenuAction, FormDetailMode, FormSearchTermService } from '@opal20/domain-components';
import { Component, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  FORM_QUERY_MODE,
  FormApiService,
  FormModel,
  FormQueryModeEnum,
  FormQuestionModel,
  FormStatus,
  FormSurveyType,
  FormType,
  QuestionOption,
  QuestionType,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { CCPMRoutePaths } from '../ccpm.config';
import { ContextMenuItem } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormListComponent } from './form-list.component';
import { IFormCreationOption } from '../models/form-creation-option.model';
import { IFormEditorPageNavigationData } from '../ccpm-navigation-data';
import { ImportFormDialogComponent } from './dialogs/import-form-dialog.component';

@Component({
  selector: 'form-repository-page',
  templateUrl: './form-repository-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class FormRepositoryPageComponent extends BasePageComponent {
  @ViewChild('kendoTabstrip', { static: true })
  public kendoTabstrip: TabStripComponent;
  public formPermission = CCPM_PERMISSIONS;
  public filterBySurveyTypes: FormSurveyType[];
  public excludeBySurveyTypes: FormSurveyType[] = [FormSurveyType.PostCourse];

  public filterItems: IDataItem[] = [
    {
      text: this.translateCommon('All'),
      value: FormStatus.All
    },
    {
      text: this.translateCommon('Approved'),
      value: FormStatus.Approved
    },
    {
      text: this.translateCommon('Draft'),
      value: FormStatus.Draft
    },
    {
      text: this.translateCommon('Pending Approval'),
      value: FormStatus.PendingApproval
    },
    {
      text: this.translateCommon('Published'),
      value: FormStatus.Published
    },
    {
      text: this.translateCommon('Ready For Use'),
      value: FormStatus.ReadyToUse
    },
    {
      text: this.translateCommon('Rejected'),
      value: FormStatus.Rejected
    },
    {
      text: this.translateCommon('Unpublished'),
      value: FormStatus.Unpublished
    }
  ];

  public contextMenuItemsForListingTab: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Duplicate,
      text: this.translateCommon('Duplicate'),
      icon: 'aggregate-fields'
    },
    {
      id: ContextMenuAction.Rename,
      text: this.translateCommon('Rename'),
      icon: 'edit'
    },
    {
      id: ContextMenuAction.Publish,
      text: this.translateCommon('Publish'),
      icon: 'check'
    },
    {
      id: ContextMenuAction.Unpublish,
      text: this.translateCommon('Unpublish'),
      icon: 'cancel'
    },

    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    },
    {
      id: ContextMenuAction.TransferOwnership,
      text: this.translateCommon('Transfer Ownership'),
      icon: 'user'
    },
    {
      id: ContextMenuAction.Archive,
      text: this.translateCommon('Archive'),
      icon: 'select-box'
    }
  ];

  public contextMenuItemsForApprovalTab: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Publish,
      text: this.translateCommon('Publish'),
      icon: 'check'
    },
    {
      id: ContextMenuAction.Unpublish,
      text: this.translateCommon('Unpublish'),
      icon: 'cancel'
    },
    {
      id: ContextMenuAction.Rename,
      text: this.translateCommon('Rename'),
      icon: 'edit'
    },
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    }
  ];

  public contextMenuItemsForArchivedTab: ContextMenuItem[] = [
    {
      id: ContextMenuAction.TransferOwnership,
      text: this.translateCommon('Transfer Ownership'),
      icon: 'user'
    },
    {
      id: ContextMenuAction.Duplicate,
      text: this.translateCommon('Duplicate'),
      icon: 'aggregate-fields'
    }
  ];

  public formFilterStatus: FormStatus = FormStatus.All;
  public textSearch: string = '';
  public searchTerm: string | undefined;
  public formCreationButtonIcon: string = 'plus';
  public formCreationOptions: IFormCreationOption[] = [
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, FormType.Quiz),
      value: FormType.Quiz,
      click: (dataItem: IFormCreationOption) => {
        this.onFormCreationClick(dataItem.value);
      }
    },
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, FormType.Survey),
      value: FormType.Survey,
      click: (dataItem: IFormCreationOption) => {
        this.onFormCreationClick(dataItem.value);
      }
    },
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, FormType.Poll),
      value: FormType.Poll,
      click: (dataItem: IFormCreationOption) => {
        this.onFormCreationClick(dataItem.value);
      }
    },
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, 'Holistic rubric'),
      value: FormType.Holistic,
      click: (dataItem: IFormCreationOption) => {
        this.onAssessmentCreationClick(dataItem.value);
      }
    },
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, 'Analytic rubric'),
      value: FormType.Analytic,
      click: (dataItem: IFormCreationOption) => {
        this.onAssessmentCreationClick(dataItem.value);
      }
    }
  ];

  public formImportOption: Array<IFormCreationOption> = [
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, 'Quiz'),
      value: FormType.Quiz,
      click: (dataItem: IFormCreationOption) => {
        this.onFormImportClick(dataItem.value);
      }
    },
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, 'Survey'),
      value: FormType.Survey,
      click: (dataItem: IFormCreationOption) => {
        this.onFormImportClick(dataItem.value);
      }
    },
    {
      text: new TranslationMessage(this.moduleFacadeService.translator, 'Poll'),
      value: FormType.Poll,
      click: (dataItem: IFormCreationOption) => {
        this.onFormImportClick(dataItem.value);
      }
    }
  ];

  public readonly formQueryMode = FORM_QUERY_MODE;
  public readonly formQueryModeEnum = FormQueryModeEnum;
  @ViewChild('formList', { static: false }) public formList: FormListComponent;

  public get isContentCreator(): boolean {
    if (!this.currentUser) {
      return false;
    }
    return (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.ContentCreator, SystemRoleEnum.CourseContentCreator)
    );
  }

  public get isCourseFaciliator(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator);
  }

  public get canViewPendingApprovalList(): boolean {
    return (
      this.currentUser &&
      (this.currentUser.hasAdministratorRoles() ||
        this.currentUser.hasRole(
          SystemRoleEnum.MOEHQContentApprovingOfficer,
          SystemRoleEnum.SchoolContentApprovingOfficer,
          SystemRoleEnum.CourseApprovingOfficer
        ))
    );
  }

  public get canViewArchivedlList(): boolean {
    return (
      this.currentUser &&
      (this.currentUser.hasAdministratorRoles() ||
        this.currentUser.hasRole(SystemRoleEnum.CourseContentCreator, SystemRoleEnum.ContentCreator, SystemRoleEnum.CourseFacilitator))
    );
  }

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private formApiService: FormApiService,
    public searchTermService: FormSearchTermService
  ) {
    super(moduleFacadeService);
    this.initTextSearch();
  }

  public onInit(): void {
    this.updateDeeplink('ccpm/form');
  }

  public onFormImportClick(formtype: FormType): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: ImportFormDialogComponent });
    const configurationPopup = dialogRef.content.instance as ImportFormDialogComponent;
    configurationPopup.formType = formtype;

    dialogRef.result.subscribe((isNeedToReloadData: boolean) => {
      if (isNeedToReloadData === true) {
        this.formList.loadItems();
      }
    });
  }

  public onFormCreationClick(formType: FormType): void {
    const formData = new FormModel().initBasicFormData(formType);

    if (formType === FormType.Poll) {
      formData.isAllowedDisplayPollResult = true;
    }

    this.formApiService.createForm(formData, [], [], true).subscribe(response => {
      this.navigateToFormDetail({ formId: response.form.id, formStatus: formType, mode: FormDetailMode.Edit });
    });
  }

  public onAssessmentCreationClick(formType: FormType): void {
    const formData = new FormModel().initBasicFormData(formType);
    const newScale = new FormQuestionModel().initBasicQuestionData(QuestionType.Scale);
    const assessmentsData = [newScale];

    if (formType === FormType.Analytic) {
      const newCriteria = new FormQuestionModel().initBasicQuestionData(QuestionType.Criteria, 1);
      const criteriaOption = new QuestionOption(1, '', null, null, null, null, false, newScale.id);
      newCriteria.questionOptions.push(criteriaOption);

      assessmentsData.push(newCriteria);
    }

    this.formApiService.createForm(formData, assessmentsData, [], true).subscribe(response => {
      this.navigateToFormDetail({ formId: response.form.id, formStatus: formType, mode: FormDetailMode.Edit });
    });
  }

  public onSearch(): void {
    this.searchTerm = this.textSearch.slice();
    if (this.searchTermService.searchText !== this.searchTerm) {
      this.searchTermService.searchText = this.searchTerm;
      this.searchTermService.state.skip = 0;
    }
  }

  public onFormTabSelect(tabSelectedEvent: SelectEvent): void {
    switch (tabSelectedEvent.title) {
      case this.translateCommon(this.formQueryMode.get(FormQueryModeEnum.PendingApproval)):
        this.searchTermService.queryMode = FormQueryModeEnum.PendingApproval;
        this.searchTermService.searchStatuses = [FormStatus.PendingApproval];
        this.searchTermService.isSurveyTemplate = null;
        this.formFilterStatus = FormStatus.PendingApproval;
        break;
      case this.translateCommon(this.formQueryMode.get(FormQueryModeEnum.PostCourseTemplate)):
        this.searchTermService.queryMode = FormQueryModeEnum.PostCourseTemplate;
        this.searchTermService.searchStatuses = [];
        this.searchTermService.isSurveyTemplate = null;
        this.filterBySurveyTypes = [FormSurveyType.PostCourse];
        break;
      case this.translateCommon(this.formQueryMode.get(FormQueryModeEnum.Archived)):
        this.searchTermService.queryMode = FormQueryModeEnum.Archived;
        this.searchTermService.searchStatuses = [FormStatus.Archived];
        this.searchTermService.isSurveyTemplate = null;
        this.formFilterStatus = FormStatus.Archived;
        break;
      default:
        this.searchTermService.queryMode = FormQueryModeEnum.All;
        this.searchTermService.isSurveyTemplate = false;
        this.formFilterStatus = FormStatus.All;
        this.excludeBySurveyTypes = [FormSurveyType.PostCourse];
    }
  }

  public get canShowFilterBtn(): boolean {
    if (!this.kendoTabstrip.tabs) {
      return false;
    }
    return this.kendoTabstrip.tabs.some(x => x.title === 'Forms' && x.selected === true);
  }

  public onOpalSelectControlFocus(): void {
    const event = document.createEvent('MouseEvents');
    event.initEvent('mousedown', true, true);
    document.dispatchEvent(event);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  private navigateToFormDetail(data: IFormEditorPageNavigationData): void {
    this.navigateTo(CCPMRoutePaths.FormDetail, data);
  }

  private initTextSearch(): void {
    this.formFilterStatus = this.searchTermService.searchStatuses ? this.searchTermService.searchStatuses[0] : FormStatus.All;
    if (this.searchTermService.searchText) {
      this.textSearch = this.searchTermService.searchText;
      this.onSearch();
    }

    if (this.hasPermission(CCPM_PERMISSIONS.ViewListForm)) {
      this.searchTermService.queryMode = FormQueryModeEnum.All;
      this.searchTermService.isSurveyTemplate = false;
      return;
    } else if (this.hasPermission(CCPM_PERMISSIONS.ViewListSubmittedForm)) {
      this.searchTermService.queryMode = FormQueryModeEnum.PendingApproval;
      return;
    } else {
      this.searchTermService.queryMode = FormQueryModeEnum.Archived;
    }
  }
}
