import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { BlockoutDateViewModel } from '../../models/blockout-date-view.model';
import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { ListBlockoutDateGridDisplayColumns } from '../list-blockout-date-grid/list-blockout-date-grid.component';

@Component({
  selector: 'blockout-date-dependencies-detail-dialog',
  templateUrl: './blockout-date-dependencies-detail-dialog.component.html'
})
export class BlockOutDateDependenciesDetailDialog extends BaseComponent {
  @Input() public availableBlockOutDates: BlockoutDateViewModel[] = [];
  public displayColumns: ListBlockoutDateGridDisplayColumns[] = [
    ListBlockoutDateGridDisplayColumns.title,
    ListBlockoutDateGridDisplayColumns.description,
    ListBlockoutDateGridDisplayColumns.startDateTime,
    ListBlockoutDateGridDisplayColumns.endDateTime,
    ListBlockoutDateGridDisplayColumns.serviceSchemes
  ];
  constructor(public moduleFacadeService: ModuleFacadeService, private dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }
}
