import { Component, ChangeDetectionStrategy, ViewEncapsulation, Input, ElementRef, ChangeDetectorRef, OnChanges, SimpleChanges } from '@angular/core';
import { BaseComponent } from '../../abstracts/base.component';
import { MediaObserver } from '@angular/flex-layout';
import { CxLoaderUI, CxLoaderModuleConfig } from './cx-loader.model';

@Component({
    selector: 'cx-loader',
    templateUrl: './cx-loader.component.html',
    styleUrls: ['./cx-loader.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    encapsulation: ViewEncapsulation.None
})
export class CxLoaderComponent extends BaseComponent {
    private _loading = false;
    @Input() get loading() {
        return this._loading;
    }
    set loading(val) {
        this._loading = val;
        this.changeDetectorRef.detectChanges();

    }

    private _uiModel: CxLoaderUI = new CxLoaderUI();
    @Input() get uiModel() {
        return this._uiModel;
    }
    set uiModel(val) {
        this._uiModel = val;
        this.changeDetectorRef.detectChanges();
    }
    constructor(changeDetectorRef: ChangeDetectorRef,
        elementRef: ElementRef,
        media: MediaObserver) {
        super(changeDetectorRef,
            elementRef,
            media);
    }

    ngOnInit() {
        super.ngOnInit();
    }

    public setUpConfig(config: CxLoaderModuleConfig) {
        this.uiModel = config.loaderUi;
    }
}
