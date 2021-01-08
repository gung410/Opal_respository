import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';
import { ClassRun, IClassRun } from '../models/classrun.model';

import { Constant } from '@opal20/authentication';
import { IChangeClassRunStatusRequest } from './../dtos/change-classrun-status-request';
import { IClassRunCancellationStatusRequest } from '../dtos/change-classrun-cancellation-status-request';
import { IClassRunRescheduleStatusRequest } from '../dtos/change-classrun-reschedule-status-request';
import { ISaveClassRunRequest } from './../dtos/save-classrun-request';
import { ISearchClassRunRequest } from '../dtos/search-classrun-request';
import { IToggleCourseAutomateRequest } from './../dtos/toggle-course-automate-request';
import { IToggleCourseCriteriaRequest } from './../dtos/toggle-course-criteria-request';
import { ITotalParticipantClassRunRequest } from '../dtos/total-participant-classrun-request';
import { Injectable } from '@angular/core';
import { SearchClassRunResult } from '../dtos/search-classrun-result';
import { SearchClassRunType } from '../models/search-classrun-type.model';
import { TotalParticipantClassRunResult } from '../dtos/total-participant-classrun-result';
import { map } from 'rxjs/operators';

@Injectable()
export class ClassRunApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public saveClassRun(request: ISaveClassRunRequest): Promise<ClassRun> {
    return this.post<ISaveClassRunRequest, IClassRun>(`/classrun/save`, request)
      .pipe(map(data => new ClassRun(data)))
      .toPromise();
  }
  public getClassRunById(id: string, loadHasLearnerStarted: boolean = false, showSpinner?: boolean): Promise<ClassRun> {
    return this.get<IClassRun>(`/classrun/${id}`, { loadHasLearnerStarted: loadHasLearnerStarted }, showSpinner)
      .pipe(map(data => new ClassRun(data)))
      .toPromise();
  }
  public getClassRunByCourseId(
    courseId: string,
    searchType: SearchClassRunType = SearchClassRunType.Owner,
    searchText: string = '',
    filter: IFilter = null,
    notStarted: boolean = false,
    notEnded: boolean = false,
    skipCount: number = 0,
    maxResultCount: number = 10,
    loadHasContentInfo?: boolean,
    showSpinner?: boolean
  ): Promise<SearchClassRunResult> {
    return this.post<ISearchClassRunRequest, SearchClassRunResult>(
      `/classrun/byCourseId`,
      {
        courseId,
        searchText,
        filter,
        notStarted,
        notEnded,
        skipCount,
        searchType,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount,
        loadHasContentInfo
      },
      showSpinner
    )
      .pipe(
        map(_ => {
          return new SearchClassRunResult(_);
        })
      )
      .toPromise();
  }

  public changeClassRunStatus(request: IChangeClassRunStatusRequest): Promise<void> {
    return this.put<IChangeClassRunStatusRequest, void>(`/classrun/changeStatus`, request).toPromise();
  }

  public checkClassIsFull(classRunId: string): Promise<boolean> {
    return this.get<boolean>(`/classrun/checkClassIsFull`, { classRunId }).toPromise();
  }

  public changeCancellationStatus(request: IClassRunCancellationStatusRequest): Promise<void> {
    return this.put<IClassRunCancellationStatusRequest, void>(`/classrun/changeCancellationStatus`, request).toPromise();
  }

  public changeRescheduleStatus(request: IClassRunRescheduleStatusRequest): Promise<void> {
    return this.put<IClassRunRescheduleStatusRequest, void>(`/classrun/changeRescheduleStatus`, request).toPromise();
  }

  public getClassRunsByIds(classRunIds: string[], showSpinner?: boolean): Promise<ClassRun[]> {
    return this.post<string[], IClassRun[]>(`/classrun/getClassRunsByIds`, classRunIds, showSpinner)
      .pipe(map(classruns => classruns.map(classrun => new ClassRun(classrun))))
      .toPromise();
  }

  public toggleCourseCriteria(request: IToggleCourseCriteriaRequest): Promise<void> {
    return this.put<IToggleCourseCriteriaRequest, void>(`/classrun/toggleCourseCriteria`, request).toPromise();
  }

  public toggleCourseAutomate(request: IToggleCourseAutomateRequest): Promise<void> {
    return this.put<IToggleCourseAutomateRequest, void>(`/classrun/toggleCourseAutomate`, request).toPromise();
  }

  public getTotalParticipantInClassRun(request: ITotalParticipantClassRunRequest): Promise<TotalParticipantClassRunResult[]> {
    return this.post<ITotalParticipantClassRunRequest, TotalParticipantClassRunResult[]>(
      `/classrun/getTotalParticipantInClassRun`,
      request
    ).toPromise();
  }
}
