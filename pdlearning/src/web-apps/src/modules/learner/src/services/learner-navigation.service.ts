import { Inject, Injectable } from '@angular/core';
import { LearnerRoutePaths, NavigationMenuService } from '@opal20/domain-components';

import { APP_BASE_HREF } from '@angular/common';
import { ModuleFacadeService } from '@opal20/infrastructure';

@Injectable()
export class LearnerNavigationService {
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private navigationMenuService: NavigationMenuService,
    @Inject(APP_BASE_HREF) private baseHref: string
  ) {}

  public toCourseDetail(courseId: string, caller: LearnerRoutePaths | undefined): void {
    const parameters = {
      courseId: courseId,
      caller: caller
    };
    this.moduleFacadeService.navigationService.navigateTo(LearnerRoutePaths.MyLearning, parameters);
    this.navigationMenuService.activate(caller !== undefined ? caller : LearnerRoutePaths.MyLearning);
  }

  public navigateTo<T>(path: LearnerRoutePaths, parameters?: T): void {
    this.moduleFacadeService.navigationService.navigateTo(path, parameters !== undefined ? parameters : {});
    this.navigationMenuService.activate(path);
  }
}
