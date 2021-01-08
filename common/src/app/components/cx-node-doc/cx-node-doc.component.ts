import { Component, OnInit, ViewChild } from '@angular/core';
import { NodeStatusType, CxNode } from 'projects/cx-angular-common/src/lib/components/cx-node/models/cx-node.model';
import { CxNodeComponent } from 'projects/cx-angular-common/src';

@Component({
  selector: 'cx-node-doc',
  templateUrl: './cx-node-doc.component.html',
  styleUrls: ['./cx-node-doc.component.scss']
})
export class CxNodeDocComponent implements OnInit {
  public treeData = new CxNode('');
  public selectedNodeComponent: CxNodeComponent;
  @ViewChild(CxNodeComponent) treeComponent: CxNodeComponent;
  constructor() {
  }

  ngOnInit() {
    const node1: CxNode = {
      name: 'Learning Plan 001',
      iconClass: 'icon-opj-lp',
      status: {
        shortName: 'LP',
        type: NodeStatusType.Pending,
        text : 'Acknowledged '
      },
      dataObject: {},
      children: [],
      hideChildren: false
    };

    const node2: CxNode = {
      name: 'Digital Learinng: Design UI/UX advanced courses',
      iconClass: 'icon-opj-ld',
      status: {
        shortName: 'LD',
        type: NodeStatusType.Approved,
        text : 'Approved'
      },
      dataObject: {},
      children: [],
      canCreateChildren: true,
      createChildrenIcon: 'icon-add',
      hideChildren: false
    };
    const node3: CxNode = {
      name: 'Digital Literacy: Technology Literacy',
      iconClass: 'icon-opj-klp',
      dataObject: {},
      children: [],
      subName: 'All officers',
      hideChildren: false
    };
    node2.children = [this.clone(node3), this.clone(node3), this.clone(node3), this.clone(node3)];
    node1.children = [this.clone(node2), this.clone(node2), this.clone(node2)];
    this.treeData = node1;
  }

  nodeClickedHandler(nodeComponent: CxNodeComponent) {
    console.log('Clicked: ' + nodeComponent);
  }

  createNodeClickedHandler(nodeComponent: CxNodeComponent) {
    console.log('Create children for: ' + nodeComponent);
  }

  newNodeCreatedHandler(component: CxNodeComponent) {
    console.log(component);
    this.selectedNodeComponent = component;
  }

  clone(obj) {
    return JSON.parse(JSON.stringify(obj));
  }

  rootUpdate() {
  }
}
