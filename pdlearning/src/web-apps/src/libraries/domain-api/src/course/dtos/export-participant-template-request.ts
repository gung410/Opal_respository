export interface IExportParticipantTemplateRequest {
  fileFormat: ExportParticipantTemplateRequestFileFormat;
}

export enum ExportParticipantTemplateRequestFileFormat {
  CSV = 'Csv',
  Excel = 'Excel'
}
