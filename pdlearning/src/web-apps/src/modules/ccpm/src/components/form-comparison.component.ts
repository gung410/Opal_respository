import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { CompareSide, IComparisonFormViewModel, ItemChangeType } from './dialogs/compare-version-form-dialog.component';
import { Component, Input } from '@angular/core';
@Component({
  selector: 'form-comparison',
  templateUrl: './form-comparison.component.html'
})
export class FormComparisonComponent extends BaseComponent {
  @Input() public comparisonFormData: IComparisonFormViewModel[] = [];
  @Input() public side: CompareSide = CompareSide.Left;

  public ITEMCHANGETYPE = ItemChangeType;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public checkBlankArea(changeType: ItemChangeType): boolean {
    return (
      (this.side === CompareSide.Right && changeType === ItemChangeType.Removed) ||
      (this.side === CompareSide.Left && changeType === ItemChangeType.AddNew)
    );
  }
}
