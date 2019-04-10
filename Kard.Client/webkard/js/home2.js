var homeCover = {
    init: function () {

        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: basejs.requestDomain+"/cover/",
            success: function (data) {

                //data = JSON.parse(data);
                if (!data) {
                    return;
                }


                //data.media.hasOwnProperty("path")&&


                $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + imageUrl+"/assets/media/" + (data.essayCoverPath + "." + data.media.mediaExtension || "") + ")").fadeIn("slow");
                $(".essay-content>blackquote>q").text(data.media.essay.simpleContent || "");
                $(".author").text("@" + data.media.kuser.nickName || "");
                $(".location").text((data.media.essay.location || "") + " 凌晨5点");

                topCover.scroll();

            }
        });
        helper.send();
    }
};

var hostSection = {
    init: function () {
        var _this = this;
        _this.sectionHostLeftObj = $('.section-host .section-host-left');
        _this.splashObj = $('#splash');
        _this.setPicture();
        _this.setEssay();
    },
    setPicture: function () {
        var _this = this;

        //设置host
        var helper = new httpHelper({
            url: basejs.requestDomain + "/api/getpicture/",
            contentType: "application/json;charset=utf-8",
            success: function (data) {

                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
                var topMediaPictureHtml = "";
                //data.media.hasOwnProperty("path")&&
                for (var index in data) {
                    var media = data[index];
                    var picturePath = imageUrl + "/assets/media/" + media.cdnPath + "." + media.mediaExtension;
                    var pictureCropPath = imageUrl + "/assets/media/" + media.cdnPath + "_170x150." + media.mediaExtension;
                    topMediaPictureHtml += "<div class='picture-warp'>" +
                        "<a href= '" + picturePath + "' >" +
                        "<img src='" + pictureCropPath + "' data-origin='" + picturePath + "' alt='' />" +
                        "</a >" +
                        "<div class='picture-desc'>" +
                        "<span class='picture-name'><a href='" + picturePath + "'>" + (media.firstTagName || media.creatorNickName).substring(0, 6) + "</a></span>" +
                        "<span class='picture-num'>" + media.essayMediaCount + "张</span>" +
                        "<a class='href-label picture-like'>" + media.essayLikeNum + "人喜欢</a>" +
                        "</div>" +
                        "</div >";
                }
                _this.sectionHostLeftObj.append(topMediaPictureHtml);
            }
        });
        helper.send();
    },
    setEssay: function () {
        var _this = this;

        //设置host
        var helper = new httpHelper({
            url: basejs.requestDomain + "/api/getessay/",
            contentType: "application/json;charset=utf-8",
            success: function (data) {

                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
                var topMediaPictureHtml = "";
                //data.media.hasOwnProperty("path")&&
                for (var index in data) {
                    var media = data[index];
                    var picturePath = imageUrl + "/assets/media/" + media.cdnPath + "." + media.mediaExtension;
                    var pictureCropPath = imageUrl + "/assets/media/" + media.cdnPath + "_170x150." + media.mediaExtension;
                    topMediaPictureHtml += "<div class='picture-warp'>" +
                        "<a href= '" + picturePath + "' >" +
                        "<img src='" + pictureCropPath + "' data-origin='" + picturePath + "' alt='' />" +
                        "</a >" +
                        "<div class='picture-desc'>" +
                        "<span class='picture-name'><a href='" + picturePath + "'>" + (media.firstTagName || media.creatorNickName).substring(0, 6) + "</a></span>" +
                        "<span class='picture-num'>" + media.essayMediaCount + "张</span>" +
                        "<a class='href-label picture-like'>" + media.essayLikeNum + "人喜欢</a>" +
                        "</div>" +
                        "</div >";
                }
                _this.sectionHostLeftObj.append(topMediaPictureHtml);
            }
        });
        helper.send();
    }
};

$(function () {
    //封面
    homeCover.init();
    hostSection.init();
});


