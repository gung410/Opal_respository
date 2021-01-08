import { Component, ViewEncapsulation, ChangeDetectionStrategy, ViewChild, Input, TemplateRef } from '@angular/core';
import { CxTreeComponent } from '../cx-tree.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CxNarrowTreeNodeComponent } from './cx-narrow-tree-node/cx-narrow-tree-node.component';
import { CxAnimations } from '../../../constants/cx-animation.constant';

@Component({
    selector: 'cx-narrow-tree',
    templateUrl: './cx-narrow-tree.component.html',
    styleUrls: ['./cx-narrow-tree.component.scss'],
    encapsulation: ViewEncapsulation.None,
    changeDetection: ChangeDetectionStrategy.OnPush,
    animations: [CxAnimations.smoothAppendRemove],
})
export class CxNarrowTreeComponent<T> extends CxTreeComponent<T> {
    @ViewChild(CxNarrowTreeNodeComponent) treeNodeComponent: CxNarrowTreeNodeComponent<T>;
    @Input() navigatedNodeData: T;
    @Input() maxWidth = '100%';
    @Input() addButtonTemplate: TemplateRef<any>;
    @Input() backgroundColor = '#fff';
    constructor(ngbModal: NgbModal) {
        super(ngbModal);
    }
}
