import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { StandaloneSurveyRoutePaths } from '@opal20/domain-components';

@Component({
  selector: 'standalone-survey-learning-page',
  templateUrl: 'standalone-survey-learning-page.component.html'
})
export class StandaloneSurveyLearningPageComponent extends BasePageComponent {
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

  public onDetailClicked(): void {
    const formId = 'this-is-form-id';
    this.navigateTo(StandaloneSurveyRoutePaths.PlayerPage, {
      communityId: this.communityId,
      formId: formId
    });
    const routePath = `standalone-survey/community/${this.communityId}/${StandaloneSurveyRoutePaths.PlayerPage}/${formId}`;
    this.updateDeeplink(routePath);
  }
}
