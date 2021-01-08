import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { AppConstant } from 'app/shared/app.constant';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { TargetUserType } from 'app/shared/constants/target-user-type.enum';
import {
  BroadcastMessagesDto,
  IBroadcastMessageDto,
  Recipients
} from './broadcast-messages.model';

export interface IBroadcastMessageViewModel extends IBroadcastMessageDto {
  createdBy: string;
  messageStatus: string;
  validFromDateLabel: string;
  validToDateLabel: string;
  recipientsLabel: string;
  createdDateLabel: string;
}

export class BroadcastMessageViewModel extends BroadcastMessagesDto {
  static readonly DEPARTMENTS: string = 'Department(s)';
  static readonly ROLES: string = 'Role(s)';
  static readonly GROUPS: string = 'Group(s)';
  static readonly USERS: string = 'User(s)';
  static readonly ALL_USERS: string = 'All users';
  static readonly EXTERNAL_USERS: string = 'External users';
  static readonly HRMS_USERS: string = 'HRMS users';
  static readonly RECIPIENTS_SEPARATOR: string = ' , ';

  static createFromModel(
    broadcastMessage: IBroadcastMessageDto,
    createdBy: string,
    messageStatus: string
  ): BroadcastMessageViewModel {
    return new BroadcastMessageViewModel({
      ...broadcastMessage,
      createdBy,
      messageStatus: broadcastMessage.status,
      validFromDateLabel: this.buildValidDateLabel(
        broadcastMessage.validFromDate
      ),
      validToDateLabel: this.buildValidDateLabel(broadcastMessage.validToDate),
      recipientsLabel: this.recipientsLabelBuilder(
        broadcastMessage.targetUserType,
        broadcastMessage.recipients
      ),
      createdDateLabel: DateTimeUtil.toDateString(broadcastMessage.createdDate)
    });
  }

  private static buildValidDateLabel(validDate: Date): string {
    return DateTimeUtil.toDateString(validDate, AppConstant.dateTimeFormat);
  }

  private static recipientsLabelBuilder(
    targetUserType: TargetUserType,
    recipients: Recipients
  ): string {
    switch (targetUserType) {
      case TargetUserType.AllUser:
        return this.ALL_USERS;
      case TargetUserType.ExternalUser:
        return this.EXTERNAL_USERS;
      case TargetUserType.HRMSUser:
        return this.HRMS_USERS;
      case TargetUserType.SpecificTargetUser: {
        const specificRecipients: string[] = [];
        if (recipients.departmentIds.length) {
          specificRecipients.push(this.DEPARTMENTS);
        }
        if (recipients.groupIds.length) {
          specificRecipients.push(this.GROUPS);
        }
        if (recipients.roleIds.length) {
          specificRecipients.push(this.ROLES);
        }
        if (recipients.userIds.length) {
          specificRecipients.push(this.USERS);
        }

        return specificRecipients.join(this.RECIPIENTS_SEPARATOR);
      }
      default:
        return;
    }
  }

  validFromDateLabel: string;
  validToDateLabel: string;
  createdBy: string;
  messageStatus: string;
  recipientsLabel: string;
  createdDateLabel: string;

  constructor(data?: IBroadcastMessageViewModel) {
    super(data);
    if (data != null) {
      this.createdBy = data.createdBy;
      this.messageStatus = data.messageStatus;
      this.validFromDateLabel = data.validFromDateLabel;
      this.validToDateLabel = data.validToDateLabel;
      this.recipientsLabel = data.recipientsLabel;
      this.createdDateLabel = data.createdDateLabel;
    }
  }
}
