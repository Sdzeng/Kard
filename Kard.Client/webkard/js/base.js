var basejs = {
    requestDomain: "http://localhost:3703",//window.location.protocol + "//"+window.location.host;// "http://localhost:3706";//"https://www.localyc.com";
    cdnDomain: "http://localhost:3703",//"http://image.localyc.com";
    defaults: {
        type: "POST",
        async: true,
        contentType: "application/x-www-form-urlencoded",//"application/json;charset=utf-8",
        traditional: false,
        processData: true,
        data: null,
        loading: "off"
    },
    getPath: function () {
        var hash = window.location.hash, reg = /^#!/;
        if (reg.test(hash)) {

            return hash.replace(reg, '');
        } else {
            return hash;//storage.local.getItem('redirect') || '';
        }
    },
    lazyInof: function (id) {
        $(id).lazyload({ effect: "fadeIn", threshold: 50 });
    },
    worksDefaultPicInfo: function () {
        return this.src = basejs.cdnDomain + "/image/default/default-picture_190x180.png";
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
        return $.Deferred(function ($dfd) {
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
                success: function (data, textStatus, jqXHR) {//success
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
                        alert("您未登录不能查看该内容");
                        //var redirectUri = jqXHR.getResponseHeader("Location");
                        //redirectUri = redirectUri.substring(0, redirectUri.indexOf("?"));
                        //window.location.href = redirectUri;
                        window.location.href = "/home.htm";


                    }
                }
            });
        });


    }

});

//菜单
var topMenu = {
    init: function (menuObj) {
        var _this = this;
        _this.splashObj = $('#splash');
        _this.quoteObj = _this.splashObj.children('.quote');
        _this.menuList = $('#menuList');


        //菜单
        //$.when($.getJSON(basejs.requestDomain + '/assets/json/menu.json'), $.get(basejs.requestDomain + '/api/menu/'))
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
    }

}

//封面
var topCover = {
    scroll: function (opts) {
        var _this = this;
        _this.navbarObj = $('#navbar');
        _this.splashObj = $('#splash');
        _this.quoteObj = _this.splashObj.children('.quote');
        _this.contentBlackquoteObj = _this.quoteObj.children('.essay-content').children("blackquote");
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


$(function () {

    topMenu.init();
});


