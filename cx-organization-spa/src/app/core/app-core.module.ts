import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { PDCatalogueDataService } from 'app/user-accounts/services/pd-catalouge-data.service';

import { CheckingUserRolesService } from 'app/user-accounts/services/checking-user-roles.service';
import { CareerPathsDataService } from './store-data-services/career-paths-data.service';
import { DevelopmentalRolesDataService } from './store-data-services/developmental-roles-data';
import { LearningFrameWorksDataService } from './store-data-services/learning-frame-works-data';
import { PersonnelGroupsDataService } from './store-data-services/personnel-groups-data.service';
import { SystemRolesDataService } from './store-data-services/system-roles-data.service';
import { CareerPathsStoreService } from './store-services/career-paths-data.service';
import { PDCatalogueStoreService } from './store-services/pd-catalogue-store.service';
import { PersonnelGroupsStoreService } from './store-services/personnel-groups-store.service';
import { GlobalKeySearchStoreService } from './store-services/search-key-store.service';
import { SystemRolesStoreService } from './store-services/system-roles-store.service';

@NgModule({
  declarations: [],
  imports: [HttpClientModule],
  providers: [
    PersonnelGroupsDataService,
    PersonnelGroupsStoreService,
    SystemRolesDataService,
    SystemRolesStoreService,
    CareerPathsDataService,
    CareerPathsStoreService,
    GlobalKeySearchStoreService,
    LearningFrameWorksDataService,
    DevelopmentalRolesDataService,
    PDCatalogueStoreService,
    PDCatalogueDataService,
    CheckingUserRolesService
  ]
})
export class AppCoreModule {}
