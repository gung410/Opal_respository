import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { AppConstant } from 'app/shared/app.constant';
import { LearningNeedService } from 'app-services/idp/learning-need/learning-needs.service';
import { MobileAuthService } from 'app/mobile/services/mobile-auth.service';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
@Component({
  selector: 'app-learning-need-mobile',
  templateUrl: './learning-need-mobile.component.html',
  styleUrls: ['./learning-need-mobile.component.scss'],
})
export class LearningNeedMobileComponent implements OnInit {
  user: Staff;
  learningNeedList: IdpDto[];
  backRoute: string = AppConstant.mobileUrl.pdPlanner;
  pageNameTranslatePath: string = 'Mobile.PDPlan.PageHeader.LearningNeeds';

  constructor(
    private mobileAuthService: MobileAuthService,
    private learningNeedService: LearningNeedService,
    private globalLoader: CxGlobalLoaderService
  ) {}

  ngOnInit(): void {
    this.getData();
  }

  private async getData(): Promise<void> {
    this.globalLoader.showLoader();
    this.user = await this.mobileAuthService.getCurrentStaffProfile();
    const userId = await this.mobileAuthService.getCurrentUserIdAsync();
    this.learningNeedList = await this.learningNeedService.getLearningNeedsSubmittedAsync(
      userId
    );
    this.globalLoader.hideLoader();
  }
}
