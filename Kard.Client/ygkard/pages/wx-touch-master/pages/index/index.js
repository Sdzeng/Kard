//index.js
//获取应用实例
var app = getApp();
var wxTouch = require('../../lib/wxtouch.js');
Page({
  data: {
    motto: 'Hello World',
    userInfo: {}
  },
  //事件处理函数
  bindViewTap: function() {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  onLoad: function () {
    console.log('onLoad')
    var that = this
    wxTouch.init(this);
    console.log(this);
    var handler = wxTouch.createHandler('test', 'swipeH', function(e, input) { console.log(input)});
    //调用应用实例的方法获取全局数据
    app.getUserInfo(function(userInfo){
      //更新数据
      that.setData({
        userInfo:userInfo
      })
    })
  }
})
