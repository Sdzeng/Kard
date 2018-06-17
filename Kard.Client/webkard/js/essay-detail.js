var essaydetailjs = {
    data: { scope: $("#essayDetailPage") },
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
    }
}

$(function () {
    //菜单
    essaydetailjs.topMenu();

});
