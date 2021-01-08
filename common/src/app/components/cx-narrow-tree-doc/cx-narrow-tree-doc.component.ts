import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { CxTreeDocComponent } from '../cx-tree/cx-tree-doc.component';
import { CxNarrowTreeComponent, CxTreeIcon, CxTreeButtonCondition } from 'projects/cx-angular-common/src';

@Component({
  selector: 'cx-narrow-tree-doc',
  templateUrl: './cx-narrow-tree-doc.component.html',
  styleUrls: ['./cx-narrow-tree-doc.component.scss']
})
export class CxNarrowTreeDocComponent extends CxTreeDocComponent {
  public iconsData = new CxTreeIcon({
    collapse: 'material-icons arrow-drop-down',
    expand: 'material-icons arrow-right',
    root: ' ',
    emptyNode: ' ',
    node: ' ',
    add: 'material-icons add'
  });
  public currentRoutes: any;
  public currentNavigatedDepartment: any;
  public buttonConditions = new CxTreeButtonCondition({
    enableEdit: () => false,
    enableMove: () => false,
    enableRemove: () => false
  });
  selectedObjectIds: any = {};
  @ViewChild(CxNarrowTreeComponent) treeComponent: CxNarrowTreeComponent<any>;
  constructor(cdRef: ChangeDetectorRef) {
    super(cdRef);
  }

  ngOnInit() {
  }


  onSelectItem(item: { object: any, route: any }) {
    this.selectedObjectIds = {};
    this.selectedObjectIds[item.object.identity.id] = item.object.identity.id;
    setTimeout(() => {
      this.currentNavigatedDepartment = item.object;
    }, 1000);
  }

  onAddDepartment(addingData: { parentObject: any, childName: string }) {
    console.log(addingData);
  }

  clone(obj) {
    return JSON.parse(JSON.stringify(obj));
  }
}
