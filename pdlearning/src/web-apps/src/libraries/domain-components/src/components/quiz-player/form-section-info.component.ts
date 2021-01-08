import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { FormSectionViewModel } from '@opal20/domain-api';

@Component({
  selector: 'form-section-info',
  templateUrl: './form-section-info.component.html'
})
export class FormSectionInfoComponent extends BaseComponent {
  @Input() public formSectionVm: FormSectionViewModel;

  public get formSectionInfo(): { title: string; description: string } {
    if (this.formSectionVm) {
      return { title: this.formSectionVm.title, description: this.formSectionVm.description };
    }
  }
  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
