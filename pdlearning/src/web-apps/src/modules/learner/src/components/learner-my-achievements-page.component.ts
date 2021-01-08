import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Output } from '@angular/core';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';

import { LearnerRoutePaths } from '@opal20/domain-components';
import { MY_ACHIEVEMENT_TYPE_ENUM } from '../constants/my-achievement.constant';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'learner-my-achievements-page',
  templateUrl: './learner-my-achievements-page.component.html'
})
export class LearnerMyAchievementsPageComponent extends BasePageComponent {
  @Output() public onDigitalBadgesBackClick: EventEmitter<void> = new EventEmitter<void>();

  public showingECertificates: boolean = false;
  public showingDigitalBadges: boolean = false;
  public navigateData: { myAchievementsType: MY_ACHIEVEMENT_TYPE_ENUM; displayText: string };

  constructor(protected moduleFacadeService: ModuleFacadeService, private subNavigateRouter: Router) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.updateDeeplinkWithParameterInNavigation();
    this.subNavigateRouter.events
      .pipe(
        filter((event: RouterEvent) => event instanceof NavigationEnd),
        this.untilDestroy()
      )
      .subscribe(() => {
        this.updateDeeplinkWithParameterInNavigation();
      });
  }

  public loadData(): void {
    this.navigateData = this.getNavigateData();
  }

  public onECertificatesBack(): void {
    if (this.showingECertificates) {
      this.showingECertificates = false;
      this.updateDeeplink(`learner/${LearnerRoutePaths.MyAchievements}`);
    }
  }

  public onShowMoreECertificatesClicked(): void {
    this.showingECertificates = true;
    this.updateDeeplink(`learner/${LearnerRoutePaths.MyAchievements}/${MY_ACHIEVEMENT_TYPE_ENUM.ECertificates}`);
  }

  public onDigitalBadgesBack(): void {
    if (this.showingDigitalBadges) {
      this.showingDigitalBadges = false;
      this.updateDeeplink(`learner/${LearnerRoutePaths.MyAchievements}`);
    }
  }

  public onShowMoreDigitalBadgesClicked(): void {
    this.showingDigitalBadges = true;
    this.updateDeeplink(`learner/${LearnerRoutePaths.MyAchievements}/${MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges}`);
  }

  private updateDeeplinkWithParameterInNavigation(): void {
    let deepLink = `learner/${LearnerRoutePaths.MyAchievements}`;

    this.showingECertificates = false;
    this.showingDigitalBadges = false;

    this.navigateData = this.getNavigateData();
    if (this.navigateData && this.navigateData.myAchievementsType) {
      switch (this.navigateData.myAchievementsType) {
        case MY_ACHIEVEMENT_TYPE_ENUM.ECertificates:
          this.showingECertificates = true;
          deepLink += `/${MY_ACHIEVEMENT_TYPE_ENUM.ECertificates}`;
          break;
        case MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges:
          this.showingDigitalBadges = true;
          deepLink += `/${MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges}`;
          break;
        default:
          break;
      }
    }
    if (deepLink) {
      this.updateDeeplink(deepLink);
    }
  }
}
