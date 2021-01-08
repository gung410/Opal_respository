import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, HostBinding, Input } from '@angular/core';

import { LMMTabConfiguration } from '@opal20/domain-components';
import { SelectEvent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'announcement-page',
  templateUrl: './announcement-page.component.html'
})
export class AnnouncementPageComponent extends BaseComponent {
  @Input() public anchorEl: ElementRef;
  @Input() public stickyDependElement: HTMLElement;

  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
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
  private _courseId: string;
  private _classRunId: string;

  private selectedTab: LMMTabConfiguration = LMMTabConfiguration.NewAnnouncementTab;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.selectedTab = announcementPageTabIndexMap[event.index];
  }
}

export const announcementPageTabIndexMap = {
  0: LMMTabConfiguration.NewAnnouncementTab,
  1: LMMTabConfiguration.AnnouncementTab
};
