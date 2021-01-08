import { AssignmentDetailViewModel, AssignmentMode } from '@opal20/domain-components';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input } from '@angular/core';

import { FormGroup } from '@angular/forms';
import { OpalDialogService } from '@opal20/common-components';
import { PreviewAssessmentDialogComponent } from './dialogs/preview-assessment-dialog.component';
import { SCORE_MODE_TYPE_MAP } from '../models/score-mode-type-map.model';
import { ScoreMode } from '@opal20/domain-api';

@Component({
  selector: 'assignment-editor-config',
  templateUrl: './assignment-editor-config.component.html'
})
export class AssignmentEditorConfigComponent extends BaseComponent {
  @Input() public assignmentVm: AssignmentDetailViewModel;
  @Input() public form: FormGroup;
  @Input() public mode: AssignmentMode = AssignmentMode.Edit;

  @Input() public assignmentVmChange: EventEmitter<AssignmentDetailViewModel> = new EventEmitter();

  public showPreviewAssessment: boolean = false;
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;
  public ScoreMode: typeof ScoreMode = ScoreMode;
  public scoreModeTypeMap = SCORE_MODE_TYPE_MAP;

  public get messageScoreMode(): string {
    return this.assignmentVm.scoreMode === ScoreMode.Instructor
      ? this.translate('Please contact instructors to assess learners in class.')
      : this.translate('Please check peer assessment setup and inform your class.');
  }

  constructor(protected moduleFacadeService: ModuleFacadeService, private opalDialogService: OpalDialogService) {
    super(moduleFacadeService);
  }

  public displayPreviewAssessment(): boolean {
    return (
      this.assignmentVm.assessmentId &&
      this.assignmentVm.assessmentDic[this.assignmentVm.assessmentId] != null &&
      this.mode === AssignmentMode.View
    );
  }

  public displayScoreMode(): boolean {
    return this.assignmentVm.scoreMode != null;
  }

  public emitAssignmentVmChanged(): void {
    this.assignmentVmChange.emit(this.assignmentVm);
  }

  public onAssessmentSelectItemPreviewBtnClicked(e: Event, assessmentId: string): void {
    e.stopImmediatePropagation();
    this.showPreviewAssessment = true;

    // TODO
    const dialogRef = this.opalDialogService.openDialogRef(
      PreviewAssessmentDialogComponent,
      {
        assessmentId: assessmentId
      },
      {
        maxWidth: '100vw',
        maxHeight: '100vh',
        width: '100vw',
        height: '100vh',
        borderRadius: '0'
      }
    );

    this.subscribe(dialogRef.result, () => {
      this.showPreviewAssessment = false;
    });
  }

  public isLocalDevelopmentMode(): boolean {
    return AppGlobal.environment.envName === 'development' || AppGlobal.environment.envName === 'local';
  }
}
