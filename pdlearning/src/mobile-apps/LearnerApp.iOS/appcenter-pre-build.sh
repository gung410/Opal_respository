#!/usr/bin/env bash

# APP_ENV, APP_VERSION_NAME, APP_VERSION_CODE, APP_BUNDLE_SHORT_VERSION_STRING must be passed through AppCenter Configuration
# This is for local testing purpose.
# APP_ENV=prod
# APP_VERSION_NAME="1.0"
# APP_BUNDLE_SHORT_VERSION_STRING="1.0.0"
# APPCENTER_SOURCE_DIRECTORY=C:/Users/toan.nguyen/Documents/Dev/opal20-platform
APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/src/mobile-apps/LearnerApp/GlobalSettings.cs
APP_CONFIG_FILE=$APPCENTER_SOURCE_DIRECTORY/src/mobile-apps/configs/${APP_ENV}.properties
APP_PLIST=$APPCENTER_SOURCE_DIRECTORY/src/mobile-apps/LearnerApp.iOS/Info.plist
APP_ANDROID_MANIFEST=$APPCENTER_SOURCE_DIRECTORY/src/mobile-apps/LearnerApp.Droid/Properties/AndroidManifest.xml

echo "Read file: "$APP_CONSTANT_FILE"."
echo "Read file: "$APP_CONFIG_FILE"."
echo "Read file: "$APP_PLIST"."
echo "Read file: "$APP_ANDROID_MANIFEST"."

