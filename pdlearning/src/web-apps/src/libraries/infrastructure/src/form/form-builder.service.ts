import {
  AbstractControl,
  AbstractControlOptions,
  AsyncValidatorFn,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn
} from '@angular/forms';
import { ITranslationParams, TranslationMessage } from '../translation/translation.models';
import { Observable, of } from 'rxjs';
import { filter, map, mergeMap, take, tap } from 'rxjs/operators';

import { CustomFormControl } from './form-control';
import { CustomFormGroup } from './form-group';
import { FormOptions } from './form-options';
import { GlobalSpinnerService } from '../spinner/global-spinner.service';
import { GlobalTranslatorService } from '../translation/global-translator.service';
import { IFormTranslationMessageMap } from './models/form-message-map';
import { Injectable } from '@angular/core';
import { Utils } from '../utils/utils';
import { ValidatorType } from './models/validator-definition';

@Injectable()
export class FormBuilderService extends FormBuilder {
  public static runValidators(
    translator: GlobalTranslatorService,
    form: CustomFormGroup,
    validateControls?: string[]
  ): Observable<AbstractControl> {
    const controls: AbstractControl[] = this.getControlsAsArray(form, validateControls);

    controls.forEach(p => {
      if (p instanceof CustomFormControl) {
        p.hideTooltip();
      }
    });

    return controls
      .reduce<Observable<AbstractControl>>(
        (previousValue, currentValue) =>
          previousValue.pipe(
            mergeMap(value =>
              value ? of(value) : this.runControlValidators(currentValue).pipe(map(valid => (valid ? null : currentValue)))
            )
          ),
        of(null)
      )
      .pipe(
        tap(control => {
          // Run validate method will returned a control which has errors.
          if (control) {
            this.showError(translator, <CustomFormControl>control, false);
          }
        })
      );
  }

  public static getControlName(control: CustomFormControl): string {
    const controls: { [key: string]: AbstractControl } | AbstractControl[] = control.parent.controls;

    return Object.keys(controls).find(name => {
      return Object.is(control, controls[name]);
    });
  }

  public static showControlError(translator: GlobalTranslatorService, control: CustomFormControl): boolean {
    const controlName: string = FormBuilderService.getControlName(control);
    const errors: ValidationErrors | undefined = control.errors;

    if (errors === undefined || errors === null) {
      control.hideTooltip();
      return false;
    }
    if (errors.tooltipMessage) {
      control.showTooltip(errors.tooltipMessage);
    } else {
      /**
       * TODO: Consider finding the priority error in the collection by @see FormOptions.validationErrorPriority
       */
      const type: ValidatorType = Object.keys(errors)[0];
      const form: CustomFormGroup = <CustomFormGroup>control.parent;
      const messageMap: { [message: string]: TranslationMessage | ((control: AbstractControl) => TranslationMessage) } =
        form.translationMessageMap !== undefined ? form.translationMessageMap[controlName] || {} : {};
      const params: ITranslationParams = errors[type];
      const messageMapItem = messageMap[type];
      let message =
        messageMapItem != null ? (messageMapItem instanceof TranslationMessage ? messageMapItem : messageMapItem(control)) : null;

      if (!message) {
        message = new TranslationMessage(translator, `common.validationMessage.${type}`, params);
      }

      control.showTooltip(message);
    }
    return true;
  }

  public static showError(translator: GlobalTranslatorService, control: CustomFormControl, checkValidateGroup: boolean = false): void {
    const hasError = FormBuilderService.showControlError(translator, control);

    if (hasError || !checkValidateGroup || !(control.parent instanceof FormGroup)) {
      return;
    }
    const formControlName = FormBuilderService.getControlName(control);
    const customFormGroupControlParent = <CustomFormGroup>control.parent;
    if (customFormGroupControlParent.validateByGroupControlNames != null) {
      customFormGroupControlParent.validateByGroupControlNames.forEach(validateGroup => {
        if (validateGroup.includes(formControlName)) {
          this.runValidators(translator, customFormGroupControlParent, validateGroup.filter(p => p !== formControlName)).subscribe();
        }
      });
    }
  }

  private static getControlsAsArray(formOrControl: FormGroup | FormArray | FormControl, validateControls?: string[]): AbstractControl[] {
    if (formOrControl instanceof FormControl) {
      return [formOrControl];
    }

    if (formOrControl instanceof FormGroup || formOrControl instanceof FormArray) {
      const controls: AbstractControl[] = [];
      const validateControlsDic = validateControls ? Utils.toDictionary(validateControls) : null;
      (formOrControl instanceof FormGroup
        ? Object.keys(formOrControl.controls)
            .filter(name => {
              if (validateControlsDic == null) {
                return true;
              }

              return validateControlsDic[name] != null;
            })
            .map(name => formOrControl.controls[name])
        : formOrControl instanceof FormArray
        ? formOrControl.controls
        : []
      ).forEach(control => controls.push(...this.getControlsAsArray(<FormControl>control)));

      return controls;
    }

    return [];
  }

