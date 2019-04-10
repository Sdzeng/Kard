//  上传适配器，格式官网上有，以一种Promise 的方式。Promise好像是有阻塞的意思在里面。



var editorjs = {
    data: {
        scope: $("#editorPage"),
        queryString: basejs.getQueryString(),
        editor: null,
        essayId: 0
    },
    init: function () {
        var _this = this;

        $(".isay-info-category-span", _this.data.scope).click(function () {
            $(".isay-info-category-span-checked", _this.data.scope).removeClass("isay-info-category-span-checked");
            $("#category", _this.data.scope).val("");
            $(this).addClass("isay-info-category-span-checked");

        });

        $("#category", _this.data.scope).change(function () {
            $(".isay-info-category-span-checked", _this.data.scope).removeClass("isay-info-category-span-checked");
        });

        $(".btn-save", _this.data.scope).click(function () {
            _this.saveEssay(false, _this.data.editor.getData());
        });

        //$(".btn-publish", _this.data.scope).click(function () {
        //    _this.saveEssay(true,_this.data.editor.getData());
        //});

        _this.bindEditor();
        _this.bindEssay();
        _this.bindUploadImg();
    },

    bindUploadImg: function () {
        var _this = this;

        $("#btnAddPic", _this.data.scope).click(function () {
            $("#btnAddPicHide", _this.data.scope).trigger("click");
        });

        $("#btnAddPicHide", _this.data.scope).change(function () {

            var formData = new FormData();
            var files = $(this).get(0).files;
            if (files.length != 0) {
                formData.append("file", files[0]);
            }
            var helper = new httpHelper({
                url: basejs.requestDomain + "/common/uploadfile",
                type: 'POST',
                async: false,
                data: formData,
                contentType: false,
                processData: false,
                success: function (resultDto) {

                    if (resultDto.result) {
                        $("#btnAddPic", _this.data.scope)
                            .css("background-image", "url('" + basejs.requestDomain + "/" + resultDto.data.fileUrl + "_260x194." + resultDto.data.fileExtension + "')")
                            .css("color", "white")
                            .attr("data-file-url", resultDto.data.fileUrl)
                            .attr("data-file-extension", resultDto.data.fileExtension);

                    }
                }
            });


            helper.send();
        });

    },

    //displayStatus: function (editor) {
    //    var _this = this;
    //    const pendingActions = editor.plugins.get('PendingActions');
    //    const statusIndicator = $('.isay-info-status', _this.data.scope);

    //    pendingActions.on('change:hasAny', (evt, propertyName, newValue) => {
    //        if (newValue) {
    //            statusIndicator.addClass('busy');
    //        } else {
    //            statusIndicator.removeClass('busy');
    //        }
    //    });
    //},
    bindEditor: function () {
        var _this = this;

       


        var editorData = null;
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
            //ckfinder: {
            //    uploadUrl: basejs.requestDomain + "/common/editoruploadfile?command=QuickUpload&type=Files&responseType=json",
            //    options: {
            //    }
            //},
            //autosave: {
            //    save(editor) {
            //        // The saveData() function must return a promise
            //        // which should be resolved when the data is successfully saved.
            //        var data = editor.getData();


            //        return new Promise(resolve => {
            //            if (data) {
            //                _this.autoSaveData(data);
            //            }
            //            resolve();
            //        });
            //    }

            //},
            toolbar: ['heading', '|',
                'bold', 'italic', 'link', 'bulletedList', 'numberedList', '|',
                'fontSize', 'fontFamily', 'subscript', 'superscript', 'highlight', 'alignment:left', 'alignment:right', 'alignment:center', 'alignment:justify', '|',
                'imageUpload', 'blockQuote', 'insertTable', 'mediaEmbed', 'underline', 'strikethrough', 'code', 'undo', 'redo'
            ],
        })
            .then(function (editor) {
                _this.data.editor = editor;
                editorData = editor.getData();
                //window.editor = editor;
                //var data = editor.getData();

                //_this.displayStatus(editor);
                // 这个地方加载了适配器
                editor.plugins.get('FileRepository').createUploadAdapter = function (loader){
                    return new UploadAdapter(loader);
                };

            })
            .catch(function (err) {
                console.error(err.stack);
            });

        $(".ck-content", _this.data.scope).addClass("");
    },
    bindEssay: function () {
        var _this = this;
        var helperOptions = {};
        if (_this.data.queryString && _this.data.queryString.id) {
            _this.data.essayId = _this.data.queryString.id;

            helperOptions = {
                url: basejs.requestDomain + "/essay/updateinfo?id=" + _this.data.queryString.id,
                type: 'GET',
                success: function (resultDto) {

                    if (resultDto.result) {
                        $("#btnAddPic", _this.data.scope)
                            .css("background-image", "url('" + basejs.requestDomain + "/" + resultDto.data.coverPath + "_260x194." + resultDto.data.coverExtension + "')")
                            .css("color", "white")
                            .attr("data-file-url", resultDto.data.coverPath)
                            .attr("data-file-extension", resultDto.data.coverExtension);
                        $("#isayTitle", _this.data.scope).val(resultDto.data.title);
                        if (resultDto.data.tagList.length > 0) {
                            $("input:radio[name='tagRadio'][value='" + resultDto.data.tagList[0].tagName + "']", _this.data.scope).attr("checked", "checked");
                        }

                        var categoryObj = $(".isay-info-category-span[data-val='" + resultDto.data.category + "']");
                        if (categoryObj && categoryObj.length > 0) {
                            categoryObj.addClass("isay-info-category-span-checked");
                        }
                        else {
                            $("#category", _this.data.scope).val(resultDto.data.category);
                        }

                        _this.data.editor.setData(resultDto.data.content);
                    }
                }
            };

            var helper = new httpHelper(helperOptions);
            helper.send();
        }
    },
    saveEssay: function (isPublish, data) {
        var _this = this;

        var btnSave = $('.btn-save', _this.data.scope);


        var title = $("#isayTitle", _this.data.scope).val();
        var tag = $("input:radio[name='tagRadio']:checked", _this.data.scope).val();

        var category = $(".isay-info-category-span-checked", _this.data.scope).attr("data-val");
        if (!category) {
            category = $("#category", _this.data.scope).val();
        }
        //var isOriginal = $("#isOriginal", _this.data.scope).prop('checked');




        var content = data;
        if (!category) {
            alert("请选择分类");
            return;
        }
        if (!tag) {
            alert("请填写标签");
            return;
        }
        if (!title) {
            alert("请填写标题");
            return;
        }
        if (!content) {
            alert("请填写内容");
            return;
        }


        btnSave.text('发布中...');

        var isAdd = !(_this.data.essayId && _this.data.essayId > 0);
        var helper = new httpHelper({
            url: basejs.requestDomain + "/essay/" + (isAdd ? "add" : "update"),
            type: 'POST',
            data: {
                essayEntity: {
                    id: _this.data.essayId,
                    title: title,
                    coverMediaType: "picture",
                    coverPath: $("#btnAddPic", _this.data.scope).attr("data-file-url"),
                    coverExtension: $("#btnAddPic", _this.data.scope).attr("data-file-extension"),
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
                    if (isAdd) {
                        _this.data.essayId = resultDto.data;
                    }
                    btnSave.text('发布成功');
                }
                else {
                    btnSave.css("background", "#ccc").css("color", "red").text(resultDto.message);
                }

            }
        });


        helper.send();


    }
};



function UploadAdapter(loader) {
    this.loader = loader;
}

UploadAdapter.prototype.upload = function () {
    var _this = this;

    return new Promise(function (resolve, reject) {

        _this.loader.file.then(function (file) {
            const formData = new FormData();
            formData.append('flie', file);

            var helper = new httpHelper({
                url: basejs.requestDomain + "/common/uploadfile",
                type: 'POST',
                async: false,
                data: formData,
                contentType: false,
                processData: false,
                success: function (resultDto) {

                    if (resultDto.result) {
                        resolve({
                            default: resultDto.data.url
                        });
                    }
                    else {
                        reject(resultDto.message);
                    }
                }
            });


            helper.send();
        });

    });
}

UploadAdapter.prototype.abort = function () {
}

$(function () {
    //编辑器
    editorjs.init();
});
