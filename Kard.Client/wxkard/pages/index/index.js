'use strict'
//index.js
//获取应用实例
const app = getApp();

Page({
  data: {
    hasEmptyGrid: false,
    empytGrids: [],
    yearMonthGrids:{},
    newYearMonthGrids:null,
    dayGrids: [],
    keepPage:false,
    taskList: [
      { id:"001", txt: "游泳", time: "07:00至09:00" },
      { id: "002",txt: "买眼镜", time: "12:00至13:00" },
      { id: "003",txt: "健身", time: "17:00至18:00" },
      { id: "004",txt: "学习新技术", time: "20:00至21:45" },
      { id: "005",txt: "整理", time: "21:50至22:00" },
      { id: "006",txt: "看闲书", time: "22:00至22:30" },
      { id: "007",txt: "玩手机", time: "22:30至22:45" }
    ],
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

  bindViewTap: function () {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  onLoad: function () {
    //渲染日历格子
    this.calculateDays(app.globalData.currentDate.year, app.globalData.currentDate.month);
    //渲染任务列表
    this.data.taskList.forEach(function (item, i) {
      item.isTouchMove = false;
    });
    //更新数据
    this.setData({
      taskList: this.data.taskList
    });
    //日期选择器
    //initDatepicker();

    // //拿取app的UserInfo
    // if (app.globalData.userInfo) {
    //   this.setData({
    //     userInfo: app.globalData.userInfo,
    //     hasUserInfo: true
    //   })
    // } else if (this.data.canIUse) {
    //   // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
    //   // 所以此处加入 callback 以防止这种情况
    //   app.userInfoReadyCallback = res => {
    //     this.setData({
    //       userInfo: res.userInfo,
    //       hasUserInfo: true
    //     })
    //   }
    // } else {
    //   // 在没有 open-type=getUserInfo 版本的兼容处理
    //   wx.getUserInfo({
    //     success: res => {
    //       app.globalData.userInfo = res.userInfo
    //       this.setData({
    //         userInfo: res.userInfo,
    //         hasUserInfo: true
    //       })
    //     }
    //   })
    // }


  },
  onPullDownRefresh: function () {
    wx.showNavigationBarLoading();

    wx.hideNavigationBarLoading(); //完成停止加载
    wx.stopPullDownRefresh(); //停止下拉刷新

  },
  // 日历
  getThisMonthDays(year, month) {
    return new Date(year, month, 0).getDate();
  },
  getFirstDayOfWeek(year, month) {
    return new Date(Date.UTC(year, month - 1, 1)).getDay();
  },
  calculateEmpty(year, month) {
    const firstDayOfWeek = this.getFirstDayOfWeek(year, month);
    let emptyCount = firstDayOfWeek == 0 ? 7 : firstDayOfWeek;
    emptyCount = emptyCount >= 1 ? (emptyCount - 1) : (7 - emptyCount);
    console.log(firstDayOfWeek);
    let empytGrids = [];
    
    if (emptyCount>0) {
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
    const thisMonthDays = this.getThisMonthDays(year, month);
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
      newYearMonthGrids:null
    });
  },
  preventTouchMove(){},
  pickerChange(e) {
    const val = e.detail.value;
    let newYearMonthGrids={year:this.data.pickerYearList[val[0]],month:this.data.pickerMonthList[val[1]]};
    this.setData({
      newYearMonthGrids
    });
  },
  tapPickerBtn(e) {
    const btnType = e.currentTarget.dataset.type;
    if (btnType === 'confirm' && this.data.newYearMonthGrids!=null) {
      this.calculateDays(this.data.newYearMonthGrids.year, this.data.newYearMonthGrids.month);
    }
    else if (btnType === 'today')
    {
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
    if (Math.abs(angle) > 30) {
      that.data.keepPage = false;
    }
    else {

      that.data.keepPage = true;


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
        keepPage:that.data.keepPage,
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
    // this.data.taskList.push({ txt: e.detail.value, time: "待定"});
    // //更新数据
    // this.setData({
    //   taskList: this.data.taskList
    // });

    wx.navigateTo({
      url: '../task/add'
    })

  },
  btnEdit: function (e) {
    // this.data.taskList.push({ txt: e.detail.value, time: "待定"});
    // //更新数据
    // this.setData({
    //   taskList: this.data.taskList
    // });
    console.log("id"+e.currentTarget.id);
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

  setting:function(){

    wx.openSetting({
      success:(res)=>{}
    })
  }

})






