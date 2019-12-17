var homejs = {
    data: {
        scope: $("#homePage"),
        loadMorePars: {
            //设置essays加载更多
            offOn: false,
            page: 1,
            isChangeCategory: false
        }
    },
    init: function () {
        var _this = this;
        _this.bindCover();
        _this.hostSection.init();

        $('.go-to-top', _this.data.scope).goToTop();


    
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
            "<div class='picture-header'>#{title}</div>" +
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

            switch (data.essayCoverMediaType) {
                case "picture":
                    //_2560x1200
                    var backgroundImage = "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + data.essayCoverPath + "." + data.essayCoverExtension + ")";
                    $(".navbar", _this.data.scope).addClass("bg-default");
                    $(".bg-default", _this.data.scope).css("background-image", backgroundImage);
                    break;

                case "video":
                    //$(".bg-default", _this.data.scope).removeClass("bg-default");

                    var bgVedioHtml = homejs.template.bgVideo.format({
                        videoCoverPath: basejs.cdnDomain + "/" + data.essayCoverPath + ".jpg",
                        videoPath: basejs.cdnDomain + "/" + data.essayCoverPath + "." + data.essayCoverExtension,
                        videoExtension: data.essayCoverExtension
                    });

                    $(".splash-bg", _this.data.scope).html(bgVedioHtml);
                    break;
            }



            //$(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.essayCover.cdnPath + "_2560x1200.gif") + ")");


           var essayDetailPage = "/" + data.essayPageUrl;

            $("blackquote.splash-txt>q", _this.data.scope).html("<a href='"+essayDetailPage+"' style='color:#fff;'>"+data.essayContent+" 点击查看</a>");
            //图片懒加载
            basejs.lazyInof('blackquote.splash-author>img.lazy');
            var avatarArr = data.kuserAvatarUrl.split('.');
            var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];
            $("blackquote.splash-author>img", _this.data.scope).attr("data-original", avatarCropPath);
            $("blackquote.splash-author>a", _this.data.scope).text(data.kuserNickName || "");
            $("blackquote.splash-author>span:first", _this.data.scope).text((data.essayLocation || ""));
            $("blackquote.splash-author>span:last", _this.data.scope).text(basejs.getDateDiff(basejs.getDateTimeStamp(data.essayCreationTime)));

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

            var $loadMore=$(".load-more>span", homejs.data.scope);
            $loadMore.text("加载中...");

            var httpPars = {
                url: basejs.requestDomain + "/home/essays",
                type: "GET",
                data: { keyword: "",pageIndex:1, pageSize: 15,orderBy:"" },
                success: function (resultDto) {
                    //设置essays加载更多
                    if (!resultDto.result) {
                        return;
                    }
                   
                    _this.showPicture(homejs.data.loadMorePars.isChangeCategory,_this.hostTitleObj, _this.hostBodyObj, resultDto.data.essayList);
                    //图片懒加载
                    $imageLazy = $(".section-style-body-block img.lazy", homejs.data.scope);
                    basejs.lazyInof($imageLazy);
                    $imageLazy.removeClass("lazy");

                    if (resultDto.data.hasNextPage) {
                        homejs.data.loadMorePars.offOn = true;
                        homejs.data.loadMorePars.page++;
                        $loadMore.text("加载更多");
                    }
                    else {
                        homejs.data.loadMorePars.offOn = false;
                        $loadMore.text("已经是底部");
                    }
                },
                error: function () {
                    homejs.data.loadMorePars.offOn = true;
                    $(".section-style-title-little", _this.hostTitleObj).empty();
                    $(".section-style-body-block", _this.hostBodyObj).empty();
                }
            };

            var essaysHttpHelper = new httpHelper(httpPars);
            essaysHttpHelper.send();

            $(".section-style-title-big>span", homejs.data.scope).click(function () {
                $(".section-style-title-big-active", homejs.data.scope).removeClass("section-style-title-big-active");
                $(this).addClass("section-style-title-big-active");
                homejs.data.loadMorePars.offOn = false;
                homejs.data.loadMorePars.page = 1;
                homejs.data.loadMorePars.isChangeCategory = true;
                httpPars.data.keyword = $(this).attr("data-keyword");
                httpPars.data.pageIndex = homejs.data.loadMorePars.page;
                $loadMore.text("加载中...");
                essaysHttpHelper = new httpHelper(httpPars);
                essaysHttpHelper.send();
            });

     
            $loadMore.loadMore(50, function () {
                //这里用 [ off_on ] 来控制是否加载 （这样就解决了 当上页的条件满足时，一下子加载多次的问题啦）
                if (homejs.data.loadMorePars.offOn) {
                    homejs.data.loadMorePars.offOn = false;
                    homejs.data.loadMorePars.isChangeCategory = false;
                    httpPars.data.keyword = $(".section-style-title-big-active", homejs.data.scope).attr("data-keyword");
                    httpPars.data.pageIndex = homejs.data.loadMorePars.page;
                    $loadMore.text("加载中...");
                    essaysHttpHelper = new httpHelper(httpPars);
                    essaysHttpHelper.send();
                }
            });


        },
        showPicture: function (isChangeCategory,$title, $body, data) {

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
                    var essayDetailPage = "/" + topMediaDto.pageUrl;
                    var defaultPicturePath = "/image/default-picture_260x195.jpg";
                    var pictureCropPath = "";
                    switch (topMediaDto.coverMediaType) {
                        case "picture": pictureCropPath = basejs.cdnDomain + "/" + topMediaDto.coverPath + "_260x195." + topMediaDto.coverExtension; break;
                        case "video": pictureCropPath = basejs.cdnDomain + "/" + topMediaDto.coverPath + "_260x195.jpg"; break;
                    }




                    var avatarArr = topMediaDto.avatarUrl.split('.');
                    var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];

                    var tagSpan = "";
                    if (topMediaDto.tagList && topMediaDto.tagList.length > 0) {
                        tagSpan += "<span title='" + topMediaDto.tagList[0].tagName + "'>" + topMediaDto.tagList[0].tagName + "</span>";//(topMediaDto.tagList[0].tagName.length > 4 ? topMediaDto.tagList[0].tagName.substr(0, 3) + "..." : topMediaDto.tagList[0].tagName);
                        titleTagArr.push(topMediaDto.tagList[0].tagName);
                    }
                    tagSpan += "<span title='" + topMediaDto.category + "'>" + topMediaDto.category + "</span>";
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
                       
                        //isOriginal:(topMediaDto.isOriginal ? "原创" : "分享"),
                        title: topMediaDto.title,
                        allstarClass: basejs.getStarClass("minstar", topMediaDto.score),
                        score: topMediaDto.score,
                        creatorNickName: topMediaDto.creatorNickName,
                        //likeNum: basejs.getNumberDiff(topMediaDto.likeNum),
                        //shareNum: basejs.getNumberDiff(topMediaDto.shareNum),
                        browseNum: basejs.getNumberDiff(topMediaDto.browseNum),
                        tagSpan: tagSpan,
                        defaultAvatarPath: basejs.defaults.avatarPath,
                        avatarCropPath: avatarCropPath,
                        //location: topMediaDto.location,
                        creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(topMediaDto.creationTime))

                    });


                    if ((current % 5 == 0) || current == data.length) {
                        pictureHtml += "<div class='section-style-body-row'>" + pictureRowHtml + "</div>";
                        pictureRowHtml = "";
                    }
                }

                //for (var i = 0; i < 2; i++) {
                //    topMediaPictureHtml += topMediaPictureHtml;
                //}
            }

            if (isChangeCategory) {
                //$(".section-style-title-little", $title).html("<span>" + basejs.arrDistinct(titleTagArr).join("</span><span>") + "</span>");
                $(".section-style-body-block", $body).html(pictureHtml);
            }
            else {
                //$(".section-style-title-little", $title).append("<span>" + basejs.arrDistinct(titleTagArr).join("</span><span>") + "</span>");
                $(".section-style-body-block", $body).append(pictureHtml);
            }
 

            if($(".section-style-title-little", $title).html()==""){
                $(".section-style-title-little", $title).html("<span>" + basejs.arrDistinct(titleTagArr).join("</span><span>") + "</span>");
            }

         

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


