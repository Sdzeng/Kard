 

var essaydetailjs = {
    data: {
        scope: $("#essayDetailPage"),
        //queryString: basejs.getQueryString(),
        template: {
            comment: ("<div class='comment-info' data-id='#{id}'>"+
                    "<div class='comment-info-auth' >" +
                        "<div  class='comment-info-auth-avatar'><img class='lazy' src='/image/default-avatar.jpg' data-original='#{avatarUrl}'></div>" +
                        //"<div class='comment-info-auth-like'><div>222</div><div>喜欢</div></div>" +
                    "</div>" +
                    "<div class='comment-info-content'>"+
                        "<div class='comment-info-content-user'><div><a>#{nickName}</a>#{introduction}</div><div><span>#{creationTime}</span><span class='comment-info-user-reply'>回复</span></div></div>"+
                        "<div class='comment-info-content-txt'>#{content}</div>"+
                        //"<div class='comment-info-content-btns' ><span>赞#{likeNum}</span><span class='comment-info-content-btns-reply'>回复</span></div>"+
                    "</div>"+
                "</div >"),
            parentComment: ("<div class='comment-info-content-txt-parent'>" +
                "<div><a> #{nickName}</a>#{content}</div> " +
                "#{commentParent}" +
                "</div >"),
            like: ("<div class='like-info'> <span class='like-info-avatar'><img  class='lazy' src='/image/default-avatar.jpg' data-original='#{avatarUrl}'>#{nickName} 喜欢了这篇文章</span><span>#{creationTime}</span></div>"),
            video: (
                "<video class='bg-video' autoplay='autoplay' loop='loop' poster='#{videoCoverPath}' id='bgvideo'>" +
                "<source src='#{videoPath}' type='video/#{videoExtension}' >" +
                "</video >"
            )
        }
    },
    init: function () {
        var _this = this;
        //图片懒加载
      
        _this.bindCover();
        _this.bindEssay();
        _this.bindSimilar();
        _this.bindOther();
        _this.bindEvent();
        _this.bindCommentList();
        _this.bindLikeList();
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

            $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + data.essayCoverPath + (data.essayCoverMediaType =="picture"?"." + data.essayCoverExtension:".jpg") + ")");
        });
    },
    bindEssay: function () {
        var _this = this;
        var essayId=$(".essay-detail-info",_this.data.scope).attr("data-id");
        var avatarImg=$("img[data-initpic]",_this.data.scope);
        var creationTime=$("span[data-creationtime]",_this.data.scope);
        // var isLike=$('span[data-islike]', _this.data.scope);
        // var shareNum=$('span[data-sharenum]', _this.data.scope);

        var avatarArr =avatarImg.attr("data-initpic").split('.');
        var avatarUrl=basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];
        avatarImg.attr("data-original",avatarUrl);
        $(".essay-author-avatar>img", _this.data.scope).attr("data-original", basejs.cdnDomain + "/" + avatarArr[0] + "_80x80." + avatarArr[1]);
        basejs.lazyInof('.essay-detail-info-left img.lazy');
        creationTime.text(basejs.getDateDiff(basejs.getDateTimeStamp(creationTime.attr("data-creationtime"))) + "发布");
        // isLike.text((isLike.attr("data-islike")=="true" ? "已喜欢 " : "喜欢 ") + (parseInt(isLike.attr("data-likenum"))>0?isLike.attr("data-likenum"):""));
        // shareNum.text("分享 "+(parseInt(shareNum.attr("data-sharenum"))>0?shareNum.attr("data-sharenum"):""));
        
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/"+essayId,
            
            type: "GET",
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
            //     $(".category", _this.data.scope).text(data.category).attr("href","search.html?keyword="+data.category);
            //     $(".essay-detail-title", _this.data.scope).text(data.title);
            //     $("title").text(data.title+"_"+data.kuserNickName + "_核心技术");
            //     $("meta[property='og:description']").attr("content",data.meta);

              
            //     var avatarArr = data.kuserAvatarUrl.split('.');

            //     $('.essay-detail-remark', _this.data.scope).html(
            //        "<span><img  class='lazy' src='/image/default-avatar.jpg' data-original='" + basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1] + "' >" + data.kuserNickName + "</span>" +
            //         "<span>" + data.location + "</span><span>" + basejs.getDateDiff(basejs.getDateTimeStamp(data.creationTime)) + "发布</span><span>" + data.browseNum + "阅读</span>");


            //     var tagSpan = "";
            //     for (var i in data.tagList) {
            //         var tag = data.tagList[i];
            //         tagSpan += "<span data-tagid='" + tag.id + "'>" + tag.tagName + "</span>";
            //     }
            //     $(".essay-detail-tag", _this.data.scope).html(tagSpan);

            

            //    /* var imgs = "";
            //     for (var i in data.mediaList) {
            //         var media = data.mediaList[i];
            //         switch (media.mediaType) {
            //             case "picture": imgs += " <img src='" + basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension + "' style=''>"; break;
            //             case "video":
            //                 imgs += _this.data.template.video.format({
            //                     videoCoverPath: basejs.cdnDomain + "/" + media.cdnPath + ".jpg",
            //                     videoPath: basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension,
            //                     videoExtension: media.mediaExtension
            //                 });
            //                 break;
            //         }
                  
            //     }
            //     $('.essay-detail-content', _this.data.scope).html("<p>" + data.content + "</p><p>" + imgs + "</p>");*/

            //     $('.essay-detail-content', _this.data.scope).html(data.content);

             $(".browse-num", _this.data.scope).text(data.browseNum+"阅读");
             $('.essay-detail-like-share', _this.data.scope).html("<span id='btnLike' data-islike='" + data.isLike + "'>" + (data.isLike ? "已喜欢 " : "喜欢 ") + data.likeNum + "</span><span>分享 " + data.shareNum + "</span><span>举报</span>");

           
            //     $(".essay-author-avatar>img", _this.data.scope).attr("data-original", basejs.cdnDomain + "/" + avatarArr[0] + "_80x80." + avatarArr[1]);
            //     $(".essay-author-txt-name>span:eq(0)", _this.data.scope).text(data.kuserNickName);
            //     $(".essay-author-txt-introduction", _this.data.scope).text(data.kuserIntroduction);


                $(".essay-score-num", _this.data.scope).text(data.score);
                $(".people-grade", _this.data.scope).text((data.likeNum>0?data.likeNum +"个喜欢":"推荐分"));


                $(".big-star", _this.data.scope).addClass(basejs.getStarClass("bigstar",data.score));

            //     basejs.lazyInof('.essay-detail-info-left img.lazy');

                $("#btnLike", _this.data.scope).click(function () {
                    var $btnLike = $(this);
                    var isLikeChange = $btnLike != "true";
                    (new httpHelper({
                        url: basejs.requestDomain + "/essay/like",
                        type: "POST",
                        data: { essayId:essayId},
                        success: function (resultDto) {

                            if (resultDto.result) {
                                var data = resultDto.data;
                                $btnLike.attr("data-islike", data.isLike.toString());
                                $btnLike.text((data.isLike ? "已喜欢 " : "喜欢 ") + data.likeNum.toString());
                                _this.bindLikeList();
                            }
                        }
                    })).send();

                });
            }
        });
        helper.send();
    },
    bindSimilar: function () {
        var _this = this;

        //设置相似列表
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/similarlist",
            type: "GET",
            data: { essayId: $(".essay-detail-info",_this.data.scope).attr("data-id") },
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }

                var essayAHtml ="";
                for (var index in data) {
                    var essay = data[index];
                    essayAHtml += "<a href='"+basejs.cdnDomain+"/" + essay.pageUrl + "'><div>" + essay.title +"</div><div>" +(essay.likeNum>0?basejs.getNumberDiff(essay.likeNum) + "人喜欢":"")+"</div></a>";
                }
                if (essayAHtml == "") {
                    essayAHtml = "<div class='div-empty'><div><img src='"+basejs.cdnDomain+"/image/empty.gif'></div><div>空空如也</div></div>";
                }
                $(".essay-similar-a", _this.data.scope).html(essayAHtml);
            }
        });
        helper.send();
    },
    bindOther: function () {
        var _this = this;

        //设置其他列表
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/otherlist",
            type: "GET",
            data: { essayId: $(".essay-detail-info",_this.data.scope).attr("data-id") },
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }

                var essayAHtml = "";
                for (var index in data) {
                    var essay = data[index];
                    essayAHtml += "<a href='"+basejs.cdnDomain+"/" + essay.pageUrl + "'><div>" + essay.title + "</div><div>" + (essay.likeNum>0?basejs.getNumberDiff(essay.likeNum) + "人喜欢":"")+"</div></a>";
                }
                if (essayAHtml == "") {
                    essayAHtml = "<div class='div-empty'><div><img src='"+basejs.cdnDomain+"/image/empty.gif'></div><div>空空如也</div></div>";
                }
                $(".essay-other-a", _this.data.scope).html(essayAHtml);
            }
        });
        helper.send();
    },
    bindEvent: function () {
        var _this = this;
        $("ul.comment-like-menu>li", _this.data.scope).click(function () {
            var $btnLi = $(this);
            $btnLi.siblings().removeClass("comment-like-menu-active");
            $btnLi.addClass("comment-like-menu-active");
            $(".comment-like-list>div", _this.data.scope).removeClass("comment-like-content-active").eq($btnLi.index()).addClass("comment-like-content-active");
        });

        $(".comment-new-submit", _this.data.scope).click(function () {

            var $newComment = $("#newComment", _this.data.scope);
            var parentId = $newComment.attr("data-parent-id");
            var data = {
                essayId: $(".essay-detail-info",_this.data.scope).attr("data-id"),
                content: $newComment.val()
            };
            if (parentId != "" && parentId != null) {
                data["parentId"] = parseInt(parentId);
            }


            var helper = new httpHelper({
                url: basejs.requestDomain + "/essay/addcomment",
                type: "POST",
                data: data,
                success: function (resultDto) {
                    if (!resultDto.result) {
                        alert(resultDto.message);
                    }
                    else {
                        $newComment.val("");
                        _this.bindCommentList();
                    }
                }
            });
            helper.send();
        });

        $('.go-to-top', _this.data.scope).goToTop();
    },
    bindCommentList: function () {
        var _this = this;
        var $newComment = $("#newComment", _this.data.scope);
       

        //评论列表
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/commentlist",
            type: "GET",
            data: { essayId: $(".essay-detail-info",_this.data.scope).attr("data-id")},
            success: function (resultDto) {
                if (!resultDto.result) {
                    alert(resultDto.message);
                }
                var commentHtml = "";
                 
                for (var index in resultDto.data) {
                    var dto = resultDto.data[index];
                    var avatarArr = dto.kuserAvatarUrl.split('.');
                    var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_50x50." + avatarArr[1];

                    var parentCommentHtml = _this._getParentCommentHtml(dto.parentCommentDtoList);
                    var content = parentCommentHtml == "" ? dto.content : ("<div>" + dto.content + "</div>" + parentCommentHtml );

                    commentHtml+= _this.data.template.comment.format({
                        avatarUrl: avatarCropPath,
                        nickName: dto.kuserNickName,
                        creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(dto.creationTime)),
                        content: content,
                        id:dto.id,
                        likeNum: dto.likeNum
                    });
                }

                if (commentHtml == "") {
                    commentHtml =  "<div class='div-empty'><div><img src='"+basejs.cdnDomain+"/image/empty.gif'></div><div>空空如也</div></div>";
                }
      
                $(".comment-info-list", _this.data.scope).html(commentHtml);
                basejs.lazyInof('.comment-info-list img.lazy');

                $(".comment-info-content-btns-like", _this.data.scope).click(function () {
                    var $btnCommentLike = $(this);
                });

                $(".comment-info-user-reply", _this.data.scope).click(function () {
                    var $btnCommentReply = $(this);
                    var parentId = $btnCommentReply.parent().parent().parent().parent().attr("data-id");
                    var replyUserName = $btnCommentReply.parent().parent().find("a").text();
                    $newComment.attr("data-parent-id", parentId).attr("placeholder", "回复：" + replyUserName).focus();
                });
            }
        });
        helper.send();











    },
    bindLikeList: function () {
        var _this = this;

        //喜欢列表
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/likelist",
            type: "GET",
            data: { essayId: $(".essay-detail-info",_this.data.scope).attr("data-id") },
            success: function (resultDto) {
                if (!resultDto.result) {
                    alert(resultDto.message);
                } else {
                   
                    if (resultDto.data) {
                        var likeHtml = "";
                        var likeNum=resultDto.data.length;
                        $(".people-grade", _this.data.scope).text((likeNum>0?likeNum +"个喜欢":"推荐分"));

                        for (var index in resultDto.data) {
                            var entity = resultDto.data[index];
                            var avatarArr = entity.kuserAvatarUrl.split('.');
                            var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];

                            likeHtml += _this.data.template.like.format({
                                avatarUrl: avatarCropPath,
                                nickName: entity.kuserNickName,
                                creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(entity.creationTime)),
                            });
                        }

                        if (likeHtml == "") {
                            likeHtml = "<div class='div-empty'><div><img src='"+basejs.cdnDomain+"/image/empty.gif'></div><div>空空如也</div></div>";
                        }

                        $(".like-list", _this.data.scope).html(likeHtml);
                        basejs.lazyInof('.like-list img.lazy');
                    }
                     

                   
                }
            }
        });
        helper.send();


    },
    _getParentCommentHtml: function (parentCommentDtoList) {
        var _this = this;
        var commentHtml="";
        if (parentCommentDtoList) {
            for (var index in parentCommentDtoList) {
                var dto = parentCommentDtoList[index];
                var commentParentHtml = _this._getParentCommentHtml(dto.parentCommentDtoList);

                commentHtml += _this.data.template.parentComment.format({
                    commentParent: commentParentHtml,
                    nickName: dto.kuser.nickName,
                    content: dto.content
                });
            }
        }
        else {
            commentHtml = "";
        }
        return commentHtml;
    }
}

$(function () {
    //菜单
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    essaydetailjs.init();
 
});
