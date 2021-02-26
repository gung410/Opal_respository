// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'OrganizationSpa',
  issuer: 'https://idm.uat-next.opal2.conexus.net',
  apiGatewayOrigin: 'https://api.uat-next.opal2.conexus.net',
  apiGatewayResource: {
    organization: 'uat-next-competence-opal-api-organization',
    assessment: 'uat-next-competence-opal-api-assessment',
    cxId: 'uat-next-cxid-opal-idp',
    portal: 'uat-next-competence-opal-api-portal',
    report: 'uat-next-competence-opal-api-report',
    event: 'uat-next-competence-opal-api-event',
    communication: 'uat-next-datahub-opal-api-communication',
    dataHub: 'uat-next-datahub-opal-api-query',
    learningCatalog: 'uat-next-competence-opal-api-learningcatalog',
    tagging: 'uat-next-learnapp-opal-api-tagging'
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
        'https://idm.uat-next.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.uat-next.opal2.conexus.net/Home/TermsOfUse'
    },
    dateRelease: '12 June 2020'
  },
  baseProfileAppUrl: 'https://www.uat-next.opal2.conexus.net',
  baseProfileUrl: 'https://admin.uat-next.opal2.conexus.net',
  autoNavigateToIDP: true,
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  buildVersion: '1.0.0',
  UserIdleTimeOut: 1500, // seconds
  SessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  OtherDepartmentId: 20748,
  topDepartmentId: 1,
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyDWLtQfk-qzpYQ4YegYPT32RagPAw9g4qo',
      authDomain: 'moe-opal-uat-next.firebaseapp.com',
      databaseURL: 'https://moe-opal-uat-next.firebaseio.com',
      projectId: 'moe-opal-uat-next',
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
      'https://www.uat-next.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.uat-next.opal2.conexus.net/opal-account/notifications/alert'
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
    LearnerWeb: 'https://www.uat-next.opal2.conexus.net/app/learner',
    Report: 'https://www.uat-next.opal2.conexus.net/report'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
