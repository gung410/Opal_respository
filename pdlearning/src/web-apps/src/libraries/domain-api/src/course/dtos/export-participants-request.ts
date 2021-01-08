export interface IExportParticipantRequest {
  courseId: string;
  classRunIds: string[];
  fileFormat: ExportParticipantFileFormatType;
}
export enum ExportParticipantFileFormatType {
  Excel = 'Excel',
  CSV = 'Csv'
}
