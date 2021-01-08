// tslint:disable:all
export const AppConstant = {
  api: {
    organization: `https://apigw.csc.cxs.cloud/development-competence-opal-api-organization`,
    assessment: `https://apigw.csc.cxs.cloud/development-competence-opal-api-assessment`,
    portal: `https://apigw.csc.cxs.cloud/development-competence-opal-api-portal`,
    competence: `https://apigw.csc.cxs.cloud/development-competence-opal-api`,
    communication: `https://apigw.csc.cxs.cloud/development-datahub-opal-api-communication`,
    learningcatalog: `https://apigw.csc.cxs.cloud/development-competence-opal-api-learningcatalog`
  },
  clientId: 'CompetenceSpa',
  customerId: 2052,
  cxId: {
    ProfileUrl: `https://www.systemtest.opal2.conexus.net/opal-account/opal-account/profile`
  },
  menuType: {
    MENU_ITEM: 'Default',
    APP_SWITCHER: 'AppSwitcher'
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
      mpj: 'mpj',
      odp: 'odp',
      reviewMPJ: 'review-mpj',
      myPdJourney: 'my-pd-journey'
    }
  },
  mobileUrl: {
    pdPlanner: 'pd-planner',
    plannedActivities: 'planned-activities',
    learningNeed: 'learning-need',
    learningNeedAnalysis: 'learning-need-analysis'
  },
  ItemPerPage: 50,
  theme: {
    mainBackgroundColor: '#253e5a'
  },
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyDrCwGH3NYGWShiufFpoXdeNC8j4ilV2pc',
      authDomain: 'moe-opal-dev.firebaseapp.com',
      databaseURL: 'https://moe-opal-dev.firebaseio.com',
      projectId: 'moe-opal-dev',
      storageBucket: '',
      messagingSenderId: '618258569847',
      appId: '1:618258569847:web:630e0839aed07b12'
    },
    publicVapidKey: 'BBc173qKd9yyWWAgw5mkf3GxGrbEIhDv0owojXfXKL_myjUCa0-RKcY49H3H9Rg08CoeHMSQtLA0gdowgyhDFTI'
  },
  sessionVariable: {
    sessionTimeout: 'is-session-timeout',
    redirectToExternalSite: 'redirect-to-external-site',
    workingSite: 'working-site',
    accessToken: 'access_token'
  },
  externalNavigationPath: {
    changePassword: 'Manage/ChangePassword',
    profile: 'opal-account/profile'
  },
  httpRequestAvoidIntercepterCatchError: 'avoid-intercepter-catch',
  surveyDateFormat: 'dd/mm/yy',
  backendDateFormat: 'DD/MM/YYYY',
  mobile: {
    comment: {
      dateTimeFormat: 'H:mm DD/MM/YYYY'
    }
  },
  defaultAvatar: 'https://d2nbu0ncciz7qy.cloudfront.net/avatar/default.png'
};

export class APIConstant {
  static readonly GATE_WAY: string = `https://apigw.csc.cxs.cloud`;
  static readonly BASE_URL_COMPETENCE: string = `${APIConstant.GATE_WAY}/development-competence-opal-api`;

  // ENDPOINTS FOR IDP NEED
  static readonly IDP_NEED_BASE_URL: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/needs`;
  static readonly IDP_NEED_RESULTS: string = `${APIConstant.IDP_NEED_BASE_URL}/results`;
  static readonly IDP_NEED_CONFIG: string = `${APIConstant.IDP_NEED_BASE_URL}/config`;
  static readonly IDP_NEED_REPORTS: string = `${APIConstant.IDP_NEED_BASE_URL}/reports`;

  // ENDPOINTS FOR IDP ACTION ITEMS
  static readonly IDP_ACTION_ITEM_BASE_URL: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/actionitems`;
  static readonly IDP_ACTION_ITEM_RESULTS: string = `${APIConstant.IDP_ACTION_ITEM_BASE_URL}/results`;
  static readonly IDP_ACTION_ITEM_CONFIG: string = `${APIConstant.IDP_ACTION_ITEM_BASE_URL}/config`;
  static readonly IDP_ACTION_ITEM_ASSIGN_CONTENT: string = `${APIConstant.IDP_ACTION_ITEM_BASE_URL}/assign_contents`;
  static readonly IDP_ACTION_ITEM_DEACTIVATE: string = `${APIConstant.IDP_ACTION_ITEM_RESULTS}/deactivate`;

  // ENDPOINTS FOR IDP PLAN
  static readonly IDP_PLAN_BASE_URL: string = `${APIConstant.BASE_URL_COMPETENCE}/idp/plans`;
  static readonly IDP_PLAN_RESULTS: string = `${APIConstant.IDP_PLAN_BASE_URL}/results`;

  // ENDPOINTS FOR PD CATALOGUE TO GET PD OPPORTUNITIES
  static readonly PD_CATALOGUE: string = `${APIConstant.BASE_URL_COMPETENCE}/learningopportunities`;
}

export class Constant {
  // Constant
  static readonly MOE_SYSTEM_URI: string = 'urn:opal2.moe.sg';
  static readonly STRING_EMPTY: string = '';
  static readonly LONG_TEXT_MAX_LENGTH: number = 1000;
  static readonly SHORT_TEXT_MAX_LENGTH: number = 100;
  static readonly REQUEST_TIME_OUT: number = 60000;
  static readonly MIDDLE_MONTH_OF_YEAR_VALUE: number = 5;
  static readonly MINUTE_PER_HOUR: number = 60;
  static readonly INVALID_DATE_STRING: string = 'Invalid date';
  static readonly MAX_ITEMS_PER_REQUEST: number = 100000;

  // Date format
  static readonly SURVEY_DATE_FORMAT: string = 'DD/MM/YYYY';
  static readonly COMPARE_DATE_FORMAT: string = 'DD/MM/YYYY';

  // HTTP error code
  static readonly CANNOT_GET_USER_INFO: string = 'CANNOT_GET_USER_INFO';
  static readonly PENDING_USER_ERROR_CODE: string = 'ERROR_USER_NOT_ACTIVE';
  static readonly FORBBIDEN_USER_ERROR_CODE: string = 'ERROR_ACCESS_DENIED_USER';
  static readonly INVALID_SESSION_ERROR_403: string = 'Key not authorised';
  static readonly KEY_NOT_AUTHORIZE_ERROR_401: string = 'Key not authorized';
  static readonly KEY_NOT_AUTHORISE_ERROR_401: string = 'Key not authorised';
}

export class RouteConstant {
  // Error page router
  static readonly ERROR_COMMON: string = '/error';
  static readonly ERROR_PENDING_USER: string = '/error/pending-user';
  static readonly ERROR_FORBIDDEN_USER: string = '/error/forbidden-user';
}
