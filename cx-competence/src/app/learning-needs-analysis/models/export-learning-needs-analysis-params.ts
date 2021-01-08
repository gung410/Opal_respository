import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';

export class ExportLearningNeedsAnalysisParams {
  employeeFilter: FilterParamModel;
  exportOptions: ExportLearningNeedsAnalysisOptions;
  emailOption?: EmailOption;
  sendEmail?: boolean;
  constructor(data?: Partial<ExportLearningNeedsAnalysisParams>) {
    if (!data) {
      return;
    }

    this.employeeFilter = data.employeeFilter;
    this.exportOptions = data.exportOptions;
    this.emailOption = data.emailOption;
    this.sendEmail = data.sendEmail;
  }
}

export class ExportLearningNeedsAnalysisOptions {
  careerAspirationEvaluation?: boolean;
  competencyEvaluation?: boolean;
  learningAreaEvaluation?: boolean;

  constructor(data?: Partial<ExportLearningNeedsAnalysisOptions>) {
    if (!data) {
      return;
    }

    this.careerAspirationEvaluation = data.careerAspirationEvaluation;
    this.competencyEvaluation = data.competencyEvaluation;
    this.learningAreaEvaluation = data.learningAreaEvaluation;
  }
}

export class EmailOption {
  subject?: string;
  downloadUrl: string;

  constructor(data?: Partial<EmailOption>) {
    if (!data) {
      return;
    }

    this.subject = data.subject;
    this.downloadUrl = data.downloadUrl;
  }
}

export class AcceptExportDto {
  message: string;
  fileName: string;
  downloadUrl: string;
  filePath: string;
}
