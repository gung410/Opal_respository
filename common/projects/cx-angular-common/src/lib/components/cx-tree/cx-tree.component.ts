import { Component, OnInit, ViewEncapsulation, Input, ViewChild, Output, EventEmitter, TemplateRef } from '@angular/core';
import { CxTreeNodeComponent } from './cx-tree-node/cx-tree-node.component';
import { trigger, transition, style, animate } from '@angular/animations';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CxTreeDialogComponent } from './cx-tree-dialog/cx-tree-dialog.component';
import { CxTreeButtonCondition } from './models/cx-tree-button-condition.model';
import { CxObjectRoute } from './models/cx-object-route.model';
import { CxTreeText } from './models/cx-tree-text.model';
import { CxTreeIcon } from './models/cx-tree-icon.model';
import { CxObjectUtil } from '../../utils/object.util';
import { CxAnimations } from '../../constants/cx-animation.constant';
import { CxAbstractTreeComponent } from '../cx-extensive-tree/cx-abstract-tree.component';

@Component({
  selector: 'cx-tree',
  templateUrl: './cx-tree.component.html',
  styleUrls: ['./cx-tree.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: [CxAnimations.smoothAppendRemove],
  providers: []
})
export class CxTreeComponent<T> extends CxAbstractTreeComponent<T> implements OnInit {
  private _flatObjectsArray: T[] = [];
  @Input() isViewMode: boolean;
  @Input() displayTemplate: TemplateRef<any>;

  get flatObjectsArray() {
    return this._flatObjectsArray;
  }
  @Input() set flatObjectsArray(flatObjectsArray: T[]) {
    this.rootObject = this.getRootObjectFromArray(flatObjectsArray);
    this._flatObjectsArray = flatObjectsArray;
  }

  @Input() destinationItems: T[];
  /*
   * Identity string is path to id field of object
   * for example: object has structure {identity: {id: 1}}
   * to access id field, we input the string like: 'identity.id'
   */
  @Input() currentRoutes: object[] = [];
  @Input() selectedObjectIds: {} = {};
  @Input() showTreeHeader = true;
  @Input() displayCheckboxes = true;
  /*
   * Starts from 0
   */
  @Input() defaultExpandLevel = 1;
  @Output() moveItem: EventEmitter<{ originObjects: CxObjectRoute<T>[], destinationObjectId: any }> = new EventEmitter();
  public rootObject: T;
  public selectedItems: CxObjectRoute<T>[] = [];
  public originObjects: T[];
  constructor(private ngbModal: NgbModal) {
    super();
   }
  @ViewChild(CxTreeNodeComponent) treeNodeComponent: CxTreeNodeComponent<T>;
  ngOnInit() {
    this.rootObject = this.getRootObjectFromArray(this.flatObjectsArray);
  }

  public onDeleteItem(deletingObjects: CxObjectRoute<T>[] | CxObjectRoute<T>) {
    this.deleteItem.emit(Array.isArray(deletingObjects) ? deletingObjects : [deletingObjects]);
    this.unselect();
  }

  public onAddItem(addingItem: { parentObject: T, childName: string }) {
    this.addItem.emit(addingItem);
  }

  public onEditItem(editedObject: T) {
    this.editItem.emit(editedObject);
  }

  public onMoveItem(currentRouteAndObject?: CxObjectRoute<T>) {
    if (currentRouteAndObject) {
      this.onSelectItem(currentRouteAndObject);
    }
    const selectedObjectIds = {};
    this.selectedItems.forEach(item => {
      const itemId = CxObjectUtil.getPropertyValue(item.object, this.idFieldRoute);
      selectedObjectIds[itemId] = itemId;
    });
    if (!this.ngbModal.hasOpenModals()) {
      const modalRef = this.ngbModal.open(CxTreeDialogComponent, { size: 'lg', centered: true });
      (modalRef.componentInstance as CxTreeDialogComponent<T>).flatObjectsArray = this.flatObjectsArray;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).idFieldRoute = this.idFieldRoute;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).parentIdFieldRoute = this.parentIdFieldRoute;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).displayFieldRoute = this.displayFieldRoute;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).currentRoutes = this.selectedItems.map(item => item.route);
      (modalRef.componentInstance as CxTreeDialogComponent<T>).icon = this.icon;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).text = this.text;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).displayTemplate = this.displayTemplate;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).isViewMode = true;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).selectedObjectIds = selectedObjectIds;
      (modalRef.componentInstance as CxTreeDialogComponent<T>).moveItem.subscribe(data => {
        if (data) {
          this.onMovingItemToAnotherDestination(data);
        }
        this.unselect();
        modalRef.close();
      });
    }
  }

  public onMovingItemToAnotherDestination(destinationId: any) {
    this.moveItem.emit({ originObjects: this.selectedItems, destinationObjectId: destinationId });
  }

  public onClickItem(objectRoute: CxObjectRoute<T>) {
    this.clickItem.emit(objectRoute);
  }

  public executeDeleteItem(deletedObjects: CxObjectRoute<T>[]) {
    deletedObjects.forEach(item => {
      this.treeNodeComponent.executeDeleteItem(item);
    });
  }

  public executeUpdateItem(currentObjectIdFieldRoute: any, newObject: T) {
    this.treeNodeComponent.executeUpdateItem(currentObjectIdFieldRoute, newObject);
  }

  public executeAddItem(addedObject: T) {
    this.treeNodeComponent.executeAddItem(addedObject);
  }

  public executeEditItem(editedItem: T) {
    this.treeNodeComponent.executeEditItem(editedItem);
  }

  public unselect() {
    this.selectedItems = [];
    this.treeNodeComponent.unselect();
  }

  public executeMoveItem(originObjects: CxObjectRoute<T>[], destinationId: any) {
    originObjects.forEach(object => {
      this.treeNodeComponent.executeMoveItem(object, destinationId);
    });
  }

  public onSelectItem(selectedItem: CxObjectRoute<T>) {
    const selectedItemIndex = this.selectedItems.findIndex(x => x.object === selectedItem.object);
    selectedItemIndex >= 0 ? this.selectedItems.splice(selectedItemIndex) : this.selectedItems.push(selectedItem);
  }
}
