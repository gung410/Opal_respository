//#region Properties and Fields
var startTime;
var isRequiredRequestSent = false;

if (!window.ScormPlayer) {
  window.ScormPlayer = {};
}

window.ScormPlayer.createOrUpdateLearningPackageUrl = function() {
  return window.ScormPlayer.initData.learnerApiUrl + '/me/learning/createOrUpdateLearningPackage';
};

window.ScormPlayer.getLearningPackageUrl = function() {
  return window.ScormPlayer.initData.learnerApiUrl + '/me/learning/getLearningPackage';
};

window.ScormPlayer.onContentLoaded = function() {
  raiseEvent('ContentLoadedEvent');
};

window.ScormPlayer.onTokenInvalid = function() {
  raiseEvent('InvalidTokenEvent');
};

window.ScormPlayer.onFinished = function() {
  raiseEvent('FinishedEvent');
};

window.ScormPlayer.onPackageInvalid = function() {
  raiseEvent('PackageInvalid');
};

window.ScormPlayer.getScormData = getScormData;
window.ScormPlayer.sendScormData = sendScormData;
window.ScormPlayer.init = init;

/**
 * event format:
 * {
 *    key: string,
 *    // 'preview' | 'learn' | 'view'
 *    displayMode: string,
 *    contentApiUrl: string,
 *    learnerApiUrl: string,
 *    cloudFrontUrl: string,
 *    accessToken: string,
 *    digitalContentId: string,
 *    myLectureId: string,
 *    myDigitalContentId: string,
 *    reinitializePlayer: boolean  (true: force re-assign data for window.ScormPlayer.initData, false: normal)
 * }
 */
window.addEventListener('message', function(event) {
  try {
    var message = JSON.parse(event.data);

    if (message.key === 'scorm-init') {
      init(message);
    }

    // Trigger by LMS
    if (message.key === 'FinishedToTheEnd') {
      sendScormData(
        {
          completion_status: message.completion_status,
          success_status: message.success_status,
          total_time: Math.floor((Date.now() - startTime) / 1000),
          data: window.ScormPlayer.data
        },
        true
      );
    }

    // Manually triggered by users
    if (message.key === 'lecture-next-button-clicked') {
      sendScormData(
        {
          completion_status: false,
          success_status: false,
          total_time: Math.floor((Date.now() - startTime) / 1000),
          data: window.ScormPlayer.data
        },
        true
      );
    }
  } catch (error) {
    // Ignore the error when parsing data from window event.
  }
});
//#endregion

//#region Core Functions
function init(message) {
  // Start timer
  startTime = Date.now();

  // Add reinitializePlayer to assign new data when user click in table of content at the left side in UI
  if (!window.ScormPlayer.initData || (message && message.reinitializePlayer)) {
    var initData = {
      displayMode: message.displayMode || 'local',
      contentApiUrl: message.contentApiUrl,
      learnerApiUrl: message.learnerApiUrl,
      cloudFrontUrl: message.cloudFrontUrl,
      accessToken: message.accessToken,
      digitalContentId: message.digitalContentId,
      myLectureId: message.myLectureId,
      myDigitalContentId: message.myDigitalContentId,
      userId: message.userId || '',
      fileLocation: message.fileLocation
    };
    window.ScormPlayer.initData = initData;
    window.ScormPlayer.accessToken = message.accessToken;
  }

  if (window.ScormPlayer.initData.displayMode === 'local') {
    if (window.ScormPlayer.initData.fileLocation) {
      openStartScreen();
    } else {
      getDigitalContent().then(() => openStartScreen());
    }
  } else {
    Promise.all([getScormData(), getDigitalContent()]).then(results => {
      var response = results[0];

      if (!response) {
        openStartScreen();
        return;
      }

      if (response.completion_status) {
        window.ScormPlayer.initData.displayMode = 'view';
        openCompleteScreen();

        if (window.ScormPlayer.initData.myDigitalContentId) {
          raiseEvent('FinishedToTheEnd');
        }
      } else {
        if (window.ScormPlayer.initData.displayMode === 'view') {
          openCompleteScreen();
        } else {
          openStartScreen();
        }
      }

      if (response.data) {
        Player.PersistentStateStorage.prototype.scormData = response.data;
      }
    });
  }
}

function openStartScreen() {
  document.querySelector('#start-screen').style.display = 'flex';
  document.querySelector('#complete-screen').style.display = 'none'; // Need to hide complete-screen while start-screen displayed
}

