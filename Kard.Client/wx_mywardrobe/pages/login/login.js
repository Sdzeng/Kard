// pages/login/login.js

//index.js
//获取应用实例
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {

  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    
  },
  
  wxlogin: function (e) {
    console.log('wxlogin')
    const that = this;

    app.globalData.user.info = e.detail.userInfo
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
            url: app.globalData.apiHost + '/wx/login',
            data: {
              code: res.code,
              userInfo: app.globalData.user.info
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
 
})