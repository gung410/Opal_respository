import { BasePageComponent, ModuleFacadeService, UploadParameters } from '@opal20/infrastructure';
import { CellClickEvent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FileType, PERSONAL_FILE_SORT_ITEMS, PersonalFileModel, PersonalFileRepository } from '@opal20/domain-api';
import { FileUploaderUtils, OpalDialogService } from '@opal20/common-components';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { PersonalFilePreviewDialogComponent } from '../personal-file-preview-dialog/personal-file-preview-dialog.component';
import { PersonalSpaceSearchTermService } from './../../services/personal-space-search-term.service';
import { FileUploaderHelpers } from '../../helpers/file-uploader.helper';

export enum PersonalFileMode {
  View = 'View',
  Import = 'Import'
}

export enum ImportMechanism {
  SingleFile = 'SingleFile',
  MultipleFile = 'MultipleFile'
}

@Component({
  selector: 'personal-file-list',
  templateUrl: './personal-file-list.component.html'
})
export class PersonalFileListComponent extends BasePageComponent {
  @Input() public mode: PersonalFileMode = PersonalFileMode.View;
  @Input() public importMechanism: ImportMechanism = ImportMechanism.MultipleFile;
  @Input() public maxFileCount: number = 20;
  @Input() public totalFile: number;

  public personalFileMode = PersonalFileMode;
  public textSearch: string = '';
  public submitSearch: string = '';

  // Filter
  @Input()
  public set filterByType(value: FileType) {
    if (!this.initiated || !value) {
      return;
    }

    this._fileType = value;
    this.resetPaging();
    this.loadPersonalFileItems();
  }

  @Input()
  public filterByExtensions: string[] = [];

  // Sort
  @Input()
  public set sortModeId(value: number) {
    if (!this.initiated || !value) {
      return;
    }

    this._sortModeId = value;
    this.resetPaging();
    this.loadPersonalFileItems();
  }

  // Search
  @Input()
  public set search(value: string | undefined) {
    if (!this.initiated) {
      return;
    }

    this._searchText = value;
    this.resetPaging();
    this.loadPersonalFileItems();
  }

  public get search(): string | undefined {
    return this._searchText;
  }

  @Output('viewFileMedia') public viewFileMediaEvent: EventEmitter<unknown> = new EventEmitter();
  @Output('deleteFile') public deleteFile: EventEmitter<PersonalFileModel> = new EventEmitter();

  public gridData: GridDataResult;
  public state: PageChangeEvent;
  public skip: number;
  public pageSize: number = 25;
  public uploadParameters: UploadParameters[] = [];
  private _searchText: string | undefined;
  private _fileType: FileType = FileType.All;
  private _sortModeId: number;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    public searchTermService: PersonalSpaceSearchTermService,
    public apiSv: PersonalFileRepository
  ) {
    super(moduleFacadeService);
    this.initSearchData();
  }

  public loadPersonalFileItems(): void {
    this.updateSearchTearmService();
    this.getFiles();
  }

  public reloadData(): void {
    this.resetPaging();
    this.getFiles();
  }

  public onPageChange(state: PageChangeEvent): void {
    this.skip = state.skip;
    this.loadPersonalFileItems();
  }

  public onItemClick(event: CellClickEvent): void {
    if (
      event.dataItem === undefined ||
      (event.columnIndex !== 0 && event.columnIndex !== 1) ||
      (event.columnIndex === 0 && this.mode === PersonalFileMode.Import)
    ) {
      return;
    }

    this.viewFileMedia(event.dataItem);
  }

  public onDeleteFile(data: PersonalFileModel): void {
    this.moduleFacadeService.modalService.showConfirmMessage('Are you sure you want to delete this file?', () => {
      this.deleteFile.emit(data);
    });
  }

  public getFileTypeDisplay(type: FileType): string {
    return FileUploaderHelpers.getFileTypeDisplay(type);
  }

  public onInit(): void {
    this.resetPaging();
    this.updateSearchTearmService();
    this.getFiles();
  }

  public onCheckItem(checked: boolean, item: PersonalFileModel): void {
    if (checked && this.uploadParameters.length + this.totalFile < this.maxFileCount) {
      const uploadParameter = new UploadParameters();
      uploadParameter.fileName = item.fileName;
      uploadParameter.fileId = item.id;
      uploadParameter.fileExtension = item.fileExtension;
      uploadParameter.fileLocation = item.fileLocation;
      uploadParameter.fileSize = item.fileSize;
      this.uploadParameters.push(uploadParameter);
    } else {
      this.uploadParameters = this.uploadParameters.filter(x => x.fileId !== item.id);
    }
  }

  public checkChecked(item: PersonalFileModel): boolean {
    return this.uploadParameters.findIndex(x => x.fileId === item.id) >= 0;
  }

  public disabledItem(item: PersonalFileModel): boolean {
    return (
      this.totalFile >= this.maxFileCount ||
      (this.uploadParameters.length >= this.maxFileCount && this.uploadParameters.findIndex(x => x.fileId === item.id) < 0) ||
      (this.uploadParameters.length + this.totalFile >= this.maxFileCount && this.uploadParameters.findIndex(x => x.fileId === item.id) < 0)
    );
  }

  public viewFileMedia(mediaFile: PersonalFileModel): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(PersonalFilePreviewDialogComponent);
    const configurationPopup = dialogRef.content.instance as PersonalFilePreviewDialogComponent;
    configurationPopup.personalFileVm = mediaFile;
  }

  private getFiles(): void {
    this.apiSv
      .loadPersonalFiles(
        this.searchTermService.searchText,
        this.searchTermService.state.skip,
        this.pageSize,
        this.searchTermService.filterByExtensions,
        [this.searchTermService.filterByType],
        this.searchTermService.sortDirection,
        this.searchTermService.sortBy
      )
      .subscribe(data => {
        this.gridData = {
          data: data.items,
          total: data.totalCount
        };
      });
  }

  private initSearchData(): void {
    this.pageSize = this.searchTermService.state.take;
    this.skip = this.searchTermService.state.skip;

    if (this.searchTermService.searchText && this.search === undefined) {
      this.search = this.searchTermService.searchText;
    }

    if (this.searchTermService.filterByType) {
      this.filterByType = this.searchTermService.filterByType;
    }
  }

  private resetPaging(): void {
    this.skip = 0;
    this.searchTermService.state.skip = 0;
  }

  private updateSearchTearmService(): void {
    this.searchTermService.state.skip = this.skip;
    this.searchTermService.searchText = this._searchText;
    this.searchTermService.filterByType = this._fileType;
    this.searchTermService.filterByExtensions = this.filterByExtensions;

    const sortMode = PERSONAL_FILE_SORT_ITEMS.find(data => data.id === this._sortModeId);

    if (sortMode) {
      this.searchTermService.sortBy = sortMode.sortMode.sortField;
      this.searchTermService.sortDirection = sortMode.sortMode.sortDirection;
    }
  }
}
