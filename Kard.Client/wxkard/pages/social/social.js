//index.js
//获取应用实例
const app = getApp()

Page({
  data: {
    currentDate: "00", 
    currentMonth:"00",
    taskArray:[
      {info:"花花刚完成了游泳",date:"17:23"},
      {info: "李俊基陆续完成了健身156天", date: "12:21" },
      {info: "挨踢学习了微信小程序新技术", date: "09:00" },
      {info: "老真实现愿望换了眼镜", date: "19:32" },
      {info: "亦菲完成1329英语单词", date: "10:03" },
      { info: "李蒿楠刚看完《人类简史》", date: "12:03" },
      { info: "叶子考完了钢琴二级", date: "08:11" },
      { info: "迈敖刚看完《鲁迅杂文》", date: "10:03" },
      { info: "亦菲刚看完《人类简史》", date: "20:03" }
      
      ],
    motto: 'Hello World',
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo')
  },
  //事件处理函数
  bindViewTap: function () {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  onLoad: function () {
    const date = new Date();
    this.setData({ currentDate: date.getDate()});
    this.setData({ currentMonth: date.getMonth()+1 });

    if (app.globalData.userInfo) {
      this.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } else if (this.data.canIUse) {
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.userInfoReadyCallback = res => {
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    } else {
      // 在没有 open-type=getUserInfo 版本的兼容处理
      wx.getUserInfo({
        success: res => {
          app.globalData.userInfo = res.userInfo
          this.setData({
            userInfo: res.userInfo,
            hasUserInfo: true
          })
        }
      })
    }


  },
  getUserInfo: function (e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  }
})


