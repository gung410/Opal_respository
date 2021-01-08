import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxTagGroupDocComponent } from './cx-tag-group-doc.component';

describe('CxTagGroupDocComponent', () => {
  let component: CxTagGroupDocComponent;
  let fixture: ComponentFixture<CxTagGroupDocComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxTagGroupDocComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxTagGroupDocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
