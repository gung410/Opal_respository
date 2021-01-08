import {
  AttachmentType,
  CatalogResourceType,
  CatalogueRepository,
  ICatalogRecommendationRequest,
  ICatalogSearchRequest,
  ICatalogSearchResult,
  ICatalogSearchV2Request,
  INewlyAddedCoursesRequest,
  MetadataTagModel,
  ResourceStatistics,
  UserInfoModel
} from '@opal20/domain-api';
import { EventTrackParam, SearchCatalogEventPayload } from '../user-activities-tracking/user-tracking.models';
import { Observable, combineLatest, of } from 'rxjs';
import { PDCatalogueHelper, PDCatalogueV2Helper } from './pd-catalogue-helper.service';
import { map, switchMap } from 'rxjs/operators';

import { AdvanceSearchCatalogModel } from '../models/catalogue-adv-search-filter.model';
import { CourseDataService } from './course-data.service';
import { CslDataService } from './csl-data.service';
import { DigitalContentDataService } from './digital-content.service';
import { ILearningItemModel } from '../models/learning-item.model';
import { Injectable } from '@angular/core';
import { LearnerLearningPathDataService } from './learningpath-data.service';
import { LearnerLearningPathModel } from '../models/learning-path.model';
import { StandaloneFormDataService } from './standalone-form-service';
import { TrackingSourceService } from '../user-activities-tracking/tracking-souce.service';
import { UserTrackingService } from '../user-activities-tracking/user-tracking.service';
import { Utils } from '@opal20/infrastructure';

@Injectable()
export class CatalogueDataService {
  private _showingModel: AdvanceSearchCatalogModel;
  private _searchingModel: AdvanceSearchCatalogModel;
  public get model(): AdvanceSearchCatalogModel {
    return this._showingModel;
  }

  private tags: MetadataTagModel[];

  constructor(
    private catalogueRepository: CatalogueRepository,
    private courseDataSvc: CourseDataService,
    private digitalContentDataService: DigitalContentDataService,
    private learnerLearningPathDataService: LearnerLearningPathDataService,
    private cslDataService: CslDataService,
    private trackingSource: TrackingSourceService,
    private userTrackingService: UserTrackingService,
    private standaloneFormDataService: StandaloneFormDataService
  ) {
    this.courseDataSvc.getCourseMetadata().subscribe(tags => {
      this.tags = tags;
      this.clearFilter();

      this._showingModel.updateMetadataTags(tags);
    });
  }

  public clearFilter(): void {
    this._searchingModel = new AdvanceSearchCatalogModel(this.tags);
    this._showingModel = this._searchingModel.clone();
  }

  public applyFilter(): void {
    this._searchingModel = this._showingModel.clone();
    this._showingModel = this._showingModel;
  }

  public cancel(): void {
    this._showingModel = this._searchingModel.clone();
  }

  public searchCourse(
    searchText: string,
    resourceType: CatalogResourceType,
    allResourceTypes: CatalogResourceType[],
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
    resourceStatistics: ResourceStatistics;
  }> {
    const request: ICatalogSearchV2Request = {
      page: PDCatalogueHelper.calculatePageNum(skipCount, maxResultCount),
      limit: maxResultCount,
      statisticResourceTypes: allResourceTypes,
      searchText: searchText,
      searchFields: PDCatalogueHelper.defaultSearchFields,
      useFuzzy: true,
      useSynonym: true,

      resourceType: resourceType,
      enableStatistics: true,
      attachmentTypeFilter:
        this._searchingModel.attachmentType && this._searchingModel.attachmentType !== AttachmentType.All
          ? this._searchingModel.attachmentType
          : undefined
    };

    PDCatalogueV2Helper.setupDefaultResourceFilter(request, allResourceTypes);

    const searchTagIds = this._searchingModel.getAllTagIds();
    PDCatalogueV2Helper.addTagsFilter(request, allResourceTypes, searchTagIds);

    PDCatalogueV2Helper.addCommnunityIntoFilters(request, 'community', this._searchingModel.communitiesType);

    this.createAndSendTrackingSearchEvent(request, allResourceTypes);

    return this.getLearningItemsFromTheirsRepository(this.catalogueRepository.searchV2(request, showSpinner));
  }

  public searchPDCatalogueByType(
    searchText: string,
    resourceType: CatalogResourceType,
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const request: ICatalogSearchV2Request = {
      page: PDCatalogueHelper.calculatePageNum(skipCount, maxResultCount),
      limit: maxResultCount,
      statisticResourceTypes: [resourceType],
      searchText: searchText,
      searchFields: PDCatalogueHelper.defaultSearchFields,
      useFuzzy: true,
      useSynonym: true,

      resourceType: resourceType,
      enableStatistics: true,
      attachmentTypeFilter: this._searchingModel.attachmentType
    };

    PDCatalogueV2Helper.setupDefaultResourceFilter(request, [resourceType]);

    const searchTagIds = this._searchingModel.getAllTagIds();
    PDCatalogueV2Helper.addTagsFilter(request, [resourceType], searchTagIds);

    return this.getLearningItemsByType(this.catalogueRepository.searchV2(request, showSpinner), resourceType);
  }

