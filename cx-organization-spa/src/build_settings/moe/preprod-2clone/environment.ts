// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  showAuthDebugInformation: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'OrganizationSpa',
  issuer: 'https://idm2clone.pre-prod.opal2.conexus.net',
  apiGatewayOrigin: 'https://api2clone.pre-prod.opal2.conexus.net',
  apiGatewayResource: {
    organization: 'preprod-2clone-competence-opal-api-organization',
    assessment: 'preprod-2clone-competence-opal-api-assessment',
    cxId: 'preprod-2clone-cxid-opal-idp',
    portal: 'preprod-2clone-competence-opal-api-portal',
    report: 'preprod-2clone-competence-opal-api-report',
    event: 'preprod-2clone-competence-opal-api-event',
    communication: 'preprod-2clone-datahub-opal-api-communication',
    dataHub: 'preprod-2clone-datahub-opal-api-query',
    learningCatalog: 'preprod-2clone-competence-opal-api-learningcatalog',
    tagging: 'preprod-2clone-learnapp-opal-api-tagging/api'
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
        'https://idm2clone.pre-prod.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl:
        'https://idm2clone.pre-prod.opal2.conexus.net/Home/TermsOfUse'
    },
    dateRelease: '12 June 2020'
  },
  baseProfileAppUrl: 'https://www2clone.pre-prod.opal2.conexus.net',
  baseProfileUrl: 'https://admin2clone.pre-prod.opal2.conexus.net',
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
  showTechnicalInfo: true,
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
      'https://www2clone.pre-prod.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www2clone.pre-prod.opal2.conexus.net/opal-account/notifications/alert'
  },
  permissionConfig: {
    departmentHierarchiesFilter: {
      hideForDepartmentTypeExtIds: ['school']
    }
  },
  moduleLink: {
    PDPM: 'https://www2clone.pre-prod.opal2.conexus.net/pdplanner/',
    SAM: 'https://admin2clone.pre-prod.opal2.conexus.net/',
    LearnerAppAndroid:
      'https://www2clone.pre-prod.opal2.conexus.net/pdplanner/',
    LearnerAppIOS: 'https://www2clone.pre-prod.opal2.conexus.net/pdplanner/',
    LearnerWeb: 'https://www2clone.pre-prod.opal2.conexus.net/app/learner',
    Report: 'https://www2clone.pre-prod.opal2.conexus.net/report',
    CSL: 'https://www2clone.pre-prod.opal2.conexus.net/csl'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
