import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CxLazySurveyjsService {
  private formLength: number = 0;
  private formIds: string[] = [];

  constructor() {}

  /**
   * Init the new container which controlling when to render a new form.
   */
  init(): void {
    this.formLength = 0;
    this.formIds = [];
  }

  /**
   * Adds a new form into the container and returns the delay time of rendering the form.
   * @param formId The survey js form id which is usually the object's identifier. e.g: ResultId
   */
  addNewForm(formId: string): number {
    if (!this.formIds.includes(formId)) {
      this.formIds.push(formId);
      this.formLength++;
    }

    // Estimated each form needs 1 second to render it should be waited for 1 second before adding the next form.
    // tslint:disable-next-line:no-magic-numbers
    return this.formLength * 1000;
  }
}
