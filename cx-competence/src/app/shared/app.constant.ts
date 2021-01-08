import { environment } from 'app-environments/environment';

export const AppConstant = {
  api: {
    organization: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.organization}`,
    assessment: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.assessment}`,
    portal: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.portal}`,
    competence: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.competence}`,
    communication: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.communication}`,
    learningcatalog: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.learningcatalog}`,
    coursepad: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.coursepad}`,
  },
  clientId: environment.clientId,
  customerId: environment.CustomerId,
  cxId: {
    ProfileUrl: `${environment.issuer}/manage/index`,
  },
  menuType: {
    MENU_ITEM: 'Default',
    APP_SWITCHER: 'AppSwitcher',
  },
  pendingUserErrorCode: 'ERROR_USER_NOT_ACTIVE',
  siteURL: {
    login: 'login',
    error: 'error',
    management: 'management',
    monitor: 'monitor',
    sessionTimeout: 'session-timeout',
    profile: 'profile',
    menus: {
      myCalendar: 'my-calendar',
      teamCalendar: 'team-calendar',
      listEmployee: 'employee',
      pendingRequestIDP: 'pending-request-idp',
      pendingRequestODP: 'pending-request-odp',
      mpj: 'mpj',
      odp: 'odp',
      strategicThrusts: 'strategic-thrusts',
      reviewMPJ: 'review-mpj',
      myPdJourney: 'my-pd-journey',
      adhocNominations: 'adhoc-nominations',
      report: 'report',
    },
  },
  mobileUrl: {
    pdPlanner: 'mobile-mpj-module',
    plannedActivities: 'planned-activities',
    learningNeed: 'learning-need',
    learningNeedAnalysis: 'learning-need-analysis',
  },
  ItemPerPage: 50,
  /**
   * The number of items showing on a dialog should be shorten since there is less space on the dialog.
   */
  ItemPerPageOnDialog: 10,
  theme: {
    mainBackgroundColor: '#253e5a',
  },
  firebase: environment.firebase,
  sessionVariable: {
    sessionTimeout: 'is-session-timeout',
    accessToken: 'access_token',
  },
  externalNavigationPath: {
    changePassword: 'Manage/ChangePassword',
    profile: 'opal-account/profile',
  },
  httpRequestAvoidIntercepterCatchError: 'avoid-intercepter-catch',
  surveyDateFormat: 'dd/mm/yy',
  backendDateFormat: 'DD/MM/YYYY',
  backendDateTimeFormat: 'DD/MM/YYYY - HH:mm:ss',
  mobile: {
    comment: {
      dateTimeFormat: 'H:mm DD/MM/YYYY',
    },
  },
  defaultAvatar: 'assets/images/default-avatar.png',
  noContentImage: 'assets/images/default-image.png',
  moduleLink: {
    report: environment.moduleLink.report,
    calendar: environment.moduleLink.calendar,
  },
};

export class APIConstant {
  static readonly GATE_WAY: string = `${environment.apiGatewayOrigin}`;

  static readonly BASE_URL_COMPETENCE: string = `${APIConstant.GATE_WAY}/${environment.apiGatewayResource.competence}`;
  // For local environment
  // static readonly BASE_URL_COMPETENCE: string = `https://localhost:44386`;
  static readonly BASE_URL_ORGANIZATION: string = `${APIConstant.GATE_WAY}/${environment.apiGatewayResource.organization}`;
  static readonly BASE_URL_API_TAGGING: string = `${APIConstant.GATE_WAY}/${environment.apiGatewayResource.apitagging}/api`;
  static readonly BASE_URL_LEARNING_CATALOG: string = `${APIConstant.GATE_WAY}/${environment.apiGatewayResource.learningcatalog}/api`;
  static readonly BASE_URL_COURSE_API: string = `${APIConstant.GATE_WAY}/${environment.apiGatewayResource.courseapi}/api`;
  static readonly BASE_URL_COMPETENCE_V2: string = `${APIConstant.BASE_URL_COMPETENCE}/v2`;
  static readonly BASE_URL_LEARNER_API: string = `${APIConstant.GATE_WAY}/${environment.apiGatewayResource.learner}/api`;
  static readonly BASE_URL_LNA_SURVEY_API: string = `${APIConstant.GATE_WAY}/${environment.apiGatewayResource.lnaSurvey}/api`;

  // ENDPOINTS FOR IDP NEED
  static readonly IDP_NEED_BASE_URL: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/needs`;
  static readonly IDP_NEED_RESULTS: string = `${APIConstant.IDP_NEED_BASE_URL}/results`;
  static readonly IDP_NEED_CONFIG: string = `${APIConstant.IDP_NEED_BASE_URL}/config`;
  static readonly IDP_NEED_REPORTS: string = `${APIConstant.IDP_NEED_BASE_URL}/reports`;
  static readonly IDP_NEED_CHANGE_STATUS: string = `${APIConstant.IDP_NEED_BASE_URL}/results/changeStatus`;
  static readonly IDP_NEED_CARRER_ASPIRATION: string = `${APIConstant.IDP_NEED_BASE_URL}/careeraspiration/series`;
  static readonly IDP_NEED_REMINDER: string = `${APIConstant.IDP_NEED_BASE_URL}/reminder`;

  // ENDPOINTS FOR IDP ACTION ITEMS
  static readonly IDP_ACTION_ITEM_BASE_URL: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/actionitems`;
  static readonly IDP_ACTION_ITEM_RESULTS: string = `${APIConstant.IDP_ACTION_ITEM_BASE_URL}/results`;
  static readonly IDP_ACTION_ITEM_CONFIG: string = `${APIConstant.IDP_ACTION_ITEM_BASE_URL}/config`;
  static readonly IDP_ACTION_ITEM_ASSIGN_CONTENT: string = `${APIConstant.IDP_ACTION_ITEM_BASE_URL}/assign_contents`;
  static readonly IDP_ACTION_ITEM_DEACTIVATE: string = `${APIConstant.IDP_ACTION_ITEM_RESULTS}/deactivate`;
  static readonly IDP_ACTION_ITEM_CHANGE_STATUS: string = `${APIConstant.IDP_ACTION_ITEM_RESULTS}/changestatus`;

  // ENDPOINTS FOR IDP PLAN
  static readonly IDP_PLAN_BASE_URL: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/plans`;
  static readonly IDP_PLAN_RESULTS: string = `${APIConstant.IDP_PLAN_BASE_URL}/results`;
  static readonly IDP_PLAN_PDOS: string = `${APIConstant.IDP_PLAN_BASE_URL}/pdos`;
  static readonly IDP_PLAN_PDOS_PENDIND: string = `${APIConstant.IDP_PLAN_PDOS}/external/self-registered/pending`;
  static readonly IDP_PLAN_CHANGE_STATUS: string = `${APIConstant.IDP_PLAN_BASE_URL}/results/changeStatus`;

  // ENDPOINTS FOR PD CATALOGUE TO GET PD OPPORTUNITIES
  static readonly PD_CATALOGUE_SEARCH: string = `${APIConstant.BASE_URL_LEARNING_CATALOG}/PDCatalogue/search`;
  static readonly PD_CATALOGUE_GET_BY_IDS: string = `${APIConstant.BASE_URL_COURSE_API}/courses/getByIds`;
  static readonly PD_CLASSRUN_GET_BY_COURSE_ID: string = `${APIConstant.BASE_URL_COURSE_API}/classrun/byCourseId`;
  static readonly PD_CLASSRUN_GET_BY_ID: string = `${APIConstant.BASE_URL_COURSE_API}/classrun`;
  static readonly GET_SESSION_BY_CLASSRUNID: string = `${APIConstant.BASE_URL_COURSE_API}/session/byClassRunId`;
  static readonly GET_SESSION_BY_CLASSRUNIDS: string = `${APIConstant.BASE_URL_COURSE_API}/session/byClassRunIds`;
  static readonly GET_TABLEOFCONTENT: string = `${APIConstant.BASE_URL_COURSE_API}/learningcontent`;
  static readonly PD_NO_REGISTRATION_FINISHED: string = `${APIConstant.BASE_URL_COURSE_API}/registration/getNoOfRegistrationFinished`;

  static readonly PD_CATALOGUE: string = `${APIConstant.BASE_URL_COMPETENCE}/learningopportunities`;

  // CLASS REGISTRATIONS
  static readonly CLASS_REGISTRATIONS: string = `${APIConstant.PD_CATALOGUE}/registrations`;
  static readonly CLASS_REGISTRATIONS_CHANGE_STATUS: string = `${APIConstant.PD_CATALOGUE}/registration/changeStatus`;
  static readonly CLASS_WITHDRAWS_CHANGE_STATUS: string = `${APIConstant.PD_CATALOGUE}/withdraw/changeStatus`;
  static readonly CLASS_CHANGE_REQUEST_CHANGE_STATUS: string = `${APIConstant.PD_CATALOGUE}/changeClassRun/changeStatus`;

  static readonly GET_CLASSRUN_BY_ID: string = `${APIConstant.PD_CATALOGUE}/classrun`;
  static readonly GET_CLASSRUN_BY_COURSEID: string = `${APIConstant.PD_CATALOGUE}/classruns/byCourseId`;

  // ENDPOINTS FOR PD CATALOGUE TO GET BOOKMARK
  static readonly GET_PD_BOOKMARK: string = `${APIConstant.BASE_URL_COMPETENCE_V2}/bookmarks`;
  static readonly DELETE_PD_BOOKMARK: string = `${APIConstant.BASE_URL_COMPETENCE_V2}/bookmark`;
  static readonly POST_PD_BOOKMARK: string = `${APIConstant.BASE_URL_COMPETENCE_V2}/bookmark`;
  static readonly GET_USER_GROUP: string = `${APIConstant.BASE_URL_ORGANIZATION}/userpools`;

  // ENDPOINTS FOR NOMINATION
  static readonly NOMINATE_RESOURCE: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/nominations`;
  static readonly POST_NOMINATE: string = `${APIConstant.NOMINATE_RESOURCE}`;
  static readonly GET_NOMINATED_LEARNERS: string = `${APIConstant.NOMINATE_RESOURCE}/learners`;
  static readonly GET_NOMINATED_GROUPS: string = `${APIConstant.NOMINATE_RESOURCE}/groups`;
  static readonly GET_NOMINATED_DEPARTMENT: string = `${APIConstant.NOMINATE_RESOURCE}/department`;
  static readonly GET_MASS_NOMINATED_RESULTS: string = `${APIConstant.NOMINATE_RESOURCE}/massnominations`;

  // KLP MASS NOMINATION
  static readonly CHECK_VALID_MASS_NOMINATION: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/massnominations/data/validate`;
  static readonly CHECK_VALID_MASS_NOMINATION_FILE: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/massnominations/file/validate`;
  static readonly POST_MASS_NOMINATE: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/nominations/massnominations`;
  static readonly DOWNLOAD_MASS_NOMINATE_REPORT_FILE: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/massnominations/report/download/async`;

  // ENDPOINTS FOR ADHOC NOMINATION
  static readonly ADHOC_NOMINATE_RESOURCE: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/adhocnominations`;
  static readonly POST_ADHOC_NOMINATE: string = `${APIConstant.ADHOC_NOMINATE_RESOURCE}`;
  static readonly POST_ADHOC_MASS_NOMINATE: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/adhocnominations/massnominations`;
  static readonly GET_ADHOC_NOMINATED_LEARNERS: string = `${APIConstant.ADHOC_NOMINATE_RESOURCE}/learners`;
  static readonly GET_ADHOC_NOMINATED_GROUPS: string = `${APIConstant.ADHOC_NOMINATE_RESOURCE}/groups`;
  static readonly GET_ADHOC_NOMINATED_DEPARTMENT: string = `${APIConstant.ADHOC_NOMINATE_RESOURCE}/department`;
  static readonly GET_ADHOC_MASS_NOMINATED_RESULTS: string = `${APIConstant.ADHOC_NOMINATE_RESOURCE}/massnominations`;
  static readonly GET_ADHOC_MASS_NOMINATED_LEARNER: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/adhocmassnominations/learners`;

  // ENDPOINTS FOR RECOMMENDATION
  static readonly RECOMMEND_RESOURCE: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/recommendations`;
  static readonly POST_RECOMMEND_PDOS: string = `${APIConstant.RECOMMEND_RESOURCE}`;
  static readonly GET_RECOMMENDED_LEARNERS: string = `${APIConstant.RECOMMEND_RESOURCE}/learners`;
  static readonly GET_RECOMMENDED_GROUPS: string = `${APIConstant.RECOMMEND_RESOURCE}/groups`;
  static readonly GET_RECOMMENDED_DEPARTMENT: string = `${APIConstant.RECOMMEND_RESOURCE}/department`;

  // METADATA TAG
  static readonly GET_PDO_METADATA_TAG: string = `${APIConstant.BASE_URL_API_TAGGING}/metadataTag`;

  // LEARNER
  static readonly GET_COURSE_INFO_FOR_LEARNER: string = `${APIConstant.BASE_URL_LEARNER_API}/me/courses/details/byCourseId`;
  static readonly GET_COURSE_REVIEWS_FOR_LEARNER: string = `${APIConstant.BASE_URL_LEARNER_API}/reviews`;
  static readonly GET_LNA_SURVEY: string = `${APIConstant.BASE_URL_LNA_SURVEY_API}/forms/newest-assigned-survey-link`;
}

