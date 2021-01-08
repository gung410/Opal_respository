import { Component } from '@angular/core';
import { DigitalContentLongCardComponent } from './digital-content-long-card.component';
import { LearningActionService } from '../services/learning-action.service';
import { ModuleFacadeService } from '@opal20/infrastructure';

@Component({
  selector: 'digital-content-card',
  templateUrl: './digital-content-card.component.html'
})
export class DigitalContentCardComponent extends DigitalContentLongCardComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected learningActionService: LearningActionService) {
    super(moduleFacadeService, learningActionService);
  }
}
