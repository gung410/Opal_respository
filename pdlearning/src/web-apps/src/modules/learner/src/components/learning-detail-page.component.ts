import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { LearnerRoutePaths, NavigationMenuService, QuizPlayerIntegrationsService } from '@opal20/domain-components';

import { FormAnswerModel } from '@opal20/domain-api';
import { LearningType } from '../models/learning-item.model';

@Component({
  selector: 'learning-detail-page',
  templateUrl: './learning-detail-page.component.html'
})
export class LearningDetailPage extends BasePageComponent {
  @Input() public learningId: string;
  @Input() public learningType: LearningType;
  @Input()
  public canContinueTask: boolean = false;
  @Input()
  public canStartTask: boolean = false;
  @Output() public back: EventEmitter<void> = new EventEmitter<void>();
  public LearningType: typeof LearningType = LearningType;
  public navigateData: { courseId: string; courseType: string };
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private navigationMenuService: NavigationMenuService,
    private quizPlayerService: QuizPlayerIntegrationsService
  ) {
    super(moduleFacadeService);
  }
  public onBack(): void {
    if (this.navigateData && this.navigateData.courseId) {
      this.moduleFacadeService.navigationService.navigateTo(LearnerRoutePaths.Catalogue);
    } else {
      this.back.emit();
    }
    this.clearNavigateData();
  }

  public onInit(): void {
    // case use deeplink detail
    if (this.learningId == null && this.learningType == null) {
      this.navigationMenuService.activate(LearnerRoutePaths.Catalogue);
    }

    if (this.learningId && this.learningType === LearningType.StandaloneForm) {
      this.openStandaloneFormPlayer();
    }
    // Support get course detail by url
    this.navigateData = this.getNavigateData();
    if (this.navigateData && this.navigateData.courseId) {
      this.learningId = this.navigateData.courseId;
      if (this.navigateData.courseType === LearningType.Course.toLowerCase()) {
        this.learningType = LearningType.Course;
      } else if (this.navigateData.courseType === LearningType.DigitalContent.toLowerCase()) {
        this.learningType = LearningType.DigitalContent;
      }

      this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${this.learningType.toLowerCase()}/${this.learningId}`);
    }
  }

  public onDestroy(): void {
    // this.clearNavigateData();
  }

  private openStandaloneFormPlayer(): void {
    this.quizPlayerService.setup({
      onQuizFinished: (formAnswerModel: FormAnswerModel) => {
        return;
      }
    });

    this.quizPlayerService.setFormOriginalObjectId(this.learningId);
  }
}
