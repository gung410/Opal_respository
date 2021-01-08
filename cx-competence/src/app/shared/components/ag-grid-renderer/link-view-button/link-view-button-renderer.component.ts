import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { StaffListService } from 'app/staff/staff.container/staff-list.service';

@Component({
  selector: 'view-cell',
  template: `<span
    ><a routerLink="{{ detailLink }}" (click)="resetSearchBar($event)"
      >View</a
    ></span
  >`,
})
export class LinkViewButtonRendererComponent
  implements ICellRendererAngularComp {
  public params: any;
  public detailLink: any;
  constructor(private staffListService: StaffListService) {}

  // called on init
  public agInit(params: any): void {
    this.params = params;
    this.detailLink = this.params.value;
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    this.params = params;
    this.detailLink = this.params.value;

    return true;
  }

  public resetSearchBar(event: any) {
    this.staffListService.resetSearchValueSubject.next(true);
  }
}
