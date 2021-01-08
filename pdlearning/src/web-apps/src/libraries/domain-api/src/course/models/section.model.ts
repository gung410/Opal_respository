export interface ISectionModel {
  id?: string;
  description: string;
  courseId: string;
  title: string;
  order: number;
  classRunId: string | null;
  creditsAward?: number;
}

export class SectionModel implements ISectionModel {
  public id?: string;
  public description: string = '';
  public courseId: string = '';
  public title: string = '';
  public order: number = 0;
  public classRunId: string | null = null;
  public creditsAward?: number = 0;

  constructor(data?: ISectionModel) {
    if (data) {
      this.id = data.id;
      this.description = data.description;
      this.courseId = data.courseId;
      this.title = data.title;
      this.order = data.order;
      this.classRunId = data.classRunId;
      this.creditsAward = data.creditsAward;
    }
  }
}
