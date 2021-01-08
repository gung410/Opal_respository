import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchDepartmentHierarchicalComponent } from './search-department-hierarchical.component';

describe('SearchDepartmentHierarchicalComponent', () => {
  let component: SearchDepartmentHierarchicalComponent;
  let fixture: ComponentFixture<SearchDepartmentHierarchicalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SearchDepartmentHierarchicalComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchDepartmentHierarchicalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
