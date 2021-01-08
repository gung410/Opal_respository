import { GlobalTranslatorService } from '../translation/global-translator.service';

export class TimeUtils {
  public static displayDurationInHoursAndMinutes(durationInMinutes: number, translator: GlobalTranslatorService): string {
    if (durationInMinutes === 0) {
      return translator.translate('##minutes## minute(s)', { minutes: 0 });
    }

    const hours: number = Math.floor(durationInMinutes / 60);
    const spareMinutes = durationInMinutes - hours * 60;

    if (hours === 0) {
      return translator.translate('##minutes## min(s)', { minutes: spareMinutes });
    }

    let result: string = translator.translate('##hours## hour(s)', { hours: hours });

    if (spareMinutes > 0) {
      result = `${result} ${translator.translate('##minutes## min(s)', { minutes: spareMinutes })}`;
    }

    return result;
  }

  public static displayDurationInHAndM(durationInMinutes: number): string {
    if (durationInMinutes === 0) {
      return '0M';
    }

    const hours: number = Math.floor(durationInMinutes / 60);
    const spareMinutes = durationInMinutes - hours * 60;

    if (hours === 0) {
      return spareMinutes + 'M';
    }

    let result: string = hours + 'H';

    if (spareMinutes > 0) {
      result = `${result} ${spareMinutes}M`;
    }

    return result;
  }
}
