import { AsyncValidatorFn, ValidatorFn } from '@angular/forms';
import { IBusinessValidator, IBusinessValidatorDefinition } from './models/validator-definition';
import { IFormControl as IControlDefinition, IFormControlDefinition } from './models/form-control-definition';
import { filter, map, take } from 'rxjs/operators';

import { CustomFormControl } from './form-control';
import { CustomFormGroup } from './form-group';
import { FormCollection } from './form-collection';
import { IFormBuilderDefinition } from './models/form-builder-definition';
import { IFormTranslationMessageMap } from './models/form-message-map';
import { LangUtils } from '../utils/lang.utils';
import { ModuleFacadeService } from '../services/module-facade.service';

export class FormManager {
  private _businessValidatorDefinition: IBusinessValidatorDefinition;
  private _formBuilderDefinitions: IFormBuilderDefinition[];
  private _formCollection: FormCollection;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    this._formCollection = new FormCollection(moduleFacadeService, this.getFormBuilderDefinitions.bind(this));
    this._businessValidatorDefinition = {};
    this._formBuilderDefinitions = [];
  }

  public get form(): CustomFormGroup {
    return this._formCollection.form;
  }

  public get forms(): CustomFormGroup[] {
    return this._formCollection.forms;
  }

  //#region Form Initialization
  public initializeForm(formBuilderDefinition: IFormBuilderDefinition, createBusinessValidators: () => IBusinessValidator[]): void {
    this._formBuilderDefinitions.push(formBuilderDefinition);
    this.createForm(formBuilderDefinition, formBuilderDefinition.formName);
    this._businessValidatorDefinition = {
      [formBuilderDefinition.formName]: createBusinessValidators()
    };
  }

  public initializeForms(
    formBuilderDefinitions: IFormBuilderDefinition[],
    createBusinessValidatorsDefinition: () => IBusinessValidatorDefinition
  ): void {
    this._formBuilderDefinitions.push(...formBuilderDefinitions);
    formBuilderDefinitions.forEach((definition: IFormBuilderDefinition) => this.createForm(definition, definition.formName || ''));
    this._businessValidatorDefinition = {
      ...this._businessValidatorDefinition,
      ...createBusinessValidatorsDefinition()
    };
  }
  //#endregion

  //#region Form Handling
  public isFormDirty(form?: CustomFormGroup): boolean {
    return this._formCollection.isDirty(form);
  }

  public isFormValid(form?: CustomFormGroup, controls?: string[]): boolean {
    return this._formCollection.isValid(form, controls);
  }

  public resetForm(form?: CustomFormGroup): void {
    return form ? this._formCollection.reset(form) : this._formCollection.resetAll();
  }

  public markAsPristine(form?: CustomFormGroup): void {
    return form ? this._formCollection.markAsPristine(form) : this._formCollection.markAllAsPristine();
  }

  public patchFormValue(value: { [key: string]: unknown }, form?: CustomFormGroup): void {
    return form ? this._formCollection.patchFormValue(value, form) : this._formCollection.patchAllFormValue(value);
  }

  public patchInitialFormValue(value: { [key: string]: unknown }, form?: CustomFormGroup): void {
    return form ? this._formCollection.patchInitialFormValue(value, form) : this._formCollection.patchInitialAllFormValue(value);
  }
  //#endregion

  //#region Form Validation
  public validate(
    onValidateBusinessError: (form: CustomFormGroup) => void,
    onValidateFormError: (form: CustomFormGroup, control: CustomFormControl) => void,
    ignoreValidateForm?: () => boolean,
    controls?: string[]
  ): Promise<boolean> {
    const pendingForm: CustomFormGroup = this._formCollection.getPendingForm(controls);

    if (pendingForm) {
      return pendingForm.statusChanges
        .pipe(
          filter(status => status !== 'PENDING'),
          take(1),
          map(() => {
            return this.handleBusinessValidation(onValidateBusinessError, onValidateFormError, ignoreValidateForm, controls);
          })
        )
        .toPromise();
    } else {
      return Promise.resolve(this.handleBusinessValidation(onValidateBusinessError, onValidateFormError, ignoreValidateForm, controls));
    }
  }
  //#endregion

  //#region Save Data
  public checkAndCancel(isFormDirty: () => boolean, performCancel: () => void): void {
    if (!isFormDirty()) {
      return;
    }

    performCancel();
  }

  public checkAndSave(
    canSave: () => boolean | Promise<boolean>,
    saveData: () => void | Promise<boolean | void>,
    onCannotSaved?: () => unknown
  ): Promise<boolean | void> {
    return Promise.resolve()
      .then(() => this.handleCanSave(canSave))
      .then(result => {
        if (!result && onCannotSaved != null) {
          onCannotSaved();
        }
        return result ? this.handleSaveData(saveData) : Promise.resolve(result);
      })
      .catch(() => Promise.resolve(false));
  }

  private handleCanSave(canSave: () => boolean | Promise<boolean>): Promise<boolean> {
    const result: boolean | Promise<boolean> = canSave();

    if (LangUtils.isPromise(result)) {
      return result;
    } else {
      return Promise.resolve(result);
    }
  }

  private handleSaveData(saveData: () => void | Promise<boolean | void>): Promise<boolean | void> {
    const result: void | Promise<boolean | void> = saveData();

    if (LangUtils.isPromise(result)) {
      return result;
    }

    return Promise.resolve(result);
  }
  //#endregion

  //#region Private Methods
  private getFormBuilderDefinitions(): IFormBuilderDefinition[] {
    return this._formBuilderDefinitions;
  }

  private createForm(formBuilderDefinition: IFormBuilderDefinition, formName: string): void {
    const formControlDefinition: IFormControlDefinition = formBuilderDefinition.controls;
    const controlsConfig: { [name: string]: unknown[] } = {};
    const translationMessageMap: IFormTranslationMessageMap = {};

    for (const controlName in formControlDefinition) {
      if (!formControlDefinition[controlName]) {
        continue;
      }

      const controlDefinition: IControlDefinition = formControlDefinition[controlName];
      controlsConfig[controlName] = [];

      const validators: ValidatorFn[] = [];
      const asyncValidators: AsyncValidatorFn[] = [];

      (controlDefinition.validators || []).forEach(definition => {
        if (definition.isAsync) {
          asyncValidators.push(<AsyncValidatorFn>definition.validator);
        } else {
          validators.push(definition.validator);
        }

        if (definition.message) {
          if (translationMessageMap[controlName] == null) {
            translationMessageMap[controlName] = { [definition.validatorType]: definition.message };
          } else {
            translationMessageMap[controlName] = {
              ...translationMessageMap[controlName],
              ...{ [definition.validatorType]: definition.message }
            };
          }
        }
      });

      controlsConfig[controlName] = [controlDefinition.defaultValue, validators, asyncValidators];
    }

    const form: CustomFormGroup = this.moduleFacadeService.formBuilder.rootGroup(
      formName,
      controlsConfig,
      null,
      translationMessageMap,
      formBuilderDefinition.options,
      formBuilderDefinition.validateByGroupControlNames
    );

    this._formCollection.add(form);
  }

  private handleBusinessValidation(
    onValidateBusinessError: (form: CustomFormGroup) => void,
    onValidateFormError: (form: CustomFormGroup, control: CustomFormControl) => void,
    ignoreValidateForm?: () => boolean,
    controls?: string[]
  ): boolean {
    if (!(ignoreValidateForm != null && ignoreValidateForm()) && !this.isFormValid(null, controls)) {
      this.onValidateError(this._formCollection.getInvalidForm(controls), onValidateFormError, controls);

      return false;
    }

    const invalidForm: CustomFormGroup = this.getBusinessErrorForm();

    if (invalidForm) {
      onValidateBusinessError(invalidForm);

      return false;
    }

    return true;
  }

  private onValidateError(
    form: CustomFormGroup,
    onValidateFormError: (form: CustomFormGroup, control: CustomFormControl) => void,
    controls?: string[]
  ): void {
    this.moduleFacadeService.formBuilder.runValidators(form, controls).subscribe((errorControl: CustomFormControl) => {
      onValidateFormError(form, errorControl);
    });
  }

  private getBusinessErrorForm(): CustomFormGroup {
    if (!this._businessValidatorDefinition) {
      return undefined;
    }

    for (const formName in this._businessValidatorDefinition) {
      const businessValidators: IBusinessValidator[] = this._businessValidatorDefinition[formName];

      if (businessValidators.some(validator => !validator.validate())) {
        return this._formCollection.getFormByName(formName);
      }
    }

    return undefined;
  }
  //#endregion
}
