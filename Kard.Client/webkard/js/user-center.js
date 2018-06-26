var usercenterjs = {
    data: { scope: $("#userCenterPage") },
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


                //data.media.hasOwnProperty("path")&&


                $(".bg-layer").css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0.3) 100%),url(" + basejs.cdnDomain + "/" + (data.coverPath || "") + ")").fadeIn("slow");
                //$(".essay-content>blackquote>q").text("测试");
                $(".user-center-author-txt-name>span:eq(0)", _this.data.scope).text( data.nickName || "");
                $(".user-center-author-avatar>img", _this.data.scope).attr("src", basejs.cdnDomain + "/" + data.avatarUrl);;

                topCover.scroll();

            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
        helper.send();


    },
    setPictures: function () {
        var _this = this;

        //设置host
        var helper = new httpHelper({
            url: basejs.requestDomain + "/user/pictures",
            type: "GET",
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }
                var topMediaPictureHtml = "";
                //data.media.hasOwnProperty("path")&&

                for (var index in data) {
                    var media = data[index];
                    var picturePath = basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension;
                    var pictureCropPath = basejs.cdnDomain + "/" + media.cdnPath + "_170x150." + media.mediaExtension;
                    topMediaPictureHtml += "<div class='picture-warp'>" +
                        "<a href= '" + picturePath + "' >" +
                        "<img src='" + pictureCropPath + "' data-origin='" + picturePath + "' alt='' />" +
                        "</a >" +
                        "<div class='picture-desc'>" +
                        "<span class='picture-name'><a href='" + picturePath + "'>" + (media.firstTagName || media.creatorNickName).substring(0, 6) + "</a></span>" +
                        "<span class='picture-num'>" + media.essayMediaCount + "张</span>" +
                        "<a class='href-label picture-like'>" + media.essayLikeNum + "人喜欢</a>" +
                        "</div>" +
                        "</div >";
                }

                $(".section-userinfo-left", _this.data.scope).append(topMediaPictureHtml);

            }
        });
        helper.send();
    },


    uploadImg: function () {
        var _this = this;

        $("#btnAddPic", _this.data.scope).click(function () {
            $("#btnAddPicHide", _this.data.scope).trigger("click");
        });

        $("#btnAddPicHide", _this.data.scope).change(function () {



            //formData.append("Id", rackId);
            //formData.append("PicturePath", $("#PicturePath", $maskScope).val());
            //var files = $("#uploadImgFile", $maskScope).get(0).files;
            //if (files.length != 0) {
            //    formData.append("PictureContent", files[0]);
            //}
            ////else if (!$("#PicturePath", $maskScope).val()) {
            ////    dialog.alert("请选择摆放示意图", "warn").load();
            ////    return false;
            ////}

            //formData.append("RackGoodsLocationJson", JSON.stringify(rackGoodsLocation));
            //$.ajax({
            //    url: basejs.requestDomain + "/web/user/uploadMedia/",
            //    type: "POST",
            //    async: false,
            //    contentType: false,
            //    processData: false,
            //    xhrFields: {
            //        withCredentials: true
            //    },
            //    crossDomain: true,
            //    data: formData,
            //    success: function (res) {
            //        console.info(res.data);
            //        layer.msg(res.message, { time: 1500 });
            //        if (res.code == '200') {
            //            url = res.data;
            //        }
            //    },
            //    error: function (res) {
            //        console.log(res);
            //        //layer.msg("网络异常，请稍后再试", { time: 1500 });
            //    }
            //});

            var formData = new FormData();
            var files = $(this).get(0).files;
            if (files.length != 0) {
                formData.append("mediaFile", files[0]);
            }
            var helper = new httpHelper({
                url: basejs.requestDomain + "/essay/uploadmedia",
                type: 'POST',
                async: false,
                data: formData,
                contentType: false,
                processData: false,
                success: function (resultDto) {

                    if (resultDto.result) {
                        $("#btnAddPic", _this.data.scope).before("<img class='isay-info-buttom-medias-item' data-file-url='" + resultDto.data.fileUrl + "' data-file-extension='" + resultDto.data.fileExtension + "' src='" + basejs.requestDomain + "/" + resultDto.data.fileUrl + "_50x50" + resultDto.data.fileExtension + "'></img>");
                    }
                }
            });


            helper.send();
        });

    },
    saveIsay: function () {
        var _this = this;

        $(".isay-info-category-span", _this.data.scope).click(function () {
            $(".isay-info-category-span-checked", _this.data.scope).removeClass("isay-info-category-span-checked");
            $(this).addClass("isay-info-category-span-checked");
        });

        $(".isay-info-submit", _this.data.scope).click(function () {
            var mediaArr = [];
            $(".isay-info-buttom-medias>img", _this.data.scope).each(function (index, item) {
                var $item = $(item);
                mediaArr.push({
                    sort: index + 1,
                    mediaType: "picture",
                    cdnPath: $item.attr("data-file-url"),
                    mediaExtension: $item.attr("data-file-extension")
                });
            });
            alert($(".isay-info-category-span-checked", _this.data.scope).text());
            var helper = new httpHelper({
                url: basejs.requestDomain + "/essay/add",
                type: 'POST',
                data: {
                    essayEntity: {
                        title: $("#isayTitle", _this.data.scope).val(),
                        category: $(".isay-info-category-span-checked", _this.data.scope).text(),
                        content: $("#isayContent", _this.data.scope).val()
                    },
                    mediaList: mediaArr
                },
                success: function (resultDto) {
                    if (resultDto.result) {
                        $("#isayTitle", _this.data.scope).val("");
                        $("#isayContent", _this.data.scope).val("");
                        $(".isay-info-buttom-medias>img", _this.data.scope).remove();
                        alert("成功成功");
                    }
                }
            });


            helper.send();
        });


    }

};

$(function () {
    //封面
    usercenterjs.userCover();
    usercenterjs.setPictures();
    usercenterjs.uploadImg();
    usercenterjs.saveIsay();
});
