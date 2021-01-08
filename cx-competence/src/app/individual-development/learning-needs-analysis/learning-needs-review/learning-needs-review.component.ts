import { Component, Input, OnInit } from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { ChartInfo, IdpConfigParams } from 'app-models/pdplan.model';
import { IdpService } from 'app-services/idp.service';
import LearningAreaChartHelper from 'app/individual-development/shared/learning-area-chart/learning-area-chart.helper';
import { LearningAreaChartModel } from 'app/individual-development/shared/learning-area-chart/learning-area-chart.model';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import {
  ILearningNeedPermission,
  LearningNeedPermission,
} from './../../../shared/models/common/permission/learning-need-permission';
import { IDPMode } from './../../idp.constant';

const NUMBER_OF_REVIEWS = 3;

@Component({
  selector: 'learning-needs-review',
  templateUrl: './learning-needs-review.component.html',
  styleUrls: ['./learning-needs-review.component.scss'],
})
export class LearningNeedsReviewComponent
  implements OnInit, ILearningNeedPermission {
  @Input() needsResults: IdpDto[];
  @Input() mode: IDPMode;
  /**
   * Map resultId to period
   */
  periods: Map<number, string> = new Map([]);

  /**
   * Map resultId to report charts
   */
  learningNeedsReviews: Map<number, LearningAreaChartModel[]> = new Map([]);
  learningNeedPermission: LearningNeedPermission;

  constructor(
    private idpService: IdpService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private authService: AuthService
  ) {}

  initLearningNeedPermission(loginUser: User): void {
    this.learningNeedPermission = new LearningNeedPermission(
      loginUser,
      this.mode
    );
  }

  ngOnInit(): void {
    if (!this.needsResults) {
      return;
    }
    this.initLearningNeedPermission(this.authService.userData().getValue());
    this.needsResults
      .filter((result, index) => index < NUMBER_OF_REVIEWS)
      .forEach((result) => {
        this.periods.set(
          result.resultIdentity.id,
          result.surveyInfo.displayName
        );
      });
  }

  getLearningNeedsReportData(resultId: number): void {
    if (this.learningNeedsReviews.has(resultId)) {
      return;
    }
    const params = new IdpConfigParams({ resultId });
    this.cxGlobalLoaderService.showLoader();
    this.idpService.getLearningNeedReport(params).subscribe(
      (reports) => {
        this.cxGlobalLoaderService.hideLoader();
        if (!reports) {
          this.learningNeedsReviews.set(resultId, []);

          return;
        }
        const chartModels: LearningAreaChartModel[] = [];
        reports.forEach((report) => {
          const prioritisationChart: ChartInfo = LearningAreaChartHelper.reportDataToChartInfo(
            report
          );
          const chartModel = new LearningAreaChartModel({
            prioritisationChart,
          });
          chartModels.push(chartModel);
        });
        this.learningNeedsReviews.set(resultId, chartModels);
      },
      (error) => this.cxGlobalLoaderService.hideLoader()
    );
  }

  // this will disable ordering of keyvalue pipe
  returnZero(): number {
    return 0;
  }
}
