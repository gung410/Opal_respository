import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxCommonComponent } from './cx-angular-common.component';

describe('CxCommonComponent', () => {
  let component: CxCommonComponent;
  let fixture: ComponentFixture<CxCommonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxCommonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxCommonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
