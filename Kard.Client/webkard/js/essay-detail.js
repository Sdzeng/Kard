var essaydetailjs = {
    data: { scope: $("#essayDetailPage") },
    init: function () {
        var _this = this;

        _this.topMenu();
        _this.bindEssay();
    },
    topMenu: function () {
        var _this = this;

        //设置菜单封面
        var helper = new httpHelper({
            url: basejs.requestDomain + "/api/home/cover/",
            type: "GET",
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
                $(".bg-layer", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.media.cdnPath + "_2560x1200." + data.media.mediaExtension || "") + ")");
            }
        });
        helper.send();
    },
    bindEssay: function(){
        var _this = this;
        var queryString = basejs.getQueryString();
        //设置菜单封面
        var helper = new httpHelper({
            url: basejs.requestDomain + "/api/home/essay/",
            type: "GET",
            data: { id: queryString.id},
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
                    imgs += " <img src='" + basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension+"' style=''>";
                }
                $('.essay-detail-content', _this.data.scope).html("<p>" + data.content + "</p><p>" + imgs + "</p>");

                $('.essay-detail-like-share', _this.data.scope).html("<span>喜欢 " + data.likeNum + "</span><span>分享 " + data.shareNum +"</span><span>举报</span>");

                $(".essay-author-avatar>img", _this.data.scope).attr("src", basejs.cdnDomain + "/" +data.kuser.avatarUrl);
                $(".essay-author-txt-name>span:eq(0)", _this.data.scope).text(data.kuser.nickName);
                $(".essay-author-txt-introduction", _this.data.scope).text(data.kuser.introduction)
                
            }
        });
        helper.send();
    }
}

$(function () {
 
    essaydetailjs.init();
 
});
