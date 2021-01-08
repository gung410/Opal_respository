// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'OrganizationSpa',
  issuer: 'https://idm.pre-prod.opal2.conexus.net',
  apiGatewayOrigin: 'https://api.pre-prod.opal2.conexus.net',
  apiGatewayResource: {
    organization: 'pre-prod-competence-opal-api-organization',
    assessment: 'pre-prod-competence-opal-api-assessment',
    cxId: 'pre-prod-cxid-opal-idp',
    portal: 'pre-prod-competence-opal-api-portal',
    report: 'pre-prod-competence-opal-api-report',
    event: 'pre-prod-competence-opal-api-event',
    communication: 'pre-prod-datahub-opal-api-communication',
    dataHub: 'pre-prod-datahub-opal-api-query',
    learningCatalog: 'pre-prod-competence-opal-api-learningcatalog'
  },
  site: {
    title: 'System Admin',
    logo: {
      imageUrl: 'assets/images/pre-prod-opal-logo-slogan.png',
      imageAlt: 'System Admin',
      routeLink: '',
      text: 'System Admin'
    },
    footer: {
      vulnerabilityUrl: 'https://tech.gov.sg/report_vulnerability',
      privacyStatementUrl:
        'https://idm.pre-prod.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.pre-prod.opal2.conexus.net/Home/TermsOfUse'
    },
    dateRelease: '12 June 2020'
  },
  baseProfileAppUrl: 'https://www.pre-prod.opal2.conexus.net',
  auditLogAppUrl: 'https://admin.pre-prod.opal2.conexus.net',
  autoNavigateToIDP: true,
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  buildVersion: '1.0.0',
  UserIdleTimeOut: 1500, // seconds
  SessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  OtherDepartmentId: 20747,
  topDepartmentId: 1,
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyAQYaxjtyRu8_BEnG83lyKNbAJo9eNVRRw',
      authDomain: 'moe-opal-pre-prod.firebaseapp.com',
      databaseURL: 'https://moe-opal-pre-prod.firebaseio.com',
      projectId: 'moe-opal-pre-prod',
      storageBucket: '',
      messagingSenderId: '718367215580',
      appId: '1:718367215580:web:0536314cc4263190'
    },
    publicVapidKey:
      'BDSteR2mKaYolfsqltZz662WMFTj4IODM_BngAWrYHbMYXQXxBWxJHYG_OErSf9wffPIbTOI7JrT5OnlszOPfr8'
  },
  notification: {
    enableToggleToFireBase: false,
    enableShowBellIcon: true,
    enableBroadCast: true,
    bellUrl:
      'https://www.pre-prod.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.pre-prod.opal2.conexus.net/opal-account/notifications/alert'
  },
  permissionConfig: {
    departmentHierarchiesFilter: {
      hideForDepartmentTypeExtIds: ['school']
    }
  },
  moduleLink: {
    PDPM: 'https://www.pre-prod.opal2.conexus.net/pdplanner/',
    SAM: 'https://admin.pre-prod.opal2.conexus.net/',
    LearnerAppAndroid: 'https://www.pre-prod.opal2.conexus.net/pdplanner/',
    LearnerAppIOS: 'https://www.pre-prod.opal2.conexus.net/pdplanner/',
    LearnerWeb: 'https://www.pre-prod.opal2.conexus.net/app/learner',
    Report: 'https://www.pre-prod.opal2.conexus.net/report'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
