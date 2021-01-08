// tslint:disable:max-line-length

export const REGISTRATION_MODULES: IRegistrationModule[] = [
  {
    id: 'dashboard',
    name: 'OPAL 2.0 Dashboard',
    shortName: 'Dashboard',
    description: 'This is OPAL 2.0 Dashboard.',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.dashboard"*/ '../../modules/dashboard/src/dashboard.module.ngfactory')
  },
  {
    id: 'ccpm',
    name: 'Content Creation & Publishing',
    shortName: 'CCPM',
    description: 'This is Content Creation & Publishing module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.ccpm"*/ '../../modules/ccpm/src/ccpm.module.ngfactory')
  },
  {
    id: 'lmm',
    name: 'Learning Management',
    shortName: 'LMM',
    description: 'This is Learning Management module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.lmm"*/ '../../modules/lmm/src/lmm.module.ngfactory')
  },
  {
    id: 'learner',
    name: 'Learner',
    shortName: 'Learner',
    description: 'This is Learner module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.learner"*/ '../../modules/learner/src/learner.module.ngfactory')
  },
  {
    id: 'quiz-player',
    name: 'Quiz Player',
    shortName: 'Quiz Player',
    description: 'This is Quiz Player module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.quiz-player"*/ '../../players/quiz-player/src/quiz-player.module.ngfactory')
  },
  {
    id: 'scorm-player',
    name: 'Scorm Player',
    shortName: 'Scorm Player',
    description: 'This is Scorm Player module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.scorm-player"*/ '../../players/scorm-player-integrator/src/scorm-player-integrator.module.ngfactory')
  },
  {
    id: 'digital-content-player',
    name: 'Digital Content Player',
    shortName: 'Digital Content ',
    description: 'This is Digital Content module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.digital-content-player"*/ '../../players/digital-content-player/src/digital-content-player.module.ngfactory')
  },
  {
    id: 'community-metadata',
    name: 'Community Metadata',
    shortName: 'Community Metadata',
    description: 'Community Metadata',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.community-metadata"*/ '../../players/community-metadata/src/community-metadata.module.ngfactory')
  },
  {
    id: 'calendar',
    name: 'calendar',
    shortName: 'calendar',
    description: 'calendar',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.calendar"*/ '../../players/calendar/src/calendar.module.ngfactory')
  },
  {
    id: 'poc',
    name: 'Framework Examples',
    shortName: 'POC',
    description: 'This is Proof of Concept module',
    development: true,
    loadNgModule: () => import(/*webpackChunkName:"opal20.poc"*/ '../../modules/poc/src/poc.module.ngfactory')
  },
  {
    id: 'cam',
    name: 'Course Admin Management',
    shortName: 'CAM',
    description: 'This is Course Admin Management module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.cam"*/ '../../modules/cam/src/cam.module.ngfactory')
  },
  {
    id: 'common',
    name: 'Common',
    shortName: 'Common',
    description: 'This is Common module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.common"*/ '../../modules/common/src/common.module.ngfactory')
  },
  {
    id: 'assignment-player',
    name: 'Assignment Player',
    shortName: 'Assignment Player',
    description: 'This is Assignment Player module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.assignment-player"*/ '../../players/assignment-player/src/assignment-player.module.ngfactory')
  },
  {
    id: 'form-standalone',
    name: 'Form Standalone',
    shortName: 'Form Standalone',
    description: 'This is Form Standalone player',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.form-standalone"*/ '../../players/form-standalone/src/form-standalone.module.ngfactory')
  },
  {
    id: 'form-standalone-player',
    name: 'Form Standalone Player',
    shortName: 'Form Standalone Player',
    description: 'This is Form Standalone player',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.form-standalone-player"*/ '../../players/form-standalone-player/src/form-standalone-player.module.ngfactory')
  },
  {
    id: 'video-annotation-player',
    name: 'Video Annotation Player',
    shortName: 'Video Annotation',
    description: 'This is Video Annotation module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.video-annotation-player"*/ '../../players/video-annotation-player/src/video-annotation-player.module.ngfactory')
  },
  {
    id: 'assessment-player',
    name: 'Assessment Player',
    shortName: 'Assessment Player',
    description: 'This is Assessment Player module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.assessment-player"*/ '../../players/assessment-player/src/assessment-player.module.ngfactory')
  },
  {
    id: 'standalone-survey',
    name: 'Standalone Survey',
    shortName: 'Standalone Survey',
    description: 'This is Standalone Survey module',
    development: false,
    loadNgModule: () => import(/*webpackChunkName:"opal20.standalone-survey"*/ '../../players/standalone-survey/src/standalone-survey.module.ngfactory')
  }
];
