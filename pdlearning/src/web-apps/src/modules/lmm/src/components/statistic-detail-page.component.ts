import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Course, CourseRepository } from '@opal20/domain-api';
import { Observable, Subscription, of } from 'rxjs';

import { LMMTabConfiguration } from '@opal20/domain-components';

@Component({
  selector: 'statistic-detail-page',
  templateUrl: './statistic-detail-page.component.html'
})
export class StatisticDetailPageComponent extends BasePageComponent {
  @Input() public stickyDependElement: HTMLElement;

  public get courseId(): string {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string) {
    this._courseId = v;
  }

  public get course(): Course | undefined {
    return this._course;
  }

  public set course(v: Course | undefined) {
    if (Utils.isDifferent(this._course, v)) {
      this._course = v;
    }
  }

  public loadingCourseVmData: boolean = false;
  public hiddenPostCourseTab: boolean = false;
  public hiddenPreCourseTab: boolean = false;
  public activeTab: string = LMMTabConfiguration.PreCourseStatisticTab;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public _courseId: string;

  private _course: Course | null;
  private _loadCourseInfoSub: Subscription = new Subscription();

  constructor(public moduleFacadeService: ModuleFacadeService, private courseRepository: CourseRepository) {
    super(moduleFacadeService);
  }

  public loadCourse(): void {
    this._loadCourseInfoSub.unsubscribe();
    const courseObs: Observable<Course | null> = this.courseId != null ? this.courseRepository.loadCourse(this.courseId) : of(null);
    this._loadCourseInfoSub = courseObs.pipe(this.untilDestroy()).subscribe(course => {
      this.course = course;
      this.activeTab = course.preCourseEvaluationFormId
        ? LMMTabConfiguration.PreCourseStatisticTab
        : LMMTabConfiguration.PostCourseStatisticTab;
      this.hiddenTab();
    });
  }

  protected onInit(): void {
    this.loadCourse();
  }

  private hiddenTab(): void {
    if (this.course) {
      if (!this.course.preCourseEvaluationFormId) {
        this.hiddenPreCourseTab = true;
      }
      if (!this.course.postCourseEvaluationFormId) {
        this.hiddenPostCourseTab = true;
      }
    }
  }
}
