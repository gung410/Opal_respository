import { Buffer } from 'buffer';
import { environment } from '../environments/environment';
import { v4 as uuidv4 } from 'uuid';

export function getModuleIdFromUrl(baseHref: string): string {
  const pathName: string = document.location.pathname;

  if (pathName.startsWith(`${baseHref}index.html`)) {
    return '';
  }

  if (pathName.startsWith('/app/')) {
    return pathName.slice(5, pathName.length);
  }

  if (pathName.startsWith('/app')) {
    return pathName.slice(4, pathName.length);
  }

  return pathName.slice(1, pathName.length);
}

function getBaseHref(): string {
  const base: HTMLBaseElement | null = document.querySelector('base');

  if (base) {
    return base.href.replace(document.location.origin, '');
  }

  return '/';
}

function initializeWindowContext(): IWindow {
  const windowContext: IWindow = <IWindow>(<unknown>window);
  // Declare systemjs mapping;
  windowContext.System.define = windowContext.define;
  windowContext.SystemJs = windowContext.System;
  windowContext.Buffer = Buffer;

  // Declare global app setting.
  windowContext.AppGlobal = {
    environment,
    libraries: new Map(),
    registrationModules: [],
    accessibleModules: [],
    mode: 'aot',
    router: new Router(),
    quizPlayerIntegrations: {
      setFormId: (formId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentFormId = formId;
      },
      setFormOriginalObjectId: (formOriginalObjectId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentFormOriginalObjectId = formOriginalObjectId;
      },
      setResourceId: (resourceId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentResourceId = resourceId;
      },
      setClassRunId: (classRunId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentClassRunId = classRunId;
      },
      setMyCourseId: (myCourseId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentMyCourseId = myCourseId;
      },
      setAssignmentId: (assignmentId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentAssignmentId = assignmentId;
      },
      setPassingRateEnableStringValue: (value: 'true' | 'false') => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        windowContext.AppGlobal.quizPlayerIntegrations.isPassingMarkEnabled = enable;
      },
      setReviewOnlyStringValue: (value: 'true' | 'false') => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        windowContext.AppGlobal.quizPlayerIntegrations.reviewOnly = enable;
      },
      setAuthToken: (authToken: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentAuthToken = authToken;
      },
      onQuizFinished: () => {
        console.log('quiz-player-finished');
      },
      onQuizExited: () => {
        console.log('quiz-player-exited');
      },
      onQuizFinishedForMobile: () => {
        console.log('quiz-player-finished-mobile');
      },
      onQuizSubmitted: () => {
        console.log('quiz-player-submitted');
      },
      onQuizTimeout: () => {
        console.log('quiz-player-timeout');
      },
      onQuizInitiated: () => {
        console.log('quiz-player-initiated');
      },
      onAuthTokenError: undefined,
      currentAuthToken: '',
      currentFormId: '',
      currentResourceId: null,
      currentClassRunId: null,
      currentMyCourseId: null,
      currentAssignmentId: null,
      isPassingMarkEnabled: null,
      reviewOnly: false,
      currentFormOriginalObjectId: ''
    },
    standaloneSurveyIntegration: {
      setFormId: (formId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentFormId = formId;
      },
      setFormOriginalObjectId: (formOriginalObjectId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentFormOriginalObjectId = formOriginalObjectId;
      },
      setResourceId: (resourceId: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentResourceId = resourceId;
      },
      setPassingRateEnableStringValue: (value: 'true' | 'false') => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        windowContext.AppGlobal.quizPlayerIntegrations.isPassingMarkEnabled = enable;
      },
      setReviewOnlyStringValue: (value: 'true' | 'false') => {
        const enable = value === 'true' || value === 'false' ? JSON.parse(value) : true;
        windowContext.AppGlobal.quizPlayerIntegrations.reviewOnly = enable;
      },
      setAuthToken: (authToken: string) => {
        windowContext.AppGlobal.quizPlayerIntegrations.currentAuthToken = authToken;
      },
      onQuizFinished: () => {
        console.log('quiz-player-finished');
      },
      onQuizExited: () => {
        console.log('quiz-player-exited');
      },
      onQuizFinishedForMobile: () => {
        console.log('quiz-player-finished-mobile');
      },
      onQuizSubmitted: () => {
        console.log('quiz-player-submitted');
      },
      onQuizTimeout: () => {
        console.log('quiz-player-timeout');
      },
      onQuizInitiated: () => {
        console.log('quiz-player-initiated');
      },
      onAuthTokenError: undefined,
      currentAuthToken: '',
      currentFormId: '',
      currentResourceId: null,
      isPassingMarkEnabled: null,
      reviewOnly: false,
      currentFormOriginalObjectId: ''
    },
    getModuleIdFromUrl: getModuleIdFromUrl,
    getBaseHref: getBaseHref,
    scormPlayerIntegrations: {},
    digitalContentPlayerIntergrations: {},
    videoAnnotationPlayerIntergrations: {},
    communityMetadataIntergrations: {},
    calendarIntergrations: {},
    standaloneSurveyManagementIntegrations: {},
    assignmentPlayerIntegrations: {
      setAssignmentId: (assignmentId: string) => {
        windowContext.AppGlobal.assignmentPlayerIntegrations.currentAssignmentId = assignmentId;
      },
      setParticipantAssignmentTrackId: (participantAssignmentTrackId: string) => {
        windowContext.AppGlobal.assignmentPlayerIntegrations.currentParticipantAssignmentTrackId = participantAssignmentTrackId;
      },
      setIsMobile: (isMobileApp: string) => {
        windowContext.AppGlobal.assignmentPlayerIntegrations.isMobile = isMobileApp;
      },
      setAuthToken: (authToken: string) => {
        windowContext.AppGlobal.assignmentPlayerIntegrations.currentAuthToken = authToken;
      },
      onAssignmentInitiated: () => {
        console.log('assignment-player-initiated');
      },
      onAssignmentSaved: () => {
        console.log('assignment-player-saved');
      },
      onAssignmentSubmitted: () => {
        console.log('assignment-player-submitted');
      },
      onAssignmentBack: () => {
        console.log('assignment-player-back');
      },
      onAssignmentQuestionChanged: () => {
        console.log('assignment-question-changed');
      },
      onAuthTokenError: undefined,
      currentAuthToken: '',
      currentAssignmentId: null,
      currentParticipantAssignmentTrackId: null,
      isMobile: undefined
    },
    assessmentPlayerIntegrations: {
      setAssessmentId: (assessmentId: string) => {
        windowContext.AppGlobal.assessmentPlayerIntegrations.currentAssessmentId = assessmentId;
      },
      setAssessmentAnswerId: (assessmentAnswerId: string) => {
        windowContext.AppGlobal.assessmentPlayerIntegrations.currentParticipantAssignmentTrackId = assessmentAnswerId;
      },
      setParticipantAssignmentTrackId: (participantAssignmentTrackId: string) => {
        windowContext.AppGlobal.assessmentPlayerIntegrations.currentParticipantAssignmentTrackId = participantAssignmentTrackId;
      },
      setUserId: (userId: string) => {
        windowContext.AppGlobal.assessmentPlayerIntegrations.currentUserId = userId;
      },
      setIsMobile: (isMobileApp: string) => {
        windowContext.AppGlobal.assessmentPlayerIntegrations.isMobile = isMobileApp;
      },
      setAuthToken: (authToken: string) => {
        windowContext.AppGlobal.assessmentPlayerIntegrations.currentAuthToken = authToken;
      },
      onAssessmentInitiated: () => {
        console.log('assessment-player-initiated');
      },
      onAssessmentSaved: () => {
        console.log('assessment-player-saved');
      },
      onAssessmentSubmitted: () => {
        console.log('assessment-player-submitted');
      },
      onAssessmentBack: () => {
        console.log('assessment-player-back');
      },
      onAuthTokenError: undefined,
      currentAuthToken: '',
      currentAssessmentId: null,
      currentAssessmentAnswerId: null,
      currentParticipantAssignmentTrackId: null,
      currentUserId: null,
      isMobile: undefined
    },
    sessionUUID: uuidv4()
  };
  AppGlobal = windowContext.AppGlobal;

  // Initialize router.
  AppGlobal.router.configure({
    html5history: true,
    convert_hash_in_init: true,
    run_handler_in_init: false,
    async: false,
    strict: false,
    notfound: () => {
      const baseHref = document.querySelector('base').getAttribute('href');

      if (location.pathname === `${baseHref}index.html`) {
        history.back();
        return;
      }

      if (AppGlobal.getModuleIdFromUrl('') === 'common') {
        return;
      }

      if (AppGlobal.accessibleModules.findIndex(m => m.id === 'learner') > -1) {
        AppGlobal.router.setRoute('learner');
        return;
      }

      AppGlobal.router.setRoute('common');
    }
  });

  return windowContext;
}

export { initializeWindowContext };
