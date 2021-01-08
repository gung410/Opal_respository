import { IHeaderAngularComp } from '@ag-grid-community/angular';
import {
  IAfterGuiAttachedParams,
  IHeaderParams,
} from '@ag-grid-community/core';
import { Component } from '@angular/core';

@Component({
  selector: 'lna-header',
  template: ` <div>
    LNA STATUS / <br />
    COMPLETE BY
    <div></div>
  </div>`,
  styleUrls: ['../staff-list.component.scss'],
})
export class LNAHeaderComponent implements IHeaderAngularComp {
  agInit(params: IHeaderParams): void {}
  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {}
  refresh(params: IHeaderParams): boolean {
    return true;
  }
}
