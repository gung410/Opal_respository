export interface IUserPreferenceModel {
  id: string;
  userId: string;
  key: string;
  value: string | number | boolean;
  valueType: UserPreferenceValueType;
}

export class UserPreferenceModel implements IUserPreferenceModel {
  public id: string;
  public userId: string;
  public key: string;
  public value: string | number | boolean;
  public valueType: UserPreferenceValueType;

  constructor(data?: IUserPreferenceModel) {
    if (data == null) {
      return;
    }

    this.id = data.id;
    this.userId = data.userId;
    this.key = data.key;
    this.value = data.value;
    this.valueType = data.valueType;
  }
}
export type UserPreferenceValueType = 'string' | 'number' | 'boolean';
