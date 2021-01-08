import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  ViewEncapsulation
} from '@angular/core';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import * as moment from 'moment';

import { AppConstant } from 'app/shared/app.constant';
import { AuditActionType } from '../constants/user-field-mapping.constant';
import { AuditHistory } from '../models/audit-history.model';

@Component({
  selector: 'audit-history',
  templateUrl: './audit-history.component.html',
  styleUrls: ['./audit-history.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuditHistoryComponent extends BaseSmartComponent {
  @Input() auditHistories: AuditHistory[];
  @Input() displayTemplate: (key: any, value: any) => string;
  @Input() displaySubTemplate: (auditHistory: AuditHistory) => string;
  @Input() subLabelConstant: any;
  readonly ACCOUNT_CREATED_TYPE: AuditActionType =
    AuditActionType.AccountCreated;
  defaultActionUserAvatar: string = AppConstant.defaultAvatar;
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  /**
   * Gets display date time.
   * @param date The date string in ISO format.
   */
  getDisplayDateTime(date: string): string {
    const dateOfEvent = new Date(date);
    const yesterday = moment(new Date().toISOString());
    yesterday.subtract(1, 'days');
    if (dateOfEvent.valueOf() > yesterday.valueOf()) {
      return moment(date).fromNow();
    }
    return moment(date).format(AppConstant.dateTimeFormat);
  }

  getDateTimeTooltip(date: string): string {
    return `${moment(date).fromNow()} - ${moment(date).format('lll')}`;
  }
}
