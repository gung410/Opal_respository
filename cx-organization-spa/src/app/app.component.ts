import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import {
  CxCommonService,
  CxFooterData,
  CxInformationDialogService,
  MultipleLanguages,
  NotificationItem,
  NotificationList
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserIdleService } from 'angular-user-idle';
import { AuthService } from 'app-auth/auth.service';
import { environment } from 'app-environments/environment';
import { User } from 'app-models/auth.model';
import {
  CommandBody,
  CommandIdentity,
  CommandPayload,
  CommandRouting,
  NotificationCommand
} from 'app-models/communication-command.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { NotificationDataService } from 'app-services/notification-data.service';
import { NotificationService } from 'app-services/notification.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { DeviceDetectorService } from 'ngx-device-detector';
import { reject } from 'q';
import Swal from 'sweetalert2';

import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { AppService } from './app.service';
import { AppConstant, RouteConstant } from './shared/app.constant';
import { BaseScreenComponent } from './shared/components/component.abstract';
import { findIndexCommon } from './shared/constants/common.const';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [AppService]
})
export class AppComponent
  extends BaseScreenComponent
  implements OnInit, AfterViewInit {
  googleAnalyticId: string;
  hasLoading: boolean = false;
  hasValidUser: boolean = false;
  showHeader: boolean = false;
  notifications: NotificationList;
  multipleLanguages: MultipleLanguages;
  isShowNotificationBell: boolean = environment.notification.enableShowBellIcon;
  notificationAlertUrl: SafeResourceUrl;
  enableBroadCast: boolean = environment.notification.enableBroadCast;
  cssClass: string;
  heightIframe: string = 'auto';

  private isSessionTimeoutWarning: boolean = false;
  private finishedInitFirebaseCloudMessaging: boolean = false;

  constructor(
    private router: Router,
    private deviceService: DeviceDetectorService,
    private translateAdapterService: TranslateAdapterService,
    protected authService: AuthService,
    changeDetectorRef: ChangeDetectorRef,
    private userIdle: UserIdleService,
    private notificationService: NotificationService,
    private notificationDataService: NotificationDataService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private globalModal: NgbModal,
    private informationDialogService: CxInformationDialogService,
    private cxCommonService: CxCommonService,
    private domSanitizer: DomSanitizer
  ) {
    super(changeDetectorRef, authService);
    this.router.events.subscribe((data) => {
      if (data instanceof NavigationEnd) {
        if (
          data.urlAfterRedirects.indexOf('/error') > findIndexCommon.notFound ||
          data.urlAfterRedirects.indexOf('/session-timeout') >
            findIndexCommon.notFound
        ) {
          this.hasLoading = false;
          this.showHeader = false;
        } else {
          this.showHeader = true;
        }
      }
    });
  }

  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent): void {
    this.receiveMessageFromIframe(event);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.setLanguageDefault();
    this.authService.isAuthenticating.next(true);
    this.getUserInfo();
    this.authService.configureWithNewConfigApi();
    this.initHeader();
    this.translateAdapterService
      .getValueBasedOnKey('Common.Mandatory_Text')
      .subscribe(
        (translatedText: string) =>
          (this.cxCommonService.textMandatoryNeedToShow = translatedText)
      );
  }

  ngAfterViewInit(): void {
    this.notificationAlertUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(
      environment.notification.alertUrl
    );
  }

  onLogout(): void {
    this.authService.logout();
  }

  onChangePassword(): void {
    this.authService.navigateToExternalSite(
      environment.issuer,
      AppConstant.externalNavigationPath.changePassword
    );
  }

  onGoToProfile(): void {
    // we have different profile url in uat and prod environment
    this.authService.navigateToExternalSite(
      environment.baseProfileAppUrl && environment.baseProfileAppUrl !== ''
        ? environment.baseProfileAppUrl
        : environment.issuer,
      AppConstant.externalNavigationPath.profile
    );
  }

  onOpenNotificationPopup(isOpen: boolean): void {
    if (isOpen) {
      this.notificationDataService
        .trackNotificationData(this.currentUser.id)
        .subscribe((notification) => {
          this.notifications.totalNewCount = 0;
        });
    }
  }

  onClickNotificationItem(notification: NotificationItem): void {
    if (!notification.dateReadUtc) {
      notification.dateReadUtc = new Date();
      const notificationCommand = this.buildNotificationCommandModel(
        notification.id,
        notification.messageId
      );
      // tslint:disable-next-line:no-empty
      this.notificationDataService
        .readNotification(notificationCommand)
        .subscribe(() => {});
    }
  }

  setupUserIdleDetection(): void {
    // Start watching for user inactivity.
    this.userIdle.startWatching();

    // Start watching when user idle is starting.
    this.userIdle.onTimerStart().subscribe((count) => {
      if (!this.isSessionTimeoutWarning) {
        const sessionTimeoutMessage: string = this.translateAdapterService.getValueImmediately(
          'SessionTimeoutPopup.Content'
        );
        this.informationDialogService
          .warning({ message: sessionTimeoutMessage })
          .then((result) => {
            this.userIdle.stopTimer();
            this.isSessionTimeoutWarning = false;
          });
        this.isSessionTimeoutWarning = true;
      }
    });

    // Start watch when time is up.
    this.userIdle.onTimeout().subscribe(() => {
      this.userIdle.stopWatching();
      this.globalModal.dismissAll();
      const cxSurveys: any = document.getElementsByTagName('cx-surveyjs');
      if (cxSurveys && cxSurveys[0]) {
        setTimeout(() => {
          cxSurveys[0].click();
        }, 100);
      }
      if (environment.userIdleTimeoutLogout) {
        this.authService.logout();
      } else {
        this.authService.clearStorages();
        Swal.close();
        this.router.navigate(['/' + AppConstant.siteURL.sessionTimeout]);
      }
      sessionStorage.setItem(
        AppConstant.sessionStorageVariable.timeout,
        'true'
      );
    });
  }

  get footerData(): CxFooterData {
    const releaseDate =
      (this.currentUser && this.currentUser.releaseDate) ||
      environment.site.dateRelease;

    return this.authService.getFooterData(releaseDate);
  }

  private buildNotificationCommandModel(
    notificatonId: string | number,
    messageId: string | number
  ): NotificationCommand {
    const currentDateUTC = new Date().toUTCString();
    const commandPayload = new CommandPayload({
      identity: new CommandIdentity({
        clientId: AppConstant.clientId,
        customerId: AppConstant.customerId.toString(),
        userId: this.currentUser.id
      }),
      body: new CommandBody({
        NotificationId: messageId
      })
    });
    const commandRouting = new CommandRouting({
      action: 'pdpm.communication.mark.notification.read',
      actionVersion: '1.0',
      entity: 'Notification',
      entityId: notificatonId
    });
    const notificationCommand = new NotificationCommand({
      type: 'event',
      created: currentDateUTC,
      payload: commandPayload,
      routing: commandRouting
    });

    return notificationCommand;
  }

  private getUserInfo(): void {
    this.hasLoading = true;
    this.authService.userData().subscribe((user: User) => {
      if (user) {
        this.currentUser = user;
        this.initSurveyJSVariables(user);
        this.initHeader();
        this.hasLoading = false;
        this.hasValidUser = true;
        this.setupUserIdleDetection();
        if (this.isShowNotificationBell) {
          if (!this.finishedInitFirebaseCloudMessaging) {
            this.initFirebaseCloudMessageService(this.currentUser.id)
              .then(() => {
                this.finishedInitFirebaseCloudMessaging = true;
              })
              .catch((err) => {
                this.finishedInitFirebaseCloudMessaging = true;
              });
          }
        }
      } else {
        this.authService
          .loadDiscoveryDocumentAndTryLogin()
          .then(this.actionForValidUser, (_) => {
            this.hasLoading = false;
          });
      }
    });
  }

  private initSurveyJSVariables(user: User): void {
    this.cxSurveyjsExtendedService.setAPIVariables();
    this.cxSurveyjsExtendedService.setCurrentUserVariables(user);
    this.cxSurveyjsExtendedService.setCurrentDepartmentVariables(
      user.departmentId
    );
  }

  private initFirebaseCloudMessageService(userId: string): Promise<unknown> {
    return this.notificationService
      .initGettingNotificationFlow(userId.toString())
      .then(
        () => {
          return new Promise((resolve) => {
            resolve();
          });
        },
        () => {
          return reject();
        }
      );
  }

  private actionForValidUser = (user: User) => {
    const hasValidAccessToken = this.authService.hasValidAccessToken();
    if (hasValidAccessToken && user) {
      this.hasValidUser = true;
      this.currentUser = user;
      this.initHeader();
      // this line to enable navigate in promise resolver (Navigation ID is not equal to the current navigation ID)
      this.router.routeReuseStrategy.shouldReuseRoute = () => {
        return false;
      };
      const returnUrl = localStorage.getItem(RouteConstant.RETURN_URL);
      // Remove return_url and return_url_path_map after use
      localStorage.removeItem(RouteConstant.RETURN_URL);
      localStorage.removeItem(RouteConstant.RETURN_URL_PATH);
      let url = returnUrl ? returnUrl : location.pathname + location.search;
      if (url.includes('/' + AppConstant.siteURL.login)) {
        url = '/';
      }
      this.router.navigateByUrl(url);
    } else {
      this.hasValidUser = false;
    }
    this.hasLoading = false;
  };

  private setLanguageDefault(): void {
    const languageCode = localStorage.getItem('language-code');
    const languageName = localStorage.getItem('language-name');
    moment.locale(languageCode);
    if (languageCode && languageName) {
      this.translateAdapterService.mergeLanguage(languageCode);
      this.translateAdapterService.setDefaultLanguage(languageCode);
      localStorage.setItem('language-code', languageCode);
      localStorage.setItem('language-name', languageName);
    } else {
      const fallbackLanguage = AppConstant.fallbackLanguage;
      const fallbackLanguageName = AppConstant.fallbackLanguageName;
      this.translateAdapterService.mergeLanguage(fallbackLanguage);
      this.translateAdapterService.setDefaultLanguage(fallbackLanguage);
      localStorage.setItem('language-code', fallbackLanguage);
      localStorage.setItem('language-name', fallbackLanguageName);
    }
  }

  private initHeader(): void {
    localStorage.setItem(
      'is-desktop',
      this.deviceService.isDesktop().toString()
    );
    this.translateAdapterService
      .getValueBasedOnKey('Header')
      .subscribe((headerLanguages) => {
        if (this.currentUser && this.currentUser.headerData) {
          if (!this.currentUser.avatarUrl) {
            this.currentUser.avatarUrl = AppConstant.defaultAvatar;
          }
          this.currentUser.headerData.title = headerLanguages.Title;
          this.currentUser.headerData.multipleLanguages = {
            notifications: headerLanguages.Notification,
            search: headerLanguages.Search
          };
        }
      });
  }

  private receiveMessageFromIframe(event: MessageEvent): void {
    const data = event.data.paramsAlert;

    if (!data) {
      return;
    }
    console.log(`Messsage getting: ${data}`);
    if (!data.showNotification || !data.height) {
      this.cssClass = 'empty';

      return;
    }
    if (data.height) {
      const notificationHeight = 40;
      this.heightIframe = data.height + notificationHeight;
    }
  }
}
