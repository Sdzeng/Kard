var userCover = {
    init: function () {

        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: baseUrl + "/web/user/cover/",
            type:"GET",
            success: function (data) {
              
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }


                //data.media.hasOwnProperty("path")&&


                $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + imageUrl+"/"+( data.coverPath || "") + ")").fadeIn("slow");
                $(".essay-content>blackquote>q").text("测试");
                $(".author").text("@" + data.nickName || "");
                

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
    },
    testAuth:function () {
        
     $("#btnTest").click(function() {
          var helper = new httpHelper({
            url: baseUrl + "/web/user/1",
            type:"GET",
            success: function (data) {
              
               alert(data);
                 

            },
            error: function (jqXHR, textStatus, errorThrown)
            {
               alert("error:"+textStatus);
            }
        });
        helper.send();
     })

     $("#btnLogout").click(function(){

           var helper = new httpHelper({
            url: baseUrl + "/web/user/logout",
            type:"GET",
            success: function (data) {
              if(data.result)
            {
                window.location.href="/home.htm"; 
            }
            else{
              alert(data.message);
            }

            },
            error: function (jqXHR, textStatus, errorThrown)
            {
               alert("error:"+textStatus);
            }
        });
        helper.send();

     })
    }
};

$(function () {
    //封面
    userCover.init();
    userCover.testAuth();
});
