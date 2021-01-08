import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CxFooterData,
  CxGlobalLoaderService,
} from '@conexus/cx-angular-common';
import { ResizedEvent } from 'angular-resize-event';
import { environment } from 'app-environments/environment';
import { LearningNeedsDataModel } from 'app-models/mpj/idp.model';
import { LearningNeedService } from 'app-services/idp/learning-need/learning-needs.service';
import { UserService } from 'app-services/user.service';
import { IDPMode } from 'app/individual-development/idp.constant';
import { AppConstant } from 'app/shared/app.constant';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { isEmpty } from 'lodash';
import { Subscription } from 'rxjs';
import { MobileAuthService } from '../services/mobile-auth.service';

@Component({
  selector: 'pd-planner',
  templateUrl: './pd-planner.component.html',
  styleUrls: ['./pd-planner.component.scss'],
})
export class PDPlannerComponent implements OnInit, OnDestroy {
  showLNA: boolean = false;
  lnaPeriod: string = '';
  learningNeedsData: LearningNeedsDataModel;
  tokenReadySub: Subscription;
  isMobile: boolean;

  // MPJ web
  user: Staff;
  mode: string = IDPMode.Learner;
  footerData: CxFooterData;
  loading: boolean = true;

  private routerEventSubscription: Subscription;
  private readonly minWebWidth: number = 768; // px

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private learningNeedService: LearningNeedService,
    private mobileAuthService: MobileAuthService,
    private userService: UserService,
    private globalLoader: CxGlobalLoaderService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.globalLoader.showLoader();
  }

  ngOnInit(): void {
    if (this.mobileAuthService.hasValidToken) {
      this.onTokenReady();
    }
    // tslint:disable-next-line:no-string-literal
    if (typeof window['onExternalMessage'] !== 'function') {
      this.mobileAuthService.initForCommunicateService();
    }
    this.tokenReadySub = this.mobileAuthService.onAccessTokenInitReady.subscribe(
      this.onTokenReady
    );
    this.isMobile = document.body.offsetWidth < this.minWebWidth;
  }

  ngOnDestroy(): void {
    if (this.tokenReadySub) {
      this.tokenReadySub.unsubscribe();
    }
    if (this.routerEventSubscription) {
      this.routerEventSubscription.unsubscribe();
    }
  }

  onTokenReady = (): void => {
    this.getLearningNeedsInfo();
  };

  onClickedLearningNeedAnalysis(): void {
    this.navigateTo(AppConstant.mobileUrl.learningNeedAnalysis);
  }

  onClickedLearningNeeds(): void {
    this.navigateTo(AppConstant.mobileUrl.learningNeed);
  }

  onClickedPDPlan(): void {
    this.navigateTo(AppConstant.mobileUrl.plannedActivities);
  }

  onResized(event: ResizedEvent): void {
    this.mobileAuthService.sendMessageViewSizeChange(
      event.newWidth,
      event.newHeight
    );
  }

  private navigateTo(route: string): void {
    if (route) {
      this.router.navigate([route], { relativeTo: this.route });
    }
  }

  private async getLearningNeedsInfo(): Promise<void> {
    this.globalLoader.showLoader();
    this.loading = true;
    if (this.isMobile) {
      const userId = await this.mobileAuthService.getCurrentUserIdAsync();
      const learningNeedUnsubmitted = await this.learningNeedService.getLearningNeedUnsubmittedAsync(
        userId
      );
      if (!isEmpty(learningNeedUnsubmitted)) {
        this.showLNA = true;
        this.lnaPeriod = learningNeedUnsubmitted.surveyInfo
          ? learningNeedUnsubmitted.surveyInfo.displayName
          : '';
      }
    } else {
      this.user = await this.mobileAuthService.getCurrentStaffProfile();
      const releaseDate =
        (this.mobileAuthService.siteData &&
          this.mobileAuthService.siteData.releaseDate) ||
        environment.site.dateRelease;
      this.footerData = this.userService.getFooterData(releaseDate);
    }
    this.loading = false;
    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  }
}
