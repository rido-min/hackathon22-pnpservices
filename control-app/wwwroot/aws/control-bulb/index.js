var AWS = require('aws-sdk');
var AWSIoTData = require('aws-iot-device-sdk');
var AWSConfiguration = require('./aws-configuration.js');

console.log('Loaded AWS SDK for JavaScript and AWS IoT SDK for Node.js');
AWS.config.region = AWSConfiguration.region;

AWS.config.credentials = new AWS.CognitoIdentityCredentials({
   IdentityPoolId: AWSConfiguration.poolId
});

var currentlySubscribedTopic = '$aws/things/bulb1/#';
var messageHistory = '';
var clientId = 'mqtt-explorer-' + (Math.floor((Math.random() * 100000) + 1));
AWS.config.region = AWSConfiguration.region;

AWS.config.credentials = new AWS.CognitoIdentityCredentials({
   IdentityPoolId: AWSConfiguration.poolId
});

const mqttClient = AWSIoTData.device({
    //
    // Set the AWS region we will operate in.
    //
    region: AWS.config.region,
    //
    ////Set the AWS IoT Host Endpoint
    host:AWSConfiguration.host,
    //
    // Use the clientId created earlier.
    //
    clientId: clientId,
    //
    // Connect via secure WebSocket
    //
    protocol: 'wss',
    //
    // Set the maximum reconnect time to 8 seconds; this is a browser application
    // so we don't want to leave the user waiting too long for reconnection after
    // re-connecting to the network/re-opening their laptop/etc...
    //
    maximumReconnectTimeMs: 8000,
    //
    // Enable console debugging information (optional)
    //
    debug: true,
    //
    // IMPORTANT: the AWS access key ID, secret key, and sesion token must be 
    // initialized with empty strings.
    //
    accessKeyId: '',
    secretKey: '',
    sessionToken: ''
 });

 var cognitoIdentity = new AWS.CognitoIdentity();
AWS.config.credentials.get(function(err, data) {
   if (!err) {
      console.log('retrieved identity: ' + AWS.config.credentials.identityId);
      var params = {
         IdentityId: AWS.config.credentials.identityId
      };
      cognitoIdentity.getCredentialsForIdentity(params, function(err, data) {
         if (!err) {
            //
            // Update our latest AWS credentials; the MQTT client will use these
            // during its next reconnect attempt.
            //
            mqttClient.updateWebSocketCredentials(data.Credentials.AccessKeyId,
               data.Credentials.SecretKey,
               data.Credentials.SessionToken);
         } else {
            console.log('error retrieving credentials: ' + err);
            alert('error retrieving credentials: ' + err);
         }
      });
   } else {
      console.log('error retrieving identity:' + err);
      alert('error retrieving identity: ' + err);
   }
});

window.mqttClientConnectHandler = function() {
    console.log('connect');
    document.getElementById("connecting-div").style.visibility = 'hidden';
    // document.getElementById("explorer-div").style.visibility = 'visible';
    // document.getElementById('subscribe-div').innerHTML = '<p><br></p>';
    messageHistory = '';
 
    //
    // Subscribe to our current topic.
    //
    mqttClient.subscribe(currentlySubscribedTopic);
    mqttClient.subscribe('pnp/bulb1/#');
 };

 window.mqttClientReconnectHandler = function() {
    console.log('reconnect');
    //document.getElementById("connecting-div").style.visibility = 'visible';
    //document.getElementById("explorer-div").style.visibility = 'hidden';
 };

 window.isUndefined = function(value) {
    return typeof value === 'undefined' || typeof value === null;
 };

 window.mqttClientMessageHandler = function(topic, payload) {
    console.log('message: ' + topic + ':' + payload.toString());
    if (topic==='pnp/bulb1/telemetry') {
        var msg = JSON.parse(payload)
        document.getElementById('battLife').innerText = msg.batteryLife
    }

    if (topic==='$aws/things/bulb1/shadow/update') {
        var msg = JSON.parse(payload)
        console.log('REPORTED STATE:', msg.state.reported)    
        if (msg &&
            msg.state &&
            msg.state.reported &&
            msg.state.reported.lightState ) {

            document.getElementById('lightState').innerText = msg.state.reported.lightState.value === 1 ? 'On' : 'Off'
            document.getElementById('lightStateDesc').innerText = msg.state.reported.lightState.ad + ' [v: ' + msg.state.reported.lightState.av + ']'
            document.getElementById('stateOn').checked = undefined
            document.getElementById('stateOff').checked = undefined
        }
    }

    //messageHistory = messageHistory + topic + ':' + payload.toString() + '</br></br>';
    //document.getElementById('subscribe-div').innerHTML = '<p>' + messageHistory + '</p>';
 };

 window.updateSubscriptionTopic = function() {
    var subscribeTopic = document.getElementById('subscribe-topic').value;
    //document.getElementById('subscribe-div').innerHTML = '';
    mqttClient.unsubscribe(currentlySubscribedTopic);
    currentlySubscribedTopic = subscribeTopic;
    mqttClient.subscribe(currentlySubscribedTopic);
 };

 window.clearHistory = function() {
    if (confirm('Delete message history?') === true) {
       document.getElementById('subscribe-div').innerHTML = '<p><br></p>';
       messageHistory = '';
    }
 };

 window.updatePublishTopic = function() {};

 window.updatePublishData = function() {
    //var publishText = document.getElementById('publish-data').value;
    //var publishTopic = document.getElementById('publish-topic').value;
 
    //mqttClient.publish(publishTopic, publishText);
    //document.getElementById('publish-data').value = '';
 };

mqttClient.on('connect', window.mqttClientConnectHandler);
mqttClient.on('reconnect', window.mqttClientReconnectHandler);
mqttClient.on('message', window.mqttClientMessageHandler);

//
// Initialize divs.
//
// document.getElementById('connecting-div').style.visibility = 'visible';
// document.getElementById('explorer-div').style.visibility = 'hidden';
document.getElementById('connecting-div').innerHTML = '<p>attempting to connect to aws iot...</p>';

window.toggle = function () {
    var radio = document.getElementById('stateOn')
    mqttClient.publish('$aws/things/bulb1/shadow/update', JSON.stringify( { 
            state : { 
                desired : {
                    lightState : radio.checked ? 1 : 0
                }
            }
        })
    )
}