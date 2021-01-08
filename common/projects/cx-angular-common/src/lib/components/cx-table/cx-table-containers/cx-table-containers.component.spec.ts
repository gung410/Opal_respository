import { CxTableContainersComponent } from './cx-table-containers.component';
import { TestBed, ComponentFixture, async, fakeAsync } from '@angular/core/testing';
import { EventEmitter, NO_ERRORS_SCHEMA } from '@angular/core';
import { employeesData } from 'src/app/components/cx-table-containers-doc/mock-data/item.data';
import { CxColumnSortType, CxTableIcon } from '../cx-table.model';
import { CxButtonComponent } from '../../cx-button/cx-button.component';
import { CxCustomizedCheckboxComponent } from '../../cx-customized-checkbox/cx-customized-checkbox.component';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { CxButtonDirective } from '../../../directives/button.directive';
import { FormsModule } from '@angular/forms';
import * as $ from 'jquery';
import { departmentsData } from 'src/app/components/cx-table-containers-doc/mock-data/container.data';
(window as any).$ = $;

describe('CxTableContainersComponent', () => {
    let component: CxTableContainersComponent<any, any>;
    let fixture: ComponentFixture<CxTableContainersComponent<any, any>>;
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
                CxTableContainersComponent,
                CxButtonComponent,
                CxCustomizedCheckboxComponent,
                CxButtonDirective
            ],
            schemas: [NO_ERRORS_SCHEMA]
        }).compileComponents()
            .then(() => {
                fixture = TestBed.createComponent(CxTableContainersComponent);
                component = fixture.componentInstance;
                element = fixture.debugElement.nativeElement;
            });
    }));

    beforeEach(() => {
        component.items = initItems;
        component.currentSortType = CxColumnSortType.UNSORTED;
        component.currentFieldSort = 'Name';
        component.headers = headers;
        component.itemIdRoutes = ['Identity.Id'];
        component.icon = icon;

        fixture.detectChanges();
    });

    it('should create an cx-table-containers component', () => {
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

    it('should change data when user click sort in header', fakeAsync(() => {
        spyOn(component, 'sortByFieldClick');
        component.sortByFieldClick('Name', component.currentSortType as CxColumnSortType);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            expect(component.sortByFieldClick).toHaveBeenCalled();
            expect(component.sortByFieldClick).toHaveBeenCalledWith('Name', component.currentSortType);
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

    it('should tracks that call move item (onMoveItemClicked)', () => {
        const item = employeesData.items[0];
        spyOn(component.moveItem, 'emit');
        component.onMoveItemClicked(item);
        expect(component.moveItem.emit).toHaveBeenCalled();
        expect(component.moveItem.emit).toHaveBeenCalledWith(item);
    });

    it('should tracks that call when click info item (containerInfoClick)', () => {
        const item = employeesData.items[1];
        spyOn(component.containerClick, 'emit');
        component.containerInfoClick(item);
        expect(component.containerClick.emit).toHaveBeenCalled();
        expect(component.containerClick.emit).toHaveBeenCalledWith(item);
    });

    it('should make sure get icon function run correctly (getContainerIcon)', () => {
        const container = departmentsData[0];
        spyOn(component, 'getContainerIcon');
        component.getContainerIcon(container);
        expect(component.getContainerIcon).toHaveBeenCalled();
        expect(component.getContainerIcon).toHaveBeenCalledWith(container);
    });
});
