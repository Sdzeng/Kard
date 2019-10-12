//index.js
//获取应用实例
var util = require('../../utils/util.js');
var app = getApp()
var wxviewType = require('../../lib/wxview.js');


var touchData = {
  init: function() {
    this.firstTouchX = 0;
    this.firstTouchY = 0;
    this.lastTouchX = 0;
    this.lastTouchY = 0;
    this.lastTouchTime = 0;
    this.swipeDirection = 0;
    this.deltaX = 0;
    this.deltaY = 0;
    this.totalDelateX = 0;
    this.speedY = 0;
  },
  touchstart: function(e) {
    this.init();
    this.firstTouchX = this.lastTouchX = e.touches[0].clientX;
    this.firstTouchY = this.lastTouchY = e.touches[0].clientY;
    this.lastTouchTime = e.timeStamp;
  },
  touchmove: function(e) {
    this.deltaX = e.touches[0].clientX - this.lastTouchX;
    this.deltaY = e.touches[0].clientY - this.lastTouchY;
    this.totalDelateX += this.deltaX;
    this.lastTouchX = e.touches[0].clientX;
    this.lastTouchY = e.touches[0].clientY;
    this.lastTouchTime = e.timeStamp;
    if (this.swipeDirection === 0) {
      if (Math.abs(this.deltaX) > Math.abs(this.deltaY)) {
        this.swipeDirection = 1;
      }
      else {
        this.swipeDirection = 2;
      }
    }
  },
  touchend: function(e) {
    var deltaTime = e.timeStamp - this.lastTouchTime;
    this.speedY = this.deltaY / deltaTime;
  }
}

Page({
  data: {
    msgList:[],
    height:0,
  },
  swipeCheckX:35, //激活检测滑动的阈值
  swipeCheckState:0, //0未激活 1激活
  maxMoveLeft:185, //消息列表项最大左滑距离
  correctMoveLeft:175, //显示菜单时的左滑距离
  thresholdMoveLeft: 75,//左滑阈值，超过则显示菜单
  lastShowMsgId:'', //记录上次显示菜单的消息id
  moveX:0,  //记录平移距离
  showState:0, //0 未显示菜单 1显示菜单
  touchStartState:0, // 开始触摸时的状态 0 未显示菜单 1 显示菜单
  render: function() {
    this.setData(this.renderData);
    this.renderData = {};
  },
  getRenderData: function() {
    return this.renderData;
  },
  onLoad: function() {
    this.renderData = {};

    this.msgListView = wxviewType.createWXView();
    this.msgListView.setAnimationParam('msgListAnimation');
    this.msgListView.page = this;

    var windowWidth = app.data.deviceInfo.windowWidth; 
    var windowHeight = app.data.deviceInfo.windowHeight;
    var height = 0;
    for (var i = 0; i < 100; i++) {
      var msg = {};
      msg.userName = '' + '用户' + i+1;
      msg.msgText = '您有新的消息'
      msg.id = 'id-' + i+1;
      msg.headerImg = '../../res/img/head.png';
      this.data.msgList.push(msg);
      height += util.rpx2px(150, windowWidth);
    }
    this.msgListView.setWH(windowWidth, windowHeight);
    console.log(windowHeight);
    console.log(height);
    console.log(Math.min(0, windowHeight - height));
    this.msgListView.setBound(Math.min(0, windowHeight - height), 0);
    this.setData({msgList:this.data.msgList});
  },

  ontouchstart: function(e) {
    this.msgListView.ontouchstart(e);
    touchData.touchstart(e);
    if (this.showState === 1) {
      this.touchStartState = 1;
      this.showState = 0;
      this.moveX = 0;
      this.translateXMsgItem(this.lastShowMsgId, 0, 200);
      this.lastShowMsgId = "";
      return;
    }
    if (touchData.firstTouchX > this.swipeCheckX) {
      this.swipeCheckState = 1;
    }
  },

  ontouchmove: function(e) {
    touchData.touchmove(e);
    if (this.swipeCheckState === 0) {
      return;
    }
    //当开始触摸时有菜单显示时，不处理滑动操作
    if (this.touchStartState === 1) {
      return;
    }
    //滑动container，只处理垂直方向
    if (e.target.id === 'id-container') {
      this.msgListView.ontouchmove(e, touchData.deltaY);
      return;
    }
    //已触发垂直滑动
    if (touchData.swipeDirection === 2) {
      this.msgListView.ontouchmove(e, touchData.deltaY);
      return;
    }
    var moveX = touchData.totalDelateX;
    //处理边界情况
    if (moveX > 0) {
      moveX = 0;
    }
    //检测最大左滑距离
    if (moveX < -this.maxMoveLeft) {
      moveX = -this.maxMoveLeft;
    }
    this.moveX = moveX;
    this.translateXMsgItem(e.target.id, moveX, 0);
  },
  ontouchend: function(e) {
    touchData.touchend(e);
    this.swipeCheckState = 0;
    if (this.touchStartState === 1) {
      this.touchStartState = 0;
      return;
    } 
    //滑动container，只处理垂直方向
    if (e.target.id === 'id-container') {
      this.msgListView.ontouchend(e, touchData.speedY);
      return;
    }
    //垂直滚动
    if (touchData.swipeDirection === 2) {
      this.msgListView.ontouchend(e, touchData.speedY);
      return;
    }
    if (this.moveX === 0) {
      this.showState = 0;
      return;
    }
    if (this.moveX === this.correctMoveLeft) {
      this.showState = 1;
      this.lastShowMsgId = e.target.id;
      return;
    }  
    if (this.moveX < -this.thresholdMoveLeft) {
      this.moveX = -this.correctMoveLeft;
      this.showState = 1;
      this.lastShowMsgId = e.target.id;
    }
    else {
      this.moveX = 0;
      this.showState = 0;
    }
    this.translateXMsgItem(e.target.id, this.moveX, 200);
  },
  getItemIndex: function(id) {
    var msgList = this.data.msgList;
    for (var i = 0; i < msgList.length; i++) {
      if (msgList[i].id === id) {
        return i;
      }
    }
    return -1;
  },
  translateXMsgItem: function(id, x, duration) {
    var animation = wx.createAnimation({duration:duration});
    animation.translateX(x).step();
    this.animationMsgItem(id, animation);
  },
  animationMsgItem: function(id, animation) {
    var index = this.getItemIndex(id);
    var param = {};
    var indexString = 'msgList[' + index + '].animation';
    param[indexString] = animation.export();
    this.setData(param);
  },
})
