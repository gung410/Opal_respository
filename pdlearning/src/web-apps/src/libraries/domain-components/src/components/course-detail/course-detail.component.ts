import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';

import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { CourseTabInfo } from '../../models/course-tab.model';
import { FormGroup } from '@angular/forms';
import { ScrollableMenu } from '@opal20/common-components';

@Component({
  selector: 'course-detail',
  templateUrl: './course-detail.component.html'
})
export class CourseDetailComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public mode: CourseDetailMode;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;

  public get course(): CourseDetailViewModel {
    return this._course;
  }
  @Input()
  public set course(v: CourseDetailViewModel) {
    if (Utils.isDifferent(this._course, v) && v != null) {
      this._course = v;
    }
  }

  @ViewChild('basicInfoTabContainer', { static: false })
  public basicInfoTabContainer: ElementRef;

  @ViewChild('providerInfoTabContainer', { static: false })
  public providerInfoTabContainer: ElementRef;

  @ViewChild('metadataTabContainer', { static: false })
  public metadataTabContainer: ElementRef;

  @ViewChild('copyRightTabContainer', { static: false })
  public copyRightTabContainer: ElementRef;

  @ViewChild('targetaudienceTabContainer', { static: false })
  public targetaudienceTabContainer: ElementRef;

  @ViewChild('coursePlanningTabContainer', { static: false })
  public coursePlanningTabContainer: ElementRef;

  @ViewChild('evaluationEcertificateTabContainer', { static: false })
  public evaluationEcertificateTabContainer: ElementRef;

  @ViewChild('courseAdministrationTabContainer', { static: false })
  public courseAdministrationTabContainer: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: CourseTabInfo.OverviewInfo,
      title: 'Overview',
      elementFn: () => {
        return this.basicInfoTabContainer;
      }
    },
    {
      id: CourseTabInfo.ProviderInfo,
      title: 'Provider',
      elementFn: () => {
        return this.providerInfoTabContainer;
      }
    },
    {
      id: CourseTabInfo.MetaData,
      title: 'Metadata ',
      elementFn: () => {
        return this.metadataTabContainer;
      }
    },
    {
      id: CourseTabInfo.MetaDataCopyRight,
      title: 'Copyrights',
      isHidden: () => {
        return this.isPlanningVerificationRequired();
      },
      elementFn: () => {
        return this.copyRightTabContainer;
      }
    },
    {
      id: CourseTabInfo.TargetAudience,
      title: 'Target Audience',
      elementFn: () => {
        return this.targetaudienceTabContainer;
      }
    },
    {
      id: CourseTabInfo.CoursePlanning,
      title: 'Course Planning',
      elementFn: () => {
        return this.coursePlanningTabContainer;
      }
    },
    {
      id: CourseTabInfo.EvaluationEcertificate,
      title: 'Evaluation & E-Certificate',
      isHidden: () => {
        return this.course.isPlanningVerificationRequired || this.course.isMicrolearning;
      },
      elementFn: () => {
        return this.evaluationEcertificateTabContainer;
      }
    },
    {
      id: CourseTabInfo.CourseAdministration,
      title: 'Course Administration',
      elementFn: () => {
        return this.courseAdministrationTabContainer;
      }
    }
  ];
  public activeTab: string = CourseTabInfo.OverviewInfo;
  public loadingData: boolean = false;
  private _course: CourseDetailViewModel;

  public static asViewMode(mode: CourseDetailMode): boolean {
    return (
      mode === CourseDetailMode.View ||
      mode === CourseDetailMode.ForApprover ||
      mode === CourseDetailMode.ForVerifier ||
      mode === CourseDetailMode.EditContent
    );
  }

  public static asViewModeForCompletingCourseForPlanning(course: CourseDetailViewModel): boolean {
    return course.courseData.isCompletingCourseForPlanning();
  }

  public static isPlanningVerificationRequired(course: CourseDetailViewModel): boolean {
    return course.courseData.isPlanningVerificationRequired();
  }

  public static canViewFieldOfCourseInPlanningCycle(course: CourseDetailViewModel): boolean {
    return course.courseData.canViewFieldOfCourseInPlanningCycle();
  }

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public isPlanningVerificationRequired(): boolean {
    return CourseDetailComponent.isPlanningVerificationRequired(this.course);
  }
}
