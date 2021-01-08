import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from 'app-services/user.service';

@Component({
  selector: 'service-scheme-cell',
  template: `<cx-expandable-list
    [items]="personnelGroups"
    class="staff-table__items-user-group"
    (expandEvent)="expandEvent($event)"
    [expandTemplate]="expandTemplate"
    [symbol]="','"
    [numberOfDisplay]="2"
  >
    <ng-template #expandTemplate let-item="item">
      <span #itemExpand>{{ item }}</span>
    </ng-template>
  </cx-expandable-list>`,
  styleUrls: ['../staff-list.component.scss'],
})
export class SLServiceSchemeRendererComponent
  implements ICellRendererAngularComp {
  public personnelGroups: string[];
  @ViewChild('itemExpand', { read: ElementRef }) itemExpand: ElementRef;
  private params: any;
  private defaultHeight: number = 100;
  private defaultPadding: number = 10;
  constructor(private router: Router, private userService: UserService) {}

  // called on init
  public agInit(params: any): void {
    this.refresh(params);
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    this.params = params;
    this.personnelGroups = this.params.value.map(
      (serviceScheme) => serviceScheme.name
    );

    return true;
  }

  public expandEvent(params: any): void {
    if (this.params && this.params.api) {
      const newHeight =
        this.itemExpand &&
        this.itemExpand.nativeElement &&
        this.itemExpand.nativeElement.offsetHeight
          ? this.itemExpand.nativeElement.offsetHeight *
              this.personnelGroups.length +
            this.defaultPadding
          : this.defaultHeight;
      this.params.node.setRowHeight(newHeight);
      this.params.api.onRowHeightChanged();
    }
  }
}
