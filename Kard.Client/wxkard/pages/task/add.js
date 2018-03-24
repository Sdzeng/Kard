const app = getApp();

Page({
  data: {
    isLongTerm: false,
    weekList: ['日', '一', '二', '三', '四', '五', '六']

  },
  switchIsLongTerm: function (e) {
    this.data.isLongTerm = e.detail.value;
    this.setData({
      isLongTerm: this.data.isLongTerm
    });
  },
  chooseTaskStartDate: function (e) {
    let pickerYearList = [];
    let pickerMonthList = [];
    for (let i = 1900; i <= 2200; i++) {
      pickerYearList.push(i);
    }
    for (let i = 1; i <= 12; i++) {
      pickerMonthList.push(i);
    }
    const idxYear = pickerYearList.indexOf(app.globalData.currentDate.year);
    const idxMonth = pickerMonthList.indexOf(app.globalData.currentDate.month);
    this.setData({
      pickerValue: [idxYear, idxMonth],
      pickerYearList,
      pickerMonthList,
      showPicker: true,
      newYearMonthGrids: null
    });



  },
  preventTouchMove() { },
  pickerChange(e) {
    const val = e.detail.value;
    let newYearMonthGrids = { year: this.data.pickerYearList[val[0]], month: this.data.pickerMonthList[val[1]] };
    this.setData({
      newYearMonthGrids
    });
  },
  tapPickerBtn(e) {
    const type = e.currentTarget.dataset.type;
    if (type === 'confirm' && this.data.newYearMonthGrids != null) {
      this.calculateDays(this.data.newYearMonthGrids.year, this.data.newYearMonthGrids.month);
    }

    this.setData({
      showPicker: false,
    });
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