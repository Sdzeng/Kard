var basejs = {
    requestDomain: "http://api.localyc.com:443",//"http://192.168.10.3:3703"
    cdnDomain: "http://api.localyc.com:443",//"http://192.168.10.3:3703"
    defaults: {
        type: "POST",
        async: true,
        contentType: "application/x-www-form-urlencoded",//"application/json;charset=utf-8",
        traditional: false,
        processData: true,
        data: null,
        loading: "off",
        avatarPath:"/image/default-avatar.jpg"
    },
    getPath: function () {
        var hash = window.location.hash, reg = /^#!/;
        if (reg.test(hash)) {

            return hash.replace(reg, '');
        } else {
            return hash;//storage.local.getItem('redirect') || '';
        }
    },
   
    getQueryString: function () {
        var queryString = {};
        var name, value;
        var str = location.href; //取得整个地址栏
        var num = str.indexOf("?")
        str = str.substr(num + 1); //取得所有参数   stringvar.substr(start [, length ]

        var arr = str.split("&"); //各个参数放到数组里
        for (var i = 0; i < arr.length; i++) {
            num = arr[i].indexOf("=");
            if (num > 0) {
                name = arr[i].substring(0, num);
                value = arr[i].substr(num + 1);
                queryString[name] = value;
            }
        }
        return queryString;
    },
  
    lazyInof: function (id) {
        $(id).lazyload({ effect: "fadeIn", threshold: 100 });
    },
    worksDefaultPicInfo: function () {
        return this.src = basejs.cdnDomain + "/image/default/default-picture_190x180.png";
    },
    getDateTimeStamp: function (dateStr) {
        return Date.parse(dateStr.replace(/T/g, ' ').replace(/-/gi, "/"));
    },
    getDateDiff: function (dateTimeStamp) {
        var minute = 1000 * 60;
        var hour = minute * 60;
        var day = hour * 24;
        var halfamonth = day * 15;
        var month = day * 30;
        var now = new Date().getTime();
        var diffValue = now - dateTimeStamp;
        if (diffValue < 0) { return; }
        var monthC = diffValue / month;
        var weekC = diffValue / (7 * day);
        var dayC = diffValue / day;
        var hourC = diffValue / hour;
        var minC = diffValue / minute;
        if (monthC >= 1) {
            result = "" + parseInt(monthC) + "月前";
        }
        else if (weekC >= 1) {
            result = "" + parseInt(weekC) + "周前";
        }
        else if (dayC >= 1) {
            result = "" + parseInt(dayC) + "天前";
        }
        else if (hourC >= 1) {
            result = "" + parseInt(hourC) + "小时前";
        }
        else if (minC >= 1) {
            result = "" + parseInt(minC) + "分钟前";
        } else
            result = "刚刚";
        return result;
    },
    getNumberDiff: function (number) {

        if (number < 1000) {
            result = number;
        }
        else if (number < 10000) {
            result = (number / 1000).toFixed(1) + "k";
        }
        else {
            result = (number / 10000).toFixed(1) + "w";
        }

        return result;
    }
};

var httpHelper = function httpHelper() {
    this.init.apply(this, arguments);
};


