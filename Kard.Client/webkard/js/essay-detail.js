 

var essaydetailjs = {
    data: {
        scope: $("#essayDetailPage"),
        queryString: basejs.getQueryString(),
        template: {
            comment: ("<div class='comment-info'>"+
                "<a class='comment-info-avatar' > <img class='lazy' src='/image/default-avatar.jpg' data-original='#{avatarUrl}'></a>"+
                    "<div class='comment-info-content'>"+
                        "<div class='comment-info-content-user'><a>#{nickName}</a><span>#{creationTime}</span></div>"+
                        "<div class='comment-info-content-txt'>#{content}</div>"+
                        "<div class='comment-info-content-btns' data-id='#{id}'><span>赞#{likeNum}</span><span class='comment-info-content-btns-reply'>回复</span></div>"+
                    "</div>"+
                "</div >"),
            parentComment: ("<div class='comment-info-content-txt-parent'>" +
                    "#{commentParent}" +
                    "<div><a> #{nickName}</a>#{content}</div> "+
                "</div >"),
            like: ("<div class='like-info'> <span class='like-info-avatar'><img  class='lazy' src='/image/default-avatar.jpg' data-original='#{avatarUrl}'>#{nickName} 收集了这件单品</span><span>#{creationTime}</span></div>")
        }
    },
    init: function () {
        var _this = this;
        //图片懒加载
      
        _this.bindCover();
        _this.bindEssay();
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
            $(".bg-layer", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.media.cdnPath + "_2560x1200." + data.media.mediaExtension || "") + ")");
        });
    },
    bindEssay: function () {
        var _this = this;
      
        //设置菜单封面
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/" + _this.data.queryString.id,
            type: "GET",
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
                $("#category", _this.data.scope).text(data.category);
                $(".essay-detail-title", _this.data.scope).text(data.title);

                var tagSpan = "";
                for (var i in data.tagList) {
                    var tag = data.tagList[i];
                    tagSpan += "<span data-tagid='" + tag.id + "'>" + tag.tagName + "</span>";
                }
                $(".essay-detail-tag", _this.data.scope).html(tagSpan);
                $('.essay-detail-remark', _this.data.scope).html("<span>" + basejs.getDateDiff(basejs.getDateTimeStamp(data.creationTime)) + "发布</span> <span>" + data.location + "</span><span>" + data.browseNum + "阅读</span>");


                var imgs = "";
                for (var i in data.mediaList) {
                    var media = data.mediaList[i];
                    imgs += " <img src='" + basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension + "' style=''>";
                }
                $('.essay-detail-content', _this.data.scope).html("<p>" + data.content + "</p><p>" + imgs + "</p>");

                var isLike = (data.essayLike != null);
                $('.essay-detail-like-share', _this.data.scope).html("<span id='btnLike' data-islike='" + isLike + "'>" + (isLike ? "已收集 " : "收集 ") + data.likeNum + "</span><span>分享 " + data.shareNum + "</span><span>举报</span>");

                basejs.lazyInof('.essay-author-avatar img.lazy');
                var avatarArr = data.kuser.avatarUrl.split('.');
                $(".essay-author-avatar>img", _this.data.scope).attr("data-original", basejs.cdnDomain + "/" + avatarArr[0] + "_60x60." + avatarArr[1]);
                $(".essay-author-txt-name>span:eq(0)", _this.data.scope).text(data.kuser.nickName);
                $(".essay-author-txt-introduction", _this.data.scope).text(data.kuser.introduction)

                $("#btnLike", _this.data.scope).click(function () {
                    var $btnLike = $(this);
                    var isLikeChange = $btnLike != "true";
                    (new httpHelper({
                        url: basejs.requestDomain + "/essay/like",
                        type: "POST",
                        data: { essayId: _this.data.queryString.id },
                        success: function (resultDto) {

                            if (resultDto.result) {
                                var data = resultDto.data;
                                $btnLike.attr("data-islike", data.isLike.toString());
                                $btnLike.text((data.isLike ? "已收集 " : "收集 ") + data.likeNum.toString());
                                _this.bindLikeList();
                            }
                        }
                    })).send();

                });
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
                essayId: _this.data.queryString.id,
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
    },
    bindCommentList: function () {
        var _this = this;
        var $newComment = $("#newComment", _this.data.scope);
       

        //评论列表
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/commentlist",
            type: "GET",
            data: { essayId: _this.data.queryString.id},
            success: function (resultDto) {
                if (!resultDto.result) {
                    alert(resultDto.message);
                }
                var commentHtml = "";
                 
                for (var index in resultDto.data) {
                    var dto = resultDto.data[index];
                    var avatarArr = dto.kuser.avatarUrl.split('.');
                    var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_50x50." + avatarArr[1];

                    var parentCommentHtml = _this._getParentCommentHtml(dto.parentCommentDtoList);
                    var content = parentCommentHtml == "" ? dto.content : (parentCommentHtml + "<div>" + dto.content + "</div>");

                    commentHtml+= _this.data.template.comment.format({
                        avatarUrl: avatarCropPath,
                        nickName: dto.kuser.nickName,
                        creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(dto.creationTime)),
                        content: content,
                        id:dto.id,
                        likeNum: dto.likeNum
                    });
                }

      
                $(".comment-info-list", _this.data.scope).html(commentHtml);
                basejs.lazyInof('.comment-info-list img.lazy');

                $(".comment-info-content-btns-like", _this.data.scope).click(function () {
                    var $btnCommentLike = $(this);
                });

                $(".comment-info-content-btns-reply", _this.data.scope).click(function () {
                    var $btnCommentReply = $(this);
                    var parentId = $btnCommentReply.parent().attr("data-id");
                    var replyUserName = $btnCommentReply.parent().parent().children(".comment-info-content-user").children("a").text();
                    $newComment.attr("data-parent-id", parentId).attr("placeholder", "回复：" + replyUserName).focus();
                });
            }
        });
        helper.send();











    },
    bindLikeList: function () {
        var _this = this;

        //收集列表
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/likelist",
            type: "GET",
            data: { essayId: _this.data.queryString.id },
            success: function (resultDto) {
                if (!resultDto.result) {
                    alert(resultDto.message);
                } else {
                   
                    if (resultDto.data) {
                        var likeHtml = "";
                        for (var index in resultDto.data) {
                            var entity = resultDto.data[index];
                            var avatarArr = entity.kuser.avatarUrl.split('.');
                            var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_30x30." + avatarArr[1];

                            likeHtml += _this.data.template.like.format({
                                avatarUrl: avatarCropPath,
                                nickName: entity.kuser.nickName,
                                creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(entity.creationTime)),
                            });
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
