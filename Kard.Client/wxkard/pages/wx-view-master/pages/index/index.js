//index.js
//获取应用实例
var util = require('../../utils/util');
var wxView = require('wxview.js');
var wxTouch = require('wxtouch.js');
var app = getApp()
Page({
  data: {
    wxViewAnimation:{},
    itemList:[],
  },

  initTouchHandler: function() {
    var s = this;
    wxTouch.init(this);
    this.viewTouchHandler = wxTouch.createHandler
    ('id-wx-view', 'touch', function(e, input) {
      console.log(input);
      if (wxTouch.istouchstart(input)) {
        s.testView.ontouchstart();
      }
      else if (wxTouch.istouchmove(input)) {
        s.testView.ontouchmove(input.deltaY);
      }
      else if (wxTouch.istouchend(input)) {
        s.testView.ontouchend(input.speedY);
      }
    })
  },
 
  onLoad: function () {
    this.initTouchHandler();
    this.deviceInfo = app.appData.deviceInfo;
    var windowWidth = this.deviceInfo.windowWidth;
    var windowHeight = this.deviceInfo.windowHeight;
    var viewHeight = 0;
    for (var i = 0; i < 30; i++) {
      var item = {};
      item.text = '' + '您有新的消息' + i+1;
      item.height = util.rpx2px(501, windowWidth);
      viewHeight += item.height;
      this.data.itemList.push(item);
    }

    var testView = this.testView = wxView.createWXView();
    testView.setWH(windowWidth, viewHeight);
    var upBound = windowHeight - viewHeight; 
    if (upBound > 0) {
      upBound = 0;
    }
    testView.setBound(upBound, 0);
    var s = this;
    testView.setRender(function(y) {
       var animation = wx.createAnimation({duration:0});
       animation.translate3d(0, y, 0).step();
       s.setData({wxViewAnimation:animation.export()});
    })
    this.setData({itemList:this.data.itemList});
  },

  ontouchstart: function(e) {
    this.firstTouchX = e.touches[0].clientX;
    this.firstTouchY = e.touches[0].clientY;
    this.lastTouchY = e.touches[0].clientY;
    this.lTimeStamp = e.timeStamp;
    this.testView.ontouchstart();
  },

  ontouchmove: function(e) {
    this.dy = e.touches[0].clientY - this.lastTouchY;
    this.lastTouchY = e.touches[0].clientY;
    this.lTimeStamp = e.timeStamp;
    this.testView.ontouchmove(this.dy);
  },

  ontouchend: function(e) {
    var data = this.viewScrollData; 
    var dt = e.timeStamp - this.lTimeStamp;
    var speed = this.dy / dt;
    this.testView.ontouchend(speed);
  }
})
