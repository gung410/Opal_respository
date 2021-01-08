import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ILearningItemModel, LearningItemModel } from '../models/learning-item.model';
import { LearnerRoutePaths, MyLearningTab } from '@opal20/domain-components';

import { LearnerMyLearningComponent } from './learner-my-learning.component';
import { LearnerNavigationService } from '../services/learner-navigation.service';

@Component({
  selector: 'learner-my-learning-page',
  templateUrl: './learner-my-learning-page.component.html'
})
export class LearnerMyLearningPageComponent extends BasePageComponent implements OnInit {
  public defaultTab: MyLearningTab | undefined = MyLearningTab.Courses;
  public currentLearningItem: ILearningItemModel | undefined;

  @ViewChild('myLearning', { static: false })
  private myLearningComponent: LearnerMyLearningComponent;

  constructor(protected moduleFacadeService: ModuleFacadeService, private learnerNavigationService: LearnerNavigationService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    const routingParameters: {
      activeTab: MyLearningTab;
      pathId: string;
    } = this.getNavigateData() || {
      activeTab: undefined,
      pathId: undefined
    };

    if (routingParameters.activeTab) {
      this.defaultTab = routingParameters.activeTab;
    } else {
      this.defaultTab = MyLearningTab.Courses;
    }

    let deeplink = `learner/${LearnerRoutePaths.MyLearning}/${this.defaultTab}`;

    if (routingParameters.pathId) {
      deeplink += `/${routingParameters.pathId}`;
    }

    this.updateDeeplink(deeplink);
  }

  public onCourseDetailBackClick(): void {
    this.currentLearningItem = undefined;
    if (this.myLearningComponent !== undefined) {
      this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${this.myLearningComponent.activeTab}`);
      this.myLearningComponent.reloadData();
    }
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    this.currentLearningItem = event;
    if (event instanceof LearningItemModel) {
      this.onMicrolearningClick(event);
      return;
    }
    this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${event.type.toLocaleLowerCase()}/${event.id}`);
  }

  private onMicrolearningClick(event: LearningItemModel): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${event.type.toLocaleLowerCase()}/${event.id}`);
    if (event.isExpiredCourse) {
      return;
    }
  }
}
