var homejs = {
    data: { scope: $("#homePage") },
    init: function () {
        var _this = this;
        _this.bindCover();
        _this.hostSection.init();

        $('.go-to-top',_this.data.scope).goToTop();
    },
    template: {
        bgVideo: (
            "<video class='bg-video' autoplay='autoplay' loop='loop' poster='#{videoCoverPath}' id='bgvideo'>" +
            "<source src='#{videoPath}' type='video/#{videoExtension}' >" +
            "</video >"
        ),
        pictureRow: ("<div class='picture-warp'>" +
            "<a href= '#{essayDetailPage}' >" +
            "<img class='lazy' src='#{defaultPicturePath}' data-original='#{ pictureCropPath }'    />" +
            "</a >" +
            "<div class='picture-info'>" +
            "<div class='picture-header'><span>#{category}</span><span title='#{title}'>#{title}</span></div>" +
            "<div class='picture-body'><div><span class='min-star #{allstarClass}'></span><span class='essay-score'>#{score}</span></div><div class='picture-body-tag'>#{tagSpan}</div></div>" +
            //"<div class='picture-body'><div class='picture-body-tag'>#{tagSpan}</div><div class='picture-body-num'><span class='essay-like-num'>#{ likeNum}</span><span class='essay-share-num'>#{shareNum}</span><span class='essay-browse-num'>#{browseNum}</span></div></div>" +//media.creatorNickName).substring(0, 6)
            //"<div class='picture-footer'><div class='picture-footer-author '><span class='essay-avatar'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{ avatarCropPath }'   /> </span><span>#{creatorNickName} </span></div> <div><span class='essay-city'>#{location}</span><span>#{creationTime}</span></div></div>" +
            "<div class='picture-footer'><div class='picture-footer-author '><span class='essay-avatar'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{ avatarCropPath }'   /> </span><span>#{creatorNickName} </span></div> <div><span class='essay-creationtime'>#{creationTime}</span><span>#{browseNum}阅读</span></div></div>" +
            "</div>" +
            "</div >")
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
            //data.media.hasOwnProperty("path")&&

            switch (data.media.mediaType) {
                case "picture":
                    //_2560x1200
                    var backgroundImage = "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + data.media.cdnPath + "." + data.media.mediaExtension + ")";
                    $(".navbar", _this.data.scope).addClass("bg-default");
                    $(".bg-default", _this.data.scope).css("background-image", backgroundImage);
                    break;

                case "video":
                    //$(".bg-default", _this.data.scope).removeClass("bg-default");

                    var bgVedioHtml = homejs.template.bgVideo.format({
                        videoCoverPath: basejs.cdnDomain + "/" + data.media.cdnPath + ".jpg",
                        videoPath: basejs.cdnDomain + "/" + data.media.cdnPath + "." + data.media.mediaExtension,
                        videoExtension: data.media.mediaExtension
                    });

                    $(".splash-bg", _this.data.scope).html(bgVedioHtml);
                    break;
            }



            //$(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.media.cdnPath + "_2560x1200.gif") + ")");




            $("blackquote.splash-txt>q", _this.data.scope).text(data.media.essay.content || data.media.essay.title);
            //图片懒加载
            basejs.lazyInof('blackquote.splash-author>img.lazy');
            var avatarArr = data.media.kuser.avatarUrl.split('.');
            var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];
            $("blackquote.splash-author>img", _this.data.scope).attr("data-original", avatarCropPath);
            $("blackquote.splash-author>a", _this.data.scope).text(data.media.kuser.nickName || "");
            $("blackquote.splash-author>span:first", _this.data.scope).text((data.media.essay.location || ""));
            $("blackquote.splash-author>span:last", _this.data.scope).text(basejs.getDateDiff(basejs.getDateTimeStamp(data.media.essay.creationTime)));

            topCover.scroll({ page: "home" });
        });

    },

    hostSection: {

        init: function () {
            var _this = this;
            _this.hostTitleObj = $('#hostTitle', homejs.data.scope);
            _this.hostBodyObj = $('#hostBody', homejs.data.scope);
            //_this.cosmeticsTitleObj = $('#cosmeticsTitle', homejs.data.scope);
            //_this.cosmeticsBodyObj = $('#cosmeticsBody', homejs.data.scope);
            //_this.fashionSenseTitleObj = $('#fashionSenseTitle', homejs.data.scope);
            //_this.fashionSenseBodyObj = $('#fashionSenseBody', homejs.data.scope);
            //_this.originalityTitleObj = $('#originalityTitle', homejs.data.scope);
            //_this.originalityBodyObj = $('#originalityBody', homejs.data.scope);
            //_this.excerptTitleObj = $('#excerptTitle', homejs.data.scope);
            //_this.excerptBodyObj = $('#excerptBody', homejs.data.scope);

            _this.setPicture();

        },

        setPicture: function () {
            var _this = this;

            ////设置host
            //var hostHelper = new httpHelper({
            //    url: basejs.requestDomain + "/home/hostpictures",
            //    type: "GET",
            //    success: function (resultDto) {
            //        _this.showPicture(_this.hostTitleObj, _this.hostBodyObj, resultDto.data);
            //        //图片懒加载
            //        basejs.lazyInof('.section-style-body-block:first img.lazy');
            //    }
            //});
            //hostHelper.send();

            //var categoryHelper = new httpHelper({
            //    url: basejs.requestDomain + "/home/categorypictures",
            //    type: "GET",
            //    success: function (resultDto) {

            //        _this.showPicture(_this.cosmeticsTitleObj, _this.cosmeticsBodyObj, resultDto.data.cosmeticsList);
            //        _this.showPicture(_this.fashionSenseTitleObj, _this.fashionSenseBodyObj, resultDto.data.fashionSenseList);
            //        _this.showPicture(_this.originalityTitleObj, _this.originalityBodyObj, resultDto.data.originalityList);
            //        //_this.showPicture(_this.excerptTitleObj, _this.excerptBodyObj, resultDto.data.excerptList);

            //        //图片懒加载
            //        basejs.lazyInof('.section-style-body-block:not(:first) img.lazy');
            //    }
            //});
            //categoryHelper.send();

            //设置essays
            var pars = {
                url: basejs.requestDomain + "/home/essays",
                type: "GET",
                data: { category: "优选", count: 18 },
                success: function (resultDto) {
                    _this.showPicture(_this.hostTitleObj, _this.hostBodyObj, resultDto.data);
                    //图片懒加载
                    basejs.lazyInof('.section-style-body-block:first img.lazy');
                }
            };

            var essaysHttpHelper = new httpHelper(pars);
            essaysHttpHelper.send();

            $(".section-style-title-big>span", homejs.data.scope).click(function () {
                $(".section-style-title-big-active", homejs.data.scope).removeClass("section-style-title-big-active");
                $(this).addClass("section-style-title-big-active");
                pars.data.category = $(this).text();
                essaysHttpHelper = new httpHelper(pars);
                essaysHttpHelper.send();
            });

        
        },
        showPicture: function ($title, $body, data) {

            var _this = this;
            var titleTagArr = [];
            var pictureHtml = "";
            //data = JSON.parse(data);
            if (data) {
                var pictureRowHtml = "";
                //data.media.hasOwnProperty("path")&&
                for (var index in data) {
                    var current = parseInt(index) + 1;
                    var topMediaDto = data[index];
                    var essayDetailPage = "/essay-detail.html?id=" + topMediaDto.id;
                    var defaultPicturePath = "/image/default-picture_210x180.jpg";
                    var pictureCropPath = "";
                    switch (topMediaDto.mediaType) {
                        case "picture": pictureCropPath = basejs.cdnDomain + "/" + topMediaDto.cdnPath + "_210x180." + topMediaDto.mediaExtension; break;
                        case "video": pictureCropPath = basejs.cdnDomain + "/" + topMediaDto.cdnPath + "_210x180.jpg"; break;
                    }




                    var avatarArr = topMediaDto.avatarUrl.split('.');
                    var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];

                    var tagSpan = "";
                    if (topMediaDto.tagList && topMediaDto.tagList.length > 0) {
                        tagSpan += "<span title='" + topMediaDto.tagList[0].tagName + "'>" + topMediaDto.tagList[0].tagName + "</span>";//(topMediaDto.tagList[0].tagName.length > 4 ? topMediaDto.tagList[0].tagName.substr(0, 3) + "..." : topMediaDto.tagList[0].tagName);
                        titleTagArr.push(topMediaDto.tagList[0].tagName);
                    }

                    //pictureRowHtml += _this.template.pictureRow.format({
                    //    essayDetailPage,
                    //    defaultPicturePath,
                    //    pictureCropPath,
                    //    title:topMediaDto.title,
                    //    creatorNickName:topMediaDto.creatorNickName,
                    //    likeNum: basejs.getNumberDiff(topMediaDto.likeNum),
                    //    shareNum: basejs.getNumberDiff(topMediaDto.shareNum),
                    //    browseNum: basejs.getNumberDiff(topMediaDto.browseNum),
                    //    tagSpan,
                    //    defaultAvatarPath,
                    //    avatarCropPath,
                    //    location: topMediaDto.location,
                    //    creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(topMediaDto.creationTime))

                    //});

                    pictureRowHtml += homejs.template.pictureRow.format({
                        essayDetailPage: essayDetailPage,
                        defaultPicturePath: defaultPicturePath,
                        pictureCropPath: pictureCropPath,
                        category: topMediaDto.category,
                        //isOriginal:(topMediaDto.isOriginal ? "原创" : "分享"),
                        title: topMediaDto.title,
                        allstarClass: basejs.getStarClass("minstar", topMediaDto.score),
                        score: topMediaDto.score,
                        creatorNickName: topMediaDto.creatorNickName,
                        //likeNum: basejs.getNumberDiff(topMediaDto.likeNum),
                        //shareNum: basejs.getNumberDiff(topMediaDto.shareNum),
                        browseNum: basejs.getNumberDiff(topMediaDto.browseNum),
                        tagSpan: "<span>" + (topMediaDto.isOriginal ? "原创" : "分享") + "</span>"+tagSpan,
                        defaultAvatarPath: basejs.defaults.avatarPath,
                        avatarCropPath: avatarCropPath,
                        //location: topMediaDto.location,
                        creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(topMediaDto.creationTime))

                    });


                    if ((current % 6 == 0) || current == data.length) {
                        pictureHtml += "<div class='section-style-body-row'>" + pictureRowHtml + "</div>";
                        pictureRowHtml = "";
                    }
                }

                //for (var i = 0; i < 2; i++) {
                //    topMediaPictureHtml += topMediaPictureHtml;
                //}
            }
            $(".section-style-title-little", $title).html("<span>" + basejs.arrDistinct(titleTagArr).join("</span><span>") +"</span>");
            $(".section-style-body-block", $body).html(pictureHtml);

        }

    }
};

$(function () {
    //菜单
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    homejs.init();


});


