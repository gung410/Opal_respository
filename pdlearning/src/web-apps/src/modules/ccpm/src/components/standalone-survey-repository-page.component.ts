import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ViewChild } from '@angular/core';
import { ContextMenuAction, StandaloneSurveyDetailMode, StandaloneSurveySearchTermService } from '@opal20/domain-components';
import {
  IStandaloneSurveyModel,
  STANDALONE_SURVEY_QUERY_MODE,
  StandaloneSurveyApiService,
  StandaloneSurveyModel,
  SurveyQueryModeEnum,
  SurveyStatus
} from '@opal20/domain-api';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { CCPMRoutePaths } from '../ccpm.config';
import { ContextMenuItem } from '@opal20/common-components';
import { IStandaloneSurveyEditorPageNavigationData } from '../ccpm-navigation-data';

@Component({
  selector: 'standalone-survey-repository-page',
  templateUrl: './standalone-survey-repository-page.component.html'
})
export class StandaloneSurveyRepositoryPageComponent extends BasePageComponent {
  @ViewChild('kendoTabstrip', { static: true })
  public kendoTabstrip: TabStripComponent;

  public searchText: string = '';
  public searchTerm: string | undefined;
  public filterStatus: SurveyStatus = SurveyStatus.All;

  public filterItems: IDataItem[] = [
    {
      text: this.translateCommon('All'),
      value: SurveyStatus.All
    },
    {
      text: this.translateCommon('Draft'),
      value: SurveyStatus.Draft
    },
    {
      text: this.translateCommon('Unpublished'),
      value: SurveyStatus.Unpublished
    },
    {
      text: this.translateCommon('Published'),
      value: SurveyStatus.Published
    }
  ];

  public contextMenuItemsForFormsTab: ContextMenuItem[] = [
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

  public readonly lnaFormQueryMode = STANDALONE_SURVEY_QUERY_MODE;
  public readonly SurveyQueryModeEnum = SurveyQueryModeEnum;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public searchTermService: StandaloneSurveySearchTermService,
    public formApiService: StandaloneSurveyApiService
  ) {
    super(moduleFacadeService);
    this.initTextSearch();
  }

  public onInit(): void {
    this.updateDeeplink('ccpm/lnaform');
  }

  public onSearch(): void {
    this.searchTerm = this.searchText.slice();
    if (this.searchTermService.searchText !== this.searchTerm) {
      this.searchTermService.searchText = this.searchTerm;
      this.searchTermService.state.skip = 0;
    }
  }

  public onLnaSurveysTabSelect(selectEvent: SelectEvent): void {
    if (selectEvent.title === this.translateCommon(this.lnaFormQueryMode.get(SurveyQueryModeEnum.Archived))) {
      this.searchTermService.queryMode = SurveyQueryModeEnum.Archived;
      this.searchTermService.searchStatuses = [SurveyStatus.Archived];
      this.filterStatus = SurveyStatus.Archived;
    } else {
      this.searchTermService.queryMode = SurveyQueryModeEnum.All;
      this.filterStatus = SurveyStatus.All;
    }
  }

  public onAddNewButtonClick(): void {
    const formData: StandaloneSurveyModel = new StandaloneSurveyModel(<IStandaloneSurveyModel>{
      title: 'Draft',
      status: SurveyStatus.Draft
    });
    this.formApiService.createSurvey(formData, [], [], true).subscribe(response => {
      this.navigateToFormDetail({ formId: response.form.id, mode: StandaloneSurveyDetailMode.Edit });
    });
  }

  public get canShowFilterBtn(): boolean {
    if (!this.kendoTabstrip.tabs) {
      return false;
    }
    return this.kendoTabstrip.tabs.some(x => x.title === 'Forms' && x.selected === true);
  }

  private navigateToFormDetail(data: IStandaloneSurveyEditorPageNavigationData): void {
    this.navigateTo(CCPMRoutePaths.StandaloneSurveyDetailPage, data);
  }
  private initTextSearch(): void {
    this.filterStatus = this.searchTermService.searchStatuses ? this.searchTermService.searchStatuses[0] : SurveyStatus.All;
    if (this.searchTermService.searchText) {
      this.searchText = this.searchTermService.searchText;
      this.onSearch();
    }
  }
}
