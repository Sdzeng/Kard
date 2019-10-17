//app.js
App({
  onLaunch: function () {
    const that=this
    // 展示本地存储能力
    // var logs = wx.getStorageSync('logs') || []
    // logs.unshift(Date.now())
    // wx.setStorageSync('logs', logs)

    
  },
  
  globalData: {
    apiHost: "http://192.168.10.3:5000",
    user: { cookie: '', info: null }
     
  }
})