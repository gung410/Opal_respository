import {
  CoursePlanningCycleRepository,
  CourseRepository,
  CourseStatus,
  SearchCourseResult,
  SearchCourseType,
  TaggingRepository,
  UserRepository
} from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CourseViewModel } from '../models/course-view.model';
import { Injectable } from '@angular/core';

@Injectable()
export class ListCourseGridComponentService {
  constructor(
    private courseRepository: CourseRepository,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository,
    private taggingRepository: TaggingRepository,
    private userRepository: UserRepository
  ) {}

  public loadCourses(
    searchText: string = '',
    filter: IFilter = null,
    searchCourseType: SearchCourseType = SearchCourseType.Owner,
    searchStatus: CourseStatus[] = null,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    checkCourseContent: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null,
    coursePlanningCycleId?: string
  ): Observable<OpalGridDataResult<CourseViewModel>> {
    return this.progressCourses(
      this.courseRepository.loadSearchCourses(
        searchText,
        filter,
        searchCourseType,
        searchStatus,
        skipCount,
        maxResultCount,
        checkCourseContent,
        coursePlanningCycleId
      ),
      checkAll,
      selectedsFn
    );
  }

  private progressCourses(
    courseObs: Observable<SearchCourseResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<CourseViewModel>> {
    return courseObs.pipe(
      switchMap(searchCourseResult => {
        if (searchCourseResult.totalCount === 0) {
          return of(<OpalGridDataResult<CourseViewModel>>{
            data: [],
            total: searchCourseResult.totalCount
          });
        }

        return combineLatest(
          this.taggingRepository.loadAllMetaDataTags(),
          this.userRepository.loadPublicUserInfoList({
            userIds: Utils.uniq(Utils.flatTwoDimensionsArray(searchCourseResult.items.map(_ => [_.createdBy, _.archivedBy])))
          }),
          this.coursePlanningCycleRepository.loadCoursePlanningCyclesByIds(
            Utils.uniq(searchCourseResult.items.filter(x => x.coursePlanningCycleId != null).map(_ => _.coursePlanningCycleId))
          )
        ).pipe(
          map(([metadata, users, coursePlanningCycles]) => {
            const metadataDic = Utils.toDictionary(metadata, p => p.tagId);
            const userDic = Utils.toDictionary(users, p => p.id);
            const coursePlanningCyclesDict = Utils.toDictionary(coursePlanningCycles, p => p.id);
            return <OpalGridDataResult<CourseViewModel>>{
              data: searchCourseResult.items.map(_ =>
                CourseViewModel.createFromModel(
                  _,
                  metadataDic,
                  userDic[_.createdBy],
                  userDic[_.archivedBy],
                  checkAll,
                  selectedsFn != null ? selectedsFn() : {},
                  _.coursePlanningCycleId && coursePlanningCyclesDict[_.coursePlanningCycleId]
                    ? coursePlanningCyclesDict[_.coursePlanningCycleId]
                    : null
                )
              ),
              total: searchCourseResult.totalCount
            };
          })
        );
      })
    );
  }
}
