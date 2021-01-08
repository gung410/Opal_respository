export enum OutstandingTaskType {
  Course = 'Course',
  Assignment = 'Assignment',
  DigitalContent = 'DigitalContent',
  Microlearning = 'Microlearning',
  StandaloneForm = 'StandaloneForm'
}

export enum OutstandingTaskStatus {
  NotStarted = 'NotStarted',
  Continue = 'Continue'
}

export type FormTye = 'Quiz' | 'Survey' | 'Poll' | 'Onboarding';

export interface IOutstandingTask {
  courseId: string;
  assignmentId: string;
  digitalContentId: string;
  formId: string;
  name: string;
  startDate?: Date;
  dueDate?: Date;
  progress?: number;
  status: OutstandingTaskStatus;
  type: OutstandingTaskType;
  fileExtension: string;
  formType?: FormTye;
}

export class OutstandingTask implements IOutstandingTask {
  public courseId: string;
  public assignmentId: string;
  public digitalContentId: string;
  public formId: string;
  public name: string;
  public startDate?: Date;
  public dueDate?: Date;
  public progress?: number;
  public status: OutstandingTaskStatus;
  public type: OutstandingTaskType;
  public fileExtension: string;
  public formType?: FormTye;

  constructor(data?: IOutstandingTask) {
    if (data == null) {
      return;
    }
    this.courseId = data.courseId;
    this.assignmentId = data.assignmentId;
    this.digitalContentId = data.digitalContentId;
    this.name = data.name;
    this.startDate = data.startDate ? new Date(data.startDate) : undefined;
    this.dueDate = data.dueDate ? new Date(data.dueDate) : undefined;
    this.progress = data.progress;
    this.status = data.status;
    this.type = data.type;
    this.fileExtension = data.fileExtension;
    this.formId = data.formId;
    this.formType = data.formType;
  }

  public get isNotStarted(): boolean {
    return this.status === OutstandingTaskStatus.NotStarted;
  }

  public get isAssignmentTask(): boolean {
    return this.type === OutstandingTaskType.Assignment;
  }

  public get isDigitalContentTask(): boolean {
    return this.type === OutstandingTaskType.DigitalContent;
  }

  public get isStandaloneFormTask(): boolean {
    return this.type === OutstandingTaskType.StandaloneForm;
  }

  public get isMicrolearningTask(): boolean {
    return this.type === OutstandingTaskType.Microlearning;
  }

  public get isCourseTask(): boolean {
    return this.type === OutstandingTaskType.Course;
  }
}
