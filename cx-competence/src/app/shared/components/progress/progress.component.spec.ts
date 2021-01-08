import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressComponent } from './progress.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ProgressComponent', () => {
  let component: ProgressComponent;
  let fixture: ComponentFixture<ProgressComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ProgressComponent],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});