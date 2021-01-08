import { Component, ViewEncapsulation } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'cell-remove-button-action',
  templateUrl: './cell-remove-button-action.component.html',
  styleUrls: ['./cell-remove-button-action.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CellRemoveButtonActionComponent
  implements ICellRendererAngularComp {
  private removeEventEmitter: (extId: string) => void;
  private params: any;

  private userExtId: string;

  agInit(params: any): void {
    this.removeEventEmitter = params.onClick;
    this.userExtId = params.data.identity.extId;
    this.params = params;
  }

  btnClickedHandler(): void {
    this.removeEventEmitter(this.userExtId);
  }

  refresh(params: any): boolean {
    throw new Error('Method not implemented.');
  }
}
