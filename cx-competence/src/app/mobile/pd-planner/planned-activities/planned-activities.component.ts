import { Component, OnInit, Input } from '@angular/core';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { AppConstant } from 'app/shared/app.constant';
import { MobileAuthService } from 'app/mobile/services/mobile-auth.service';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { LearningNeedService } from 'app-services/idp/learning-need/learning-needs.service';

@Component({
  selector: 'app-planned-activities',
  templateUrl: './planned-activities.component.html',
  styleUrls: ['./planned-activities.component.scss'],
})
export class PlannedActivitiesComponent implements OnInit {
  @Input() needsResults: IdpDto[];
  // Main data variables
  user: Staff;
  learningNeedsResults: IdpDto[];

  // Static data
  backRoute: string = AppConstant.mobileUrl.pdPlanner;
  pageNameTranslatePath: string = 'Mobile.PDPlan.PageHeader.PlannedActivities';

  constructor(
    private authService: MobileAuthService,
    private learningNeedService: LearningNeedService
  ) {}

  ngOnInit(): void {
    this.initData();
  }

  // Private functions
  private async initData(): Promise<void> {
    this.user = await this.authService.getCurrentStaffProfile();
    const userId = await this.authService.getCurrentUserIdAsync();
    this.learningNeedsResults = await this.learningNeedService.getLearningNeedsSubmittedAsync(
      userId
    );
  }

  // TODO: goBackAfterSubmitSuccess(): void {
  //   $('#sunmit-plan-success').on('hide.bs.modal', () => {
  //     this.router.navigate([AppConstant.mobileUrl.pdPlanner]);
  //   });
  // }
}
