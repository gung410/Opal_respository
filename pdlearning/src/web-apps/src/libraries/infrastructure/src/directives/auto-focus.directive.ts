import { AfterViewInit, Directive, ElementRef, Inject, OnInit, Optional } from '@angular/core';

import { BaseComponent } from '../base-components/base-component';
import { IHasFocusableProcessing } from './reactive-focus-integration.directive';
import { KendoInput } from '@progress/kendo-angular-common';
import { ModuleFacadeService } from '../services/module-facade.service';

@Directive({
  selector: '[autoFocus]'
})
export class AutoFocusDirective extends BaseComponent implements OnInit, AfterViewInit {
  constructor(
    moduleFacadeService: ModuleFacadeService,
    private elementRef: ElementRef,
    @Optional()
    @Inject(KendoInput)
    private component: IHasFocusableProcessing
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    super.ngOnInit();
    // Prevent ExpressionChangedAfterItHasBeenCheckedError
    setTimeout(() => {
      this.focus();
    });
  }

  public focus(): void {
    if (this.component && this.component.focus) {
      this.component.focus();
    } else {
      (this.elementRef.nativeElement as HTMLInputElement | HTMLTextAreaElement).focus();
    }
  }
}
