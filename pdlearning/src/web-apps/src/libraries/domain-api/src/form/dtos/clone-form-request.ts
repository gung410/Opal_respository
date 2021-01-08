export class CloneFormRequest {
  public formId: string;
  public newTitle: string;

  constructor(formId: string, newTitle: string) {
    this.formId = formId;
    this.newTitle = newTitle;
  }
}
