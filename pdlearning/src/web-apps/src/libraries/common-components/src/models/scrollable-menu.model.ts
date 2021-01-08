import { ElementRef } from '@angular/core';

export class ScrollableMenu {
  public id: string;
  public title: string;
  public elementFn: () => ElementRef;
  public isHidden?: () => boolean;
}
