import { Location } from '@angular/common';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
} from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { NavigationEnd, Router } from '@angular/router';
import {
  CxCommonService,
  CxFooterData,
  CxGlobalLoaderService,
  CxInformationDialogService,
  MultipleLanguages,
  NotificationList,
  OAuthService,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserIdleService } from 'angular-user-idle';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { UserService } from 'app-services/user.service';
import { environment } from 'environments/environment';
import { DeviceDetectorService } from 'ngx-device-detector';
import { filter } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { pageSlideAnimation } from './app-sliding.animation';
import { AppService } from './app.service';
import { AppConstant, VariableConstant } from './shared/app.constant';
import { BaseScreenComponent } from './shared/components/component.abstract';
import { StaffListService } from './staff/staff.container/staff-list.service';

// tslint:disable-next-line:max-line-length
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [AppService],
  animations: [pageSlideAnimation],
})
export class AppComponent
  extends BaseScreenComponent
  implements OnInit, AfterViewInit {
  googleAnalyticId: string;
  roles: string[];
  multipleLanguages: MultipleLanguages;
  hasValidUser: boolean = false;
  showHeader: boolean = false;
  showBreadcrumb: boolean = true;
  currentBreadcrumb: any[] = [];
  termsOfUseUrl: string;
  privacyUrl: string;
  vulnerabilityUrl: string;
  notifications: NotificationList;
  isShowNotificationBell: boolean = environment.notification.enableShowBellIcon;
  enableBroadCast: boolean = environment.notification.enableBroadCast;
  footerData: CxFooterData;
  onClickStaffDevelopmentTab: boolean;

  public cssClass: string;
  public heightIframe: string = 'auto';
  public notificationAlertUrl: SafeResourceUrl;
  private isSessionTimeoutWarning: boolean = false;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private deviceService: DeviceDetectorService,
    private translateAdapterService: TranslateAdapterService,
    private router: Router,
    protected authService: AuthService,
    private userService: UserService,
    private authAdapterService: OAuthService,
    private userIdle: UserIdleService,
    private breadcrumbSettingService: BreadcrumbSettingService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private globalModal: NgbModal,
    private informationDialogService: CxInformationDialogService,
    private cxCommonService: CxCommonService,
    private globalLoader: CxGlobalLoaderService,
    private domSanitizer: DomSanitizer,
    private staffListService: StaffListService,
    private windowLocation: Location
  ) {
    super(changeDetectorRef, authService);
    this.router.events
      .pipe(filter((e) => e instanceof NavigationEnd))
      .subscribe(this.handleRouterNavigateEndHandler);

    this.breadcrumbSettingService.changeBreadcrumbEvent.subscribe(
      this.breadCrumEventHandler
    );
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    this.receiveMessageFromIframe(event);
  }

  onClickBreadcrumbItem(route: any): void {
    if (route && this.currentUser) {
      this.router.navigate([route]);
      const itemBreadcrumbs = this.breadcrumbSettingService.mapRouteToBreadcrumb(
        this.currentUser.headerData.menus,
        route
      );
      this.addItemToBreadcrumb(itemBreadcrumbs);
    }
  }

  async ngOnInit(): Promise<void> {
    super.ngOnInit();
    await this.setLanguageDefault();
    // Prevent site config main flow if it in MPJ Iframe
    if (this.isMPJLearnerModule()) {
      return;
    }
    this.initData();
  }

  ngAfterViewInit(): void {
    this.notificationAlertUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(
      environment.notification.alertUrl
    );
  }

  onSupport(): void {
    //navigate to place to support
  }

  navigateToProfile(): void {
    // we have different profile url in uat and prod environment
    this.authService.navigateToExternalSite(
      environment.baseProfileAppUrl && environment.baseProfileAppUrl !== ''
        ? environment.baseProfileAppUrl
        : environment.issuer,
      AppConstant.externalNavigationPath.profile
    );
  }

  onSearch(searchTerm: string): void {
    if (location.pathname.includes('/detail')) {
      this.staffListService.searchingBehaviorSubject.next(searchTerm);
      this.router.navigate(['/employee']);

      return;
    }
    this.staffListService.searchValueSubject.next(searchTerm);
  }

  onClickedSettings(): void {}

  onLogout(): void {
    this.authService.logout();
  }

  onChangePassword(): void {
    this.authService.navigateToExternalSite(
      environment.issuer,
      AppConstant.externalNavigationPath.changePassword
    );
  }

  private addItemToBreadcrumb(items: any): void {
    this.currentBreadcrumb = items;
    this.changeDetectorRef.detectChanges();
  }

  private getUserInfo(): void {
    this.globalLoader.showLoader();
    this.authService.userData().subscribe((user: User) => {
      if (user) {
        this.currentUser = user;
        this.cxSurveyjsExtendedService.initCxSurveyVariable(user);
        this.initHeader();
        this.globalLoader.hideLoader();
        this.hasValidUser = true;
        this.setupUserIdleDetection(); // detect user idle if token valid
      } else {
        this.authService
          .loadDiscoveryDocumentAndTryLogin()
          .then(this.actionForValidUser, (_) => {
            this.globalLoader.hideLoader();
          });
      }
    });
  }

  private setupUserIdleDetection(): void {
    this.authService.userData().subscribe((user: User) => {
      if (user) {
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
          // close all modals are currently displaying in session timeout page
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
            sessionStorage.clear();
            localStorage.clear();
            Swal.close();
            this.router.navigate(['/' + AppConstant.siteURL.sessionTimeout]);
          }
          sessionStorage.setItem(
            AppConstant.sessionVariable.sessionTimeout,
            'true'
          );
        });
      }
    });
  }

  private actionForValidUser = (user: User) => {
    const hasValidAccessToken = this.authService.hasValidAccessToken();
    if (hasValidAccessToken && user) {
      this.hasValidUser = true;
      this.currentUser = user;
      this.initHeader();
      this.initFooter();
      // this line to enable navigate in promise resolver
      // (Navigation ID is not equal to the current navigation ID)
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
      const claims = this.authAdapterService.getIdentityClaims();
      const roles = (claims as any).role;
      this.roles = roles;
      const returnUrl = localStorage.getItem(VariableConstant.RETURN_URL);
      // Remove return_url and return_url_path_map after use
      localStorage.removeItem(VariableConstant.RETURN_URL);
      localStorage.removeItem(VariableConstant.RETURN_URL_PATH_MAP);
      let url = returnUrl ? returnUrl : location.pathname + location.search;
      if (url.includes('/' + AppConstant.siteURL.login)) {
        url = '/';
      }
      this.router.navigateByUrl(url);
    } else {
      this.hasValidUser = false;
    }
    this.globalLoader.hideLoader();
  };

  private async setLanguageDefault(): Promise<void> {
    const languageCode = localStorage.getItem('language-code');
    const languageName = localStorage.getItem('language-name');
    if (languageCode && languageName) {
      await this.translateAdapterService.mergeLanguage(languageCode);
      this.translateAdapterService.setDefaultLanguage(languageCode);
      localStorage.setItem('language-code', languageCode);
      localStorage.setItem('language-name', languageName);
    } else {
      const fallbackLanguage = environment.fallbackLanguage;
      const fallbackLanguageName = environment.fallbackLanguageName;
      await this.translateAdapterService.mergeLanguage(fallbackLanguage);
      this.translateAdapterService.setDefaultLanguage(fallbackLanguage);
      localStorage.setItem('language-code', fallbackLanguage);
      localStorage.setItem('language-name', fallbackLanguageName);
    }
  }

  private initData(): void {
    this.authService.isAuthenticating.next(true);
    this.getUserInfo();
    this.authService.configureWithNewConfigApi();
    this.initHeader();
    this.getTranslateText();
  }

  private getTranslateText(): void {
    this.translateAdapterService
      .getValueBasedOnKey('Common.Mandatory_Text')
      .subscribe(
        (translatedText: string) =>
          (this.cxCommonService.textMandatoryNeedToShow = translatedText)
      );
  }

  private initHeader(): void {
    localStorage.setItem(
      'is-desktop',
      this.deviceService.isDesktop().toString()
    );
    this.translateAdapterService
      .getValueBasedOnKey('Common.Header')
      .subscribe((headerLanguages) => {
        if (this.currentUser && this.currentUser.headerData) {
          this.currentUser.headerData.title = headerLanguages.Title;
        }
        if (this.currentUser && !this.currentUser.avatarUrl) {
          this.currentUser.avatarUrl = AppConstant.defaultAvatar;
        }
        this.multipleLanguages = {
          notifications: headerLanguages.Notification,
          search: 'Search',
        };
      });
  }

  private initFooter(): void {
    const releaseDate =
      (this.currentUser && this.currentUser.releaseDate) ||
      environment.site.dateRelease;
    this.footerData = this.userService.getFooterData(releaseDate);
  }

  private isMPJLearnerModule(): boolean {
    return location.href.includes('/' + AppConstant.mobileUrl.pdPlanner);
  }

  private handleRouterNavigateEndHandler = (data: NavigationEnd): void => {
    if (
      data.urlAfterRedirects.includes('/error') ||
      data.urlAfterRedirects.includes('/login') ||
      data.urlAfterRedirects.includes('/' + AppConstant.mobileUrl.pdPlanner) ||
      data.urlAfterRedirects.includes('/' + AppConstant.siteURL.sessionTimeout)
    ) {
      this.showHeader = false;
    } else {
      this.showHeader = true;
    }
    this.showBreadcrumb = data.urlAfterRedirects.includes('/odp/plan-detail')
      ? false
      : true;
  };

  private breadCrumEventHandler = (result: any): void => {
    if (result && this.currentUser) {
      const itemBreadcrumbs = this.breadcrumbSettingService.mapRouteToBreadcrumb(
        this.currentUser.headerData.menus,
        result.route,
        result.param
      );
      this.addItemToBreadcrumb(itemBreadcrumbs);
    }
  };

  /**
   * Determines whether the include sub organization checkbox is ticked.
   */
  private isIncludedSubOrganizations(): boolean {
    const includeSubOrgCxCheckbox: HTMLElement = document.querySelector(
      'cx-checkbox.checkbox-sub-organisation'
    );
    if (includeSubOrgCxCheckbox) {
      const includeSubOrgCheckbox = includeSubOrgCxCheckbox.querySelector(
        'input'
      );

      return includeSubOrgCheckbox.checked;
    }

    return false;
  }

  private receiveMessageFromIframe(event: MessageEvent): void {
    const data = event.data.paramsAlert;
    if (!data) {
      return;
    }

    if (!data.showNotification || !data.height) {
      this.cssClass = 'empty';
      return;
    }

    if (data.height) {
      this.heightIframe = data.height + 40;
    }
  }
}
