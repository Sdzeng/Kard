//index.js
//获取应用实例
// const app = getApp()

// Page({
//   data: {
//     motto: 'Hello World',
//     userInfo: {},
//     hasUserInfo: false,
//     canIUse: wx.canIUse('button.open-type.getUserInfo')
//   },
//   //事件处理函数
//   bindViewTap: function() {
//     wx.navigateTo({
//       url: '../logs/logs'
//     })
//   },
//   onLoad: function () {
//     if (app.globalData.userInfo) {
//       this.setData({
//         userInfo: app.globalData.userInfo,
//         hasUserInfo: true
//       })
//     } else if (this.data.canIUse){
//       // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
//       // 所以此处加入 callback 以防止这种情况
//       app.userInfoReadyCallback = res => {
//         this.setData({
//           userInfo: res.userInfo,
//           hasUserInfo: true
//         })
//       }
//     } else {
//       // 在没有 open-type=getUserInfo 版本的兼容处理
//       wx.getUserInfo({
//         success: res => {
//           app.globalData.userInfo = res.userInfo
//           this.setData({
//             userInfo: res.userInfo,
//             hasUserInfo: true
//           })
//         }
//       })
//     }
//   },
//   getUserInfo: function(e) {
//     console.log(e)
//     app.globalData.userInfo = e.detail.userInfo
//     this.setData({
//       userInfo: e.detail.userInfo,
//       hasUserInfo: true
//     })
//   }
// })


var touchDot = 0;//触摸时的原点
var time = 0;// 时间记录，用于滑动时且时间小于1s则执行左右滑动
var interval = "";// 记录/清理 时间记录
var nth = 0;// 设置活动菜单的index
var nthMax = 5;//活动菜单的最大个数
var tmpFlag = true;// 判断左右华东超出菜单最大值时不再执行滑动事件

// 触摸开始事件
Page({
  touchStart: function(e) {
  touchDot = e.touches[0].pageX; // 获取触摸时的原点
  // 使用js计时器记录时间  
  interval = setInterval(function () {
    time++;
  }, 100);
},
// 触摸移动事件
touchMove: function(e) {
  var touchMove = e.touches[0].pageX;
  console.log("touchMove:" + touchMove + " touchDot:" + touchDot + " diff:" + (touchMove - touchDot));
  // 向左滑动  
  if (touchMove - touchDot <= -40 && time < 10) {
    if (tmpFlag && nth < nthMax) { //每次移动中且滑动时不超过最大值 只执行一次
      var tmp = this.data.menu.map(function (arr, index) {
        tmpFlag = false;
        if (arr.active) { // 当前的状态更改
          nth = index;
          ++nth;
          arr.active = nth > nthMax ? true : false;
        }
        if (nth == index) { // 下一个的状态更改
          arr.active = true;
          name = arr.value;
        }
        return arr;
      })
      this.getNews(name); // 获取新闻列表
      this.setData({ menu: tmp }); // 更新菜单
    }
  }
  // 向右滑动
  if (touchMove - touchDot >= 40 && time < 10) {
    if (tmpFlag && nth > 0) {
      nth = --nth < 0 ? 0 : nth;
      var tmp = this.data.menu.map(function (arr, index) {
        tmpFlag = false;
        arr.active = false;
        // 上一个的状态更改
        if (nth == index) {
          arr.active = true;
          name = arr.value;
        }
        return arr;
      })
      this.getNews(name); // 获取新闻列表
      this.setData({ menu: tmp }); // 更新菜单
    }
  }
  // touchDot = touchMove; //每移动一次把上一次的点作为原点（好像没啥用）
},
// 触摸结束事件
touchEnd: function(e) {
  clearInterval(interval); // 清除setInterval
  time = 0;
  tmpFlag = true; // 回复滑动事件
}
})