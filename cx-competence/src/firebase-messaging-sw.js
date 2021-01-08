importScripts('https://www.gstatic.com/firebasejs/6.2.4/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/6.2.4/firebase-messaging.js');

firebase.initializeApp({ messagingSenderId: "618258569847" });
var messaging = firebase.messaging.isSupported() ? firebase.messaging() : null;

self.addEventListener('push', function (event) {
    var data = event.data.json();
    clients.matchAll({ includeUncontrolled: true, type: 'window' }).then(function (clientList) {
        if (clientList.length > 0) {
            data.broadcastFromServiceWorker = true;
            clientList.forEach(client => {
                if(client.visibilityState === 'visible') { return; }
                messageToClient(client, data);
            })
        }
    });
});

function messageToClient(client, data) {
    return new Promise(function (resolve, reject) {
        const channel = new MessageChannel();
        client.postMessage(data, [channel.port2]);
    });
}