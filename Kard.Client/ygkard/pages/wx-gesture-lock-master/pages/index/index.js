//index.js

var Lock = require('../../lib/gesture_lock.js');

//获取应用实例
var app = getApp()
Page({
  data: {
    userInfo: {}
  },
  onLoad: function () {
    var s = this;
    this.lock = new Lock("id-gesture-lock", wx.createCanvasContext("id-gesture-lock"), function(checkPoints, isCancel) {
      s.lock.gestureError();
      setTimeout(function() {
        s.lock.reset();
      }, 1000);
    }, {width:300, height:300})
    this.lock.drawGestureLock();
  },
  onTouchStart: function (e) {
    this.lock.onTouchStart(e);
  },
  onTouchMove: function (e) {
    this.lock.onTouchMove(e);
  },
  onTouchEnd: function (e) {
    this.lock.onTouchEnd(e);
  },
  onTouchMove2: function(e) {
    
  }
})
