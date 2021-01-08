import { AuthService, UrlHelperService } from '@opal20/authentication';

import { CommunityInfo } from '../models/community-info.model';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CommunityIntergrationService {
  protected urlParameters: { [key: string]: string | number | unknown };
  protected communityInfo: CommunityInfo = new CommunityInfo();

  constructor(protected urlHelper: UrlHelperService, private authService: AuthService) {}

  public verifyIntergrationUrl(): void {
    let urlParamString = location.search;
    if (urlParamString && urlParamString.startsWith('?')) {
      urlParamString = urlParamString.substr(1, urlParamString.length - 1);
    }
    this.urlParameters = this.urlHelper.parseQueryString(urlParamString) as {
      [key: string]: unknown;
    };

    if (!this.urlParameters.data) {
      throw new Error('The data was missed');
    }

    const communityInfoJson = atob(this.urlParameters.data as string);

    this.communityInfo = JSON.parse(communityInfoJson);
    this.communityInfo.startDate = this.communityInfo.startDate ? new Date(this.communityInfo.startDate) : null;
    this.communityInfo.archivedDate = this.communityInfo.archivedDate ? new Date(this.communityInfo.archivedDate) : null;
    this.communityInfo.lastPostedDate = this.communityInfo.lastPostedDate ? new Date(this.communityInfo.lastPostedDate) : null;

    if (!this.communityInfo.accessToken) {
      throw new Error('The access token was missed');
    }

    if (!this.communityInfo.communityId) {
      throw new Error('The community ID was missed');
    }

    this.authService.setAccessToken(this.communityInfo.accessToken);
  }

  public getCommunityInfo(): CommunityInfo {
    return this.communityInfo;
  }
}
