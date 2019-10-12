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
    hasUserInfo: false
    
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

    


  },
  onShow: function () {
    const that=this
    if (app.globalData.userInfo) {
      that.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } 
    else{
        app.auth(() => {
          app.getUserInfo(()=>{
            that.setData({
              userInfo: app.globalData.user.info,
              hasUserInfo: true
            })
          })
        })
    }
  }
})


