let chooseYear = null;
let chooseMonth = null;
const conf = {
  // getThisMonthDays(year, month) {
  //   return new Date(year, month, 0).getDate();
  // },
  // getFirstDayOfWeek(year, month) {
  //   return new Date(Date.UTC(year, month - 1, 1)).getDay();
  // },
  // calculateEmptyGrids(year, month) {
  //   const firstDayOfWeek = conf.getFirstDayOfWeek(year, month);
  //   let empytGrids = [];
  //   if (firstDayOfWeek > 0) {
  //     for (let i = 0; i < firstDayOfWeek; i++) {
  //       empytGrids.push(i);
  //     }
  //     this.setData({
  //       'calendar.hasEmptyGrid': true,
  //       'calendar.empytGrids': empytGrids,
  //     });
  //   } else {
  //     this.setData({
  //       'calendar.hasEmptyGrid': false,
  //       'calendar.empytGrids': [],
  //     });
  //   }
  // },
  // calculateDays(year, month) {
  //   let days = [];

  //   const thisMonthDays = conf.getThisMonthDays(year, month);

  //   for (let i = 1; i <= thisMonthDays; i++) {
  //     days.push({
  //       day: i,
  //       choosed: false
  //     });
  //   }

  //   this.setData({
  //     'calendar.days': days,
  //   });
  // },
  // handleCalendar(e) {
  //   const handle = e.currentTarget.dataset.handle;
  //   const curYear = this.data.calendar.curYear;
  //   const curMonth = this.data.calendar.curMonth;
  //   if (handle === 'prev') {
  //     let newMonth = curMonth - 1;
  //     let newYear = curYear;
  //     if (newMonth < 1) {
  //       newYear = curYear - 1;
  //       newMonth = 12;
  //     }

  //     conf.calculateDays.call(this, newYear, newMonth);
  //     conf.calculateEmptyGrids.call(this, newYear, newMonth);

  //     this.setData({
  //       'calendar.curYear': newYear,
  //       'calendar.curMonth': newMonth,
  //     });
  //   } else {
  //     let newMonth = curMonth + 1;
  //     let newYear = curYear;
  //     if (newMonth > 12) {
  //       newYear = curYear + 1;
  //       newMonth = 1;
  //     }

  //     conf.calculateDays.call(this, newYear, newMonth);
  //     conf.calculateEmptyGrids.call(this, newYear, newMonth);

  //     this.setData({
  //       'calendar.curYear': newYear,
  //       'calendar.curMonth': newMonth
  //     });
  //   }
  // },
  // tapDayItem(e) {
  //   const idx = e.currentTarget.dataset.idx;
  //   const days = this.data.calendar.days;
  //   days[idx].choosed = !days[idx].choosed;
  //   this.setData({
  //     'calendar.days': days,
  //   });
  // },
  chooseYearAndMonth() {
    let pickerYear = [];
    let pickerMonth = [];
    for (let i = 1900; i <= 2100; i++) {
      pickerYear.push(i);
    }
    for (let i = 1; i <= 12; i++) {
      pickerMonth.push(i);
    }
    this.setData({
      'datepicker.showPicker': true,
    });
  },
  pickerChange(e) {
    const val = e.detail.value;
    chooseYear = this.data.pickerYear[val[0]];
    chooseMonth = this.data.pickerMonth[val[1]];
  },
  tapPickerBtn(e) {
    const type = e.currentTarget.dataset.type;
    const datepicker = {
      showPicker: false,
    };
    if (type === 'confirm') {
      datepicker.curYear = chooseYear;
      datepicker.curMonth = chooseMonth;
      // this.calculateEmptyGrids(chooseYear, chooseMonth);
      // this.calculateDays(chooseYear, chooseMonth);
    }

    this.setData({
      datepicker
      });
  }
};

function _getCurrentPage() {
  const pages = getCurrentPages();
  const last = pages.length - 1;
  return pages[last];
}

export default (e) => {
  const self = _getCurrentPage();

  self.setData({
    datepicker: {
      showPicker: false
    }
  });
  // conf.calculateEmptyGrids.call(self, curYear, curMonth);
  // conf.calculateDays.call(self, curYear, curMonth);
  // self.tapDayItem = conf.tapDayItem.bind(self);
  // self.handleCalendar = conf.handleCalendar.bind(self);
  self.chooseYearAndMonth = conf.chooseYearAndMonth.bind(self);
  self.pickerChange = conf.pickerChange.bind(self);
  self.tapPickerBtn = conf.tapPickerBtn.bind(self);
};
