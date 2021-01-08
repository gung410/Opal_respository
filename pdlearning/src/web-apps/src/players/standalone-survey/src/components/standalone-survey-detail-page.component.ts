import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { StandaloneSurveyRoutePaths } from '@opal20/domain-components';

@Component({
  selector: 'standalone-survey-detail-page',
  templateUrl: 'standalone-survey-detail-page.component.html'
})
export class StandaloneSurveyDetailPageComponent extends BasePageComponent {
  public communityId: string;
  public formId: string;
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
    this.formId = this.navigationData.formId;
  }

  public backToRepository(): void {
    this.navigateTo(StandaloneSurveyRoutePaths.RepositoryPage, {
      communityId: this.communityId
    });
    const routePath = `standalone-survey/community/${this.communityId}/${StandaloneSurveyRoutePaths.RepositoryPage}`;
    this.updateDeeplink(routePath);
  }
}
