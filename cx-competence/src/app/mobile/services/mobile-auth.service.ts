import { EventEmitter, Injectable } from '@angular/core';
import { AuthDataService } from 'app-auth/auth-data.service';
import { AuthService } from 'app-auth/auth.service';
import { AccessRightModuleEnum } from 'app-models/access-right/access-right-module.enum';
import { SiteData, User } from 'app-models/auth.model';
import { Identity } from 'app-models/common.model';
import { AccessRightService } from 'app-services/access-right.service';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { UserService } from 'app-services/user.service';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import {
  PDPlanActionEnum,
  PDPlannerSetTokenPayload,
  PDPlannerViewSizeChangePayload,
  WebViewMessage,
} from '../models/webview-message.model';

@Injectable()
export class MobileAuthService {
  // Event fire when SPA got access token when init
  onAccessTokenInitReady: EventEmitter<void> = new EventEmitter<void>();
  onAccessTokenUpdated: EventEmitter<void> = new EventEmitter<void>();
  currentUser: User;
  siteData: SiteData;
  initedData: boolean;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private authDataService: AuthDataService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private accessRightService: AccessRightService
  ) {}

  initForCommunicateService(): void {
    const onExternalMessage = 'onExternalMessage';
    window[onExternalMessage] = this.onMessage;
    window.addEventListener('message', this.onWindowMessage);
    if (this.authService.hasValidAccessToken()) {
      this.initDataWhenAccessTokenReady();
    }
    setTimeout(() => {
      const notifySPAInitedMessage = new WebViewMessage();
      notifySPAInitedMessage.action = PDPlanActionEnum.WEBVIEW_READY;
      this.sendMessage(notifySPAInitedMessage);
    });
  }

  async getCurrentUserIdAsync(): Promise<number> {
    const currentUser = await this.getCurrentUserProfileAsync();

    return currentUser.identity.id;
  }

  async getCurrentUserIdentityAsync(): Promise<Identity> {
    const currentUser = await this.getCurrentUserProfileAsync();

    return currentUser.identity;
  }

  async getCurrentUserProfileAsync(): Promise<User> {
    if (!this.currentUser) {
      const user = new User({});
      const userProfile = await this.authDataService.getUserProfileAsync();
      if (!userProfile) {
        return;
      }

      user.emails = userProfile.emails;
      user.fullName = userProfile.fullName;
      user.departmentId = userProfile.departmentId;
      user.systemRoles = userProfile.systemRoles;
      user.id = userProfile.identity ? userProfile.identity.id : undefined;
      user.extId = userProfile.identity
        ? userProfile.identity.extId
        : undefined;
      user.identity = userProfile.identity;
      user.avatarUrl = userProfile.jsonDynamicAttributes
        ? userProfile.jsonDynamicAttributes.avatarUrl
        : undefined;
      user.permissions = await this.accessRightService
        .getMyPermissions({
          modules: [AccessRightModuleEnum.PDPM, AccessRightModuleEnum.Learner],
        })
        .toPromise();

      this.currentUser = user;
      this.authService.updateUser(user);
    }

    return this.currentUser;
  }

  async getCurrentStaffProfile(): Promise<Staff> {
    const userId = await this.getCurrentUserIdAsync();
    if (userId) {
      return await this.userService.getStaffProfile(userId);
    }
  }

  sendMessage(message: WebViewMessage): void {
    if (window.hasOwnProperty('postMessage')) {
      const messageString = JSON.stringify(message);
      window.postMessage(messageString, '*');
      if (window.parent) {
        window.parent.postMessage(messageString, '*');
      }
    }
  }

  get hasValidToken(): boolean {
    return this.authService.hasValidAccessToken();
  }

  sendMessageViewSizeChange(width: number, height: number): void {
    const payload = new PDPlannerViewSizeChangePayload(width, height);
    const message = new WebViewMessage({
      action: PDPlanActionEnum.VIEW_SIZE_CHANGE,
      payload,
    });
    this.sendMessage(message);
  }

  private processSetTokenMessage = async (
    payload: PDPlannerSetTokenPayload
  ): Promise<void> => {
    if (!payload || !payload.accessToken) {
      return;
    }

    const token = this.authService.getAccessToken();
    this.authService.setAccessToken(payload.accessToken);

    // Check case first launch
    if (!token || token === '') {
      await this.initDataWhenAccessTokenReady();
      this.onAccessTokenInitReady.emit();
    } else {
      this.onAccessTokenUpdated.emit();
    }
  };

  private async initDataWhenAccessTokenReady(): Promise<void> {
    if (!this.initedData) {
      const currentUser = await this.getCurrentUserProfileAsync();
      this.siteData = await this.authDataService.getSiteDataByCurrentUser();
      this.cxSurveyjsExtendedService.initCxSurveyVariable(currentUser);
      this.initedData = true;
    }
  }

  private onWindowMessage = (eventData: any): void => {
    if (!eventData || eventData.origin !== location.origin) {
      return;
    }
    this.onMessage(eventData.data);
  };

  private onMessage = (messageObject: object): void => {
    let message: WebViewMessage;
    if (typeof messageObject === 'string') {
      try {
        message = JSON.parse(messageObject) as WebViewMessage;
      } catch (error) {
        console.error('Cannot parse message');

        return;
      }
    }

    if (typeof messageObject === 'object') {
      message = messageObject as WebViewMessage;
    }

    if (!message || !message.action) {
      return;
    }

    switch (message.action) {
      case PDPlanActionEnum.SET_ACCESS_TOKEN:
        const payload = message.payload as PDPlannerSetTokenPayload;
        this.processSetTokenMessage(payload);
        break;
      default:
        break;
    }
  };
}
