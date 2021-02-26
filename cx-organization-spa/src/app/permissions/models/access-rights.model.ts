import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { Utils } from '../../shared/utilities/utils';
import { GrantedType } from '../enum/granted-type.enum';
import { ObjectType } from '../enum/object-type.enum';

export class AccessRightsModel {
  id: number;
  action?: string;
  objectType: ObjectType;
  module?: string;
  grantedType: GrantedType;
  parentId?: number;
  no?: number;
  hideConfiguration?: boolean;
  localizedData?: LocalizedDataItem[];

  constructor(data?: AccessRightsModel) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.action = data.action;
    this.objectType = data.objectType;
    this.module = data.module;
    this.grantedType = data.grantedType;
    this.parentId = data.parentId;
    this.no = data.no;
    this.localizedData = Utils.cloneDeep(data.localizedData);
    this.hideConfiguration = data.hideConfiguration;
  }
}
