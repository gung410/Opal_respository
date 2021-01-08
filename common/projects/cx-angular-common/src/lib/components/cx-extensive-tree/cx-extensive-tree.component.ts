import {
  ChangeDetectionStrategy, ChangeDetectorRef, Component,
  EventEmitter, Input, OnInit, Output, TemplateRef, OnChanges
} from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { cloneDeep } from 'lodash';
import { CxObjectUtil } from '../../utils/object.util';
import { CxObjectRoute } from '../cx-tree/models/cx-object-route.model';
import { CxAbstractTreeComponent } from './cx-abstract-tree.component';
import { CxExtensiveTreeModel } from './models/cx-extensive-tree-model';

@Component({
  selector: 'cx-extensive-tree',
  templateUrl: './cx-extensive-tree.component.html',
  styleUrls: ['./cx-extensive-tree.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxExtensiveTreeComponent<T> extends CxAbstractTreeComponent<T> implements OnInit, OnChanges {

  private _flatObjectsArray: T[] = [];
  get flatObjectsArray() {
    return this._flatObjectsArray;
  }
  @Input() set flatObjectsArray(flatObjectsArray: T[]) {
    if (flatObjectsArray && flatObjectsArray.length) {
      this._flatObjectsArray = flatObjectsArray;
      this._flatObjectsArray.forEach((element: any) => {
        element.enableAddBtn = this.buttonCondition.enableAdd(element);
        element.enableEditBtn = this.buttonCondition.enableEdit(element);
        element.enableMoveBtn = this.buttonCondition.enableMove(element);
        element.enableRemoveBtn = this.buttonCondition.enableRemove(element);
      });
      this.rebuildTree();
    } else {
      this.itemArray = [];
    }

  }

  @Input() countChildrenFieldRoute;
  @Input() isViewMode = false;
  @Input() itemExtensiveTemplate: TemplateRef<any>;
  @Input() headerExtensiveTemplate: TemplateRef<any>;
  @Input() nameDepartmentTemplate: TemplateRef<any>;
  @Input() movedItem: any = {};
  @Input() headerText = '';
  @Input() extensiveTreeHeader = '';
  @Input() havingExtensiveArea = false;
  @Input() currentDepartmentId: number;
  @Input() isDetectExpandTree: boolean;
  @Output() move: EventEmitter<{ originObjects: CxObjectRoute<T>[], destinationObjectId: any }> = new EventEmitter();
  @Output() loadChildren: EventEmitter<any> = new EventEmitter();
  public rootObject: T;
  public isMovingItem = false;
  public selectedItem: any = {};
  public itemArray: Array<CxExtensiveTreeModel> = [];
  public rebuidItemArray: Array<CxExtensiveTreeModel> = [];
  public modalRef: any;
  public isLoadingForDialog = false;
  constructor(
    private ngbModal: NgbModal,
    protected changeDetectorRef: ChangeDetectorRef) {
    super();
  }

  ngOnInit() {}
  ngOnChanges() {

    if (this.isDetectExpandTree) {
      const parentDepartments = [];
      let departmentID = this.currentDepartmentId;
      for (let i = 0; i < this.itemArray.length; i++) {
        if (this.itemArray[i].object.identity.id === departmentID) {
          parentDepartments.push(this.itemArray[i]);
          departmentID = this.itemArray[i].object.parentDepartmentId;
          i = 0;
          continue;
        }
      }
      if (parentDepartments.length > 0) {
        parentDepartments.forEach(element => {
          element.showChildren = true;
          element.loadChildren = true;
          this.expandItem(element);
        });
      }
    }
  }

  private addFirstNode(item: T) {
    const newItem = item;
    const departmentID = CxObjectUtil.getPropertyValue(item, this.idFieldRoute);
    const oldItem = this.itemArray.find(item =>
      CxObjectUtil.getPropertyValue(item.object, this.idFieldRoute) === departmentID);
    const firstNode = new CxExtensiveTreeModel({
      object: newItem,
      showChildren: oldItem ? oldItem.showChildren : false,
      isDisplay: true,
      level: 0,
      isEditingItem: false,
      isAddingNewItem: false,
      isLoadChildren: !!this.getChildrenNodes(newItem).length,
      children: this.getChildrenNodes(newItem)
    });
    this.rebuidItemArray.push(firstNode);
    return firstNode;
  }

  public initTree(items: T[]) {
    const hasOnlyOneRoot = items.length === 1;
    items.forEach(item => {
      const rootNode = this.addFirstNode(item);
      if (hasOnlyOneRoot) { rootNode.showChildren = true; }

      const rootNodeIndex = this.rebuidItemArray.indexOf(rootNode);
      this.addNode(rootNode, rootNodeIndex, 0);
    });

    this.itemArray = cloneDeep(this.rebuidItemArray);
    this.changeDetectorRef.detectChanges();
  }

  public addNode(node: CxExtensiveTreeModel, index: number, level: number) {
    const childrenNodes = node.children.map(element => {
      const departmentID = CxObjectUtil.getPropertyValue(element, this.idFieldRoute);
      const oldItem = this.itemArray.find(item =>
        CxObjectUtil.getPropertyValue(item.object, this.idFieldRoute) === departmentID);
      return new CxExtensiveTreeModel({
        object: element,
        isSelected: this.currentDepartmentId === departmentID,
        showChildren: oldItem ? oldItem.showChildren : false,
        isDisplay: (node.showChildren && node.isDisplay) ? true : false,
        level: level + 1,
        isEditingItem: oldItem ? oldItem.isEditingItem : false,
        isAddingNewItem: oldItem ? oldItem.isAddingNewItem : false,
        children: this.getChildrenNodes(element),
        isLoadChildren: !!this.getChildrenNodes(element).length
      });
    });
    this.rebuidItemArray = this.rebuidItemArray.slice(0, index + 1).concat(childrenNodes).concat(this.rebuidItemArray.slice(index + 1));
    this.changeDetectorRef.detectChanges();
    childrenNodes.forEach(child => {
      if (child.children.length > 0) {
        const inx = this.rebuidItemArray.indexOf(child);
        this.addNode(child, inx, level + 1);
      }
    });
  }

  public rebuildTree() {
    const roots = this.getRootObjectsFromArray(this.flatObjectsArray);
    this.rebuidItemArray = [];
    this.initTree(roots);

  }

  public getChildrenNodes(object: T) {
    return this.flatObjectsArray.filter(element => CxObjectUtil.getPropertyValue(element, this.parentIdFieldRoute)
      === CxObjectUtil.getPropertyValue(object, this.idFieldRoute));
  }

  public getNumberOfChildren(field: CxExtensiveTreeModel) {
    return CxObjectUtil.getPropertyValue(field.object, this.countChildrenFieldRoute);
  }

  public getDisplayText(field: CxExtensiveTreeModel) {
    const item = field.object;
    return CxObjectUtil.getPropertyValue(item, this.displayFieldRoute);
  }

  public setPaddingNode(field: CxExtensiveTreeModel) {
    const width = field.level * 32;
    return `${width}px`;
  }

  public setDisplayValue(element: CxExtensiveTreeModel, value: boolean) {
    if (element) {
      element.isDisplay = value;
    }
  }
  public getAvatar(field: CxExtensiveTreeModel) {
    if (this.dynamicIcon && this.dynamicIcon.length) {
      let icon;
      this.dynamicIcon.forEach(dynamicIcon => {
        if (dynamicIcon(field.object)) {
          icon = dynamicIcon(field.object);
          return;
        }
      });
      if (icon) { return icon; }
    }
    const children = CxObjectUtil.getPropertyValue(field.object, this.countChildrenFieldRoute);
    const folderIcon = 'icon icon-folder';
    const folderEmptyIcon = 'icon icon-folder-empty';
    return children > 0 ? folderIcon : folderEmptyIcon;
  }

  public onAddNew(field: CxExtensiveTreeModel, index: number) {
    field.isEditingItem = false;
    field.isAddingNewItem = true;
    this.addItem.emit({ parentObject: field.object, level: field.level });
  }

  public onSaveNewItem(field: CxExtensiveTreeModel) {
    this.addItem.emit({ parentObject: field.object, level: field.level });
    field.isAddingNewItem = false;
  }

  public executeAddItem(addedObject: T, level: number) {
    const parentItem = this.itemArray.find(x =>
      CxObjectUtil.getPropertyValue(x.object, this.idFieldRoute) === CxObjectUtil.getPropertyValue(addedObject, this.parentIdFieldRoute));
    if (parentItem && !parentItem.showChildren) {
      this.clickExpandShowChildren(parentItem, this.itemArray.indexOf(parentItem));
      this.changeDetectorRef.detectChanges();
    }
  }

  public onEdit(field: CxExtensiveTreeModel) {
    const editingObject = Object.assign({}, field.object);
    this.editItem.emit(editingObject);
  }

  public executeEditItem(editedObject: T) {
    const editItem = this.itemArray.find(x =>
      CxObjectUtil.getPropertyValue(x.object, this.idFieldRoute)
      === CxObjectUtil.getPropertyValue(editedObject, this.idFieldRoute));
    if (editItem) {
      editItem.object = editedObject;
      editItem.isEditingItem = false;
      this.changeDetectorRef.detectChanges();
    }
  }

  public onSave(field: CxExtensiveTreeModel) {
    this.editItem.emit(field.object);
  }

  public onClickItem(field: CxExtensiveTreeModel) {
    this.selectedItem = field;
    this.changeDetectorRef.detectChanges();
    this.clickItem.emit(field);
  }

  public onDeleteItem(field: CxExtensiveTreeModel) {
    this.deleteItem.emit(field);
  }
  public clickExpandShowChildren(item: CxExtensiveTreeModel, index?: number) {
    this.selectedItem = item;
    if (item) {
      if (item.showChildren) {
        this.collapseItem(item);
        item.showChildren = !item.showChildren;
        this.changeDetectorRef.detectChanges();
      } else {
        if (!item.isLoadChildren) {
          item.showChildren = !item.showChildren;
          this.loadChildren.emit(item.object);
        } else {
          this.expandItem(item);
          item.showChildren = !item.showChildren;
          this.changeDetectorRef.detectChanges();
        }
      }
    }
  }


  public onClose(field: CxExtensiveTreeModel) {
    field.isEditingItem = false;
    field.isAddingNewItem = false;
    this.changeDetectorRef.detectChanges();
  }

  public collapseItem(field: CxExtensiveTreeModel) {
    if (field.children && field.children.length > 0) {
      field.children.forEach(element => {
        const selectedItem = this.itemArray.find(item =>
          CxObjectUtil.getPropertyValue(item.object, this.idFieldRoute) === CxObjectUtil.getPropertyValue(element, this.idFieldRoute));
        if (selectedItem) {
          this.setDisplayValue(selectedItem, false);
          this.collapseItem(selectedItem);
        }
      });
    } else {
      this.setDisplayValue(field, false);
    }
  }

  public expandItem(field: CxExtensiveTreeModel) {
    if (field.children && field.children.length > 0) {
      field.children.forEach(element => {
        const selectedItem = this.itemArray.find(item =>
          CxObjectUtil.getPropertyValue(item.object, this.idFieldRoute) === CxObjectUtil.getPropertyValue(element, this.idFieldRoute));
        if (selectedItem) {
          this.setDisplayValue(selectedItem, true);
          if (selectedItem.showChildren) {
            this.expandItem(selectedItem);
          }
        }
      });
    }
  }
}
