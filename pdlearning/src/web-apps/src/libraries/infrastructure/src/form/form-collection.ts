import { AbstractControl } from '@angular/forms';
import { CustomFormGroup } from './form-group';
import { IFormBuilderDefinition } from './models/form-builder-definition';
import { ModuleFacadeService } from '../services/module-facade.service';
import { Utils } from '../utils/utils';

export class FormCollection {
  private _forms: CustomFormGroup[] = [];

  constructor(private moduleFacadeService: ModuleFacadeService, private getFormBuilderDefinitions: () => IFormBuilderDefinition[]) {}

  /**
   * Return the form collection.
   */
  public get forms(): CustomFormGroup[] {
    return this._forms;
  }

  /**
   * Get the first form of collection.
   */
  public get form(): CustomFormGroup | undefined {
    return this._forms[0];
  }

  /**
   * Get the name of the first form in collection.
   */
  public get formName(): string | undefined {
    return this.form && this.form.name;
  }

  /**
   * Add a form to collection.
   * @param form form group.
   */
  public add(form: CustomFormGroup): void {
    this._forms.push(form);
  }

  public getFormByName(formName: string): CustomFormGroup {
    return this._forms.find(form => form.name === formName);
  }

  public getInvalidForm(controls?: string[]): CustomFormGroup {
    return this._forms.find(form => this.isInvalidFormChecking(form, controls));
  }

  public getPendingForm(controls?: string[]): CustomFormGroup {
    const controlsDic = controls ? Utils.toDictionary(controls) : null;
    return this._forms.find(form => {
      // Just return the first enabled and pending control.
      return Object.keys(form.controls).some(controlName => {
        if (controlsDic != null && controlsDic[controlName] == null) {
          return false;
        }
        const control: AbstractControl = form.get(controlName);

        return control.enabled && control.status === 'PENDING';
      });
    });
  }

  /**
   * Perform dirty checking for each form in collection.
   * @param manualChecking Perform manual checking flag.
   * @param form The specific form that you want to check.
   * @returns 'true' when any form is dirty, otherwise is false.
   */
  public isDirty(form?: CustomFormGroup): boolean {
    if (!form) {
      form = this.form;
    }

    return form.options.manualTrackingChange ? this.performManualDirtyChecking(form) : this.performDefaultDirtyChecking(form);
  }

  public isValid(form?: CustomFormGroup, controls?: string[]): boolean {
    const forms: CustomFormGroup[] = form ? [form] : this._forms;

    return forms.every(f => !this.isInvalidFormChecking(f, controls));
  }

  /**
   * Perform reset a specific form.
   */
  public reset(form: CustomFormGroup): void {
    this.moduleFacadeService.formBuilder.reset(form);
    form.markAsPristine();
    form.markAsUntouched();
    form.setErrors(null);
  }

  /**
   * Perform reset all forms in the collection.
   */
  public resetAll(): void {
    this._forms.forEach(formGroup => this.reset(formGroup));
  }

  /**
   * Perform marks all forms to pristine status.
   */
  public markAllAsPristine(): void {
    this._forms.forEach(form => this.markAsPristine(form));
  }

  /**
   * Perform mark a specific form to pristine status.
   */
  public markAsPristine(form: CustomFormGroup): void {
    this.moduleFacadeService.formBuilder.markAsPristine(form);
  }

  /**
   * Perform update form value controls for all forms.
   * @param value `{ [key: string]: any }`
   */
  public patchAllFormValue(value: { [key: string]: unknown }): void {
    this._forms.forEach(form => this.patchFormValue(value, form));
  }

  /**
   * Perform update form value control.
   * @param value `{ [key: string]: any }`
   * @param form Specific form that you wants perform patch value
   */
  public patchFormValue(value: { [key: string]: unknown }, form: CustomFormGroup): void {
    if (!value) {
      return;
    }

    let hasValueChanged: boolean = false;
    const formValue: { [key: string]: unknown } = {};

    for (const key in value) {
      if (value[key] !== undefined) {
        formValue[key] = value[key];
        hasValueChanged = true;
      }
    }

    if (hasValueChanged) {
      form.patchValue(formValue);
    }
  }

  /**
   * Perform update form value controls for all forms and mark form is pristine.
   * @param value `{ [key: string]: any }`
   */
  public patchInitialAllFormValue(value: { [key: string]: unknown }): void {
    this._forms.forEach(form => this.patchInitialFormValue(value, form));
  }

  /**
   * Perform update form value control and mark form is pristine.
   * @param value `{ [key: string]: any }`
   * @param form specific form that you wants perform patch value
   */
  public patchInitialFormValue(value: { [key: string]: unknown }, form: CustomFormGroup): void {
    if (!value) {
      return;
    }

    let hasValueChanged: boolean = false;
    const formValue: { [key: string]: unknown } = {};

    for (const key in value) {
      if (value[key] !== undefined) {
        formValue[key] = value[key];
        hasValueChanged = true;
      }
    }

    if (hasValueChanged) {
      form.patchValue(formValue);
      this.markAsPristine(form);
    }
  }

  private isInvalidFormChecking(form: CustomFormGroup, controls?: string[]): boolean {
    const controlsDic = controls ? Utils.toDictionary(controls) : null;
    return Object.keys(form.controls).some(controlName => {
      if (controlsDic != null && controlsDic[controlName] == null) {
        return false;
      }
      const control: AbstractControl = form.get(controlName);

      return control.enabled && control.invalid;
    });
  }

  private performManualDirtyChecking(form?: CustomFormGroup): boolean {
    const forms: CustomFormGroup[] = form ? [form] : this._forms;

    return forms.some(f => this.manualCheckFormDirty(f));
  }

  private performDefaultDirtyChecking(form?: CustomFormGroup): boolean {
    const forms: CustomFormGroup[] = form ? [form] : this._forms;

    return forms.some(f => Object.keys(f.controls).some(controlName => this.checkFormControlDirty(f.get(controlName))));
  }

  private manualCheckFormDirty(form: CustomFormGroup): boolean {
    const formBuilderDefinition: IFormBuilderDefinition = this.findFormBuilderDefinition(form.name);

    for (const controlName in form.controls) {
      if (!formBuilderDefinition.controls[controlName].manualTrackChange) {
        continue;
      }

      if (this.checkFormControlDirty(form.get(controlName))) {
        return true;
      }
    }

    return false;
  }

  /**
   * Perform checking dirty for form control which is not disabled.
   * @param control The form control.
   */
  private checkFormControlDirty(control: AbstractControl): boolean {
    return !control.disabled && control.dirty;
  }

  private findFormBuilderDefinition(formName: string): IFormBuilderDefinition | undefined {
    return this.getFormBuilderDefinitions().find(definition => definition.formName === formName);
  }
}
