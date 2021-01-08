import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { ChartInfo, IdpConfigParams } from 'app-models/pdplan.model';
import { IdpService } from 'app-services/idp.service';
import { IDPMode } from 'app/individual-development/idp.constant';
import LearningAreaChartHelper from 'app/individual-development/shared/learning-area-chart/learning-area-chart.helper';
import { LearningAreaChartModel } from 'app/individual-development/shared/learning-area-chart/learning-area-chart.model';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import {
  ILearningNeedAnalysisPermission,
  LearningNeedAnalysisPermission,
} from './../../../shared/models/common/permission/learning-need-analysis-permission';

@Component({
  selector: 'learning-needs-preview',
  templateUrl: './learning-needs-preview.component.html',
  styleUrls: ['./learning-needs-preview.component.scss'],
})
export class LearningNeedsPreviewComponent
  implements OnInit, OnChanges, ILearningNeedAnalysisPermission {
  @Input() learningNeeds: IdpDto;
  @Input() mode: IDPMode;
  learningAreaCharts: LearningAreaChartModel[];
  learningNeedAnalysisPermission: LearningNeedAnalysisPermission;

  constructor(
    private idpService: IdpService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private authService: AuthService
  ) {}

  initLearningNeedAnalysisPermission(loginUser: User): void {
    this.learningNeedAnalysisPermission = new LearningNeedAnalysisPermission(
      loginUser,
      this.mode
    );
  }
  ngOnInit(): void {
    this.initLearningNeedAnalysisPermission(
      this.authService.userData().getValue()
    );
  }
  ngOnChanges(): void {}

  getLearningNeedsReportData(): void {
    if (!this.learningNeeds) {
      return;
    }
    const params = new IdpConfigParams({
      resultId: this.learningNeeds.resultIdentity.id,
    });
    this.cxGlobalLoaderService.showLoader();
    this.idpService.getLearningNeedReport(params).subscribe(
      (reports) => {
        if (!reports) {
          this.learningAreaCharts = [];

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
        this.learningAreaCharts = chartModels;
      },
      (error) => this.cxGlobalLoaderService.hideLoader(),
      () => this.cxGlobalLoaderService.hideLoader()
    );
    // because the chart is still not refreshed on server
  }
}
