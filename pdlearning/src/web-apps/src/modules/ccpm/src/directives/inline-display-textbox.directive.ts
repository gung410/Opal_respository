import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Directive, ElementRef, HostBinding, HostListener } from '@angular/core';

@Directive({
  selector: '[inlineDisplayTextbox]'
})
export class InlineDisplayTextboxDirective extends BaseComponent {
  @HostBinding('class.inline-display-textbox') public mainClassHostBinding: boolean = true;
  @HostBinding('class.-as-text') public get asTextClassHostBinding(): boolean {
    return !this.focusing && !this.hovering;
  }
  public get element(): HTMLInputElement {
    return this.elementRef.nativeElement;
  }
  protected focusing: boolean = false;
  protected hovering: boolean = false;

  constructor(protected moduleFacadeService: ModuleFacadeService, protected elementRef: ElementRef) {
    super(moduleFacadeService);
  }

  @HostListener('mouseover') public onMouseOver(): void {
    this.hovering = true;
  }
  @HostListener('mouseout') public onMouseOut(): void {
    this.hovering = false;
  }
  @HostListener('focus') public onFocus(): void {
    this.focusing = true;
  }
  @HostListener('focusout') public onFocusOut(): void {
    this.focusing = false;
  }
}
