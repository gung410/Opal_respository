import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { FormParticipantStatus } from '@opal20/domain-api';
import { LearningActionService } from '../services/learning-action.service';
import { StandaloneFormItemModel } from '../models/standalone-form-item.model';

@Component({
  selector: 'standalone-form-card',
  templateUrl: './standalone-form-card.component.html'
})
export class StandaloneFormCardComponent extends BaseComponent {
  @Input()
  public learningCardItem: StandaloneFormItemModel;
  @Input()
  public displayOnlyMode: boolean = false;

  @Output()
  public learningCardClick: EventEmitter<StandaloneFormItemModel> = new EventEmitter<StandaloneFormItemModel>();

  public myFormParticipantStatus: typeof FormParticipantStatus = FormParticipantStatus;

  constructor(protected moduleFacadeService: ModuleFacadeService, private learningActionService: LearningActionService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    if (!this.learningCardItem) {
      return;
    }
  }

  public onStandaloneFormCardClick(): void {
    this.learningCardClick.emit(this.learningCardItem);
  }

  public get learningStatusText(): string {
    if (!this.learningCardItem.formParticipantStatus) {
      return;
    }
    return LEARNER_FORM_PARTICIPANT_STATUS_MAP[this.learningCardItem.formParticipantStatus].text;
  }
}

const LEARNER_FORM_PARTICIPANT_STATUS_MAP = {
  [FormParticipantStatus.Completed]: {
    text: FormParticipantStatus.Completed
  },
  [FormParticipantStatus.Incomplete]: {
    text: 'In Progress'
  }
};
