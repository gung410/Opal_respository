import { Location } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { ApprovalTargetEnum } from 'app/approval-page/models/approval.enum';
import { ApprovalRequestTargetItem } from 'app/approval-page/models/approval.model';
import { AppConstant } from 'app/shared/app.constant';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { CommonHelpers } from 'app/shared/helpers/common.helpers';
import { ApprovalConstants } from '../helpers/approval-page.constant';
@Component({
  selector: 'approval-bar',
  templateUrl: './approval-bar.component.html',
  styleUrls: ['./approval-bar.component.scss'],
})
export class ApprovalBarComponent extends BaseComponent implements OnInit {
  @Input() showApprovalButton: boolean;
  @Input() enableApprovalButton: boolean;
  @Input() approveButtonText: string = 'Common.Action.Approve';
  @Output() approve: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() reject: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output()
  changeApprovalTarget: EventEmitter<ApprovalTargetEnum> = new EventEmitter<ApprovalTargetEnum>();

  // Main variables
  approvalTargets: ApprovalRequestTargetItem[];
  subApprovalTargets: ApprovalRequestTargetItem[];
  currentUserRoles: string[] = [];
  currentUser: User;
  defaultApprovalTarget: ApprovalTargetEnum;
  defaultSubApprovalTarget: ApprovalTargetEnum;

  // Secondary variables
  currentApprovalTarget: ApprovalRequestTargetItem;
  currentApprovalSubTarget: ApprovalRequestTargetItem;

  // Filters
  @Input() showFilter: boolean;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    protected authService: AuthService,
    private location: Location,
    protected changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
    this.subscribeUserData();
    this.setDefaultApprovalTarget();
  }

  ngOnInit(): void {
    this.initData();
  }

  onClickedApprove(): void {
    this.approve.emit();
  }

  onClickedReject(): void {
    this.reject.emit();
  }

  onApprovalTargetClicked(item: ApprovalRequestTargetItem): void {
    this.setActiveTarget(item);
  }

  onClickedApprovalSubTarget(item: ApprovalRequestTargetItem): void {
    this.setApprovalSubTarget(item);
  }

  get isIDPMode(): boolean {
    return this.router.url.includes(
      `/${AppConstant.siteURL.menus.pendingRequestIDP}`
    );
  }

  /**
   * From the url segments, the system will select a specific tab by default.
   * e.g: If the user browses to url "/pending-request-idp/class-change-request",
   * then the "Class Change Request" tab should be chosen by default.
   */
  private setDefaultApprovalTarget(): void {
    const params = this.activatedRoute.snapshot.params;
    const target = params.target as string;
    const subTarget = params.subTarget as string;

    if (!this.defaultApprovalTarget && target) {
      const defaultApprovalTarget = CommonHelpers.toEnumIgnoreCase(
        ApprovalTargetEnum,
        target
      );
      if (defaultApprovalTarget) {
        this.defaultApprovalTarget = defaultApprovalTarget;
      }
    }

    if (!this.defaultSubApprovalTarget && subTarget) {
      const defaultSubApprovalTarget = CommonHelpers.toEnumIgnoreCase(
        ApprovalTargetEnum,
        subTarget
      );
      if (defaultSubApprovalTarget) {
        this.defaultSubApprovalTarget = defaultSubApprovalTarget;
      }
    }
  }

  private subscribeUserData(): void {
    this.subscriptionAdder = this.authService.userData().subscribe((u) => {
      this.currentUser = u;
      if (this.currentUserRoles) {
        this.currentUserRoles = this.currentUser.systemRoles.map(
          (role) => role.identity.extId
        );
      }
    });
  }

  private initData(): void {
    this.setTargetsByCurrentUserRole();
  }

  private setTargetsByCurrentUserRole(): void {
    const targets = this.isIDPMode
      ? ApprovalConstants.IDP_APPROVAL_REQUEST_TARGET_ITEMS
      : ApprovalConstants.ODP_APPROVAL_REQUEST_TARGET_ITEMS;

    this.approvalTargets = targets.filter((target) =>
      target.hasPerrmission(this.currentUser)
    );

    const defaultApprovalRequestTarget = this.getDefaultApprovalRequestTarget(
      this.approvalTargets,
      this.defaultApprovalTarget
    );
    this.setActiveTarget(defaultApprovalRequestTarget, true);
  }

  private getDefaultApprovalRequestTarget(
    availableApprovalRequestTargets: ApprovalRequestTargetItem[],
    defaultApprovalTarget: ApprovalTargetEnum
  ): ApprovalRequestTargetItem {
    const defaultApprovalRequestTarget = availableApprovalRequestTargets.find(
      (t) => t.target === defaultApprovalTarget
    );

    return defaultApprovalRequestTarget
      ? defaultApprovalRequestTarget
      : availableApprovalRequestTargets[0];
  }

  private setActiveTarget(
    item: ApprovalRequestTargetItem,
    onSetDefaultValue: boolean = false
  ): void {
    if (this.currentApprovalTarget === item) {
      return;
    }

    this.currentApprovalTarget = item;

    if (item.target === ApprovalTargetEnum.Nominations) {
      this.subApprovalTargets =
        ApprovalConstants.NOMINATION_APPROVAL_SUB_TARGET_ITEMS;

      this.subApprovalTargets = this.subApprovalTargets.filter((target) =>
        target.hasPerrmission(this.currentUser)
      );
      let subTargetItem = this.subApprovalTargets[0];
      if (onSetDefaultValue && this.defaultSubApprovalTarget) {
        const defaultSubTarget = this.subApprovalTargets.find(
          (i) => i.target === this.defaultSubApprovalTarget
        );
        subTargetItem = defaultSubTarget || subTargetItem;
      }

      return this.setApprovalSubTarget(subTargetItem);
    }

    if (item.target === ApprovalTargetEnum.AdhocNominations) {
      this.subApprovalTargets =
        ApprovalConstants.ADHOC_NOMINATION_APPROVAL_SUB_TARGET_ITEMS;

      this.subApprovalTargets = this.subApprovalTargets.filter((target) =>
        target.hasPerrmission(this.currentUser)
      );
      let subTargetItem = this.subApprovalTargets[0];
      if (onSetDefaultValue && this.defaultSubApprovalTarget) {
        const defaultSubTarget = this.subApprovalTargets.find(
          (i) => i.target === this.defaultSubApprovalTarget
        );
        subTargetItem = defaultSubTarget || subTargetItem;
      }

      return this.setApprovalSubTarget(subTargetItem);
    }

    this.updateCurrentLocation(item.target);
    this.changeApprovalTarget.emit(item.target);
    this.subApprovalTargets = undefined;
    this.currentApprovalSubTarget = undefined;
  }

  private setApprovalSubTarget(item: ApprovalRequestTargetItem): void {
    if (this.currentApprovalSubTarget === item) {
      return;
    }
    this.updateCurrentLocation(this.currentApprovalTarget.target, item.target);
    this.currentApprovalSubTarget = item;
    this.changeApprovalTarget.emit(item.target);
  }

  private updateCurrentLocation(
    currentTab: ApprovalTargetEnum,
    currentSubTab?: ApprovalTargetEnum
  ): void {
    const location = this.isIDPMode
      ? `${AppConstant.siteURL.menus.pendingRequestIDP}/${currentTab}${
          currentSubTab ? `/${currentSubTab}` : ''
        }`
      : `${AppConstant.siteURL.menus.pendingRequestODP}/${currentTab}${
          currentSubTab ? `/${currentSubTab}` : ''
        }`;
    this.location.go(location);
  }
}
