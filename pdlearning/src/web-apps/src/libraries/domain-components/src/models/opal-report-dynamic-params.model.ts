export interface IOpalReportDynamicParams {
  reportName: string;
  reportDisplayName?: string;
  schema?: OpalReportDynamicParamsSchema;
  viewName?: string;
  select?: string;
  header?: string;
  filter?: string;
  colPatterns_0?: string;
  colPatterns_1?: string;
  colPatterns_2?: string;
  colformats_0?: string;
  colformats_1?: string;
  colformats_2?: string;
}

export enum OpalReportDynamicParamsSchema {
  external = 'external'
}
