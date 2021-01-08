import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { CxPeopleListComponent } from 'app/cx-people-picker/cx-people-list/cx-people-list.component';

@Component({
  selector: 'remove-user-renderer',
  templateUrl: './remove-user-renderer.component.html',
  styleUrls: ['./remove-user-renderer.component.scss'],
})
export class RemoveUserRendererComponent implements ICellRendererAngularComp {
  params: any;
  id: number;
  constructor() {}

  // called on init
  agInit(params: any): void {
    this.params = params;
    this.id = this.params.value.identity.id;
  }

  // called when the cell is refreshed
  refresh(params: any): boolean {
    this.params = params;
    this.id = this.params.value.identity.id;

    return true;
  }

  removeUser(): void {
    const cxPeopleListComponent: CxPeopleListComponent = this.params.context
      .componentParent;
    cxPeopleListComponent.onPeopleRemoved(this.params.data);
  }
}