$.extend(httpHelper.prototype, {

    init: function (opts) {

        var _this = this;
        _this.opts = opts;
        _this.opts.type = opts.type || basejs.defaults.type;
        _this.opts.async = (opts.async == null) ? basejs.defaults.async : opts.async;
        _this.opts.contentType = (opts.contentType == null) ? basejs.defaults.contentType : opts.contentType;
        _this.opts.traditional = (opts.traditional == null) ? basejs.defaults.traditional : opts.traditional;
        _this.opts.processData = (opts.processData == null) ? basejs.defaults.processData : opts.processData;
        _this.opts.data = opts.data || basejs.defaults.data;
        _this.opts.loading = opts.loading || basejs.defaults.loading;

        if (!_this.opts.url) {
            console.error("url is empty");
            alert("url is empty");
            return null;
        }
        else { return _this; }
    },




    // 发送数据
    send: function () {
        var _this = this;
        //return $.Deferred(function ($dfd) {
        $.ajax({
            url: _this.opts.url,
            type: _this.opts.type,
            xhrFields: {
                withCredentials: true //配置http跨域请求中携带cookie
            },
            crossDomain: true,
            async: _this.opts.async,
            data: _this.opts.data,
            contentType: _this.opts.contentType,
            traditional: _this.opts.traditional,
            processData: _this.opts.processData,
            success: function (resultDto, textStatus, jqXHR) {//success
                _this.opts.success && _this.opts.success.apply(this, arguments);
            },
            beforeSend: function () {
                // loading();
                //_this.opts.loading !== 'off' && dialog.loading();
                _this.opts.beforeSend && _this.opts.beforeSend.apply(this, arguments);
            },
            complete: function () {
                // _this.opts.loading !== 'off' && dialog.loading.fade();
                _this.opts.complete && _this.opts.complete.apply(this, arguments);
            },

            error: function (jqXHR, textStatus, errorThrown) {

                console.error(errorThrown);
                //Unauthorized
                if (jqXHR.status == 401 || jqXHR.status == 403) {
                    //Location=context.RedirectUri
                    storage.local.setItem("isLogin", "false");
                    _this.opts.error && _this.opts.error.apply(this, arguments);
                    var truthBeTold = window.confirm("请先登陆");
                    if (truthBeTold) {
                        window.location.href = "/login.html";
                    }
                    
                    //var redirectUri = jqXHR.getResponseHeader("Location");
                    //redirectUri = redirectUri.substring(0, redirectUri.indexOf("?"));
                    //window.location.href = redirectUri;
           


                }
            }
        });
        // });


    }

});

//菜单
var topMenu = {
    bindMenu: function (menuObj) {
        var _this = this;
        _this.splashObj = $('#splash');
        _this.quoteObj = _this.splashObj.children('.quote');
        _this.menuList = $('#menuList');


        //菜单
        //$.when($.getJSON(basejs.requestDomain + '/assets/json/menu.json'), $.get(basejs.requestDomain + '/menu/'))
        $.getJSON('/json/menu.json', function (data) {

            _this.setMenu(data.menu || {});
            // _this.initShow();
        });

    },
    setMenu: function (menuObj) {
        var _this = this;
        function addMenu(item, title, wrapper) {

            for (var key in item) {
                for (var i = 0; i < item[key].length; i++) {
                    var m = item[key][i];
                    var child = $(
                        m.sublist ? "<li class='dropdown'><a class='dropdown-toggle' data-toggle='dropdown' href='javascript:;' >"
                            + m.namech
                            + "<span class='caret'></span></a><ul class='dropdown-menu' aria-labelledby='themes'></ul></li>"
                            : "<li><a href='javascript:;'>" + m.namech + "</a></li>").appendTo(wrapper)
                        .children();
                    /*
                     * var child = $("<li " + (m.sublist ? "class='treeview'" : "") + ">" + "<a
                     * href='javascript:;' >" + (isRoot ? "<i class='fa
                     * fa-link'></i> <span>" + m.namech + "</span><span
                     * class='pull-right-container'><i class='fa fa-angle-left
                     * pull-right'></i></span>" : "<i class='fa fa-circle-o'></i>" +
                     * m.namech) + "</a>" + (m.sublist ? "<ul class='treeview-menu'></ul>" :
                     * "") + "</li>" ).appendTo(wrapper).children();
                     */

                    m.title = (title ? (title + ' > ') : "") + m.namech;

                    if (m.url) {
                        _this.menuClick(child.eq(0), m);
                    }

                    if (m.sublist) {
                        addMenu(m.sublist, m.title, child.eq(1));
                    }

                }
            }
        }

        addMenu(menuObj, null, _this.menuList.empty());
    },
    // 触发load
    fireLoad: function (url, title, data) {
        window.location.href = url;
		/*	var _this = this;
			_this.prog.begin();
			pageview.load(url, title, function() {
				_this.prog.finish();
				_this.section.hide().eq(1).show();
			}, data);*/
    },
    menuClick: function (target, item) {
        var _this = this;
        target.on('click', function (event, data) {

            console.log(item.title + ":" + item.url);
            // item['@append_param'] = data;
            // _this.setHash(item.url);

            _this.fireLoad(item.url, item.title, data);
        });

        // target.on('hash', function () {
        // _this.fireLoad(item.url, item.title, item['@append_param']);
        // item['@append_param'] = null;
        // });

        if (basejs.getPath().indexOf(item.url) > -1) {

            _this.fireLoad(item.url, item.title);
            target.addClass("active");

        }
    },
    logout: function () {
        var _this = this;
        var isLogin = storage.local.getItem("isLogin") == "true";

        $("#authBtns1").css("display", isLogin ? "none" : "block");
        $("#authBtns2").css("display", isLogin ? "block" : "none");


        $("#btnLogout").click(function () {

            var helper = new httpHelper({
                url: basejs.requestDomain + "/webuser/logout",
                type: "GET",
                success: function (data) {
                    if (data.result) {
                        storage.local.setItem("isLogin", "false");
                        window.location.href = "/home.html";
                    }
                    else {
                        alert(data.message);
                    }

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("error:" + textStatus);
                }
            });
            helper.send();

        });

    },
    authTest: function () {
        var _this = this;
        $("#btnTest").click(function () {
            var helper = new httpHelper({
                url: basejs.requestDomain + "/user/1",
                type: "GET",
                success: function (data) {
                    alert("登陆状态");
                }
            });
            helper.send();
        });
    }
}

