import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'reverse' })
export class ReversePipe implements PipeTransform {
  public transform(value: unknown[]): unknown[] {
    return value.slice().reverse();
  }
}
