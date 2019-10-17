//index.js
//获取应用实例
const app = getApp()

Page({
  data: {

  },
  //事件处理函数
  bindViewTap: function() {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  onLoad: function() {

  },
  chooseImage(e) {
    const that = this
    const handle = e.currentTarget.dataset.handle
    wx.chooseImage({
      count: 1,
      sizeType: ['original', 'compressed'],
      sourceType: ['album', 'camera'],
      success(res) {
        // tempFilePath可以作为img标签的src属性显示图片
        const imagePath = res.tempFilePaths

        switch (handle) {
          case "clothes":
            that.data.clothesImage = imagePath
            break;
          case "bottoms":
            that.data.bottomsImage = imagePath
            break;
          case "accessories":
            that.data.accessoriesImage = imagePath
            break;
          case "socks":
            that.data.socksImage = imagePath
            break;
          case "shoe":
            that.data.shoeImage = imagePath
            break;
        }

        that.setData(that.data);
      }
    })

  },
  saveMatch(e) {
    const that = this

    wx.showLoading({
      title: '正在保存...',
      mask: true
    })

    // var images = [that.data.clothesImage, that.data.bottomsImage, that.data.accessoriesImage, that.data.socksImage, that.data.shoeImage];
    console.log(that.data.clothesImage);
    var images = [that.data.clothesImage];
    // 将选择的图片组成一个Promise数组，准备进行并行上传
    const arr = images.map(path => {
      return wx.uploadFile({
        url: app.globalData.apiHost + '/common/uploadfile',
        header: {
          'cookie': wx.getStorageSync("cookie")
        },
        filePath: path,
        name: 'file',
      })
    })
    console.log(arr);
    // 开始并行上传图片
    Promise.all(arr).then(res => {
      // 上传成功，获取这些图片在服务器上的地址，组成一个数组
      return res.map(item => JSON.parse(item.data).url)
    }).catch(err => {
      console.log(">>>> upload images error:", err)
    }).then(urls => {
      // 调用保存问题的后端接口
      return createQuestion({
        title: title,
        content: content,
        images: urls
      })
    }).then(res => {
      // // 保存问题成功，返回上一页（通常是一个问题列表页）
      // const pages = getCurrentPages();
      // const currPage = pages[pages.length - 1];
      // const prevPage = pages[pages.length - 2];

      // // 将新创建的问题，添加到前一页（问题列表页）第一行
      // prevPage.data.questions.unshift(res)
      // $digest(prevPage)

      // wx.navigateBack()
    }).catch(err => {
      console.log(">>>> create question error:", err)
    }).then(() => {
      wx.hideLoading()
    })

  }

})