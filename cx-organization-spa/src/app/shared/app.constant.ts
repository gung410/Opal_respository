import { environment } from 'app-environments/environment';

// tslint:disable: max-classes-per-file
// tslint:disable-next-line:variable-name
export const AppConstant = {
  fallbackLanguage: environment.fallbackLanguage,
  fallbackLanguageName: environment.fallbackLanguageName,
  api: {
    organization: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.organization}`,
    assessment: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.assessment}`,
    portal: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.portal}`,
    communication: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.communication}`,
    learningCatalog: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.learningCatalog}`,
    tagging: `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.tagging}`
  },
  cxId: {
    ProfileUrl: `${environment.issuer}/manage/index`
  },
  ownerId: environment.OwnerId,
  customerId: environment.CustomerId,
  topDepartmentId: environment.topDepartmentId,
  clientId: 'OrganizationSpa',
  communicationClientId: 'organization_api',
  menuType: {
    MENU_ITEM: 'Default',
    APP_SWITCHER: 'AppSwitcher'
  },
  siteURL: {
    login: 'login',
    error: 'error',
    management: 'management',
    monitor: 'monitor',
    menus: {
      userAccounts: 'user-accounts',
      organization: 'organization',
      reports: 'reports',
      userGroups: 'user-groups',
      broadcastMessages: 'broadcast-messages',
      systemAuditLog: 'audit',
      permissions: 'permissions',
      taxonomy: 'taxonomy-management',
      batchJobsMonitoring: 'batch-jobs-monitoring',
      socialEngagement: 'social-engagement'
    },
    sessionTimeout: 'session-timeout'
  },
  Item25PerPage: 25,
  ItemPerPage: 50,
  theme: {
    mainBackgroundColor: '#253e5a'
  },
  sessionStorageVariable: {
    timeout: 'is-session-timeout'
  },
  externalNavigationPath: {
    changePassword: 'Manage/ChangePassword',
    profile: 'opal-account/profile'
  },
  firebase: environment.firebase,
  httpRequestAvoidIntercepterCatchError: 'avoid-intercepter-catch',
  httpRequestAvoidHideLoaderWhenError: 'avoid-hideloader-when-error',
  surveyDateFormat: 'dd/mm/yy',
  backendDateFormat: 'DD/MM/YYYY',
  DayMonthFormat: 'DD/MM',
  dateTimeFormat: 'DD/MM/YYYY hh:mm A',
  fullDateTimeFormat: 'DD/MM/YYYY hh:mm:ss',
  defaultAvatar: 'assets/images/default-avatar.png',
  moduleLink: {
    PDPM: environment.moduleLink.PDPM,
    SAM: environment.moduleLink.SAM,
    LearnerAppAndroid: environment.moduleLink.LearnerAppAndroid,
    LearnerAppIOS: environment.moduleLink.LearnerAppIOS,
    Report: environment.moduleLink.Report,
    LearnerWeb: environment.moduleLink.LearnerWeb,
    CSL: environment.moduleLink.CSL
  }
};

export class Constant {
  // HTTP error code
  static readonly PENDING_USER_ERROR_CODE: string = 'ERROR_USER_NOT_ACTIVE';
  static readonly FORBBIDEN_USER_ERROR_CODE: string =
    'ERROR_ACCESS_DENIED_USER';
  static readonly INVALID_SESSION_ERROR_403: string = 'Key not authorised';
  static readonly KEY_NOT_AUTHORIZE_ERROR_401: string = 'Key not authorized';
  static readonly KEY_NOT_AUTHORISE_ERROR_401: string = 'Key not authorised';
  static readonly VALIDATION_DEPARTMENT_EXTID_EXISTS: string =
    'VALIDATION_DEPARTMENT_EXTID_EXISTS_CUSTOMER';
}

export class RouteConstant {
  // Error page router
  static readonly ERROR_COMMON: string = '/error';
  static readonly ERROR_PENDING_USER: string = '/error/pending-user';
  static readonly ERROR_FORBIDDEN_USER: string = '/error/forbidden-user';
  // The return url after login success
  static readonly RETURN_URL: string = 'return_url';
  // 'Path' of route which was mapped to 'ReturnUrl'
  static readonly RETURN_URL_PATH: string = 'return_url_path';
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