export class ODPAPIConstant {
  static readonly GATE_WAY: string = `${environment.apiGatewayOrigin}`;
  static readonly BASE_URL_COMPETENCE: string = `${ODPAPIConstant.GATE_WAY}/${environment.apiGatewayResource.competence}`;
  static readonly BASE_URL_ORGANIZATION: string = `${ODPAPIConstant.GATE_WAY}/${environment.apiGatewayResource.organization}`;
  static readonly BASE_URL_COMPETENCE_V2: string = `${ODPAPIConstant.BASE_URL_COMPETENCE}/v2`;

  static readonly ODP_BASE_URL: string = `${ODPAPIConstant.BASE_URL_COMPETENCE}/odp`;
  static readonly KEY_LEARNING_PROGRAM_BASE_URL: string = `${ODPAPIConstant.ODP_BASE_URL}/programmes/results`;
}

export class Constant {
  // Constant
  static readonly MOE_SYSTEM_URI: string = 'urn:opal2.moe.sg';
  static readonly STRING_EMPTY: string = '';
  static readonly LONG_TEXT_MAX_LENGTH: number = 1000;
  static readonly SHORT_TEXT_MAX_LENGTH: number = 100;
  static readonly REQUEST_TIME_OUT: number = 60000;
  static readonly MIDDLE_MONTH_OF_YEAR_VALUE: number = 5;
  static readonly DELAY_RENDER_DOM: number = 300; // ms
  static readonly MINUTE_PER_HOUR: number = 60;
  static readonly INVALID_DATE_STRING: string = 'Invalid date';
  static readonly EXTERNAL_PDO_THUMBNAIL_PATH: string =
    'assets/images/mpj/custom-pdo-thumbnail.jpg';
  static readonly E_LEARNING_COURSE_TAG_ID: string =
    '5df4dfda-db9f-11e9-b8d9-0242ac120004';

