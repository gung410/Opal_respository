import { Component, OnInit, ViewChild } from '@angular/core';
import { CxTreeComponent } from 'projects/cx-angular-common/src';
import { flatDepartmentsArray } from 'src/app/data/tree-data';
import { buttonConditionsData } from 'src/app/data/button-condition-data';
import { iconsData } from 'src/app/data/icon-data';
import { textData } from 'src/app/data/text-data';
@Component({
  selector: 'cx-tree-dropdown-doc',
  templateUrl: './cx-tree-dropdown-doc.component.html',
  styleUrls: ['./cx-tree-dropdown-doc.component.scss']
})
export class CxTreeDropdownDocComponent implements OnInit {

  public flatDepartmentsArray = flatDepartmentsArray;
  public buttonConditions = buttonConditionsData;
  public iconsData = iconsData;
  public textData = textData;
  @ViewChild(CxTreeComponent) treeComponent: CxTreeComponent<any>;
  public currentRoutes = [
    {
      1: '1',
      7504: '7504',
      13732: '13732',
      13733: '13733',
      13734: '13734',
      13735: '13735'
    }
  ];

  public selectedObjectIds: any = {
    13735: '13735'
  };

  constructor() {}

  ngOnInit() {
  }

  public onSelectItem(item: any) {
    this.selectedObjectIds = {};
    this.selectedObjectIds[item.object.identity.id] = item.object.identity.id;
  }
}
