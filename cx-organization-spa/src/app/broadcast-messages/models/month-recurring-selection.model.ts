import { MonthRepetition } from '../constant/month-repetition.enum';

export class MonthRecurringSelection {
  monthRepetition: MonthRepetition;
  description: string;

  constructor(data?: MonthRecurringSelection) {
    if (!data) {
      return;
    }

    this.monthRepetition = data.monthRepetition;
    this.description = data.description;
  }
}
