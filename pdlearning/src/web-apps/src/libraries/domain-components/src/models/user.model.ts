export interface IUserModel {
  id: string;
  originalUserId: number;
  firstName: string;
  lastName: string;
  email: string;
  departmentId: string;
}

export class UserModel implements IUserModel {
  public id: string;
  public originalUserId: number;
  public firstName: string;
  public lastName: string;
  public email: string;
  public departmentId: string;
  constructor(data?: IUserModel) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.originalUserId = data.originalUserId;
    this.firstName = data.firstName;
    this.lastName = data.lastName;
    this.email = data.email;
    this.departmentId = data.departmentId;
  }

  public get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}
