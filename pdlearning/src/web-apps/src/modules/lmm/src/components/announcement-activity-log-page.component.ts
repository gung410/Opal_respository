import { AnnouncementFilterModel, AnnouncementViewModel, ContextMenuAction, ContextMenuEmit } from '@opal20/domain-components';
import { AnnouncementRepository, AnnouncementStatus } from '@opal20/domain-api';
import {
  BaseFormComponent,
  IFormBuilderDefinition,
  IGridFilter,
  ModuleFacadeService,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import { Component, HostBinding, Input } from '@angular/core';

import { AnnouncementActivityLogViewModel } from '../models/announcement-activity-log-view.model';
import { startEndValidator } from '@opal20/common-components';

@Component({
  selector: 'announcement-activity-log-page',
  templateUrl: './announcement-activity-log-page.component.html'
})
export class AnnouncementActivityLogPageComponent extends BaseFormComponent {
  @Input() public stickyDependElement: HTMLElement;

  public searchText: string = '';
  public filterData: AnnouncementFilterModel = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };

  public get courseId(): string | undefined {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string | undefined) {
    if (Utils.isDifferent(this._courseId, v)) {
      this._courseId = v;
    }
  }

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
    }
  }

  public announcementVm: AnnouncementActivityLogViewModel = new AnnouncementActivityLogViewModel();
  private _courseId: string;
  private _classRunId: string;

  constructor(public moduleFacadeService: ModuleFacadeService, private announcementRepository: AnnouncementRepository) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onApplyFilter(): void {
    this.filterData = this.announcementVm.filterData;
    this.validate().then(valid => {
      if (valid) {
        this.filter = {
          ...this.filter,
          filter: this.filterData.convert()
        };
      }
    });
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData.convert()
    };
  }

  public onGridContextMenuSelected(contextMenuEmit: ContextMenuEmit<AnnouncementViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Send:
        this.changeAnnouncementStatus([contextMenuEmit.dataItem], AnnouncementStatus.Sent);
        break;
      case ContextMenuAction.Cancel:
        this.changeAnnouncementStatus([contextMenuEmit.dataItem], AnnouncementStatus.Cancelled);
        break;
    }
  }

  public changeAnnouncementStatus(announcements: AnnouncementViewModel[], status: AnnouncementStatus): void {
    this.announcementRepository
      .changeAnnouncementStatus({
        status: status,
        ids: announcements.map(p => p.id)
      })
      .subscribe(_ => {
        this.showNotification(this.translate('Course Announcement is sent successfully.'));
      });
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition | undefined {
    return {
      formName: 'announcement',
      validateByGroupControlNames: [['fromCreatedDate', 'toCreatedDate']],
      controls: {
        fromCreatedDate: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator('fromCreatedDate', p => p.value, p => this.announcementVm.toCreatedDate, true, 'dateOnly'),
              validatorType: 'fromCreatedDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'From Date cannot be greater than To Date')
            }
          ]
        },
        toCreatedDate: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator('endCreatedDate', p => this.announcementVm.fromCreatedDate, p => p.value, true, 'dateOnly'),
              validatorType: 'endCreatedDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'To Date cannot be less than From Date')
            }
          ]
        }
      }
    };
  }
}
