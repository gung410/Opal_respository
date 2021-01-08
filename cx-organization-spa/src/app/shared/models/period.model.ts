export class Period {
  name: string;
  startDate: string;
  endDate: string;
  constructor(data?: Partial<Period>) {
    if (!data) {
      return;
    }
    this.name = data.name;
    this.startDate = data.startDate;
    this.endDate = data.endDate;
  }
}