function openCompleteScreen() {
  document.querySelector('#start-screen').style.display = 'none'; // Need to hide start-screen while complete-screen displayed
  document.querySelector('#complete-screen').style.display = 'flex';
}

function getDigitalContent() {
  var parameters = window.ScormPlayer.initData;
  var url = parameters.contentApiUrl + '/contents/' + parameters.digitalContentId;

  return get(url).then(result => {
    parameters.fileLocation = parameters.cloudFrontUrl + '/' + JSON.parse(result.responseText).fileLocation;
  });
}

function initPlayer() {
  var parameters = window.ScormPlayer.initData;
  PlayerConfiguration.Debug = false;
  PlayerConfiguration.StorageSupport = true;
  PlayerConfiguration.TreeMinusIcon = 'images/minus.gif';
  PlayerConfiguration.TreePlusIcon = 'images/plus.gif';
  PlayerConfiguration.TreeLeafIcon = 'images/leaf.gif';
  PlayerConfiguration.TreeActiveIcon = 'images/select.gif';
  PlayerConfiguration.BtnPreviousLabel = 'Previous';
  PlayerConfiguration.BtnContinueLabel = 'Continue';
  PlayerConfiguration.BtnExitLabel = 'Exit';
  PlayerConfiguration.BtnExitAllLabel = 'Exit All';
  PlayerConfiguration.BtnAbandonLabel = 'Abandon';
  PlayerConfiguration.BtnAbandonAllLabel = 'Abandon All';
  PlayerConfiguration.BtnSuspendAllLabel = 'Suspend All';

  Run.ManifestByURL(parameters.fileLocation + '/' + 'imsmanifest.xml');
  document.querySelector('#start-screen').style.display = 'none';
  document.querySelector('#complete-screen').style.display = 'none';
}

function get(url, ignoreHeaders) {
  return request('GET', url, null, ignoreHeaders);
}

function post(url, data) {
  return request('POST', url, data);
}

function request(method, url, data, ignoreHeaders) {
  return new Promise((resolve, reject) => {
    var xhr = new XMLHttpRequest();

    xhr.open(method, url, true);
    if (!ignoreHeaders) {
      xhr.setRequestHeader('Authorization', 'Bearer ' + window.ScormPlayer.accessToken);
      xhr.setRequestHeader('Content-Type', 'application/json');
    }
    xhr.onload = function(e) {
      if (xhr.readyState === 4) {
        if (xhr.status === 200 || xhr.status === 204) {
          resolve(this);
        } else if (xhr.status === 401 || xhr.status === 403) {
          raiseEvent('InvalidToken');
          reject(this);
        } else {
          console.error(xhr.statusText);
          reject(this);
        }
      }
    };
    xhr.onerror = function(e) {
      console.error(xhr.statusText);
    };

    if (data) {
      xhr.send(data);
    } else {
      xhr.send();
    }
  });
}

function getScormData() {
  var parameters = window.ScormPlayer.initData;

  var url = window.ScormPlayer.getLearningPackageUrl();

  if (parameters.myLectureId) {
    url += '?myLectureId=' + parameters.myLectureId;
  }

  if (parameters.myDigitalContentId) {
    url += url.includes('?') ? '&' : '?';
    url += 'myDigitalContentId=' + parameters.myDigitalContentId;
  }

  return get(url).then(result => {
    if (!result.responseText) {
      return;
    }

    var jsonResult = JSON.parse(result.responseText);
    var data = JSON.parse(jsonResult.state);

    return data;
  });
}

function sendScormData(status, isRequiredRequest = false) {
  if (!isRequiredRequest || isRequiredRequestSent || window.ScormPlayer.initData.displayMode !== 'learn') {
    return;
  }
  isRequiredRequestSent = true;
  setTimeout(function() {}, 100);

  var request = {
    myLectureId: window.ScormPlayer.initData.myLectureId,
    myDigitalContentId: window.ScormPlayer.initData.myDigitalContentId,
    type: 'SCORM',
    state: status
  };

  raiseEvent('FinishedToTheEnd');
  return post(window.ScormPlayer.createOrUpdateLearningPackageUrl(), JSON.stringify(request));
}

function raiseEvent(eventName) {
  var msg = JSON.stringify({
    key: eventName
  });
  window.parent.postMessage(msg, '*');

  if (eventName === 'FinishedToTheEnd' && window.ScormPlayer.onScormPlayerFinishHandler) {
    window.ScormPlayer.onScormPlayerFinishHandler();
  }
}
//#endregion
