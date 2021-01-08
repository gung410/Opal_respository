import { CxTableComponent } from './cx-table.component';
import { TestBed, ComponentFixture, async, fakeAsync } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { employeesData } from 'src/app/components/cx-table-containers-doc/mock-data/item.data';
import { CxColumnSortType, CxTableIcon } from '../cx-table.model';
import { CxButtonComponent } from '../../cx-button/cx-button.component';
import { CxCustomizedCheckboxComponent } from '../../cx-customized-checkbox/cx-customized-checkbox.component';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { CxButtonDirective } from '../../../directives/button.directive';
import { FormsModule } from '@angular/forms';
import * as $ from 'jquery';
(window as any).$ = $;

describe('CxTableComponent', () => {
    let component: CxTableComponent<any>;
    let fixture: ComponentFixture<CxTableComponent<any>>;
    let element: HTMLElement;
    const headers = [
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
    const initItems = employeesData.items;
    const icon: CxTableIcon = new CxTableIcon();

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [NgbDropdownModule, FormsModule],
            declarations: [
                CxTableComponent,
                CxButtonComponent,
                CxCustomizedCheckboxComponent,
                CxButtonDirective
            ],
            schemas: [NO_ERRORS_SCHEMA]
        }).compileComponents()
            .then(() => {
                fixture = TestBed.createComponent(CxTableComponent);
                component = fixture.componentInstance;
                element = fixture.debugElement.nativeElement;
            });
    }));

    beforeEach(() => {
        component.items = initItems;
        component.currentSortType = CxColumnSortType.UNSORTED;
        component.isDataLazyLoad = true;
        component.currentFieldSort = 'Name';
        component.headers = headers;
        component.itemIdRoutes = ['Identity.Id'];
        component.icon = icon;

        fixture.detectChanges();
    });

    it('should create an cx-table component', () => {
        expect(component).toBeTruthy();
    });

    it('should init component with item equal object', () => {
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            expect(component.items).toEqual(employeesData.items);
            expect(component.currentSortType).toEqual(CxColumnSortType.UNSORTED);
            expect(component.currentFieldSort).toEqual('Name');
            expect(component.headers).toEqual(headers);
        });
    });

    it('should emit an object when user click sort in header', fakeAsync(() => {
        spyOn(component, 'sortByFieldClick');
        const spy = spyOn(component.sortTypeChange, 'emit');
        component.sortByFieldClick('Name', component.currentSortType as CxColumnSortType);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            expect(component.sortByFieldClick).toHaveBeenCalled();
            expect(component.sortByFieldClick).toHaveBeenCalledWith('Name', component.currentSortType);
            expect(spy).toHaveBeenCalled();
            expect(spy).toHaveBeenCalledWith({
                fieldSort: 'Name', sortType: CxColumnSortType.ASCENDING
            });
            headers.forEach(item => {
                if (item.fieldSort !== 'Name' && item.fieldSort !== '') {
                  item.sortType = CxColumnSortType.UNSORTED;
                } else {
                  item.sortType = CxColumnSortType.ASCENDING;
                }
              });
            expect(component.headers).toEqual(headers);
        });
    }));
});
