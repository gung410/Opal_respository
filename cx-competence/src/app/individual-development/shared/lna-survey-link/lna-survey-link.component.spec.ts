import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LnaSurveyLinkComponent } from './lna-survey-link.component';

describe('LnaSurveyLinkComponent', () => {
  let component: LnaSurveyLinkComponent;
  let fixture: ComponentFixture<LnaSurveyLinkComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LnaSurveyLinkComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LnaSurveyLinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
