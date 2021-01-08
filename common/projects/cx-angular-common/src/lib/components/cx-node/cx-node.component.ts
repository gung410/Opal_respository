import {
  Component,
  ViewEncapsulation,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Input,
  Output,
  EventEmitter,
  ViewChildren,
  QueryList,
  SimpleChanges,
  OnChanges,
  AfterViewInit
} from '@angular/core';

import { CxAnimations } from '../../constants/cx-animation.constant';
import { CxNode } from './models/cx-node.model';
@Component({
  selector: 'cx-node',
  templateUrl: './cx-node.component.html',
  styleUrls: ['./cx-node.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: [CxAnimations.smoothAppendRemove],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxNodeComponent implements OnChanges, AfterViewInit {
  @Input() node: CxNode;
  @Input() selectedNodeComponent: CxNodeComponent;
  @Input() level = 0; // Don't input this properties when use, it only use for  internal
  @Output() nodeClicked = new EventEmitter<any>();
  @Output() createNodeClicked = new EventEmitter<any>();
  @Output() newNodeCreated = new EventEmitter<any>();
  @Output() ready = new EventEmitter<CxNodeComponent>();
  @ViewChildren(CxNodeComponent) treeNodeComponents: QueryList<CxNodeComponent>;

  currentComponent = this;
  constructor(protected changeDetectorRef: ChangeDetectorRef) {}

  ngOnChanges(changes: SimpleChanges) {
    if (!changes || !changes.node) {
      return;
    }

    if (!changes.node.previousValue) {
      // New node added
      this.newNodeCreated.emit(this.currentComponent);
    }
  }

  ngAfterViewInit(): void {
    this.ready.emit(this);
  }

  refresh() {
    this.changeDetectorRef.detectChanges();
  }

  nodeClickedHandler(component: CxNodeComponent) {
    this.nodeClicked.emit(component);
  }

  createNodeClickedHandler(component: CxNodeComponent) {
    this.createNodeClicked.emit(component);
  }

  newNodeCreatedHandler(component: CxNodeComponent) {
    this.newNodeCreated.emit(component);
  }

  findComponentById(id: string): CxNodeComponent {
    if (id === undefined || id === '' || !this.node) {
      return;
    }

    if (id === this.node.id) {
      return this;
    }

    if (!this.treeNodeComponents) {
      return;
    }

    const childrenNodeComponent = this.treeNodeComponents.toArray();

    for (const nodeComponent of childrenNodeComponent) {
      const result = nodeComponent.findComponentById(id);
      if (result) {
        return result;
      }
    }
  }
}
