import { SearchClassRunType } from '@opal20/domain-api';
import { SessionDetailMode } from '@opal20/domain-components';

/**
 * Please dont change this file. If you want change it, you must update deeplink for Front-end/Back-end
 */
export class SessionDetailPageInput {
  public mode: SessionDetailMode;
  public id?: string;
  public courseId?: string;
  public classRunId?: string;
  public classRunSearchType?: SearchClassRunType;
  public isRescheduleMode: boolean;
}
