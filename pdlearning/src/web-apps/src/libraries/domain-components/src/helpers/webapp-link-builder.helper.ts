import { ClassRun, Course } from '@opal20/domain-api';

import { CourseCriteriaDetailMode } from '../models/course-criteria-detail-mode.model';
import { LMMRoutePaths } from '../module-constants/lmm/route-path';
import { LearnerRoutePaths } from '../module-constants/learner/learner-route-paths';
import { MyLearningTab } from '../module-constants/learner/my-learning-tab';
import { RouterPageInputExt } from '../models/router-info.model';

export class WebAppLinkBuilder {
  public static CAM_MODULE: string = 'cam';
  public static LEARNER_MODULE: string = 'learner';
  public static LMM_MODULE: string = 'lmm';
  public static PDPM_MODULE: string = 'pdplanner';
  public static CSL_MODULE: string = 'csl';

  public static buildDigitalContentUrl(formId: string): string {
    return AppGlobal.environment.appUrl + '/ccpm/content/' + formId;
  }

  public static buildFormUrl(formId: string): string {
    return AppGlobal.environment.appUrl + '/ccpm/form/' + formId;
  }

  public static buildLnaFormUrl(formId: string): string {
    return AppGlobal.environment.appUrl + '/ccpm/lnaform/' + formId;
  }

  public static buildStandaloneFormUrl(formId: string, submodule: 'lnaform' | 'form' = 'form'): string {
    return submodule === 'form'
      ? `${AppGlobal.environment.appUrl}/form-standalone/form/${formId}`
      : `${AppGlobal.environment.appUrl}/form-standalone/standalonesurvey/lna/${formId}`;
  }

  public static buildClassRunDetailLinkForLMMModule(
    courseManagementPageActiveTab: string,
    courseDetailActiveTab: string,
    courseDetailSubActiveTab: string,
    courseDetailMode: string,
    activeTab: string,
    mode: string,
    courseId: string,
    id: string
  ): string {
    const routeDataLevel1 = RouterPageInputExt.buildRouterPageInput(
      LMMRoutePaths.CourseManagementPage,
      LMMRoutePaths.CourseManagementPage,
      {
        activeTab: courseManagementPageActiveTab
      }
    );
    const routeDataLevel2 = RouterPageInputExt.buildRouterPageInput(
      LMMRoutePaths.CourseDetailPage,
      routeDataLevel1.path,
      {
        activeTab: courseDetailActiveTab,
        subActiveTab: courseDetailSubActiveTab,
        data: {
          mode: courseDetailMode,
          id: courseId,
          courseCriteriaMode: CourseCriteriaDetailMode.View
        }
      },
      routeDataLevel1
    );
    const routeDataLevel3 = RouterPageInputExt.buildRouterPageInput(
      LMMRoutePaths.ClassRunDetailPage,
      routeDataLevel1.path,
      {
        activeTab: activeTab,
        data: {
          mode: mode,
          id: id,
          course: courseId
        }
      },
      routeDataLevel2
    );
    return `${AppGlobal.environment.appUrl}/${WebAppLinkBuilder.LMM_MODULE}${RouterPageInputExt.pageInputToNavigatePath(routeDataLevel3)}`;
  }

  public static buildCourseDetailLinkForLMMModule(
    courseManagementPageActiveTab: string,
    activeTab: string,
    subActiveTab: string,
    mode: string,
    id: string
  ): string {
    const routeDataLevel1 = RouterPageInputExt.buildRouterPageInput(
      LMMRoutePaths.CourseManagementPage,
      LMMRoutePaths.CourseManagementPage,
      {
        activeTab: courseManagementPageActiveTab
      }
    );
    const routeDataLevel2 = RouterPageInputExt.buildRouterPageInput(
      LMMRoutePaths.CourseDetailPage,
      routeDataLevel1.path,
      {
        activeTab: activeTab,
        subActiveTab: subActiveTab,
        data: {
          mode: mode,
          id: id
        }
      },
      routeDataLevel1
    );

    return `${AppGlobal.environment.appUrl}/${WebAppLinkBuilder.LMM_MODULE}${RouterPageInputExt.pageInputToNavigatePath(routeDataLevel2)}`;
  }

  public static buildLearningPathDetailUrl(learningPathId: string, fromLmm: boolean): string {
    const learningPathDetailUrl = `${AppGlobal.environment.appUrl}/${WebAppLinkBuilder.LEARNER_MODULE}/${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}/${learningPathId}`;

    return learningPathDetailUrl + (fromLmm ? '/fromlmm' : '');
  }

  public static buildCSLCommunityDetailForCourseUrl(course: Course): string {
    if (course.isPublishedOnce()) {
      return `${window.location.origin}/${WebAppLinkBuilder.CSL_MODULE}/s/${course.courseCode}`;
    }
    return `${window.location.origin}/${WebAppLinkBuilder.CSL_MODULE}/s/course-${course.id}`;
  }

  public static buildCSLCommunityDetailForClassUrl(classRun: ClassRun): string {
    if (classRun.isPublishedOnce()) {
      return `${window.location.origin}/${WebAppLinkBuilder.CSL_MODULE}/s/${classRun.classRunCode}`;
    }
    return `${window.location.origin}/${WebAppLinkBuilder.CSL_MODULE}/s/class-${classRun.id}`;
  }
}
