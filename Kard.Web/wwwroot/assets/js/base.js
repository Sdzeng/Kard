var baseUrl = "http://localhost:3707";//"https://www.localyc.com";
var defaults = {
    type: "POST",
    async:true,
	contentType:null,//"application/json;charset=utf-8",
	data:null,
	loading:"off"
};


function getPath() {
	var hash = window.location.hash, reg = /^#!/;
	if (reg.test(hash)) {

		return hash.replace(reg, '');
	} else { 
		return hash;//storage.session.getItem('redirect') || '';
	}
}

var httpHelper =function httpHelper() {
	this.init.apply(this, arguments);
};

$.extend(httpHelper.prototype, {

    init: function (opts) {
        
		var _this=this;
		_this.opts =opts;
        _this.opts.type = opts.type || defaults.type;
        _this.opts.async = opts.async || defaults.async;
		_this.opts.contentType=opts.contentType||defaults.contentType;
		_this.opts.data=opts.data||defaults.data;
		_this.opts.loading=opts.loading||defaults.loading;
		
	    if (!_this.opts.url) {
	    	console.error("url is empty");
	    	alert("url is empty");
	    	return null;
	    }
	    else
	    {return _this;}
	},
	// 发送数据
	send:function () {
	       	var _this=this;
	        return $.Deferred(function ($dfd) {
		        $.ajax({
		            url: _this.opts.url,
                    type: _this.opts.type,
                    async: _this.opts.async,
		            data: _this.opts.data,
		            contentType: _this.opts.contentType,
		            traditional: _this.opts.traditional,
		            success: function (data, textStatus, jqXHR) {//success
		                // session超时、操作失败
		                if (/\"out\":true|\"flag\":false/.test(data)) {
		                	// dialog.alert(JSON.parse(data).msg, 'warn');
		                	// 用户登录框
		                	if (/\"out\":true/.test(data)) {
		                		alert("请重新登陆");
		                	}
		                	else {
		                		alert(data.msg||"内容有误");
		                	}
		                }
		                else {
		                	_this.opts.success && _this.opts.success.apply(this, arguments);
		                }
		                
		            },
		            beforeSend: function() {
		                // loading();
		                //_this.opts.loading !== 'off' && dialog.loading();
		                _this.opts.beforeSend && _this.opts.beforeSend.apply(this, arguments);
		            },
		            complete: function() {
		               // _this.opts.loading !== 'off' && dialog.loading.fade();
		                _this.opts.complete && _this.opts.complete.apply(this, arguments);
		            },
		            error: function () {
		                console.error(arguments[2]);
		                alert(arguments[2]);
		            }
		        });
		    });


	}

});

//菜单
var topMenu = {
		init : function(menuObj) {
			var _this = this;
			_this.splashObj = $('#splash');
			_this.quoteObj = _this.splashObj.children('.quote');		
			_this.menuList = $('#menuList');

			
			//菜单
            //$.when($.getJSON(baseUrl + '/assets/json/menu.json'), $.get(baseUrl + '/api/menu/'))
            $.when($.getJSON(baseUrl + '/assets/json/menu.json'))
			.done(
					function() {
					 
						var arg=arguments;
						//var data = arguments[0][0];// root = arguments[1][0].toUpperCase().split(',');
                        var data = arguments[0];
						_this.setMenu(data.menu||{});
						// _this.initShow();
			});
		},
		setMenu : function(menuObj) {
			var _this = this;
			function addMenu(item, title, wrapper) {

				for ( var key in item) {
					for (var i = 0; i < item[key].length; i++) {
						var m = item[key][i];
						var child = $(
								m.sublist ? "<li class='dropdown'><a class='dropdown-toggle' data-toggle='dropdown' href='javascript:;' >"
										+ m.namech
										+ "<span class='caret'></span></a><ul class='dropdown-menu' aria-labelledby='themes'></ul></li>"
										: "<li><a href='javascript:;'>" + m.namech+ "</a></li>").appendTo(wrapper)
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
		fireLoad : function(url, title, data) {
		/*	var _this = this;
			_this.prog.begin();
			pageview.load(url, title, function() {
				_this.prog.finish();
				_this.section.hide().eq(1).show();
			}, data);*/
		},
		menuClick : function(target, item) {
			var _this = this;
			target.on('click', function(event, data) {

				console.log(item.title + ":" + item.url);
				// item['@append_param'] = data;
				// _this.setHash(item.url);
				if (getPath() != item.url) {
					window.location.hash = '#!' + item.url;
				}
				_this.fireLoad(item.url, item.title, data);
			});

			// target.on('hash', function () {
			// _this.fireLoad(item.url, item.title, item['@append_param']);
			// item['@append_param'] = null;
			// });

			if (getPath().indexOf(item.url) > -1) {

				_this.fireLoad(item.url, item.title);
				target.addClass("active");

			}
		}

	}

//封面
var topCover = {
		init : function() {
			var _this = this;
			_this.navbarObj = $('#navbar');
			_this.splashObj = $('#splash');
			_this.quoteObj = _this.splashObj.children('.quote');
			_this.contentBlackquoteObj=_this.quoteObj.children('.essay-content').children("blackquote");
			//_this.otherBlackquoteObj=_this.quoteObj.children('.essay-other').children("blackquote");

			_this.setCover();
		},
		setCover:function(){
			var _this = this;
			
		    //设置首页封面
			var helper=	new httpHelper({
                url: baseUrl+"/api/cover/",
                success: function (data) {
           
					//data = JSON.parse(data);
	                if(!data)
				    {
	                  return;
                    }

           
					//data.media.hasOwnProperty("path")&&
					 
                       
                    $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(/assets/media/" + (data.media.cdnPath + "." + data.media.mediaExtension || "") + ")").fadeIn("slow");
                    $(".essay-content>blackquote>q").text(data.media.essay.simpleContent||"");
						$(".author").text("@"+data.media.kuser.nikeName||"");
						$(".location").text((data.media.essay.location||"")+" 凌晨5点");
				 
				 
				
				}
			});
			helper.send();
			
		  //设置首页滚动样式
		   $(window).scroll(function() {
								var scrollTop = $(document).scrollTop();
								var navbarHeight = _this.navbarObj.outerHeight();
								var splashHeight = _this.splashObj.outerHeight();
								var splashNoNavbarHeight=(splashHeight - navbarHeight);
								
								var contentBlackquoteTop= 200;//parseInt(_this.contentBlackquoteObj.css("top"));
								var contentBlackquoteHeight= _this.contentBlackquoteObj.outerHeight();
								var contentBlackquoteTopRange=(splashHeight - contentBlackquoteTop - contentBlackquoteHeight);
								var coeff=(scrollTop/(splashNoNavbarHeight - contentBlackquoteHeight));
								

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
									_this.quoteObj.css('opacity',1);
								}
								else{
									_this.quoteObj.css('opacity',0);
								}
								_this.contentBlackquoteObj.css({'top':(contentBlackquoteTop + contentBlackquoteTopRange * coeff)});
										
				});
			

		}
	}


$(function() {
	//封面
	topCover.init();
    topMenu.init();
});

 
