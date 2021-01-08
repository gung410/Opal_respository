import {
  AssignmentRepository,
  AttendanceRatioOfPresentInfo,
  AttendanceTrackingRepository,
  Course,
  CourseRepository,
  DepartmentLevelModel,
  Designation,
  LearningCatalogRepository,
  NoOfAssignmentDoneInfo,
  OrganizationRepository,
  PublicUserInfo,
  RegistrationRepository,
  SearchRegistrationResult,
  SearchRegistrationsType,
  TaggingRepository,
  TypeOfOrganization,
  UserRepository
} from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { RegistrationViewModel } from '../models/registration-view.model';

@Injectable()
export class ListRegistrationGridComponentService {
  constructor(
    private registrationRepository: RegistrationRepository,
    private userRepository: UserRepository,
    private assignmentRepository: AssignmentRepository,
    private attendanceTrackingRepository: AttendanceTrackingRepository,
    private learningRepository: LearningCatalogRepository,
    private taggingRepository: TaggingRepository,
    private organizationRepository: OrganizationRepository,
    private courseRepository: CourseRepository
  ) {}

  public loadRegistration(
    courseId: string,
    classRunId: string,
    searchType: SearchRegistrationsType = SearchRegistrationsType.ClassRunRegistration,
    searchText: string = '',
    applySearchTextForCourse: boolean = false,
    userFilter: IFilter = null,
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null,
    loadNoOfAssignmentDone?: boolean,
    loadAttendanceRatioOfPresent?: boolean,
    assignmentId?: string
  ): Observable<OpalGridDataResult<RegistrationViewModel>> {
    return this.processSearchRegistrations(
      this.registrationRepository.loadSearchRegistration(
        courseId,
        classRunId,
        assignmentId,
        searchType,
        searchText,
        applySearchTextForCourse,
        userFilter,
        filter,
        skipCount,
        maxResultCount
      ),
      classRunId,
      checkAll,
      selectedsFn,
      loadNoOfAssignmentDone,
      loadAttendanceRatioOfPresent
    );
  }

  private processSearchRegistrations(
    regisObs: Observable<SearchRegistrationResult>,
    classRunId: string,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null,
    loadNoOfAssignmentDone?: boolean,
    loadAttendanceRatioOfPresent?: boolean
  ): Observable<OpalGridDataResult<RegistrationViewModel>> {
    return regisObs.pipe(
      switchMap(searchRegistrationResult => {
        if (searchRegistrationResult.totalCount === 0) {
          return of(<OpalGridDataResult<RegistrationViewModel>>{
            data: [],
            total: 0
          });
        }

        return combineLatest(
          this.userRepository.loadPublicUserInfoList({ userIds: Utils.uniq(searchRegistrationResult.items.map(_ => _.userId)) }),
          <Observable<NoOfAssignmentDoneInfo[]>>(
            (loadNoOfAssignmentDone
              ? this.assignmentRepository.loadNoOfAssignmentDone(searchRegistrationResult.items.map(_ => _.id), classRunId)
              : of(null))
          ),
          <Observable<AttendanceRatioOfPresentInfo[]>>(
            (loadAttendanceRatioOfPresent
              ? this.attendanceTrackingRepository.loadAttendenceRatioOfPresents(searchRegistrationResult.items.map(_ => _.id), classRunId)
              : of(null))
          ),
          this.learningRepository.loadUserDesignationList(),
          this.learningRepository.loadUserTypeOfOrganizationList(),
          this.organizationRepository.loadOrganizationalLevels(),
          this.courseRepository.loadCourses(Utils.distinct(searchRegistrationResult.items.map(c => c.courseId)))
        ).pipe(
          switchMap(
            ([registers, noOfAssignmentDones, attendanceRatioOfPresents, designations, orgUnitTypes, orgLevels, courses]: [
              PublicUserInfo[],
              NoOfAssignmentDoneInfo[],
              AttendanceRatioOfPresentInfo[],
              Designation[],
              TypeOfOrganization[],
              DepartmentLevelModel[],
              Course[]
            ]) => {
              const registersAllMetadataIds = Utils.distinct(Utils.flatTwoDimensionsArray(registers.map(p => p.getAllMetadataIds())));
              const registersCriteriaAllMetadataIds = Utils.distinct(
                Utils.flatTwoDimensionsArray(
                  searchRegistrationResult.items
                    .filter(_ => _.courseCriteriaViolation != null && _.courseCriteriaViolation.isViolated)
                    .map(_ => _.courseCriteriaViolation.getAllRelatedMetadataIds())
                )
              );
              const learnerPreRequisiteCourseViolationIds = Utils.distinct(
                Utils.flatTwoDimensionsArray(
                  searchRegistrationResult.items
                    .filter(_ => _.courseCriteriaViolation != null && _.courseCriteriaViolation.isViolated)
                    .map(_ => _.courseCriteriaViolation.getAllViolationPreRequisiteCourseIds())
                )
              );
              const metadatasObs = Utils.isEmpty(registersAllMetadataIds.concat(registersCriteriaAllMetadataIds))
                ? of([])
                : this.taggingRepository.loadMetaDataTagsByIds(registersAllMetadataIds.concat(registersCriteriaAllMetadataIds));
              const coursesObs = Utils.isEmpty(learnerPreRequisiteCourseViolationIds)
                ? of([])
                : this.courseRepository.loadCourses(learnerPreRequisiteCourseViolationIds);
              const departmentObs = this.organizationRepository.loadDepartmentInfoList({
                departmentId: 1,
                includeChildren: true,
                includeDepartmentType: true
              });
              return combineLatest(metadatasObs, coursesObs, departmentObs).pipe(
                map(([metadatas, courseCriteria, departments]) => {
                  return {
                    registers,
                    noOfAssignmentDones,
                    attendanceRatioOfPresents,
                    designations,
                    orgUnitTypes,
                    orgLevels,
                    metadatas,
                    courseCriteria,
                    departments,
                    courses
                  };
                })
              );
            }
          ),
          map(data => {
            const noOfAssignmentDonesDic = Utils.toDictionary(data.noOfAssignmentDones, p => p.registrationId);
            const attendanceRatioOfPresentsDic = Utils.toDictionary(data.attendanceRatioOfPresents, p => p.registrationId);
            const registersDic = Utils.toDictionary(data.registers, p => p.id);
            const coursesDic = Utils.toDictionary(data.courses, p => p.id);

            return <OpalGridDataResult<RegistrationViewModel>>{
              data: searchRegistrationResult.items.map(registration =>
                RegistrationViewModel.createFromModel(
                  registration,
                  registersDic[registration.userId],
                  checkAll,
                  selectedsFn != null ? selectedsFn() : {},
                  Utils.toDictionary(data.metadatas, p => p.id),
                  Utils.toDictionary(data.designations, p => p.id),
                  Utils.toDictionary(data.courseCriteria, p => p.id),
                  Utils.toDictionary(data.orgUnitTypes, p => p.id),
                  Utils.toDictionary(data.orgLevels, p => p.id),
                  Utils.toDictionary(data.departments, p => p.id),
                  noOfAssignmentDonesDic[registration.id],
                  attendanceRatioOfPresentsDic[registration.id],
                  coursesDic[registration.courseId]
                )
              ),
              total: searchRegistrationResult.totalCount
            };
          })
        );
      })
    );
  }
}
