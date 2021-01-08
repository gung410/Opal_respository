import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { BlockOutDateDependenciesDetailDialog } from '../blockout-date-dependencies-detail-dialog/blockout-date-dependencies-detail-dialog.component';
import { BlockoutDateViewModel } from '../../models/blockout-date-view.model';
import { OpalDialogService } from '@opal20/common-components';

@Component({
  selector: 'blockout-date-warning',
  templateUrl: './blockout-date-warning.component.html'
})
export class BlockoutDateWarningComponent extends BaseComponent {
  @Input() public blockOutDates: BlockoutDateViewModel[] = [];

  constructor(public moduleFacadeService: ModuleFacadeService, private opalDialogService: OpalDialogService) {
    super(moduleFacadeService);
  }

  public viewDetailBlockoutDateDependencies(): void {
    const dialogRef = this.opalDialogService.openDialogRef(BlockOutDateDependenciesDetailDialog, <BlockOutDateDependenciesDetailDialog>{
      availableBlockOutDates: this.blockOutDates
    });
  }
}
