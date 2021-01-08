import { Location } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { MatTab, MatTabChangeEvent } from '@angular/material/tabs';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { environment } from 'app-environments/environment';
import {
  IPDPlanPermission,
  PDPlanPermission,
} from 'app-models/common/permission/pdplan-permission';
import { IdpService } from 'app-services/idp.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import {
  IdpDto,
  IdpFilterParams,
} from 'app/organisational-development/models/idp.model';
import { AppConstant, Constant } from 'app/shared/app.constant';
import { cloneDeep, isEmpty } from 'lodash';
import { BaseComponent } from './../shared/components/component.abstract';
import { User } from './../shared/models/auth.model';
import {
  EPortfolioPermission,
  IEPortfolioPermission,
} from './../shared/models/common/permission/eportfolio-permission';
import {
  ILearningNeedAnalysisPermission,
  LearningNeedAnalysisPermission,
} from './../shared/models/common/permission/learning-need-analysis-permission';
import {
  ILearningNeedPermission,
  LearningNeedPermission,
} from './../shared/models/common/permission/learning-need-permission';
import {
  IDPMode,
  IdpStatusCodeEnum,
  IdpStatusEnum,
  IDPTabsMenuEnum,
} from './idp.constant';
import { IndividualDevelopmentHelper } from './individual-development.helper';
import { SubmittedLNAEventData } from './models/pd-evaluation.model';
@Component({
  selector: 'individual-development',
  templateUrl: './individual-development.component.html',
  styleUrls: ['./individual-development.component.scss'],
  styles: [
    `
      :host {
        display: block;
      }
    `,
  ],
  encapsulation: ViewEncapsulation.None,
})
export class IndividualDevelopmentComponent
  extends BaseComponent
  implements
    OnInit,
    ILearningNeedAnalysisPermission,
    ILearningNeedPermission,
    IPDPlanPermission,
    IEPortfolioPermission {
  @Input() mode: IDPMode;
  @Input() user: any;
  public learningNeedsResults: IdpDto[];
  public learningNeedsAnalysis: IdpDto;

  public lnaPeriod: string;
  public serviceScheme: string;
  public avatarSource: string;
  matTabIndex: number;
  @ViewChild('learningNeedsAnalysisTab') learningNeedsAnalysisTab: MatTab;
  @ViewChild('learningNeedsTab') learningNeedsTab: MatTab;
  @ViewChild('pdPlanTab') pdPlanTab: MatTab;
  @ViewChild('ePortfolioTab') ePortfolioTab: MatTab;
  /**
   * Whether the LNA tab should be shown or hidden.
   * It should be shown at initialization but later will be hidden if it is not valid to be shown.
   */
  showLearningNeedsAnalysisTab: boolean = true;
  /**
   * Whether the content of the LNA tab has been loaded. Default is false until the tab is active.
   */
  showLearningNeedsAnalysisContentTab: boolean = false;
  /**
   * Whether the content of the Learning Needs tab has been loaded. Default is false until the tab is active.
   */
  showLearningNeedsContentTab: boolean = false;
  /**
   * Whether the content of the PD Plan tab has been loaded. Default is false until the tab is active.
   */
  showPdPlanContentTab: boolean = false;
  /**
   * Whether the content of the E-Portfolio tab has been loaded. Default is false until the tab is active.
   */
  showEPortfolioContentTab: boolean = false;

  IDPMode: any = IDPMode;

  ePortfolioUrl: SafeResourceUrl;

  loadedData: boolean = false;
  theNumberOfInvalidPermissiontab: number;
  learningNeedPermission: LearningNeedPermission;
  learningNeedAnalysisPermission: LearningNeedAnalysisPermission;
  pdPlanPermission: PDPlanPermission;
  ePortfolioPermission: EPortfolioPermission;
  private readonly STATUS_CODE_FOR_LNA: IdpStatusCodeEnum[] = [
    IdpStatusCodeEnum.NotAdded,
    IdpStatusCodeEnum.NotStarted,
    IdpStatusCodeEnum.Started,
    IdpStatusCodeEnum.Rejected,
  ];

  private readonly STATUS_CODE_LEARNING_NEED: IdpStatusCodeEnum[] = [
    IdpStatusCodeEnum.PendingForApproval,
    IdpStatusCodeEnum.Approved,
    IdpStatusCodeEnum.Rejected,
    IdpStatusCodeEnum.Completed,
  ];

  private isLoadedEportfolioIframe: boolean = false;

  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    private idpService: IdpService,
    private sanitizer: DomSanitizer,
    private translateAdapterService: TranslateAdapterService,
    private location: Location,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private globalLoader: CxGlobalLoaderService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    const currentUser = this.authService.userData().getValue();
    this.initLearningNeedAnalysisPermission(currentUser);
    this.initLearningNeedPermission(currentUser);
    this.initPDPlanPermission(currentUser);
    this.initEPortfolioPermission(currentUser);
    this.theNumberOfInvalidPermissiontab =
      (!this.learningNeedAnalysisPermission.allowView ? 1 : 0) +
      (!this.learningNeedPermission.allowView ? 1 : 0) +
      (!this.pdPlanPermission.allowView ? 1 : 0) +
      (!this.ePortfolioPermission.allowView ? 1 : 0);
    this.processUserData();
    this.getUserLearningData();
  }

  initLearningNeedAnalysisPermission(loginUser: User) {
    this.learningNeedAnalysisPermission = new LearningNeedAnalysisPermission(
      loginUser,
      this.mode
    );
  }

  initLearningNeedPermission(loginUser: User): void {
    this.learningNeedPermission = new LearningNeedPermission(
      loginUser,
      this.mode
    );
  }
  initEPortfolioPermission(loginUser: User): void {
    this.ePortfolioPermission = new EPortfolioPermission(loginUser, this.mode);
  }
  initPDPlanPermission(loginUser: User): void {
    this.pdPlanPermission = new PDPlanPermission(loginUser, this.mode);
  }

  selectedTabChange(matTabChangeEvent: MatTabChangeEvent): void {
    const matTabIndex = matTabChangeEvent.index;
    const tabId = this.tabList[matTabIndex];
    this.showTabById(tabId);
    this.updateRouteByTabId(tabId);
    if (
      tabId === IDPTabsMenuEnum.EPortfolio &&
      !this.isLoadedEportfolioIframe
    ) {
      this.globalLoader.showLoader();
      setTimeout(() => {
        this.globalLoader.hideLoader();
      }, Constant.REQUEST_TIME_OUT);
    }
  }

  onSubmittedLearningNeeds(eventData: SubmittedLNAEventData): void {
    this.handleSubmitLNA(eventData);
  }

  onNeedToReloadLNA(): void {
    this.resetData();
    this.updateRouteByTabId(IDPTabsMenuEnum.LearningNeedAnalysis);
    this.getUserLearningData();
  }

  onIframeLoad(): void {
    this.globalLoader.hideLoader();
    this.isLoadedEportfolioIframe = true;
  }

  onClickBack(): void {
    this.router.navigate(['/employee']);
  }

  get isLearnerMode(): boolean {
    return this.mode === IDPMode.Learner;
  }

  get isReportingOfficerMode(): boolean {
    return (
      this.mode === IDPMode.ReportingOfficer || this.mode === IDPMode.Normal
    );
  }

  get userAvatar(): string {
    return this.user?.avatarUrl || AppConstant.defaultAvatar;
  }

  private processUserData(): void {
    this.avatarSource = this.getUserImage(this.user);
    const userExtId = this.user.identity.extId;
    const ePortfolioUrlStr = `${environment.ePortfolioUrl}?id=${userExtId}`;
    this.ePortfolioUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
      ePortfolioUrlStr
    );

    if (this.user.personnelGroups && this.user.personnelGroups.length) {
      //TODO: Description is empty now. Need to return description of Service scheme. replace externalID to description
      const externalIdNotApplicable = '72a1df40-d592-11e9-9740-0242ac120004';
      this.serviceScheme =
        this.user.personnelGroups[0].identity.extId !== externalIdNotApplicable
          ? this.user.personnelGroups[0].description
          : this.translateAdapterService.getValueImmediately(
              'MyPdJourney.Message.NoServiceScheme'
            );
    }
  }

  private async getUserLearningData(): Promise<void> {
    await this.getLearningNeedsResult();
    this.handleDefaultTabByRouting();
    this.loadedData = true;
    this.changeDetectorRef.detectChanges();
  }

  private async getLearningNeedsResult(): Promise<void> {
    const filterParams = new IdpFilterParams({
      userIds: [this.user.identity.id],
    });

    const reponse = await this.idpService.getNeedsResultAsync(filterParams);
    if (!reponse || isEmpty(reponse.data)) {
      this.learningNeedsResults = [];
      this.showLearningNeedsAnalysisTab = false;

      return;
    }

    const results = reponse.data.sort((result1, result2) => {
      const date1 = +new Date(result1.surveyInfo.startDate);
      const date2 = +new Date(result2.surveyInfo.startDate);

      return date2 - date1;
    });

    switch (this.mode) {
      case this.IDPMode.Learner:
        this.processDataModeLearner(results);
        break;
      case this.IDPMode.Normal:
      case this.IDPMode.ReportingOfficer:
        this.learningNeedsResults = results;
        break;
      default:
        break;
    }

    this.showLearningNeedsAnalysisTab = !isEmpty(this.learningNeedsAnalysis);
  }

  private handleSubmitLNA(eventData: SubmittedLNAEventData): void {
    const learningNeeds = eventData.result;
    const clonedLearningNeeds = cloneDeep(learningNeeds);
    const resultIndex = this.learningNeedsResults.findIndex(
      (result) =>
        result.resultIdentity.extId === learningNeeds.resultIdentity.extId
    );
    if (resultIndex > -1) {
      this.learningNeedsResults[resultIndex] = clonedLearningNeeds;
    } else {
      this.learningNeedsResults.push(clonedLearningNeeds);
    }
    this.learningNeedsResults = [...this.learningNeedsResults];
    this.learningNeedsAnalysis = undefined;
    this.showLearningNeedsAnalysisTab = false;
    if (eventData.navigateToPDPlan) {
      this.navigateToPdplan();
    } else {
      this.showLearningNeedsContentTab = true;
      window.scroll(0, 0);
    }
  }

  private processDataModeLearner(results: IdpDto[]): void {
    const statusCodesForAO = this.STATUS_CODE_LEARNING_NEED.map((code) =>
      code.toString()
    );
    this.learningNeedsResults = results.filter((res) =>
      statusCodesForAO.includes(res.assessmentStatusInfo.assessmentStatusCode)
    );

    const statusCodesForLearner = this.STATUS_CODE_FOR_LNA.map((code) =>
      code.toString()
    );
    this.learningNeedsAnalysis = results.find((res) =>
      statusCodesForLearner.includes(
        res.assessmentStatusInfo.assessmentStatusCode
      )
    );
    if (this.learningNeedsAnalysis) {
      this.lnaPeriod = this.learningNeedsAnalysis.surveyInfo.displayName;
    }
  }

  private navigateToPdplan(): void {
    setTimeout(() => {
      const pdplanTabIndex = 1;
      this.matTabIndex = pdplanTabIndex;
    });
  }

  private handleDefaultTabByRouting(): void {
    const params = this.route.snapshot.paramMap;
    let tabId = params.get('target');

    tabId = this.getTabIdByPermission(tabId);
    if (!this.checkValidTabId(tabId)) {
      tabId = IDPTabsMenuEnum.LearningNeedAnalysis;
      this.updateRouteByTabId(tabId);
    }

    // Case need to show LNA tab but learner has empty LNA data, we'll show Learning need tab instead
    if (
      tabId === IDPTabsMenuEnum.LearningNeedAnalysis &&
      isEmpty(this.learningNeedsAnalysis)
    ) {
      tabId = IDPTabsMenuEnum.LearningNeed;
      this.updateRouteByTabId(tabId);
    }

    this.matTabIndex = IndividualDevelopmentHelper.getTabIndexByEnum(
      tabId,
      this.tabActiveEnumMapping
    );
    this.matTabIndex -= this.theNumberOfInvalidPermissiontab;
    this.showTabById(tabId);
  }

  private getTabIdByPermission(tabId: string) {
    if (!this.tabActiveEnumPermission[tabId]) {
      for (const tab of this.tabList) {
        if (this.tabActiveEnumPermission[tab] && tab !== tabId) {
          this.updateRouteByTabId(tab);
          return tab;
        }
      }
    }
    return tabId;
  }

  private showTabById(tabId: string): void {
    switch (tabId) {
      case IDPTabsMenuEnum.LearningNeedAnalysis:
        this.showLearningNeedsAnalysisContentTab = true;
        break;
      case IDPTabsMenuEnum.LearningNeed:
        this.showLearningNeedsContentTab = true;
        break;
      case IDPTabsMenuEnum.PDPlan:
        this.showPdPlanContentTab = true;
        break;
      case IDPTabsMenuEnum.EPortfolio:
        this.showEPortfolioContentTab = true;
        break;
      default:
        this.learningNeedsAnalysis
          ? (this.showLearningNeedsAnalysisContentTab = true)
          : (this.showLearningNeedsContentTab = true);
        break;
    }
  }

  private get tabActiveEnumMapping(): any {
    return {
      [IDPTabsMenuEnum.LearningNeedAnalysis]: this.showLearningNeedsAnalysisTab,
      [IDPTabsMenuEnum.LearningNeed]: true,
      [IDPTabsMenuEnum.PDPlan]: true,
      [IDPTabsMenuEnum.EPortfolio]: true,
    };
  }

  private get tabActiveEnumPermission(): any {
    return {
      [IDPTabsMenuEnum.LearningNeedAnalysis]: this
        .learningNeedAnalysisPermission.allowView,
      [IDPTabsMenuEnum.LearningNeed]: this.learningNeedPermission.allowView,
      [IDPTabsMenuEnum.PDPlan]: this.pdPlanPermission.allowView,
      [IDPTabsMenuEnum.EPortfolio]: this.ePortfolioPermission.allowView,
    };
  }

  private get tabList(): any[] {
    const tabs = [];
    if (
      this.learningNeedsAnalysis &&
      (this.learningNeedsAnalysis.assessmentStatusInfo.assessmentStatusId ===
        IdpStatusEnum.Rejected ||
        this.learningNeedsAnalysis.assessmentStatusInfo.assessmentStatusId ===
          IdpStatusEnum.NotStarted ||
        this.learningNeedsAnalysis.assessmentStatusInfo.assessmentStatusId ===
          IdpStatusEnum.Started) &&
      this.learningNeedAnalysisPermission.allowView
    ) {
      tabs.push(IDPTabsMenuEnum.LearningNeedAnalysis);
    }
    if (this.learningNeedPermission.allowView) {
      tabs.push(IDPTabsMenuEnum.LearningNeed);
    }
    if (this.pdPlanPermission.allowView) {
      tabs.push(IDPTabsMenuEnum.PDPlan);
    }
    if (this.ePortfolioPermission.allowView) {
      tabs.push(IDPTabsMenuEnum.EPortfolio);
    }
    return tabs;
  }

  private getTargetUrlByTabId(tabId: string): string {
    const params = this.route.snapshot.paramMap;
    const currentTabId = params.get('target');
    const currentUrl = this.router.url;

    return !!currentTabId
      ? currentUrl.replace(currentTabId, tabId)
      : `${currentUrl}/${tabId}`;
  }

  private updateRouteByTabId(tabId: string): void {
    const targetUrl = this.getTargetUrlByTabId(tabId);

    this.location.go(targetUrl);
  }

  private checkValidTabId(tabId: string): boolean {
    if (!tabId) {
      return false;
    }

    const isActiveValue = this.tabActiveEnumMapping[tabId];

    return isActiveValue === true || isActiveValue === false;
  }

  private resetData(): void {
    this.loadedData = false;
    this.showLearningNeedsAnalysisTab = true;
    this.showLearningNeedsAnalysisContentTab = false;
    this.showLearningNeedsContentTab = false;
    this.showPdPlanContentTab = false;
    this.showEPortfolioContentTab = false;
  }
}
