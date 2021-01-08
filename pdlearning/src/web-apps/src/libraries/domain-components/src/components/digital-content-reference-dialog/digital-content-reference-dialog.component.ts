import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, ViewEncapsulation } from '@angular/core';
import { ContentApiService, DigitalContentStatus, IDigitalContent } from '@opal20/domain-api';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'digital-content-reference-dialog',
  templateUrl: './digital-content-reference-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalContentReferenceDialog extends BaseFormComponent {
  public gridView: GridDataResult;
  public pageSize: number = 10;
  public pageNumber: number = 0;
  public searchText: string = '';
  public filterByExtensions: string[] | undefined;
  public filterByStatuses: DigitalContentStatus[] | undefined;
  public withinDownloadableContent: boolean | undefined;
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    private digitalContentShareService: ContentApiService
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('class.column')
  public getContentClass(): boolean {
    return true;
  }

  @HostBinding('class.lmm-reference-dialog')
  public getDialogClass(): boolean {
    return true;
  }

  public cancel(): void {
    this.performCancel();
  }

  public onSearchDigitalContents(): void {
    this.pageNumber = 0;
    this.loadDigitalContents();
  }

  public addItem(item: IDigitalContent): void {
    this.dialogRef.close(item);
  }

  public onGridPageChange(event: PageChangeEvent): void {
    this.pageNumber = event.skip;
    this.loadDigitalContents();
  }

  protected onInit(): void {
    this.loadDigitalContents();
  }

  protected performCancel(): void {
    this.dialogRef.close();
  }

  protected saveData(): void {
    this.dialogRef.close();
  }

  private loadDigitalContents(): Promise<void> {
    return this.digitalContentShareService
      .searchDigitalContent(
        {
          searchText: this.searchText,
          queryMode: null,
          pagedInfo: {
            skipCount: this.pageNumber,
            maxResultCount: this.pageSize
          },
          includeContentForImportToCourse: true,
          filterByStatus:
            this.filterByStatuses == null
              ? [DigitalContentStatus.Published, DigitalContentStatus.Approved, DigitalContentStatus.ReadyToUse]
              : this.filterByStatuses,
          withinCopyrightDuration: true,
          withinDownloadableContent: this.withinDownloadableContent,
          filterByExtensions: this.filterByExtensions
        },
        true
      )
      .then(result => {
        this.gridView = {
          data: result.items,
          total: result.totalCount
        };
      });
  }
}
