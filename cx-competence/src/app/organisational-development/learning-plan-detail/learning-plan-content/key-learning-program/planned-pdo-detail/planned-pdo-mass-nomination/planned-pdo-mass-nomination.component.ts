import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,


  OnInit, Output,
  ViewChild
} from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { ResultHelper } from 'app-services/idp/result-helpers';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { MassNominationResultsComponent } from '../adhoc-mass-nomination/mass-nomination-results/mass-nomination-results.component';
import { AssignModeEnum } from '../planned-pdo-detail.model';
@Component({
  selector: 'planned-pdo-mass-nomination',
  templateUrl: './planned-pdo-mass-nomination.component.html',
  styleUrls: ['./planned-pdo-mass-nomination.component.scss'],
})
export class PlannedPdoMassNominationComponent implements OnInit {
  @Input() klpDto: IdpDto;
  @Output() clickedBackBtn: EventEmitter<void> = new EventEmitter();
  @ViewChild('uploadFile') uploadFileElement: ElementRef;
  @ViewChild('massNominationResult', { static: true })
  massNominationResulElement: MassNominationResultsComponent;
  assignMode: AssignModeEnum = AssignModeEnum.Nominate;

  klpExtId: string;
  departmentId: number;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private globalLoader: CxGlobalLoaderService
  ) { }

  ngOnInit() {
    this.globalLoader.showLoader();
    if (!this.processAndValidateData()) {
      console.error('Invalid input data for this component');
      this.globalLoader.hideLoader();

      return;
    }
    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  onClickBack(): void {
    this.clickedBackBtn.emit();
  }

  getUserExtId(userSelectItem: CxSelectItemModel<Staff>): string {
    if (!userSelectItem || !userSelectItem.dataObject) {
      return;
    }

    return userSelectItem.dataObject.identity.extId;
  }

  loadReportFileResults(): void {
    this.massNominationResulElement.loadFileResults();
  }

  private processAndValidateData(): boolean {
    this.klpExtId = ResultHelper.getResultExtId(this.klpDto);
    this.departmentId = ResultHelper.getObjectiveId(this.klpDto);

    return !!this.klpExtId && !!this.departmentId;
  }
}
