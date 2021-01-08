import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { ClassRunDetailMode, showClassRunDetailViewOnly } from '../../models/classrun-detail-mode.model';
import { ClassRunRescheduleStatus, SearchClassRunType } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

import { ClassRunDetailViewModel } from './../../view-models/classrun-detail-view.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'classrun-overview-info-tab',
  templateUrl: './classrun-overview-info-tab.component.html'
})
export class ClassRunOverviewInfoTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public classRun: ClassRunDetailViewModel;
  @Input() public mode: ClassRunDetailMode | undefined;
  @Input() public searchType: SearchClassRunType | undefined;
  @Input() public isRescheduleMode: boolean = false;

  public ClassRunRescheduleStatus: typeof ClassRunRescheduleStatus = ClassRunRescheduleStatus;
  public ClassRunDetailMode: typeof ClassRunDetailMode = ClassRunDetailMode;
  public SearchClassRunType: typeof SearchClassRunType = SearchClassRunType;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public showClassRunDetailViewOnly(): boolean {
    return showClassRunDetailViewOnly(this.mode);
  }
}
