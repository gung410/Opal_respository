import { CxItemCustomActionComponent } from './cx-item-custom-action.component';
import { TestBed, ComponentFixture, async } from '@angular/core/testing';
import { EventEmitter } from '@angular/core';

describe('CxItemCustomActionComponent', () => {
    let component: CxItemCustomActionComponent<any>;
    let fixture: ComponentFixture<CxItemCustomActionComponent<any>>;
    let element: HTMLElement;
    const itemTemp = {
        Test: 'Test value',
    };

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [CxItemCustomActionComponent]
        }).compileComponents()
            .then(() => {
                fixture = TestBed.createComponent(CxItemCustomActionComponent);
                component = fixture.componentInstance;
                element = fixture.debugElement.nativeElement;
            });
    }));

    beforeEach(() => {
        component.item = {
            Test: 'Test value',
        };
        component.icon = 'material-icons edit-icon';
        component.customActionClick = new EventEmitter<any>();
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should init component with item equal input data', () => {
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            expect(component.item).toEqual(itemTemp);
            expect(component.icon).toEqual('material-icons edit-icon');
        });
    });

    it('should emit an item when user click action button', () => {
        spyOn(component.customActionClick, 'emit');
        fixture.detectChanges();
        component.onItemClicked();
        expect(component.customActionClick.emit).toHaveBeenCalled();
        expect(component.customActionClick.emit).toHaveBeenCalledWith(itemTemp);
    });

});
