import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';

@Component({
  selector: 'user-groups-cell',
  template: `<cx-expandable-list
    [items]="userGroupInfos"
    class="staff-table__items-user-group"
    [expandTemplate]="expandTemplate"
    [symbol]="','"
    [numberOfDisplay]="2"
  >
    <ng-template #expandTemplate let-item="item">
      <span>{{ item.name }}</span>
    </ng-template>
  </cx-expandable-list> `,
  styleUrls: ['../staff-list.component.scss'],
})
export class SLUserGroupsRendererComponent implements ICellRendererAngularComp {
  public userGroupInfos: any;
  private params: any;
  constructor() {}

  // called on init
  public agInit(params: any): void {
    this.params = params;
    this.userGroupInfos = this.params.value;
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    this.params = params;
    this.userGroupInfos = this.params.value;
    return true;
  }

  public valueUserGroupInfos() {
    return this.userGroupInfos;
  }
}
