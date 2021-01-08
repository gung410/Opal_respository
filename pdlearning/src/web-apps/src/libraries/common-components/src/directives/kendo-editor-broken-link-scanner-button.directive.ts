import { Directive, Host, Input, OnDestroy, OnInit } from '@angular/core';

import { BrokenLinkReportType } from '@opal20/domain-api';
import { BrokenLinkScannerDialogComponent } from '../../../domain-components/src/components/broken-link-scanner-dialog/broken-link-scanner-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { EditorComponent } from '@progress/kendo-angular-editor';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription } from 'rxjs';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

enum ToolbarButton {
  Text = 'Check broken link',
  Title = 'Check broken link',
  ShowText = 'both',
  Icon = 'check'
}

@Directive({
  selector: 'kendo-toolbar-button[kendoEditorBrokenLinkScannerButton]'
})
export class KendoEditorBrokenLinkScannerButtonDirective implements OnInit, OnDestroy {
  @Input() public serviceType: BrokenLinkReportType = BrokenLinkReportType.DigitalContent;
  private subs: Subscription[] = [];

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private button: ToolBarButtonComponent,
    @Host() private editor: EditorComponent
  ) {}

  public ngOnInit(): void {
    this.button.style = { width: '100%' };
    this.button.showText = ToolbarButton.ShowText;
    this.button.text = ToolbarButton.Text;
    this.button.title = ToolbarButton.Title;
    this.button.icon = ToolbarButton.Icon;

    this.subs.push(
      this.button.click.subscribe(() => {
        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: BrokenLinkScannerDialogComponent });
        const configurationPopup = dialogRef.content.instance as BrokenLinkScannerDialogComponent;
        configurationPopup.html = this.editor.value;
        configurationPopup.brokenLinkReportType = this.serviceType;
        const dropDown = document.querySelector('kendo-popup:not(.error-tooltip)');
        if (dropDown) {
          dropDown.remove();
        }
      })
    );
  }

  public ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }
}
