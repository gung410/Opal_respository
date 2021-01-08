import {
  DigitalBadgeModel,
  ECertificateModel,
  IMyECertificateRequest,
  IPagedResultDto,
  IPagedResultRequestDto,
  MyAchievementAPIService,
  SearchRegistrationsType
} from '@opal20/domain-api';

import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MY_ACHIEVEMENT_TYPE_ENUM } from '../constants/my-achievement.constant';
import { Observable } from 'rxjs';
@Injectable()
export class MyAchievementService {
  constructor(private myAchievementAPIService: MyAchievementAPIService) {}

  public getMyECertificates(skipCount?: number, maxResultCount?: number): Promise<IPagedResultDto<ECertificateModel>> {
    this.myAchievementAPIService.initApiService(MY_ACHIEVEMENT_TYPE_ENUM.ECertificates);

    const request: IMyECertificateRequest = {
      searchType: SearchRegistrationsType.IssuanceTracking,
      searchText: '',
      applySearchTextForCourse: true,
      myRegistrationOnly: true,
      skipCount: skipCount,
      maxResultCount: maxResultCount
    };
    return this.myAchievementAPIService.getMyECertificates(request);
  }

  public getECertificateFromImg(id: string, isIOSDevice: boolean = false): Observable<HttpResponse<Blob>> {
    this.myAchievementAPIService.initApiService(MY_ACHIEVEMENT_TYPE_ENUM.ECertificates);
    return this.myAchievementAPIService.downloadECertificate(id, 'IMAGE', isIOSDevice);
  }

  public downloadECertificate(id: string, isIOSDevice: boolean = false): Observable<HttpResponse<Blob>> {
    this.myAchievementAPIService.initApiService(MY_ACHIEVEMENT_TYPE_ENUM.ECertificates);
    return this.myAchievementAPIService.downloadECertificate(id, 'PDF', isIOSDevice);
  }

  public getMyDigitalBadges1(): Promise<DigitalBadgeModel[]> {
    this.myAchievementAPIService.initApiService(MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges);
    return this.myAchievementAPIService.getMyDigitalBadges1();
  }

  public getMyDigitalBadges(skipCount?: number, maxResultCount?: number): Promise<IPagedResultDto<DigitalBadgeModel>> {
    this.myAchievementAPIService.initApiService(MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges);

    const request: IPagedResultRequestDto = {
      skipCount: skipCount,
      maxResultCount: maxResultCount
    };
    return this.myAchievementAPIService.getMyDigitalBadges(request);
  }
}
