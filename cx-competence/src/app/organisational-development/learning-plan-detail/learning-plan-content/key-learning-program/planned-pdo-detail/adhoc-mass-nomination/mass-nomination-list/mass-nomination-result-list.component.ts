import { Component, Input, OnChanges } from '@angular/core';
import { MassAssignPDOResultModel } from 'app-models/mpj/assign-pdo.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { NominationFileNameRendererComponent } from 'app/approval-page/ag-grid-renderer/nomination-file-name-renderer.component';
import { AppConstant } from 'app/shared/app.constant';

@Component({
  selector: 'mass-nomination-result-list',
  templateUrl: './mass-nomination-result-list.component.html',
  styleUrls: ['./mass-nomination-result-list.component.scss'],
})
export class MassNominationResultListComponent implements OnChanges {
  @Input() items: MassAssignPDOResultModel[] = [];
  private gridColumnApi: any;
  private columns: any[] = [];
  private gridApi: any;
  rowData: any[];
  columnDefs: any;
  rowSelection: any;
  defaultColDef: any;
  context: any;
  noRowsTemplate: string = '<div class="grid-nodata">No data</div>';
  frameworkComponents: any;
  constructor() {}

  ngOnChanges(): void {
    this.defaultColDef = {
      headerCheckboxSelection: false,
      checkboxSelection: this.isFirstColumn,
      resizable: true,
    };
    this.frameworkComponents = {
      fileInfoRenderer: NominationFileNameRendererComponent,
    };
    this.context = { componentParent: this };
    this.setColumnDefs();
    this.setRowData(this.items);
  }

  isFirstColumn(params: any): boolean {
    const displayedColumns = params.columnApi.getAllDisplayedColumns();
    const thisIsFirstColumn = displayedColumns[0] === params.column;

    return thisIsFirstColumn;
  }
  onGridReady(params: any): void {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.gridApi.hideOverlay();
  }
  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }
  private setRowData(massNominationResults: MassAssignPDOResultModel[]): void {
    if (!massNominationResults || massNominationResults.length === 0) {
      this.rowData = new Array();

      return;
    }

    if (this.gridColumnApi && this.gridColumnApi.columnController) {
      if (this.columns && this.columns.length <= 0) {
        this.columns = this.gridColumnApi.columnController.columnDefs;
        this.columns.pop();
      }
    }

    this.rowData = new Array();
    massNominationResults.forEach((result) => {
      this.rowData.push({
        created: DateTimeUtil.toDateString(
          result.created,
          AppConstant.backendDateTimeFormat
        ),
        createdBy: result.createdBy ? result.createdBy.email : 'N/A',
        fileInfo: {
          displayFileName: result.originalFileName,
          resultId: result.id,
          fileId: result.fileId,
        },
        numberOfRegisters: result.numberOfRegisters,
        numberOfPendingApprovals: result.numberOfPendingApprovals,
      });
    });
  }
  private setColumnDefs(): void {
    this.columnDefs = [
      {
        headerName: 'FILE NAME',
        field: 'fileInfo',
        colId: 'fileInfo',
        minWidth: 450,
        checkboxSelection: false,
        cellRenderer: 'fileInfoRenderer',
      },
      {
        headerName: 'NOMINATED BY',
        field: 'createdBy',
        colId: 'createdBy',
        minWidth: 250,
        checkboxSelection: false,
      },
      {
        headerName: 'NOMINATION DATE',
        field: 'created',
        colId: 'created',
        minWidth: 150,
        checkboxSelection: false,
      },
      {
        headerName: 'NO. OF REGISTERS',
        field: 'numberOfRegisters',
        colId: 'numberOfRegisters',
        minWidth: 50,
        checkboxSelection: false,
      },
      {
        headerName: 'NO. OF PENDING APPROVAL',
        field: 'numberOfPendingApprovals',
        colId: 'numberOfPendingApprovals',
        minWidth: 100,
        checkboxSelection: false,
      },
    ];
  }
}
