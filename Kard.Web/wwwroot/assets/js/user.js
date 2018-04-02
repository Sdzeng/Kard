var userCover = {
    init: function () {

        var _this = this;

        //������ҳ����
        var helper = new httpHelper({
            url: baseUrl + "/api/web/cover/",
            success: function (data) {
              
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }


                //data.media.hasOwnProperty("path")&&


                $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + baseUrl+"/"+( data.coverPath || "") + ")").fadeIn("slow");
                $(".essay-content>blackquote>q").text("����");
                $(".author").text("@" + data.nikeName || "");
                

                topCover.scroll();

            },
            error: function (jqXHR, textStatus, errorThrown)
            {
                //Unauthorized
                if (jqXHR.status == 401) {
                    //Location=context.RedirectUri
                    var redirectUri = jqXHR.getResponseHeader("Location");
                    redirectUri = redirectUri.substring(0, redirectUri.indexOf("?"));

                    window.location.href = redirectUri;
                }
            }
        });
        helper.send();
    }
};

$(function () {
    //����
    userCover.init();
});
