import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { MassAssignPDOResultModel } from 'app-models/mpj/assign-pdo.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { KlpNominationService } from 'app-services/idp/assign-pdo/klp-nomination.service';
import { AppConstant } from 'app/shared/app.constant';
import { AssignModeEnum } from '../../planned-pdo-detail.model';

@Component({
  selector: 'mass-nomination-results',
  templateUrl: './mass-nomination-results.component.html',
  styleUrls: ['./mass-nomination-results.component.scss'],
})
export class MassNominationResultsComponent implements OnInit {
  @Input() klpExtId: string;
  @Input() assignMode: AssignModeEnum;
  @Input() departmentId: number;
  massNominationResults: MassAssignPDOResultModel[] = [];

  pagedData: PagingResponseModel<any>;
  totalItems: number = 0;
  currentPageIndex: number = 1;
  currentPageSize: number = AppConstant.ItemPerPageOnDialog;
  defaultPageSize: number = AppConstant.ItemPerPageOnDialog;

  constructor(
    private klpNominationService: KlpNominationService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadFileResults();
  }

  async loadFileResults(): Promise<void> {
    this.pagedData = await this.getPagingData();
    this.totalItems = this.pagedData ? this.pagedData.totalItems : 0;
    this.massNominationResults = this.pagedData ? this.pagedData.items : [];
    this.changeDetectorRef.detectChanges();
  }

  onCurrentPageChange(pageIndex: number): void {
    this.currentPageIndex = pageIndex;
    this.loadFileResults();
  }

  onPageSizeChange(pageSize: number): void {
    this.currentPageIndex = 1;
    this.currentPageSize = +pageSize;
    this.loadFileResults();
  }

  private async getPagingData(): Promise<
    PagingResponseModel<MassAssignPDOResultModel>
  > {
    if (this.assignMode === AssignModeEnum.Nominate) {
      return await this.klpNominationService.getMassNominationAssignedPDOAsync(
        this.departmentId,
        this.klpExtId,
        this.currentPageIndex,
        this.currentPageSize
      );
    } else if (this.assignMode === AssignModeEnum.AdhocNominate) {
      return await this.klpNominationService.getAdHocMassNominationAssignedPDOAsync(
        this.departmentId,
        null,
        this.currentPageIndex,
        this.currentPageSize
      );
    }
  }
}
