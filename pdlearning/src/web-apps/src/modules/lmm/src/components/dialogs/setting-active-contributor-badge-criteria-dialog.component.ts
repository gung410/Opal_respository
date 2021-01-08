import {
  ActiveContributorsBadgeCriteria,
  BadgeId,
  BadgeLevelEnum,
  BadgeRepository,
  BadgeType,
  BadgeWithCriteria
} from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription, combineLatest } from 'rxjs';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { IOpalSelectDefaultItem } from '@opal20/common-components';

@Component({
  selector: 'setting-active-contributor-badge-criteria-dialog',
  templateUrl: './setting-active-contributor-badge-criteria-dialog.component.html'
})
export class SettingActiveContributorBadgeCriteriaDialogComponent extends BaseComponent {
  public badgeLevelItems: IOpalSelectDefaultItem<string>[] = [
    {
      value: BadgeLevelEnum.Level1,
      label: this.translate('Level 1')
    },
    {
      value: BadgeLevelEnum.Level2,
      label: this.translate('Level 2')
    },
    {
      value: BadgeLevelEnum.Level3,
      label: this.translate('Level 3')
    },
    {
      value: null,
      label: this.translate('Not Applicable')
    }
  ];

  public communityBadgesItems = [];
  public badge: BadgeWithCriteria<ActiveContributorsBadgeCriteria> = new BadgeWithCriteria();
  private _loadDataSub: Subscription = new Subscription();

  constructor(protected moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef, private badgeRepository: BadgeRepository) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onSetup(): void {
    this.badgeRepository
      .saveActiveContributorCriteria({
        criteria: this.badge.criteria
      })
      .pipe(this.untilDestroy())
      .subscribe(() => {
        this.dialogRef.close();
      });
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();

    this._loadDataSub = combineLatest(
      this.badgeRepository.getBadgeById<ActiveContributorsBadgeCriteria>(BadgeId.ActiveContributor),
      this.badgeRepository.getAllBadges()
    )
      .pipe(this.untilDestroy())
      .subscribe(([badge, badges]) => {
        this.badge = badge;
        this.communityBadgesItems = badges.filter(x => x.type === BadgeType.Tag);
      });
  }

  protected onInit(): void {
    this.loadData();
  }
}
