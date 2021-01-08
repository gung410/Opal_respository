import { Component, OnInit, ViewEncapsulation, ViewChildren, QueryList, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CxColumnSortType } from 'projects/cx-angular-common/src/lib/components/cx-table/cx-table.model';
import { CxItemTableHeaderModel, CxTableComponent } from 'projects/cx-angular-common/src';
import { employeesForLazyLoading } from './mock-data/employees-data';
import { Md5 } from 'md5-typescript';
import { departmentsData } from '../cx-table-containers-doc/mock-data/container.data';
import { employeesData } from '../cx-table-containers-doc/mock-data/item.data';

@Component({
    selector: 'cx-table-doc',
    templateUrl: './cx-table-doc.component.html',
    styleUrls: ['./cx-table-doc.component.scss'],
    encapsulation: ViewEncapsulation.None,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxTableDocComponent implements OnInit {

    public headers: CxItemTableHeaderModel[] = [
        {
            text: "Name",
            fieldSort: "DepartmentName;FirstName",
            sortType: CxColumnSortType.UNSORTED
        },
        {
            text: "Roles",
            sortType: CxColumnSortType.UNSORTED
        },
        {
            text: "Status",
            fieldSort: "",
            sortType: CxColumnSortType.UNSORTED
        }
    ];
    public pageSizeRanges = [50, 100];
    public employeesRoleMap = {};
    public departments = departmentsData;
    public itemsData = [];
    public employeesData = employeesData;
    constructor(private changeDetectorRef: ChangeDetectorRef) { }
    public employeesForLazyLoading = employeesForLazyLoading;
    public employeesPerPage = 10;
    public currentEmployeesPage = 1;
    public currentEmployeesInPage: any[];
    public lazyLoadTableSortType: CxColumnSortType = CxColumnSortType.ASCENDING;
    private employeeSortField = 'FirstName';
    @ViewChildren(CxTableComponent) employeesTables: QueryList<CxTableComponent<any>>;
    public showActionsButtonInLastRow = false;
    public cxTableHeaders: CxItemTableHeaderModel[] = [
        {
            text: 'Name',
            fieldSort: 'FirstName',
            sortType: CxColumnSortType.ASCENDING
        },
        {
            text: 'Roles',
            sortType: CxColumnSortType.UNSORTED
        },
        {
            text: 'Status',
            fieldSort: '',
            sortType: CxColumnSortType.UNSORTED
        }
    ];
    currentCxTablePage: 1;

    public cxTableHeaders1: CxItemTableHeaderModel[] = [
        {
            text: 'Name 1',
            fieldSort: 'FirstName',
            sortType: CxColumnSortType.ASCENDING
        },
        {
            text: 'Roles 1',
            sortType: CxColumnSortType.UNSORTED
        },
        {
            text: 'Status 1',
            fieldSort: '',
            sortType: CxColumnSortType.UNSORTED
        }
    ];

    ngOnInit() {
        setTimeout(() => {
            this.currentEmployeesInPage = this.getEmployeesInPage(
                this.employeesForLazyLoading,
                this.currentEmployeesPage,
                this.employeesPerPage,
                this.lazyLoadTableSortType,
                this.employeeSortField
            );
            this.changeDetectorRef.detectChanges();
        }, 1000);
    }

    ngDoCheck() {
        console.log("Doc do check");
    }
    onCurrentPageChange(nextPage: number) {
      this.currentEmployeesInPage = this.getEmployeesInPage(
          this.employeesForLazyLoading,
          nextPage,
          this.employeesPerPage,
          this.lazyLoadTableSortType,
          this.employeeSortField
      );
      this.currentEmployeesPage = nextPage;
      this.changeDetectorRef.detectChanges();
    }

    onPageSizeChange(pageSize: number) {
      this.employeesPerPage = pageSize;
      this.currentEmployeesInPage = this.getEmployeesInPage(
          this.employeesForLazyLoading,
          this.currentEmployeesPage,
          this.employeesPerPage,
          this.lazyLoadTableSortType,
          this.employeeSortField
      );
      this.changeDetectorRef.detectChanges();
    }

    changeData() {
    }

    changeData1() {
    }

    onMoveEmployee(item: any) {
    }

    onRemoveEmployeesInTable(items: any[]) {

    }

    public getAvatarFromEmail(
        email: string,
        imageSize: number = 80,
        gavatarTypeD: string = 'mm'
    ): string {
        let imageURL = '';
        const gravataBaseUrl = 'http://www.gravatar.com';
        const hash = Md5.init(email.toLowerCase()).toLowerCase();
        imageURL = `${gravataBaseUrl}/avatar/${hash}.jpg?s=${imageSize}&d=${gavatarTypeD}`;
        return imageURL;
    }

    private getEmployeesInPage(
        allEmployees: any[],
        page: number,
        employeesPerPage: number,
        sortType: CxColumnSortType,
        fieldSort: string
    ): any[] {
        const startedIndex = employeesPerPage * (page - 1);
        const sortedEmployees = JSON.parse(JSON.stringify(allEmployees)) as any[];
        switch (sortType) {
            case CxColumnSortType.ASCENDING:
                sortedEmployees.sort((emp1, emp2) => {
                    return emp1[fieldSort] > emp2[fieldSort] ? 1 : -1;
                });
                break;
            case CxColumnSortType.DESCENDING:
                sortedEmployees.sort((emp1, emp2) => {
                    return emp1[fieldSort] > emp2[fieldSort] ? -1 : 1;
                });
                break;
            case CxColumnSortType.UNSORTED:
                break;
            default:
                break;
        }
        return sortedEmployees.slice(startedIndex, startedIndex + employeesPerPage);
    }

    onSortTypeChange($event: { fieldSort: string; sortType: CxColumnSortType }) {
        this.currentEmployeesInPage = this.getEmployeesInPage(
            this.employeesForLazyLoading,
            this.currentEmployeesPage,
            this.employeesPerPage,
            $event.sortType,
            $event.fieldSort
        );
        this.lazyLoadTableSortType = $event.sortType;
        this.employeeSortField = $event.fieldSort;
        this.changeDetectorRef.detectChanges();
    }

    onIncreaseEmployeesPerPage() {
        setTimeout(() => {
            this.employeesPerPage++;
            this.currentEmployeesInPage = this.getEmployeesInPage(
                this.employeesForLazyLoading,
                this.currentEmployeesPage,
                this.employeesPerPage,
                this.lazyLoadTableSortType,
                this.employeeSortField
            );
            this.changeDetectorRef.detectChanges();
        }, 500);
    }

    onDecreaseEmployeesPerPage() {
        setTimeout(() => {
            this.employeesPerPage--;
            this.currentEmployeesInPage = this.getEmployeesInPage(
                this.employeesForLazyLoading,
                this.currentEmployeesPage,
                this.employeesPerPage,
                this.lazyLoadTableSortType,
                this.employeeSortField
            );
            this.changeDetectorRef.detectChanges();
        }, 500);
    }

    onRemoveEmployeesInETable(employees: any[]) {
        setTimeout(() => {
            this.employeesForLazyLoading = this.employeesForLazyLoading.filter(e => {
                return !employees.find(emp => e.Identity.Id === emp.Identity.Id);
            });
            this.employeesTables.last.executeRemoveItems(employees);
            this.changeDetectorRef.detectChanges();
        }, 1000);
    }

    public onAcceptClicked() {
        return Object.values(this.employeesTables.last.getSelectedItemsMap());
    }

    public onRejectClicked() {
        return Object.values(this.employeesTables.last.getSelectedItemsMap());
    }

    onSelectItem(data: {
        selected: boolean;
        value: any;
    }) {
    }
}
