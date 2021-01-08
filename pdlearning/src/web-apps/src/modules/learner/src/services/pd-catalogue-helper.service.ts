import * as moment from 'moment';

import {
  CatalogResourceType,
  CourseStatus,
  ICatalogSearchRequest,
  ICatalogSearchV2Request,
  INewlyAddedCoursesRequest,
  RegistrationMethod,
  ResourceTypeFilter,
  SEARCH_DATE_FORMAT
} from '@opal20/domain-api';

export class PDCatalogueHelper {
  public static defaultSearchFields = ['title', 'description', 'code', 'externalcode', 'tag', 'attachment'];

  public static calculatePageNum(skipCount: number, maxResultCount: number): number {
    return Math.max(1, Math.ceil((skipCount + 1) / maxResultCount));
  }

  public static addResourceTypeFilterCriteria(request: ICatalogSearchRequest | INewlyAddedCoursesRequest, resourceTypes: string[]): void {
    request.searchCriteria = request.searchCriteria || {};

    request.searchCriteria.resourceType = ['contains', ...resourceTypes];
  }

  public static addStatusFilterCriteria(request: ICatalogSearchRequest | INewlyAddedCoursesRequest): void {
    request.searchCriteria = request.searchCriteria || {};

    request.searchCriteria.status = ['contains', CourseStatus.Published.toLowerCase().toString()];
  }

  public static addSpecifyStatusFilterCriteria(request: ICatalogSearchRequest, status: string[]): void {
    if (status && status.length === 0) {
      return;
    }
    request.searchCriteria = request.searchCriteria || {};

    request.searchCriteria.status = ['contains', ...status];
  }

  public static addSpecifyTagFilterCriteria(request: ICatalogSearchRequest, tags: string[]): void {
    if (tags && tags.length === 0) {
      return;
    }
    request.searchCriteria = request.searchCriteria || {};

    request.searchCriteria['tags.id'] = ['contains', ...tags];
  }

  public static addSearchCriteriaOr(request: ICatalogSearchRequest, searchOr: {}): void {
    if (searchOr === undefined) {
      return;
    }
    request.searchCriteriaOr = searchOr;
  }

  public static addRegistrationMethodFilterCriteria(request: ICatalogSearchRequest | INewlyAddedCoursesRequest): void {
    request.searchCriteria = request.searchCriteria || {};

    request.searchCriteria.registrationMethod = [
      'contains',
      ...[RegistrationMethod.Public.toLowerCase().toString(), RegistrationMethod.Restricted.toLowerCase().toString(), 'resourceType:course']
    ];
  }

  public static addDigitalContentFilterCriteria(request: ICatalogSearchRequest | INewlyAddedCoursesRequest): void {
    const nowUtcDateTime = moment.utc().format(SEARCH_DATE_FORMAT);
    request.searchCriteria = request.searchCriteria || {};
    request.searchCriteria.startDate = ['lte', nowUtcDateTime, 'resourceType:content'];
    request.searchCriteria.expiredDate = ['gt', nowUtcDateTime, 'resourceType:content'];
    request.searchCriteria.IsArchived = ['equals', 'false', 'resourceType:content'];
  }
}

export class PDCatalogueV2Helper {
  private static ignoredResourceFilters: CatalogResourceType[] = ['all'];
  public static setupDefaultResourceFilter(request: ICatalogSearchV2Request, resourceTypes: CatalogResourceType[]): void {
    PDCatalogueV2Helper.addResourceStatusFilters(request, resourceTypes);
    PDCatalogueV2Helper.addDefaultFilters(request, resourceTypes);
  }

  public static addTagsFilter(request: ICatalogSearchV2Request, resourceTypes: CatalogResourceType[], tagIds: string[]): void {
    if (!tagIds.length) {
      return;
    }

    const addDefaultFilterPredicate: (resourceType: CatalogResourceType) => ResourceTypeFilter[] = (resource: CatalogResourceType) => [
      { fieldName: 'tags.id', operator: 'equals', values: [...tagIds] }
    ];

    PDCatalogueV2Helper.addFilters(request, resourceTypes, addDefaultFilterPredicate);
  }

