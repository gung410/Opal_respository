import { Component, ViewEncapsulation, ChangeDetectionStrategy, ChangeDetectorRef, QueryList, ViewChildren, Input, TemplateRef, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CxTreeNodeComponent } from '../../cx-tree-node/cx-tree-node.component';
import { CxAnimations } from '../../../../constants/cx-animation.constant';
import { CxObjectUtil } from '../../../../utils/object.util';

@Component({
    selector: 'cx-narrow-tree-node',
    templateUrl: './cx-narrow-tree-node.component.html',
    styleUrls: ['./cx-narrow-tree-node.component.scss'],
    encapsulation: ViewEncapsulation.None,
    changeDetection: ChangeDetectionStrategy.OnPush,
    animations: [
        CxAnimations.smoothAppendRemove
    ],
})
export class CxNarrowTreeNodeComponent<T> extends CxTreeNodeComponent<T> {
    public currentDataHorizontalLinePathLengthInPx = 18;
    public get childrenVerticalLinePathLengthInPx(): number {
        return this.currentPadding + 18;
    }
    @Input() navigatedNodeData: T;
    @Input() addButtonTemplate: TemplateRef<any>;
    @Input() backgroundColor: string;
    @ViewChildren(CxNarrowTreeNodeComponent) treeNodeComponents: QueryList<CxNarrowTreeNodeComponent<T>>;

    constructor(changeDetectorRef: ChangeDetectorRef) {
        super(changeDetectorRef);
    }

    public executeAddItem(addedItem: T) {
        const addedObjectIsChildOfCurrentObject =
            CxObjectUtil.getPropertyValue(addedItem, this.parentIdFieldRoute)
            === CxObjectUtil.getPropertyValue(this.object, this.idFieldRoute
            );
        if (addedObjectIsChildOfCurrentObject) {
            this.childrenObjects.push(addedItem);
            this.isAddingNewItem = false;
            this.showChildren = true;
            this.changeDetectorRef.detectChanges();
        } else {
            this.treeNodeComponents.forEach(item => {
                item.executeAddItem(addedItem);
            });
        }
    }

    public get isNavigatedNode(): boolean {
        const isCurrentNodeNavigated = CxObjectUtil.getPropertyValue(this.object, this.idFieldRoute)
        === CxObjectUtil.getPropertyValue(this.navigatedNodeData, this.idFieldRoute);
        return isCurrentNodeNavigated;
    }

    public onAddNew() {
        this.addItem.emit({parentObject: this.object, childName: ''});
    }
}
