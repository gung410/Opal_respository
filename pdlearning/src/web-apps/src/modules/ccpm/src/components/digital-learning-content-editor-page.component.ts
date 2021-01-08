import { BaseComponent, FileUploaderSetting, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, HostBinding, HostListener, Input, Renderer2, ViewChild, ViewContainerRef, ViewEncapsulation } from '@angular/core';
import { DigitalContentDetailMode, DigitalContentDetailViewModel, OpalKendoEditorComponent } from '@opal20/domain-components';

import { BrokenLinkReportType } from '@opal20/domain-api';
import { Constants } from '../constants/ccpm.common.constant';
import { FormGroup } from '@angular/forms';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

@Component({
  selector: 'digital-learning-content-editor-page',
  templateUrl: './digital-learning-content-editor-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalLearningContentEditorPageComponent extends BaseComponent {
  public DigitalContentDetailMode: typeof DigitalContentDetailMode = DigitalContentDetailMode;
  @Input() public form: FormGroup;
  @Input() public contentViewModel: DigitalContentDetailViewModel;
  @Input() public mode: DigitalContentDetailMode;
  public outputHtml: string;
  public paletteSettings: object = {
    palette: Constants.PALETTE_COLORS
  };

  public dropFileUploaderSettings: FileUploaderSetting = new FileUploaderSetting({ isShowExtensionsOnError: true });
  public selected: HTMLElement;
  public editorIframe: HTMLIFrameElement;
  public isFileDragOver: boolean = false;
  public brokenLinkReportTypeDigitalContent: BrokenLinkReportType = BrokenLinkReportType.DigitalContent;
  @ViewChild('kendoToolbar', { read: ViewContainerRef, static: true })
  private kendoToolbar: ViewContainerRef;
  @ViewChild('kendoEditor', { static: true })
  private editor: OpalKendoEditorComponent;
  @ViewChild('subscriptButton', { static: true })
  private subscriptButton: ToolBarButtonComponent;
  @ViewChild('superscriptButton', { static: true })
  private superscriptButton: ToolBarButtonComponent;
  @ViewChild('editorToolbar', { read: ViewContainerRef, static: true })
  private editorToolbar: ViewContainerRef;
  constructor(protected renderer: Renderer2, protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('style.flex-direction') get getFlexDirectionStyle(): string {
    return 'column';
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
    const listAchor: NodeList = this.editor.viewMountElement.querySelectorAll('a');
    let hasChanged = false;
    listAchor.forEach((a: HTMLAnchorElement) => {
      const href = Utils.getHrefFromAnchor(a);
      if (!Utils.isAbsoluteUrl(href)) {
        const newHref = `http://${href}`;
        const newHtml = a.outerHTML.replace(href, newHref);
        editorHtml = editorHtml.replace(a.outerHTML, newHtml);
        hasChanged = true;
      }
    });

    if (editorHtml.match('</table>$')) {
      this.editor.value += '<p></p>';
    }

    // Replace table column resize
    this.outputHtml = Utils.replaceColumnResizeHtml(this.editor.value);
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

  public onFileDropped(fileUrl: String): void {
    if (fileUrl && typeof fileUrl === 'string') {
      this.editor.exec('insertText', { text: '[insertHTML]' });
      const src = this.editor.value.replace('<p>[insertHTML]</p>', `${fileUrl}<p></p>`).replace('[insertHTML]', `</p>${fileUrl}<p>`);
      this.editor.exec('setHTML', src);
    }
    this.isFileDragOver = false;
  }

  public onFileDragOver(): void {
    // prevent the case if some dialogs are opening, should not open drop zone
    const openingDialog = document.querySelector('kendo-dialog');
    if (openingDialog) {
      return;
    }

    if (this.mode === DigitalContentDetailMode.Create || this.mode === DigitalContentDetailMode.Edit) {
      this.isFileDragOver = true;
    }
  }

  public onFileDragLeave(): void {
    if (this.isFileDragOver) {
      this.isFileDragOver = false;
    }
  }

  public onTextDropped(text: String): void {
    this.editor.exec('insertText', { text: text });
    this.isFileDragOver = false;
  }

  public loadSelected(): void {
    this.selected = this.editor.viewMountElement.querySelector('.ProseMirror-selectednode');
  }

  protected onAfterViewInit(): void {
    this.renderer.appendChild(this.editorToolbar.element.nativeElement, this.kendoToolbar.element.nativeElement);
    this.editorIframe = document.querySelector('digital-learning-content-editor-page iframe') as HTMLIFrameElement;
    this.outputHtml = this.contentViewModel.htmlContent;
    this.editor.value = this.contentViewModel.htmlContent;
  }
}

enum EditorCmd {
  InserImage = 'insertImage'
}
