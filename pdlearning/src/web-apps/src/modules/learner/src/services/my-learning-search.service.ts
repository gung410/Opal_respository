import {
  CatalogResourceType,
  CommunityStatisticType,
  CommunityStatus,
  ICatalogSearchRequest,
  IMyLearningSearchRequest,
  LearningPathStatisticType,
  MetadataTagGroupCode,
  MyLearningPathApiService,
  MyLearningStatisticType,
  StatisticType,
  UserInfoModel
} from '@opal20/domain-api';
import {
  CommunitySearchLearningType,
  CourseSearchLearningType,
  FilterStatisticResult,
  LearningPathSearchLearningType,
  MyLearningSearchFilterResult,
  SearchFilterModel,
  SearchLearningType
} from '../models/my-learning-search-filter-model';
import { LearnerLearningPathModel, LearnerLearningPathSearchFilterResultModel } from '../models/learning-path.model';
import { Observable, combineLatest, from } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CatalogueDataService } from './catalogue-data.service';
import { CommunityItemModel } from '../models/community-item.model';
import { CourseDataService } from './course-data.service';
import { CslDataService } from './csl-data.service';
import { DigitalContentDataService } from './digital-content.service';
import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { Injectable } from '@angular/core';
import { LearningItemModel } from '../models/learning-item.model';
import { MyLearningPagingModel } from '../models/my-learning-paging-model';
import { PDCatalogueHelper } from './pd-catalogue-helper.service';

@Injectable()
export class MyLearningSearchDataService {
  constructor(
    private catalogueDataService: CatalogueDataService,
    private courseDataService: CourseDataService,
    private digitalContentDataService: DigitalContentDataService,
    private cslDataService: CslDataService,
    private myLearningPathApiService: MyLearningPathApiService
  ) {}

