var loginjs = {
    data: { scope: $("#loginPage") },
    login: function () {
        $("#loginform", this.data.scope).submit(function (event) {

            // 阻止表单提交  
            event.preventDefault();

            var helper = new httpHelper({
                url: basejs.requestDomain + "/api/webuser/login",//this.url || this.form.action,
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
    },
    logout: function () {
        var isLogin = storage.local.getItem("isLogin") == "true";

        $("#authBtns1").css("display", isLogin ? "none" : "block");
        $("#authBtns2").css("display", isLogin ? "block" : "none");


        $("#btnLogout").click(function () {

            var helper = new httpHelper({
                url: basejs.requestDomain + "/api/webuser/logout",
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

        $("#btnTest").click(function () {
            var helper = new httpHelper({
                url: basejs.requestDomain + "/api/user/1",
                type: "GET",
                success: function (data) {
                    alert("登陆状态");
                }
            });
            helper.send();
        });
    }
};

$(function () {
    loginjs.login();
    loginjs.logout();
    loginjs.authTest();
});