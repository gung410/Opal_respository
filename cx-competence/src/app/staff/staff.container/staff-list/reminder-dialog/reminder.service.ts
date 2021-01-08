import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { APIConstant } from 'app/shared/app.constant';
import { Observable } from 'rxjs';
import { LearningNeedAnalysisRemindingRequest } from '../models/reminder-request.model';
import { LearningNeedAnalysisRemindingList } from './../models/reminder-request.model';

@Injectable()
export class LearningNeedAnalysisReminderService {
  constructor(private http: HttpHelpers) {}

  remindToCompleteLearningNeedAnalysis(
    reminderRequests: LearningNeedAnalysisRemindingList
  ): Observable<any> {
    return this.http.post<LearningNeedAnalysisRemindingRequest>(
      APIConstant.IDP_NEED_REMINDER,
      reminderRequests
    );
  }
}
