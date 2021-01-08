import { BaseComponent, FileUploaderSetting, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { FormGroup } from '@angular/forms';
import { LearningPathDetailViewModel } from './../../view-models/learning-path-detail-view.model';

@Component({
  selector: 'learning-path-basic-info-tab',
  templateUrl: './learning-path-basic-info-tab.component.html'
})
export class LearningPathBasicInfoTabComponent extends BaseComponent {
  @Input() public isViewMode: boolean;

  public get learningPathDetailVM(): LearningPathDetailViewModel {
    return this._learningPathDetailVM;
  }

  @Input()
  public set learningPathDetailVM(v: LearningPathDetailViewModel) {
    if (Utils.isDifferent(v, this._learningPathDetailVM)) {
      this._learningPathDetailVM = v;
    }
    this.fileUploaderSetting.extensions = this._learningPathDetailVM.allowedThumbnailExtensions;
  }
  public fileUploaderSetting: FileUploaderSetting;

  @Input()
  public form: FormGroup;

  private _learningPathDetailVM: LearningPathDetailViewModel;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
  }
}
