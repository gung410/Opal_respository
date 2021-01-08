export type ScormPlayerMode = 'local' | 'learn' | 'view';

export interface IScormPlayerParameters {
  key: string;
  displayMode?: ScormPlayerMode;
  contentApiUrl?: string;
  learnerApiUrl?: string;
  cloudFrontUrl?: string;
  accessToken?: string;
  digitalContentId?: string;
  myLectureId?: string;
  myDigitalContentId?: string;
  reinitializePlayer?: boolean;
  fileLocation?: string;
}
