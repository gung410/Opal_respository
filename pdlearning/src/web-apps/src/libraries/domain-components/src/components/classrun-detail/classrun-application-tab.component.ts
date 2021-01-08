import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { ClassRunDetailMode, showClassRunDetailViewOnly } from '../../models/classrun-detail-mode.model';
import { Component, Input } from '@angular/core';

import { ClassRunDetailViewModel } from '../../view-models/classrun-detail-view.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'classrun-application-tab',
  templateUrl: './classrun-application-tab.component.html'
})
export class ClassApplicationTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public classRun: ClassRunDetailViewModel;
  @Input() public mode: ClassRunDetailMode | undefined;

  public ClassRunDetailMode: typeof ClassRunDetailMode = ClassRunDetailMode;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public showClassRunDetailViewOnly(): boolean {
    return showClassRunDetailViewOnly(this.mode);
  }
}
