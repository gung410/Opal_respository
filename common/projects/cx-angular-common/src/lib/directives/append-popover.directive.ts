import {
  ChangeDetectorRef,
  Directive,
  ElementRef,
  Input,
  OnInit,
  Renderer2
} from '@angular/core';
import { BaseDirective } from '../abstracts/base.directive';
import { CxHtmlUtil } from '../utils/html-util';

@Directive({
  selector: '[cxAppendPopover]'
})
export class CxAppendPopoverDirective extends BaseDirective implements OnInit {
  private textVal = '';
  public get text() {
    return this.textVal;
  }

  @Input('cx-append-popover')
  public set text(value: string) {
    if (this.textVal === value) {
      return;
    }
    this.textVal = value;
    if (this.initiated) {
      this._updatePopoverText();
    }
  }

  private isActiveVal = false;
  @Input('cx-append-popover-is-active')
  public set isActive(value: boolean) {
    if (this.isActiveVal === value) {
      return;
    }
    this.isActiveVal = value;
    if (this.initiated && !this.isActiveVal) {
      this._destroyPopoverElement();
    } else {
      this.initPopover();
    }
  }
  public get isActive() {
    return this.isActiveVal;
  }
  private innerVal = false;
  public get inner(): boolean {
    return this.innerVal;
  }
  @Input('cx-append-popover-inner') public set inner(value) {
    if (this.innerVal === value) {
      return;
    }
    this.innerVal = value;
    this.updateInnerClass();
  }

  private popoverElementVal: HTMLElement | undefined;
  private popoverTextContainerElementVal: HTMLElement | undefined;

  public initPopover() {
    if (!this.initiated || !this.isActive) {
      return;
    }

    const popoverElement = this._setupPopoverElement();
    this.element.insertBefore(popoverElement, this.element.firstChild);
  }

  public updateInnerClass() {
    if (this.popoverElementVal === undefined) {
      return;
    }
    if (this.inner) {
      CxHtmlUtil.addClass(this.popoverElementVal, '-inner');
    } else {
      CxHtmlUtil.removeClass(this.popoverElementVal, '-inner');
    }
  }

  private _setupPopoverElement() {
    const popoverTextContainer = this.renderer.createElement(
      'div'
    ) as HTMLElement;

    popoverTextContainer.textContent = this.text;

    const popoverArrowHtmlElement = this.renderer.createElement('div');
    this._setArrowContainerElementStyle(popoverArrowHtmlElement);

    const popoverContainerElement = this.renderer.createElement('div');
    popoverContainerElement.appendChild(popoverTextContainer);
    popoverContainerElement.appendChild(popoverArrowHtmlElement);
    this._setPopoverContainerElementStyle(popoverContainerElement);

    const popoverElement: HTMLElement = this.renderer.createElement('div');
    popoverElement.appendChild(popoverContainerElement);
    this._setCxPopoverElementStyle(popoverElement);

    this.popoverElementVal = popoverElement;
    this.popoverTextContainerElementVal = popoverTextContainer;

    this.updateInnerClass();

    return popoverElement;
  }

  private _updatePopoverText() {
    if (this.popoverTextContainerElementVal === undefined) {
      return;
    }
    this.popoverTextContainerElementVal.textContent = this.text;
  }

  private _setCxPopoverElementStyle(el: HTMLElement) {
    el.className = 'cx-popover';
  }

  private _setPopoverContainerElementStyle(el: HTMLElement) {
    el.className = 'cx-popover__container';
  }

  private _setArrowContainerElementStyle(el: HTMLElement) {
    el.className = 'cx-popover__arrow';
  }

  private _destroyPopoverElement() {
    if (this.popoverElementVal === undefined) {
      return;
    }
    this.popoverElementVal.remove();
    this.popoverElementVal = undefined;
  }

  constructor(
    elementRef: ElementRef,
    renderer: Renderer2,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(elementRef, renderer, changeDetectorRef);
  }

  ngOnInit() {
    super.ngOnInit();
    this.initPopover();
    this.element.style.position = 'relative';
  }
}
