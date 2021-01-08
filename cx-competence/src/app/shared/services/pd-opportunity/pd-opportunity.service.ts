import { Injectable } from '@angular/core';
import { ClassRunModel } from 'app-models/classrun.model';
import { PDOpportunityModel } from 'app-models/mpj/pdo-action-item.model';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { IDPService } from 'app-services/idp/idp.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { isEmpty } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MetadataTagModel } from './metadata-tag.model';
import { PDOpportunityDetailModel } from './pd-opportunity-detail.model';
import { PDOpportunityFilterModel } from './pd-opportunity-filter.model';

@Injectable()
export class PDOpportunityService {
  private metadataTag: MetadataTagModel[] = [];

  constructor(
    private idpService: IDPService,
    private toastrService: ToastrService
  ) {}

  async getPDCatalogPDODetailAsync(
    courseId: string
  ): Promise<PDOpportunityDetailModel> {
    const pdoDetailModelList = await this.getPDCatalogPDODetailListAsync([
      courseId,
    ]);

    return pdoDetailModelList[0];
  }

  async getPDCatalogPDODetailListAsync(
    courseIds: string[]
  ): Promise<PDOpportunityDetailModel[]> {
    const response = await this.idpService.getListCourseDetailByIds(courseIds);
    const listResponsePDO = response.data;

    if (response.error && isEmpty(listResponsePDO)) {
      this.toastrService.warning(
        'Oops, System cannot get PD Opportunity infomation.'
      );

      return [];
    }

    const metadataTag = await this.getMetadataTags();
    const pdoDetailModelList = listResponsePDO.map(
      (item) => new PDOpportunityDetailModel(item, metadataTag)
    );

    return pdoDetailModelList;
  }

  async getAllMetaDataTagsAsync(): Promise<PDOpportunityFilterModel> {
    const metadataTag = await this.getMetadataTags();
    const filterModel = new PDOpportunityFilterModel(metadataTag);

    return filterModel;
  }

  getClassRunByCourseIdObs = (
    courseId: string
  ): Observable<CxSelectItemModel<ClassRunModel>[]> => {
    // TODO: Implement paging for get classrun by courseId
    const pageIndex = 0;
    const pageSize = 1000;

    return this.idpService
      .getClassRunsByCourseId(courseId, pageIndex, pageSize)
      .pipe(map(AssignPDOHelper.mapPagedClassRunsToCxSelectItems));
  };

  async checkCourseHaveAnyClassRunAsync(courseId: string): Promise<boolean> {
    try {
      const listItem = await this.getClassRunByCourseIdObs(
        courseId
      ).toPromise();

      return !isEmpty(listItem);
    } catch (err) {
      return false;
    }
  }

  async getNoRegistrationCompleted(
    courseId: string,
    departmentId: number,
    planStartDateString: string,
    planEndDateString: string
  ): Promise<number> {
    if (!courseId || !departmentId) {
      console.error(
        'Cannot get finished registrations, getNoRegistrationCompleted: invalid params'
      );

      return 0;
    }

    const response = await this.idpService.getNORegistrationFinished({
      courseId,
      departmentId,
      forClassRunEndAfter: planStartDateString,
      forClassRunEndBefore: planEndDateString,
    });

    if (response.error && isEmpty(response.data)) {
      return 0;
    }

    return response.data.totalFinishedLearner;
  }

  async getPDOActionItemById(
    actionItemId: number
  ): Promise<PDOpportunityModel> {
    if (!actionItemId) {
      console.error('getPDOActionItemById: Invalid actionItemId');

      return;
    }

    const response = await this.idpService.getActionItems({
      resultIds: [actionItemId],
    });

    if (!response || isEmpty(response.data)) {
      return;
    }

    const actionItemDto = response.data[0];

    return PDPlannerHelpers.toPDOpportunityModel(actionItemDto);
  }

  private async getMetadataTags(): Promise<MetadataTagModel[]> {
    if (isEmpty(this.metadataTag)) {
      const response = await this.idpService.getMetadataTag();
      if (response.error || isEmpty(response.data)) {
        return;
      }
      this.metadataTag = response.data.map(
        (tagDTO) => new MetadataTagModel(tagDTO)
      );
    }

    return this.metadataTag;
  }
}
