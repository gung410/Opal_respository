import { Component, OnInit } from '@angular/core';
import { LearningNeedService } from 'app-services/idp/learning-need/learning-needs.service';
import { MobileAuthService } from 'app/mobile/services/mobile-auth.service';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { AppConstant } from 'app/shared/app.constant';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';

@Component({
  selector: 'app-learning-need-analysis-mobile',
  templateUrl: './learning-need-analysis-mobile.component.html',
  styleUrls: ['./learning-need-analysis-mobile.component.scss'],
})
export class LearningNeedAnalysisMobileComponent implements OnInit {
  // Main data variables
  lnaResult: IdpDto;
  lnaResults: IdpDto[];
  user: Staff;

  // Secondary data
  isSubmitted: boolean = false;

  // Static data
  backRoute: string = AppConstant.mobileUrl.pdPlanner;
  pageNameTranslatePath: string =
    'Mobile.PDPlan.PageHeader.LearningNeedsAnalysis';

  constructor(
    private learningNeedService: LearningNeedService,
    private mobileAuthService: MobileAuthService,
    private globalLoader: CxGlobalLoaderService
  ) {}

  async ngOnInit(): Promise<void> {
    this.globalLoader.showLoader();
    await this.getData();
    this.globalLoader.hideLoader();
  }

  onSubmittedLearningNeeds(): void {
    this.isSubmitted = true;
  }

  private async getData(): Promise<void> {
    this.user = await this.mobileAuthService.getCurrentStaffProfile();
    const userId = await this.mobileAuthService.getCurrentUserIdAsync();
    this.lnaResult = await this.learningNeedService.getLearningNeedUnsubmittedAsync(
      userId
    );
    this.lnaResults = await this.learningNeedService.getLearningNeedsSubmittedAsync(
      userId
    );
  }
}
