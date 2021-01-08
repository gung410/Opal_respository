import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from 'app-services/user.service';

@Component({
  selector: 'approving-officer-cell',
  template: `<cx-expandable-list
    #approvingOfficer
    [items]="userGroupInfos"
    class="staff-table__items-user-group"
    (expandEvent)="expandEvent($event)"
    [expandTemplate]="expandTemplate"
    [symbol]="','"
    [numberOfDisplay]="2"
  >
    <ng-template #expandTemplate let-item="item">
      <span #itemExpand>{{ item.name }}</span>
    </ng-template>
  </cx-expandable-list> `,
  styleUrls: ['../staff-list.component.scss'],
})
export class SLApprovingOfficerRendererComponent
  implements ICellRendererAngularComp {
  public userGroupInfos: any;
  @ViewChild('itemExpand', { read: ElementRef }) itemExpand: ElementRef;
  private params: any;
  private defaultHeight = 100;
  private defaultPadding = 10;
  constructor(private router: Router, private userService: UserService) {}

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

  public expandEvent(params: any) {
    if (this.params && this.params.api) {
      const newHeight =
        this.itemExpand &&
        this.itemExpand.nativeElement &&
        this.itemExpand.nativeElement.offsetHeight
          ? this.itemExpand.nativeElement.offsetHeight *
              this.userGroupInfos.length +
            this.defaultPadding
          : this.defaultHeight;
      this.params.node.setRowHeight(newHeight);
      this.params.api.onRowHeightChanged();
    }
  }
}