  private static runControlValidators(control: AbstractControl): Observable<boolean> {
    // Do not run validate on disabled control.
    if (control.disabled) {
      return of(true);
    }

    // Execute control validators.
    control.updateValueAndValidity();

    if (control.pending) {
      return control.statusChanges.pipe(
        filter((status: string) => status !== 'PENDING'),
        take(1),
        map(() => control.valid)
      );
    } else {
      return of(control.valid);
    }
  }

  constructor(private globalSpinnerService: GlobalSpinnerService, private globalTranslator: GlobalTranslatorService) {
    super();
  }

  /**
   * Create root of form group
   * @param key The identifier of form group
   * @param controlsConfig The cotrols configuraions
   * @param extra The extra settings
   */
  public rootGroup(
    name: string,
    controlsConfig: {
      [key: string]: unknown;
    },
    controlsOptions?:
      | AbstractControlOptions
      | {
          [key: string]: unknown;
        }
      | null,
    translationMessageMap?: IFormTranslationMessageMap,
    options?: Partial<FormOptions>,
    validateByGroupControlNames?: string[][]
  ): CustomFormGroup {
    options = Object.assign(new FormOptions(), options);
    this.normalize(controlsConfig);

    const form: CustomFormGroup = this.group(controlsConfig, controlsOptions);

    form.originalValue = form.value;
    form.name = name;
    form.isRoot = true;
    form.options = options;
    form.translationMessageMap = translationMessageMap;
    form.validateByGroupControlNames = validateByGroupControlNames;

    if (options.autoAsyncIndicator) {
      form.statusChanges.subscribe(this.toggleIndicator.bind(this));
    }

    return form;
  }

  /**
   * @internal
   * https://github.com/angular/angular/blob/4.4.6/packages/forms/src/form_builder.ts
   * Overrides default `control` method of original form builder
   */
  public control(
    formState: unknown,
    validatorOrOpts?: ValidatorFn | ValidatorFn[] | AbstractControlOptions | null,
    asyncValidator?: AsyncValidatorFn | AsyncValidatorFn[] | null
  ): FormControl {
    return new CustomFormControl(formState, validatorOrOpts, asyncValidator);
  }

  /**
   * Run form validators.
   * @param CustomFormGroup form
   */
  public runValidators(form: CustomFormGroup, validateControls?: string[]): Observable<AbstractControl> {
    return FormBuilderService.runValidators(this.globalTranslator, form, validateControls);
  }

  public showError(control: CustomFormControl, checkValidateGroup: boolean = false): void {
    FormBuilderService.showError(this.globalTranslator, control, checkValidateGroup);
  }

  /**
   * Discard the form value changes.
   * @param form An abstract form control instant.
   */
  public reset(form: CustomFormGroup): void {
    form.reset(form.originalValue);
  }

  /**
   * Update the form original value.
   * @param form An abstract form control instant
   */
  public markAsPristine(form: CustomFormGroup): void {
    form.originalValue = form.value;
    form.markAsPristine();
  }

  /**
   * Determine the form control whether or not pristine
   * @param form MjsFormControl
   */
  public isPristine(form: CustomFormGroup): boolean {
    // Using stringify to make a comparision between 2 objects.
    // Make sure the value of form control does not exist properties with value as function type.
    return JSON.stringify(form.originalValue) === JSON.stringify(form.value);
  }

  /**
   * To explain the reason of normalize method.
   * Refer: https://github.com/angular/angular/blob/master/packages/forms/src/form_builder.ts
   * By default, DefaultValueAccessor of Angular will normalize value when null/undefined to empty string
   * to allocate memory location.
   * Refer: https://github.com/angular/angular/blob/master/packages/forms/src/directives/default_value_accessor.ts
   * But there is a bug of Wijmo that WjValueAccessor do not normalize value when null/undefined
   * There for we need this method
   */
  private normalize(controlsConfig: { [key: string]: unknown }): void {
    for (const controlName in controlsConfig) {
      if (!controlName) {
        continue;
      }

      const control: unknown = controlsConfig[controlName];

      if (Array.isArray(control)) {
        const value: unknown = control[0];

        control[0] = value == null ? '' : value;
      }
    }
  }

  private toggleIndicator(status: string): void {
    if (status === 'PENDING') {
      this.globalSpinnerService.show();
    } else {
      this.globalSpinnerService.hide();
    }
  }
}
