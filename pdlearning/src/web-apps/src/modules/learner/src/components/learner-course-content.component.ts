import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CourseContentItemModel, CourseContentItemType } from '@opal20/domain-api';

@Component({
  selector: 'learner-course-content',
  templateUrl: './learner-course-content.component.html'
})
export class LearnerCourseContentComponent extends BaseComponent {
  public contentType: typeof CourseContentItemType = CourseContentItemType;
  public lessContents: CourseContentItemModel[] = [];
  public displayContents: CourseContentItemModel[] = [];
  public showAll: boolean = false;

  @Input()
  public set tableOfContents(value: CourseContentItemModel[]) {
    this._tableOfContents = value;
    this.calculateLessContents();
    this.displayContents = this.lessContents;
  }
  @Input()
  public completedLectureIds: string[] = [];
  @Input()
  public highlightLectureId: string | undefined;
  @Input()
  public showCursorPointerLectureIds: string[] = [];
  @Input()
  public disableLearning: boolean = false;

  @Input()
  public set showMore(value: boolean) {
    if (this.showMore === value) {
      return;
    }

    this._showMore = value;
    this.showMoreChange.emit(this.showMore);
    if (this.showMore) {
      this.showAllContents();
    } else {
      this.displayContents = this.lessContents;
    }
  }

  @Output()
  public showMoreChange = new EventEmitter<boolean>();
  @Output()
  public fullContents: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output()
  public lectureClick: EventEmitter<string> = new EventEmitter<string>();

  private _tableOfContents: CourseContentItemModel[] = [];
  public get tableOfContents(): CourseContentItemModel[] {
    return this._tableOfContents;
  }

  private _showMore: boolean = false;
  public get showMore(): boolean {
    return this._showMore;
  }

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public showAllContents(): void {
    this.displayContents = this.tableOfContents;
  }

  public onLectureClick(lectureId: string): void {
    if (!this.canClickOnLecture(lectureId)) {
      return;
    }
    this.lectureClick.emit(lectureId);
  }

  private calculateLessContents(): void {
    this.lessContents = [];
    const maxDisplayLectures = 10;
    let lecturesCount: number = 0;
    for (let i = 0; i < this.tableOfContents.length; i++) {
      if (lecturesCount >= maxDisplayLectures) {
        break;
      }
      this.lessContents.push(this.tableOfContents[i]);
      lecturesCount +=
        this.tableOfContents[i].type === CourseContentItemType.Section && this.tableOfContents[i].items
          ? this.tableOfContents[i].items.length
          : 1;
    }
    this.fullContents.emit(this.lessContents.length === this.tableOfContents.length);
  }

  private canClickOnLecture(lectureId: string): boolean {
    return this.highlightLectureId === lectureId || this.completedLectureIds.indexOf(lectureId) !== -1;
  }
}
