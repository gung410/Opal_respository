import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  FormModel,
  IRevertVersionTrackingResult,
  IVersionTracking,
  VersionTrackingApiService,
  VersionTrackingComponentService,
  VersionTrackingType,
  VersionTrackingViewModel
} from '@opal20/domain-api';

import { CompareVersionContentDialogComponent } from '../../dialogs/compare-version-content-dialog.component';
import { CompareVersionFormDialogComponent } from '../../dialogs/compare-version-form-dialog.component';
import { CompareVersionStandaloneSurveyDialogComponent } from '../../dialogs/compare-version-standalone-survey-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { GridDataResult } from '@progress/kendo-angular-grid';

@Component({
  selector: 'audit-log-tab',
  templateUrl: './audit-log-tab.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AuditLogTabComponent extends BasePageComponent {
  @Input() public formData: FormModel;

  @Input()
  public set originalObjectId(v: string) {
    this._originalObjectId = v;
    if (this._originalObjectId) {
      this.loadData(false);
      this.loadActiveVersion(false);
    }
  }
  @Input()
  public activeObjectId: string;

  @Input()
  public allowRevert: boolean = true;

  @Input()
  public set versionTrackingType(v: VersionTrackingType) {
    this._versionTrackingType = v;
    this.versionTrackingApiService.initApiService(v);
  }

  @Output()
  public revertChange: EventEmitter<IRevertVersionTrackingResult> = new EventEmitter<IRevertVersionTrackingResult>();

  public pageNumber: number = 0;
  public pageSize: number = 25;

  public gridView: GridDataResult;
  public revertResult: IRevertVersionTrackingResult;

  public revertableVersions: IVersionTracking[];
  public activeVersion: IVersionTracking;
  public selectedRevertVersion: IVersionTracking;
  public isReverting: boolean = false;
  public showUndoBtn: boolean = true;
  public dialogRef: DialogRef;
  public comparisonItems: VersionTrackingViewModel[] = [];
  public revertPlaceholderItem: IVersionTracking;
  private _versionTrackingType: VersionTrackingType;
  private _originalObjectId: string = '';

  @ViewChild('revertDialogTemplate', { static: false })
  private revertDialogTemplate: TemplateRef<unknown>;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private versionTrackingApiService: VersionTrackingApiService,
    private versionTrackingComponentService: VersionTrackingComponentService
  ) {
    super(moduleFacadeService);
    this.initRevertDropdownPlaceholder();
  }

  public itemDisabled(itemArgs: { dataItem: IVersionTracking; index: number }): boolean {
    return itemArgs.dataItem.isDisable;
  }

  public openModalRevertVersion(): void {
    this.dialogRef = this.moduleFacadeService.dialogService.open({
      content: this.revertDialogTemplate
    });
  }

  public openCompareDialog(): void {
    this.comparisonItems = this.comparisonItems.sort((item1, item2) => {
      return new Date(item1.createdDate).getTime() - new Date(item2.createdDate).getTime();
    });

    switch (this._versionTrackingType) {
      case VersionTrackingType.DigitalContent:
        this.openCompareVersionContentDialog();
        break;
      case VersionTrackingType.Form:
        this.openCompareVersionFormDialog();
        break;
      case VersionTrackingType.LnaForm:
        this.openCompareVersionLnaFormDialog();
        break;
    }
  }

  public undoRevert(revertVersionId?: string): void {
    const version = this.gridView.data.find(v => v.id && v.id === revertVersionId);
    this.showUndoBtn = false;
    this.doRevertVersion(version);
  }

  public onClickRevertVersion(): void {
    const version = this.revertableVersions.find(v => v.id && v.id === this.selectedRevertVersion.id);
    this.showUndoBtn = true;
    this.doRevertVersion(version);
  }

  public doRevertVersion(version?: IVersionTracking): void {
    if (version) {
      this.isReverting = true;
      this.versionTrackingApiService
        .revertVersion(
          {
            currentActiveId: this.activeObjectId,
            revertFromRecordId: version.revertObjectId,
            objectType: version.objectType,
            versionTrackingId: version.id
          },
          true
        )
        .then(result => {
          this.revertResult = result;
          this.revertChange.emit(result);
          this.dialogRef.close();
          this.loadData(false);
          this.loadRevertableVersions(false);
          this.loadActiveVersion(false);
          this.selectedRevertVersion = null;
          this.isReverting = false;
        });
    }
  }

  public closeAlertRevertVersion(): void {
    this.revertResult = null;
  }

  public onPageChange(event: { skip: number }): void {
    this.pageNumber = event.skip;
    this.loadData(false);
  }

  public loadData(showSpinner: boolean = false): void {
    this.comparisonItems = [];
    this.versionTrackingComponentService
      .loadVersionTrackings(this._originalObjectId, this.pageNumber, this.pageSize, showSpinner)
      .subscribe(result => {
        this.gridView = result;
      });
  }

  public loadRevertableVersions(showSpinner: boolean = true): void {
    this.versionTrackingApiService.getRevertableVersions(this._originalObjectId, showSpinner).then(result => {
      this.revertableVersions = [this.revertPlaceholderItem].concat(result);
      this.disableRevertCurrentVersion();
    });
  }

  public loadActiveVersion(showSpinner: boolean = true): void {
    this.versionTrackingApiService.getActiveVersion(this._originalObjectId, showSpinner).then(result => {
      this.activeVersion = result;
      this.disableRevertCurrentVersion();
    });
  }

  public onCheckItem(checked: boolean, item: VersionTrackingViewModel): void {
    this.comparisonItems = checked ? this.comparisonItems.concat(item) : this.comparisonItems.filter(x => x.id !== item.id);
  }

  public disabledComparisonCheck(item: VersionTrackingViewModel): boolean {
    return !this.comparisonItems.includes(item) && this.comparisonItems.length === 2;
  }

  public comparisonChecked(item: VersionTrackingViewModel): boolean {
    return this.comparisonItems.includes(item);
  }

  public get disabledComparisonBtn(): boolean {
    return this.comparisonItems.length < 2;
  }

  public get canRevertVersion(): boolean {
    return (
      this.allowRevert &&
      this.revertableVersions &&
      this.revertableVersions.length &&
      this.activeVersion &&
      (this.activeVersion.majorVersion >= 2 || (this.activeVersion.majorVersion >= 1 && this.activeVersion.minorVersion > 0))
    );
  }

  private disableRevertCurrentVersion(): void {
    if (this.revertableVersions && this.activeVersion) {
      this.selectedRevertVersion = this.revertableVersions[0];
      this.revertableVersions.forEach(version => {
        version.isDisable = this.activeVersion.versionString === version.versionString || !version.id;
      });
    }
  }

  private initRevertDropdownPlaceholder(): void {
    this.revertPlaceholderItem = {
      id: null,
      canRollback: false,
      changedByUserId: null,
      createdDate: null,
      majorVersion: null,
      minorVersion: null,
      objectType: null,
      originalObjectId: null,
      versionString: this.translate('Select...')
    };
  }

  private openCompareVersionContentDialog(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: CompareVersionContentDialogComponent });
    const compareVersionContentDialog = dialogRef.content.instance as CompareVersionContentDialogComponent;
    compareVersionContentDialog.oldVersionTrackingVm = this.comparisonItems[0];
    compareVersionContentDialog.newVersionTrackingVm = this.comparisonItems[1];
  }

  private openCompareVersionFormDialog(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: CompareVersionFormDialogComponent });
    const configurationPopup = dialogRef.content.instance as CompareVersionFormDialogComponent;
    configurationPopup.oldVersionTrackingVm = this.comparisonItems[0];
    configurationPopup.newVersionTrackingVm = this.comparisonItems[1];
    configurationPopup.type = this.formData.type;
  }

  private openCompareVersionLnaFormDialog(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: CompareVersionStandaloneSurveyDialogComponent });
    const configurationPopup = dialogRef.content.instance as CompareVersionStandaloneSurveyDialogComponent;
    configurationPopup.oldVersionTrackingVm = this.comparisonItems[0];
    configurationPopup.newVersionTrackingVm = this.comparisonItems[1];
  }
}
