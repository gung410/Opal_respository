import { Directive, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';

import { MetadataTagModel } from '@opal20/domain-api';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription } from 'rxjs';
import { TreeViewComponent } from '@progress/kendo-angular-treeview';

@Directive({
  selector: 'kendo-treeview[metadataAutoCheckAllChilds]'
})
export class KendoTreeviewMetadataAutoCheckAllChildsDirective implements OnInit, OnDestroy {
  @Input() public checkedKeys: string[] = [];
  @Output() public checkedKeysChange: EventEmitter<string[]> = new EventEmitter<string[]>();
  private subs: Subscription[] = [];
  constructor(protected treeView: TreeViewComponent, protected moduleFacadeService: ModuleFacadeService) {}

  public ngOnInit(): void {
    this.subs.push(
      this.treeView.checkedChange.subscribe(key => {
        const item: MetadataTagModel = key.item.dataItem;
        if (this.isItemChecked(item)) {
          this.checkedKeys = MetadataTagModel.selectAllTreeItemIds(item, this.checkedKeys);
        } else {
          this.checkedKeys = MetadataTagModel.deselectAllTreeItemIds(item, this.checkedKeys);
        }
        this.checkedKeysChange.emit(this.checkedKeys.slice());
      })
    );
  }

  public ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }

  public isItemChecked(item: MetadataTagModel): boolean {
    return this.checkedKeys.includes(item.tagId);
  }
}
