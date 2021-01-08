import { Directive, ElementRef, Inject, OnInit, Optional } from '@angular/core';

import { BaseComponent } from '../base-components/base-component';
import { CustomFormControl } from '../form/form-control';
import { DomUtils } from '../utils/dom.utils';
import { FormControlName } from '@angular/forms';
import { KendoInput } from '@progress/kendo-angular-common';
import { ModuleFacadeService } from '../services/module-facade.service';

export interface IHasFocusableProcessing {
  /**
   * Focus on the appropriate html element on this control.
   * Only necessary for leaf component.
   */
  focus(): void;
  /**
   * Speicifies whether the control can currently receive focus.
   * For example: disabled control should not receive focus.
   */
  canFocus(): boolean;
}

/**
 * Receive and handle focus request from form control.
 */
@Directive({
  selector: '[formControlName]'
})
export class ReactiveFocusIntegrationDirective extends BaseComponent implements OnInit {
  constructor(
    protected formControlName: FormControlName,
    @Optional()
    @Inject(KendoInput)
    private component: IHasFocusableProcessing,
    private elementRef: ElementRef,
    moduleFacadeService: ModuleFacadeService
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    if (this.formControlName.control && this.formControlName.control instanceof CustomFormControl) {
      this.formControlName.control.focusOnError.pipe(this.untilDestroy()).subscribe(() => {
        if (this.component && this.component.focus) {
          this.component.focus();
        } else {
          (this.elementRef.nativeElement as HTMLInputElement | HTMLTextAreaElement).focus();
          DomUtils.scrollToView(this.elementRef.nativeElement);
        }
      });
    }
  }
}
