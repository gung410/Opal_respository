import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Course, DigitalContent, SearchClassRunType } from '@opal20/domain-api';

import { FormGroup } from '@angular/forms';
import { ScrollableMenu } from '@opal20/common-components';
import { SessionDetailMode } from './../../models/session-detail-mode.model';
import { SessionDetailViewModel } from './../../view-models/session-detail-view.model';
import { SessionTabInfo } from './../../models/session-tab.model';

@Component({
  selector: 'session-detail',
  templateUrl: './session-detail.component.html'
})
export class SessionDetailComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public mode: SessionDetailMode;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;
  @Input() public course: Course = new Course();
  @Input() public isRescheduleMode: boolean = false;
  @Input() public forModule: 'lmm' | 'cam' | string;
  public get session(): SessionDetailViewModel {
    return this._session;
  }
  @Input()
  public set session(v: SessionDetailViewModel) {
    this._session = v;
  }
  @Input() public classRunSearchType: SearchClassRunType;

  @Output('preRecordClipChange') public preRecordClipChangeEvent: EventEmitter<DigitalContent | null> = new EventEmitter();
  @Output('usePreRecordClipChange') public usePreRecordClipChangeEvent: EventEmitter<boolean> = new EventEmitter();

  @ViewChild('sessionOverviewInfoTab', { static: false })
  public sessionOverviewInfoTab: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: SessionTabInfo.OverviewInfo,
      title: 'Overview',
      elementFn: () => {
        return this.sessionOverviewInfoTab;
      }
    }
  ];
  public activeTab: string = SessionTabInfo.OverviewInfo;
  public loadingData: boolean = false;
  private _session: SessionDetailViewModel;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onPreRecordClipChanged(e: DigitalContent | null): void {
    this.preRecordClipChangeEvent.emit(e);
  }

  public onUsePreRecordClipChanged(e: boolean): void {
    this.usePreRecordClipChangeEvent.emit(e);
  }
}
