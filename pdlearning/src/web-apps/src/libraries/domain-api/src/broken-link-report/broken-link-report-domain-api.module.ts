import { ModuleWithProviders, NgModule } from '@angular/core';

import { BrokenLinkReportApiService } from './services/broken-link-report-api.service';
import { BrokenLinkReportComponentService } from './services/broken-link-report-component.service';
import { BrokenLinkReportRepository } from './repository/broken-link-report-repository';
import { BrokenLinkReportRepositoryContext } from './broken-link-report-repository-context';

@NgModule({
  providers: [BrokenLinkReportRepository, BrokenLinkReportComponentService, BrokenLinkReportApiService]
})
export class BrokenLinkReportDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: BrokenLinkReportDomainApiModule,
      providers: [BrokenLinkReportRepositoryContext]
    };
  }
}
