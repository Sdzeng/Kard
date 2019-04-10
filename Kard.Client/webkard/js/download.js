var downloadjs = {
    data: { scope: $("#downloadPage") },
    init: function () {
        var _this = this;
        _this.bindCover();

    },
    bindCover: function () {
        var _this = this;

        //������ҳ����
        topCover.getHomeCover(function (resultDto) {
            var data = resultDto.data;
            //data = JSON.parse(data);
            if (!data) {
                return;
            }
            $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + data.essayCoverPath + (data.essayCoverMediaType == "picture" ? "." + data.essayCoverExtension : ".jpg") + ")");
        });
    },

};

$(function () {
    //�˵�
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    downloadjs.init();
});