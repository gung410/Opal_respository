import * as Survey from 'survey-angular';
import { Injectable } from '@angular/core';
import { CxSurveyjsVariable } from './cx-surveyjs.model';
// tslint:disable:variable-name
@Injectable({
  providedIn: 'root'
})
export class CxSurveyjsService {
  private _variables: CxSurveyjsVariable[] = [];
  private _locale: string;

  public initVariables(variables: CxSurveyjsVariable[]) {
    if (this._variables && this._variables.length) { throw new Error('Variables can only be initialized once.'); }
    this._variables = variables;
  }

  public setVariables(variableObject: object) {
    for (const key in variableObject) {
      if (variableObject.hasOwnProperty(key)) {
        // Find existing variable to update the new value or add new one if doesn't exist.
        const newValue = variableObject[key];
        const existingVariable = this._variables.find(p => p.name === key);
        if (existingVariable) {
          existingVariable.value = newValue;
        } else {
          this._variables.push(new CxSurveyjsVariable({ name: key, value: newValue }));
        }
      }
    }
  }

  public get variables() {
    return this._variables;
  }

  public clearVariables() {
    this._variables = [];
  }

  public setLocale(locale: string) {
    if (this._locale) { throw new Error('Locale has been defined.'); }
    this._locale = locale;
  }

  public get locale() {
    return this._locale;
  }

  constructor() {
    this.overrideEnglishLanguage();
  }

  private overrideEnglishLanguage() {
    const englishLanguageData = Survey.surveyLocalization.locales.en;
    englishLanguageData.optionsCaption = 'Select...'; // Origin: 'Choose...'
  }
}
