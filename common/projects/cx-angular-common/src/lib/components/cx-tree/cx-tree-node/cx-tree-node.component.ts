import {
    Component,
    OnInit,
    Input,
    ViewEncapsulation,
    ViewChildren,
    Output,
    EventEmitter,
    QueryList,
    ChangeDetectionStrategy,
    ChangeDetectorRef,
    TemplateRef,
    ElementRef,
    ViewChild
} from '@angular/core';
import CxTreeUtil from '../../../utils/tree.util';
import {
  clone,
  cloneDeep
} from 'lodash';
import { CxObjectRoute } from '../models/cx-object-route.model';
import { CxTreeButtonCondition } from '../models/cx-tree-button-condition.model';
import { CxTreeText } from '../models/cx-tree-text.model';
import { CxTreeIcon } from '../models/cx-tree-icon.model';
import { CxObjectUtil } from '../../../utils/object.util';
import { CxAnimations } from '../../../constants/cx-animation.constant';
@Component({
    selector: 'cx-tree-node',
    templateUrl: './cx-tree-node.component.html',
    styleUrls: ['./cx-tree-node.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: [
        CxAnimations.smoothAppendRemove
    ],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxTreeNodeComponent<T> implements OnInit {
    @Input() level: number;
    @Input() isViewMode: boolean;
    @Input() buttonCondition: CxTreeButtonCondition<T> = new CxTreeButtonCondition();
    @Input() dynamicIcon: ((object: T) => string)[];
    @Input() text: CxTreeText = new CxTreeText();
    @Input() icon: CxTreeIcon = new CxTreeIcon();
    @Input() displayTemplate: TemplateRef<any>;
    @Input() object: T;
    @Input() remainingObjectsArray: T[]; // To push to children
    @Input() displayFieldRoute: string;
    @Input() idFieldRoute: string;
    @Input() parentIdFieldRoute: string;
    @Input() isRoot: boolean;
    @Input() route: {} = {};
    @Input() public get currentRoutes(): any[] {
        return this._currentRoutes;
    }
    @Input() public get selectedObjectIds(): {} {
        return this._selectedObjectIds;
    }
    @Input() public get destinationItems(): any[] {
        return this._destinationItems;
    }
    @Input() displayCheckboxes: boolean;
    @Input() defaultExpandLevel: number;
    @Output() objectChange: EventEmitter<T> = new EventEmitter();
    @Output() move: EventEmitter<CxObjectRoute<T>> = new EventEmitter();
    @Output() clickItem: EventEmitter<CxObjectRoute<T>> = new EventEmitter();

    public childrenObjects: any[] = [];
    public remainingObjectArrayToPushDownChildren: T[];
    public showChildren = false;
    public isAddingNewItem: boolean;
    public isEditingItem: boolean;
    public name = '';
    public checked: boolean;
    public objectId = '';

    public enableAddBtn = true;
    public enableEditBtn = true;
    public enableMoveBtn = true;
    public enableRemoveBtn = true;
    public showCollapseExpandButton = true;

    public isActive: boolean;
    public isChosen: boolean;
    public routeDepartments = {};
    private _currentRoutes = [];
    private _destinationItems = [];
    protected _selectedObjectIds = {};
    @Output() editItem: EventEmitter<T> = new EventEmitter();
    @Output() deleteItem: EventEmitter<CxObjectRoute<T>> = new EventEmitter();
    @Output() addItem: EventEmitter<{ parentObject: T, childName: string }> = new EventEmitter();
    @Output() selectItem: EventEmitter<CxObjectRoute<T>> = new EventEmitter();
    @ViewChildren(CxTreeNodeComponent) treeNodeComponents: QueryList<CxTreeNodeComponent<T>>;
    @ViewChild('editInput') editInput: ElementRef;
    @ViewChild('addInput') addInput: ElementRef;
    public get currentPadding() {
        return this.level * 40;
    }
    public get childPadding() {
        return (this.level + 1) * 40;
    }
    constructor(protected changeDetectorRef: ChangeDetectorRef) {
    }

    public set selectedObjectIds(objectIds: {}) {
        this._selectedObjectIds = objectIds;
        if (this.objectId !== '') {
            this.isChosen = this.isViewMode
                && (this.routeDepartments[this.objectId] !== undefined)
                && (this.selectedObjectIds[this.objectId] !== undefined)
                && (this.routeDepartments[this.objectId].toString() === this.selectedObjectIds[this.objectId].toString());
        }
        this.isActive = false;
    }

    public set destinationItems(items: any[]) {
        this._destinationItems = items;
        this.isActive = this.destinationItems &&
            Object.keys(this.destinationItems).length > 0 && this.destinationItems[this.objectId] === this.objectId;
    }

    public set currentRoutes(routes: any[]) {
        this._currentRoutes = routes;
        if (this.objectId !== '') {
            this.showChildren = this.currentRoutes && (this.currentRoutes.findIndex(route => route[this.objectId] !== undefined) > -1);
        }
    }

    ngOnInit() {
        this.childrenObjects = Object.values(
            CxTreeUtil.getChildrenIdsMap(
                this.remainingObjectsArray,
                this.object,
                this.idFieldRoute,
                this.parentIdFieldRoute
            )
        );
        this.remainingObjectArrayToPushDownChildren = CxTreeUtil.getObjectsExceptCurrentObjectAndDirectChildren(
            this.remainingObjectsArray,
            this.object,
            this.idFieldRoute,
            this.parentIdFieldRoute
        );
        this.objectId = CxObjectUtil.getPropertyValue(this.object, this.idFieldRoute);
        this.showChildren = this.currentRoutes && (this.currentRoutes.findIndex(route => route[this.objectId] !== undefined) > -1);
        this.routeDepartments = clone(this.route);
        this.routeDepartments[this.objectId] = this.objectId;
        this.isChosen = this.isViewMode
            && (this.routeDepartments[this.objectId] !== undefined)
            && (this.selectedObjectIds[this.objectId] !== undefined)
            && (this.routeDepartments[this.objectId].toString() === this.selectedObjectIds[this.objectId].toString());

        this.initButtons();
        this.showChildren = this.level < this.defaultExpandLevel;
    }

    public get nodeStatusClass(): string {
        if (this.checked) { return 'cx-tree-node__current-data--selected'; }
        if (this.isViewMode) {
            if (this.isChosen) {
                return 'cx-tree-node__current-data--chosen';
            }
            if (this.isActive) {
                return 'cx-tree-node__current-data--selected';
            }
            return 'cx-tree-node__current-data--active';
        }
    }

    public initButtons() {
        // TODO: should call the function directly in the template (to be able to update)
        this.enableAddBtn = this.buttonCondition.enableAdd(this.object);
        this.enableEditBtn = this.buttonCondition.enableEdit(this.object);
        this.enableMoveBtn = this.buttonCondition.enableMove(this.object);
        this.enableRemoveBtn = this.buttonCondition.enableRemove(this.object);
        this.showCollapseExpandButton = this.buttonCondition.showCollapseExpand(this.object);
    }

    public onToggleShowingChildrenClicked() {
        this.showChildren = !this.showChildren;
    }

    public onAddNew() {
        this.isEditingItem = false;
        this.name = '';
        this.showChildren = true;
        this.isAddingNewItem = true;
        setTimeout(() =>
            this.addInput.nativeElement.focus());
    }

    public onClose() {
        this.isEditingItem = false;
        this.isAddingNewItem = false;
    }

    public onMoveItemFromChildrenFolder(currentRouteAndObject: CxObjectRoute<T>) {
        this.move.emit({ route: currentRouteAndObject.route, object: currentRouteAndObject.object });
    }

    public onMoveItemFromCurrentFolder() {
        this.move.emit({ route: this.route, object: this.object });
    }

    public onSelectItemFromCurrentItem() {
        this.isActive = true;
        this.clickItem.emit({ route: this.route, object: this.object });
    }

    public onSelectItemFromChildrenItem(currentRouteAndObject: CxObjectRoute<T>) {
        this.clickItem.emit(currentRouteAndObject);
    }

    public get avatar() {
        if (this.dynamicIcon && this.dynamicIcon.length) {
            let icon;
            this.dynamicIcon.forEach(dynamicIcon => {
                if (dynamicIcon(this.object)) {
                    icon = dynamicIcon(this.object);
                    return;
                }
            });
            if (icon) { return icon; }
        }
        if (this.isRoot) {
            return this.icon.root;
        }
        return this.childrenObjects.length ? this.icon.node : this.icon.emptyNode;
    }

    public onEdit() {
        this.isAddingNewItem = false;
        this.isEditingItem = true;
        this.name = CxObjectUtil.getPropertyValue(
            this.object,
            this.displayFieldRoute
        );
        setTimeout(() =>
            this.editInput.nativeElement.focus());
    }

    public onDelete() {
        const objectRoute = new CxObjectRoute<T>({
            object: this.object,
            route: this.route
        });
        this.deleteItem.emit(objectRoute);
    }

    public onSaveNewItem() {
        this.addItem.emit({ parentObject: this.object, childName: this.name });
    }

    public onSave() {
        const editingObject = Object.assign({}, this.object);
        CxObjectUtil.setPropertyValue(editingObject, this.displayFieldRoute, this.name);
        this.editItem.emit(editingObject);
    }

    public executeMoveItem(originObjectRoute: CxObjectRoute<T>, destinationId: any) {
        this.executeDeleteItem(originObjectRoute);
        const newObjectRoute = cloneDeep(originObjectRoute);
        CxObjectUtil.setPropertyValue(newObjectRoute.object, this.parentIdFieldRoute, destinationId);
        this.executeAddItem(newObjectRoute.object);
    }

    public executeAddItem(addedItem: T) {
        const addedObjectIsChildOfCurrentObject =
            CxObjectUtil.getPropertyValue(addedItem, this.parentIdFieldRoute)
            === CxObjectUtil.getPropertyValue(this.object, this.idFieldRoute
            );
        if (addedObjectIsChildOfCurrentObject) {
            this.childrenObjects.unshift(addedItem);
            this.isAddingNewItem = false;
            this.showChildren = true;
            this.changeDetectorRef.detectChanges();
        } else {
            this.treeNodeComponents.forEach(item => {
                item.executeAddItem(addedItem);
            });
        }
    }

    public executeDeleteItem(deletedObjectRoute: CxObjectRoute<T>) {
        const deletedObjectIsChildOfCurrentObject =
            this.childrenObjects.findIndex(x => CxObjectUtil.getPropertyValue(x, this.idFieldRoute)
                === CxObjectUtil.getPropertyValue(deletedObjectRoute.object, this.idFieldRoute)) > -1;
        if (deletedObjectIsChildOfCurrentObject) {
            const childIndex = this.childrenObjects.findIndex(x =>
                CxObjectUtil.getPropertyValue(x, this.idFieldRoute)
                === CxObjectUtil.getPropertyValue(deletedObjectRoute.object, this.idFieldRoute
                ));
            this.childrenObjects.splice(childIndex, 1);
            this.changeDetectorRef.detectChanges();
        } else {
            this.treeNodeComponents.forEach(item => {
                item.executeDeleteItem(deletedObjectRoute);
            });
        }
    }

    public executeUpdateItem(currentObjectIdFieldRoute: any, newObject: T) {
        const editedObjectIsCurrentObject =
            CxObjectUtil.getPropertyValue(this.object, this.idFieldRoute)
            === currentObjectIdFieldRoute;
        if (editedObjectIsCurrentObject) {
            this.object = newObject;
            this.isEditingItem = false;
            this.changeDetectorRef.detectChanges();
        } else {
            this.treeNodeComponents.forEach(item => {
                item.executeUpdateItem(currentObjectIdFieldRoute, newObject);
            });
        }
    }

    public executeEditItem(editedObject: T) {
        const editedObjectIsCurrentObject =
            CxObjectUtil.getPropertyValue(this.object, this.idFieldRoute)
            === CxObjectUtil.getPropertyValue(editedObject, this.idFieldRoute
            );
        if (editedObjectIsCurrentObject) {
            this.object = editedObject;
            this.isEditingItem = false;
            this.changeDetectorRef.detectChanges();
        } else {
            this.treeNodeComponents.forEach(item => {
                item.executeEditItem(editedObject);
            });
        }
    }

    public unselect() {
        this.checked = false;
        this.changeDetectorRef.detectChanges();
        this.treeNodeComponents.forEach(item => {
            item.unselect();
        });
    }

    public toggleCheckbox() {
        this.selectItem.emit({ route: this.route, object: this.object });
    }

    //#region Parent Context
    public onDeleteItem(deletingObject: CxObjectRoute<T>) {
        this.deleteItem.emit(deletingObject);
    }

    public onAddItem(savingItem: { parentObject: T, childName: string }) {
        this.addItem.emit(savingItem);
    }

    public onEditItem(editingItem: T) {
        this.editItem.emit(editingItem);
    }

    public onSelectItem(selectedItem: CxObjectRoute<T>) {
        this.selectItem.emit(selectedItem);
    }
    //#endregion
}
