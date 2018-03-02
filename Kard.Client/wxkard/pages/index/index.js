'use strict'
//index.js
//获取应用实例
const app = getApp();

Page({
  data: {
    hasEmptyGrid: false,
    empytGrids:[],
    days:[],
    taskList: [
      { txt: "游泳", time:"07:00至09:00"},
      { txt: "买眼镜", time: "12:00至13:00" },
      { txt: "健身", time: "17:00至18:00" },
      { txt: "学习新技术", time: "20:00至21:45"},
      { txt: "看闲书", time: "21:50至22:00" }
    ],
    viewYear: "--",
    viewMonth: "--",
    pickerValue: [],
    pickerYearList:[],
    pickerMonthList: [],
    newPickerYear:0,
    newPickerMonth:0,
    showPicker: false,
    startX: 0, //开始坐标
    startY: 0,
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo')
  },
  //事件处理函数
  btnAdd: function (e) {
    this.data.taskList.push({ txt: e.detail.value, time: "待定"});
    //更新数据
    this.setData({
      taskList: this.data.taskList
    });

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
      viewYear: app.globalData.currentDate.year,
      viewMonth: app.globalData.currentDate.month,
      taskList: this.data.taskList
    });
    //日期选择器
    //initDatepicker();

    //拿取app的UserInfo
    if (app.globalData.userInfo) {
      this.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } else if (this.data.canIUse) {
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.userInfoReadyCallback = res => {
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    } else {
      // 在没有 open-type=getUserInfo 版本的兼容处理
      wx.getUserInfo({
        success: res => {
          app.globalData.userInfo = res.userInfo
          this.setData({
            userInfo: res.userInfo,
            hasUserInfo: true
          })
        }
      })
    }


  },
  // 日历
  getThisMonthDays(year, month) {
    return new Date(year, month, 0).getDate();
  },
  getFirstDayOfWeek(year, month) {
    return new Date(Date.UTC(year, month - 1, 1)).getDay();
  },
  calculateEmptyGrids(year, month) {
    const firstDayOfWeek = this.getFirstDayOfWeek(year, month);

    let empytGrids = [];
    if (firstDayOfWeek > 0) {
      for (let i = 0; i < firstDayOfWeek; i++) {
        empytGrids.push(i);
      }
      this.setData({
        hasEmptyGrid: true,
        empytGrids
      });
    } else {
      this.setData({
        hasEmptyGrid: false,
        empytGrids: []
      });
    }

    return firstDayOfWeek;
  },
  calculateDays(year, month) {
    var dayOfWeek=this.calculateEmptyGrids(year,month);

    let days = [];
    const thisMonthDays = this.getThisMonthDays(year, month);
    const isCurrentMonth = (year == app.globalData.currentDate.year )&&(month == app.globalData.currentDate.month);
    for (let i = 1; i <= thisMonthDays; i++ , dayOfWeek = (dayOfWeek+1)%7 ) {
      
      days.push({
        day: i,
        choosed: false,
        dayOfWeek: dayOfWeek,
        isToday: isCurrentMonth && (i == app.globalData.currentDate.day)
      });
    }

    this.setData({
      days
    });
  },
  handleCalendar(e) {
    const handle = e.currentTarget.dataset.handle;
    const viewYear = this.data.viewYear;
    const viewMonth = this.data.viewMonth;
    let newYear=0,
        newMonth=0;
    switch(handle) {
      case "prev":
        newMonth = viewMonth - 1;
        newYear = viewYear;
      if (newMonth < 1) {
        newYear = viewYear - 1;
        newMonth = 12;
      }
      break;
     
    case "next":
         newMonth = viewMonth + 1;
         newYear = viewYear;
      if (newMonth > 12) {
        newYear = viewYear + 1;
        newMonth = 1;
      }

      break;
    case "today":
        newMonth = app.globalData.currentDate.month;
        newYear = app.globalData.currentDate.year;
        break;
    }

    this.calculateDays(newYear, newMonth);
    
    this.setData({
      viewYear: newYear,
      viewMonth: newMonth
    });
  },
  tapDayItem(e) {
    const idx = e.currentTarget.dataset.idx;
    const days = this.data.days;
    days[idx].choosed = !days[idx].choosed;
    this.setData({
      days,
    });
  },
  chooseYearAndMonth() {
 
    let pickerYearList = [];
    let pickerMonthList = [];
    for (let i = 1900; i <= 2100; i++) {
      pickerYearList.push(i);
    }
    for (let i = 1; i <= 12; i++) {
      pickerMonthList.push(i);
    }
    const idxYear = pickerYearList.indexOf(this.data.viewYear);
    const idxMonth = pickerMonthList.indexOf(this.data.viewMonth);
    this.setData({
      pickerValue: [idxYear, idxMonth],
      pickerYearList,
      pickerMonthList,
      showPicker: true,
    });
  },
  pickerChange(e) {
    const val = e.detail.value;
    const newPickerYear = this.data.pickerYearList[val[0]];
    const newPickerMonth = this.data.pickerMonthList[val[1]];
    this.setData({
      newPickerYear,
      newPickerMonth
    });
  },
  tapPickerBtn(e) {
    const type = e.currentTarget.dataset.type;
    const o = {
      showPicker: false,
    };
    if (type === 'confirm') {
      o.viewYear = this.data.newPickerYear;
      o.viewMonth = this.data.newPickerMonth;
       
      this.calculateDays(this.data.newPickerYear, this.data.newPickerMonth);
    }

    this.setData(o);
  },
 //用户
  getUserInfo: function (e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  },

  //任务列表
  //手指触摸动作开始 记录起点X坐标
  taskInfoTouchStart: function (e) {
    //开始触摸时 重置所有删除
    this.data.taskList.forEach(function (v, i) {
      if (v.isTouchMove)//只操作为true的
        v.isTouchMove = false;
    })
    this.setData({
      startX: e.changedTouches[0].clientX,
      startY: e.changedTouches[0].clientY,
      taskList: this.data.taskList
    })
  },
  //滑动事件处理
  taskInfoTouchMove: function (e) {
    var that = this,
      index = e.currentTarget.dataset.index,//当前索引
      startX = that.data.startX,//开始X坐标
      startY = that.data.startY,//开始Y坐标
      touchMoveX = e.changedTouches[0].clientX,//滑动变化坐标
      touchMoveY = e.changedTouches[0].clientY,//滑动变化坐标
      //获取滑动角度
      angle = that.angle({ X: startX, Y: startY }, { X: touchMoveX, Y: touchMoveY });
    that.data.taskList.forEach(function (v, i) {
      v.isTouchMove = false
      //滑动超过30度角 return
      if (Math.abs(angle) > 30) return;
      if (i == index) {
        if (touchMoveX > startX) //右滑
          v.isTouchMove = false
        else //左滑
          v.isTouchMove = true
      }
    })
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
  //删除事件
  btnDelete: function (e) {
    this.data.taskList.splice(e.currentTarget.dataset.index, 1)
    this.setData({
      taskList: this.data.taskList
    })
  }

})






