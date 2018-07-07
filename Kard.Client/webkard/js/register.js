var registerjs = {
    data: { scope: $("#registerPage") },
    init: function () {
        var _this = this;
        _this.bindCover();
        _this.login();
    },
    bindCover: function () {
        var _this = this;

        //������ҳ����
        topCover.getHomeCover(function (resultDto) {
            var data = resultDto.data;
            //data = JSON.parse(data);
            if (!data) {
                return;
            }
            $(".bg-layer", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.media.cdnPath + "_2560x1200." + data.media.mediaExtension || "") + ")");
        });
    },
    login: function () {
        var _this = this;
 
        $(".register-form", _this.data.scope).submit(function (event) {

            // ��ֹ���ύ  
            event.preventDefault();

            var helper = new httpHelper({
                url: basejs.requestDomain + "/webuser/register",//this.url || this.form.action,
                type: 'POST',
                //contentType: "application/json;charset=utf-8",
                data: $(this).serialize(),//{"username":$("#username").val()},//
                success: function (data) {
                    //var result = JSON.parse(data);
                    if (data.result) {
                   
                        window.location.href = "/login.html";
                    }
                    else {
                    
                        alert(data.message);

                    }
                },
                error: function () {
                
                }
            });

            helper.send();
        });

    }
};

$(function () {
    //�˵�
    topMenu.bindMenu();

    registerjs.init();
});