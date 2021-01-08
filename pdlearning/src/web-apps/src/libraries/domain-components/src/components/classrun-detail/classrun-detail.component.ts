import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';

import { ClassRunDetailMode } from '../../models/classrun-detail-mode.model';
import { ClassRunDetailViewModel } from './../../view-models/classrun-detail-view.model';
import { ClassRunTabInfo } from '../../models/classrun-tab.model';
import { FormGroup } from '@angular/forms';
import { ScrollableMenu } from '@opal20/common-components';
import { SearchClassRunType } from '@opal20/domain-api';

@Component({
  selector: 'classrun-detail',
  templateUrl: './classrun-detail.component.html'
})
export class ClassRunDetailComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public mode: ClassRunDetailMode;
  @Input() public isRescheduleMode: boolean = false;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;
  @Input() public searchType: SearchClassRunType;
  @Input() public classRun: ClassRunDetailViewModel;

  @ViewChild('classRunOverviewInfoTab', { static: false })
  public classRunOverviewInfoTab: ElementRef;

  @ViewChild('classRunPlanningTab', { static: false })
  public classRunPlanningTab: ElementRef;

  @ViewChild('classRunApplicationTab', { static: false })
  public classRunApplicationTab: ElementRef;

  @ViewChild('classRunReasonTab', { static: false })
  public classRunReasonTab: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: ClassRunTabInfo.OverviewInfo,
      title: 'Overview',
      elementFn: () => {
        return this.classRunOverviewInfoTab;
      }
    },
    {
      id: ClassRunTabInfo.Planning,
      title: 'Planning',
      elementFn: () => {
        return this.classRunOverviewInfoTab;
      }
    },
    {
      id: ClassRunTabInfo.Application,
      title: 'Application',
      elementFn: () => {
        return this.classRunOverviewInfoTab;
      }
    }
  ];

  public activeTab: string = ClassRunTabInfo.OverviewInfo;
  public loadingData: boolean = false;
  public SearchClassRunType: typeof SearchClassRunType = SearchClassRunType;

  private _classRun: ClassRunDetailViewModel;
  private _searchType: SearchClassRunType;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
