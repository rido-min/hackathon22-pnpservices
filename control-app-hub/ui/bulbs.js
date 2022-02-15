import * as apiClient from './apiClient.js'
const protocol = document.location.protocol.startsWith('https') ? 'wss://' : 'ws://'
const webSocket = new window.WebSocket(protocol + window.location.host)

  ; (async () => {
    const createVueApp = () => {
      return new Vue({
        el: '#app',
        data: {
          devices: [],
          replaced: Date.now(),
          replacedAgo : '',
          battLife: 100,
          lightState: 'on',
          lightStateDesc : '',
          desiredState : ''
        },
        methods: {
          toggle: function () {
          }
        },
        watch: {
          desiredState : async function(nv) {
            //console.log(nv==='Off' ? 0 : 1)
            if (nv!==undefined) {
              await apiClient.updateDeviceTwin('bulb1', 'lightState', nv==='Off' ? 0 : 1)
            }
          }
        }
      })
    }
    const app = createVueApp()
    const twin = await apiClient.getDeviceTwin('bulb1')

    const updateReportedUI = r => {
      if (r['lastBatteryReplacement']) {
        Vue.set(app, 'replaced', r['lastBatteryReplacement'])
      }
      if (r['lightState']) {
        Vue.set(app, 'lightState', r['lightState'].value===1 ? 'On' : 'Off')
        Vue.set(app, 'lightStateDesc', r['lightState'].ad + '[v: ' + r['lightState'].av + ']' )
        Vue.set(app, 'desiredState', undefined)
      }

      // for (const p in r) {
      //   console.log(p, r[p])
      //   if (p === 'lastBatteryReplacement') {
      //     Vue.set(app.data, 'replaced', p)
      //     //Vue.set(prop, 'lastUpdated', moment(desired.$metadata[p].$lastUpdated).fromNow())
      //   }
      // }
    }

    const updateTwin = async t => {
      await apiClient.updateDeviceTwin('bulb1', lightState, t==='Off' ? 1 : 0)
    }

    webSocket.onmessage = (message) => {
      const messageData = JSON.parse(message.data)
      //console.log(messageData)
      if (messageData.IotData.properties && messageData.IotData.properties.reported) {
        updateReportedUI(messageData.IotData.properties.reported)
        return
      }
      if (messageData.IotData.batteryLife) {
        Vue.set(app, 'battLife', messageData.IotData.batteryLife)
        Vue.set(app, 'replacedAgo', moment(app.replaced).fromNow())
      }
      // if (messageData.IotData.properties && messageData.IotData.properties.desired) {
      //   updateDesired(messageData.IotData.properties.desired)
      //   return
      // }


    }

  })()
