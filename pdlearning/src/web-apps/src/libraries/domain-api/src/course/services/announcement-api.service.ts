import { AnnouncementTemplate, IAnnouncementTemplate } from '../models/announcement-template.model';
import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';
import { IPreviewAnnouncementTemplate, PreviewAnnouncementTemplate } from '../models/preview-announcement-template.model';
import { ISearchAnnouncementResult, SearchAnnouncementResult } from './../dtos/search-announcement-result';
import { ISearchAnnouncementTemplateResult, SearchAnnouncementTemplateResult } from '../dtos/search-announcement-template-result';
import {
  ISendAnnouncemmentEmailTemplateModel,
  SendAnnouncemmentEmailTemplateModel
} from '../models/send-announcement-email-template.model';

import { AnnouncementType } from '../models/announcement-type.model';
import { Constant } from '@opal20/authentication';
import { IChangeAnnouncementStatusRequest } from './../dtos/change-announcement-status-request';
import { IPreviewAnnouncementRequest } from './../dtos/preview-announcement-request';
import { ISaveAnnouncementTemplateRequest } from '../dtos/save-announcement-template-request';
import { ISearchAnnoucementRequest } from './../dtos/search-announcement-request';
import { ISendAnnouncementRequest } from '../dtos/send-announcement-request';
import { ISendNominationRequest } from '../dtos/send-course-nomination-request';
import { ISendOrderRefreshmentRequest } from '../dtos/send-order-refreshment-request';
import { ISendPlacementLetterRequest } from '../dtos/send-placement-letter-request';
import { ISendPublicityRequest } from '../dtos/send-course-publicity-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class AnnouncementApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public searchAnnouncementTemplate(
    searchText: string = '',
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<SearchAnnouncementTemplateResult> {
    return this.get<ISearchAnnouncementTemplateResult>(
      '/announcement/searchTemplate',
      {
        searchText,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(
        map(_ => {
          return new SearchAnnouncementTemplateResult(_);
        })
      )
      .toPromise();
  }

  public saveAnnouncementTemplate(request: ISaveAnnouncementTemplateRequest, showSpinner: boolean = true): Promise<AnnouncementTemplate> {
    return this.post<ISaveAnnouncementTemplateRequest, IAnnouncementTemplate>(`/announcement/saveTemplate`, request, showSpinner)
      .pipe(map(data => new AnnouncementTemplate(data)))
      .toPromise();
  }

  public deleteAnnouncementTemplate(announcementTemplateId: string, showSpinner: boolean = true): Promise<void> {
    return this.delete<void>(`/announcement/${announcementTemplateId}`, showSpinner).toPromise();
  }

  public sendAnnouncement(request: ISendAnnouncementRequest, showSpinner: boolean = true): Promise<AnnouncementTemplate> {
    return this.post<ISendAnnouncementRequest, AnnouncementTemplate>(`/announcement/sendAnnouncement`, request, showSpinner).toPromise();
  }

  public searchAnnouncement(
    courseId: string,
    classRunId: string,
    filter: IFilter,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<SearchAnnouncementResult> {
    return this.post<ISearchAnnoucementRequest, ISearchAnnouncementResult>(
      '/announcement/search',
      {
        courseId,
        classRunId,
        filter,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(
        map(_ => {
          return new SearchAnnouncementResult(_);
        })
      )
      .toPromise();
  }

  public changeAnnouncementStatus(request: IChangeAnnouncementStatusRequest): Promise<void> {
    return this.put<IChangeAnnouncementStatusRequest, void>(`/announcement/changeStatus`, request).toPromise();
  }

  public getSendAnnouncementDefaultTemplate(
    announcementType: AnnouncementType = AnnouncementType.CoursePublicity,
    courseId?: string,
    showSpinner?: boolean
  ): Promise<SendAnnouncemmentEmailTemplateModel> {
    return this.post<object, ISendAnnouncemmentEmailTemplateModel>(
      `/announcement/getSendAnnouncementDefaultTemplate`,
      {
        announcementType,
        courseId
      },
      showSpinner
    )
      .pipe(map(data => new SendAnnouncemmentEmailTemplateModel(data)))
      .toPromise();
  }

  public previewAnnouncementTemplate(request: IPreviewAnnouncementRequest, showSpinner?: boolean): Promise<IPreviewAnnouncementTemplate> {
    return this.post<IPreviewAnnouncementRequest, IPreviewAnnouncementTemplate>(
      `/announcement/previewAnnouncementTemplate`,
      request,
      showSpinner
    )
      .pipe(map(data => new PreviewAnnouncementTemplate(data)))
      .toPromise();
  }

  public sendCoursePublicity(request: ISendPublicityRequest): Promise<void> {
    return this.post<ISendPublicityRequest, void>(`/announcement/sendCoursePublicity`, request).toPromise();
  }

  public sendCourseNominationAnnoucement(request: ISendNominationRequest): Promise<void> {
    return this.post<ISendNominationRequest, void>(`/announcement/sendCourseNominationAnnoucement`, request).toPromise();
  }

  public sendPlacementLetter(request: ISendPlacementLetterRequest): Promise<void> {
    return this.post<ISendPlacementLetterRequest, void>('/announcement/sendPlacementLetter', request).toPromise();
  }

  public sendOrderRefreshment(request: ISendOrderRefreshmentRequest): Promise<void> {
    return this.post<ISendOrderRefreshmentRequest, void>('/announcement/sendOrderRefreshment', request).toPromise();
  }
}
