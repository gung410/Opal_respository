import { BaseRepository, RepoLoadStrategy, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { ClassRun } from '../models/classrun.model';
import { Course } from '../models/course.model';
import { CourseContentItemModel } from '../models/course-content-item.model';
import { CourseRepositoryContext } from '../course-repository-context';
import { IChangeClassRunContentStatusRequest } from '../dtos/change-classrun-content-status-request';
import { IChangeContentOrderRequest } from '../dtos/change-content-order-request';
import { IChangeCourseContentStatusRequest } from '../dtos/change-course-content-status-request';
import { ICloneContentForClassRunRequest } from '../dtos/clone-content-for-classrun-request';
import { ICloneContentForCourseRequest } from '../dtos/clone-content-for-course-request';
import { ISaveLectureRequest } from '../dtos/save-lecture-request';
import { ISaveSectionRequest } from './../dtos/save-section-request';
import { Injectable } from '@angular/core';
import { LearningContentApiService } from '../services/learning-content-api.service';
import { LectureIdMapNameModel } from '../models/lecture-id-map-name.model';
import { LectureModel } from '../models/lecture.model';
import { SectionModel } from './../models/section.model';

@Injectable()
export class LearningContentRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: LearningContentApiService) {
    super(context);
  }

  public getTableOfContents(
    courseId: string,
    classRunId: string | null = null,
    searchText: string = '',
    includeAdditionalInfo: boolean = false,
    strategy: RepoLoadStrategy = 'implicitReload'
  ): Observable<CourseContentItemModel[]> {
    return this.processUpsertData(
      this.context.coursesContentSubject,
      implicitLoad => from(this.apiSvc.getTableOfContents(courseId, classRunId, searchText, includeAdditionalInfo, !implicitLoad)),
      'getTableOfContents',
      [courseId, classRunId, searchText, includeAdditionalInfo],
      strategy,
      (repoData, apiResult) => {
        return apiResult.map(_ => repoData[_.id]).filter(p => p != null);
      },
      apiResult => apiResult,
      x => x.id,
      true
    );
  }

  public cloneContentForClassRun(request: ICloneContentForClassRunRequest): Promise<CourseContentItemModel[]> {
    return this.apiSvc.cloneContentForClassRun(request).then(_ => {
      this.refreshForEditingContent(request.courseId, request.classRunId);
      return _;
    });
  }

  public cloneContentForCourse(request: ICloneContentForCourseRequest): Promise<CourseContentItemModel[]> {
    return this.apiSvc.cloneContentForCourse(request).then(_ => {
      this.refreshForEditingContent(request.toCourseId);
      return _;
    });
  }

  public getLecture(id: string): Observable<LectureModel> {
    return this.processUpsertData(
      this.context.lectureSubject,
      implicitLoad => from(this.apiSvc.getLecture(id, !implicitLoad)),
      'getLecture',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        return repoData[apiResult.id];
      },
      apiResult => [apiResult],
      x => x.id,
      true
    );
  }

  public getSection(id: string): Observable<SectionModel> {
    return this.processUpsertData(
      this.context.sectionSubject,
      implicitLoad => from(this.apiSvc.getSection(id, !implicitLoad)),
      'getSection',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        return repoData[apiResult.id];
      },
      apiResult => [apiResult],
      x => x.id,
      true
    );
  }

  public saveSection(request: ISaveSectionRequest): Observable<SectionModel> {
    return from(
      this.apiSvc.saveSection(request).then(section => {
        this.upsertData(this.context.sectionSubject, [Utils.cloneDeep(section)], item => item.id, true);
        this.refreshForEditingContent(request.data.courseId, request.data.classRunId);
        return section;
      })
    );
  }

  public saveLecture(request: ISaveLectureRequest, showSpinner?: boolean): Observable<LectureModel> {
    return from(
      this.apiSvc.saveLecture(request, showSpinner).then(lecture => {
        this.upsertData(this.context.lectureSubject, [Utils.cloneDeep(lecture)], item => item.id, true);
        this.refreshForEditingContent(request.courseId, request.classRunId);
        return lecture;
      })
    );
  }

  public deleteLecture(id: string, courseId: string, classRunId?: string): Promise<void> {
    return this.apiSvc.deleteLecture(id, courseId).then(_ => {
      this.refreshForEditingContent(courseId, classRunId);
    });
  }
  public deleteSection(id: string, courseId: string, classRunId?: string): Promise<void> {
    return this.apiSvc.deleteSection(id, courseId).then(_ => {
      this.refreshForEditingContent(courseId, classRunId);
    });
  }

  public changeCourseContentStatus(request: IChangeCourseContentStatusRequest): Observable<void> {
    return from(
      this.apiSvc.changeCourseContentStatus(request).then(_ => {
        this.upsertData(
          this.context.coursesSubject,
          request.ids.map(id => <Partial<Course>>{ id: id, contentStatus: request.contentStatus }),
          item => item.id
        );
        return _;
      })
    );
  }

  public changeClassRunContentStatus(request: IChangeClassRunContentStatusRequest): Observable<void> {
    return from(
      this.apiSvc.changeClassRunContentStatus(request).then(_ => {
        this.upsertData(
          this.context.classRunSubject,
          request.ids.map(id => <Partial<ClassRun>>{ id: id, contentStatus: request.contentStatus }),
          item => item.id
        );
        this.processRefreshData('loadClassRunsByCourseId');
        return _;
      })
    );
  }

  public changeContentOrder(request: IChangeContentOrderRequest): Promise<CourseContentItemModel[]> {
    return this.apiSvc.changeContentOrder(request).then(_ => {
      this.refreshForEditingContent(request.courseId, request.classRunId);
      return _;
    });
  }

  public loadAllLectureNamesByListIds(lectureIds: string[]): Observable<LectureIdMapNameModel[]> {
    return this.processUpsertData(
      this.context.lectureIdMapNameSubject,
      implicitLoad => from(this.apiSvc.getAllLectureNamesByListIds(lectureIds, !implicitLoad)),
      'loadAllLectureNamesByListIds',
      [lectureIds],
      'implicitReload',
      (repoData, apiResult) => {
        return apiResult.map(_ => repoData[_.lectureId]).filter(p => p != null);
      },
      apiResult => apiResult,
      x => x.lectureId,
      true
    );
  }

  private refreshForEditingContent(courseId: string, classRunId?: string): void {
    this.processRefreshData('getTableOfContents', [courseId, classRunId]);
    this.processRefreshData('loadCourse', [courseId]);
    if (classRunId != null) {
      this.processRefreshData('loadClassRunById', [classRunId]);
    }
  }
}