  // Date format
  static readonly SURVEY_DATE_FORMAT: string = 'DD/MM/YYYY';
  static readonly COMPARE_DATE_FORMAT: string = 'DD/MM/YYYY';

  // HTTP error code
  static readonly CANNOT_GET_USER_INFO: string = 'CANNOT_GET_USER_INFO';
  static readonly PENDING_USER_ERROR_CODE: string = 'ERROR_USER_NOT_ACTIVE';
  static readonly FORBBIDEN_USER_ERROR_CODE: string =
    'ERROR_ACCESS_DENIED_USER';
  static readonly INVALID_SESSION_ERROR_403: string = 'Key not authorised';
  static readonly KEY_NOT_AUTHORIZE_ERROR_401: string = 'Key not authorized';
  static readonly KEY_NOT_AUTHORISE_ERROR_401: string = 'Key not authorised';
}

export class RouteConstant {
  // Error page router
  static readonly ERROR_COMMON: string = '/error';
  static readonly ERROR_PENDING_USER: string = '/error/pending-user';
  static readonly ERROR_FORBIDDEN_USER: string = '/error/forbidden-user';
  static readonly ERROR_ACCESS_DENIED: string = '/error/access-denied';
}

export class VariableConstant {
  // The return url after login success
  static readonly RETURN_URL: string = 'return_url';
  // 'Path' of route which was mapped to 'ReturnUrl'
  static readonly RETURN_URL_PATH_MAP: string = 'return_url_path';
}

export class OAuthEventConstant {
  static readonly TOKEN_RECIEVED: string = 'token_received';
  static readonly TOKEN_EXPIRES: string = 'token_expires';
  static readonly SESSION_TERMINATED: string = 'session_terminated';
  static readonly CODE_ERROR: string = 'code_error';
  static readonly SESSION_ERROR: string = 'session_error';
  static readonly DOCUMENT_LOADED: string = 'discovery_document_loaded';
  static readonly DOCIMENT_LOAD_FAILED: string =
    'discovery_document_load_error';
  static readonly TOKEN_ERROR: string = 'token_error';
}

export abstract class IdleConfig {
  public static readonly AUTOSAVE_INTERVALTIME: number = 2000;
  public static readonly AUTOSAVE_PD_CONTENT_TASK: string =
    'autosave_pd_content_task';
  public static readonly userEvents: string =
    'DOMMouseScroll keyup mousewheel mousedown touchstart touchmove scroll';
}
