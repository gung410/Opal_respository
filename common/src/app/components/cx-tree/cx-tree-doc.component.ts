import { Component, OnInit, ViewChild, ViewEncapsulation, ViewChildren, QueryList, ChangeDetectorRef } from '@angular/core';
import { buttonConditionsData } from 'src/app/data/button-condition-data';
import { iconsData } from 'src/app/data/icon-data';
import { textData } from 'src/app/data/text-data';
import { flatDepartmentsArray, flatDepartmentsArray2 } from 'src/app/data/tree-data';
import { CxTreeIcon, CxTreeButtonCondition, CxTreeComponent, CxTreeText } from 'projects/cx-angular-common/src';
import { CxCheckboxComponent } from 'projects/cx-angular-common/src/lib/components/cx-checkbox/cx-checkbox.component';
import { CxExtensiveTreeComponent } from 'projects/cx-angular-common/src/lib/components/cx-extensive-tree/cx-extensive-tree.component';

@Component({
  selector: 'cx-tree-doc',
  templateUrl: './cx-tree-doc.component.html',
  styleUrls: ['./cx-tree-doc.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CxTreeDocComponent implements OnInit {
  public flatDepartmentsArray = flatDepartmentsArray;
  public flatDepartmentsArray2 = flatDepartmentsArray2;
  public rootFlatDepartmentsArray = [];
  public havingExtensiveArea = true;
  public buttonConditions = new CxTreeButtonCondition(buttonConditionsData);
  public iconsData = new CxTreeIcon(iconsData);
  public textData = new CxTreeText(textData);
  @ViewChild(CxTreeComponent) treeComponent: CxTreeComponent<any>;
  @ViewChild(CxExtensiveTreeComponent) extensiveTreeComponent: CxExtensiveTreeComponent<any>;
  @ViewChildren('checkbox') checkboxList: QueryList<CxCheckboxComponent>;
  public isInlineMode = true;
  public checkboxes = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
  icon: CxTreeIcon = new CxTreeIcon();
  public dynamicIcon = [
    (x: any) => x.identity.archetype === 'Country' ? 'material-icons close-icon-global' : null,
  ];

  constructor(private cdRef: ChangeDetectorRef) { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.cdRef.detectChanges();
  }

  getChildrenNode(parentId: number) {
    return this.flatDepartmentsArray.filter(x => x.parentDepartmentId === parentId);
  }


  public onDeleteDepartment(deletedDepartments: any[]) {
    setTimeout(() => {
      deletedDepartments.forEach(item => {
        this.flatDepartmentsArray = this.flatDepartmentsArray.filter(dep => dep.identity.id !== item.object.identity.id);
      });
      this.treeComponent.executeDeleteItem(deletedDepartments);
    }, 100);
  }

  public onDeleteDepartmentExtensive(deletedDepartment: any) {
    setTimeout(() => {
      this.treeComponent.executeDeleteItem(deletedDepartment);
    }, 100);
  }

  public onMoveItemToDestination(moveItemToDestinationObject: {
    originObjects: any[];
    destinationObjectId: any;
  }) {
    setTimeout(() => {
      this.flatDepartmentsArray.map(department => {
        moveItemToDestinationObject.originObjects.map(item => {
          if (item.object.identity.id === department.identity.id) {
            department.parentDepartmentId = moveItemToDestinationObject.destinationObjectId;
          }
          return department;
        });
      });
      this.treeComponent.executeMoveItem(
        moveItemToDestinationObject.originObjects,
        moveItemToDestinationObject.destinationObjectId
      );
    }, 1000);
  }

  public onAddDepartment(addingDepartmentData: { parentObject: any, childName: string, level?: number }) {
    setTimeout(() => {
      const newChild = {
        parentDepartmentId: addingDepartmentData.parentObject.identity.id,
        identity: {
          extId: '',
          ownerId: 2001,
          customerId: 1792,
          archetype: 'Country',
          id: Math.floor(Math.random() * 100000) + 1
        },
        departmentInfo: { name: addingDepartmentData.childName },
        departmentDescription: ''
      };
      this.flatDepartmentsArray.push(newChild);
      this.treeComponent.executeAddItem(newChild);
    }, 100);
  }

  public onEditDepartment(editedDepartmentData: any) {
    setTimeout(() => {
      const currentDepartmentData = this.rootFlatDepartmentsArray.find(x => x.identity.id === editedDepartmentData.identity.id);
      const foundIndex = this.rootFlatDepartmentsArray.findIndex(x => x.identity.id === editedDepartmentData.identity.id);
      if (foundIndex > 0) {
        this.rootFlatDepartmentsArray[foundIndex] = editedDepartmentData;
        this.rootFlatDepartmentsArray = [...this.rootFlatDepartmentsArray];
        this.cdRef.detectChanges();
      }
      if (currentDepartmentData) {
        currentDepartmentData.departmentInfo = editedDepartmentData.departmentInfo;
        currentDepartmentData.departmentDescription = editedDepartmentData.departmentDescription;
        this.treeComponent.executeEditItem(currentDepartmentData);
      }
    }, 100);
  }

  public onCollapsibleSubmitted(submittedValue: string) {
  }

  public onSubmitted(submittedValue) {
  }

  public onCxBreadCrumbClick(data: any): void {
  }

  public buildOrderedDepartments(
    filteredDepartments: any[], hierarchicalUserDepartments: any[], userDepartmentId: number, currentDepartmentId: number): any[] {
    if (!filteredDepartments) {
      filteredDepartments = [];
    }
    for (const department of hierarchicalUserDepartments) {
      if (department.identity.id === currentDepartmentId) {
        filteredDepartments.push(department);
        if (department.identity.id !== userDepartmentId) {
          currentDepartmentId = department.parentDepartmentId;
          return this.buildOrderedDepartments(
            filteredDepartments, hierarchicalUserDepartments, userDepartmentId, currentDepartmentId);
        }
      }
    }
    return filteredDepartments;
  }

}
