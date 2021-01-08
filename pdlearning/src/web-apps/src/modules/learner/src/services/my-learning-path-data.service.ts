import {
  CatalogResourceType,
  CatalogueRepository,
  Course,
  CourseRepository,
  ICatalogSearchRequest,
  ISearchUsersForLearningPathRequestDto,
  MyLearningPathApiService,
  PublicUserInfo,
  UserRepository
} from '@opal20/domain-api';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PDCatalogueHelper } from './pd-catalogue-helper.service';

@Injectable()
export class MyLearningPathDataService {
  constructor(
    private myLearningPathApiService: MyLearningPathApiService,
    private userRepository: UserRepository,
    private catalogueRepository: CatalogueRepository,
    private courseRepository: CourseRepository
  ) {}

  public searchUsers(request: ISearchUsersForLearningPathRequestDto): Observable<PublicUserInfo[]> {
    return this.myLearningPathApiService.searchUsers(request).pipe(
      map(response => response.items),
      switchMap(users => {
        const userIds = users.map(user => user.id);
        const followingUserIds = users.filter(user => user.isFollowing).map(user => user.id);
        return this.loadPublicUsers(userIds, followingUserIds);
      })
    );
  }

  public loadPublicUsers(userIds: string[], followingUserIds: string[]): Observable<PublicUserInfo[]> {
    return this.userRepository.loadPublicUserInfoList({ userIds }).pipe(
      map(publicUsers =>
        followingUserIds.length
          ? publicUsers.map(publicUser => {
              publicUser.isFollowing = followingUserIds.includes(publicUser.id);
              return publicUser;
            })
          : publicUsers
      )
    );
  }

  public searchCourses(searchText: string, skipCount: number, maxResultCount: number): Observable<Course[]> {
    const resourceTypes: CatalogResourceType[] = ['microlearning', 'course'];
    const request: ICatalogSearchRequest = {
      page: PDCatalogueHelper.calculatePageNum(skipCount, maxResultCount),
      limit: maxResultCount,
      statisticResourceTypes: resourceTypes,
      resourceTypesFilter: resourceTypes,
      searchText: searchText,
      searchFields: PDCatalogueHelper.defaultSearchFields,
      useFuzzy: true,
      useSynonym: true
    };

    PDCatalogueHelper.addResourceTypeFilterCriteria(request, resourceTypes);
    PDCatalogueHelper.addStatusFilterCriteria(request);
    PDCatalogueHelper.addRegistrationMethodFilterCriteria(request);

    return this.catalogueRepository.search(request).pipe(
      switchMap(response => {
        const courseIds = response.resources ? response.resources.map(rsc => rsc.id) : [];
        return this.courseRepository.loadCourses(courseIds).pipe(
          map(courses => {
            courses.sort((a, b) => courseIds.indexOf(a.id) - courseIds.indexOf(b.id));
            return courses;
          })
        );
      })
    );
  }
}
