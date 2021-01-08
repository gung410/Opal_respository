import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { BaseUserInfo, MetadataId, SystemRoleEnum, UserInfoModel, UserRepository, UserUtils } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'course-administration-tab',
  templateUrl: './course-administration-tab.component.html'
})
export class CourseAdministrationTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  public get course(): CourseDetailViewModel {
    return this._course;
  }
  @Input()
  public set course(v: CourseDetailViewModel) {
    if (Utils.isDifferent(this._course, v) && v != null) {
      this._course = v;
      this.fetchCourseApprovingOfficerItemsFn = this._createFetchUserSelectItemFn(
        CourseDetailViewModel.courseApprovingOfficerItemsRoles(this.course.departmentsDic),
        p => p.filter(x => x.id !== this.course.alternativeApprovingOfficerId && x.id !== this.course.currentUser.id)
      );
      this.fetchAlternativeCourseApprovingOfficerItemsFn = this._createFetchUserSelectItemFn(
        CourseDetailViewModel.courseApprovingOfficerItemsRoles(this.course.departmentsDic),
        p => p.filter(x => x.id !== this.course.primaryApprovingOfficerId && x.id !== this.course.currentUser.id)
      );
    }
  }
  @Input() public mode: CourseDetailMode | undefined;
  public fetchCourseAdministratorItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchAlternativeCourseAdministratorItemsFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<BaseUserInfo[]>;
  public fetchCourseApprovingOfficerItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchAlternativeCourseApprovingOfficerItemsFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<BaseUserInfo[]>;
  public fetchCourseFacilitatorItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchCourseCoFacilitatorItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchCollaborativeContentCreatorItemsFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<BaseUserInfo[]>;
  public fetchUserItemsByIdsFn: (ids: string[]) => Observable<BaseUserInfo[]>;
  public ignoreCourseAdministratorItemFn: (item: BaseUserInfo) => boolean;
  public ignoreAlternativeCourseAdministratorItemFn: (item: BaseUserInfo) => boolean;
  public ignoreCourseApprovingOfficerItemFn: (item: BaseUserInfo) => boolean;
  public ignoreAlternativeCourseApprovingOfficerItemFn: (item: BaseUserInfo) => boolean;
  public ignoreCourseFacilitatorItemFn: (item: BaseUserInfo) => boolean;
  public ignoreCourseCoFacilitatorItemFn: (item: BaseUserInfo) => boolean;
  public MetadataId: typeof MetadataId = MetadataId;
  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  private currentUser = UserInfoModel.getMyUserInfo();
  private _course: CourseDetailViewModel;
  constructor(public moduleFacadeService: ModuleFacadeService, private userRepository: UserRepository) {
    super(moduleFacadeService);

    this.fetchCourseAdministratorItemsFn = this._createFetchUserSelectItemFn(CourseDetailViewModel.courseAdministratorItemsPermissions);
    this.fetchAlternativeCourseAdministratorItemsFn = this._createFetchUserSelectItemFn(
      CourseDetailViewModel.courseAdministratorItemsPermissions
    );
    this.fetchCourseFacilitatorItemsFn = this._createFetchUserSelectItemFn(CourseDetailViewModel.courseFacilitatorItemsPermissions);
    this.fetchCourseCoFacilitatorItemsFn = this._createFetchUserSelectItemFn(CourseDetailViewModel.courseFacilitatorItemsPermissions);

    this.fetchCollaborativeContentCreatorItemsFn = this._createFetchUserSelectItemFn(
      CourseDetailViewModel.collaborativeContentCreatorItemPermissions
    );
    this.fetchUserItemsByIdsFn = this._createFetchUserItemsByIdsFn();
    this.ignoreCourseAdministratorItemFn = x =>
      x.id === this.course.firstAdministratorId ||
      (x.id === this.course.currentUser.id && !this.course.currentUser.hasRole(SystemRoleEnum.CourseAdministrator));
    this.ignoreAlternativeCourseAdministratorItemFn = x =>
      x.id === this.course.secondAdministratorId ||
      (x.id === this.course.currentUser.id && !this.course.currentUser.hasRole(SystemRoleEnum.CourseAdministrator));
    this.ignoreCourseApprovingOfficerItemFn = x => x.id === this.course.primaryApprovingOfficerId || x.id === this.course.currentUser.id;
    this.ignoreAlternativeCourseApprovingOfficerItemFn = x =>
      x.id === this.course.alternativeApprovingOfficerId || x.id === this.course.currentUser.id;
    this.ignoreCourseFacilitatorItemFn = x => x.id === this.course.courseFacilitatorId;
    this.ignoreCourseCoFacilitatorItemFn = x => x.id === this.course.courseCoFacilitatorId;
  }

  public asViewMode(): boolean {
    return (
      CourseDetailComponent.asViewMode(this.mode) ||
      (this.mode !== CourseDetailMode.NewCourse &&
        !this.course.originCourseData.hasApprovalPermission(this.currentUser) &&
        !this.course.originCourseData.hasOwnerPermission(this.currentUser))
    );
  }

  public asViewModeForCompletingCourseForPlanning(): boolean {
    return CourseDetailComponent.asViewModeForCompletingCourseForPlanning(this.course);
  }

  public isPlanningVerificationRequired(): boolean {
    return CourseDetailComponent.isPlanningVerificationRequired(this.course);
  }

  private _createFetchUserSelectItemFn(
    hasPermissions?: string[],
    mapFn?: (data: BaseUserInfo[]) => BaseUserInfo[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    const createFetchUsersFn = UserUtils.createFetchUsersByPermissionsFn(hasPermissions, this.userRepository, false);
    return (searchText: string, skipCount: number, maxResultCount: number) =>
      createFetchUsersFn(searchText, skipCount, maxResultCount).pipe(
        map(users => {
          if (mapFn) {
            return mapFn(users);
          }
          return users;
        })
      );
  }

  private _createFetchUserItemsByIdsFn(): (ids: string[]) => Observable<BaseUserInfo[]> {
    return UserUtils.createFetchUsersByIdsFn(this.userRepository, false, ['All']);
  }
}
