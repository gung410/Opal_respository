import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { ClassRunModel } from 'app-models/classrun.model';
import { ICRUDPermission } from 'app-models/common/permission/permission-setting';
import { CourseContentItemModel } from 'app-models/course-content-item.model';
import { CourseReviewManagementModel } from 'app-models/course-review.model';
import {
  PDOpportunityDTO,
  PDOpportunityModel,
} from 'app-models/mpj/pdo-action-item.model';
import { CourseDetailTabEnum, SessionModel } from 'app-models/session.model';
import { IDPService } from 'app-services/idp/idp.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { ApprovalClassRunModel } from 'app/approval-page/models/class-registration.model';
import { isEmpty } from 'lodash';

@Component({
  selector: 'course-detail-modal',
  templateUrl: './course-detail-modal.component.html',
  styleUrls: ['./course-detail-modal.component.scss'],
})
export class CourseDetailModalComponent implements OnInit {
  @Input() resultId: number;
  @Input() courseId: string;
  @Input() isExternalPDO: boolean = false;
  @Input() classrunModel: ApprovalClassRunModel;
  @Input() pdoModel: PDOpportunityModel;
  @Input() selectedTab: CourseDetailTabEnum;
  @Input() pdPlanMode: boolean = false;
  @Input() allowEditExternalPDO: boolean = false;
  @Input() selectCourseMode: boolean = false;
  @Input() selectCourseModeDisplayText: string;
  @Input() selectCourseModeSelected: boolean = false;
  @Input() commentPermission: ICRUDPermission;
  @Output()
  updatedExternalPDO: EventEmitter<PDOpportunityDTO> = new EventEmitter<PDOpportunityDTO>();
  @Output() close: EventEmitter<void> = new EventEmitter();
  @Output() addToPlan: EventEmitter<string> = new EventEmitter();

  plannedPDODetail: PDOpportunityDetailModel;
  classrunDetail: ClassRunModel[] = [];
  sessions: SessionModel[];
  reviews: CourseReviewManagementModel;
  contents: CourseContentItemModel[];
  userInfo: any;
  dataLoaded: boolean = false;

  constructor(
    private pdOpportunityService: PDOpportunityService,
    private idpService: IDPService,
    private globalLoader: CxGlobalLoaderService
  ) {}

  ngOnInit(): void {
    this.isExternalPDO
      ? this.processExternalPDODetail()
      : this.processCatalogueCourseDetail();
  }

  async processCatalogueCourseDetail(): Promise<void> {
    if (!this.courseId) {
      return;
    }

    this.globalLoader.showLoader();
    await this.getResultData();
    await this.processCourseInfo();
    await this.processReviewInfo();
    await this.processContentInfo();
    await this.processClassRunInfo();
    await this.processSessionInfo();

    this.dataLoaded = true;
    this.globalLoader.hideLoader();
  }

  async processExternalPDODetail(): Promise<void> {
    this.globalLoader.showLoader();

    await this.getResultData();

    if (this.pdoModel && this.pdoModel.answerDTO) {
      const pdoDto = this.pdoModel.answerDTO.learningOpportunity;
      PDPlannerHelpers.updateTagForExternalPDO(pdoDto);
      this.plannedPDODetail = PDPlannerHelpers.generatePDODetailFromExternalPDODto(
        pdoDto
      );
    }

    this.dataLoaded = true;
    this.globalLoader.hideLoader();
  }

  async processCourseInfo(): Promise<void> {
    this.plannedPDODetail = await this.pdOpportunityService.getPDCatalogPDODetailAsync(
      this.courseId
    );

    if (
      this.pdoModel?.answerDTO?.learningOpportunity &&
      this.plannedPDODetail
    ) {
      this.pdoModel.answerDTO.learningOpportunity = PDPlannerHelpers.updateCoursePadPDOInfo(
        this.plannedPDODetail,
        this.pdoModel.answerDTO
      );
    }
  }

  async processClassRunInfo(): Promise<void> {
    if (this.classrunModel) {
      const classrunDto = await this.idpService.getClassRunById(
        this.classrunModel.id
      );
      this.classrunDetail.push(new ClassRunModel(classrunDto.data));
    } else {
      const classrunDtos = await this.idpService.getClassRunByCourseId(
        this.courseId
      );
      classrunDtos.data.items.map((p) =>
        this.classrunDetail.push(new ClassRunModel(p))
      );
    }
  }

  async processSessionInfo(): Promise<void> {
    let sessionDto;

    sessionDto = await this.idpService.getSessionsByClassRunIds(
      this.classrunDetail.map((p) => p.id)
    );
    this.sessions = sessionDto.data.map((p) => new SessionModel(p));
  }

  async processReviewInfo(): Promise<void> {
    this.getReviews();
  }

  async processContentInfo(): Promise<void> {
    const tocDto = await this.idpService.getTableOfContent(this.courseId);
    this.contents = tocDto.data.map((p) => new CourseContentItemModel(p));
  }

  public async getReviews(
    itemCount: number = 3,
    skipCount: number = 0
  ): Promise<void> {
    const reviewsDto = await this.idpService.getCourseReviews(
      this.courseId,
      'Course',
      'CreatedDate desc',
      skipCount,
      itemCount
    );

    if (reviewsDto.data && reviewsDto.data.items) {
      this.reviews = new CourseReviewManagementModel(
        reviewsDto.data.items,
        reviewsDto.data.rating,
        reviewsDto.data.totalCount
      );
    }
  }

  onLoadMoreReview(itemCount: number): void {
    this.getReviews(itemCount);
  }

  onClose(): void {
    this.close.emit();
  }

  onAddToPlanClicked(courseId: string): void {
    this.addToPlan.emit(courseId);
  }

  private async getResultData(): Promise<void> {
    if (isEmpty(this.resultId)) {
      return;
    }

    this.globalLoader.showLoader();
    if (!this.pdoModel) {
      this.pdoModel = await this.pdOpportunityService.getPDOActionItemById(
        this.resultId
      );
    }
  }
}
