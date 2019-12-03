var loginjs = {
    data: { scope: $("#loginPage") },
    init: function () {
        var _this = this;
        _this.bindCover();
        _this.wechatLogin();
        _this.accountLogin();
    },
    bindCover: function () {
        var _this = this;

        //设置首页封面
        topCover.getHomeCover(function (resultDto) {
            var data = resultDto.data;
            //data = JSON.parse(data);
            if (!data) {
                return;
            }
            $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + data.essayCoverPath + (data.essayCoverMediaType == "picture" ? "." + data.essayCoverExtension : ".jpg") + ")");
        });
    },
    wechatLogin: function () {
        var _this = this;

        //微信二维码
        var obj = new WxLogin({
            self_redirect:true,
            id:"login_qr", 
            appid: "wx60bdfaa34a437035", 
            scope: "snsapi_userinfo", 
            redirect_uri: "http%3A%2F%2Fapi.coretn.cn%2Fwechatlogincallback",
            state: "",
            style: "",
            href: "https://open.weixin.qq.com/connect/oauth2/authorize?appid={appId}&redirect_uri=http%3A%2F%2Fapi.coretn.cn%2Fwechatlogincallback&response_type=code&scope=snsapi_userinfo&state=STATE"
          });


    },
    accountLogin: function () {
        var _this = this;

        $("#showPhoneLogin", _this.data.scope).click(function () {
            $("#accountLogin", _this.data.scope).removeClass("login-form-active");
            $("#phoneLogin", _this.data.scope).addClass("login-form-active");
        });
        $("#showAccountLogin", _this.data.scope).click(function () {
            $("#phoneLogin", _this.data.scope).removeClass("login-form-active");
            $("#accountLogin", _this.data.scope).addClass("login-form-active");
        });

        $(".login-form", _this.data.scope).submit(function (event) {

            // 阻止表单提交  
            event.preventDefault();

            var formData = $(this).serialize();
            var queryData = basejs.getQueryString();
            //if (queryData.returnUrl) {
            //    formData += "&returnUrl=" + queryData.returnUrl;
            //}
            var helper = new httpHelper({
                url: basejs.requestDomain + "/login/accountlogin",//this.url || this.form.action,
                type: 'POST',
                //contentType: "application/json;charset=utf-8",
                data: formData,//{"username":$("#username").val()},//
                success: function (data) {
                    //var result = JSON.parse(data);
                    if (data.result) {
                        storage.local.setItem("isLogin", "true");
                        if (queryData.returnUrl) {
                            window.location.href = decodeURI(queryData.returnUrl);
                        }
                        else {
                            window.location.href = "/user-center.html";
                        }
                       
                    }
                    else {
                        storage.local.setItem("isLogin", "false");
                        alert(data.message);

                    }
                },
                error: function () {
                    storage.local.setItem("isLogin", "false");
                }
            });

            helper.send();
        });

    }
};

$(function () {
    //菜单
    topMenu.bindMenu();

    loginjs.init();
});