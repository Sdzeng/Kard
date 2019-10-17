
var util = require('../../utils/util.js')
'use strict'
//index.js
//获取应用实例
const app = getApp();

Page({
  data: {
    hasEmptyGrid: false,
    empytGrids: [],
    yearMonthGrids: {},
    newYearMonthGrids: null,
    dayGrids: [],

    taskList: [],
    pickerValue: [],
    pickerYearList: [],
    pickerMonthList: [],
    // newPickerYear: 0,
    // newPickerMonth: 0,
    showPicker: false,
    startX: 0, //开始坐标
    startY: 0,
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo')
  },

  onLoad: function () {
   
  },
  onShow:function(){

    //渲染日历格子
    this.calculateDays(app.globalData.currentDate.year, app.globalData.currentDate.month);
    //渲染任务列表
    this.getTask();
  },
  onPullDownRefresh: function () {
    const that=this;
    //wx.showNavigationBarLoading();

    that.getTask(() => {
      //wx.hideNavigationBarLoading(); //完成停止加载
      wx.stopPullDownRefresh();//停止下拉刷新
    });



  },
  getTask: function (callback) {
    const that = this;

    wx.request({
      url: app.globalData.apiHost + '/wx/gettask',
      header: {
        'cookie': wx.getStorageSync("cookie")
      },
      success: function (res) {

        if (res.data.result) {
          //更新数据
          res.data.data.forEach(function (item, i) {
            item.isTouchMove = false;
          });
          that.setData({
            taskList: res.data.data
          });
        }
      },
      fail: function () {
        console.log('获取任务列表失败')
      },
      complete: function () {
        if (callback)
        {
          callback();
        }

      }


    })

  },
  calculateEmpty(year, month) {
    const firstDayOfWeek = util.getFirstDayOfWeek(year, month);
    let emptyCount = firstDayOfWeek == 0 ? 7 : firstDayOfWeek;
    emptyCount = emptyCount >= 1 ? (emptyCount - 1) : (7 - emptyCount);
    
    let empytGrids = [];

    if (emptyCount > 0) {
      for (let i = 0; i < emptyCount; i++) {
        empytGrids.push(i);
      }

    }

    this.setData({
      hasEmptyGrid: emptyCount > 0,
      empytGrids

    });

    return firstDayOfWeek;
  },

  calculateDays(year, month) {
    var firstDayOfWeek = this.calculateEmpty(year, month);
    let yearMonthGrids = { year: year, month: month };
    let dayGrids = [];
    const thisMonthDays = util.getThisMonthDays(year, month);
    const isCurrentMonth = (year == app.globalData.currentDate.year) && (month == app.globalData.currentDate.month);
    for (let i = 1; i <= thisMonthDays; i++ , firstDayOfWeek = (firstDayOfWeek + 1) % 7) {

      dayGrids.push({
        day: i,
        choosed: false,
        dayOfWeek: firstDayOfWeek,
        isToday: isCurrentMonth && (i == app.globalData.currentDate.day)
      });
    }

    this.setData({
      yearMonthGrids,
      dayGrids

    });

     
  },
  handleCalendar(e) {
    const handle = e.currentTarget.dataset.handle;
    const yearMonthGrids = this.data.yearMonthGrids;
    let newYear = 0,
      newMonth = 0;
    switch (handle) {
      case "prev":
        newMonth = yearMonthGrids.month - 1;
        newYear = yearMonthGrids.year;
        if (newMonth < 1) {
          newYear = yearMonthGrids.year - 1;
          newMonth = 12;
        }
        break;

      case "next":
        newMonth = yearMonthGrids.month + 1;
        newYear = yearMonthGrids.year;
        if (newMonth > 12) {
          newYear = yearMonthGrids.year + 1;
          newMonth = 1;
        }

        break;

    }

    this.calculateDays(newYear, newMonth);

  },
  tapDayItem(e) {
    const idx = e.currentTarget.dataset.idx;
    const dayGrids = this.data.dayGrids;
    dayGrids[idx].choosed = !dayGrids[idx].choosed;
    this.setData({
      dayGrids
    });
  },
  chooseYearAndMonth() {

    let pickerYearList = [];
    let pickerMonthList = [];
    for (let i = 1900; i <= 2200; i++) {
      pickerYearList.push(i);
    }
    for (let i = 1; i <= 12; i++) {
      pickerMonthList.push(i);
    }
    const idxYear = pickerYearList.indexOf(this.data.yearMonthGrids.year);
    const idxMonth = pickerMonthList.indexOf(this.data.yearMonthGrids.month);
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
    const btnType = e.currentTarget.dataset.type;
    if (btnType === 'confirm' && this.data.newYearMonthGrids != null) {
      this.calculateDays(this.data.newYearMonthGrids.year, this.data.newYearMonthGrids.month);
    }
    else if (btnType === 'today') {
      this.calculateDays(app.globalData.currentDate.year, app.globalData.currentDate.month);
    }

    this.setData({
      showPicker: false,
    });
  },
  // //用户
  // getUserInfo: function (e) {
  //   console.log(e)
  //   app.globalData.userInfo = e.detail.userInfo
  //   this.setData({
  //     userInfo: e.detail.userInfo,
  //     hasUserInfo: true
  //   })
  // },

  //任务列表
  //手指触摸动作开始 记录起点X坐标
  taskInfoTouchStart: function (e) {
    //开始触摸时 重置所有删除
    var param = {
      startX: e.changedTouches[0].clientX,
      startY: e.changedTouches[0].clientY
    };

    this.data.taskList.forEach(function (v, i) {
      if (v.isTouchMove)//只操作为true的
      {
        // v.isTouchMove = false;
        param['taskList[' + i + '].isTouchMove'] = false;
      }

    })
    this.setData(param);
  },
  //滑动事件处理
  taskInfoTouchMove: function (e) {

    var that = this,
      id = e.currentTarget.id,//当前索引
      startX = that.data.startX,//开始X坐标
      startY = that.data.startY,//开始Y坐标
      touchMoveX = e.changedTouches[0].clientX,//滑动变化坐标
      touchMoveY = e.changedTouches[0].clientY,//滑动变化坐标
      //获取滑动角度
      angle = that.angle({ X: startX, Y: startY }, { X: touchMoveX, Y: touchMoveY });

    //滑动超过30度角 return
    if (Math.abs(angle) <= 30) {

      that.data.taskList.forEach(function (v, i) {
        v.isTouchMove = false

        if (v.id == id) {
          if (touchMoveX > startX) //右滑
            v.isTouchMove = false
          else //左滑
            v.isTouchMove = true
        }
      })
    }
    //更新数据
    that.setData({

      taskList: that.data.taskList
    })
  },
  /**
   * 计算滑动角度
   * @param {Object} start 起点坐标
   * @param {Object} end 终点坐标
   */
  angle: function (start, end) {
    var _X = end.X - start.X,
      _Y = end.Y - start.Y;
    //返回角度 /Math.atan()返回数字的反正切值
    return 360 * Math.atan(_Y / _X) / (2 * Math.PI);
  },
  //事件处理函数
  btnAdd: function (e) {

    wx.navigateTo({
      url: '../task/add'
    })

  },
  btnEdit: function (e) {

    console.log("id" + e.currentTarget.id);
    wx.navigateTo({
      url: '../task/edit?id=' + e.currentTarget.id
    })

  },
  btnDelete: function (e) {
    this.data.taskList.splice(e.currentTarget.dataset.index, 1)
    this.setData({
      taskList: this.data.taskList
    })
  },

  setting: function () {

    wx.openSetting({
      success: (res) => { }
    })
  }

})






