import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxBackBtnComponent } from './cx-back-btn.component';

describe('CxBackBtnComponent', () => {
  let component: CxBackBtnComponent;
  let fixture: ComponentFixture<CxBackBtnComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxBackBtnComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxBackBtnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
