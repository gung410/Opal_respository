import { ModuleWithProviders, NgModule } from '@angular/core';

import { BadgeApiService } from './services/badge-api.service';
import { BadgeRepository } from './repositories/badge.repository';
import { BadgeRepositoryContext } from './badge-repository-context';
import { YearlyUserStatisticApiService } from './services/yearly-user-statistic-api.service';
import { YearlyUserStatisticRepository } from './repositories/yearly-user-statistic.repository';

@NgModule({
  providers: [BadgeApiService, BadgeRepository, YearlyUserStatisticApiService, YearlyUserStatisticRepository]
})
export class BadgeDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: BadgeDomainApiModule,
      providers: [BadgeRepositoryContext]
    };
  }
}
