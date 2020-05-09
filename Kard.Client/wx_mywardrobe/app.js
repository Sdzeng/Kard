//app.js
App({
  onLaunch: function () {
    const that=this
    // 展示本地存储能力
    // var logs = wx.getStorageSync('logs') || []
    // logs.unshift(Date.now())
    // wx.setStorageSync('logs', logs)
    that.wxlogin()
    
  },
  wxlogin: function (callback) {
    console.log('wxlogin')
    const that = this;

    wx.showLoading({
      title: '登陆中',
    })

    var failProcess = (message) => {
      wx.showToast({
        title: message,
        icon: 'none',
        duration: 2000
      })
    }

    // 登录
    wx.login({
      success: res => {
        // 发送 res.code 到后台换取 openId, sessionKey, unionId
        if (res.code) {

          //发起网络请求
          wx.request({
            url: this.globalData.apiHost + '/login/wxapplogin',
            data: {
              code: res.code
            },
            header: {
              'content-type': 'application/json' // 默认值
            },
            success: function (res) {
              wx.setStorageSync("cookie", res.header["Set-Cookie"])
              //that.globalData.user.cookie = res.header["Set-Cookie"]
              if (res.data.result) {
                console.log('登录成功！')
                if (callback) {
                  callback()
                }
              } else {
                failProcess(res.data.message)
              }
            },
            fail: function () {
              failProcess('登录失败')
            },
            complete: function () {
              wx.hideLoading()
            }


          })

        } else {
          failProcess('登录失败:' + res.errMsg)

        }
      },
      fail: function () {
        failProcess('登录失败')
      }


    })


  },
  
  globalData: {
    apiHost: "http://192.168.10.3:5000",
    user: { cookie: '', info: null }
     
  }
})