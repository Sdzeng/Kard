var homejs = {
    data: { scope: $("#homePage")},
 
    homeCover: function () {

        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: baseUrl + "/api/cover/",
            type: "GET",
            success: function (data) {

                //data = JSON.parse(data);
                if (!data) {
                    return;
                }


                //data.media.hasOwnProperty("path")&&


                $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + imageUrl + "/media/" + (data.media.cdnPath + "." + data.media.mediaExtension || "") + ")").fadeIn("slow");
                $(".essay-content>blackquote>q").text(data.media.essay.simpleContent || "");
                $(".author").text("@" + data.media.kuser.nickName || "");
                $(".location").text((data.media.essay.location || "") + " 凌晨5点");

                topCover.scroll();

            }
        });
        helper.send();

    },

    hostSection: {
        init: function () {
            var _this = this;
            _this.sectionPicturesObj = $('.section-pictures');
            _this.splashObj = $('#splash');
            _this.setPicture();
            _this.setEssay();
        },
        setPicture: function () {
            var _this = this;

            //设置host
            var helper = new httpHelper({
                url: baseUrl + "/api/getpicture/",
                type: "GET",

                success: function (data) {

                    //data = JSON.parse(data);
                    if (!data) {
                        return;
                    }
                    var topMediaPictureHtml = "";
                    //data.media.hasOwnProperty("path")&&
                    for (var index in data) {
                        var media = data[index];
                        var picturePath = imageUrl + "/media/" + media.cdnPath + "." + media.mediaExtension;
                        var pictureCropPath = imageUrl + "/media/" + media.cdnPath + "_190x180." + media.mediaExtension;
                        topMediaPictureHtml += "<div class='picture-warp'>" +
                            "<a href= '" + picturePath + "' >" +
                            "<img src='" + pictureCropPath + "' data-origin='" + picturePath + "' alt='' />" +
                            "</a >" +
                            "<div class='picture-info'>" +
                            "<div class='picture-fond'>" + media.essayLikeNum + "人喜欢</div>" +
                            "<div class='picture-desc'><a href='" + picturePath + "'>" + (media.firstTagName || media.creatorNickName).substring(0, 6) + "</a></div>" +

                            "</div>" +
                            "</div >";
                    }

                    for (var i = 0; i < 2; i++) {
                        topMediaPictureHtml += topMediaPictureHtml;
                    }

                    _this.sectionPicturesObj.append(topMediaPictureHtml);
                }
            });
            helper.send();
        },
        setEssay: function () {
            var _this = this;

            //设置host
            var helper = new httpHelper({
                url: baseUrl + "/api/getessay/",
                type: "GET",
                success: function (data) {

                    //data = JSON.parse(data);
                    if (!data) {
                        return;
                    }
                    var topMediaPictureHtml = "";
                    //data.media.hasOwnProperty("path")&&

                    for (var index in data) {
                        var media = data[index];
                        var picturePath = imageUrl + "/media/" + media.cdnPath + "." + media.mediaExtension;
                        var pictureCropPath = imageUrl + "/media/" + media.cdnPath + "_170x150." + media.mediaExtension;
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

                    _this.sectionPicturesObj.append(topMediaPictureHtml);

                }
            });
            helper.send();
        }
    }
};

$(function () {
    //封面
    homejs.homeCover();
    homejs.hostSection.init();
});


