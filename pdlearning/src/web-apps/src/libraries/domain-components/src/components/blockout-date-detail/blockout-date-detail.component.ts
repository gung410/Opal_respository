import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { BlockoutDateDetailMode } from './../../models/blockout-date-detail-mode.model';
import { BlockoutDateDetailViewModel } from './../../view-models/blockout-date-detail-view.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'blockout-date-detail',
  templateUrl: './blockout-date-detail.component.html'
})
export class BlockoutDateDetailComponent extends BaseComponent {
  @Input() public form: FormGroup;

  public get blockoutDate(): BlockoutDateDetailViewModel {
    return this._blockoutDate;
  }
  @Input('blockoutDateDetailVM')
  public set blockoutDate(v: BlockoutDateDetailViewModel) {
    this._blockoutDate = v;
  }
  @Input() public mode: BlockoutDateDetailMode | undefined;

  private _blockoutDate: BlockoutDateDetailViewModel;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public asViewMode(): boolean {
    return this.mode === BlockoutDateDetailMode.View;
  }

  public asViewModeWhenConfirmed(): boolean {
    return this.mode === BlockoutDateDetailMode.Edit && this.blockoutDate.data.isConfirmed === true;
  }
}
