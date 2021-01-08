export class LoginServiceClaim {
  id: number;
  value: string;
  constructor(data?: LoginServiceClaim) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.value = data.value;
  }
}
