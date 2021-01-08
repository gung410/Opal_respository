import { BaseRepository, IFilter, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { AnnouncementApiService } from '../services/announcement-api.service';
import { AnnouncementTemplate } from '../models/announcement-template.model';
import { AnnouncementType } from '../models/announcement-type.model';
import { CourseRepositoryContext } from '../course-repository-context';
import { IChangeAnnouncementStatusRequest } from '../dtos/change-announcement-status-request';
import { ISaveAnnouncementTemplateRequest } from '../dtos/save-announcement-template-request';
import { ISendAnnouncementRequest } from '../dtos/send-announcement-request';
import { Injectable } from '@angular/core';
import { SearchAnnouncementResult } from '../dtos/search-announcement-result';
import { SearchAnnouncementTemplateResult } from '../dtos/search-announcement-template-result';
import { SendAnnouncemmentEmailTemplateModel } from '../models/send-announcement-email-template.model';

@Injectable()
export class AnnouncementRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: AnnouncementApiService) {
    super(context);
  }

  public searchAnnouncementTemplate(
    searchText?: string,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Observable<SearchAnnouncementTemplateResult> {
    return this.processUpsertData(
      this.context.announcementTemplateSubject,
      implicitLoad => from(this.apiSvc.searchAnnouncementTemplate(searchText, skipCount, maxResultCount, !implicitLoad && showSpinner)),
      'searchAnnouncementTemplate',
      [searchText, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true
    );
  }

  public saveAnnouncementTemplate(request: ISaveAnnouncementTemplateRequest, showSpinner: boolean = true): Promise<AnnouncementTemplate> {
    return this.apiSvc.saveAnnouncementTemplate(request, showSpinner).then(announcementTemplate => {
      this.upsertData(
        this.context.announcementTemplateSubject,
        [new AnnouncementTemplate(Utils.cloneDeep(announcementTemplate))],
        item => item.id,
        true
      );
      this.processRefreshData('searchAnnouncementTemplate');
      return announcementTemplate;
    });
  }

  public deleteAnnouncementTemplate(announcementTemplateId: string, showSpinner: boolean = true): Promise<void> {
    return this.apiSvc.deleteAnnouncementTemplate(announcementTemplateId, showSpinner).then(_ => {
      this.processRefreshData('searchAnnouncementTemplate');
    });
  }

  public sendAnnouncement(request: ISendAnnouncementRequest, showSpinner: boolean = true): Promise<AnnouncementTemplate> {
    return this.apiSvc.sendAnnouncement(request, showSpinner).then(_ => {
      if (request.data.saveTemplate) {
        this.processRefreshData('searchAnnouncementTemplate');
      }
      return _;
    });
  }

  public searchAnnouncement(
    courseId: string,
    classRunId: string,
    filter: IFilter,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Observable<SearchAnnouncementResult> {
    return this.processUpsertData(
      this.context.announcementSubject,
      implicitLoad =>
        from(this.apiSvc.searchAnnouncement(courseId, classRunId, filter, skipCount, maxResultCount, !implicitLoad && showSpinner)),
      'searchAnnouncement',
      [courseId, classRunId, filter, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true
    );
  }

  public changeAnnouncementStatus(request: IChangeAnnouncementStatusRequest): Observable<void> {
    return from(
      this.apiSvc.changeAnnouncementStatus(request).then(_ => {
        this.processRefreshData('searchAnnouncement');
        return _;
      })
    );
  }

  public getSendAnnouncementDefaultTemplate(
    announcementType: AnnouncementType = AnnouncementType.CoursePublicity,
    courseId?: string,
    showSpinner: boolean = true
  ): Observable<SendAnnouncemmentEmailTemplateModel> {
    return this.processUpsertData(
      this.context.emailTemplateSubject,
      implicitLoad => from(this.apiSvc.getSendAnnouncementDefaultTemplate(announcementType, courseId, !implicitLoad && showSpinner)),
      'getSendCourseAnnouncementDefaultTemplate',
      [announcementType, courseId],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true,
      null,
      null,
      null
    );
  }
}
