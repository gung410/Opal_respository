import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { ILearningItemModel, LearningType } from '../models/learning-item.model';

import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { LEARNER_PERMISSIONS } from '@opal20/domain-components';
import { LearnerLearningPathModel } from '../models/learning-path.model';
import { UserInfoModel } from '@opal20/domain-api';

@Component({
  selector: 'learning-carousel',
  templateUrl: './learning-carousel.component.html'
})
export class LearningCarouselComponent extends BaseComponent {
  @ViewChild('scrollElement', { static: false })
  public scrollElement: ElementRef;

  @Input()
  public title: string;
  @Input()
  public learningCardItems: ILearningItemModel[] = [];
  @Input()
  public emptyText: string | undefined;
  @Input()
  public learningCardItemsTotalCount: number = 0;

  @Output()
  public showAllClick: EventEmitter<void> = new EventEmitter<void>();
  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();

  public learningType: typeof LearningType = LearningType;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public trackBy(index: number, item: ILearningItemModel): string {
    return item.id;
  }

  public onShowAllClick(): void {
    this.showAllClick.emit();
  }

  public scrollLeft(): void {
    if (this.scrollElement === undefined || this.scrollElement.nativeElement === undefined) {
      return;
    }
    const nativeElement = <HTMLElement>this.scrollElement.nativeElement;
    this.smoothScroll(-340);
  }

  public scrollRight(): void {
    if (this.scrollElement === undefined || this.scrollElement.nativeElement === undefined) {
      return;
    }
    this.smoothScroll(340);
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    this.learningCardClick.emit(event);
  }

  public digitalContentCardClick(event: DigitalContentItemModel): void {
    this.learningCardClick.emit(event);
  }

  public learningPathCardClick(event: LearnerLearningPathModel): void {
    this.learningCardClick.emit(event);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  public get hasPermissionToBookmark(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_Bookmark);
  }

  private smoothScroll(totalScroll: number): void {
    const step = totalScroll / 10;
    let i: number = 0;
    const initalPosition = this.scrollElement.nativeElement.scrollLeft;
    const interval = setInterval(() => {
      this.scrollElement.nativeElement.scrollLeft = initalPosition + (i + 1) * step;
      i++;
      if (i * step === totalScroll) {
        clearInterval(interval);
      }
    }, 10);
  }
}
