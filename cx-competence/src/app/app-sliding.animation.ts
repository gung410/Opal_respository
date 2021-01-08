import {
  transition,
  trigger,
  query,
  style,
  animate,
  group,
  animateChild,
  state,
} from '@angular/animations';

const slideLeftToRightValue = [
  query(':enter, :leave', style({ position: 'fixed', width: '100%' }), {
    optional: true,
  }),
  group([
    query(
      ':enter',
      [
        style({ transform: 'translateX(-100%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateX(0%)' })),
      ],
      { optional: true }
    ),
    query(
      ':leave',
      [
        style({ transform: 'translateX(0%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateX(100%)' })),
      ],
      { optional: true }
    ),
  ]),
];

const slideRightToLeftValue = [
  query(':enter, :leave', style({ position: 'fixed', width: '100%' }), {
    optional: true,
  }),
  group([
    query(
      ':enter',
      [
        style({ transform: 'translateX(100%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateX(0%)' })),
      ],
      { optional: true }
    ),
    query(
      ':leave',
      [
        style({ transform: 'translateX(0%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateX(-100%)' })),
      ],
      { optional: true }
    ),
  ]),
];

const slideTopToBottomValue = [
  query(':enter, :leave', style({ position: 'fixed', width: '100%' }), {
    optional: true,
  }),
  group([
    query(
      ':enter',
      [
        style({ transform: 'translateY(100%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateY(0%)' })),
      ],
      { optional: true }
    ),
    query(
      ':leave',
      [
        style({ transform: 'translateY(0%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateY(-100%)' })),
      ],
      { optional: true }
    ),
  ]),
];

const slideBottomToTopValue = [
  query(':enter, :leave', style({ position: 'fixed', width: '100%' }), {
    optional: true,
  }),
  group([
    query(
      ':enter',
      [
        style({ transform: 'translateY(-100%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateY(0%)' })),
      ],
      { optional: true }
    ),
    query(
      ':leave',
      [
        style({ transform: 'translateY(0%)' }),
        animate('0.3s ease-in-out', style({ transform: 'translateY(100%)' })),
      ],
      { optional: true }
    ),
  ]),
];

export const pageSlideAnimation = trigger('pageSlideAnimation', [
  transition('PDPlanner => *', slideRightToLeftValue),
  transition('LearningNeed => *', slideLeftToRightValue),
  transition('PlannedActivities => *', slideLeftToRightValue),
  transition('LearningNeedAnalysis => *', slideLeftToRightValue),
]);
