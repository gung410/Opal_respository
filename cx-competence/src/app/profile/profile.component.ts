import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import {
  UserManagement,
  UserManagementQueryModel,
} from 'app-models/user-management.model';
import { UserService } from 'app-services/user.service';
import { AppConstant } from 'app/shared/app.constant';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { Survey } from 'survey-angular';
import {
  XCategoryType,
  XCategoryTypeQueryModel,
} from '../shared/models/xcategory.model';
import { ApprovalGroupTypeEnum } from './../shared/constants/approval-group.enum';
import { UserTypeService } from './../shared/services/user-type.service';
import { XCategoryService } from './../shared/services/xcategory.service';
import { ProfileFormJSON } from './form/profile-form';
@Component({
  selector: 'profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  profileFormJson: any = ProfileFormJSON;
  currentUser: any;
  profileFormData: any;
  userInfo: any;
  serviceSchemes: any[];
  careerPaths: any[];
  developmentalRoles: any[];
  profileInfoSurveyVariables: CxSurveyjsVariable[] = [];
  isLoading = true;

  constructor(
    private router: Router,
    private authService: AuthService,
    private userTypeService: UserTypeService,
    private userService: UserService,
    private xCategoryService: XCategoryService,
    private toastService: ToastrService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private translate: TranslateService
  ) {}

  ngOnInit() {
    this.cxGlobalLoaderService.showLoader();
    this.currentUser = this.authService.userData().getValue();
    this.initProfileForm();
  }

  initProfileForm() {
    const xCategoryTypeFilter = new XCategoryTypeQueryModel();
    xCategoryTypeFilter.xCategoryTypeExtIds = 'TeachingSubject';
    this.xCategoryService
      .getXCategoryTypes(xCategoryTypeFilter)
      .subscribe((teachingSubjects: XCategoryType[]) => {
        if (teachingSubjects.length > 0) {
          this.profileInfoSurveyVariables.push(
            new CxSurveyjsVariable({
              name: 'xCategoryTypeId',
              value: teachingSubjects[0].identity.id.toString(),
            })
          );
        }
      });
    this.getAndRenderUserInfo(this.currentUser.id);
    this.userTypeService.getPersonnelGroups().subscribe((serviceSchemes) => {
      this.serviceSchemes = serviceSchemes;
    });
    this.userTypeService.getCareerPaths().subscribe((careerPaths) => {
      this.careerPaths = careerPaths;
    });
    this.userTypeService
      .getDevelopmentalRole()
      .subscribe((developmentalRoles) => {
        this.developmentalRoles = developmentalRoles;
      });
  }

  getAndRenderUserInfo(userId: number) {
    const params = new UserManagementQueryModel();
    params.userIds = [userId];
    this.userService.getUsers(params).subscribe(
      (pagingUsers: any) => {
        if (pagingUsers.totalItems > 0) {
          this.userInfo = pagingUsers.items[0];
          this.profileFormData = this.convertUserDtoToProfileFormData(
            this.userInfo
          );
          this.cxGlobalLoaderService.hideLoader();
          this.isLoading = false;
        }
      },
      (err) => {
        this.cxGlobalLoaderService.hideLoader();
        this.isLoading = false;
      }
    );
  }

  convertUserDtoToProfileFormData(userInfo: UserManagement) {
    const approvingOfficer = userInfo.groups
      ? userInfo.groups.find(
          (g) => g.type === ApprovalGroupTypeEnum.PrimaryApprovalGroup
        )
      : null;
    const alternateApprovingOfficer = userInfo.groups
      ? userInfo.groups.find(
          (g) => g.type === ApprovalGroupTypeEnum.AlternativeApprovalGroup
        )
      : null;

    return {
      firstName: userInfo.firstName,
      emailAddress: userInfo.emailAddress,
      ssn: userInfo.ssn,
      departmentName: userInfo.departmentName,
      departmentAddress: userInfo.departmentAddress,
      dateOfBirth: moment(userInfo.dateOfBirth).format(
        AppConstant.backendDateFormat
      ),
      serviceScheme:
        userInfo.personnelGroups && userInfo.personnelGroups.length > 0
          ? userInfo.personnelGroups[0].identity.id
          : null,
      careerPath:
        userInfo.careerPaths && userInfo.careerPaths.length > 0
          ? userInfo.careerPaths.map((c) => c.identity.id)
          : [],
      developmentalRole:
        userInfo.developmentalRoles && userInfo.developmentalRoles.length > 0
          ? userInfo.developmentalRoles[0].identity.id
          : null,
      gender: userInfo.gender,
      created: moment(userInfo.created).format(AppConstant.backendDateFormat),
      expirationDate: moment(userInfo.entityStatus.expirationDate).format(
        AppConstant.backendDateFormat
      ),
      approvingOfficer: approvingOfficer ? approvingOfficer.name : '',
      alternateApprovingOfficer: alternateApprovingOfficer
        ? alternateApprovingOfficer.name
        : '',
      teachingSubjects: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.teachingSubjects
        : [],
      notificationPreference: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.notificationPreference
        : [],
      designation: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.designation
        : '',
      titleSalutation: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.titleSalutation
        : '',
      zone: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.zone
        : '',
      dateJoinedMinistry: userInfo.jsonDynamicAttributes
        ? moment(userInfo.jsonDynamicAttributes.dateJoinedMinistry).format(
            AppConstant.backendDateFormat
          )
        : '',
      teachingLevel: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.teachingLevel
        : [],
      teachingCourse: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.teachingCourse
        : [],
      learningFramework: userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.learningFramework
        : [],
    };
  }

  profileInfoChanged(basicChangedInfoSurvey) {
    const survey = basicChangedInfoSurvey.survey as Survey;
  }

  onCancel() {
    this.router.navigate(['']);
  }

  onSubmit(profileData: any) {
    this.cxGlobalLoaderService.showLoader();
    const userDto = this.updateExistingUserProfileDto(
      profileData,
      this.userInfo
    );
    this.userService.editUser(userDto).subscribe(
      (employee: UserManagement) => {
        this.cxGlobalLoaderService.hideLoader();
        this.toastService.success(
          `User ${profileData.firstName} updated successfully`
        );
        this.toastService.info(this.translate.instant('Profile.Logout'));
        this.profileFormData = this.convertUserDtoToProfileFormData(userDto);
        this.reloadProfileForm();
      },
      (error) => {
        this.cxGlobalLoaderService.hideLoader();
        this.toastService.error(error.error.Message, 'Error');
        this.reloadProfileForm();
      }
    );
  }

  updateExistingUserProfileDto(
    newUserProfile: any,
    oldUserProfile: UserManagement
  ) {
    const personnelGroups = this.serviceSchemes
      ? [
          this.serviceSchemes.find(
            (s) => s.identity.id === newUserProfile.serviceScheme
          ),
        ].filter((x) => x != null)
      : [];
    const developmentalRoles = this.developmentalRoles
      ? [
          this.developmentalRoles.find(
            (s) => s.identity.id === newUserProfile.developmentalRole
          ),
        ].filter((x) => x != null)
      : [];
    const careerPaths = newUserProfile.careerPath
      ? this.careerPaths.filter((s) =>
          newUserProfile.careerPath.includes(s.identity.id)
        )
      : [];
    return new UserManagement(
      Object.assign({}, oldUserProfile, {
        firstName: newUserProfile.firstName,
        ssn: newUserProfile.ssn,
        gender: newUserProfile.gender,
        dateOfBirth: moment(
          newUserProfile.dateOfBirth,
          AppConstant.backendDateFormat
        )
          .toDate()
          .toISOString(),
        emailAddress: newUserProfile.emailAddress,
        personnelGroups,
        careerPaths,
        developmentalRoles,
        jsonDynamicAttributes: Object.assign(
          {},
          oldUserProfile.jsonDynamicAttributes
            ? oldUserProfile.jsonDynamicAttributes
            : {},
          {
            notificationPreference: newUserProfile.notificationPreference
              ? newUserProfile.notificationPreference
              : [],
            teachingSubjects: newUserProfile.teachingSubjects
              ? newUserProfile.teachingSubjects
              : [],
            designation: newUserProfile.designation
              ? newUserProfile.designation
              : '',
            titleSalutation: newUserProfile.titleSalutation
              ? newUserProfile.titleSalutation
              : '',
            zone: newUserProfile.zone ? newUserProfile.zone : '',
            dateJoinedMinistry: newUserProfile.dateJoinedMinistry
              ? moment(
                  newUserProfile.dateJoinedMinistry,
                  AppConstant.backendDateFormat
                ).format(AppConstant.backendDateFormat)
              : '',
            teachingLevel: newUserProfile.teachingLevel
              ? newUserProfile.teachingLevel
              : [],
            teachingCourse: newUserProfile.teachingCourse
              ? newUserProfile.teachingCourse
              : [],
            learningFramework: newUserProfile.learningFramework
              ? newUserProfile.learningFramework
              : [],
          }
        ),
      })
    );
  }

  reloadProfileForm() {
    this.profileFormJson = { ...ProfileFormJSON };
  }
}
