var homejs = {
    data: { scope: $("#homePage") },

    homeCover: function () {

        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: basejs.requestDomain + "/api/home/cover/",
            type: "GET",
            success: function (data) {

                //data = JSON.parse(data);
                if (!data) {
                    return;
                }


                //data.media.hasOwnProperty("path")&&

                 

              $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.media.cdnPath + "." + data.media.mediaExtension || "") + ")");
                $(".essay-content>blackquote>q").text(data.media.essay.content || data.media.essay.title);
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
            _this.hostTitleObj = $('#hostTitle', homejs.data.scope);
            _this.hostBodyObj = $('#hostBody', homejs.data.scope);
            _this.splashObj = $('#splash', homejs.data.scope);
            _this.setPicture();

        },
        setPicture: function () {
            var _this = this;

            //设置host
            var helper = new httpHelper({
                url: basejs.requestDomain + "/api/home/pictures/",
                type: "GET",

                success: function (data) {

                    //data = JSON.parse(data);
                    if (!data) {
                        return;
                    }

                    var titleTagArr =[];
                    var pictureHtml = "";
                    var pictureRowHtml = "";
                    //data.media.hasOwnProperty("path")&&
                    for (var index in data) {
                        var media = data[index];
                        var picturePath = basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension;
                        var defaultPicturePath =  "/image/default-picture_190x180.jpg";
                        var pictureCropPath = basejs.cdnDomain + "/" + media.cdnPath + "_190x180." + media.mediaExtension;
                        var tagSpan = "";
 
                        if (media.tagList.length > 0) {
                            tagSpan = (media.tagList[0].tagName.length > 4 ? media.tagList[0].tagName.substr(0, 3) + "..." : media.tagList[0].tagName);
                            titleTagArr.push(media.tagList[0].tagName);
                        }

                        pictureRowHtml += "<div class='picture-warp'>" +
                            "<a href= '" + picturePath + "' >" +
                            "<img class='lazy' src='" + defaultPicturePath + "' data-original='" + pictureCropPath + "'    />" +
                            "</a >" +
                            "<div class='picture-info'>" +
                            "<div class='picture-title'><div>" + media.title + "</div><div><span>by </span><span>" + media.creatorNickName + "</span></div></div>" +
                           
                            "<div class='picture-body'><div class='picture-body-tag'><span>" + tagSpan +"</span></div><div class='picture-body-num'><span class='essay-like-num'>" + media.likeNum + "</span><span class='essay-repost-num'>" + media.repostNum + "</span><span class='essay-comment-num'>" + media.commentNum + "</span></div></div>" +//media.creatorNickName).substring(0, 6) 

                            "</div>" +
                            "</div >";

                        if ((parseInt(index) + 1) % 6 == 0) {
                            pictureHtml += "<div class='section-style-body-row'>"+pictureRowHtml+"</div>";
                            pictureRowHtml = "";
                        }
                    }

                    //for (var i = 0; i < 2; i++) {
                    //    topMediaPictureHtml += topMediaPictureHtml;
                    //}
                    $("h6", _this.hostTitleObj).text(titleTagArr.join(" "));
                    _this.hostBodyObj.append(pictureHtml);
                    //图片懒加载
                    basejs.lazyInof('img.lazy');
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


