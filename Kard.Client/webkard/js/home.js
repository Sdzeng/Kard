var homejs = {
    data: { scope: $("#homePage") },

    homeCover: function () {

        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: basejs.requestDomain + "/api/home/cover/",
            type: "GET",
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }


                //data.media.hasOwnProperty("path")&&
              
                var avatarArr = data.media.kuser.avatarUrl.split('.');
                var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];

                $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.media.cdnPath + "." + data.media.mediaExtension || "") + ")");
                $(".essay-content>blackquote>q").text(data.media.essay.content || data.media.essay.title);
                $(".essay-other>blackquote>img").attr("src", avatarCropPath);
                $(".essay-other>blackquote>a").text( data.media.kuser.nickName || "");
                $(".essay-other>blackquote>span:first").text((data.media.essay.location || ""));
                $(".essay-other>blackquote>span:last").text(basejs.getDateDiff(basejs.getDateTimeStamp(data.media.essay.creationTime)));

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
            _this.cosmeticsTitleObj = $('#cosmeticsTitle', homejs.data.scope);
            _this.cosmeticsBodyObj = $('#cosmeticsBody', homejs.data.scope);
            _this.fashionSenseTitleObj = $('#fashionSenseTitle', homejs.data.scope);
            _this.fashionSenseBodyObj = $('#fashionSenseBody', homejs.data.scope);
            _this.originalityTitleObj = $('#originalityTitle', homejs.data.scope);
            _this.originalityBodyObj = $('#originalityBody', homejs.data.scope);
            _this.excerptTitleObj = $('#excerptTitle', homejs.data.scope);
            _this.excerptBodyObj = $('#excerptBody', homejs.data.scope);
         
            _this.setPicture();

        },
        template: {
            pictureRow: ("<div class='picture-warp'>" +
                "<a href= '#{picturePath}' >" +
                "<img class='lazy' src='#{defaultPicturePath}' data-original='#{ pictureCropPath }'    />" +
                "</a >" +
                "<div class='picture-info'>" +
                "<div class='picture-title'>#{title}</div>" +
                "<div class='picture-body'><div class='picture-body-tag'><span>#{tagSpan}</span></div><div class='picture-body-num'><span class='essay-like-num'>#{ likeNum}</span><span class='essay-collect-num'>#{ collectNum}</span> <span class='essay-repost-num'>#{repostNum}</span></div></div>" +//media.creatorNickName).substring(0, 6) 
                "<div class='picture-footer'><div class='picture-footer-author '><span class='essay-avatar'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{ avatarCropPath }'   /> </span><span>#{creatorNickName} </span></div> <div><span class='essay-city'>#{location}</span><span>#{creationTime}</span></div></div>" +
                "</div>" +
                "</div >")
        },
        setPicture: function () {
            var _this = this;

            //设置host
            var hostHelper = new httpHelper({
                url: basejs.requestDomain + "/api/home/hostpictures/",
                type: "GET",
                success: function (resultDto) {
                    _this.showPicture(_this.hostTitleObj, _this.hostBodyObj, resultDto.data);
                    //图片懒加载
                    basejs.lazyInof('.section-style-body-block:first img.lazy');
                }
            });
            hostHelper.send();

            var categoryHelper = new httpHelper({
                url: basejs.requestDomain + "/api/home/categorypictures/",
                type: "GET",
                success: function (resultDto) {
                    
                    _this.showPicture(_this.cosmeticsTitleObj, _this.cosmeticsBodyObj, resultDto.data.cosmeticsList);
                    _this.showPicture(_this.fashionSenseTitleObj, _this.fashionSenseBodyObj, resultDto.data.fashionSenseList);
                    _this.showPicture(_this.originalityTitleObj, _this.originalityBodyObj, resultDto.data.originalityList);
                    _this.showPicture(_this.excerptTitleObj, _this.excerptBodyObj, resultDto.data.excerptList);

                    //图片懒加载
                    basejs.lazyInof('.section-style-body-block:not(:first) img.lazy');
                }
            });
            categoryHelper.send();
          
        },
        showPicture: function ($title, $body, data) {
            var _this = this;
            //data = JSON.parse(data);
            if (!data) {
                return;
            }

            var titleTagArr = [];
            var pictureHtml = "";
            var pictureRowHtml = "";
            //data.media.hasOwnProperty("path")&&
            for (var index in data) {
                var media = data[index];
                var picturePath = basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension;
                var defaultPicturePath = "/image/default-picture_210x180.jpg";
                var pictureCropPath = basejs.cdnDomain + "/" + media.cdnPath + "_210x180." + media.mediaExtension;
                var tagSpan = "";
                var defaultAvatarPath = "/image/default-avatar.png";
                var avatarArr = media.avatarUrl.split('.');
                var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];
                var current = parseInt(index) + 1;

                if (media.tagList.length > 0) {
                    tagSpan = (media.tagList[0].tagName.length > 4 ? media.tagList[0].tagName.substr(0, 3) + "..." : media.tagList[0].tagName);
                    titleTagArr.push(media.tagList[0].tagName);
                }
            
                pictureRowHtml += _this.template.pictureRow.format({
                    picturePath,
                    defaultPicturePath,
                    pictureCropPath,
                    title:media.title,
                    creatorNickName:media.creatorNickName,
                    likeNum: media.likeNum,
                    collectNum: media.collectNum,
                    repostNum:media.repostNum,
                 
                    tagSpan,
                    defaultAvatarPath,
                    avatarCropPath,
                    location: media.location,
                    creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(media.creationTime))

                });


                if ((current % 6 == 0) || current == data.length) {
                    pictureHtml += "<div class='section-style-body-row'>" + pictureRowHtml + "</div>";
                    pictureRowHtml = "";
                }
            }

            //for (var i = 0; i < 2; i++) {
            //    topMediaPictureHtml += topMediaPictureHtml;
            //}
            $("h6",$title).text(titleTagArr.join(" "));
            $(".section-style-body-block",$body).html(pictureHtml);
            
        }

    }
};

$(function () {
    //封面
    homejs.homeCover();
    homejs.hostSection.init();
});