  public searchCourses(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<LearningItemModel>> {
    const request: IMyLearningSearchRequest = {
      includeStatistic: searchModel.includeStatistic,
      maxResultCount: searchModel.maxResultCount,
      skipCount: searchModel.skipCount,
      searchText: searchModel.searchText,
      type: searchModel.type,
      statusFilter: this.convertSearchTypeToFilterRequestType(searchModel.learningTypeFilter),
      statisticsFilter: this.convertSearchTypesToFilterRequestTypes(searchModel.learningTypes)
    };
    return this.courseDataService.getMyCoursesBySearch(request).pipe(
      map(data => {
        if (data) {
          const items = data.items.map(p => new LearningItemModel(p));
          return new MyLearningSearchFilterResult<LearningItemModel>({
            items: items,
            totalCount: data.totalCount,
            statistics: data.statistics.map(
              p =>
                new FilterStatisticResult({
                  totalCount: p.totalCount,
                  type: STATISTIC_TYPE_TO_SEARCH_TYPE.get(p.type)
                })
            )
          });
        }
      })
    );
  }

  public searchDigitalContents(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<DigitalContentItemModel>> {
    const request: IMyLearningSearchRequest = {
      maxResultCount: searchModel.maxResultCount,
      skipCount: searchModel.skipCount,
      searchText: searchModel.searchText,
      type: searchModel.type,
      includeStatistic: searchModel.includeStatistic,
      statusFilter: this.convertSearchTypeToFilterRequestType(searchModel.learningTypeFilter),
      statisticsFilter: this.convertSearchTypesToFilterRequestTypes(searchModel.learningTypes)
    };

    return this.digitalContentDataService.getMyDigitalContentsBySearch(request).pipe(
      map(response => {
        const items = response.items.map(DigitalContentItemModel.createDigitalContentItemModel);
        return new MyLearningSearchFilterResult<DigitalContentItemModel>({
          statistics: response.statistics.map(
            p =>
              new FilterStatisticResult({
                totalCount: p.totalCount,
                type: STATISTIC_TYPE_TO_SEARCH_TYPE.get(p.type)
              })
          ),
          totalCount: response.totalCount,
          items: items
        });
      })
    );
  }

  public searchLearningPaths(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<LearnerLearningPathModel>> {
    return combineLatest([this.searchLearnerLearningPath(searchModel), this.searchLMMLearningPaths(searchModel)]).pipe(
      map(([learnerLP, lmmLP]) => {
        const result = new MyLearningSearchFilterResult<LearnerLearningPathModel>({
          items: learnerLP.items,
          totalCount: learnerLP.totalCount,
          statistics: learnerLP.statistics.map(
            p =>
              new FilterStatisticResult({
                totalCount: p.totalCount,
                type: STATISTIC_TYPE_TO_SEARCH_TYPE.get(p.type)
              })
          )
        });
        if (searchModel.learningTypeFilter === LearningPathSearchLearningType.Recommended) {
          result.items = lmmLP.items;
          result.totalCount = lmmLP.total;
        }
        this.addRecommendedLearningPathToSearchResult(result, lmmLP);
        return result;
      })
    );
  }

  public searchCommunities(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<CommunityItemModel>> {
    const request: ICatalogSearchRequest = {
      page: PDCatalogueHelper.calculatePageNum(searchModel.skipCount, searchModel.maxResultCount),
      limit: searchModel.maxResultCount,
      searchText: searchModel.searchText,
      searchFields: PDCatalogueHelper.defaultSearchFields,
      resourceTypesFilter: ['community'],
      useFuzzy: true,
      useSynonym: true
    };

    PDCatalogueHelper.addSpecifyStatusFilterCriteria(request, [CommunityStatus.enabled]);

    if (searchModel.searchText || searchModel.includeStatistic) {
      this.processCriteriaBySearching(searchModel, request);
    } else {
      this.processCriteriaBySingle(searchModel, request);
    }

    return this.cslDataService.getCommunities(request).pipe(
      map(response => {
        return new MyLearningSearchFilterResult<CommunityItemModel>({
          statistics: response.resourceStatistics.map(
            p =>
              new FilterStatisticResult({
                totalCount: p.total,
                type: STATISTIC_TYPE_TO_SEARCH_TYPE.get(p.type as StatisticType)
              })
          ),
          totalCount: response.total,
          items: response.items.map(CommunityItemModel.createCommunityItemModel)
        });
      })
    );
  }

  private processCriteriaBySearching(searchModel: SearchFilterModel, request: ICatalogSearchRequest): void {
    const myOwnResourceTypes: CatalogResourceType[] = ['createdby', 'memberships.id'];
    const joinedResourceTypes: CatalogResourceType[] = ['memberships.id', 'createdby'];
    const userExtId = UserInfoModel.getMyUserInfo().extId;

    // Always add criteriaOr when searching with text
    PDCatalogueHelper.addSearchCriteriaOr(request, {
      createdby: ['equals', userExtId],
      'memberships.id': ['contains', userExtId]
    });

    if (searchModel.learningTypeFilter === CommunitySearchLearningType.Joined) {
      request.statisticResourceTypes = joinedResourceTypes;
    }

    if (searchModel.learningTypeFilter === CommunitySearchLearningType.Owned) {
      request.statisticResourceTypes = myOwnResourceTypes;
    }
  }

  private processCriteriaBySingle(searchModel: SearchFilterModel, request: ICatalogSearchRequest): void {
    const userExtId = UserInfoModel.getMyUserInfo().extId;
    const searchForCreatedBy = {
      createdby: ['equals', userExtId]
    };

    const searchForJoined = {
      'memberships.id': ['contains', userExtId]
    };

    if (searchModel.learningTypeFilter === CommunitySearchLearningType.All) {
      PDCatalogueHelper.addSearchCriteriaOr(request, {
        createdby: ['equals', userExtId],
        'memberships.id': ['contains', userExtId]
      });
      return;
    }

    if (searchModel.learningTypeFilter === CommunitySearchLearningType.Joined) {
      PDCatalogueHelper.addSearchCriteriaOr(request, searchForJoined);
    }

    if (searchModel.learningTypeFilter === CommunitySearchLearningType.Owned) {
      PDCatalogueHelper.addSearchCriteriaOr(request, searchForCreatedBy);
    }
  }

  private addRecommendedLearningPathToSearchResult(
    result: MyLearningSearchFilterResult<LearnerLearningPathModel>,
    recommendedResult: MyLearningPagingModel<LearnerLearningPathModel>
  ): void {
    result.statistics.push({ type: LearningPathSearchLearningType.Recommended, totalCount: recommendedResult.total });
  }

  private searchLMMLearningPaths(searchModel: SearchFilterModel): Observable<MyLearningPagingModel<LearnerLearningPathModel>> {
    const maxResultCount = searchModel.learningTypeFilter === LearningPathSearchLearningType.Recommended ? 12 : 1;
    return from(this.getTagIdsOfSubjects()).pipe(
      switchMap(tagIds => {
        return this.catalogueDataService
          .getRecommendLearningPathsFromLMM(searchModel.searchText, tagIds, maxResultCount, searchModel.skipCount)
          .pipe(
            map(res => {
              return new MyLearningPagingModel<LearnerLearningPathModel>({ ...res });
            })
          );
      })
    );
  }

  private searchLearnerLearningPath(searchModel: SearchFilterModel): Observable<LearnerLearningPathSearchFilterResultModel> {
    const learnerSearchRequest: IMyLearningSearchRequest = {
      includeStatistic: searchModel.includeStatistic,
      maxResultCount: searchModel.maxResultCount,
      skipCount: searchModel.skipCount,
      searchText: searchModel.searchText,
      type: searchModel.type,
      statusFilter: this.convertSearchTypeToFilterRequestType(searchModel.learningTypeFilter),
      statisticsFilter: this.convertSearchTypesToFilterRequestTypes(searchModel.learningTypes)
    };
    return this.myLearningPathApiService.searchLearnerLearningPaths(learnerSearchRequest).pipe(
      map(response =>
        LearnerLearningPathSearchFilterResultModel.createLearningPathSearchFilterResultModel({
          ...response,
          items: response.items
        })
      )
    );
  }

  private getTagIdsOfSubjects(): Promise<string[]> {
    return this.courseDataService
      .getCourseMetadata()
      .pipe(
        map(data => {
          return data.filter(metadata => metadata.groupCode === MetadataTagGroupCode.PDO_TAXONOMY).map(metadata => metadata.id);
        })
      )
      .toPromise();
  }

  private convertSearchTypesToFilterRequestTypes<T extends StatisticType>(values: SearchLearningType[]): T[] {
    return values.map(p => this.convertSearchTypeToFilterRequestType<T>(p)).filter(p => p != null);
  }

  private convertSearchTypeToFilterRequestType<T extends StatisticType>(value: SearchLearningType): T {
    return SEARCH_TYPE_TO_STATISTIC_TYPE.get(value) as T;
  }
}

const SEARCH_TYPE_TO_STATISTIC_TYPE: Map<SearchLearningType, StatisticType> = new Map<SearchLearningType, StatisticType>([
  [CourseSearchLearningType.Registered, MyLearningStatisticType.Registered],
  [CourseSearchLearningType.Upcoming, MyLearningStatisticType.Upcoming],
  [CourseSearchLearningType.InProgress, MyLearningStatisticType.InProgress],
  [CourseSearchLearningType.Completed, MyLearningStatisticType.Completed],
  [LearningPathSearchLearningType.Owned, LearningPathStatisticType.Owned],
  [LearningPathSearchLearningType.Shared, LearningPathStatisticType.Shared],
  [CommunitySearchLearningType.All, CommunityStatisticType.All],
  [CommunitySearchLearningType.Joined, CommunityStatisticType.Joined],
  [CommunitySearchLearningType.Owned, CommunityStatisticType.Owned]
]);

const STATISTIC_TYPE_TO_SEARCH_TYPE: Map<StatisticType, SearchLearningType> = new Map<StatisticType, SearchLearningType>([
  [MyLearningStatisticType.Registered, CourseSearchLearningType.Registered],
  [MyLearningStatisticType.Upcoming, CourseSearchLearningType.Upcoming],
  [MyLearningStatisticType.InProgress, CourseSearchLearningType.InProgress],
  [MyLearningStatisticType.Completed, CourseSearchLearningType.Completed],
  [LearningPathStatisticType.Owned, LearningPathSearchLearningType.Owned],
  [LearningPathStatisticType.Shared, LearningPathSearchLearningType.Shared],
  [CommunityStatisticType.All, CommunitySearchLearningType.All],
  [CommunityStatisticType.Joined, CommunitySearchLearningType.Joined],
  [CommunityStatisticType.Owned, CommunitySearchLearningType.Owned]
]);
