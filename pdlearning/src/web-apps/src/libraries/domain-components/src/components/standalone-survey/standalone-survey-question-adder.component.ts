import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { StandaloneSurveyDetailMode } from '../../models/standalone-survey-detail-mode.model';
import { StandaloneSurveyEditModeService } from '../../services/standalone-survey-edit-mode.service';
import { StandaloneSurveyQuestionType } from '@opal20/domain-api';
import { StandaloneSurveyQuestionTypeSelectionService } from '../../services/standalone-survey-question-type-selection.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'standalone-survey-question-adder',
  templateUrl: './standalone-survey-question-adder.component.html'
})
export class StandaloneSurveyQuestionAdderComponent extends BaseComponent {
  @Input() public id: string = Utils.createUUID();
  @Input() public inlineAdd: boolean = false;
  @Input() public priority: number;
  @Input() public minorPriority: number;

  @Output('selectQuestion') public selectQuestionEvent: EventEmitter<{
    type: StandaloneSurveyQuestionType;
    priority: number;
    minorPriority: number;
  }> = new EventEmitter();
  public mode: StandaloneSurveyDetailMode = this.editModeService.initMode;
  public StandaloneSurveyDetailMode: typeof StandaloneSurveyDetailMode = StandaloneSurveyDetailMode;
  private questionSelectionSubscription: Subscription;

  constructor(
    moduleFacadeService: ModuleFacadeService,
    public editModeService: StandaloneSurveyEditModeService,
    private questionTypeSelectionService: StandaloneSurveyQuestionTypeSelectionService
  ) {
    super(moduleFacadeService);
  }

  protected onInit(): void {
    this.subscribe(this.editModeService.modeChanged, mode => {
      this.mode = mode;
    });
    this.questionSelectionSubscription = this.subscribeNewQuestionCreated();
  }

  protected onDestroy(): void {
    if (this.questionSelectionSubscription) {
      this.questionSelectionSubscription.unsubscribe();
    }
  }

  private subscribeNewQuestionCreated(): Subscription {
    return this.questionTypeSelectionService.getNewQuestionType$.subscribe(
      (data: { type: StandaloneSurveyQuestionType; priority: number; minorPriority: number }) => {
        if (data && data.priority === this.priority && data.minorPriority === this.minorPriority) {
          this.selectQuestionEvent.emit(data);
        }
      }
    );
  }
}
