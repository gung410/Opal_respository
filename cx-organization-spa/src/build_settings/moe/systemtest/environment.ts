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
  issuer: 'https://idm.systemtest.opal2.conexus.net',
  apiGatewayOrigin: 'https://apigw.systemtest.opal2.conexus.net',
  apiGatewayResource: {
    organization: 'systemtest-competence-opal-api-organization',
    assessment: 'systemtest-competence-opal-api-assessment',
    cxId: 'systemtest-cxid-opal-idp',
    portal: 'systemtest-competence-opal-api-portal',
    report: 'systemtest-competence-opal-api-report',
    event: 'systemtest-competence-opal-api-event',
    communication: 'systemtest-datahub-opal-api-communication',
    dataHub: 'systemtest-datahub-opal-api-query',
    learningCatalog: 'systemtest-competence-opal-api-learningcatalog',
    tagging: 'systemtest-learnapp-opal-api-tagging/api'
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
      privacyStatementUrl:
        'https://idm.systemtest.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.systemtest.opal2.conexus.net/Home/TermsOfUse'
    },
    dateRelease: '12 June 2020'
  },
  baseProfileAppUrl: 'https://www.systemtest.opal2.conexus.net',
  baseProfileUrl: 'https://admin.systemtest.opal2.conexus.net',
  autoNavigateToIDP: true,
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  buildVersion: '1.0.0',
  UserIdleTimeOut: 1500, // seconds
  SessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  OtherDepartmentId: 37768,
  topDepartmentId: 1,
  showTechnicalInfo: true,
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyDWLtQfk-qzpYQ4YegYPT32RagPAw9g4qo',
      authDomain: 'moe-opal-systemtest.firebaseapp.com',
      databaseURL: 'https://moe-opal-systemtest.firebaseio.com',
      projectId: 'moe-opal-systemtest',
      storageBucket: '',
      messagingSenderId: '213820359845',
      appId: '1:213820359845:web:bf78a643e041e642'
    },
    publicVapidKey:
      'BPzPibzIgnJKutFyLVMzLUxTelp3EHCTaOT26U3sX2igoUxEMtGs-UyyaM5vnwd_00fCPuNw5LdKRBO8ab-ZaDs'
  },
  notification: {
    enableToggleToFireBase: false,
    enableShowBellIcon: true,
    enableBroadCast: true,
    bellUrl:
      'https://www.systemtest.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.systemtest.opal2.conexus.net/opal-account/notifications/alert'
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
    LearnerWeb: 'https://www.systemtest.opal2.conexus.net/app/learner',
    Report: 'https://www.systemtest.opal2.conexus.net/report',
    CSL: 'https://www.systemtest.opal2.conexus.net/csl'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
