// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'CompetenceSpa',
  issuer: 'https://idm.pre-prod.opal2.conexus.net',
  apiGatewayOrigin: 'https://api.pre-prod.opal2.conexus.net',
  systemAdminUrl: 'https://admin.pre-prod.opal2.conexus.net',
  apiGatewayResource: {
    organization: 'pre-prod-competence-opal-api-organization',
    assessment: 'pre-prod-competence-opal-api-assessment',
    cxId: 'pre-prod-cxid-opal-idp',
    portal: 'pre-prod-competence-opal-api-portal',
    report: 'pre-prod-competence-opal-api-report',
    competence: 'pre-prod-competence-opal-api',
    communication: 'pre-prod-datahub-opal-api-communication',
    learningcatalog: 'pre-prod-competence-opal-api-learningcatalog',
    courseapi: 'pre-prod-learnapp-opal-api-course',
    coursepad: 'pre-prod-course-opal-api',
    apitagging: 'pre-prod-learnapp-opal-api-tagging',
    learner: 'pre-prod-learnapp-opal-api-learner',
    lnaSurvey: 'pre-prod-learnapp-opal-api-lnaform',
  },
  ePortfolioUrl: 'https://www.pre-prod.opal2.conexus.net/eportfolio',
  site: {
    title: 'PD Planner',
    logo: {
      imageUrl: 'assets/images/pre-prod-opal-logo-slogan.png',
      imageAlt: 'PD Planner',
      routeLink: '',
      text: 'PD Planner',
    },
    footer: {
      vulnerabilityUrl: 'https://tech.gov.sg/report_vulnerability',
      privacyStatementUrl:
        'https://idm.pre-prod.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.pre-prod.opal2.conexus.net/Home/TermsOfUse',
    },
    dateRelease: '12 June 2020',
  },
  baseProfileAppUrl: 'https://www.pre-prod.opal2.conexus.net',
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
      apiKey: 'AIzaSyAQYaxjtyRu8_BEnG83lyKNbAJo9eNVRRw',
      authDomain: 'moe-opal-pre-prod.firebaseapp.com',
      databaseURL: 'https://moe-opal-pre-prod.firebaseio.com',
      projectId: 'moe-opal-pre-prod',
      storageBucket: '',
      messagingSenderId: '718367215580',
      appId: '1:718367215580:web:0536314cc4263190',
    },
    publicVapidKey:
      'BDSteR2mKaYolfsqltZz662WMFTj4IODM_BngAWrYHbMYXQXxBWxJHYG_OErSf9wffPIbTOI7JrT5OnlszOPfr8',
  },
  VirtualPath: 'pdplanner',
  notification: {
    enableToggleToFireBase: false,
    enableShowBellIcon: true,
    enableBroadCast: true,
    bellUrl:
      'https://www.pre-prod.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.pre-prod.opal2.conexus.net/opal-account/notifications/alert',
  },
  moduleLink: {
    report: 'https://www.pre-prod.opal2.conexus.net/report',
    learner: 'https://www.pre-prod.opal2.conexus.net/app/learner',
    calendar: 'https://www.pre-prod.opal2.conexus.net/app/calendar',
  },
};