  public getNewlyAddedCourses(
    showSpinner: boolean = true,
    maxResultCount: number = 10,
    skipCount: number = 0
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const request: INewlyAddedCoursesRequest = {
      sort: 'desc',
      page: PDCatalogueHelper.calculatePageNum(skipCount, maxResultCount),
      limit: maxResultCount
    };

    PDCatalogueHelper.addResourceTypeFilterCriteria(request, ['content', 'microlearning', 'course']);
    PDCatalogueHelper.addStatusFilterCriteria(request);
    PDCatalogueHelper.addDigitalContentFilterCriteria(request);
    PDCatalogueHelper.addRegistrationMethodFilterCriteria(request);

    return this.courseDataSvc.getCourseModelResultFromCatalogResult(this.catalogueRepository.loadNewlyAddedCourses(request, showSpinner));
  }

  public getRecommendLearningPathsFromLMM(
    searchText: string,
    filterTagIds: string[],
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true
  ): Observable<{
    total: number;
    items: LearnerLearningPathModel[];
  }> {
    const request: ICatalogRecommendationRequest = {
      userId: UserInfoModel.getMyUserInfo().extId,
      page: PDCatalogueHelper.calculatePageNum(skipCount, maxResultCount),
      limit: maxResultCount,
      enableHighlight: true,
      resourceTypesFilter: ['learningpath'],
      searchText: searchText
    };

    return this.learnerLearningPathDataService.getLearningPathFromCatalogResult(
      this.catalogueRepository.loadSuggestedLearningItems(request, showSpinner)
    );
  }

  public getSuggestedCourses(
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const request: ICatalogRecommendationRequest = {
      userId: UserInfoModel.getMyUserInfo().extId,
      page: PDCatalogueHelper.calculatePageNum(skipCount, maxResultCount),
      limit: maxResultCount,
      enableHighlight: true,
      resourceTypesFilter: ['course', 'content', 'microlearning']
    };

    return this.courseDataSvc.getCourseModelResultFromCatalogResult(
      this.catalogueRepository.loadSuggestedLearningItems(request, showSpinner)
    );
  }

  public getSharedToMe(
    maxResultCount: number = 10,
    skipCount: number = 0
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.userTrackingService.getSharedToMeFromCatalogResult(skipCount, maxResultCount);
  }

  private createAndSendTrackingSearchEvent(request: ICatalogSearchRequest, allResourceTypes: CatalogResourceType[]): void {
    const searchTags = this._searchingModel.getTagsIncludedTagNames();
    const event: SearchCatalogEventPayload = {
      searchText: request.searchText,
      searchFields: request.searchFields,
      types: allResourceTypes,
      page: request.page,
      limit: request.limit,
      tags: searchTags
    };

    this.trackingSource.eventTrack.next(<EventTrackParam>{
      eventName: 'SearchCatalog',
      payload: event
    });
  }

  private getLearningItemsFromTheirsRepository(
    searchResult: Observable<ICatalogSearchResult>
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
    resourceStatistics: ResourceStatistics;
  }> {
    return searchResult.pipe(
      map(_ => {
        const contentIds = _.resources.filter(r => r.resourcetype === 'content').map(r => r.id);
        const courseIds = _.resources.filter(r => r.resourcetype === 'course' || r.resourcetype === 'microlearning').map(r => r.id);
        const learningPathIds = _.resources.filter(r => r.resourcetype === 'learningpath').map(r => r.id);
        const communityIds = _.resources.filter(r => r.resourcetype === 'community').map(r => r.id);
        const standaloneFormIds = _.resources.filter(r => r.resourcetype === 'form').map(r => r.id);

        const courseLearningItem$ = courseIds.length ? this.courseDataSvc.getCourseLearningItem(courseIds) : of([]);
        const contentLearningItem$ = contentIds.length ? this.digitalContentDataService.getContentLearningItems(contentIds) : of([]);
        const learningPathItem$ = learningPathIds.length
          ? this.learnerLearningPathDataService.getLearningPathItems(learningPathIds)
          : of([]);
        const communityItems$ = communityIds.length ? this.cslDataService.getCommunityItems(communityIds) : of([]);
        const standaloneFormLearningItem$ = standaloneFormIds.length
          ? this.standaloneFormDataService.getStandaloneFormLearningItems(standaloneFormIds)
          : of([]);

        const sumTotal = _.resourceStatistics.find(p => p.type === 'all').total;
        return combineLatest(
          courseLearningItem$,
          contentLearningItem$,
          learningPathItem$,
          communityItems$,
          standaloneFormLearningItem$
        ).pipe(
          map(arrayItems => {
            return {
              total: sumTotal,
              items: this.courseDataSvc.sortMergeItems(_.resources.map(r => r.id), Utils.flatTwoDimensionsArray(arrayItems)),
              resourceStatistics: _.resourceStatistics
            };
          })
        );
      }),
      switchMap(_ => _)
    );
  }

  private getLearningItemsByType(
    searchResult: Observable<ICatalogSearchResult>,
    resourceType: CatalogResourceType
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return searchResult.pipe(
      map(_ => {
        const itemIds = _.resources.filter(r => r.resourcetype === resourceType).map(r => r.id);

        if (resourceType === 'content') {
          return this.digitalContentDataService.getContentLearningItems(itemIds).pipe(
            map(response => {
              return {
                total: _.total,
                items: response
              };
            })
          );
        }

        return this.courseDataSvc.getCourseLearningItem(itemIds).pipe(
          map(response => {
            return {
              total: _.total,
              items: response
            };
          })
        );
      }),
      switchMap(_ => _)
    );
  }
}
