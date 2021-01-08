import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportLearningNeedsAnalysisDialogComponent } from './export-learning-needs-analysis-dialog.component';

describe('ExportLearningNeedsAnalysisDialogComponent', () => {
  let component: ExportLearningNeedsAnalysisDialogComponent;
  let fixture: ComponentFixture<ExportLearningNeedsAnalysisDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ExportLearningNeedsAnalysisDialogComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(
      ExportLearningNeedsAnalysisDialogComponent
    );
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
