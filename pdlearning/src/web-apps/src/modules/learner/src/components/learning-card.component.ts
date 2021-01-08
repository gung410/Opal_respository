import { LocalTranslatorService, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { CourseDataService } from '../services/course-data.service';
import { LearningActionService } from '../services/learning-action.service';
import { LearningLongCardComponent } from './learning-long-card.component';

@Component({
  selector: 'learning-card',
  templateUrl: './learning-card.component.html'
})
export class LearningCardComponent extends LearningLongCardComponent {
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public translator: LocalTranslatorService,
    protected learningActionService: LearningActionService,
    protected courseDataService: CourseDataService
  ) {
    super(moduleFacadeService, translator, learningActionService, courseDataService);
  }
}
