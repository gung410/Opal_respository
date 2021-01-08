export interface ICourseFundingAndSubsidyReference {
  title: string;
  link: string;
}

export class CourseFundingAndSubsidyReference implements ICourseFundingAndSubsidyReference {
  public title: string;
  public link: string;

  constructor(data?: ICourseFundingAndSubsidyReference) {
    if (data) {
      this.title = data.title;
      this.link = data.link;
    }
  }
}
