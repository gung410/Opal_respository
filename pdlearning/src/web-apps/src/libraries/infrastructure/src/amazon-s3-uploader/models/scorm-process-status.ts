export interface IScormProcessStatusResult {
  status: string;
}

export enum ScormProcessStatus {
  Failure = 'Failure',
  Completed = 'Completed',
  Extracting = 'Extracting',
  ExtractingFailure = 'ExtractingFailure',
  Processing = 'Processing',
  Timeout = 'Timeout',
  Invalid = 'Invalid'
}
