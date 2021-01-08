import { Pipe, PipeTransform } from '@angular/core';

/**
 * @return Length of chapter with format mm:ss
 */
@Pipe({ name: 'secondToTimeFormat' })
export class SecondToTimePipe implements PipeTransform {
  public transform(value: number): string {
    const minutes = Math.floor(value / 60);
    const minuteValue = minutes;
    const secondValue = value - minutes * 60;
    return `${this.pad(minuteValue)}:${this.pad(secondValue)}`;
  }

  private pad(value: number): string {
    return value > 9 ? value.toString() : '0' + value;
  }
}
