import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import {
  CxGlobalLoaderService,
  CxTreeButtonCondition,
  DepartmentHierarchiesModel,
} from '@conexus/cx-angular-common';
import { CxObjectRoute } from '@conexus/cx-angular-common/lib/components/cx-tree/models/cx-object-route.model';
import { TranslateService } from '@ngx-translate/core';
import { environment } from 'app-environments/environment';
import { User } from 'app-models/auth.model';
import { Department } from 'app-models/department-model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { OrganizationalUnitHelpers } from 'app-utilities/organizational-unit.helpers';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { StaffListService } from 'app/staff/staff.container/staff-list.service';
import { initUniversalToolbar } from 'app/staff/staff.container/staff-list/models/staff-action-mapping';
import { uniqBy } from 'lodash';
import { forkJoin } from 'rxjs';
import { DepartmentQueryModel } from '../models/filter-param.model';

@Component({
  selector: 'odp-department-browser',
  templateUrl: './odp-department-browser.component.html',
  styleUrls: ['./odp-department-browser.component.scss'],
})
export class OdpDepartmentBrowserComponent
  extends BaseSmartComponent
  implements OnInit {
  @Input()
  currentUser: User;

  @Output()
  changedDepartment: EventEmitter<Department> = new EventEmitter<Department>();

  buttonCondition: object = new CxTreeButtonCondition(); // Review if we could remove this.
  breadCrumbNavigation: any[] = [];
  departmentModel: DepartmentHierarchiesModel = new DepartmentHierarchiesModel();
  departmentRouteMap: object = {};
  reloadDataAfterClearSearch: boolean = false;
  selectedDepartmentIds: {} = {};

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private translateService: TranslateService,
    private translateAdapterService: TranslateAdapterService,
    private staffListService: StaffListService,
    private departmentStoreService: DepartmentStoreService,
    private cxGlobalLoaderService: CxGlobalLoaderService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    const currentDepartmentId = this.currentUser.departmentId;
    this.initDepartmentBrowser(currentDepartmentId);
    this.loadDataByDepartment(currentDepartmentId);
  }

  onSearch(searchkey: any): void {
    this.cxGlobalLoaderService.showLoader();
    if (searchkey !== '') {
      this.departmentModel.isShowSearchResult = true;
      this.subscription.add(
        this.staffListService
          .getHierarchicalDepartmentsByQuery(
            this.currentUser.departmentId,
            this.initFilterDepartment(searchkey)
          )
          .subscribe((response: any) => {
            this.departmentModel.departments = uniqBy(
              this.departmentModel.departments.concat(response),
              'identity.id'
            );
            this.departmentModel.searchDepartmentResult = response.filter(
              (department) =>
                department.departmentName
                  .trim()
                  .toLowerCase()
                  .indexOf(searchkey.trim().toLowerCase()) >= 0
            );

            this.departmentModel.departmentPathMap = [];
            this.departmentModel.searchDepartmentResult.forEach((element) => {
              this.departmentModel.departmentPathMap[
                element.identity.id
              ] = this.getPathOfDepartment(response, element);
              const routeDepartment = {};
              this.getRouteOfDepartment(response, element, routeDepartment);
              this.departmentRouteMap[element.identity.id] = routeDepartment;
            });
            this.departmentModel.isNoSearchResult =
              this.departmentModel.searchDepartmentResult.length === 0;
            this.cxGlobalLoaderService.hideLoader();
            this.changeDetectorRef.detectChanges();
          })
      );
    } else {
      this.departmentModel.isNoSearchResult = false;
      this.departmentModel.searchDepartmentResult = [];
      this.departmentModel.isShowSearchResult = false;
      this.reloadDataAfterClearSearch = true;
      this.cxGlobalLoaderService.hideLoader();
      this.changeDetectorRef.detectChanges();
    }
  }

  onClickSearchResult(department: any): void {
    const objectRoute = {
      route: this.departmentRouteMap[department.identity.id],
      object: department,
    };
    this.departmentModel.isShowSearchResult = false;
    this.departmentModel.isDetectExpandTree = true;
    this.departmentModel.searchDepartmentResult = [];
    this.changeDetectorRef.detectChanges();
    this.onSelectDepartmentNode(objectRoute);
  }

  onSelectedDepartmentLabel(selectedDepartment: any): void {
    const newRoute = {};
    this.departmentModel.currentDepartmentId = selectedDepartment.identity.id;
    this.resetDepartmentDropdownData(selectedDepartment, newRoute);
    this.reloadDepartmentAndRelevantData(
      this.departmentModel.currentDepartmentId
    );
  }

  onSelectDepartmentNode(objectRoute: CxObjectRoute<any>): void {
    this.departmentModel.currentDepartmentId = objectRoute.object.identity.id;
    this.resetDepartmentDropdownData(objectRoute.object, objectRoute.route);
    this.reloadDepartmentAndRelevantData(
      this.departmentModel.currentDepartmentId
    );
  }

  expandChildDepartment(departmentSelected: Department): void {
    if (departmentSelected && departmentSelected.identity) {
      this.getHierarchicalDepartments(
        departmentSelected.identity.id,
        this.initFilterDepartment(),
        false
      );
    }
  }

  private resetDepartmentDropdownData(newDepartment: any, route: any): void {
    this.selectedDepartmentIds = {};
    this.selectedDepartmentIds[newDepartment.identity.id] =
      newDepartment.identity.id;
  }

  private reloadDepartmentAndRelevantData(currentDepartmentId: number): void {
    this.loadDataByDepartment(currentDepartmentId);
    this.getHierarchicalDepartments(
      currentDepartmentId,
      this.initFilterDepartment()
    );
  }

  private getPathOfDepartment(
    departmentArray: Array<any>,
    department: any
  ): any {
    const parentDepartment = departmentArray.find(
      (element) => element.identity.id === department.parentDepartmentId
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
    departmentArray: Array<any>,
    department: any,
    newObject: any
  ): void {
    const parentDepartment = departmentArray.find(
      (element) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      newObject[department.identity.id] = department.identity.id;
    } else {
      newObject[parentDepartment.identity.id] = parentDepartment.identity.id;
      this.getRouteOfDepartment(departmentArray, parentDepartment, newObject);
    }
  }

  private loadDataByDepartment(currentDepartmentId: number): void {
    forkJoin([
      this.departmentStoreService.getDepartmentTypesByDepartmentId(
        currentDepartmentId
      ),
      this.departmentStoreService.getDepartmentById(currentDepartmentId),
    ]).subscribe((results) => {
      const currentDepartmentOrganizationalUnitTypes = results[0];
      const currentDepartment = results[1];
      const organizationalUnitTypesHavingOPJ =
        environment.pdplanConfig.organizationalUnitTypesHavingOPJ;
      if (
        OrganizationalUnitHelpers.hasAnyOrganizationalUnitType(
          currentDepartmentOrganizationalUnitTypes,
          organizationalUnitTypesHavingOPJ
        )
      ) {
        this.departmentModel.currentDepartmentId = currentDepartmentId;
        this.changedDepartment.emit(currentDepartment);
      } else {
        this.loadDataFromAncestor(
          currentDepartment,
          organizationalUnitTypesHavingOPJ
        );
      }
    });
  }

  private loadDataFromAncestor(
    currentDepartment: Department,
    organizationalUnitTypesHavingOPJ: string[]
  ): void {
    this.departmentStoreService
      .getAncestorDepartmentsOfDepartment(currentDepartment.identity.id, true)
      .subscribe((ancestorDepartments) => {
        const sortedAncestorDepartments = ancestorDepartments.sort(
          (a, b) => b.path.length - a.path.length
        );
        for (const ancestorDepartment of sortedAncestorDepartments) {
          if (
            OrganizationalUnitHelpers.hasAnyOrganizationalUnitType(
              ancestorDepartment.organizationalUnitTypes,
              organizationalUnitTypesHavingOPJ
            )
          ) {
            this.changedDepartment.emit(ancestorDepartment);

            break;
          }
        }
      });
  }

  private initDepartmentBrowser(currentDepartmentId: number): void {
    this.getHierarchicalDepartments(
      currentDepartmentId,
      this.initFilterDepartment()
    );
    this.departmentModel.text.header = this.translateService.instant(
      'Common.Label.OrganisationUnit'
    );
    this.departmentModel = initUniversalToolbar(this.translateAdapterService);
  }

  private initFilterDepartment(searchText?: string): DepartmentQueryModel {
    const paramQuery = new DepartmentQueryModel({
      includeParent: false,
      includeChildren: true,
      maxChildrenLevel: 1,
      countChildren: true,
    });
    if (searchText !== undefined) {
      paramQuery.searchText = searchText;
    }

    return paramQuery;
  }

  private getHierarchicalDepartments(
    currentDepartmentId: number,
    departmentQuery: DepartmentQueryModel,
    isBuildTableAndCurmb: boolean = true
  ): void {
    this.subscription.add(
      this.staffListService
        .getHierarchicalDepartmentsByQuery(currentDepartmentId, departmentQuery)
        .subscribe((response: any) => {
          if (!response) {
            return;
          }

          if (this.reloadDataAfterClearSearch) {
            this.departmentModel.departments = [];
            this.changeDetectorRef.detectChanges();
          }
          this.departmentModel.departments = uniqBy(
            this.departmentModel.departments.concat(response),
            'identity.id'
          );
          if (isBuildTableAndCurmb) {
            this.breadCrumbNavigation = [];
            this.breadCrumbNavigation = Object.assign(
              [],
              this.buildCurrentDepartmentCrumb(
                this.breadCrumbNavigation,
                this.departmentModel.departments,
                this.currentUser.departmentId,
                currentDepartmentId
              )
            );
          }
          this.departmentModel.isDetectExpandTree = false;
          this.reloadDataAfterClearSearch = false;
          this.changeDetectorRef.detectChanges();
        })
    );
  }

  private buildCurrentDepartmentCrumb(
    currentDepartmentCrumb: any[],
    hierarchicalUserDepartments: any[],
    userDepartmentId: number,
    currentDepartmentId: number
  ): any[] {
    if (!currentDepartmentCrumb) {
      currentDepartmentCrumb = [];
    }
    for (const department of hierarchicalUserDepartments) {
      if (department.identity.id !== currentDepartmentId) {
        continue;
      }
      currentDepartmentCrumb.push({
        name: department.departmentName,
        identity: department.identity,
      });
      if (department.identity.id === userDepartmentId) {
        continue;
      }
      currentDepartmentId = department.parentDepartmentId;

      return this.buildCurrentDepartmentCrumb(
        currentDepartmentCrumb,
        hierarchicalUserDepartments,
        userDepartmentId,
        currentDepartmentId
      );
    }

    return currentDepartmentCrumb.reverse();
  }
}
