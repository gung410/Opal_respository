import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding } from '@angular/core';
import { IOpalReportDynamicParams, OpalReportDynamicComponent } from '@opal20/domain-components';

@Component({
  selector: 'reports-page',
  templateUrl: './reports-page.component.html'
})
export class ReportsPageComponent extends BasePageComponent {
  public paramsReportDynamic: IOpalReportDynamicParams | null;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  protected onInit(): void {
    this.paramsReportDynamic = OpalReportDynamicComponent.buildLmmReportList();
  }
}
