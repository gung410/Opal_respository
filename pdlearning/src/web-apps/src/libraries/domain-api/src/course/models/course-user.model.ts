export interface ICourseUser {
  id: string;
  originalUserId: number;
  firstName: string;
  lastName: string;
  emailAddress: string;
  departmentId: number;
}

export class CourseUser implements ICourseUser {
  public id: string;
  public originalUserId: number;
  public firstName: string;
  public lastName: string;
  public emailAddress: string;
  public departmentId: number;
  public get fullName(): string {
    return (this.firstName ? this.firstName : '') + (this.lastName ? this.lastName : '');
  }

  constructor(data?: ICourseUser) {
    if (data == null) {
      return;
    }

    this.id = data.id;
    this.originalUserId = data.originalUserId;
    this.firstName = data.firstName;
    this.lastName = data.lastName;
    this.emailAddress = data.emailAddress;
    this.departmentId = data.departmentId;
  }
}
