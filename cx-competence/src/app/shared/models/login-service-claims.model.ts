export class LoginServiceClaim {
  public id: number;
  public value: string;
  constructor(data?: LoginServiceClaim) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.value = data.value;
  }
}
