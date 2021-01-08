import { BaseComponent, LocalTranslatorService, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, HostBinding, Input, Output, Renderer2, ViewEncapsulation, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

import { BrokenLinkReportType } from '@opal20/domain-api';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { OpalDialogService } from '@opal20/common-components';
import { RichTextEditorComponent } from '../rich-text-editor/rich-text-editor.component';

@Component({
  selector: 'question-title-editor',
  templateUrl: './question-title-editor.component.html',
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => QuestionTitleEditorComponent),
      multi: true
    }
  ]
})
export class QuestionTitleEditorComponent extends BaseComponent implements ControlValueAccessor {
  @Input() public html: string;
  @Input() public isEditable: boolean;
  @Input() public isRequired: boolean;
  @Input() public placeholder: string = 'Input content';
  @Input() public enableDragAndDropFile: boolean = false;
  @Input() public brokenLinkReportType: BrokenLinkReportType;
  @Input() public dialogZIndex?: number;

  @Output() public htmlChange = new EventEmitter();

  public selected: HTMLElement;
  public editorIframe: HTMLIFrameElement;
  public preview: SafeHtml;

  constructor(
    private sanitizer: DomSanitizer,
    protected renderer: Renderer2,
    protected moduleFacadeService: ModuleFacadeService,
    public translator: LocalTranslatorService,
    private opalDialogSvc: OpalDialogService
  ) {
    super(moduleFacadeService);
  }
  public onChange = (value: string) => undefined;

  public onTouched = () => undefined;

  public writeValue(value: string): void {
    this.html = value;
    this.onChange(value);
  }

  public registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }
  public registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  public setDisabledState(isDisabled: boolean): void {
    this.isEditable = isDisabled;
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('style.flex-direction') get getFlexDirectionStyle(): string {
    return 'column';
  }
  public onInit(): void {
    this.preview = this.sanitizer.bypassSecurityTrustHtml(this.html);
  }

  public open(): void {
    this.onTouched();
    const dialogRef: DialogRef = this.opalDialogSvc.openDialogRef(
      RichTextEditorComponent,
      {
        html: this.html,
        enableDragAndDropFile: this.enableDragAndDropFile,
        brokenLinkReportType: this.brokenLinkReportType
      },
      {
        zIndex: this.dialogZIndex
      }
    );

    this.subscribe(dialogRef.result, (html: string) => {
      this.html = typeof html === 'string' ? html : null;
      this.preview = this.sanitizer.bypassSecurityTrustHtml(this.html);
      this.htmlChange.emit(this.html);
      this.onChange(this.html);
    });
  }
}
