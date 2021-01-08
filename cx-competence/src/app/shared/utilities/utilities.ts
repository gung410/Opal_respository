import * as moment from 'moment';

import { AppConstant } from '../app.constant';

export class Utilities {
  static formatDateToISO(date: string): string {
    return moment(date, AppConstant.backendDateFormat).format();
  }

  static formatEndDateToISO(date: string): string {
    return moment(date, AppConstant.backendDateFormat)
      .add(1, 'day')
      .add(-1, 'seconds')
      .format();
  }

  static generateGUID(): string {
    const hexNumber = 16;
    const hexVar1 = 0x3;
    const hexVar2 = 0x8;

    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      // tslint:disable: no-bitwise
      const r = (Math.random() * hexNumber) | 0;
      const v = c === 'x' ? r : (r & hexVar1) | hexVar2;

      return v.toString(hexNumber);
    });
  }

  static getCurrentDateISOString(): string {
    return new Date().toISOString();
  }
}
