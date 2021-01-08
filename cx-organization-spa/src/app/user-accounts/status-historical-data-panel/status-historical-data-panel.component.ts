import {
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';

import {
  UserManagement,
  UserManagementQueryModel
} from '../models/user-management.model';
import { UserAccountsDataService } from '../user-accounts-data.service';

@Component({
  selector: 'status-historical-data-panel',
  templateUrl: './status-historical-data-panel.component.html',
  styleUrls: ['./status-historical-data-panel.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class StatusHistoricalDataPanelComponent
  extends BaseSmartComponent
  implements OnInit {
  @Input() user: UserManagement;
  @Input() statusHistoricalData: any = [];
  modifyingUsers: UserManagement[] = [];
  constructor(
    protected changeDectectorRef: ChangeDetectorRef,
    private userAccountDataService: UserAccountsDataService
  ) {
    super(changeDectectorRef);
  }

  ngOnInit(): void {
    if (this.statusHistoricalData) {
      const loginServiceClaims = this.statusHistoricalData.map(
        (data) => data.payload.identity.userId
      );
      const uniqueServiceClaims = loginServiceClaims.filter(
        (element, index, array) => index === array.indexOf(element)
      );
      this.subscription.add(
        this.userAccountDataService
          .getUsers(
            new UserManagementQueryModel({
              loginServiceClaims: uniqueServiceClaims,
              getLoginServiceClaims: true
            })
          )
          .subscribe((users) => {
            if (users) {
              loginServiceClaims.map((originalLoginServiceClaim) => {
                let isMapUser = false;
                users.items.map((user) => {
                  if (
                    user.loginServiceClaims.find(
                      (item) => item.value === originalLoginServiceClaim
                    )
                  ) {
                    this.modifyingUsers.push(user);
                    isMapUser = true;
                  }
                });
                if (!isMapUser) {
                  this.modifyingUsers.push(undefined);
                }
              });
              this.changeDectectorRef.detectChanges();
            }
          })
      );
    }
  }
}
