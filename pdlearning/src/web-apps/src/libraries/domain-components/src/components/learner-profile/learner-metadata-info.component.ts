import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { LearnerProfileViewModel } from './../../view-models/learner-profile-view.model';

@Component({
  selector: 'learner-metadata-info',
  templateUrl: './learner-metadata-info.component.html'
})
export class LearnerMetadataInfoComponent extends BaseComponent {
  public get learnerVm(): LearnerProfileViewModel {
    return this._learnerVm;
  }

  @Input()
  public set learnerVm(v: LearnerProfileViewModel) {
    this._learnerVm = v;
  }

  private _learnerVm: LearnerProfileViewModel;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
