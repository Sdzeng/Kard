//app.js
App({
  onLaunch: function () {
    const that = this;
    // 展示本地存储能力
    // var logs = wx.getStorageSync('logs') || []
    // logs.unshift(Date.now())
    // wx.setStorageSync('logs', logs)
    if (!that.globalData.deviceInfo) {
      that.globalData.deviceInfo = wx.getSystemInfoSync();
    }

    // var isLogin=()=>{
    //   that.auth(() => {
    //     that.getUserInfo(() => {
    //       that.wxregister()
    //     })
    //   })
    // }

    // var noLogin=()=>{
    //   that.wxlogin(() => {
    //     that.auth(() => {
    //       that.getUserInfo(() => {
    //         that.wxregister()
    //       })
    //     })
    //   })
    // }

    // //授权
    // wx.checkSession({
    //   success: function () {
    //     //session_key 未过期，并且在本生命周期一直有效
    //     if (wx.getStorageSync("cookie"))
    //     {
    //       that.isLogin()
    //     }
    //     else{
    //       that.noLogin()
    //     }
    //   },
    //   fail: function () {
    //     that.noLogin()
    //   }
    // })


    that.wxlogin(() => {
      that.auth(() => {
        that.getUserInfo(() => {
          that.wxregister()
        })
      })
    })



  },

  wxlogin: function (callback) {
    console.log('wxlogin')
    const that = this;

    wx.showLoading({
      title: '登陆中',
    })

    var failProcess=(message)=>{
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
            url: this.globalData.apiHost+'/wx/login',
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
              }else
              {
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
      fail:function(){
        failProcess('登录失败')
      }
      

    })


  },

  auth: function (callback) {
    console.log('auth')
    const that = this
    // 获取用户信息
    wx.getSetting({
      success: res => {

        if (res.authSetting == null||(!res.authSetting['scope.userInfo'])) { 
          wx.authorize({
            scope: 'scope.userInfo',
            success() {
              console.log('用户信息授权成功')
              if (callback) {
                callback()
              }
            },
            fail() {
             wx.openSetting({
               success: (res2) => { }
             })
            }
          })
        }else
        {
           console.log('用户信息授权成功')
              if (callback) {
                callback()
              }
        } 

       

      }
    })
  },
  getUserInfo: function (callback) {
    console.log('getUserInfo')
    const that = this
    // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
    if (!that.globalData.user.info) {
      wx.getUserInfo({
        withCredentials: true,
        success: res => {
          // 可以将 res 发送给后台解码出 unionId
          that.globalData.user.info = res.userInfo
          if (callback) {
            callback()
          }
        }
      })
    }
    else {
      if (callback) {
        callback()
      }
    }
  },
  // hasopen: function (callback) {
  //   console.log('hasopen')
  //   const that = this
  //   wx.request({
  //     url: this.globalData.apiHost+'/wx/hasopen',
  //     method: 'GET',
  //     header: that.globalData.user.cookie,
  //     success: function (res) {

  //       that.globalData.user.hasopen = res.data.result
  //       if (callback) {
  //         callback()
  //       }
  //     },
  //     fail: function () {
  //       that.globalData.user.hasopen = false
  //       if (callback) {
  //         callback()
  //       }
  //     }

  //   })
  // },

  wxregister: function (callback) {
    console.log('wxregister')
    const that = this
    var register = () => {
      wx.request({
        url: this.globalData.apiHost +'/wx/register',
        method: 'POST',
        header: {
          'cookie': wx.getStorageSync("cookie")
        },
        data: JSON.stringify(that.globalData.user.info),

        success: function (res) {

          if (res.data.result) {
            console.log(res.data.message)

            if (callback) {
              callback()
            }

             
          }
        },
        fail: function () {
          console.log('用户注册失败')
        }

      })
    }

    if (wx.getStorageSync("cookie")) {
      register()
    }
    else {
      that.wxlogin(register)

    }

  },

  onShow: function () {
    const date = new Date();
    this.globalData.currentDate.year = date.getFullYear();
    this.globalData.currentDate.month = date.getMonth() + 1;
    this.globalData.currentDate.day = date.getDate();
  },

  onError: function () {

    console.log(msg)
  },
  globalData: {
    apiHost:"http://www.localyc.com",
    currentDate: { year: 1900, month: 1, day: 1 },

    user: { cookie: '', info: null },
    deviceInfo: null
 
  }
})