import { ChangeDetectorRef, Directive, ElementRef, HostBinding, Input, Renderer2 } from '@angular/core';

import { BaseDirective } from '../abstracts/base.directive';
import { CxHtmlUtil } from '../utils/html-util';
import { CxStringUtil } from '../utils/string.util';

@Directive({
    selector: '[cxButton]'
})
export class CxButtonDirective extends BaseDirective {
    public static Types = {
        primary: 'primary',
        secondary: 'secondary',
        success: 'success',
        danger: 'danger',
        warning: 'warning',
        info: 'info',
        outlinePrimary: 'outline-primary',
        outlineSecondary: 'outline-secondary',
        outlineSuccess: 'outline-success',
        outlineDanger: 'outline-danger',
        outlineWarning: 'outline-warning',
        outlineInfo: 'outline-info'
    };

    constructor(elementRef: ElementRef, renderer: Renderer2, changeDetectorRef: ChangeDetectorRef) {
        super(elementRef, renderer, changeDetectorRef);
        this.element.classList.add('btn');
        this.element.style.cursor = 'pointer';
    }

    @Input()
    set btnType(value) {
        const oldBtnTypeClass = getBtnTypeClass(this.btnType);
        const newBtnTypeClass = getBtnTypeClass(value);
        CxHtmlUtil.replaceClass(this.element, oldBtnTypeClass, newBtnTypeClass);

        this.btnTypeVal = value;
    }
    get btnType(): string | undefined {
        return this.btnTypeVal;
    }
    private btnTypeVal: string | undefined;

    @Input() public set btnSize(value) {
        const oldBtnSizeClass = getBtnTypeClass(this.btnSize);
        const newBtnSizeClass = getBtnTypeClass(value);
        CxHtmlUtil.replaceClass(this.element, oldBtnSizeClass, newBtnSizeClass);

        this.btnSizeVal = value;
    }
    public get btnSize(): string | undefined { return this.btnSizeVal; }
    private btnSizeVal: string | undefined;

    @Input() public btnEnabled = true;

    @HostBinding('class.disabled')
    public get btnDisabled() { return !this.btnEnabled; }
}

function getBtnTypeClass(type: string | undefined): string {
    if (!CxStringUtil.isNullOrEmpty(type)) {
        return `btn-${type}`;
    }
    return '';
}
