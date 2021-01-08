import { ObjectUtilities } from 'app-utilities/object-utils';

export class AssignResultModel {
  avatar: string;
  name: string;
  email: string;
  isSuccess: boolean;
  reason: string;
  constructor(data?: Partial<AssignResultModel>) {
    if (!data) {
      return;
    }
    ObjectUtilities.copyPartialObject(data, this);
  }
}
