// Page({
//   onLoad: function (options) {
//     var duanziId = options.operation;
//     this.setData({ duanziId});
//     var that = this;
//     wx.showModal({
//       title: '提示',
//       content: '这是一个模态弹窗',
//       success: function (res) {
//         if (res.confirm) {
//           console.log('用户点击确定')

//           wx.showActionSheet({
//             itemList: ['A', 'B', 'C'],
//             success: function (res) {
//               console.log(res.tapIndex)
//             },
//             fail: function (res) {
//               console.log(res.errMsg)
//             }
//           })


//         } else if (res.cancel) {
//           console.log('用户点击取消')
//         }
//       }
//     })
 
//   } 

// })


Page({
  formSubmit: function (e) {
    console.log('form发生了submit事件，携带数据为：', e.detail.value)
  },
  formReset: function () {
    console.log('form发生了reset事件')
  }
})