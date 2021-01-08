import { Component, OnInit, Input, EventEmitter, Output, TemplateRef,
  ViewChild, OnChanges, SimpleChanges, ChangeDetectionStrategy } from '@angular/core';
import { CxObjectUtil } from '../../../utils/object.util';
import { CxObjectRoute } from '../../cx-tree/models/cx-object-route.model';
import { CxTreeIcon } from '../../cx-tree/models/cx-tree-icon.model';
import { CxTreeText } from '../../cx-tree/models/cx-tree-text.model';
import { CxExtensiveTreeComponent } from '../cx-extensive-tree.component';
import { CxExtensiveTreeModel } from '../models/cx-extensive-tree-model';

@Component({
  selector: 'cx-extensive-tree-dialog',
  templateUrl: './cx-extensive-tree-dialog.component.html',
  styleUrls: ['./cx-extensive-tree-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CxExtensiveTreeDialogComponent<T> {
  @Input() set flatObjectsArray(flatObjectsArray: T[]) {
    this._flatObjectsArray = flatObjectsArray;
    this._flatObjectsArray.forEach((element: any) => {
      element.displayText = CxObjectUtil.getPropertyValue(element, this.displayFieldRoute);
    });
  }
  private _flatObjectsArray: T[] = [];
  @Input() parentIdFieldRoute: string;
  @Input() displayFieldRoute: string;
  @Input() idFieldRoute: string;
  @Input() isViewMode: boolean;
  @Input() icon: CxTreeIcon = new CxTreeIcon();
  @Input() text: CxTreeText = new CxTreeText();
  @Input() displayTemplate: TemplateRef<any>;
  @Input() dialogHeader = 'Tree';
  @Input() selectedObjectIds: {};
  @Input() movedItem = {};
  @Input() countChildrenFieldRoute: string;
  @Input() havingExtensiveArea = false;
  @Output() moveItem: EventEmitter<any> = new EventEmitter();
  @Output() loadChildrenEvent: EventEmitter<any> = new EventEmitter();
  public destinationItem: {} = {};

  public onClickItem(field: CxExtensiveTreeModel) {
    this.destinationItem = field;
  }

  public confirm() {
    this.moveItem.emit({movedItem: this.movedItem, destinationItem: this.destinationItem});
  }

  public cancel() {
    this.moveItem.emit();
  }

  public loadChildren(field: T) {
    const children = this.flatObjectsArray.find(x => CxObjectUtil.getPropertyValue(x, this.parentIdFieldRoute)
      === CxObjectUtil.getPropertyValue(field, this.idFieldRoute));
    if (!children) {
      this.loadChildrenEvent.emit(field);
    }
  }

}
