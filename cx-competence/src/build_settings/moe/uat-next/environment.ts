// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'CompetenceSpa',
  issuer: 'https://idm.uat-next.opal2.conexus.net',
  apiGatewayOrigin: 'https://api.uat-next.opal2.conexus.net',
  systemAdminUrl: 'https://admin.uat-next.opal2.conexus.net',
  apiGatewayResource: {
    organization: 'uat-next-competence-opal-api-organization',
    assessment: 'uat-next-competence-opal-api-assessment',
    cxId: 'uat-next-cxid-opal-idp',
    portal: 'uat-next-competence-opal-api-portal',
    report: 'uat-next-competence-opal-api-report',
    competence: 'uat-next-competence-opal-api',
    communication: 'uat-next-datahub-opal-api-communication',
    learningcatalog: 'uat-next-competence-opal-api-learningcatalog',
    courseapi: 'uat-next-learnapp-opal-api-course',
    coursepad: 'uat-next-course-opal-api',
    apitagging: 'uat-next-learnapp-opal-api-tagging',
    learner: 'uat-next-learnapp-opal-api-learner',
    lnaSurvey: 'uat-next-learnapp-opal-api-lnaform',
  },
  ePortfolioUrl: 'https://www.uat-next.opal2.conexus.net/eportfolio',
  site: {
    title: 'PD Planner',
    logo: {
      imageUrl: 'assets/images/uat-opal-logo-slogan.png',
      imageAlt: 'PD Planner',
      routeLink: '',
      text: 'PD Planner',
    },
    footer: {
      vulnerabilityUrl: 'https://tech.gov.sg/report_vulnerability',
      privacyStatementUrl:
        'https://idm.uat-next.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.uat-next.opal2.conexus.net/Home/TermsOfUse',
    },
    dateRelease: '12 June 2020',
  },
  baseProfileAppUrl: 'https://www.uat-next.opal2.conexus.net',
  ItemPerPage: 50,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  buildVersion: '1.0.0',
  autoNavigateToIDP: true,
  pdplanConfig: {
    permissionConfig: {
      createLearningDirection: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      createKeyLearningProgram: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      startNewLearningPlan: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      deleteDraftVersion: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      edit: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      submit: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      approveOrReject: {
        roleExtIds: [
          'approvingofficer',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
        ],
      },
      commentOnOPJ: [
        {
          opjStatus: ['NotStarted', 'Started'],
          roleExtIds: ['*'],
        },
        {
          opjStatus: [
            'PendingForApproval',
            'Approved',
            'Rejected',
            'Completed',
          ],
          roleExtIds: [
            'schooltrainingcoordinator',
            'divisiontrainingcoordinator',
            'overallsystemadministrator',
            'schooladmin',
            'divisionadmin',
            'approvingofficer',
          ],
        },
      ],
      departmentHierarchyBrowser: {
        includeChildren: {
          roleExtIds: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'divisiontrainingcoordinator',
            'schooltrainingcoordinator',
            'approvingofficer',
          ],
        },
      },
      approvePendingRequest: {
        ODP: {
          learningPlan: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'approvingofficer',
            'divisionallearningcoordinator',
            'schooltrainingcoordinator',
          ],
          learningDirection: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'approvingofficer',
            'divisionallearningcoordinator',
            'schooltrainingcoordinator',
          ],
          klpNomination: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
        },
        IDP: {
          classRegistration: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
          classWithdrawal: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
          classChangeRequest: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
          adhocNominations: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
        },
      },
    },
    organizationalUnitTypesHavingOPJ: ['school', 'division', 'ministry'],
  },
  lnaResult: {
    roleCanDecideApprovalExtIds: [
      'overallsystemadministrator',
      'schooladmin',
      'branchadmin',
      'divisionadmin',
    ],
    allowToEditLNAApprovedXTimes: 1,
  },
  userIdleTimeOut: 1500, // seconds
  sessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  uriLearningOpportunityFieldName: 'uriLearningOpportunity',
  myPdJourneyConfig: {
    pdPlanTabName: 'PdPlanTab',
  },
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyDWLtQfk-qzpYQ4YegYPT32RagPAw9g4qo',
      authDomain: 'moe-opal-uat-next.firebaseapp.com',
      databaseURL: 'https://moe-opal-uat-next.firebaseio.com',
      projectId: 'moe-opal-uat-next',
      storageBucket: '',
      messagingSenderId: '213820359845',
      appId: '1:213820359845:web:bf78a643e041e642',
    },
    publicVapidKey:
      'BPzPibzIgnJKutFyLVMzLUxTelp3EHCTaOT26U3sX2igoUxEMtGs-UyyaM5vnwd_00fCPuNw5LdKRBO8ab-ZaDs',
  },
  VirtualPath: 'pdplanner',
  notification: {
    enableToggleToFireBase: false,
    enableShowBellIcon: true,
    enableBroadCast: true,
    bellUrl:
      'https://www.uat-next.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.uat-next.opal2.conexus.net/opal-account/notifications/alert',
  },
  moduleLink: {
    report: 'https://www.uat-next.opal2.conexus.net/report',
    learner: 'https://www.uat-next.opal2.conexus.net/app/learner',
    calendar: 'https://www.uat-next.opal2.conexus.net/app/calendar',
  },
};
