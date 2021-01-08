import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  Course,
  CourseRepository,
  DepartmentInfoModel,
  MetadataTagModel,
  OrganizationRepository,
  PlaceOfWorkType,
  PublicUserInfo,
  UserApiService,
  organizationUnitLevelConst
} from '@opal20/domain-api';

@Component({
  selector: 'learner-course-more-details',
  templateUrl: './learner-course-more-details.component.html'
})
export class LearnerCourseMoreDetailsComponent extends BaseComponent {
  public prerequisiteCoursesDic: Dictionary<Course> = {};
  public departmentsDic: Dictionary<DepartmentInfoModel> = {};
  public PlaceOfWorkType: typeof PlaceOfWorkType = PlaceOfWorkType;
  public usersDic: Dictionary<PublicUserInfo>;
  @Input()
  public courseMetadata: Dictionary<MetadataTagModel> = {};
  @Input()
  public fullCourseDetail: Course;
  @Input()
  public showLinkCommunity: boolean;
  @Input()
  public communityUrl: string;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private courseRepository: CourseRepository,
    private organizationRepository: OrganizationRepository,
    private userApiService: UserApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.courseRepository
      .loadCourses(this.fullCourseDetail.prerequisiteCourseIds)
      .pipe(this.untilDestroy())
      .subscribe(response => {
        this.prerequisiteCoursesDic = Utils.toDictionary(response, p => p.id);
      });

    this.organizationRepository
      .loadDepartmentInfoList({
        departmentId: 1,
        includeChildren: true,
        includeDepartmentType: true,
        departmentTypeIds: [
          organizationUnitLevelConst.Division,
          organizationUnitLevelConst.Branch,
          organizationUnitLevelConst.Cluster,
          organizationUnitLevelConst.School
        ]
      })
      .pipe(this.untilDestroy())
      .subscribe(respone => {
        this.departmentsDic = Utils.toDictionary(respone, p => p.id);
      });

    const administratorIds: string[] = [];
    if (this.fullCourseDetail) {
      if (this.fullCourseDetail.firstAdministratorId) {
        administratorIds.push(this.fullCourseDetail.firstAdministratorId);
      }
      if (this.fullCourseDetail.secondAdministratorId) {
        administratorIds.push(this.fullCourseDetail.secondAdministratorId);
      }
    }

    if (administratorIds.length > 0) {
      this.getPublicUserInfo(administratorIds);
    }
  }

  public openCslWindow(): void {
    window.open(this.communityUrl, '_blank');
  }

  private getPublicUserInfo(userIds: string[]): void {
    this.userApiService
      .getPublicUserInfoList({ userIds: userIds })
      .pipe(this.untilDestroy())
      .subscribe(publicUserInfoList => {
        if (publicUserInfoList && publicUserInfoList.length > 0) {
          this.usersDic = Utils.toDictionary(publicUserInfoList, p => p.id);
        }
      });
  }
}
