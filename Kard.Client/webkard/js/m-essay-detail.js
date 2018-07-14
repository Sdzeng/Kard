

var essaydetailjs = {
    data: {
        scope: $("#mEssayDetailPage"),
        queryString: basejs.getQueryString(),
        template: {
            comment: ("<div class='comment-info'>" +
                "<a class='comment-info-avatar' > <img class='lazy' src='/image/default-avatar.jpg' data-original='#{avatarUrl}'></a>" +
                "<div class='comment-info-content'>" +
                "<div class='comment-info-content-user'><a>#{nickName}</a><span>#{creationTime}</span></div>" +
                "<div class='comment-info-content-txt'>#{content}</div>" +
                "</div>" +
                "</div >"),
            parentComment: ("<div class='comment-info-content-txt-parent'>" +
                "<div><a> #{nickName}</a>#{content}</div> " +
                "#{commentParent}" +
                "</div >")
      
        }
    },
    init: function () {
        var _this = this;
        //图片懒加载

        _this.bindEssay();
 
        _this.bindCommentList();
    },
    bindEssay: function () {
        var _this = this;

        //设置单品信息
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/" + _this.data.queryString.id,
            type: "GET",
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
                $(".essay-detail-title", _this.data.scope).text(data.title);

                basejs.lazyInof('.essay-author-avatar img.lazy');
                var avatarArr = data.kuser.avatarUrl.split('.');
                var avatarUrl = basejs.cdnDomain + "/" + avatarArr[0] + "_60x60." + avatarArr[1];
                avatarUrl = avatarUrl.replace(/\\/g, "/");
                $(".essay-author-avatar>img", _this.data.scope).attr("data-original", avatarUrl);
                $('.essay-author-txt-name', _this.data.scope).text(data.kuser.nickName);
                $('.essay-author-txt-introduction', _this.data.scope).html("<span>" + basejs.getDateDiff(basejs.getDateTimeStamp(data.creationTime)) + "发布</span> <span>" + data.browseNum + " 阅读</span>");


                var imgs = "";
                for (var i in data.mediaList) {
                    var media = data.mediaList[i];
                    imgs += " <img src='" + basejs.cdnDomain + "/" + media.cdnPath + "_600x400." + media.mediaExtension + "' style=''>";
                }
                $('.essay-detail-content', _this.data.scope).html("<p>" + data.content + "</p><p>" + imgs + "</p>");

                //var tagSpan = "";
                //for (var i in data.tagList) {
                //    var tag = data.tagList[i];
                //    tagSpan += "<span data-tagid='" + tag.id + "'>" + tag.tagName + "</span>";
                //}
                //$(".essay-detail-tag", _this.data.scope).html(tagSpan);
           

                var isLike = (data.essayLike != null);
                $('.essay-detail-like-share', _this.data.scope).html("<span id='btnLike' data-islike='" + isLike + "'>" + (isLike ? "已喜欢 " : "喜欢 ") + data.likeNum + "</span><span>分享 " + data.shareNum + "</span><span>举报</span>");

                var jssdkHelper = new httpHelper({
                    url: basejs.requestDomain + '/essay/jssdk',
                    type: "POST",
                    data: { url: location.href.split('#')[0] },
                    success: function (resultDto2) {
                        if (!resultDto2.result) {
                            alert(resultDto2.message);
                        }
                        var data2 = resultDto2.data;
                        //alert(JSON.stringify(data2));
                        wx.config({
                            debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
                            appId: data2.appId, // 必填，公众号的唯一标识
                            timestamp: data2.timestamp, // 必填，生成签名的时间戳
                            nonceStr: data2.nonceStr, // 必填，生成签名的随机串
                            signature: data2.signature,// 必填，签名
                            jsApiList: ['onMenuShareTimeline','onMenuShareAppMessage']// 必填，需要使用的JS接口列表
                        });
                        wx.error(function (res) {
                            alert('wx.error: ' + JSON.stringify(res));
                        });

                        wx.ready(function () {
                            var link = location.href.split('#')[0];
                            //转发到朋友圈
                            wx.onMenuShareTimeline({
                                title: data.title,
                                link: link,
                                imgUrl: avatarUrl,
                                success: function () {
                                    //alert('转发成功！');
                                },
                                cancel: function () {
                                    //alert('转发失败！');
                                }
                            });
                            //转发给朋友
                            //alert(((data.content || "").length > 20 ? data.content.substr(0, 20) : data.content));
                            //var imgUrl = basejs.cdnDomain + "/" + avatarArr[0] + "_60x60." + avatarArr[1];
                            //imgUrl = imgUrl.replace(/\\/g, "/");
                            //alert(imgUrl);
                            wx.onMenuShareAppMessage({
                                title: data.title,
                                desc: ((data.content || "").length > 20 ? data.content.substr(0, 20)+"..." : data.content),
                                link: link,
                                imgUrl: avatarUrl,
                                type: 'link',
                                dataUrl: '',
                                success: function () {
                                    //alert('转发成功！');
                                },
                                cancel: function () {

                                    //alert('转发失败！');
                                }
                            });
                        });

                    },
                    error: function (res) {

                        alert("报错：" + res);
                        console.log(res);
                        //layer.msg("网络异常，请稍后再试", { time: 1500 });
                    }
                });

                jssdkHelper.send();
              
            }
        });
        helper.send();
    },
 
    bindCommentList: function () {
        var _this = this;


        //评论列表
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/commentlist",
            type: "GET",
            data: { essayId: _this.data.queryString.id },
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
                    var content = parentCommentHtml == "" ? dto.content : ("<div>" + dto.content + "</div>" + parentCommentHtml );

                    commentHtml += _this.data.template.comment.format({
                        avatarUrl: avatarCropPath,
                        nickName: dto.kuser.nickName,
                        creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(dto.creationTime)),
                        content: content,
                        id: dto.id,
                        likeNum: dto.likeNum
                    });
                }


                $(".comment-info-list", _this.data.scope).html(commentHtml);
                basejs.lazyInof('.comment-info-list img.lazy');

            }
        });
        helper.send();

    },
    
    _getParentCommentHtml: function (parentCommentDtoList) {
     
        var _this = this;
        var commentHtml = "";
        if (parentCommentDtoList) {
            for (var index in parentCommentDtoList) {
                var dto = parentCommentDtoList[index];
                var commentParentHtml = _this._getParentCommentHtml(dto.parentCommentDtoList);

                commentHtml +=  _this.data.template.parentComment.format({
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
    essaydetailjs.init();

});
