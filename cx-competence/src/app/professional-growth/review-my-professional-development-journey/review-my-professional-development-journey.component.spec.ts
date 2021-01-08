/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement, NO_ERRORS_SCHEMA } from '@angular/core';

import { ReviewMyProfessionalDevelopmentJourneyComponent } from './review-my-professional-development-journey.component';

describe('ReviewMyProfessionalDevelopmentJourneyComponent', () => {
  let component: ReviewMyProfessionalDevelopmentJourneyComponent;
  let fixture: ComponentFixture<ReviewMyProfessionalDevelopmentJourneyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ReviewMyProfessionalDevelopmentJourneyComponent],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(
      ReviewMyProfessionalDevelopmentJourneyComponent
    );
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
