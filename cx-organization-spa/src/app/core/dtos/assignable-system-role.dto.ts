import { Granted } from '../models/system-role-permission-subject.model';

export class AssignableSystemRole {
  id: number;
  name?: string;
  granted?: Granted;

  constructor(data?: AssignableSystemRole) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.name = data.name;
    this.granted = data.granted ? data.granted : Granted.Deny;
  }
}
