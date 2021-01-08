import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { BrokenLinkModuleIdentifier, BrokenLinkReportComponentService } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

import { GridDataResult } from '@progress/kendo-angular-grid';

@Component({
  selector: 'broken-link-report-tab',
  templateUrl: './broken-link-report-tab.component.html'
})
export class BrokenLinkReportTabComponent extends BasePageComponent {
  @Input() public stickyDependElement: HTMLElement;

  @Input() public set originalObjectId(v: string) {
    this._originalObjectId = v;
  }

  @Input() public set parentIds(v: string[]) {
    this._parentIds = v;
  }

  @Input() public loadParentIdsFn?: (showSpinner: boolean) => Promise<string[]>;

  @Input() public set loadOnInit(v: boolean) {
    this._loadOnInit = v;
  }

  @Input() public module: BrokenLinkModuleIdentifier;

  public gridData: GridDataResult;
  public _originalObjectId: string;
  public _parentIds: string[];
  public pageNumber: number = 0;
  public pageSize: number = 25;
  private _loadOnInit: boolean = false;

  constructor(public moduleFacadeService: ModuleFacadeService, private brokenLinkReportComponentService: BrokenLinkReportComponentService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    if (this._loadOnInit) {
      this.loadData(true);
    }
  }

  public onPageChange(event: { skip: number }): void {
    this.pageNumber = event.skip;
    this.loadData(false);
  }

  public async loadData(showSpinner: boolean = false): Promise<void> {
    const parentIds = this.loadParentIdsFn != null ? await this.loadParentIdsFn(showSpinner) : this._parentIds;
    return new Promise(resolve => {
      this.brokenLinkReportComponentService
        .loadBrokenLinkReport(this._originalObjectId, parentIds, this.module, this.pageNumber, this.pageSize, showSpinner)
        .subscribe(result => {
          this.gridData = result;
          resolve();
        });
    });
  }
}
