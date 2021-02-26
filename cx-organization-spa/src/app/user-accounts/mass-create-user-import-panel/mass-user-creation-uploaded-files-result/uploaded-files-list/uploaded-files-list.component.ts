import {
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit
} from '@angular/core';
import { AgGridConfigModel } from 'app-models/ag-grid-config.model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { MassUserCreationFileGridHeader } from 'app/shared/constants/mass-user-creation-file-grid-header.enum';
import { FileInfoListViewModel } from 'app/user-accounts/viewmodels/file-info-list.viewmodel';
import { MassUserCreationFileNameRendererComponent } from '../renderer-components/mass-user-creation-file-name-renderer.component';

@Component({
  selector: 'uploaded-files-list',
  templateUrl: './uploaded-files-list.component.html',
  styleUrls: ['./uploaded-files-list.component.scss']
})
export class UploadedFilesListComponent implements OnInit, OnDestroy {
  @Input() set fileInfoItems(files: FileInfoListViewModel[]) {
    if (!files) {
      return;
    }
    this.agGridConfig.rowData = files;
  }
  agGridConfig: AgGridConfigModel;
  noRowsTemplate: string = '<div class="grid-nodata">No data</div>';
  fullTableContentWidth: number = 1395;
  RESIZE_DELAY_TIME: number = 200;

  constructor(
    private translateAdapterSvc: TranslateAdapterService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initGridConfig();
    this.initGridData();
    // this.setRowData(this.items);

    this.initScreenRotationEvent();
  }

  ngOnDestroy(): void {
    this.destroyScreenRotationEvent();
  }

  onGridReady(params: any): void {
    this.agGridConfig.gridApi = params.api;
    this.agGridConfig.gridColumnApi = params.columnApi;
    this.agGridConfig.gridApi.setDomLayout('autoHeight');
    this.calculateToResize();
    this.agGridConfig.gridApi.hideOverlay();
  }

  onFirstDataRendered(params: any): void {
    this.calculateToResize();
  }

  private initScreenRotationEvent(): void {
    window.addEventListener(
      'orientationchange',
      () => {
        this.calculateToResize();
      },
      false
    );
  }

  private destroyScreenRotationEvent(): void {
    // tslint:disable-next-line:no-empty
    window.removeEventListener('orientationchange', () => {}, true);
  }

  private initGridConfig(): void {
    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDefs(),
      frameworkComponents: {
        fileInfoRenderer: MassUserCreationFileNameRendererComponent
      },
      rowData: [],
      selectedRows: [],
      context: { componentParent: this },
      defaultColDef: {
        headerCheckboxSelection: false,
        checkboxSelection: this.isFirstColumn,
        resizable: true
      }
    });
  }

  private initGridData(): void {
    this.agGridConfig.rowData = this.fileInfoItems;
    this.changeDetectorRef.detectChanges();
  }

  private isFirstColumn(params: any): boolean {
    const displayedColumns = params.columnApi.getAllDisplayedColumns();
    const thisIsFirstColumn = displayedColumns[0] === params.column;

    return thisIsFirstColumn;
  }

  private setColumnDefs(): any {
    return [
      {
        headerName: this.getImmediatelyLanguage(
          MassUserCreationFileGridHeader.fileName.text
        ),
        field: MassUserCreationFileGridHeader.fileName.fieldName,
        colId: MassUserCreationFileGridHeader.fileName.colId,
        checkboxSelection: false,
        cellRenderer: 'fileInfoRenderer'
      },
      {
        headerName: this.getImmediatelyLanguage(
          MassUserCreationFileGridHeader.CreatedBy.text
        ),
        field: MassUserCreationFileGridHeader.CreatedBy.fieldName,
        colId: MassUserCreationFileGridHeader.CreatedBy.colId,
        checkboxSelection: false
      },
      {
        headerName: this.getImmediatelyLanguage(
          MassUserCreationFileGridHeader.createdDate.text
        ),
        field: MassUserCreationFileGridHeader.createdDate.fieldName,
        colId: MassUserCreationFileGridHeader.createdDate.colId,
        checkboxSelection: false
      },
      {
        headerName: this.getImmediatelyLanguage(
          MassUserCreationFileGridHeader.numberOfAccountRequest.text
        ),
        field: MassUserCreationFileGridHeader.numberOfAccountRequest.fieldName,
        colId: MassUserCreationFileGridHeader.numberOfAccountRequest.colId,
        checkboxSelection: false
      }
    ];
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterSvc.getValueImmediately(
      'Mass_User_Creation_Panel.Uploaded_Files_Table.Headers.' + columnName
    );
  }

  private calculateToResize(): void {
    const agHeaderElement = document.querySelector('.ag-header');
    const bodyViewPortElement = document.querySelector(
      '.ag-center-cols-container'
    );

    if (!agHeaderElement || !bodyViewPortElement) {
      this.resizeColumnsToFitScreen();

      return;
    }

    setTimeout(() => {
      const screenWidth = screen.width;

      if (screenWidth <= this.fullTableContentWidth) {
        this.resizeColumnsToFitContent(false);

        const afterResizeAgHeaderWidth = agHeaderElement.getBoundingClientRect()
          .width;
        const afterResizeBodyViewPortWidth = bodyViewPortElement.getBoundingClientRect()
          .width;

        if (afterResizeAgHeaderWidth !== afterResizeBodyViewPortWidth) {
          this.resizeColumnsToFitContent(false);
        }
      } else {
        this.resizeColumnsToFitScreen();
      }
    }, this.RESIZE_DELAY_TIME);
  }

  private resizeColumnsToFitContent(isSkipHeader: boolean): void {
    const allColumnIds = [];
    this.agGridConfig.gridColumnApi.getAllColumns().forEach((column) => {
      allColumnIds.push(column.colId);
    });

    this.agGridConfig.gridColumnApi.autoSizeColumns(allColumnIds, isSkipHeader);
  }

  private resizeColumnsToFitScreen(): void {
    this.agGridConfig.gridApi.sizeColumnsToFit();
  }
}
