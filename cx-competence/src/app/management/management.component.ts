import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { ManagementDataService } from './management-data.service';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { AuthService } from 'app-auth/auth.service';

@Component({
  selector: 'management',
  templateUrl: './management.component.html',
  styleUrls: ['./management.component.scss'],
  providers: [ManagementDataService],
})
export class ManagementComponent extends BaseScreenComponent implements OnInit {
  public apiOrigins: string[] = Object.values(AppConstant.api);
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private managementDataService: ManagementDataService,
    public authService: AuthService
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit() {
    super.ngOnInit();
  }

  onOriginClicked(origin) {
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
