import { BaseFormComponent, IBusinessValidator, IFormBuilderDefinition, IModalAction, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ViewChild } from '@angular/core';
import { Observable, of } from 'rxjs';

import { NumericTextBoxComponent } from '@progress/kendo-angular-inputs';
import { POCSingleFormService } from '../../services/poc-single-form.service';
import { POCSingleFormValidator } from '../../validators/poc-single-form.validator';
import { Validators } from '@angular/forms';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'single-form',
  template: `
    <h3>Single Form Component</h3>
    <div [formGroup]="form">
      ControlA:
      <kendo-numerictextbox
        #controlA
        kendoErrorTooltip
        style="width: 200px;"
        formControlName="controlA"
        [min]="0"
        [max]="9999"
        [spinners]="false"
        [autoCorrect]="true"
      ></kendo-numerictextbox>
      <br />
      ControlB:
      <kendo-numerictextbox
        kendoErrorTooltip
        style="width: 200px;"
        formControlName="controlB"
        [min]="0"
        [max]="9999"
        [spinners]="false"
        [autoCorrect]="true"
      ></kendo-numerictextbox>
      <br />
      <button (click)="checkAndSave()">Save</button>
      <button (click)="checkAndCancel()">Cancel</button>
    </div>
  `
})
export class POCSingleFormComponent extends BaseFormComponent {
  @ViewChild('controlA', { static: true })
  private controlA: NumericTextBoxComponent;

  constructor(protected moduleFacadeService: ModuleFacadeService, private service: POCSingleFormService) {
    super(moduleFacadeService);
  }

  public canDeactivate(): Observable<boolean> {
    if (this.isFormDirty()) {
      return this.modalService
        .showConfirmMessage('The form values are changed. Are you sure to quit?', () => true, () => false)
        .result.pipe(
          switchMap((result: IModalAction) => {
            if (result.callback) {
              return <Observable<boolean>>of(result.callback());
            }

            return of(true);
          })
        );
    }

    return of(true);
  }

  protected onAfterViewInit(): void {
    this.controlA.focus();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        controlA: {
          defaultValue: '',
          validators: [
            {
              validator: Validators.required
            }
          ]
        },
        controlB: {
          defaultValue: '',
          validators: [
            {
              validator: Validators.required
            }
          ]
        }
      }
    };
  }

  protected initData(): void {
    this.service.getData().subscribe(data => {
      this.patchInitialFormValue(data);
    });
  }

  protected createBusinessValidators(): IBusinessValidator[] {
    return [new POCSingleFormValidator(this.form)];
  }

  protected onValidateBusinessError(): void {
    this.modalService.showConfirmMessage(
      'Value of the control A should be greater than B. Do you want to continue?',
      this.performSave.bind(this),
      () => this.controlA.focus()
    );
  }

  protected saveData(): void {
    this.performSave();
  }

  protected performCancel(): void {
    this.resetForm(this.form);
    this.controlA.focus();
  }

  private performSave(): void {
    this.service.updateData(this.form.value).subscribe(() => {
      this.markAsPristine(this.form);
      this.controlA.focus();
    });
  }
}
