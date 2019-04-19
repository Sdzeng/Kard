var usercenterjs = {
    data: {
        scope: $("#userCenterPage"),
        loadMorePars: {
            //设置加载更多
            offOn: false,
            page: 1

        }
    },
    init: function () {
        var _this = this;
        _this.userCover();
        _this.uploadAvathor();
        _this.bindMenu();
        _this.bindResult();
      
        $('.go-to-top', _this.data.scope).goToTop();
    },
    template: {
        news: {
            essay: ("<div class='result-warp essay-warp'>" +
                "<div class='result-auth'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{avatarCropPath}' /></div>" +
                "<div class='result-info'>" +
                "<div class='essay-header'>#{creatorNickName} 发表 <a class='essay-title' href='#{essayDetailPage}'>#{essayTitle}</a></div>" +
                "<div class='essay-content'><a  href='#{essayDetailPage}'>#{essayContent}</a></div>" +
                "<div class='essay-footer'>" +
                // "<span>#{location}</span> " +
                "<span>#{creationTime}</span> " +
                // "<span>#{browseNum}阅读</span>" +
                // "<span>#{likeNum}喜欢</span>" +
                // "<span>#{commentNum}评论</span>" +
                "<span class='essay-category'>#{category}</span>" +
                "</div>" +
                "</div>" +
                "<div class='result-cover'><a href='#{essayDetailPage}'><img class='lazy' src='#{defaultPicturePath}' data-original='#{pictureCropPath}'></a></div>" +
                "</div>"),
            essayLike: ("<div class='result-warp  essay-like-warp'>" +
                "<div class='result-auth'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{avatarCropPath}' /></div>" +
                "<div class='result-info'><div class='like-content'>#{creatorNickName} 喜欢了您的文章 <a href='#{essayDetailPage}'>#{essayTitle}</a></div><div class='like-footer'><span class='essay-like-create-date'>#{creationTime}</span></div></div>" +
                "<div class='result-cover'></div>" +
                "</div>"),
            essayComment: ("<div class='result-warp  essay-comment-warp'>" +
            "<div class='result-auth'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{avatarCropPath}' /></div>" +
            "<div class='result-info'><div class='comment-header'>#{creatorNickName} 评论了您的文章 <a href='#{essayDetailPage}'>#{essayTitle}</a></div><div class='comment-content'>#{commentContent} <a href=''>回复</a></div><div class='comment-footer'><span>#{creationTime}</span></div></div>" +
            "<div class='result-cover'></div>" +
            "</div>"),
            essayFans: "",
            essayFollow: ""
        },
        essay: "",
        essayLike: "",
        essayComment: "",
        essayFans: "",
        essayFollow: ""
    },
    userCover: function () {

        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: basejs.requestDomain + "/user/cover",
            type: "GET",
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }

                $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + (data.coverPath || "") + ")");
                //$(".essay-content>blackquote>q").text("测试");
                $(".author-txt-name>span:eq(0)", _this.data.scope).text(data.nickName || "");


                //图片懒加载
                $imageLazy = $(".author-avatar img.lazy", _this.data.scope);
                basejs.lazyInof($imageLazy);


                var avatarArr = data.avatarUrl.split('.');
                $(".author-avatar>img", _this.data.scope).attr("data-original", basejs.cdnDomain + "/" + avatarArr[0] + "_80x80." + avatarArr[1]);
                var $userCenterAuthorTxt = $(".author-txt", _this.data.scope);
                var $userCenterAuthorTxtName = $userCenterAuthorTxt.children(".author-txt-name");
                $userCenterAuthorTxtName.children("span:eq(0)").text(data.nickName);
                $userCenterAuthorTxtName.children("span:eq(1)").text(data.city);
                $userCenterAuthorTxt.children(".author-txt-introduction").text(data.introduction);
                var $userCenterAuthorTxtNum = $userCenterAuthorTxt.children(".author-txt-num");
                $userCenterAuthorTxtNum.children("span:eq(0)").text(data.followNum + "关注");
                $userCenterAuthorTxtNum.children("span:eq(1)").text(data.fansNum + "粉丝");
                $userCenterAuthorTxtNum.children("span:eq(2)").text("获得" + data.likeNum + "个喜欢");

                //topCover.scroll({ page: "user-center" });

            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
        helper.send();


    },


    uploadAvathor: function () {
        var _this = this;

        $(".author-avatar", _this.data.scope).click(function () {
            $("#btnAddAvatarHide", _this.data.scope).trigger("click");
        });

        $("#btnAddAvatarHide", _this.data.scope).change(function () {
            var formData = new FormData();
            var files = $(this).get(0).files;
            if (files.length != 0) {
                formData.append("mediaFile", files[0]);
            }
            var helper = new httpHelper({
                url: basejs.requestDomain + "/user/uploadavathor",
                type: 'POST',
                async: false,
                data: formData,
                contentType: false,
                processData: false,
                success: function (resultDto) {

                    if (resultDto.result) {
                        $(".author-avatar>img", _this.data.scope).attr("src", basejs.requestDomain + "/" + resultDto.data.fileUrl + "_80x80" + resultDto.data.fileExtension);
                    }
                }
            });


            helper.send();
        });
    },
    bindMenu: function () {
        var _this = this;

        $(".menu li", _this.data.scope).click(function () {
            $(".menu-active", _this.data.scope).removeClass("menu-active");
            $(this).addClass("menu-active");
            _this.bindResult();
        });

    },
    bindResult: function () {
        var _this = this;

        var $loadMore = $(".load-more>span", _this.data.scope);
        $loadMore.text("加载中...");

        function successFunc(resultDto){
            //图片懒加载
            $imageLazy = $("#my-content img.lazy", _this.data.scope);
            basejs.lazyInof($imageLazy);
            $imageLazy.removeClass("lazy");
            
            if (resultDto.data.hasNextPage) {
                _this.data.loadMorePars.offOn = true;
                _this.data.loadMorePars.page++;
                $loadMore.text("加载更多");
            }
            else {
                _this.data.loadMorePars.offOn = false;
                $loadMore.text("已经是底部");
            }
        };

        function errorFunc(){
            _this.data.loadMorePars.offOn = true;

            $("#my-content", _this.data.scope).empty();
        }
        
        var opt=$(".menu-active", _this.data.scope).attr("data-opt");
        var httpPars=null;
        switch(opt){
           case "news":httpPars=_this.getNewsHttpPars(successFunc,errorFunc);break;

        }

        var helper=new httpHelper(httpPars);
        helper.send();

        $loadMore.loadMore(50, function () {
            //这里用 [ off_on ] 来控制是否加载 （这样就解决了 当上页的条件满足时，一下子加载多次的问题啦）
            if (_this.data.loadMorePars.offOn) {
                _this.data.loadMorePars.offOn = false;

             
                httpPars.data.pageIndex = _this.data.loadMorePars.page;
                $loadMore.text("加载中...");
                helper = new httpHelper(httpPars);
                helper.send();
            }
        });
    },
    getNewsHttpPars: function (successFunc,errorFunc) {
        var _this = this;

        //设置httpHelper
        var httpPars = {
            url: basejs.requestDomain + "/user/news",
            type: "GET",
            data: { pageIndex: 1, pageSize: 10 },
            success: function (resultDto) {
                var data = resultDto.data.newsList;

                if (!data) {
                    return;
                }
                var newsHtml = "";


                for (var index in data) {
                    var news = data[index];
                    var avatarArr = news.dto.avatarUrl.split('.');
                    var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_40x40." + avatarArr[1];
                    var essayDetailPage = "";

                    switch (news.dto.newsType) {
                        case "essay":
                            essayDetailPage = "/essay-detail.html?id=" + news.dto.id;
                            var defaultPicturePath = "/image/default-picture_100x100.jpg";
                            var pictureCropPath = "";
                            switch (news.info.coverMediaType) {
                                case "picture": pictureCropPath = basejs.cdnDomain + "/" + news.info.coverPath + "_100x100." + news.info.coverExtension; break;
                                case "video": pictureCropPath = basejs.cdnDomain + "/" + news.info.coverPath + "_100x100.jpg"; break;
                            }

                            newsHtml += _this.template.news.essay.format({
                                defaultAvatarPath: basejs.defaults.avatarPath,
                                avatarCropPath: avatarCropPath,
                                creatorNickName: news.dto.nickName,
                                essayDetailPage: essayDetailPage,
                                essayTitle: news.info.title,
                                essayContent: news.info.content,
                                // browseNum: basejs.getNumberDiff(news.info.browseNum),
                                // likeNum: basejs.getNumberDiff(news.info.likeNum),
                                // commentNum: basejs.getNumberDiff(news.info.commentNum),
                                category: news.info.category,
                                //score: news.info.score,
                                // location: news.info.location,
                                creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(news.info.creationTime)),
                                defaultPicturePath: defaultPicturePath,
                                pictureCropPath: pictureCropPath
                            });
                            break;
                        case "essayLike":
                            essayDetailPage = "/essay-detail.html?id=" + news.info.essayId;
                            newsHtml += _this.template.news.essayLike.format({
                                defaultAvatarPath: basejs.defaults.avatarPath,
                                avatarCropPath: avatarCropPath,
                                creatorNickName: news.dto.nickName,
                                essayDetailPage: essayDetailPage,
                                essayTitle: news.dto.essayTitle,
                                creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(news.info.creationTime)),
                            });
                            break;
                            break;
                        case "essayComment":    
                        essayDetailPage = "/essay-detail.html?id=" + news.info.essayId;
                        newsHtml += _this.template.news.essayComment.format({
                            defaultAvatarPath: basejs.defaults.avatarPath,
                            avatarCropPath: avatarCropPath,
                            creatorNickName: news.dto.nickName,
                            essayDetailPage: essayDetailPage,
                            essayTitle: news.dto.essayTitle,
                            commentContent: news.info.content,
                            creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(news.info.creationTime)),
                        });
                        break;
                    }
                }

                $("#my-content", _this.data.scope).append(newsHtml);
               
                successFunc&&successFunc(resultDto);

               
            },
            error: function () {
                errorFunc&&errorFunc();
               
            }
        };

        return httpPars;
    }



};

$(function () {
    //菜单
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    usercenterjs.init();

});
