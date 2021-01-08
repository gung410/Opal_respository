import { trigger, transition, style, animate } from '@angular/animations';

export const CxAnimations = {
    smoothAppendRemove: trigger('smoothAppendRemove', [
      transition(':enter', [
        style({opacity: '0', height: '0'}),
        animate('200ms ease-in', style({opacity: '1', height: '*'}))
      ]),
      transition(':leave', [
        style({opacity: '1', height: '*'}),
        animate('200ms ease-in', style({opacity: '0', height: '0'}))
      ])
    ])
}