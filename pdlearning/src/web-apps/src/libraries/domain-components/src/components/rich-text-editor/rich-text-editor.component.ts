import { BaseComponent, FileUploaderSetting, LocalTranslatorService, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, HostBinding, HostListener, Input, Renderer2, ViewChild } from '@angular/core';

import { BrokenLinkReportType } from '@opal20/domain-api';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { OpalKendoEditorComponent } from '../opal-kendo-editor/opal-kendo-editor-component';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

@Component({
  selector: 'rich-text-editor',
  templateUrl: './rich-text-editor.component.html'
})
export class RichTextEditorComponent extends BaseComponent {
  @Input() public html: string = '';
  @Input() public enableDragAndDropFile: boolean = false;
  @Input() public brokenLinkReportType: BrokenLinkReportType;

  public dropFileUploaderSettings: FileUploaderSetting = new FileUploaderSetting({ isShowExtensionsOnError: true });
  public isFileDragOver: boolean = false;
  public selected: HTMLElement;
  public editorIframe: HTMLIFrameElement;
  @ViewChild('kendoEditor', { static: false })
  private editor: OpalKendoEditorComponent;
  @ViewChild('subscriptButton', { static: true })
  private subscriptButton: ToolBarButtonComponent;
  @ViewChild('superscriptButton', { static: true })
  private superscriptButton: ToolBarButtonComponent;

  constructor(
    protected renderer: Renderer2,
    protected moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public translator: LocalTranslatorService
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('style.flex-direction') get getFlexDirectionStyle(): string {
    return 'column';
  }

  public close(): void {
    // Replace table column resize
    const outputHtml = Utils.replaceColumnResizeHtml(this.html);
    this.dialogRef.close(outputHtml);
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

  public onValueChanged(): void {
    const el = document.createElement('div');
    el.innerHTML = this.html;
    const listAchor: NodeList = el.querySelectorAll('a');
    let hasChanged = false;
    listAchor.forEach((a: HTMLAnchorElement) => {
      const href = Utils.getHrefFromAnchor(a);
      if (!Utils.isAbsoluteUrl(href)) {
        const newHref = `http://${href}`;
        const newHtml = a.outerHTML.replace(href, newHref);
        this.html = this.html.replace(a.outerHTML, newHtml);
        hasChanged = true;
      }
    });

    if (this.html.match('</table>$')) {
      this.html += '<p></p>';
    }
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    try {
      const messageData = event.data;
      if (messageData.key === 'MathliveLoaded') {
        const mathLiveIframe: HTMLIFrameElement = document.querySelector('#mathLiveIframe');

        mathLiveIframe.contentWindow.postMessage({ key: 'MathliveInit', content: this.selected.getAttribute('alt') }, '*');
      }
    } catch {
      return;
    }
  }

  public loadSelected(): void {
    this.selected = this.editor.viewMountElement.querySelector('.ProseMirror-selectednode');
  }

  public onFileDropped(fileUrl: String): void {
    if (fileUrl && typeof fileUrl === 'string') {
      this.editor.exec('insertText', { text: '[insertHTML]' });
      const src = this.editor.value.replace('<p>[insertHTML]</p>', `${fileUrl}<p></p>`).replace('[insertHTML]', `</p>${fileUrl}<p>`);
      this.editor.exec('setHTML', src);
    }
    this.isFileDragOver = false;
  }

  public onTextDropped(text: String): void {
    this.editor.exec('insertText', { text: text });
    this.isFileDragOver = false;
  }

  public onFileDragOver(): void {
    // prevent the case if some dialogs are opening, should not open drop zone
    const openingDialog = document.querySelectorAll('kendo-dialog');
    if (openingDialog.length >= 2) {
      return;
    }

    if (this.enableDragAndDropFile) {
      this.isFileDragOver = true;
    }
  }

  public onFileDragLeave(): void {
    if (this.isFileDragOver) {
      this.isFileDragOver = false;
    }
  }

  public ngAfterViewInit(): void {
    // Must set timeout because kendo edit can't load iframe content.
    setTimeout(() => {
      this.viewInitiated = true;
    }, 0);
  }
}
