import { DigitalContentDetailMode, FormDetailMode, StandaloneSurveyDetailMode } from '@opal20/domain-components';
import { FormType, IDigitalContent } from '@opal20/domain-api';

export interface IFormEditorPageNavigationData {
  formId: string | undefined;
  formStatus: FormType | undefined;
  mode: FormDetailMode;
}

export interface IDigitalContentEditorNavigationData {
  id?: string;
  uploadContent?: IDigitalContent;
}

export interface IDigitalContentDetailNavigationData {
  id: string;
  mode: DigitalContentDetailMode;
}

export interface IDigitalContentRepositoryNavigationData {}

export interface IStandaloneSurveyEditorPageNavigationData {
  formId: string | undefined;
  mode: StandaloneSurveyDetailMode;
}
