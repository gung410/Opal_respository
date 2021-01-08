import { environment } from 'app-environments/environment';
import { Identity } from 'app-models/common.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { ObjectiveInfo } from '../../shared/models/assessment.model';

export const columnDefsConfig = [
  {
    headerName: 'Id',
    field: 'id',
    hide: true,
    suppressToolPanel: true,
  },
  {
    headerName: 'Name',
    field: 'name',
    cellRenderer: 'codeCellRenderer',
    headerCheckboxSelection: true,
    headerCheckboxSelectionFilteredOnly: true,
    minWidth: 500,
    checkboxSelection: true,
  },
  { headerName: 'Start Year', field: 'startYear', minWidth: 130 },
  { headerName: 'End Year', field: 'endYear', minWidth: 130 },
  {
    headerName: 'ResultIdentity',
    field: 'resultIdentity',
    hide: true,
    suppressToolPanel: true,
  },
];

export class StrategicThrustToDTO {
  objectiveInfo: ObjectiveInfo;
  answer: any;
  errorIfExistingResult: boolean = false;
  forceCreateResult: boolean = true;
  resultIdentity: any;
  startDate: string;
  dueDate: string;
  constructor(metadata: Partial<ParamMetadata>) {
    this.objectiveInfo = {
      identity: new Identity({
        archetype: metadata.cxSurveyjsExtendedService.getVariable(
          SurveyVariableEnum.currentDepartment_archetype
        ),
        customerId: environment.CustomerId,
        ownerId: environment.OwnerId,
        id: metadata.currentDepartmentId,
      }),
    };
    this.answer = metadata.answerJsonForm;
    const startDate = new Date(
      Date.UTC(this.answer.startYear, 0, 1)
    ).toISOString();
    const dueDate = new Date(
      Date.UTC(this.answer.endYear, 11, 31)
    ).toISOString();
    this.startDate = this.answer.startYear = startDate;
    this.dueDate = this.answer.endYear = dueDate;
    if (!metadata.isCreate && metadata.objectIdentity) {
      this.forceCreateResult = false;
      this.resultIdentity = metadata.objectIdentity.resultIdentity;
    }
  }
  isCreating(): boolean {
    return !this.resultIdentity || !this.resultIdentity.id;
  }
}

export class ParamMetadata {
  answerJsonForm: any;
  currentDepartmentId: number;
  cxSurveyjsExtendedService: CxSurveyjsExtendedService;
  isCreate: boolean;
  objectIdentity?: any;
  constructor(metadata: Partial<ParamMetadata>) {
    this.answerJsonForm = metadata.answerJsonForm;
    this.currentDepartmentId = metadata.currentDepartmentId;
    this.cxSurveyjsExtendedService = metadata.cxSurveyjsExtendedService;
    this.isCreate = metadata.isCreate;
    this.objectIdentity = metadata.objectIdentity;
  }
}

export class StrategicThrust {
  public id: string;
  public name: string;
  public startYear: string;
  public endYear: string;
  public description: string;
  public resultIdentity: {};
  constructor(data: any) {
    if (data && data.answer) {
      this.id = data.resultIdentity.id;
      this.name = data.answer.name;
      this.startYear = ParseISOToYear(data.answer.startYear);
      this.endYear = ParseISOToYear(data.answer.endYear);
      this.description = data.answer.description;
      this.resultIdentity = data.resultIdentity;
    }
  }

  public formatIndex(): {} {
    if (this.id) {
      return {
        id: this.id,
        name: this.name,
        startYear: this.startYear,
        endYear: this.endYear,
        description: this.description,
        resultIdentity: this.resultIdentity,
      };
    }

    return;
  }
}

export function ParseISOToYear(isoDate: string): string {
  if (isoDate) {
    return isoDate.substr(0, 4);
  }

  return null;
}
