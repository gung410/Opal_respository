import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfessionalDevelopmentPlanComponent } from './professional-development-plan.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ProfessionalDevelopmentPlanComponent', () => {
  let component: ProfessionalDevelopmentPlanComponent;
  let fixture: ComponentFixture<ProfessionalDevelopmentPlanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ProfessionalDevelopmentPlanComponent],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfessionalDevelopmentPlanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
