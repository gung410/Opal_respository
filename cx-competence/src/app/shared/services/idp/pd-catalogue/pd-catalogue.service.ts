import { Injectable } from '@angular/core';
import { PDCatalogSearchPayload } from 'app-models/pdcatalog/pdcatalog.dto';
import { PagingResponseModel } from 'app-models/user-management.model';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { CoursepadNoteDto } from 'app/individual-development/models/course-note.model';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
import { AppConstant } from 'app/shared/app.constant';
import { isEmpty } from 'lodash';
import { Observable } from 'rxjs';
import { IDPService } from '../idp.service';

type PagingCourseDetail = PagingResponseModel<PDOpportunityDetailModel>;
type PagingPDOCatalogCourse = PagingResponseModel<PDCatalogCourseModel>;

@Injectable()
export class PdCatalogueService {
  constructor(
    private idpService: IDPService,
    private pdOpportunityService: PDOpportunityService
  ) {}

  async searchPDCatalogue(
    searchKey: string,
    tagIds: string[] = [],
    pageIndex: number = 0,
    pageSize: number = AppConstant.ItemPerPage
  ): Promise<PagingPDOCatalogCourse> {
    const tagsIdField = ['equals'].concat(tagIds);
    const payload: PDCatalogSearchPayload = {
      page: pageIndex + 1,
      limit: pageSize,
      searchText: searchKey,
      searchFields: ['Title', 'Description', 'Code'],
      useFuzzy: true,
      useSynonym: true,
      searchCriteria: {
        'tags.id': tagsIdField,
        status: ['contains', 'published'],
        resourceType: ['contains', 'course'],
      },
    };

    const pagingCourseDetails = await this.getCourseFromPDCatalog(payload);
    const pagingPDOCatalogCourses = this.toPagingPDCatalogCourses(
      pagingCourseDetails
    );

    return pagingPDOCatalogCourses;
  }

  async getRecommendFromPDCatalogueAsync(
    tagIds: string[],
    pageIndex: number = 0,
    pageSize: number = AppConstant.ItemPerPage
  ): Promise<PagingPDOCatalogCourse> {
    if (isEmpty(tagIds)) {
      return;
    }
    const tagsIdField = ['contains'].concat(tagIds);
    const payload: PDCatalogSearchPayload = {
      page: pageIndex + 1,
      limit: pageSize,
      searchCriteria: {
        'tags.id': tagsIdField,
        status: ['contains', 'published'],
        resourceType: ['contains', 'course'],
      },
    };

    const pagingCourseDetails = await this.getCourseFromPDCatalog(payload);
    const pagingPDOCatalogCourses = this.toPagingPDCatalogCourses(
      pagingCourseDetails
    );

    return pagingPDOCatalogCourses;
  }

  async getBookmarkedPDOsMapAsync(): Promise<Map<string, CoursepadNoteDto>> {
    const response = await this.idpService.getBookmarkedPDOs();
    if (response.error) {
      return new Map([]);
    }

    const responseData = response.data;
    if (!responseData || !responseData.items || !responseData.items.length) {
      return new Map([]);
    }

    const bookmarkedPDOs = responseData.items;
    const mapResult = bookmarkedPDOs.reduce(
      (map: Map<string, CoursepadNoteDto>, course) => {
        map.set(course.courseId, course);

        return map;
      },
      new Map<string, CoursepadNoteDto>([])
    );

    return mapResult;
  }

  async getBookmarkedPDOsAsync(
    selectedCourseIds: string[]
  ): Promise<PDCatalogCourseModel[]> {
    const response = await this.idpService.getBookmarkedPDOs();
    if (response.error) {
      return [];
    }

    const responseData = response.data;
    if (!responseData || isEmpty(responseData.items)) {
      return [];
    }

    const courseIds = responseData.items.map((course) => course.courseId);
    const courseDetails = await this.pdOpportunityService.getPDCatalogPDODetailListAsync(
      courseIds
    );
    const bookmarkedPDOs = courseDetails.map((item) => {
      return this.bookmarkedPDOToOpportunityModel(item, selectedCourseIds);
    });

    return bookmarkedPDOs;
  }

  updatePDOSelectBookmarkInfo(
    items: PDCatalogCourseModel[],
    bookmarkedCourses: Map<string, CoursepadNoteDto>,
    selectedCourseIds: string[]
  ): PDCatalogCourseModel[] {
    for (const item of items) {
      const courseId = item.course.id;
      item.isBookmarked = bookmarkedCourses
        ? bookmarkedCourses.has(courseId)
        : false;
      item.isSelected = selectedCourseIds
        ? selectedCourseIds.includes(courseId)
        : false;
    }

    return items;
  }

  saveBookmarkToPlan(courseId: string): Observable<object> {
    return this.idpService.saveBookmarkToPlan(courseId);
  }

  uncheckBookmark(courseId: string): Observable<object> {
    return this.idpService.uncheckBookmark(courseId);
  }

  private bookmarkedPDOToOpportunityModel(
    courseDetail: PDOpportunityDetailModel,
    selectedCourseIds: string[]
  ): PDCatalogCourseModel {
    const courseId = courseDetail.id.toString();
    const pdo = new PDCatalogCourseModel({
      course: courseDetail,
      isSelected: selectedCourseIds.includes(courseId),
      isBookmarked: true,
    });
    pdo.isSelected = selectedCourseIds
      ? selectedCourseIds.includes(courseId)
      : false;

    return pdo;
  }

  private toPagingPDCatalogCourses(
    pagingPDOCatalogCourses: PagingCourseDetail
  ): PagingPDOCatalogCourse {
    if (!pagingPDOCatalogCourses) {
      return;
    }
    pagingPDOCatalogCourses.items = pagingPDOCatalogCourses.items || [];

    return {
      hasMoreData: pagingPDOCatalogCourses.hasMoreData,
      items: pagingPDOCatalogCourses.items.map(this.mapToPDCatalogCourseModel),
      pageIndex: pagingPDOCatalogCourses.pageIndex,
      pageSize: pagingPDOCatalogCourses.pageSize,
      totalItems: pagingPDOCatalogCourses.totalItems,
    };
  }

  private mapToPDCatalogCourseModel(
    pdoDetail: PDOpportunityDetailModel
  ): PDCatalogCourseModel {
    return new PDCatalogCourseModel({
      course: pdoDetail,
    });
  }

  private async getCourseFromPDCatalog(
    payload: PDCatalogSearchPayload
  ): Promise<PagingCourseDetail> {
    if (!payload || !(payload.page > 0) || !(payload.limit > 0)) {
      console.error('Invalid payload');

      return;
    }

    const response = await this.idpService.searchPDCatalog(payload);
    if (response.error || !response.data || isEmpty(response.data.resources)) {
      return;
    }

    const responseData = response.data;
    const courseResults = response.data.resources;
    const courseIds = courseResults.map((course) => course.id);
    const courseDetailResults = await this.pdOpportunityService.getPDCatalogPDODetailListAsync(
      courseIds
    );

    if (isEmpty(courseDetailResults)) {
      return;
    }

    const pagingModel: PagingCourseDetail = {
      hasMoreData: responseData.total > courseResults.length,
      pageIndex: payload.page,
      pageSize: payload.limit,
      totalItems: responseData.total,
      items: courseDetailResults,
    };

    return pagingModel;
  }
}