//封面
var topCover = {
    getHomeCover: function (callback) {
        var _this = this;
       
        var resultDtoJson = storage.session.getItem("homeCover");
        if (resultDtoJson) {
            //设置首页封面
            callback && callback(JSON.parse(resultDtoJson));
        }
        else {

            var helper = new httpHelper({
                url: basejs.requestDomain + "/home/cover",
                type: "GET",
                success: function (resultDto) {
                    storage.session.setItem("homeCover", JSON.stringify(resultDto));
                    //设置首页封面
                    callback && callback(resultDto);
                }
            });
            helper.send();
        }

    },
    scroll: function (opts) {
        var _this = this;
        _this.navbarObj = $('#navbar');
        _this.splashObj = $('#splash');
        _this.quoteObj = _this.splashObj.children('.quote');
        _this.contentBlackquoteObj = _this.quoteObj.children('.splash-content').children("blackquote");
        //_this.otherBlackquoteObj=_this.quoteObj.children('.essay-other').children("blackquote");

        //设置首页滚动样式
        $(window).scroll(function () {
            var scrollTop = $(document).scrollTop();
            var navbarHeight = _this.navbarObj.outerHeight();
            var splashHeight = _this.splashObj.outerHeight();
            var splashNoNavbarHeight = (splashHeight - navbarHeight);

            var contentBlackquoteTop = 200;//parseInt(_this.contentBlackquoteObj.css("top"));
            var contentBlackquoteHeight = _this.contentBlackquoteObj.outerHeight();
            var contentBlackquoteTopRange = (splashHeight - contentBlackquoteTop - contentBlackquoteHeight);
            var coeff = (scrollTop / (splashNoNavbarHeight - contentBlackquoteHeight));


            // 导航条

            if (scrollTop < navbarHeight) {
                _this.navbarObj.removeClass('navbar-min');

            } else if (scrollTop < splashNoNavbarHeight) {
                _this.navbarObj.removeClass('navbar-bottom-shadow').addClass('navbar-min');

            } else {
                _this.navbarObj.removeClass('navbar-min').addClass('navbar-bottom-shadow');
            }
            //文字
            if (scrollTop < (splashNoNavbarHeight - contentBlackquoteHeight)) {
                _this.quoteObj.css('opacity', 1);
            }
            else {
                _this.quoteObj.css('opacity', 0);
            }
            _this.contentBlackquoteObj.css({ 'top': (contentBlackquoteTop + contentBlackquoteTopRange * coeff) });

        });
    }
}


 


