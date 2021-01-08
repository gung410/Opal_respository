import { IBusinessValidator, IBusinessValidatorDefinition } from '../form/models/validator-definition';

import { BaseComponent } from './base-component';
import { CustomFormControl } from '../form/form-control';
import { CustomFormGroup } from '../form/form-group';
import { FormManager } from '../form/form-manager';
import { IFormBuilderDefinition } from '../form/models/form-builder-definition';
import { ModuleFacadeService } from '../services/module-facade.service';
import { TranslationMessage } from '../translation/translation.models';

export abstract class BaseFormComponent extends BaseComponent {
  private _formManager: FormManager;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public get form(): CustomFormGroup {
    return this._formManager.form;
  }

  public get forms(): CustomFormGroup[] {
    return this._formManager.forms;
  }

  public get formManager(): FormManager {
    return this._formManager;
  }

  protected get showDefaultCancelMessage(): boolean {
    return true;
  }

  public checkAndSaveWithResult(): Promise<boolean | void> {
    return Promise.resolve()
      .then(() => this._formManager.checkAndSave(this.canSave.bind(this), this.saveData.bind(this), () => this.onCheckToSaveInvalid()))
      .catch();
  }

  public checkAndSave(): void {
    Promise.resolve()
      .then(() => this._formManager.checkAndSave(this.canSave.bind(this), this.saveData.bind(this), () => this.onCheckToSaveInvalid()))
      .catch();
  }

  public checkAndCancel(): void {
    this._formManager.checkAndCancel(this.isFormDirty.bind(this), () => this.performCheckAndCancel());
  }

  public validate(controls?: string[]): Promise<boolean> {
    return this.canSave(controls);
  }

  protected initFormData(): void {
    this._formManager = new FormManager(this.moduleFacadeService);

    const formBuilderDefinition: IFormBuilderDefinition = this.createFormBuilderDefinition();

    if (formBuilderDefinition) {
      this._formManager.initializeForm(formBuilderDefinition, this.createBusinessValidators.bind(this));

      return;
    }

    const formBuilderDefinitions: IFormBuilderDefinition[] = this.createFormBuilderDefinitions();

    if (formBuilderDefinitions) {
      this._formManager.initializeForms(formBuilderDefinitions, this.createBusinessValidatorsDefinition.bind(this));
    }
  }

  protected performCheckAndCancel(): void {
    if (this.showDefaultCancelMessage) {
      this.modalService.showConfirmMessage(
        new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Cancel the data being edited. Are you sure?'),
        this.performCancel.bind(this)
      );
    } else {
      this.performCancel();
    }
  }

  /**
   * This function is for internal use only, please don't override it.
   * @internal FW
   */
  protected internalInit(): void {
    this.initFormData();
    this.initData();
  }

  protected initData(): void {
    // Virtual method
  }

  /**
   * Declare a form builder definition for screen.
   * @returns IFormBuilderDefinition | undefined - Form builder definition
   * @example A simple form
   * return {
   *   formName: 'form',
   *   controls: {
   *     controlA: {
   *       defaultValue: '',
   *       validators: controlsAValidators
   *     },
   *     controlB: {
   *       defaultValue: '',
   *       validators: controlBValidators
   *     }
   *   }
   * };
   */
  protected createFormBuilderDefinition(): IFormBuilderDefinition | undefined {
    return undefined;
  }

  /**
   * Declare multi form builder definitions.
   * @returns IFormBuilderDefinition[] List of form builder definitions
   * @example Multi forms
   * return [
   *   {
   *     formName: 'form1',
   *     controls: {
   *       controlA: {
   *         defaultValue: '',
   *         validators: contorlAValidators
   *       }
   *     }
   *   },
   *   {
   *     formName: 'form2',
   *     controls: {
   *       controlB: {
   *         defaultValue: '',
   *         validators: controlBValidators
   *       }
   *     }
   *   }
   * ];
   */
  protected createFormBuilderDefinitions(): IFormBuilderDefinition[] {
    return [];
  }

  protected createBusinessValidators(): IBusinessValidator[] {
    // Virtual method
    return [];
  }

  protected createBusinessValidatorsDefinition(): IBusinessValidatorDefinition {
    // Virtual method
    return {};
  }

  protected onValidateBusinessError(form?: CustomFormGroup): void {
    // Virtual method
  }

  protected onValidateFormError(form: CustomFormGroup, control: CustomFormControl): void {
    // Virtual method
    control.focus();
  }

  /**
   * Please only use Promise<boolean> type instead of boolean type for new functions.
   * However, we still keep boolean type to backward compatible for developed functions.
   *
   * Note:
   * We add an any type for developed functions have implemented coding as super.canSave().
   * this is temporary supporting and will be remove in the feature.
   * @example
   * Use Promise<boolean> type for async validation checking
   * protected canSave(): Promise<boolean> {
   *  return Promise.resolve(true/false);
   * }
   */
  protected canSave(controls?: string[]): Promise<boolean> {
    // call additionalCanSaveCheck first so that the main can save check is called later
    // to trigger validate controls later => is focused first if invalid
    const additionalCanSaveCheckResult = this.additionalCanSaveCheck(controls);
    return this._formManager
      .validate(
        form => this.onValidateBusinessError(form),
        (form, control) => this.onValidateFormError(form, control),
        () => this.ignoreValidateForm(),
        controls
      )
      .then(_ => {
        return _ ? additionalCanSaveCheckResult : Promise.resolve(_);
      });
  }

  protected ignoreValidateForm(): boolean {
    return false;
  }

  protected onCheckToSaveInvalid(): void {
    // Virtual method
  }

  protected additionalCanSaveCheck(controls?: string[]): Promise<boolean> {
    return Promise.resolve(true);
  }

  protected saveData(): void | Promise<boolean | void> {
    // Virtual method
    return Promise.resolve(true);
  }

  protected performCancel(): void {
    // Virtual method
  }

  // tslint:disable-next-line:no-any
  protected patchInitialFormValue(value: { [key: string]: any }, form?: CustomFormGroup): void {
    this._formManager.patchInitialFormValue(value, form);
  }

  // tslint:disable-next-line:no-any
  protected patchFormValue(value: { [key: string]: any }, form?: CustomFormGroup): void {
    this._formManager.patchFormValue(value, form);
  }

  protected markAsPristine(form?: CustomFormGroup): void {
    this._formManager.markAsPristine(form);
  }

  protected isFormDirty(form?: CustomFormGroup): boolean {
    return this._formManager.isFormDirty(form) || this.isDataDirty();
  }

  /**
   * An additional condition for data change checking purpose (Only use for some specical cases).
   * @override to customize data change logic checking
   * @example drag/drop tree or copy/paste tree ...
   * @returns boolean: true - mark as data changed. otherwise, return false (default case)
   */
  protected isDataDirty(): boolean {
    return false;
  }

  protected isFormValid(form?: CustomFormGroup, controls?: string[]): boolean {
    return this._formManager.isFormValid(form, controls);
  }

  protected resetForm(form?: CustomFormGroup): void {
    this._formManager.resetForm(form);
  }
}
