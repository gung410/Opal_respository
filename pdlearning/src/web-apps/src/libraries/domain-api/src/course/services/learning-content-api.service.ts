import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { CourseContentItemModel, ICourseContentItemModel } from '../models/course-content-item.model';
import { ILectureIdMapNameModel, LectureIdMapNameModel } from '../models/lecture-id-map-name.model';
import { ILectureModel, LectureModel } from '../models/lecture.model';
import { ISectionModel, SectionModel } from '../models/section.model';

import { IChangeClassRunContentStatusRequest } from '../dtos/change-classrun-content-status-request';
import { IChangeContentOrderRequest } from '../dtos/change-content-order-request';
import { IChangeCourseContentStatusRequest } from '../dtos/change-course-content-status-request';
import { ICloneContentForClassRunRequest } from '../dtos/clone-content-for-classrun-request';
import { ICloneContentForCourseRequest } from '../dtos/clone-content-for-course-request';
import { ISaveLectureRequest } from '../dtos/save-lecture-request';
import { ISaveSectionRequest } from '../dtos/save-section-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class LearningContentApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getTableOfContents(
    courseId: string,
    classRunId: string,
    searchText: string = '',
    includeAdditionalInfo: boolean = false,
    showSpinner: boolean = true
  ): Promise<CourseContentItemModel[]> {
    return this.get<ICourseContentItemModel[]>(
      `/learningcontent/${courseId}/toc`,
      { classRunId, searchText, includeAdditionalInfo },
      showSpinner
    )
      .pipe(map(result => result.map(p => new CourseContentItemModel(p))))
      .toPromise();
  }

  public cloneContentForClassRun(request: ICloneContentForClassRunRequest, showSpinner: boolean = true): Promise<CourseContentItemModel[]> {
    return this.post<object, ICourseContentItemModel[]>(`/learningcontent/cloneContentForClassRun`, request, showSpinner)
      .pipe(map(result => result.map(p => new CourseContentItemModel(p))))
      .toPromise();
  }

  public cloneContentForCourse(request: ICloneContentForCourseRequest, showSpinner: boolean = true): Promise<CourseContentItemModel[]> {
    return this.post<object, ICourseContentItemModel[]>(`/learningcontent/cloneContentForCourse`, request, showSpinner)
      .pipe(map(result => result.map(p => new CourseContentItemModel(p))))
      .toPromise();
  }

  public changeCourseContentStatus(request: IChangeCourseContentStatusRequest): Promise<void> {
    return this.put<IChangeCourseContentStatusRequest, void>(`/learningcontent/changeCourseContentStatus`, request).toPromise();
  }

  public getSection(id: string, showSpinner: boolean = true): Promise<SectionModel> {
    return this.get<SectionModel>(`/learningcontent/sections/${id}`, null, showSpinner)
      .pipe(map(_ => new SectionModel(_)))
      .toPromise();
  }
  public saveSection(request: ISaveSectionRequest): Promise<SectionModel> {
    return this.post<ISaveSectionRequest, ISectionModel>(`/learningcontent/sections/save`, request)
      .pipe(map(data => new SectionModel(data)))
      .toPromise();
  }

  public getLecture(id: string, showSpinner: boolean = true): Promise<LectureModel> {
    return this.get<LectureModel>(`/learningcontent/lectures/${id}`, null, showSpinner)
      .pipe(map(_ => new LectureModel(_)))
      .toPromise();
  }

  public getAllLectureIdsOfCourse(courseId: string): Promise<string[]> {
    return this.get<string[]>(`/learningcontent/${courseId}/lectures/getIds`).toPromise();
  }

  public saveLecture(request: ISaveLectureRequest, showSpinner?: boolean): Promise<LectureModel> {
    return this.post<ISaveLectureRequest, ILectureModel>(`/learningcontent/lectures/save`, request, showSpinner)
      .pipe(map(_ => new LectureModel(_)))
      .toPromise();
  }

  public deleteSection(id: string, courseId: string): Promise<void> {
    return this.delete<void>(`/learningcontent/${courseId}/sections/${id}`).toPromise();
  }

  public deleteLecture(id: string, courseId: string): Promise<void> {
    return this.delete<void>(`/learningcontent/${courseId}/lectures/${id}`).toPromise();
  }

  public changeContentOrder(request: IChangeContentOrderRequest): Promise<CourseContentItemModel[]> {
    return this.put<IChangeContentOrderRequest, ICourseContentItemModel[]>(
      `/learningcontent/${request.courseId}/toc/changeContentOrder`,
      request
    )
      .pipe(map(_ => _.map(__ => new CourseContentItemModel(__))))
      .toPromise();
  }

  /**
   * Check that resourceId has been referenced in any courses via leature content.
   * Return a boolean value. True mean has referenced, false vice versa.
   */
  public hasReferenceToResource(resourceId: string, showSpinner?: boolean): Promise<boolean> {
    return this.post<unknown, boolean>(`/learningcontent/hasReferenceToResource`, { resourceId: resourceId }, showSpinner).toPromise();
  }

  public changeClassRunContentStatus(request: IChangeClassRunContentStatusRequest): Promise<void> {
    return this.put<IChangeClassRunContentStatusRequest, void>(`/learningcontent/changeClassRunContentStatus`, request).toPromise();
  }

  public getAllLectureNamesByListIds(lectureIds: string[], showSpinner: boolean = true): Promise<LectureIdMapNameModel[]> {
    return this.post<string[], ILectureIdMapNameModel[]>(`/learningcontent/lectures/getAllNamesByListIds`, lectureIds, showSpinner)
      .pipe(map(res => res.map(_ => new LectureIdMapNameModel(_))))
      .toPromise();
  }
}
