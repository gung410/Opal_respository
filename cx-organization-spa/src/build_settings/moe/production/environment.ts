// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'OrganizationSpa',
  issuer: 'https://idm.opal2.moe.edu.sg',
  apiGatewayOrigin: 'https://api.opal2.moe.edu.sg',
  apiGatewayResource: {
    organization: 'prod-competence-opal-api-organization',
    assessment: 'prod-competence-opal-api-assessment',
    cxId: 'prod-cxid-opal-idp',
    portal: 'prod-competence-opal-api-portal',
    report: 'prod-competence-opal-api-report',
    event: 'prod-competence-opal-api-event',
    communication: 'prod-datahub-opal-api-communication',
    dataHub: 'prod-datahub-opal-api-query',
    learningCatalog: 'prod-competence-opal-api-learningcatalog',
    tagging: 'prod-learnapp-opal-api-tagging'
  },
  site: {
    title: 'System Admin',
    logo: {
      imageUrl: 'assets/images/opal-logo-slogan.png',
      imageAlt: 'System Admin',
      routeLink: '',
      text: 'System Admin'
    },
    footer: {
      vulnerabilityUrl: 'https://tech.gov.sg/report_vulnerability',
      privacyStatementUrl: 'https://idm.opal2.moe.edu.sg/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.opal2.moe.edu.sg/Home/TermsOfUse'
    },
    dateRelease: '12 June 2020'
  },
  baseProfileAppUrl: 'https://www.opal2.moe.edu.sg',
  baseProfileUrl: 'https://admin.opal2.moe.edu.sg',
  autoNavigateToIDP: true,
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  buildVersion: '1.0.0',
  UserIdleTimeOut: 1500, // seconds
  SessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  OtherDepartmentId: 25030,
  topDepartmentId: 1,
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyAQYaxjtyRu8_BEnG83lyKNbAJo9eNVRRw',
      authDomain: 'moe-opal-prod.firebaseapp.com',
      databaseURL: 'https://moe-opal-prod.firebaseio.com',
      projectId: 'moe-opal-prod',
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
    bellUrl: 'https://www.opal2.moe.edu.sg/opal-account/notifications/bell',
    alertUrl: 'https://www.opal2.moe.edu.sg/opal-account/notifications/alert'
  },
  permissionConfig: {
    departmentHierarchiesFilter: {
      hideForDepartmentTypeExtIds: ['school']
    }
  },
  moduleLink: {
    PDPM: 'https://www.opal2.moe.edu.sg/pdplanner/',
    SAM: 'https://admin.opal2.moe.edu.sg/',
    LearnerAppAndroid: 'https://www.opal2.moe.edu.sg/pdplanner/',
    LearnerAppIOS: 'https://www.opal2.moe.edu.sg/pdplanner/',
    LearnerWeb: 'https://www.opal2.moe.edu.sg/app/learner',
    Report: 'https://www.opal2.moe.edu.sg/report'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
