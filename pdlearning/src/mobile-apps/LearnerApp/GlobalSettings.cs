namespace LearnerApp
{
    public class GlobalSettings
    {
        public const string AuthClientId = "opal2LearnerMobile";
        public const string AuthScope = "openid SharedServicesApi cxDomainInternalApi offline_access requestPasswordlessUrl";

        // TODO: Need to change this variable to AppUrlScheme
        public const string AuthRedirectUrl = "com.opal2.moe.edu.sg.development";
        public const string AuthAuthorizeUrl = "/connect/authorize";
        public const string AuthAccessTokenUrl = "/connect/token";
        public const string AuthCallbackUrl = AuthRedirectUrl + "://oauthredirect";
        public const string LearningOpportunityUri = "urn:opal2.moe.sg:coursepad-pdo";

        public const string AppCenterSecretIOS = "6a4fff1f-6b9c-427f-b66e-91bdd73e7e9b";
        public const string AppCenterSecretAndroid = "0a86845f-2014-4352-a412-ae73b0d0b383";

        public const string WebViewUrlPdPlanner = "https://www.development.opal2.conexus.net/pdplanner/mobile-mpj-module";
        public const string WebViewUrlEportfolio = "https://www.development.opal2.conexus.net/eportfolio";
        public const string WebViewUrlSocial = "https://www.development.opal2.conexus.net/csl";
        public const string WebViewUrlAccountProfile = "https://www.development.opal2.conexus.net/opal-account/profile";
        public const string WebViewUrlOnBoarding = "https://www.development.opal2.conexus.net/opal-account/onboarding";
        public const string WebViewUrlTermOfUse = "https://idm.development.opal2.conexus.net/Home/TermsOfUse";
        public const string WebViewUrlPrivacyPolicy = "https://idm.development.opal2.conexus.net/Home/PrivacyPolicy";
        public const string WebViewUrlCalendar = BackendBaseUrl + "/app/calendar";
        public const string WebViewUrlReport = BackendBaseUrl + "/report/?q=P1dAlX6Zh2uF9DQ76MgHpLL3yQEvWQ+Chqe8ekhhojLQdZ2up8S4R3FhD3MpvIKG";

        public const string BackendBaseUrl = "https://www.development.opal2.conexus.net";
        public const string BackendApiBaseUrl = "https://api.development.opal2.conexus.net";
        public const string BackendServicePortal = "https://api.development.opal2.conexus.net/development-competence-opal-api-portal";
        public const string BackendServiceIdm = "https://idm.development.opal2.conexus.net";
        public const string BackendServiceLearner = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-learner";
        public const string BackendServiceCourse = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-course";
        public const string BackendServiceOrganization = "https://api.development.opal2.conexus.net/development-competence-opal-api-organization";
        public const string BackendServiceUserManagement = "https://api.development.opal2.conexus.net/development-competence-opal-api-organization/usermanagement";
        public const string BackendServiceContent = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-content";
        public const string BackendServiceTagging = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-tagging";
        public const string BackendServiceUploader = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-uploader";
        public const string BackendServiceUserAvatar = "https://dexzs7wx4278r.cloudfront.net/avatar";
        public const string BackendServiceCloudFront = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-cloudfront";
        public const string BackendServicePDPM = "https://api.development.opal2.conexus.net/development-competence-opal-api";
        public const string BackendPdCatelogueService = "https://api.development.opal2.conexus.net/development-competence-opal-api-learningcatalog";
        public const string BackendServiceSocial = "https://api.development.opal2.conexus.net/development-socialapp-opal-csl-api";
        public const string BackendServiceCommunication = "https://api.development.opal2.conexus.net/development-datahub-opal-api-communication";
        public const string BackendServiceBrokenLink = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-brokenlink";
        public const string BackendServiceNewsfeed = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-newsfeed";
        public const string BackendServiceWebinar = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-webinar";
        public const string BackendServiceCalendar = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-calendar";
        public const string BackendServiceForm = "https://api.development.opal2.conexus.net/development-learnapp-opal-api-form";
        public const string BackendServiceBadge = BackendApiBaseUrl + "/development-learnapp-opal-api-badge";

        public const string CloudFrontUrl = "https://dexzs7wx4278r.cloudfront.net";
        public const string AuthRedirectLogoutUrl = "https://www.development.opal2.conexus.net/app/learner";

        public const string LastUpdateString = "15 June 2020";
        public const string NotAvailable = "N/A";
        public const string DepartmentPlaceOfWork = "ApplicableForUsersInSpecificOrganisation";
        public const int MaxResultPerPage = 10;

        public const string WebViewUrlStandAloneFormPlayer = "https://www.development.opal2.conexus.net/app/form-standalone-player";
        public const string WebViewUrlAssignmentPlayer = "https://www.development.opal2.conexus.net/app/assignment-player";
        public const string WebViewUrlQuizPlayer = "https://www.development.opal2.conexus.net/app/quiz-player";
        public const string WebViewUrlDigitalContentPlayer = "https://www.development.opal2.conexus.net/app/digital-content-player";
        public const string WebViewUrlNotification = "https://www.development.opal2.conexus.net/opal-account/notifications";
        public const string WebViewLearningContentPlayer = CloudFrontUrl + "/permanent/learning-content-player/learning-content-player.html";
        public const string WebViewCloudFrontAuthenticationHtml = "<form id=\"cloudfront-form\" method=\"POST\" action=\"" + CloudFrontUrl + "/api/cloudfront/signin?returnUrl={{RETURN_URL}}\"> <input id=\"token\" name=\"AccessToken\" type=\"text\" style=\"visibility: hidden;\" value=\"{{TOKEN}}\"/> </form> <script type=\"text/javascript\">window.onload=function (){document.querySelector('#cloudfront-form').submit();}</script>";
        public const string WebViewGoogleDocumentViewer = "https://docs.google.com/gview?url=";
        public const string WebViewUrlScormPlayer = CloudFrontUrl + "/permanent/scorm-player/scorm-player.html";
    }
}
