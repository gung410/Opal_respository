import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { StandaloneSurveyRoutePaths } from '@opal20/domain-components';

@Component({
  selector: 'standalone-survey-repository-page',
  templateUrl: 'standalone-survey-repository-page.component.html'
})
export class StandaloneSurveyRepositoryPageComponent extends BasePageComponent {
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
    this.navigateTo(StandaloneSurveyRoutePaths.DetailPage, {
      communityId: this.communityId,
      formId
    });
    const routePath = `standalone-survey/community/${this.communityId}/${StandaloneSurveyRoutePaths.DetailPage}/${formId}`;
    this.updateDeeplink(routePath);
  }
}
