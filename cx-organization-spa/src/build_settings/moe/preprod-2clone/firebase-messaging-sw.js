importScripts('https://www.gstatic.com/firebasejs/6.2.4/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/6.2.4/firebase-messaging.js');

// NOTE: The source code of this file should be the same with firebase-messaging-sw.js
//TODO replace with correct one
firebase.initializeApp({ messagingSenderId: '335736611347' });
var messaging = firebase.messaging();

self.addEventListener('push', function (event) {
  var data = event.data.json();
  clients
    .matchAll({ includeUncontrolled: true, type: 'window' })
    .then(function (clientList) {
      if (clientList.length > 0) {
        data.broadcastFromServiceWorker = true;
        clientList.forEach((client) => {
          if (client.visibilityState === 'visible') {
            return;
          }
          messageToClient(client, data);
        });
      }
    });
});

function messageToClient(client, data) {
  return new Promise(function (resolve, reject) {
    const channel = new MessageChannel();
    client.postMessage(data, [channel.port2]);
  });
}
