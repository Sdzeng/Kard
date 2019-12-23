// pages/wardrobe/wardrobe.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    opacityScroll:0,
    imgUrls: [
      'http://www.coretn.cn/user/1/media/20191205/051208178429.jpg',
      'http://www.coretn.cn/user/1/media/20191205/051208178429.jpg',
      'http://www.coretn.cn/user/1/media/20191205/051208178429.jpg',
      'http://www.coretn.cn/user/1/media/20191216/162201390216.jpg',
      'http://www.coretn.cn/user/1/media/20191216/162220173392.jpg',
    ]
  },
  //滚动条监听
  scroll: function (e) {
    const that = this
    console.log("yes")
    // const scrollTop=e.detail.scrollTop
    // const opacityScroll= (scrollTop /100) > 1 ? 1 :(scrollTop /100)
    // that.setData({opacityScroll: e.detail.scrollTop})
  },
  //事件处理函数
  btnSearch: function(e){
    console.log(e);
    wx.navigateTo({
      url: '../../pages/news/news'
    })
  },
  onLoad: function () {

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