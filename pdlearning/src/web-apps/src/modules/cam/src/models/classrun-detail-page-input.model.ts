import { ClassRunDetailMode } from '@opal20/domain-components';
import { SearchClassRunType } from '@opal20/domain-api';

/**
 * Please dont change this file. If you want change it, you must update deeplink for Front-end/Back-end
 */
export class ClassRunDetailPageInput {
  public mode: ClassRunDetailMode;
  public searchType?: SearchClassRunType;
  public id?: string;
  public courseId?: string;
}
