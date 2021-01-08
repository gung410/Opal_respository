import {
  Component,
  OnInit,
  ViewEncapsulation,
  ChangeDetectorRef,
} from '@angular/core';
import { UserService } from 'app-services/user.service';
import { ActivatedRoute } from '@angular/router';
import { FilterParamModel } from '../staff.container/staff-list/models/filter-param.model';
import { ImageHelpers } from 'app-utilities/image-helpers';
import { StaffDetailDataService } from './staff-detail-data.service';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { IDPMode } from 'app/individual-development/idp.constant';
import { AuthService } from 'app-auth/auth.service';
import { Staff } from '../staff.container/staff-list/models/staff.model';
import { GroupStoreService } from 'app/core/store-services/group-store.service';
import { ApprovalGroup } from 'app-models/approval-group.model';
import { IdpAccessService } from 'app-services/idp-access.service';
import { UserStatusTypeEnum } from 'app/shared/constants/user-status-type.enum';

@Component({
  selector: 'staff-detail',
  templateUrl: './staff-detail.component.html',
  styleUrls: ['./staff-detail.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [StaffDetailDataService],
})
export class StaffDetailComponent extends BaseScreenComponent
  implements OnInit {
  public staffDetail: Staff;
  public mode: IDPMode;
  private currentUserApprovalGroups: ApprovalGroup[];
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    authService: AuthService,
    private route: ActivatedRoute,
    private userService: UserService,
    private groupStoreService: GroupStoreService,
    private idpAccessService: IdpAccessService
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    window.scroll(0, 0);
    this.route.params.subscribe(async (routeParams) => {
      const viewingUserId = routeParams.id;
      const filterEntityStatuses = [
        UserStatusTypeEnum.Active,
        UserStatusTypeEnum.New,
        UserStatusTypeEnum.Deactive,
        UserStatusTypeEnum.Inactive,
        UserStatusTypeEnum.IdentityServerLocked,
        UserStatusTypeEnum.Archived,
      ];
      const filterParam = new FilterParamModel({
        userIds: [viewingUserId],
        entityStatuses: filterEntityStatuses,
      });
      this.userService.getListEmployee(filterParam).subscribe((employees) => {
        if (!employees || !employees.items || !employees.items.length) {
          return;
        }
        this.staffDetail = employees.items[0];

        this.handleCheckApprovalGroups();
      });
    });
  }

  private handleCheckApprovalGroups(): void {
    this.groupStoreService
      .getApprovalGroups(this.currentUser.id)
      .subscribe((currentUserApprovalGroups) => {
        this.currentUserApprovalGroups = currentUserApprovalGroups;
        this.checkIDPMode();
      });
  }

  private checkIDPMode(): void {
    const isCurrentUserIsReportingOfficer =
      this.idpAccessService.userCanDecideApproval(this.currentUser) ||
      (this.idpAccessService.staffUnderAnyApprovingOfficers(this.staffDetail) &&
        this.idpAccessService.userIsApprovingOfficerOfStaff(
          this.currentUserApprovalGroups,
          this.staffDetail
        ));
    this.mode = isCurrentUserIsReportingOfficer
      ? IDPMode.ReportingOfficer
      : IDPMode.Normal;
  }
}
