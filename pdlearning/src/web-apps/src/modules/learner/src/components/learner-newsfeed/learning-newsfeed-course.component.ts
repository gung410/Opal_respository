import { BaseComponent, LocalTranslatorService, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CourseFeedModel, NewsFeedUpdateInfo } from '@opal20/domain-api';

@Component({
  selector: 'learning-newsfeed-course',
  templateUrl: './learning-newsfeed-course.component.html'
})
export class LearningNewsFeedCourseComponent extends BaseComponent {
  @Input()
  public newsfeed: CourseFeedModel;

  @Output()
  public newsFeedClick: EventEmitter<CourseFeedModel> = new EventEmitter<CourseFeedModel>();

  constructor(public translator: LocalTranslatorService, protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onNewsFeedClick(): void {
    this.newsFeedClick.emit(this.newsfeed);
  }

  public get updateInfo(): string {
    return NEWSFEED_UPDATEINFO.get(this.newsfeed.updateInfo);
  }
}

export const NEWSFEED_UPDATEINFO: Map<NewsFeedUpdateInfo, string> = new Map<NewsFeedUpdateInfo, string>([
  [NewsFeedUpdateInfo.Info, 'The information of the course has been updated'],
  [NewsFeedUpdateInfo.Content, 'The content of the course has been updated'],
  [NewsFeedUpdateInfo.Suggested, 'The information of the course has been updated']
]);
