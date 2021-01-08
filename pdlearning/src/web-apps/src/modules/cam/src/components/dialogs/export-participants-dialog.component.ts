import { BaseComponent, IFilter, ModuleFacadeService } from '@opal20/infrastructure';
import {
  ClassRun,
  ClassRunRepository,
  ClassRunStatus,
  ExportParticipantFileFormatType,
  IExportParticipantRequest,
  RegistrationApiService,
  SearchClassRunType
} from '@opal20/domain-api';
import { Component, Input } from '@angular/core';
import { ExportParticipantsViewModel, SelectParticipantClassRunType } from '../../view-models/export-participant-view.model';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/internal/operators/map';

@Component({
  selector: 'export-participants-dialog',
  templateUrl: './export-participants-dialog.component.html'
})
export class ExportParticipantDialogComponent extends BaseComponent {
  public _courseId: string = '';
  public get courseId(): string {
    return this._courseId;
  }
  @Input() public set courseId(v: string) {
    this._courseId = v;
    if (this.initiated) {
      this.exportParticipantVm.courseId = v;
    }
  }

  public _classRunIds: string[] = [];
  public get classRunIds(): string[] {
    return this._classRunIds;
  }
  @Input() public set classRunIds(v: string[]) {
    this._classRunIds = v;
  }

  public exportParticipantVm: ExportParticipantsViewModel = new ExportParticipantsViewModel();
  public loading: boolean = false;
  public SelectParticipantClassRunType: typeof SelectParticipantClassRunType = SelectParticipantClassRunType;
  public ExportParticipantFileFormatType: typeof ExportParticipantFileFormatType = ExportParticipantFileFormatType;
  public createFetchClassRunSelectFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<ClassRun[]>;
  public dataInitiated: boolean = false;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public classRunRepository: ClassRunRepository,
    public registrationSv: RegistrationApiService
  ) {
    super(moduleFacadeService);
    this.createFetchClassRunSelectFn = this._createFetchClassRunSelectFn();
  }

  public onInit(): void {
    this.exportParticipantVm.courseId = this.courseId;
  }

  public onCancel(): void {
    this.dialogRef.close();
  }
  public onProceed(): void {
    const exportParticipantRequest: IExportParticipantRequest = {
      courseId: this.exportParticipantVm.courseId,
      classRunIds: this.exportParticipantVm.classRunIds,
      fileFormat: this.exportParticipantVm.exportParticipantFileFormat
    };
    this.registrationSv.exportParticipant(exportParticipantRequest).subscribe(_ => this.showNotification());
    this.dialogRef.close();
  }
  private _createFetchClassRunSelectFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<ClassRun[]> {
    const filterClassRun: IFilter = {
      containFilters: [
        {
          field: 'Status',
          values: [ClassRunStatus.Published],
          notContain: false
        }
      ],
      fromToFilters: []
    };
    return (searchText, skipCount, maxResultCount) =>
      this.classRunRepository
        .loadClassRunsByCourseId(
          this.courseId,
          SearchClassRunType.Owner,
          searchText,
          filterClassRun,
          true,
          false,
          skipCount,
          maxResultCount,
          null,
          false
        )
        .pipe(map(_ => _.items));
  }
}
