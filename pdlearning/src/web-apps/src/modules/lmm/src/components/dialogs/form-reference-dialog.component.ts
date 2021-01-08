import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, ViewEncapsulation } from '@angular/core';
import { FormApiService, FormStatus, IDigitalContent } from '@opal20/domain-api';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'form-reference-dialog',
  templateUrl: './form-reference-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})
export class FormReferenceDialogComponent extends BaseFormComponent {
  public gridView: GridDataResult;
  public pageSize: number = 10;
  public pageNumber: number = 0;
  public searchText: string = '';
  constructor(protected moduleFacadeService: ModuleFacadeService, private dialogRef: DialogRef, private formApiService: FormApiService) {
    super(moduleFacadeService);

    this.loadForms();
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

  public addItem(item: IDigitalContent): void {
    this.dialogRef.close(item);
  }

  public onGridPageChange(event: PageChangeEvent): void {
    this.pageNumber = event.skip;
    this.loadForms();
  }

  public onSearchForms(): void {
    this.pageNumber = 0;
    this.loadForms();
  }

  public loadForms(): Promise<void> {
    return this.formApiService
      .searchForm(this.pageNumber, this.pageSize, this.searchText, [FormStatus.Published, FormStatus.Approved, FormStatus.ReadyToUse], true)
      .toPromise()
      .then(result => {
        this.gridView = {
          data: result.items,
          total: result.totalCount
        };
      });
  }

  protected performCancel(): void {
    this.dialogRef.close();
  }

  protected saveData(): void {
    this.dialogRef.close();
  }
}
