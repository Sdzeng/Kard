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

        $(".isay-info-tag-span", _this.data.scope).click(function () {
            $(".isay-info-tag-span-checked", _this.data.scope).removeClass("isay-info-tag-span-checked");
            $("#category", _this.data.scope).val("");
            $(this).addClass("isay-info-tag-span-checked");

        });

        $("#category", _this.data.scope).change(function () {
            $(".isay-info-tag-span-checked", _this.data.scope).removeClass("isay-info-tag-span-checked");
        });

        $(".btn-save", _this.data.scope).click(function () {
           
            
            _this.saveEssay(false);
        });

        //$(".btn-publish", _this.data.scope).click(function () {
        //    _this.saveEssay(true,_this.data.editor.getData());
        //});

        _this.bindEditor();
        
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
        // ClassicEditor.create(document.querySelector('#editor'), {
        //     //language: 'zh-cn',
        //     //plugins: [Markdown],
        //     fontSize: {
        //         options: [
        //             9,
        //             11,
        //             13,
        //             15,
        //             17,
        //             19,
        //             21
        //         ]
        //     },
        //     fontFamily: {
        //         options: [
        //             '默认,default',
        //             'Ubuntu, Arial, sans-serif',
        //             'Ubuntu Mono2, Courier New, Courier, monospace'
        //         ]
        //     },
        //     highlight: {
        //         options: [
        //             {
        //                 model: 'greenMarker',
        //                 class: 'marker-green',
        //                 title: '绿色标记',
        //                 color: 'rgb(25, 156, 25)',
        //                 type: 'marker'
        //             },
        //             {
        //                 model: 'yellowMarker',
        //                 class: 'marker-yellow',
        //                 title: '黄色标记',
        //                 color: '#cac407',
        //                 type: 'marker'
        //             },
        //             {
        //                 model: 'redPen',
        //                 class: 'pen-red',
        //                 title: '红色钢笔',
        //                 color: 'hsl(343, 82%, 58%)',
        //                 type: 'pen'
        //             }
        //         ]

        //     },
        //     //ckfinder: {
        //     //    uploadUrl: basejs.requestDomain + "/common/editoruploadfile?command=QuickUpload&type=Files&responseType=json",
        //     //    options: {
        //     //    }
        //     //},
        //     //autosave: {
        //     //    save(editor) {
        //     //        // The saveData() function must return a promise
        //     //        // which should be resolved when the data is successfully saved.
        //     //        var data = editor.getData();


        //     //        return new Promise(resolve => {
        //     //            if (data) {
        //     //                _this.autoSaveData(data);
        //     //            }
        //     //            resolve();
        //     //        });
        //     //    }

        //     //},
        //     toolbar: ['heading', '|',
        //         'bold', 'italic', 'link', 'bulletedList', 'numberedList', '|',
        //         'fontSize', 'fontFamily', 'subscript', 'superscript', 'highlight', 'alignment:left', 'alignment:right', 'alignment:center', 'alignment:justify', '|',
        //         'imageUpload', 'blockQuote', 'insertTable', 'mediaEmbed', 'underline', 'strikethrough', 'code', 'undo', 'redo'
        //     ],
        // })
        //     .then(function (editor) {
        //         _this.data.editor = editor;
        //         editorData = editor.getData();
        //         //window.editor = editor;
        //         //var data = editor.getData();

        //         //_this.displayStatus(editor);
        //         // 这个地方加载了适配器
        //         editor.plugins.get('FileRepository').createUploadAdapter = function (loader){
        //             return new UploadAdapter(loader);
        //         };

        //     })
        //     .catch(function (err) {
        //         console.error(err.stack);
        //     });

        tinymce.init({
            selector: '#tinyeditor',
            language: 'zh_CN',
            // plugins: 'print preview fullpage powerpaste searchreplace autolink directionality advcode visualblocks visualchars fullscreen image link media mediaembed template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount tinymcespellchecker a11ychecker imagetools textpattern help formatpainter permanentpen pageembed tinycomments mentions linkchecker', 
            // toolbar: 'formatselect | bold italic strikethrough forecolor backcolor permanentpen formatpainter | link image media pageembed | alignleft aligncenter alignright alignjustify | numlist bullist outdent indent | removeformat | addcomment',
            plugins: 'print autoresize preview fullpage paste searchreplace autolink directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking emoticons anchor toc insertdatetime advlist lists wordcount  imagetools textpattern help  link',
            toolbar: 'formatselect | bold italic strikethrough forecolor backcolor permanentpen formatpainter | link image media pagebreak table | codesample code | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent | removeformat ',
            //imagetools_toolbar: "rotateleft rotateright | flipv fliph | editimage imageoptions",
            min_height:600,
            relative_urls : false, 
            remove_script_host : false,
            //automatic_uploads: true,
            //images_upload_url: basejs.requestDomain + "/common/uploadfile",
            //images_upload_credentials: true,
            images_upload_base_path: basejs.requestDomain,
            automatic_uploads: true,
            images_upload_handler: function (blobInfo, success, failure) {
                const formData = new FormData();
                formData.append('flie', blobInfo.blob());

                var helper = new httpHelper({
                    url: basejs.requestDomain + "/common/uploadfile",
                    type: 'POST',
                    async: false,
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (resultDto) {

                        if (resultDto.result) {
                            success(basejs.cdnDomain+"/"+resultDto.data.fileUrl+"."+resultDto.data.fileExtension);
                        }
                        else {
                            failure(resultDto.message);
                        }
                    }
                });

                helper.send();

            },
            templates: [
                { title: 'Test template 1', content: 'Test 1' },
                { title: 'Test template 2', content: 'Test 2' }
            ],

            content_css: [

                '/plugins/tinymce/js/tinymce/css/codepen.min.css',
                '/css/tinymce_editor.css'
            ],
            content_style: [
                'body{padding:20px; margin:auto;font-size:16px;font-family:"Helvetica Neue",Helvetica,Arial,sans-serif; line-height:1.3; letter-spacing: -0.03em;color:#222} h1,h2,h3,h4,h5,h6 {font-weight:400;margin-top:1.2em} h1 {} h2{} .tiny-table {width:100%; border-collapse: collapse;} .tiny-table td, th {border: 1px solid #555D66; padding:10px; text-align:left;font-size:16px;font-family:"Helvetica Neue",Helvetica,Arial,sans-serif; line-height:1.6;} .tiny-table th {background-color:#E2E4E7}'
            ],
            init_instance_callback: function (editor) {
                _this.data.editor=editor;
                // editor.on('SetContent', function (e) {
                //     console.log(e.content);
                //   });
                //editor.setContent("<div style='display: flex;flex-direction:row;justify-content: center;align-items: center;'><h5>加载内容中...</h5><div>");
                _this.bindEssay();
              
            },
            setup: function(ed) {
                
             
            //    ed.ui.registry.addContextToolbar('imagealignment', {
            //     predicate: function (node) {
            //       return node.nodeName.toLowerCase() === 'img'
            //     },
            //     items: 'alignleft aligncenter alignright | rotateleft rotateright | flipv fliph | editimage imageoptions',
            //     position: 'node',
            //     scope: 'node'
            //   });
          
             
                 
            //   ed.ui.registry.addContextToolbar('textselection', {
            //     predicate: function (node) {
            //       return !ed.selection.isCollapsed();
            //     },
            //     items: 'bold italic blockquote | image | codesample',
            //     position: 'selection',
            //     scope: 'node'
            //   });

             }
             
        });

      
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
                            .css("background-image", "url('" + basejs.requestDomain + "/" + resultDto.data.essay.coverPath + "_260x194." + resultDto.data.essay.coverExtension + "')")
                            .css("color", "white")
                            .attr("data-file-url", resultDto.data.essay.coverPath)
                            .attr("data-file-extension", resultDto.data.essay.coverExtension);
                        $("#isayTitle", _this.data.scope).val(resultDto.data.essay.title);
                        if (resultDto.data.tagList.length > 0) {
                            $("input:radio[name='tagRadio'][value='" + resultDto.data.tagList[0].tagName + "']", _this.data.scope).attr("checked", "checked");
                        }

                        var tagObj = $(".isay-info-tag-span[data-val='" + resultDto.data.tagList[0].tagName + "']");
                        if (tagObj && tagObj.length > 0) {
                            tagObj.addClass("isay-info-tag-span-checked");
                        }
                        else {
                            $("#tag", _this.data.scope).val(resultDto.data.tagList[0].tagName);
                        }
                        
                        $('#isPublish', _this.data.scope).attr("checked", resultDto.data.essay.isPublish);
                        tinymce.activeEditor.setContent(resultDto.data.essayContent.content);
                    }
                }
            };

            var helper = new httpHelper(helperOptions);
            helper.send();
        }
    },
    saveEssay: function (isPublish) {
        var _this = this;
       
        // Save contents using some XHR call
        var data= _this.data.editor.getBody().innerHTML;

        var btnSave = $('.btn-save', _this.data.scope);


        var title = $("#isayTitle", _this.data.scope).val();
        var category = $("input:radio[name='categoryRadio']:checked", _this.data.scope).val();

        var tag = $(".isay-info-tag-span-checked", _this.data.scope).attr("data-val");
        if (!tag) {
            tag = $("#tag", _this.data.scope).val();
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


        btnSave.text('保存中...');

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
                    isPublish: $("#isPublish", _this.data.scope).prop("checked"),
                    score: $("#score", _this.data.scope).val()
                   
                },
                essayContentEntity: {
                    essayId: _this.data.essayId,
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
                    btnSave.text('保存成功');
                }
                else {
                    btnSave.css("background", "#ccc").css("color", "red").text(resultDto.message);
                }

            }
        });


        helper.send();


    }
};



// function UploadAdapter(loader) {
//     this.loader = loader;
// }

// UploadAdapter.prototype.upload = function () {
//     var _this = this;

//     return new Promise(function (resolve, reject) {

//         _this.loader.file.then(function (file) {
//             const formData = new FormData();
//             formData.append('flie', file);

//             var helper = new httpHelper({
//                 url: basejs.requestDomain + "/common/uploadfile",
//                 type: 'POST',
//                 async: false,
//                 data: formData,
//                 contentType: false,
//                 processData: false,
//                 success: function (resultDto) {

//                     if (resultDto.result) {
//                         resolve({
//                             default: resultDto.data.url
//                         });
//                     }
//                     else {
//                         reject(resultDto.message);
//                     }
//                 }
//             });


//             helper.send();
//         });

//     });
// }

// UploadAdapter.prototype.abort = function () {
// }

$(function () {
    //编辑器
    editorjs.init();
});