update_global_settings() {
    echo "File content (Before):"
    cat "$APP_CONSTANT_FILE"

    # Update approriate config.
    sed -i '' 's#AuthRedirectUrl = "[-A-Za-z0-9:_./]*"#AuthRedirectUrl = "'$AuthRedirectUrl'"#' $APP_CONSTANT_FILE

    sed -i '' 's#AppCenterSecretIOS = "[-A-Za-z0-9:_./]*"#AppCenterSecretIOS = "'$AppCenterSecretIOS'"#' $APP_CONSTANT_FILE
    sed -i '' 's#AppCenterSecretAndroid = "[-A-Za-z0-9:_./]*"#AppCenterSecretAndroid = "'$AppCenterSecretAndroid'"#' $APP_CONSTANT_FILE

    sed -i '' 's#WebViewUrlPdPlanner = "[-A-Za-z0-9:_./]*"#WebViewUrlPdPlanner = "'$WebViewUrlPdPlanner'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlSocial = "[-A-Za-z0-9:_./]*"#WebViewUrlSocial = "'$WebViewUrlSocial'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlAccountProfile = "[-A-Za-z0-9:_./]*"#WebViewUrlAccountProfile = "'$WebViewUrlAccountProfile'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlOnBoarding = "[-A-Za-z0-9:_./]*"#WebViewUrlOnBoarding = "'$WebViewUrlOnBoarding'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlTermOfUse = "[-A-Za-z0-9:_./]*"#WebViewUrlTermOfUse = "'$WebViewUrlTermOfUse'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlPrivacyPolicy = "[-A-Za-z0-9:_./]*"#WebViewUrlPrivacyPolicy = "'$WebViewUrlPrivacyPolicy'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlEportfolio = "[-A-Za-z0-9:_./]*"#WebViewUrlEportfolio = "'$WebViewUrlEportfolio'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlQuizPlayer = "[-A-Za-z0-9:_./]*"#WebViewUrlQuizPlayer = "'$WebViewUrlQuizPlayer'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlScormPlayer = "[-A-Za-z0-9:_./]*"#WebViewUrlScormPlayer = "'$WebViewUrlScormPlayer'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlDigitalContentPlayer = "[-A-Za-z0-9:_./]*"#WebViewUrlDigitalContentPlayer = "'$WebViewUrlDigitalContentPlayer'"#' $APP_CONSTANT_FILE

    sed -i '' 's#BackendServicePortal = "[-A-Za-z0-9:_./]*"#BackendServicePortal = "'$BackendServicePortal'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceIdm = "[-A-Za-z0-9:_./]*"#BackendServiceIdm = "'$BackendServiceIdm'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceLearner = "[-A-Za-z0-9:_./]*"#BackendServiceLearner = "'$BackendServiceLearner'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceCourse = "[-A-Za-z0-9:_./]*"#BackendServiceCourse = "'$BackendServiceCourse'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceOrganization = "[-A-Za-z0-9:_./]*"#BackendServiceOrganization = "'$BackendServiceOrganization'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceUserManagement = "[-A-Za-z0-9:_./]*"#BackendServiceUserManagement = "'$BackendServiceUserManagement'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceContent = "[-A-Za-z0-9:_./]*"#BackendServiceContent = "'$BackendServiceContent'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceTagging = "[-A-Za-z0-9:_./]*"#BackendServiceTagging = "'$BackendServiceTagging'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceUploader = "[-A-Za-z0-9:_./]*"#BackendServiceUploader = "'$BackendServiceUploader'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceUserAvatar = "[-A-Za-z0-9:_./]*"#BackendServiceUserAvatar = "'$BackendServiceUserAvatar'"#' $APP_CONSTANT_FILE 
    sed -i '' 's#BackendServiceCloudFront = "[-A-Za-z0-9:_./]*"#BackendServiceCloudFront = "'$BackendServiceCloudFront'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServicePDPM = "[-A-Za-z0-9:_./]*"#BackendServicePDPM = "'$BackendServicePDPM'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendPdCatelogueService = "[-A-Za-z0-9:_./]*"#BackendPdCatelogueService = "'$BackendPdCatelogueService'"#' $APP_CONSTANT_FILE
    sed -i '' 's#CloudFrontUrl = "[-A-Za-z0-9:_./]*"#CloudFrontUrl = "'$CloudFrontUrl'"#' $APP_CONSTANT_FILE
    sed -i '' 's#AuthRedirectUrl = "[-A-Za-z0-9:_./]*"#AuthRedirectUrl = "'$AuthRedirectUrl'"#' $APP_CONSTANT_FILE
	sed -i '' 's#AuthRedirectLogoutUrl = "[-A-Za-z0-9:_./]*"#AuthRedirectLogoutUrl = "'$AuthRedirectLogoutUrl'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceSocial = "[-A-Za-z0-9:_./]*"#BackendServiceSocial = "'$BackendServiceSocial'"#' $APP_CONSTANT_FILE
	sed -i '' 's#BackendServiceCommunication = "[-A-Za-z0-9:_./]*"#BackendServiceCommunication = "'$BackendServiceCommunication'"#' $APP_CONSTANT_FILE
	sed -i '' 's#WebViewUrlNotification = "[-A-Za-z0-9:_./]*"#WebViewUrlNotification = "'$WebViewUrlNotification'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendBaseUrl = "[-A-Za-z0-9:_./]*"#BackendBaseUrl = "'$BackendBaseUrl'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendApiBaseUrl = "[-A-Za-z0-9:_./]*"#BackendApiBaseUrl = "'$BackendApiBaseUrl'"#' $APP_CONSTANT_FILE
    sed -i '' 's#WebViewUrlAssignmentPlayer = "[-A-Za-z0-9:_./]*"#WebViewUrlAssignmentPlayer = "'$WebViewUrlAssignmentPlayer'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceBrokenLink = "[-A-Za-z0-9:_./]*"#BackendServiceBrokenLink = "'$BackendServiceBrokenLink'"#' $APP_CONSTANT_FILE
	sed -i '' 's#BackendServiceNewsfeed = "[-A-Za-z0-9:_./]*"#BackendServiceNewsfeed = "'$BackendServiceNewsfeed'"#' $APP_CONSTANT_FILE
    sed -i '' 's#BackendServiceWebinar = "[-A-Za-z0-9:_./]*"#BackendServiceWebinar = "'$BackendServiceWebinar'"#' $APP_CONSTANT_FILE
	sed -i '' 's#WebViewUrlStandAloneFormPlayer = "[-A-Za-z0-9:_./]*"#WebViewUrlStandAloneFormPlayer = "'$WebViewUrlStandAloneFormPlayer'"#' $APP_CONSTANT_FILE
	sed -i '' 's#BackendServiceCalendar = "[-A-Za-z0-9:_./]*"#BackendServiceCalendar = "'$BackendServiceCalendar'"#' $APP_CONSTANT_FILE
	sed -i '' 's#BackendServiceForm = "[-A-Za-z0-9:_./]*"#BackendServiceForm = "'$BackendServiceForm'"#' $APP_CONSTANT_FILE
    echo "File content (After):"
    cat "$APP_CONSTANT_FILE"
}

update_info_plist() {
    echo "Updating info.plist:"

    plutil -replace CFBundleIdentifier -string $AppIdentifier $APP_PLIST
    plutil -replace CFBundleDisplayName -string $AppName $APP_PLIST
    plutil -replace CFBundleVersion -string "$APPCENTER_BUILD_ID.0" $APP_PLIST
    plutil -replace CFBundleShortVersionString -string $APP_BUNDLE_SHORT_VERSION_STRING $APP_PLIST

    /usr/libexec/PlistBuddy -c "Set :CFBundleURLTypes:0:CFBundleURLSchemes:0 $AppIdentifier" $APP_PLIST

    echo "Updated info.plist:"
    cat $APP_PLIST
}

update_android_manifest() {
    echo "Updating AndroidManifest.xml:"

    sed -i '' 's/package="[^"]*"/package="'$AppIdentifier'"/' $APP_ANDROID_MANIFEST
    sed -i '' 's/android:versionName="[^"]*"/android:versionName="'$APP_VERSION_NAME'"/' $APP_ANDROID_MANIFEST

    echo "Updated AndroidManifest.xml:"
    cat $APP_ANDROID_MANIFEST
}

source $APP_CONFIG_FILE

update_global_settings
update_info_plist
update_android_manifest
