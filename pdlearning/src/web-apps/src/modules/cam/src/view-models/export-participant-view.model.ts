import { ExportParticipantFileFormatType } from '@opal20/domain-api';

export class ExportParticipantsViewModel {
  private _courseId = '';
  public get courseId(): string {
    return this._courseId;
  }
  public set courseId(courseId: string) {
    this._courseId = courseId;
  }

  private _classunIds = [];
  public get classRunIds(): string[] {
    return this._classunIds;
  }
  public set classRunIds(classRunIds: string[]) {
    this._classunIds = classRunIds;
  }
  private _participantFromClassRunType: SelectParticipantClassRunType = SelectParticipantClassRunType.ParticipantFromClassRun;
  public get participantFromClassRun(): SelectParticipantClassRunType {
    return this._participantFromClassRunType;
  }
  public set participantFromClassRun(participantFromClassRun: SelectParticipantClassRunType) {
    if (this._participantFromClassRunType === participantFromClassRun) {
      return;
    }
    this._participantFromClassRunType = participantFromClassRun;
  }
  private _exportParticipantFileFormat: ExportParticipantFileFormatType = ExportParticipantFileFormatType.CSV;
  public get exportParticipantFileFormat(): ExportParticipantFileFormatType {
    return this._exportParticipantFileFormat;
  }
  public set exportParticipantFileFormat(exportParticipantFileFormat: ExportParticipantFileFormatType) {
    if (this._exportParticipantFileFormat === exportParticipantFileFormat) {
      return;
    }
    this._exportParticipantFileFormat = exportParticipantFileFormat;
  }

  public setParticipantFromClassRun(value: SelectParticipantClassRunType): void {
    this.participantFromClassRun = value;
  }

  public checkParticipantFromClassRun(): boolean {
    return this.participantFromClassRun === SelectParticipantClassRunType.ParticipantFromClassRun;
  }

  public checkParticipantFromSelectedClassRun(): boolean {
    return this.participantFromClassRun === SelectParticipantClassRunType.ParticipantFromSelectedClassRun;
  }

  public setExportParticipantFileFormat(value: ExportParticipantFileFormatType): void {
    this.exportParticipantFileFormat = value;
  }

  public checkExportParticipantCSVFileFormat(): boolean {
    return this.exportParticipantFileFormat === ExportParticipantFileFormatType.CSV;
  }
  public checkExportParticipanExcelFileFormat(): boolean {
    return this.exportParticipantFileFormat === ExportParticipantFileFormatType.Excel;
  }
}
export enum SelectParticipantClassRunType {
  ParticipantFromClassRun = 'ParticipantFromClassRun',
  ParticipantFromSelectedClassRun = 'ParticipantFromSelectedClassRun'
}
