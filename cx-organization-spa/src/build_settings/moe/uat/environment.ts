// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'OrganizationSpa',
  issuer: 'https://idm.uat.opal2.conexus.net',
  apiGatewayOrigin: 'https://apigw.uat.opal2.conexus.net',
  apiGatewayResource: {
    organization: 'uat-competence-opal-api-organization',
    assessment: 'uat-competence-opal-api-assessment',
    cxId: 'uat-cxid-opal-idp',
    portal: 'uat-competence-opal-api-portal',
    report: 'uat-competence-opal-api-report',
    event: 'uat-competence-opal-api-event',
    communication: 'uat-datahub-opal-api-communication',
    dataHub: 'uat-datahub-opal-api-query',
    learningCatalog: 'uat-competence-opal-api-learningcatalog'
  },
  site: {
    title: 'System Admin',
    logo: {
      imageUrl: 'assets/images/uat-opal-logo-slogan.png',
      imageAlt: 'System Admin',
      routeLink: '',
      text: 'System Admin'
    },
    footer: {
      vulnerabilityUrl: 'https://tech.gov.sg/report_vulnerability',
      privacyStatementUrl:
        'https://idm.uat.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.uat.opal2.conexus.net/Home/TermsOfUse'
    },
    dateRelease: '12 June 2020'
  },
  baseProfileAppUrl: 'https://www.uat.opal2.conexus.net',
  auditLogAppUrl: 'https://admin.uat.opal2.conexus.net',
  autoNavigateToIDP: true,
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  buildVersion: '1.0.0',
  UserIdleTimeOut: 1500, // seconds
  SessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  OtherDepartmentId: 25031,
  topDepartmentId: 1,
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyAQYaxjtyRu8_BEnG83lyKNbAJo9eNVRRw',
      authDomain: 'moe-opal-uat.firebaseapp.com',
      databaseURL: 'https://moe-opal-uat.firebaseio.com',
      projectId: 'moe-opal-uat',
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
      'https://www.uat.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.uat.opal2.conexus.net/opal-account/notifications/alert'
  },
  permissionConfig: {
    departmentHierarchiesFilter: {
      hideForDepartmentTypeExtIds: ['school']
    }
  },
  moduleLink: {
    PDPM: 'https://www.uat.opal2.conexus.net/pdplanner/',
    SAM: 'https://admin.uat.opal2.conexus.net/',
    LearnerAppAndroid: 'https://www.uat.opal2.conexus.net/pdplanner/',
    LearnerAppIOS: 'https://www.uat.opal2.conexus.net/pdplanner/',
    LearnerWeb: 'https://www.uat.opal2.conexus.net/app/learner',
    Report: 'https://www.uat.opal2.conexus.net/report'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
