import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';

import { ManagementDataService } from './management-data.service';

@Component({
  selector: 'management',
  templateUrl: './management.component.html',
  styleUrls: ['./management.component.scss'],
  providers: [ManagementDataService]
})
export class ManagementComponent extends BaseScreenComponent implements OnInit {
  apiOrigins: string[] = Object.values(AppConstant.api);

  constructor(
    private managementDataService: ManagementDataService,
    public authService: AuthService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    super.ngOnInit();
  }

  onOriginClicked(origin): void {
    const action = 'clearcache';
    this.managementDataService.executeApiAction(origin, action).subscribe(
      (success) => {
        console.log(
          `Finished successfully action ${action} on ${origin}`,
          success
        );
      },
      (error) => {
        console.log(
          `An error occurs when processing action ${action} on ${origin}`,
          error
        );
      }
    );
  }
}
