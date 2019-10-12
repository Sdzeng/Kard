// pages/news/news.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    chooseImage:true,
    cameraPath:"/images/camera.png"
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    const that = this
    that.setData(that.data);
  },
  chooseImage(e) {
    const that = this
    
    wx.chooseImage({
      count: 1,
      sizeType: ['original', 'compressed'],
      sourceType: ['album', 'camera'],
      success(res) {
        // tempFilePath可以作为img标签的src属性显示图片
        const imagePath = res.tempFilePaths

        that.data.chooseImage=false
        that.data.image = imagePath

        that.setData(that.data);
      }
    })

  },
})