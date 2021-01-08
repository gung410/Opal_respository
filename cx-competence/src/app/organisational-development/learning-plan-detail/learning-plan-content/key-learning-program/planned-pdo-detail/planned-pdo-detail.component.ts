import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { TabMenuItem } from 'app-models/common/cx-tab-menu-item';
import {
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import {
  PDOpportunityDetailModel,
  PDOpportunityStatusEnum,
} from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { PlannedPDOMenuEnum } from './planned-pdo-detail.model';

@Component({
  selector: 'planned-pdo-detail',
  templateUrl: './planned-pdo-detail.component.html',
  styleUrls: ['./planned-pdo-detail.component.scss'],
})
export class PlannedPDODetailComponent implements OnInit {
  @Input() pdoAnswer: PDOpportunityAnswerDTO;
  @Input() klpDto: IdpDto;
  @Input() allowManagePDO: boolean = false;
  @Output()
  clickedBackBtn: EventEmitter<PDOpportunityAnswerDTO> = new EventEmitter();

  tabs: TabMenuItem[];
  selectedTab: TabMenuItem;
  pdoDTO: PDOpportunityDTO;
  plannedPDODetail: PDOpportunityDetailModel;

  // flags
  dataReady: boolean = false;
  unPublished: boolean;

  constructor(
    private pdOpportunityService: PDOpportunityService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getPDODetail();
  }

  onClickBack(): void {
    this.clickedBackBtn.emit(this.pdoAnswer);
  }

  onClickedTab(tab: TabMenuItem): void {
    this.selectedTab = tab;
  }

  onUpdateExternalPDO(pdoAnswer: PDOpportunityAnswerDTO): void {
    this.handleOnUpdateExternalPDO(pdoAnswer);
  }

  updateExternalPDOPlannedCost(event: PDOpportunityAnswerDTO): void {
    this.pdoAnswer = event;
    this.changeDetectorRef.detectChanges();
  }

  get isShowAboutTab(): boolean {
    return this.selectedTab && this.selectedTab.id === PlannedPDOMenuEnum.About;
  }

  get isShowNominationsTab(): boolean {
    return (
      this.selectedTab && this.selectedTab.id === PlannedPDOMenuEnum.Nominations
    );
  }

  get isShowRecommendationsTab(): boolean {
    return (
      this.selectedTab &&
      this.selectedTab.id === PlannedPDOMenuEnum.Recommendations
    );
  }

  get isCostTab(): boolean {
    return this.selectedTab && this.selectedTab.id === PlannedPDOMenuEnum.Cost;
  }

  get isExternalPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CustomPDO;
  }

  get isCoursePadPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CoursePadPDO;
  }

  private menuItemBuilder(): TabMenuItem[] {
    let menus = [
      {
        id: PlannedPDOMenuEnum.About,
        translatePath: 'Odp.LearningPlan.PlannedPDODetail.About',
      },
      {
        id: PlannedPDOMenuEnum.Nominations,
        translatePath: 'Odp.LearningPlan.PlannedPDODetail.Nominations',
      },
      {
        id: PlannedPDOMenuEnum.Recommendations,
        translatePath: 'Odp.LearningPlan.PlannedPDODetail.Recommendations',
      },
      {
        id: PlannedPDOMenuEnum.Cost,
        translatePath: 'Odp.LearningPlan.PlannedPDODetail.Cost',
      },
    ];

    if (this.isExternalPDO) {
      menus = menus.filter(
        (item) => item.id !== PlannedPDOMenuEnum.Recommendations
      );
    }

    return menus;
  }

  private async getPDODetail(): Promise<void> {
    if (!this.pdoAnswer) {
      return;
    }
    this.pdoDTO = this.pdoAnswer.learningOpportunity;
    if (this.isCoursePadPDO) {
      const courseId = PDPlannerHelpers.getIdFromPDOAnswer(this.pdoAnswer);
      this.plannedPDODetail = await this.pdOpportunityService.getPDCatalogPDODetailAsync(
        courseId
      );
      this.unPublished = this.plannedPDODetail
        ? this.plannedPDODetail.status !== PDOpportunityStatusEnum.Published
        : undefined;
    }

    if (this.isExternalPDO) {
      this.plannedPDODetail = PDPlannerHelpers.generatePDODetailFromExternalPDODto(
        this.pdoDTO
      );
    }

    this.tabs = this.menuItemBuilder();
    this.selectedTab = this.tabs[0];

    this.dataReady = true;
    this.changeDetectorRef.detectChanges();
  }

  private handleOnUpdateExternalPDO(pdoAnswer: PDOpportunityAnswerDTO): void {
    PDPlannerHelpers.updateTagForExternalPDO(pdoAnswer.learningOpportunity);
    this.pdoAnswer = pdoAnswer;
    this.pdoDTO = pdoAnswer.learningOpportunity;
    this.plannedPDODetail = PDPlannerHelpers.generatePDODetailFromExternalPDODto(
      this.pdoDTO
    );
    this.changeDetectorRef.detectChanges();
  }
}
