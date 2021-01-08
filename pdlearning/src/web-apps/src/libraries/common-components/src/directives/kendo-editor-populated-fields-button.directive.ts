import { Directive, OnDestroy, OnInit } from '@angular/core';

import { GuidelinePopulatedFieldsDialogComponent } from '../components/guideline-populated-fields-dialog/guideline-populated-fields-dialog.component';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription } from 'rxjs';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

enum ToolbarIcon {
  Image = 'question',
  Text = 'Auto populated guideline',
  Title = 'Auto populated guideline',
  ShowText = 'overflow'
}

@Directive({
  selector: 'kendo-toolbar-button[kendoEditorPopulatedFieldsButton]'
})
export class KendoEditorPopulatedFieldsButtonDirective implements OnInit, OnDestroy {
  private subs: Subscription[] = [];

  constructor(protected moduleFacadeService: ModuleFacadeService, private button: ToolBarButtonComponent) {}

  public ngOnInit(): void {
    this.button.icon = ToolbarIcon.Image;
    this.button.showText = ToolbarIcon.ShowText;
    this.button.text = ToolbarIcon.Text;
    this.button.title = ToolbarIcon.Title;

    this.subs.push(
      this.button.click.subscribe(() => {
        this.moduleFacadeService.dialogService.open({ content: GuidelinePopulatedFieldsDialogComponent });
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
