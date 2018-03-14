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
  data:{
    isLongTerm:false,
    weekList:['日', '一', '二', '三', '四', '五', '六']

  },
  switchWeekList:function(e){
    this.data.isLongTerm = e.detail.value;
    this.setData({
      isLongTerm: this.data.isLongTerm
    });
  },
  chooseTaskStartDate:function(e){
  //   <picker-view class="calender_info_choose_yearmonths_picker_view" indicator- style="height: 50px;" value= "{{pickerValue}}" bindchange= "pickerChange" >
  //     <picker-view - column >
  //     <view class="calender_info_choose_yearmonths_picker_view_cols" wx: for="{{pickerYearList}}" wx:key = "*this" > {{item }
  // }年</view >
  // </picker-view-column>
  // < picker - view - column >
  // <view class="calender_info_choose_yearmonths_picker_view_cols" wx:for="{{pickerMonthList}}" wx:key = "*this" > {{item }}月 < /view>
  //   < /picker-view-column>
  //   < /picker-view>

    wx.showModal({
      title: '开始日期',
      content: '&lt;view&gt;ggggg&lt;/view&gt;',
      success: function (res) {
        if (res.confirm) {
          console.log('用户点击确定')
        } else if (res.cancel) {
          console.log('用户点击取消')
        }
      }
    })


  },
  formSubmit: function (e) {
    console.log('form发生了submit事件，携带数据为：', e.detail.value)
  },
  formReset: function () {
    wx.navigateBack({
      delta: 1
    })
  } 
})