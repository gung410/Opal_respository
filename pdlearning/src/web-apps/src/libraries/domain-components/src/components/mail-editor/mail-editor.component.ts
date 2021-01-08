import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { EditorComponent } from '@progress/kendo-angular-editor';
import { MailTag } from './../../models/mail-tag.model';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

@Component({
  selector: 'mail-editor',
  templateUrl: './mail-editor.component.html'
})
export class MailEditorComponent extends BasePageComponent {
  @ViewChild(EditorComponent, { static: false })
  public editor: EditorComponent;
  @ViewChild('subscriptButton', { static: true })
  public subscriptButton: ToolBarButtonComponent;
  @ViewChild('superscriptButton', { static: true })
  public superscriptButton: ToolBarButtonComponent;

  @Input() public tags: MailTag[] = [];
  public _mailContent: string;
  public get mailContent(): string {
    return this._mailContent;
  }
  @Input()
  public set mailContent(v: string) {
    this._mailContent = v;
    if (this.initiated && v !== this._currentEmailContent) {
      this.editor.value = v;
      this._currentEmailContent = v;
    }
  }

  @Output() public mailContentChange = new EventEmitter();

  public initMailContent: string;
  private _currentEmailContent: string;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public removeSubscript(): void {
    if (this.editor === undefined || this.subscriptButton === undefined) {
      return;
    }

    if (this.subscriptButton.selected === true) {
      this.editor.exec('subscript');
    }
  }

  public removeSuperscript(): void {
    if (this.editor === undefined || this.superscriptButton === undefined) {
      return;
    }

    if (this.superscriptButton.selected === true) {
      this.editor.exec('superscript');
    }
  }

  public onValueChanged(editorHtml: string): void {
    editorHtml = editorHtml.replace(/<p><\/p>/gm, '<p><br></p>');

    // Process fix href
    const listAchor: NodeList = this.editor.viewMountElement.querySelectorAll('a');
    let fixHrefHasChangedData = false;
    listAchor.forEach((a: HTMLAnchorElement) => {
      const href = Utils.getHrefFromAnchor(a);
      if (!Utils.isAbsoluteUrl(href)) {
        const newHref = `http://${href}`;
        const newHtml = a.outerHTML.replace(href, newHref);
        editorHtml = editorHtml.replace(a.outerHTML, newHtml);
        fixHrefHasChangedData = true;
      }
    });
    if (fixHrefHasChangedData) {
      this.editor.value = editorHtml;
    }
    this._currentEmailContent = editorHtml;
    this.mailContentChange.emit(editorHtml);
  }

  public onSelectTag(tag: MailTag): void {
    this.editor.exec('insertText', { text: tag.value });
    this.mailContent = this.editor.value;
    this.mailContentChange.emit(this.mailContent);
  }

  protected onInit(): void {
    this.initMailContent = this.mailContent;
    this._currentEmailContent = this.mailContent;
  }
}
