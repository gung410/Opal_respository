import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { ECertificateModel, IECertificateModel } from '../models/ecertificate.model';
import { Observable, combineLatest, from } from 'rxjs';

import { CourseApiService } from '../../course/services/course-api.service';
import { DigitalBadgeModel } from '../models/digitalbadge.model';
import { HttpResponse } from '@angular/common/http';
import { IMyECertificateRequest } from '../dtos/my-ecertificate-request';
import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';
import { Injectable } from '@angular/core';
import { MY_ACHIEVEMENT_TYPE_ENUM } from 'modules/learner/src/constants/my-achievement.constant';
import { SearchRegistrationResult } from '../../course/dtos/search-registration-result';
import { TrackingSharedDetailByModel } from '../models/tracking-shared-detail-to.model';
import { map } from 'rxjs/operators';

@Injectable()
export class MyAchievementAPIService extends BaseBackendService {
  protected serviceType: MY_ACHIEVEMENT_TYPE_ENUM = MY_ACHIEVEMENT_TYPE_ENUM.ECertificates;

  protected get apiUrl(): string {
    switch (this.serviceType) {
      case MY_ACHIEVEMENT_TYPE_ENUM.ECertificates:
        return AppGlobal.environment.courseApiUrl;
      case MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges:
        return AppGlobal.environment.badgeApiUrl + '/badges';
      default:
        return '';
    }
  }

  constructor(protected commonFacadeService: CommonFacadeService, private courseApiService: CourseApiService) {
    super(commonFacadeService);
  }

  public initApiService(serviceType: MY_ACHIEVEMENT_TYPE_ENUM): void {
    this.serviceType = serviceType;
  }

  public getMyECertificates(request: IMyECertificateRequest, showSpinner: boolean = true): Promise<IPagedResultDto<ECertificateModel>> {
    return this.post<IMyECertificateRequest, SearchRegistrationResult>('/registration/search', request, showSpinner)
      .toPromise()
      .then(_ => {
        const courseIds = _.items.map(p => p.courseId);
        return combineLatest(from(this.courseApiService.getCoursesByIds(courseIds)))
          .pipe(
            map(([dataCourses]) => {
              const items: ECertificateModel[] = dataCourses.map(p => {
                const registration = _.items.find(m => m.courseId === p.id);
                const eCertificate: IECertificateModel = {
                  id: registration.id,
                  title: p.courseName,
                  tagIds: TrackingSharedDetailByModel.buildTags(p),
                  completionDate: registration.registrationDate
                };
                return eCertificate;
              });

              const data: IPagedResultDto<ECertificateModel> = {
                totalCount: _.totalCount,
                items: items
              };

              return data;
            })
          )
          .toPromise();
      });
  }

  public downloadECertificate(
    id: string,
    fileFormat: string,
    isIOSDevice: boolean = false,
    showSpinner: boolean = true
  ): Observable<HttpResponse<Blob>> {
    return this.getFileInfo(`/ecertificate/${id}/download-ecertificate?fileFormat=${fileFormat}`, {}, showSpinner, isIOSDevice);
  }

  public getMyDigitalBadges1(showSpinner: boolean = true): Promise<DigitalBadgeModel[]> {
    return this.get<DigitalBadgeModel[]>('/currentUser/GetBadges', {}, showSpinner).toPromise();
  }

  public getMyDigitalBadges(request: IPagedResultRequestDto, showSpinner: boolean = true): Promise<IPagedResultDto<DigitalBadgeModel>> {
    const queryParams: IGetParams = {
      skipCount: request.skipCount,
      maxResultCount: request.maxResultCount
    };
    const data = this.getECertificatesMockup(request);
    return new Promise(resolve => {
      const mockup: IPagedResultDto<DigitalBadgeModel> = {
        totalCount: data.totalCount,
        items: data.items
      };
      resolve(mockup);
    });
  }

  private getECertificatesMockup(request: IPagedResultRequestDto): IPagedResultDto<DigitalBadgeModel> {
    const items: DigitalBadgeModel[] = [];

    for (let i = 1; i < 50; i++) {
      const data: DigitalBadgeModel = {
        itemId: `itemId ${i}`,
        itemType: `itemType ${i}`,
        maxResultCount: i,
        skipCount: i,
        sharedByUsers: [`sharedByUsers 1`, `sharedByUsers 2`, `sharedByUsers 3`, `sharedByUsers 4`, `sharedByUsers 5`],
        tagIds: [`tagIds 5`, `tagIds 4`, `tagIds 3`, `tagIds 2`, `tagIds 1`],
        thumbnailUrl: `thumbnailUrl ${i}`,
        title: `Importance of effective communication for beginners ${i}`,
        completedDate: new Date('2020/12/27')
      };
      items.push(new DigitalBadgeModel(data));
    }

    const mockupResponse: IPagedResultDto<DigitalBadgeModel> = {
      totalCount: items.length,
      items: items.slice(request.skipCount, request.skipCount + request.maxResultCount)
    };
    return mockupResponse;
  }

  private getFileInfo<TBody>(
    url: string,
    body: TBody,
    showSpinner: boolean = true,
    isIOSDevice: boolean = false
  ): Observable<HttpResponse<Blob>> {
    return this.commonFacadeService.http.get(`${this.apiUrl}${url}`, this.getDownloadFileHttpOptions(showSpinner));
  }
}
