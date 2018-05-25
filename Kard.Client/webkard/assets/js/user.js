var userjs = {
    data: { scope: $("#userPage") },
 
    userCover:  function () {

            var _this = this;

            //设置首页封面
            var helper = new httpHelper({
                url: baseUrl + "/web/user/cover/",
                type: "GET",
                success: function (data) {

                    //data = JSON.parse(data);
                    if (!data) {
                        return;
                    }


                    //data.media.hasOwnProperty("path")&&


                    $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + imageUrl + "/" + (data.coverPath || "") + ")").fadeIn("slow");
                    $(".essay-content>blackquote>q").text("测试");
                    $(".author").text("@" + data.nickName || "");


                    topCover.scroll();

                },
                error: function (jqXHR, textStatus, errorThrown) {
                   
                }
            });
            helper.send();
         
       
    }
};

$(function () {
    //封面
    userjs.userCover();
});
