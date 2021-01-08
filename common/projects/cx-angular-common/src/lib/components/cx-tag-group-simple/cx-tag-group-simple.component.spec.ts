import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxTagGroupSimpleComponent } from './cx-tag-group-simple.component';

describe('CxTagGroupSimpleComponent', () => {
  let component: CxTagGroupSimpleComponent;
  let fixture: ComponentFixture<CxTagGroupSimpleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxTagGroupSimpleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxTagGroupSimpleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
