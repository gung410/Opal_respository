import { NgModule } from '@angular/core';
import { LearningNeedService } from './store-data-services/learning-need.services';
import { LearningNeedStoreService } from './store-services/learning-need-store.service';
import { DirectionStoreService } from './store-services/direction-store.service';
import { ProgrammeStoreService } from './store-services/programme-store.service';
import { SystemRolesStoreService } from './store-services/system-roles-store.service';
import { SystemRolesDataService } from './store-data-services/system-roles-data.service';

@NgModule({
  declarations: [],
  imports: [],
  providers: [
    LearningNeedService,
    LearningNeedStoreService,
    DirectionStoreService,
    ProgrammeStoreService,
    SystemRolesStoreService,
    SystemRolesDataService,
  ],
})
export class AppCoreModule {}
