//app.js
App({
  appData:{
    deviceInfo:{},
  },

  onLaunch: function () {
    this.appData.deviceInfo = wx.getSystemInfoSync();
  },
})