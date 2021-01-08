import { AssignmentMode, ParticipantAssignmentTrackDetailViewModel } from '@opal20/domain-components';
import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { QuizAssignmentFormQuestion } from '@opal20/domain-api';

@Component({
  selector: 'participant-assignment-track-editor',
  templateUrl: './participant-assignment-track-editor.component.html'
})
export class ParticipantAssignmentTrackEditor extends BaseFormComponent {
  @Input() public participantAssignmentTrackVm: ParticipantAssignmentTrackDetailViewModel = new ParticipantAssignmentTrackDetailViewModel();

  public AssignmentMode: typeof AssignmentMode = AssignmentMode;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public assignmentQuestionsDataTrackByFn(index: number, item: QuizAssignmentFormQuestion): string | QuizAssignmentFormQuestion {
    return item.id;
  }

  public onChangeScoreQuestion(): void {
    this.participantAssignmentTrackVm.calcTotalScoreAndScorePercentage();
  }
}
