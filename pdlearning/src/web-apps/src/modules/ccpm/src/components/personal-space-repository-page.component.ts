import { BasePageComponent, ModuleFacadeService, NotificationType, UploadParameters } from '@opal20/infrastructure';
import {
  BatchUploadFilesDialogComponent,
  FileUploaderHelpers,
  PersonalFileListComponent,
  PersonalFilePreviewDialogComponent,
  PersonalSpaceSearchTermService
} from '@opal20/domain-components';
import { Component, Input, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  FileType,
  ICreatePersonalFilesRequest,
  PERSONAL_FILE_SORT_ITEMS,
  PersonalFileModel,
  PersonalSpaceApiService,
  PersonalSpaceModel
} from '@opal20/domain-api';
import { FileUploaderUtils, OpalDialogService } from '@opal20/common-components';

import { DialogRef } from '@progress/kendo-angular-dialog';

export enum PersonalRepositoryMode {
  View = 'View',
  Import = 'Import'
}

@Component({
  selector: 'personal-space-repository-page',
  templateUrl: './personal-space-repository-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class PersonalSpaceRepositoryPageComponent extends BasePageComponent {
  @Input() public mode: PersonalRepositoryMode = PersonalRepositoryMode.View;
  @Input() public personalSpaceVm: PersonalSpaceModel = undefined;

  @ViewChild('personalFileList', { static: false }) public fileList: PersonalFileListComponent;

  public textSearch: string = '';
  public submitSearch: string = '';

  public sortItems = PERSONAL_FILE_SORT_ITEMS;

  public fileType: FileType = FileType.All;
  public fileTypeItems: IDataItem[] = [
    {
      text: this.translateCommon('All'),
      value: FileType.All
    },
    {
      text: this.translateCommon('Document'),
      value: FileType.Document
    },
    {
      text: this.translateCommon('Digital Graphic'),
      value: FileType.DigitalGraphic
    },
    {
      text: this.translateCommon('Audio'),
      value: FileType.Audio
    },
    {
      text: this.translateCommon('Video'),
      value: FileType.Video
    },
    {
      text: this.translateCommon('Learning Package'),
      value: FileType.LearningPackage
    }
  ];

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public searchTermService: PersonalSpaceSearchTermService,
    public opalDialogService: OpalDialogService,
    public personalSpaceApiService: PersonalSpaceApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.initTextSearch();
    this.loadPersonalSpaceInfo();
  }

  public onViewFileMedia(mediaFile: PersonalFileModel): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(PersonalFilePreviewDialogComponent);
    const configurationPopup = dialogRef.content.instance as PersonalFilePreviewDialogComponent;
    configurationPopup.personalFileVm = mediaFile;
  }

  public openUploadDialog(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(BatchUploadFilesDialogComponent);
    const configurationPopup = dialogRef.content.instance as BatchUploadFilesDialogComponent;

    configurationPopup.uploadFolder = 'digital-contents';
    configurationPopup.icon = 'assets/images/icons/add-file.svg';
    configurationPopup.settings.extensions = FileUploaderUtils.uploadContentAllowedExtensions;

    dialogRef.result.subscribe((result: UploadParameters[]) => {
      if (result && result.length > 0) {
        this.processUploadPersonalFile(result).then(() => {
          this.reloadPersonalSpaceInfo();
          this.fileList.reloadData();
          this.showNotification(`${result.length === 1 ? 'File' : 'Files'} successfully updated`, NotificationType.Success);
        });
      }
    });
  }

  public onDeleteFile(data: PersonalFileModel): void {
    this.personalSpaceApiService.deletePersonalFile(data.id, true).then(() => {
      this.reloadPersonalSpaceInfo();
      this.fileList.reloadData();
      this.showNotification(`File successfully deleted`, NotificationType.Success);
    });
  }

  public onSubmitSearch(): void {
    this.submitSearch = this.textSearch.slice();
    if (this.searchTermService.searchText !== this.submitSearch) {
      this.searchTermService.searchText = this.submitSearch;
      this.searchTermService.state.skip = 0;
    }
  }

  private initTextSearch(): void {
    if (this.searchTermService.searchText) {
      this.textSearch = this.searchTermService.searchText;
      this.onSubmitSearch();
    }

    this.fileType = this.searchTermService.filterByType ? this.searchTermService.filterByType : FileType.All;
  }

  private loadPersonalSpaceInfo(): void {
    this.personalSpaceApiService.getPersonalSpaceInfo().then(personalSpaceInfo => {
      this.personalSpaceVm = new PersonalSpaceModel(personalSpaceInfo);
    });
  }

  private reloadPersonalSpaceInfo(): void {
    this.loadPersonalSpaceInfo();
  }

  private processUploadPersonalFile(fileParameters: UploadParameters[]): Promise<void> {
    const createPersonalFileRequest = this.createUploadFileParams(fileParameters);
    return this.personalSpaceApiService.createPersonalFiles(createPersonalFileRequest);
  }

  private createUploadFileParams(fileParameter: UploadParameters[]): ICreatePersonalFilesRequest {
    const toUploadFileParams = fileParameter.map(item => {
      return <PersonalFileModel>{
        fileName: item.fileName,
        fileType: FileUploaderHelpers.getFileType(item.fileExtension),
        fileSize: item.fileSize,
        fileExtension: item.fileExtension,
        fileLocation: item.fileLocation
      };
    });

    return <ICreatePersonalFilesRequest>{
      personalFiles: toUploadFileParams
    };
  }
}
