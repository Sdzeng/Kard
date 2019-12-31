// pages/wardrobe/wardrobe.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    opacityScroll:0,
    imgUrls: [
      'https://hbimg.huabanimg.com/cb0b95e3b457f788000564e0c3d0f6890711e75d981001-VvlHQM_fw658',
      'http://www.coretn.cn/user/1/media/20191216/162201390216.jpg',
      'http://www.coretn.cn/user/1/media/20191205/051208178429.jpg',
      'http://www.coretn.cn/user/1/media/20191216/162201390216.jpg',
    ]
  },
  //事件处理函数
  btnSearch: function(e){
    console.log("btnSearch")
    wx.navigateTo({ url:"/pages/search/search"})

  },
  //监听屏幕滚动 判断上下滚动
  onPageScroll: function (ev) {
    var that = this;
    //当滚动的top值最大或最小时，为什么要做这一步是因为在手机实测小程序的时候会发生滚动条回弹，所以为了处理回弹，设置默认最大最小值
    if (ev.scrollTop <= 0) {
      ev.scrollTop = 0;
    } else if (ev.scrollTop > wx.getSystemInfoSync().windowHeight) {
      ev.scrollTop = wx.getSystemInfoSync().windowHeight;
    }
    const opacityScroll= (ev.scrollTop /200) > 1 ? 1 :(ev.scrollTop /200)
    that.setData({ opacityScroll: opacityScroll })
  },
  onGotUserInfo: function (e) {
    console.log(e.detail.errMsg)
    console.log(e.detail.userInfo)
    console.log(e.detail.rawData)
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  }
})