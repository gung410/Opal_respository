import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxTagGroupComponent } from './cx-tag-group.component';

describe('CxTagGroupComponent', () => {
  let component: CxTagGroupComponent<any>;
  let fixture: ComponentFixture<CxTagGroupComponent<any>>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxTagGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxTagGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
