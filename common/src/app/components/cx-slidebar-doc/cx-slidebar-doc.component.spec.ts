import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxSlidebarDocComponent } from './cx-slidebar-doc.component';

describe('CxSlidebarDocComponent', () => {
  let component: CxSlidebarDocComponent;
  let fixture: ComponentFixture<CxSlidebarDocComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxSlidebarDocComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxSlidebarDocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