  public static addCommnunityIntoFilters(request: ICatalogSearchV2Request, resourceType: CatalogResourceType, communities: string[]): void {
    if (!communities.length) {
      return;
    }

    const addCommunityFilterPredicate: ResourceTypeFilter[] = [
      {
        fieldName: 'JoinPolicyType',
        operator: 'equals',
        values: communities
      }
    ];

    PDCatalogueV2Helper.addMoreDataIntoFilters(request, resourceType, addCommunityFilterPredicate);
  }

  private static addResourceStatusFilters(request: ICatalogSearchV2Request, resourceTypes: CatalogResourceType[]): void {
    const resourceStatusFilters: Record<CatalogResourceType, string[]> = resourceTypes.reduce(
      (acc, item) => {
        if (!PDCatalogueV2Helper.ignoredResourceFilters.includes(item)) {
          acc[item] = PDCatalogueV2Helper.getResourceStatusFilters(item);
        }
        return acc;
      },
      {} as Record<CatalogResourceType, string[]>
    );

    request.resourceStatusFilters = resourceStatusFilters;
  }

  private static addDefaultFilters(request: ICatalogSearchV2Request, resourceTypes: CatalogResourceType[]): void {
    const addDefaultFilterPredicate = (resource: CatalogResourceType) => {
      switch (resource) {
        case 'content':
          return PDCatalogueV2Helper.getDigitalContentFilter();
        case 'course':
          return PDCatalogueV2Helper.getCourseFilter();
      }
    };

    PDCatalogueV2Helper.addFilters(request, resourceTypes, addDefaultFilterPredicate);
  }

  private static addFilters(
    request: ICatalogSearchV2Request,
    resourceTypes: CatalogResourceType[],
    addFilterPredicate: (resourceType: CatalogResourceType) => ResourceTypeFilter[]
  ): void {
    if (request.filters == null) {
      request.filters = {} as Record<CatalogResourceType, ResourceTypeFilter[]>;
    }

    resourceTypes.forEach(resource => {
      if (PDCatalogueV2Helper.ignoredResourceFilters.includes(resource)) {
        return;
      }

      const moreFilter: ResourceTypeFilter[] = addFilterPredicate(resource);

      if (moreFilter) {
        request.filters[resource] = request.filters[resource] ? request.filters[resource].concat(moreFilter) : moreFilter;
      }
    });
  }

  private static addMoreDataIntoFilters(
    request: ICatalogSearchV2Request,
    resourceType: CatalogResourceType,
    filterData: ResourceTypeFilter[]
  ): void {
    if (request.filters == null) {
      request.filters = {} as Record<CatalogResourceType, ResourceTypeFilter[]>;
    }

    if (filterData) {
      request.filters[resourceType] = request.filters[resourceType] ? request.filters[resourceType].concat(filterData) : filterData;
    }
  }

  private static getDigitalContentFilter(): ResourceTypeFilter[] {
    const nowUtcDateTime = moment.utc().format(SEARCH_DATE_FORMAT);
    return [
      { fieldName: 'startDate', operator: 'lte', values: [nowUtcDateTime] },
      { fieldName: 'expiredDate', operator: 'gt', values: [nowUtcDateTime] },
      { fieldName: 'IsArchived', operator: 'equals', values: ['false'] }
    ];
  }

  private static getCourseFilter(): ResourceTypeFilter[] {
    return [
      {
        fieldName: 'registrationMethod',
        operator: 'contains',
        values: [RegistrationMethod.Public.toLowerCase().toString(), RegistrationMethod.Restricted.toLowerCase().toString()]
      }
    ];
  }

  private static getResourceStatusFilters(resource: CatalogResourceType): string[] {
    if (resource === 'community') {
      return ['enabled'];
    }
    return ['published'];
  }
}
