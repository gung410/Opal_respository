import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxSlidebarComponent } from './cx-slidebar.component';

describe('CxSlidebarComponent', () => {
  let component: CxSlidebarComponent;
  let fixture: ComponentFixture<CxSlidebarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxSlidebarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxSlidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
