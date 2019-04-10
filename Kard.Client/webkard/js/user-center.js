var usercenterjs = {
    data: { scope: $("#userCenterPage") },
    init: function () {
        var _this = this;
        _this.userCover();
        //_this.setPictures();
        _this.uploadAvathor();
        _this.uploadImg();
        _this.saveIsay();
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


                //data.media.hasOwnProperty("path")&&


                $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + (data.coverPath || "") + ")");
                //$(".essay-content>blackquote>q").text("测试");
                $(".user-center-author-txt-name>span:eq(0)", _this.data.scope).text(data.nickName || "");

                basejs.lazyInof('.user-center-author-avatar img.lazy');
                var avatarArr = data.avatarUrl.split('.');
                $(".user-center-author-avatar>img", _this.data.scope).attr("data-original", basejs.cdnDomain + "/" + avatarArr[0] + "_80x80." + avatarArr[1]);
                var $userCenterAuthorTxt = $(".user-center-author-txt", _this.data.scope);
                var $userCenterAuthorTxtName = $userCenterAuthorTxt.children(".user-center-author-txt-name");
                $userCenterAuthorTxtName.children("span:eq(0)").text(data.nickName);
                $userCenterAuthorTxtName.children("span:eq(1)").text(data.city);
                $userCenterAuthorTxt.children(".user-center-author-txt-introduction").text(data.introduction);
                var $userCenterAuthorTxtNum = $userCenterAuthorTxt.children(".user-center-author-txt-num");
                $userCenterAuthorTxtNum.children("span:eq(0)").text("29关注");
                $userCenterAuthorTxtNum.children("span:eq(1)").text("1200粉丝");
                $userCenterAuthorTxtNum.children("span:eq(2)").text("获得18k个喜欢");

                //topCover.scroll({ page: "user-center" });

            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
        helper.send();


    },
    //setPictures: function () {
    //    var _this = this;

    //    //设置host
    //    var helper = new httpHelper({
    //        url: basejs.requestDomain + "/user/pictures",
    //        type: "GET",
    //        success: function (resultDto) {
    //            var data = resultDto.data;
    //            //data = JSON.parse(data);
    //            if (!data) {
    //                return;
    //            }
    //            var topMediaPictureHtml = "";
    //            //data.media.hasOwnProperty("path")&&

    //            for (var index in data) {
    //                var media = data[index];
    //                var picturePath = basejs.cdnDomain + "/" + media.cdnPath + "." + media.mediaExtension;
    //                var pictureCropPath = basejs.cdnDomain + "/" + media.cdnPath + "_170x150." + media.mediaExtension;
    //                topMediaPictureHtml += "<div class='picture-warp'>" +
    //                    "<a href= '" + picturePath + "' >" +
    //                    "<img src='" + pictureCropPath + "' data-origin='" + picturePath + "' alt='' />" +
    //                    "</a >" +
    //                    "<div class='picture-desc'>" +
    //                    "<span class='picture-name'><a href='" + picturePath + "'>" + (media.firstTagName || media.creatorNickName).substring(0, 6) + "</a></span>" +
    //                    "<span class='picture-num'>" + media.essayMediaCount + "张</span>" +
    //                    "<a class='href-label picture-like'>" + media.essayLikeNum + "人喜欢</a>" +
    //                    "</div>" +
    //                    "</div >";
    //            }

    //            $(".section-userinfo-left", _this.data.scope).append(topMediaPictureHtml);

    //        }
    //    });
    //    helper.send();
    //},

    uploadAvathor: function () {
        var _this = this;

        $(".user-center-author-avatar", _this.data.scope).click(function () {
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
                        $(".user-center-author-avatar>img", _this.data.scope).attr("src", basejs.requestDomain + "/" + resultDto.data.fileUrl + "_80x80" + resultDto.data.fileExtension);
                    }
                }
            });


            helper.send();
        });
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
                        $("#btnAddPic", _this.data.scope)
                            .css("background-image", "url('" + basejs.requestDomain + "/" + resultDto.data.fileUrl + "_100x100." + resultDto.data.fileExtension + "')")
                            .attr("data-file-url", resultDto.data.fileUrl)
                            .attr("data-file-extension", resultDto.data.fileExtension);
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

        var autoSaveData = function (data) {
            var category = $(".isay-info-category-span-checked", _this.data.scope).text();
            var isOriginal = $("#isOriginal", _this.data.scope).prop('checked');


            var tag = $("#isayTag", _this.data.scope).val();
            var title = $("#isayTitle", _this.data.scope).val();

            var content = this.html.get();
            if (!category) { alert("请选择类型"); return; }
            if (!tag) { alert("请填写标签"); return; }
            if (!title) { alert("请填写标题"); return; }
            if (!content) { alert("请填写内容"); return; }


            var helper = new httpHelper({
                url: basejs.requestDomain + "/essay/add",
                type: 'POST',
                data: {
                    essayEntity: {
                        title: title,
                        mediaType: "text",
                        coverFile: $("#btnAddPic", _this.data.scope).attr("data-file-url"),
                        coverExtension: $("#btnAddPic", _this.data.scope).attr("data-file-extension"),
                        isOriginal: isOriginal,
                        category: category,
                        content: content
                    },
                    tagList: [{
                        sort: 1,
                        tagName: tag
                    }]
                },
                success: function (resultDto) {
                    if (resultDto.result) {
                        $("#isayTitle", _this.data.scope).val("");
                        $("#isayContent", _this.data.scope).val("");
                        $(".isay-info-buttom-medias>img", _this.data.scope).remove();
                        alert("发布成功~");
                    }
                }
            });


            helper.send();

            alert('selection saved');
        };
        var displayStatus = function (editor) {
            const pendingActions = editor.plugins.get('PendingActions');
            const statusIndicator = document.querySelector('#autosave-status');

            pendingActions.on('change:hasAny', function(evt, propertyName, newValue) {
                if (newValue) {
                    statusIndicator.classList.add('busy');
                } else {
                    statusIndicator.classList.remove('busy');
                }
            });
        };
        var isayEditor = null;
        ClassicEditor.create(document.querySelector('#editor'), {
            //language: 'zh-cn',
            //plugins: [Markdown],
            fontSize: {
                options: [
                    9,
                    11,
                    13,
                    15,
                    17,
                    19,
                    21
                ]
            },
            fontFamily: {
                options: [
                    '默认,default',
                    'Ubuntu, Arial, sans-serif',
                    'Ubuntu Mono2, Courier New, Courier, monospace'
                ]
            },
            highlight: {
                options: [
                    {
                        model: 'greenMarker',
                        class: 'marker-green',
                        title: '绿色标记',
                        color: 'rgb(25, 156, 25)',
                        type: 'marker'
                    },
                    {
                        model: 'yellowMarker',
                        class: 'marker-yellow',
                        title: '黄色标记',
                        color: '#cac407',
                        type: 'marker'
                    },
                    {
                        model: 'redPen',
                        class: 'pen-red',
                        title: '红色钢笔',
                        color: 'hsl(343, 82%, 58%)',
                        type: 'pen'
                    }
                ]

            },
            ckfinder: {
                uploadUrl: basejs.requestDomain + "/common/fileupload?command=QuickUpload&type=Images&responseType=json"
            },
            autosave: {
                //save(editor) {
                //    // The saveData() function must return a promise
                //    // which should be resolved when the data is successfully saved.
                //    return autoSaveData(editor.getData());
                //}
            },
            toolbar: ['heading', '|',
                'bold', 'italic', 'link', 'bulletedList', 'numberedList', '|',
                'fontSize', 'fontFamily', 'subscript', 'superscript', 'highlight', 'alignment:left', 'alignment:right', 'alignment:center', 'alignment:justify', '|',
                'imageUpload', 'blockQuote', 'insertTable', 'mediaEmbed', 'underline', 'strikethrough', 'code', 'undo', 'redo'
            ],
        })
            .then(function(editor) {
                isayEditor = editor;
                window.editor = editor;
                var data = editor.getData();

                displayStatus(editor);
            })
            .catch(function(err) {
                console.error(err.stack);
            });








        /*
                $.FroalaEditor.DefineIcon('saveSelection', { NAME: 'upload' });
                $.FroalaEditor.RegisterCommand('saveSelection', {
                    title: '发布',
                    focus: true,
                    undo: false,
                    refreshAfterCallback: false,
                    callback: function () {
                       
                        //var mediaArr = [];
                        //$(".isay-info-buttom-medias>img", _this.data.scope).each(function (index, item) {
                        //    var $item = $(item);
                        //    var mediaExtension = $item.attr("data-file-extension");
                        //    var mediaType = "";
                        //    switch (mediaExtension) {
                        //        case "mp4": mediaType = "video"; break;
                        //        default: mediaType = "picture"; break;
                        //    }
        
                        //    mediaArr.push({
                        //        sort: index + 1,
                        //        mediaType: mediaType,
                        //        cdnPath: $item.attr("data-file-url"),
                        //        mediaExtension: mediaExtension
                        //    });
                        //});
                   
         
        
                        var category = $(".isay-info-category-span-checked", _this.data.scope).text();
                        var isOriginal = $("#isOriginal", _this.data.scope).prop('checked');
                   
                   
                        var tag = $("#isayTag", _this.data.scope).val();
                        var title = $("#isayTitle", _this.data.scope).val();
        
                        var content = this.html.get();
                        if (!category) { alert("请选择类型"); return; }
                        if (!tag) { alert("请填写标签"); return; }
                        if (!title) { alert("请填写标题"); return; }
                        if (!content) { alert("请填写内容"); return; }
                       
        
                        var helper = new httpHelper({
                            url: basejs.requestDomain + "/essay/add",
                            type: 'POST',
                            data: {
                                essayEntity: {
                                    title: title,
                                    mediaType:"text",
                                    coverFile: $("#btnAddPic", _this.data.scope).attr("data-file-url"),
                                    coverExtension: $("#btnAddPic", _this.data.scope).attr("data-file-extension"),
                                    isOriginal: isOriginal,
                                    category: category,
                                    content: content
                                },
                                tagList: [{
                                    sort: 1,
                                    tagName: tag
                                }]
                            },
                            success: function (resultDto) {
                                if (resultDto.result) {
                                    $("#isayTitle", _this.data.scope).val("");
                                    $("#isayContent", _this.data.scope).val("");
                                    $(".isay-info-buttom-medias>img", _this.data.scope).remove();
                                    alert("发布成功~");
                                }
                            }
                        });
        
        
                        helper.send();
        
                        alert('selection saved');
                   
    });



//超大屏幕
var toolbarButtons = ['saveSelection', '|', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', '|', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', '|', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', 'quote', 'insertHR', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'undo', 'redo', 'clearFormatting', 'selectAll', 'html'];
////大屏幕
//var toolbarButtonsMD = ['fullscreen', 'bold', 'italic', 'underline', 'fontFamily', 'fontSize', 'color', 'paragraphStyle', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', 'quote', 'insertHR', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'undo', 'redo', 'clearFormatting'];
////小屏幕
//var toolbarButtonsSM = ['fullscreen', 'bold', 'italic', 'underline', 'fontFamily', 'fontSize', 'insertLink', 'insertImage', 'insertTable', 'undo', 'redo'];
////手机
//var toolbarButtonsXS = ['bold', 'italic', 'fontFamily', 'fontSize', 'undo', 'redo'];


$('#isayContent', _this.data.scope)
    .on('froalaEditor.initialized', function (e, editor) {
        $('#isayContent').parents('form').on('submit', function () {
            console.log($('#isayContent').val());
            return false;
        })
    })
    .froalaEditor({
        charCounterCount: true,//默认
        charCounterMax: 3000,//默认-1
        enter: $.FroalaEditor.ENTER_P,
        placeholderText: '#标签#我的推荐理由...',
        language: 'zh_cn',
        saveInterval: 0,//不自动保存，默认10000
        theme: "red",
        heightMin: "350px",
        toolbarBottom: false,//默认
        toolbarButtons: toolbarButtons,
        //toolbarButtonsMD: toolbarButtonsMD,
        //toolbarButtonsSM: toolbarButtonsSM,
        //toolbarButtonsXS: toolbarButtonsXS,
        toolbarInline: false,//true选中设置样式,默认false
        requestWithCORS: true,//默认true
        requestWithCredentials: true,
        imageUploadMethod: 'POST',
        imageUploadURL: basejs.requestDomain + "/essay/froalaupload",
        fullPage: true
        //imageUploadParam         : 'upImg',
        //imageUploadParams: { id: "edit" }



    }) }*/

    }



};

$(function () {
    //菜单
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    usercenterjs.init();

});
