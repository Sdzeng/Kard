var loginjs = {
    data: { scope: $("#loginPage") },
    init: function () {
        var _this = this;
        _this.bindCover();
        _this.login();
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
            $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + data.essayCoverPath + (data.essayCoverMediaType == "picture" ? "_2560x1200." + data.essayCoverExtension : ".jpg") + ")");
        });
    },
    login: function () {
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

            var helper = new httpHelper({
                url: basejs.requestDomain + "/webuser/login",//this.url || this.form.action,
                type: 'POST',
                //contentType: "application/json;charset=utf-8",
                data: $(this).serialize(),//{"username":$("#username").val()},//
                success: function (data) {
                    //var result = JSON.parse(data);
                    if (data.result) {
                        storage.local.setItem("isLogin", "true");
                        window.location.href = "/user-center.html";
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