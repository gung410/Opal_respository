import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { StandaloneSurveyRoutePaths } from '@opal20/domain-components';

@Component({
  selector: 'standalone-survey-player-page',
  templateUrl: 'standalone-survey-player-page.component.html'
})
export class StandaloneSurveyPlayerPageComponent extends BasePageComponent {
  public communityId: string;
  private navigationData: { communityId: string; formId?: string };
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.navigationData = this.getNavigateData();
    if (this.navigationData == null) {
      return;
    }
    this.communityId = this.navigationData.communityId;
  }

  public backToLearning(): void {
    this.navigateTo(StandaloneSurveyRoutePaths.LearningPage, {
      communityId: this.communityId
    });
    const routePath = `standalone-survey/community/${this.communityId}/${StandaloneSurveyRoutePaths.LearningPage}`;
    this.updateDeeplink(routePath);
  }
}
