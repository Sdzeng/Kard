var hostSection = {
    init: function () {
        var _this = this;
        _this.sectionHostLeftObj = $('.section-host .section-host-left');
        _this.splashObj = $('#splash');
        _this.setPicture();

    },
    setPicture: function () {
        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: baseUrl + "/api/topMediaPicture/",
            contentType:"application/json;charset=utf-8",
            success: function (data) {

                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
                var topMediaPictureHtml = "";
                //data.media.hasOwnProperty("path")&&
                for (var index in data)
                {
                    var media = data[index];
                    var picturePath = baseUrl + "/assets/media/" + media.cdnPath + "." + media.mediaExtension;
                    var pictureCropPath = baseUrl + "/assets/media/"+media.cdnPath + "_170x150." + media.mediaExtension;
                    topMediaPictureHtml += "<div class='picture-warp'>"+
                        "<a href= '" + picturePath+"' >" +
                        "<img src='" + pictureCropPath + "' data-origin='" + picturePath+"' alt='' />" +
                        "</a >" +
                        "<div class='picture-desc'>" +
                        "<span class='picture-name'><a href='" + picturePath + "'>" + (media.title || media.creatorNikeName)+"</a></span>" +
                        "<span class='picture-num'>" + media.essayMediaCount+"张</span>" +
                        "</div>" +
                        "</div >";
                }
                _this.sectionHostLeftObj.append(topMediaPictureHtml);
            }
        });
        helper.send();
    }
}

$(function () {
    hostSection.init();
});


