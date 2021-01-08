import { Injectable } from '@angular/core';
import { StandaloneSurveyDetailMode } from '../models/standalone-survey-detail-mode.model';
import { Subject } from 'rxjs';

@Injectable()
export class StandaloneSurveyEditModeService {
  public modeChanged: Subject<StandaloneSurveyDetailMode> = new Subject();
  public initMode: StandaloneSurveyDetailMode = StandaloneSurveyDetailMode.View;
}
