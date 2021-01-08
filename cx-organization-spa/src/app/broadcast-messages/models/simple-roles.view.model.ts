export class SimpleRoleViewModel {
  id?: number;
  roleName: string;
  constructor(data?: Partial<SimpleRoleViewModel>) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.roleName = data.roleName;
  }
}
