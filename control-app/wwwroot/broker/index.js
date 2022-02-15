import { getIoTHubV2Credentials } from 'https://unpkg.com/iothub-auth'
const hostname = 'broker.azure-devices.net'
const deviceId = 'd4'
const key = 'MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA='


; (async () => {
  const createVueApp = () => {
    return new Vue({
      el: '#app',
      data: {
        devices: [],
        replaced: Date.now(),
        replacedAgo: '',
        battLife: 100,
        lightState: 'on',
        lightStateDesc: '',
        desiredState: ''
      },
      methods: {
        toggle: function () {
        }
      },
      watch: {
        desiredState: async function (nv) {
          //console.log(nv==='Off' ? 0 : 1)
          if (nv !== undefined) {
            client.publish('pnp/bulb1/props/set', JSON.stringify({
                lightState : nv==='Off' ? 0 : 1
            }))        
          }
        }
      }
    })
  }

  const app = createVueApp()

  
  const updateReportedUI = r => {
    if (r['lastBatteryReplacement']) {
      Vue.set(app, 'replaced', r['lastBatteryReplacement'])
    }
    if (r['lightState']) {
      Vue.set(app, 'lightState', r['lightState'].value === 1 ? 'On' : 'Off')
      Vue.set(app, 'lightStateDesc', r['lightState'].ad)
      Vue.set(app, 'desiredState', undefined)
    }
  }
  
  let [username, password, websocket] = await getIoTHubV2Credentials(hostname, deviceId, key, 60)
  let client = mqtt.connect(`wss://${hostname}:443/${websocket}`, { clientId: deviceId, username, password })
  
  client.on('connect', () => {
    console.log('connected', client.connected)
    client.subscribe('pnp/#')
  })

  client.on('message', (topic, message) => {
    const messageData = JSON.parse(message)
    console.log(topic, messageData)
    if (topic === 'pnp/bulb1/props/reported') {
      updateReportedUI(messageData)
      return
    }
    if (topic === 'pnp/bulb1/telemetry') {
      Vue.set(app, 'battLife', messageData.batteryLife)
    }
  })

})()
