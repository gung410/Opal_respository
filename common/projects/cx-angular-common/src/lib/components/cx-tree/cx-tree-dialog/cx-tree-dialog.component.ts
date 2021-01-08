import { Component, OnInit, Input, EventEmitter, Output, TemplateRef } from '@angular/core';
import { CxObjectUtil } from '../../../utils/object.util';
import { CxTreeIcon } from '../models/cx-tree-icon.model';
import { CxTreeText } from '../models/cx-tree-text.model';
import { CxObjectRoute } from '../models/cx-object-route.model';

@Component({
  selector: 'cx-tree-dialog',
  templateUrl: './cx-tree-dialog.component.html',
  styleUrls: ['./cx-tree-dialog.component.scss']
})
export class CxTreeDialogComponent<T> implements OnInit {
  @Input() currentRoutes: object[] = [];
  @Input() flatObjectsArray: T[] = [];
  @Input() parentIdFieldRoute: string;
  @Input() displayFieldRoute: string;
  @Input() idFieldRoute: string;
  @Input() isViewMode: boolean;
  @Input() icon: CxTreeIcon;
  @Input() text: CxTreeText;
  @Input() displayTemplate: TemplateRef<any>;
  @Input() dialogHeader = 'Tree';
  @Input() selectedObjectIds: {};
  @Output() moveItem: EventEmitter<any> = new EventEmitter();

  constructor() {
  }
  public destinationId: any;
  public destinationItems: {} = {};
  ngOnInit() {
  }

  public onClickItem(objectRoute: CxObjectRoute<T>) {
    const itemId = CxObjectUtil.getPropertyValue(objectRoute.object, this.idFieldRoute);
    this.destinationId = itemId;
    if (Object.keys(this.destinationItems).length > 0) {
      this.destinationItems = {};
    }
    this.destinationItems[itemId] = itemId;
  }

  public confirm() {
    this.moveItem.emit(this.destinationId);
  }

  public cancel() {
    this.moveItem.emit();
  }
}
