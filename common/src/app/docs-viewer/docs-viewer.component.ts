import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { flatDepartmentsArray } from '../data/tree-data';
import { iconsData } from '../data/icon-data';
import { textData } from '../data/text-data';
import { buttonConditionsData } from '../data/button-condition-data';
import { IdentityClaims } from '../data/identityClaims-data';
import { CxMenuItem, CxTreeComponent, CurrentUser, ActionsModel } from 'projects/cx-angular-common/src';

@Component({
  selector: "cxlib-docs-viewer",
  templateUrl: './docs-viewer.component.html',
  styleUrls: ['./docs-viewer.component.scss']
})
export class DocsViewerComponent implements OnInit {
  public flatDepartmentsArray = flatDepartmentsArray;
  public buttonConditions = buttonConditionsData;
  public iconsData = iconsData;
  public textData = textData;
  @ViewChild(CxTreeComponent) treeComponent: CxTreeComponent<any>;
  public currentUser = new CurrentUser(IdentityClaims);
  // These 2 variables are for testing purpose of tree dropdown
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
  public selectedObjectIds = {
    13735: '13735'
  };


  public mainMenuHeader: CxMenuItem[] = [
    {
      label: 'Landing Page',
      routingLink: '/landing-page'
    },
    {
      label: 'Monitor',
      routingLink: '/monitor'
    },
    {
      label: 'Management',
      routingLink: '/management'
    }
  ];
  filteredDepartments: [];
  displayTemplate: TemplateRef<any>;
  constructor() { }

  ngOnInit() {
  }

  public onBtnClicked(e) {
    alert('Clicked: ' + e);
  }

  public onBtnClosed(e) {
    alert('closed: ' + e);
  }

  public onSelectItemFromDropdownMenu(object: any) {
  }

  public onDeleteDepartment(deletedDepartments: any[]) {
    setTimeout(() => {
      deletedDepartments.forEach(item => {
        this.flatDepartmentsArray = this.flatDepartmentsArray.filter(dep => dep.identity.id !== item.identity.id);
      });
      this.treeComponent.executeDeleteItem(deletedDepartments);
    }, 100);
  }

  public onMoveItemToDestination(moveItemToDestinationObject: {
    originObjects: any[];
    destinationObjectId: any;
  }) {
    setTimeout(() => {
      this.flatDepartmentsArray.map(department => {
        moveItemToDestinationObject.originObjects.map(object => {
          if (object.identity.id === department.identity.id) {
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


  public onAddDepartment(addingDepartmentData: { parentObject: any, childName: string }) {
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
      const currentDepartmentData = this.flatDepartmentsArray.find(x => x.identity.id === editedDepartmentData.identity.id);
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

  onCxBreadCrumbClick($event: ActionsModel) {
  }
}
