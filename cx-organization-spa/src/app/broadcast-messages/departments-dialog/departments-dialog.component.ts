import {
  ChangeDetectorRef,
  Component,
  Inject,
  OnDestroy,
  OnInit,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CxExtensiveTreeComponent,
  CxFormModal,
  CxGlobalLoaderService,
  CxTreeButtonCondition,
  CxTreeText
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { DepartmentType } from 'app-models/department-type.model';
import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { SurveyUtils } from 'app-utilities/survey-utils';
import { DepartmentHierarchicalService } from 'app/department-hierarchical/department-hierarchical.service';
import { Department } from 'app/department-hierarchical/models/department.model';
import {
  DepartmentFilterGroupModel,
  DepartmentFilterOption,
  DepartmentQueryModel
} from 'app/department-hierarchical/models/filter-params.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { GroupFilterConst } from 'app/shared/constants/filter-group.constant';
import { UserAccountsHelper } from 'app/user-accounts/user-accounts.helper';
import * as _ from 'lodash';
import { Subscription } from 'rxjs';
import { BroadcastMessagesDetailComponent } from '../broadcast-messages-detail/broadcast-messages-detail.component';
import { DepartmentDialogResult } from '../models/department-dialog-result.model';

@Component({
  selector: 'department-dialog',
  templateUrl: './departments-dialog.component.html',
  styleUrls: ['./departments-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DepartmentsDialogComponent implements OnInit, OnDestroy {
  subscription: Subscription = new Subscription();
  flatDepartmentsArray: Department[] = [];
  selectedDepartments: Department[] = [];
  currentDepartmentId: number;
  selectedDepartmentId: number;
  departmentFilterOptions: DepartmentFilterGroupModel[];
  departmentQueryObject: DepartmentQueryModel = new DepartmentQueryModel({});
  departmentTagData: DepartmentFilterGroupModel[];
  buttonCondition: CxTreeButtonCondition<any>;
  departmentTypeTitle: string;
  currentLanguageCode: string;
  departmentTypes: DepartmentType[];
  text: CxTreeText = new CxTreeText();
  isLoadingDepartment: boolean = false;
  levelOfEducationMap: object = {};
  isNoDepartment: boolean = false;
  departmentTypeList: DepartmentType[];
  departmentPathMap: any = {};
  searchDepartmentResult: any = [];
  departmentRouteMap: any = {};
  isDetectExpandTree: boolean;
  typesOfOrganisation: [] = [];
  searchKey: string = '';
  isSearching: boolean = false;
  searchResultText: string = '';

  get selectedDepartmentIds(): number[] {
    return this.selectedDepartments.map((department) => department.identity.id);
  }

  @ViewChild(CxExtensiveTreeComponent)
  extensiveTreeComponent: CxExtensiveTreeComponent<any>;
  private currentUser: User;
  private readonly NUMBER_OF_REMOVED_ITEM: number = 1;

  constructor(
    private dialogRef: MatDialogRef<BroadcastMessagesDetailComponent>,
    private changeDetectorRef: ChangeDetectorRef,
    private departmentHierarchicalService: DepartmentHierarchicalService,
    private authService: AuthService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private translateAdapterService: TranslateAdapterService,
    private formModal: CxFormModal,
    private toastr: ToastrAdapterService,
    @Inject(MAT_DIALOG_DATA) public injectedDepartments: Department[],
    public ngbModal: NgbModal,
    private translateService: TranslateService
  ) {
    this.selectedDepartments.push(...this.injectedDepartments);
  }

  ngOnInit(): void {
    this.text.header = 'Organization';
    this.initButtonCondition();
    this.buildDepartmentTree();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  buildDepartmentTree(): void {
    this.cxGlobalLoaderService.showLoader();
    this.subscription.add(
      this.authService.userData().subscribe((user) => {
        if (user) {
          this.currentUser = user;
          this.currentLanguageCode = localStorage.getItem('language-code');
          this.currentDepartmentId = user.departmentId;
          this.selectedDepartmentId = user.departmentId;
          this.initFilterObject();
          this.getDepartments(
            UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
          );
          this.getEducationLevels();
          this.getDepartmentTypeList();

          this.subscription.add(
            this.departmentHierarchicalService
              .getTypeOfOrganisation()
              .subscribe((response) => {
                response.forEach((organisation) => {
                  this.typesOfOrganisation[organisation.id] = organisation;
                });
              })
          );
        }
      })
    );
  }

  displayText(department: any): string {
    if (
      !department ||
      !department.jsonDynamicAttributes ||
      !department.jsonDynamicAttributes.typeOfOrganizationUnits
    ) {
      return '';
    }
    const typeOfOrg: any = this.typesOfOrganisation[
      department.jsonDynamicAttributes.typeOfOrganizationUnits
    ];

    return typeOfOrg ? typeOfOrg.displayText : '';
  }

  onSearchDepartment(): void {
    if (!this.searchKey.trim().length) {
      this.onSearchTextCleared();

      return;
    }
    this.cxGlobalLoaderService.showLoader();
    this.departmentQueryObject.searchText = this.searchKey;
    this.departmentQueryObject.maxChildrenLevel = undefined;

    this.departmentHierarchicalService
      .getHierarchy(
        UserAccountsHelper.getDefaultRootDepartment(this.currentUser),
        this.departmentQueryObject
      )
      .subscribe(
        (response: Department[]) => {
          this.flatDepartmentsArray = _.uniqBy(
            this.flatDepartmentsArray.concat(response),
            'identity.id'
          );
          this.searchDepartmentResult = UserAccountsHelper.searchDepartments(
            response,
            this.searchKey
          );
          this.searchDepartmentResult.map((element: any) => {
            this.departmentPathMap[
              element.identity.id
            ] = this.getPathOfDepartment(response, element);
            const routeDepartment = {};
            this.getRouteOfDepartment(response, element, routeDepartment);
            this.departmentRouteMap[element.identity.id] = routeDepartment;
          });

          this.isSearching = true;
          this.updateSearchResultText();
          this.isNoDepartment =
            !this.searchDepartmentResult || !this.searchDepartmentResult.length;
          this.cxGlobalLoaderService.hideLoader();
        },
        () => {
          this.toastr.error('There is something wrong, Please try again.');
          this.cxGlobalLoaderService.hideLoader();
        }
      );
  }

  onClickOnDepartmentItem(selectedDepartment: Department): void {
    if (!selectedDepartment) {
      return;
    }
    const departmentIndex = this.selectedDepartments.findIndex(
      (department) => department.identity.id === selectedDepartment.identity.id
    );
    if (departmentIndex === findIndexCommon.notFound) {
      this.selectedDepartments.push(selectedDepartment);

      return;
    }
    this.onRemoveDepartmentItem(selectedDepartment.identity.id);
  }

  onRemoveDepartmentItem(departmentId: number): void {
    const departmentIndex = this.selectedDepartments.findIndex(
      (department) => department.identity.id === departmentId
    );
    if (departmentIndex === findIndexCommon.notFound) {
      return;
    }

    this.selectedDepartments.splice(
      departmentIndex,
      this.NUMBER_OF_REMOVED_ITEM
    );
  }

  loadMoreDepartments(departments: Department[]): void {
    const parentItem = this.flatDepartmentsArray.find(
      (x) => x.identity.id === this.selectedDepartmentId
    );
    const newParentItem = departments.find(
      (x) => x.identity.id === this.selectedDepartmentId
    );
    const parentIdex = this.flatDepartmentsArray.indexOf(parentItem);
    if (parentIdex > findIndexCommon.notFound) {
      this.flatDepartmentsArray[parentIdex] = newParentItem;
    }
    departments = departments.filter(
      (x) => x.identity.id !== this.selectedDepartmentId
    );
    this.flatDepartmentsArray = departments
      ? this.flatDepartmentsArray.concat(departments)
      : this.flatDepartmentsArray;
    this.isLoadingDepartment = false;
  }

  loadChildren(selectedItem: Department): void {
    if (selectedItem && selectedItem.identity) {
      this.isLoadingDepartment = true;
      this.selectedDepartmentId = selectedItem.identity.id;
      this.getDepartments(this.selectedDepartmentId);
    }
  }

  onClickItemResult(department: Department): void {
    this.isSearching = false;
    this.onClickOnDepartmentItem(department);
    this.searchDepartmentResult = [];
    this.departmentQueryObject.departmentTypeIds = [];
    this.currentDepartmentId = department.identity.id;
    this.updateSearchResultText();
    this.changeDetectorRef.detectChanges();
    this.getDepartments(this.currentDepartmentId);
    this.isDetectExpandTree = true;
  }

  onAllDepartmentsDeselected(): void {
    this.selectedDepartments = [];
  }

  onSubmit(): void {
    this.dialogRef.close(new DepartmentDialogResult(this.selectedDepartments));
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onSearchTextCleared(isExpand: boolean = false): void {
    this.searchDepartmentResult = [];
    this.departmentQueryObject.departmentTypeIds = [];
    this.isSearching = false;
    this.searchKey = '';
    this.updateSearchResultText();
    this.isDetectExpandTree = isExpand;
    this.changeDetectorRef.detectChanges();
  }

  private updateSearchResultText(): void {
    if (this.isSearching) {
      this.searchResultText = this.translateService.instant(
        `Department_Page.Department_Search.ResultMatch`,
        { resultLength: this.searchDepartmentResult.length }
      ) as string;
    } else {
      this.searchResultText = '';
    }
  }

  private initFilterOptions(departmentTypes: DepartmentType[]): void {
    this.departmentTypes = departmentTypes;
    this.departmentTypeTitle = this.translateAdapterService.getValueImmediately(
      `Department_Page.Filter_Section.Filter_Options.Education_Level`
    );
    this.departmentFilterOptions = [
      new DepartmentFilterGroupModel({
        name: this.departmentTypeTitle,
        options: this.departmentTypes.map((departmentType) => {
          return new DepartmentFilterOption({
            value: this.getLocalizedText(departmentType.localizedData),
            objectId: departmentType.identity.id
          });
        }),
        groupConstant: GroupFilterConst.DEPARTMENT_TYPE
      })
    ];
  }

  private initFilterObject(): void {
    this.departmentQueryObject = new DepartmentQueryModel({
      includeParent: false,
      includeChildren: true,
      maxChildrenLevel: 1,
      countChildren: true,
      countUser: true,
      includeDepartmentType: true
    });
  }

  private getLocalizedText(localizedData: LocalizedDataItem[]): string {
    const index = localizedData.findIndex(
      (item) => item.languageCode === this.currentLanguageCode
    );
    if (index > findIndexCommon.notFound) {
      return localizedData[index].fields[0].localizedText;
    }
  }

  private initButtonCondition(): void {
    this.buttonCondition = new CxTreeButtonCondition({
      enableAdd: (_) => false,
      enableEdit: (_) => false,
      enableMove: (_) => false,
      enableRemove: (_) => false
    });
  }

  private getDepartments(fromDepartmentId: number): void {
    this.cxGlobalLoaderService.showLoader();
    const departmentQueryCloned = _.clone(this.departmentQueryObject);
    this.departmentHierarchicalService
      .getHierarchy(fromDepartmentId, departmentQueryCloned)
      .subscribe(
        (departments: any) => {
          if (departments) {
            departments.map((department) => {
              return new Department(department);
            });
          }
          if (this.isLoadingDepartment) {
            this.loadMoreDepartments(departments);
          } else {
            this.flatDepartmentsArray = _.uniqBy(
              this.flatDepartmentsArray.concat(departments),
              'identity.id'
            );
          }
          this.flatDepartmentsArray.forEach((item) => {
            this.updateLevelOfEducationMap(item);
          });
          this.cxGlobalLoaderService.hideLoader();
          this.changeDetectorRef.detectChanges();
        },
        (error) => {
          this.cxGlobalLoaderService.hideLoader();
        },
        () => {
          this.isDetectExpandTree = false;
        }
      );
  }

  private updateLevelOfEducationMap(department: Department): void {
    this.levelOfEducationMap[department.identity.id] =
      department.levels && department.levels.length > 0
        ? this.getPropLocalizedData(department.levels, this.currentLanguageCode)
        : '';
  }

  private getPropLocalizedData(
    levels: any[],
    languageCode: string,
    field: string = 'Name'
  ): string[] {
    return levels.map((data) => {
      return SurveyUtils.getPropLocalizedData(
        data.localizedData,
        field,
        languageCode
      );
    });
  }

  private getEducationLevels(): void {
    const educationLevelArchetype = 'Level';
    this.subscription.add(
      this.departmentHierarchicalService
        .getDepartmentTypes(educationLevelArchetype)
        .subscribe((departmentTypes: DepartmentType[]) => {
          if (departmentTypes) {
            this.initFilterOptions(departmentTypes);
          }
        })
    );
  }

  private getDepartmentTypeList(): void {
    this.departmentHierarchicalService
      .getDepartmentTypes('OrganizationalUnitType')
      .subscribe((departmentTypes: DepartmentType[]) => {
        if (departmentTypes) {
          this.departmentTypeList = departmentTypes;
        }
      });
  }
  private getPathOfDepartment(departmentArray: any[], department: any): any {
    const parentDepartment = departmentArray.find(
      (element: any) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      return department.departmentName;
    } else {
      return this.getPathOfDepartment(departmentArray, parentDepartment).concat(
        `/${department.departmentName}`
      );
    }
  }
  private getRouteOfDepartment(
    departmentArray: any[],
    department: any,
    newObject: any
  ): void {
    const parentDepartment = departmentArray.find(
      (element: any) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      newObject[department.identity.id] = department.identity.id;
    } else {
      newObject[parentDepartment.identity.id] = parentDepartment.identity.id;
      this.getRouteOfDepartment(departmentArray, parentDepartment, newObject);
    }
  }
}
