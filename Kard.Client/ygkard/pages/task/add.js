var util = require('../../utils/util.js')
const app = getApp();

Page({
  data: {
    isLongTerm: false,
    weekList: [
      { text: '日', choosed:false  },
      { text: '一', choosed: true },
      { text: '二', choosed: true },
      { text: '三', choosed: true },
      { text: '四', choosed: true },
      { text: '五', choosed: true },
      { text: '六', choosed: false }
    ],
    showDateDialog: false,
    showTimeDialog: false,
    startDate: app.globalData.currentDate,
    endDate: { year: app.globalData.currentDate.year, month: app.globalData.currentDate.month, day: app.globalData.currentDate.day + 1 }
  },
  onLoad: function () {
    const that = this;
    const date = new Date();
    //不需要更新到前台，只修改后端的data
    // that.data.startTime = { hour: date.getHours,minute:date.getMinutes};
    // that.data.endTime = { hour: date.getHours+1, minute:date.getMinutes };
    //更新到前台，并修改后端的data
    this.setData({
      startTime: { hour: date.getHours(), minute: date.getMinutes() },
      endTime: { hour: date.getHours() + 1, minute: date.getMinutes() }
    });
    console.log(JSON.stringify(that.data));
  },
  switchIsLongTerm: function (e) {

    this.setData({
      isLongTerm: e.detail.value
    });
  },
  dateDialogData: function (seedDate) {

    let seedYearList = [];
    let seedMonthList = [];
    let seedDayList = [];
    const thisMonthDays = util.getThisMonthDays(seedDate.year, seedDate.month);
    for (let i = 1900; i <= 2200; i++) {
      seedYearList.push(i);
    }
    for (let i = 1; i <= 12; i++) {
      seedMonthList.push(i);
    }
    for (let i = 1; i <= thisMonthDays; i++) {
      seedDayList.push(i);
    }
    let seedData = {
      seedYearList: seedYearList,
      seedMonthList: seedMonthList,
      seedDayList: seedDayList,
      idxYear: seedYearList.indexOf(seedDate.year),
      idxMonth: seedMonthList.indexOf(seedDate.month),
      idxDay: seedDayList.indexOf(seedDate.day)
    };
    return seedData;




  },
  dateDialog() {

    const that = this;

    let startDateData = that.dateDialogData(that.data.startDate);
    let dateData = {
      dateIndexs: [startDateData.idxYear, startDateData.idxMonth, startDateData.idxDay],
      startYearList: startDateData.seedYearList,
      startMonthList: startDateData.seedMonthList,
      startDayList: startDateData.seedDayList,
      showDateDialog: true

    }

    if (that.data.isLongTerm) {
      let endDateData = that.dateDialogData(that.data.endDate);
      dateData.dateIndexs = dateData.dateIndexs.concat(0, endDateData.idxYear, endDateData.idxMonth, endDateData.idxDay);
      dateData.endYearList = endDateData.seedYearList;
      dateData.endMonthList = endDateData.seedMonthList;
      dateData.endDayList = endDateData.seedDayList;
    }

    that.setData(dateData);
  },

  preventTouchMove:function() { },
  datePickerChange(e) {
    const that = this;
    const val = e.detail.value;
    let temporaryDate = {
      temporaryStartDate: {
        year: that.data.startYearList[val[0]],
        month: that.data.startMonthList[val[1]],
        day: that.data.startDayList[val[2]]
      }
    };
    if (that.data.isLongTerm) {
      temporaryDate.temporaryEndDate = {
        year: that.data.endYearList[val[4]],
        month: that.data.endMonthList[val[5]],
        day: that.data.endDayList[val[6]]
      };
    }
    that.setData(temporaryDate);
  },
  dateDialogBtn(e) {
    const that = this;
    const btnType = e.currentTarget.dataset.type;
    let dateDialogData = {
      showDateDialog: false,
    };

    if (btnType === 'confirm' && that.data.temporaryStartDate!=null) {
      dateDialogData.startDate = that.data.temporaryStartDate;
      if (that.data.isLongTerm && that.data.temporaryEndDate!=null) {
        dateDialogData.endDate = that.data.temporaryEndDate;
      }
    }

    that.setData(dateDialogData);
  },
  timeDialogData: function (seedTime) {

    let seedHourList = [];
    let seedMinuteList = [];

    for (let i = 0; i < 24; i++) {
      seedHourList.push(i);
    }
    for (let i = 0; i < 60; i++) {
      seedMinuteList.push(i);
    }

    let seedData = {
      seedHourList: seedHourList,
      seedMinuteList: seedMinuteList,
      idxHour: seedHourList.indexOf(seedTime.hour),
      idxMinute: seedMinuteList.indexOf(seedTime.minute)
    };
    return seedData;

  },
  timeDialog() {
    const that = this;
    let startTimeData = that.timeDialogData(that.data.startTime);
    let endTimeData = that.timeDialogData(that.data.endTime);
    let timeData = {
      timeIndexs: [startTimeData.idxHour, startTimeData.idxMinute, 0, endTimeData.idxHour, endTimeData.idxMinute],
      startHourList: startTimeData.seedHourList,
      startMinuteList: startTimeData.seedMinuteList,
      endHourList: endTimeData.seedHourList,
      endMinuteList: endTimeData.seedMinuteList,
      showTimeDialog: true
    }

    that.setData(timeData);
  },


  timePickerChange(e) {
    const that = this;
    const val = e.detail.value;
    console.log("timePickerChange"+val)
    let temporaryTime = {
      temporaryStartTime: {
        hour: that.data.startHourList[val[0]],
        minute: that.data.startMinuteList[val[1]],
      },
      temporaryEndTime: {
        hour: that.data.endHourList[val[3]],
        minute: that.data.endMinuteList[val[4]]
      }
    };

    that.setData(temporaryTime);
  },
  timeDialogBtn(e) {
    const that = this;
    const btnType = e.currentTarget.dataset.type;
    let dateDialogData = {
      showTimeDialog: false
    };

    if (btnType === 'confirm' && that.data.temporaryStartTime != null && that.data.temporaryEndTime != null) {
      dateDialogData.startTime = that.data.temporaryStartTime;
      dateDialogData.endTime = that.data.temporaryEndTime;
    }

    that.setData(dateDialogData);
  },
  tapWeekItem(e) {
    const idx = e.currentTarget.dataset.idx;
    const weekList = this.data.weekList;
    weekList[idx].choosed = !weekList[idx].choosed;
    this.setData({
      weekList
    });
  },
 
  formSubmit: function (e) {
    const that = this;
   
    var data = e.detail.value || {}
    data.startTime = [that.data.startTime.hour,that.data.startTime.minute,"00"].join(':');
    data.endTime = [that.data.endTime.hour,that.data.endTime.minute,"00"].join(':');
    // data.startTime = '1900/01/01 ' + [that.data.startTime.hour, that.data.startTime.minute, "00"].join(':')
    // data.endTime = '1900/01/01 ' + [that.data.endTime.hour, that.data.endTime.minute, "00"].join(':')
    if (that.data.isLongTerm) {
      data.startDate = [that.data.startDate.year, that.data.startDate.month, that.data.startDate.day].join('/')
      data.endDate = [that.data.endDate.year, that.data.endDate.month, that.data.endDate.day].join('/')
      let week = []
      that.data.weekList.forEach(function (item, index) {
        if (item.choosed) {
          week.push(index)
        }
      });
      if (week.length > 0) {
        data.week = week.join(',')
      }
    }
    else{
      data.taskDate = [that.data.startDate.year, that.data.startDate.month, that.data.startDate.day].join('/')
    }

    //发起网络请求
    wx.request({
      url: app.globalData.apiHost+'/wx/addtask',
      method: 'POST',
      data: JSON.stringify(data),
      header: {
        'cookie': wx.getStorageSync("cookie") // 默认值
      },
      success: function (res) {

        if (res.data.result) {

          console.log('添加成功')
          wx.navigateBack({
            delta: 1
          })
        }
        else {

          wx.showToast({
            title: '添加失败:' + res.data.message,
            icon: 'none',
            duration: 2000
          })
        }
      },
      fail: function () {

        wx.showToast({
          title: '添加失败',
          icon: 'none',
          duration: 2000
        })
      }
    })
  },
  formReset: function () {
    wx.navigateBack({
      delta: 1
    })
  }
})