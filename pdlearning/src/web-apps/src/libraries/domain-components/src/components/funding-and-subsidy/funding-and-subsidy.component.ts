import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { FUNDING_AND_SUBSIDY_DATA, FundingAndSubsidy } from '@opal20/domain-api';

import { Component } from '@angular/core';

@Component({
  selector: 'funding-and-subsidy',
  templateUrl: './funding-and-subsidy.component.html'
})
export class FundingAndSubsidyComponent extends BaseComponent {
  public selecteds: { [key: number]: boolean } = {};
  public dataFundingAndSubsidy = FUNDING_AND_SUBSIDY_DATA;
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
  public toggleReadMore(data: FundingAndSubsidy): void {
    this.selecteds[data.id] = !this.selecteds[data.id];
  }
}
