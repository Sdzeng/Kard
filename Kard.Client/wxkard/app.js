//app.js
App({
  onLaunch: function () {
    // 展示本地存储能力
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)
    this.globalData.deviceInfo = wx.getSystemInfoSync();
    this.login()
   
  },
  login:function(){
    const that=this;
    // 登录
    wx.checkSession({
      success: function () {
        //session_key 未过期，并且在本生命周期一直有效
      },
      fail: function () {
        wx.login({
          success: res => {
            // 发送 res.code 到后台换取 openId, sessionKey, unionId
            if (res.code) {
              //发起网络请求
              wx.request({
                url: 'http://localhost:3706/wx/alive',
                data: {
                  code: res.code
                },
                header: {
                  'content-type': 'application/json' // 默认值
                },
                success: function (res) {
                  console.log(JSON.stringify(res.header))
                  if (res.data.result) {
                    wx.setStorageSync("cookie", res.header["Set-Cookie"])
                    that.auth()
                  }
                }
              })
            } else {
              console.log('登录失败！' + res.errMsg)
            }
          }
        })
      }
    })

  },
  auth:function(){
    const that=this
    // 获取用户信息
    wx.getSetting({
      success: res => {
       
        if (!res.authSetting['scope.userInfo']) {
          console.log("弹出")
          wx.authorize({
            scope: 'scope.userInfo',
            success(){
              console.log('success')
              that.getUserInfo()
            },
            fail() {
              console.log('授权失败！')
            },
            complete(){
              console.log('授权完成！')

            }
          })
        }
        else
        { 
          console.log("res" + JSON.stringify(res))
          that.getUserInfo()
        }
 
      }
    })
  },
  getUserInfo: function (res){
    console.log('getUserInfo')
    const that = this
    // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
    wx.getUserInfo({
      success: res => {
        // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
        // 所以此处加入 callback 以防止这种情况
         // 可以将 res 发送给后台解码出 unionId
        this.globalData.userInfo = res.userInfo
        //发起网络请求
        wx.request({
          url: 'http://localhost:3706/wx/login',
          method:'POST',
          header:{
            'cookie': wx.getStorageSync("cookie")
          },
          data:JSON.stringify(res.userInfo),
          //{
           // user:
           
            //     "AvatarUrl": res.userInfo.avatarUrl
            // gender: gender,
            // city: city,
            // country:
          //},
         
          complete: function (res1) {
            if (res1.data.result) {
              console.log('kard登陆成功')
            }
          }
        })
      }
    })
   
   
  
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
    currentDate: { year: 1900, month: 1, day: 1 },
    userInfo: null,
    deviceInfo: null

  }
})