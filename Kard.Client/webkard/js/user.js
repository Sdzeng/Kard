var usercenterjs = {
    data: {
        scope: $("#userPage"),
        queryString: basejs.getQueryString(),
        loadMorePars: {
            //设置加载更多
            offOn: false,
            page: 1

        }
    },
    init: function () {
        var _this = this;
        _this.userCover();
        
        _this.bindMenu();
        _this.bindResult();
      
        $('.go-to-top', _this.data.scope).goToTop();
    },
    template: {
        
        essay:  ("<div class='result-warp essay-warp'>"+
        "<div class='result-info' style='width:890px;'>" +
        "<div class='essay-header'><a class='essay-title' href='#{essayDetailPage}'>#{essayTitle}</a></div>" +
        "<div class='essay-content'><a  href='#{essayDetailPage}'>#{essaySubContent}</a></div>" +
        "<div class='essay-footer'>" +
        "<span>#{location}</span> " +
        "<span>#{creationTime}</span> " +
        "<span>#{browseNum}阅读</span>" +
        "<span>#{likeNum}喜欢</span>" +
        "<span>#{commentNum}评论</span>" +
        "<span class='essay-category'>#{category}</span>" +
        "</div>" +
        "</div>" +
        "<div class='result-cover'><a href='#{essayDetailPage}'><img class='lazy' src='#{defaultPicturePath}' data-original='#{pictureCropPath}'></a></div>" +
        "</div>"),
        essayFans: ("<div class='fans-warp'>" +
        "<a class='result-avatar' href='/user.html?id=#{kuserId}'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{avatarCropPath}' /></a>" +
        "<div class='result-nickname'>#{kuserNickName}</div>" +
        "</div>")
    },
    userCover: function () {

        var _this = this;

        //设置首页封面
        var helper = new httpHelper({
            url: basejs.requestDomain + "/user/cover",
            type: "GET",
            data: {userId: _this.data.queryString.id},
            success: function (resultDto) {
                var data = resultDto.data;
                //data = JSON.parse(data);
                if (!data) {
                    return;
                }

                $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + (data.coverPath || "") + ")");
                //$(".essay-content>blackquote>q").text("测试");
                $(".author-txt-name>span:eq(0)", _this.data.scope).text(data.nickName || "");


                


                var avatarCropPath = "";
                if(data.avatarUrl.indexOf("http") >= 0 ) { 
                    avatarCropPath=data.avatarUrl;
                }
                else
                {
                    var avatarArr = data.avatarUrl.split('.');
                    avatarCropPath=basejs.cdnDomain + "/" + avatarArr[0] + "_90x90." + avatarArr[1];
                }
                

               
                $(".author-avatar>img", _this.data.scope).attr("data-original",avatarCropPath);
                var $userCenterAuthorTxt = $(".author-txt", _this.data.scope);
                var $userCenterAuthorTxtName = $userCenterAuthorTxt.children(".author-txt-name");
                $userCenterAuthorTxtName.children("span:eq(0)").text(data.nickName);
                $userCenterAuthorTxtName.children("span:eq(1)").text(data.city);
                $userCenterAuthorTxt.children(".author-txt-introduction").text(data.introduction);
                var $userCenterAuthorTxtNum = $userCenterAuthorTxt.children(".author-txt-num");
                $userCenterAuthorTxtNum.children("span:eq(0)").text(data.followNum + "关注");
                $userCenterAuthorTxtNum.children("span:eq(1)").text(data.fansNum + "粉丝");
                $userCenterAuthorTxtNum.children("span:eq(2)").text("获得" + data.likeNum + "个喜欢");

                //图片懒加载
                $imageLazy = $(".author-avatar img.lazy", _this.data.scope);
                basejs.lazyInof($imageLazy);
            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
        helper.send();


    },


 
    bindMenu: function () {
        var _this = this;

        $(".menu li", _this.data.scope).click(function () {
           
            $(".menu-active", _this.data.scope).removeClass("menu-active");
            $(this).addClass("menu-active");
            _this.data.loadMorePars.offOn=false;
            _this.data.loadMorePars.page=1;
            $("#my-content", _this.data.scope).empty();
            _this.bindResult();
        });

    },
    bindResult: function () {
        var _this = this;

        var $loadMore = $(".load-more>span", _this.data.scope);
        $loadMore.text("加载中...");

        function successFunc(resultDto){
            //图片懒加载
            $imageLazy = $("#my-content img.lazy", _this.data.scope);
            basejs.lazyInof($imageLazy);
            $imageLazy.removeClass("lazy");
            
            if (resultDto.data.hasNextPage) {
                _this.data.loadMorePars.offOn = true;
                _this.data.loadMorePars.page++;
                $loadMore.text("加载更多");
            }
            else {
                _this.data.loadMorePars.offOn = false;
                $loadMore.text("已经是底部");
            }
        };

        function errorFunc(){
            _this.data.loadMorePars.offOn = true;

            $("#my-content", _this.data.scope).empty();
        }
        
        var opt=$(".menu-active", _this.data.scope).attr("data-opt");
      
        var httpPars=null;
        switch(opt){
            
           case "essay":httpPars=_this.getEssayHttpPars(successFunc,errorFunc);break;
           case "fans":httpPars=_this.getFansHttpPars(successFunc,errorFunc);break;
          

        }

        var helper=new httpHelper(httpPars);
        helper.send();

        $loadMore.loadMore(50, function () {
           
            //这里用 [ off_on ] 来控制是否加载 （这样就解决了 当上页的条件满足时，一下子加载多次的问题啦）
            if (_this.data.loadMorePars.offOn) {
                _this.data.loadMorePars.offOn = false;
                 
                var loadMoreOpt=$(".menu-active", _this.data.scope).attr("data-opt");
      
                var loadMoreHttpPars=null;
                switch(loadMoreOpt){
                  
                   case "essay":loadMoreHttpPars=_this.getEssayHttpPars(successFunc,errorFunc);break;
                   case "fans":httpPars=_this.getFansHttpPars(successFunc,errorFunc);break;
                   
                }
             
                loadMoreHttpPars.data.pageIndex = _this.data.loadMorePars.page;
                $loadMore.text("加载中...");
                helper = new httpHelper(loadMoreHttpPars);
                helper.send();
            }
        });
    },
   
    getEssayHttpPars: function (successFunc,errorFunc) {
        var _this = this;

        //设置httpHelper
        var httpPars = {
            url: basejs.requestDomain + "/user/essay",
            type: "GET",
            data: { userId: _this.data.queryString.id, pageIndex: 1, pageSize: 10 },
            success: function (resultDto) {
                var data = resultDto.data.essayList;
             
                if (!data) {
                    return;
                }
                var essayHtml = "";


                for (var index in data) {
                   
                    var essay = data[index];
                    // var avatarArr = news.dto.avatarUrl.split('.');
                    // var avatarCropPath = basejs.cdnDomain + "/" + avatarArr[0] + "_40x40." + avatarArr[1];
                    var essayDetailPage = "/" + essay.pageUrl;
                    var defaultPicturePath = "/image/default-picture_100x100.jpg";
                    var pictureCropPath = "";
                    switch (essay.coverMediaType) {
                        case "picture": pictureCropPath = basejs.cdnDomain + "/" + essay.coverPath + "_100x100." + essay.coverExtension; break;
                        case "video": pictureCropPath = basejs.cdnDomain + "/" + essay.coverPath + "_100x100.jpg"; break;
                    }

                    essayHtml += _this.template.essay.format({

                        essayDetailPage: essayDetailPage,
                        essayTitle: essay.title,
                     
                        essaySubContent: essay.subContent,
                        location: essay.location,
                        browseNum: basejs.getNumberDiff(essay.browseNum),
                        likeNum: basejs.getNumberDiff(essay.likeNum),
                        commentNum: basejs.getNumberDiff(essay.commentNum),
                        category: essay.category,
                        //score: essay.score,
                        creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(essay.creationTime)),
                        defaultPicturePath: defaultPicturePath,
                        pictureCropPath: pictureCropPath
                    });
                            
                }
                if(httpPars.data.pageIndex==1){
                   $("#my-content", _this.data.scope).empty();
                   
                }

                $("#my-content", _this.data.scope).append(essayHtml);
               
                successFunc&&successFunc(resultDto);

               
            },
            error: function () {
                errorFunc&&errorFunc();
               
            }
        };

        return httpPars;
    },
    getFansHttpPars: function (successFunc,errorFunc) {
        var _this = this;

        //设置httpHelper
        var httpPars = {
            url: basejs.requestDomain + "/user/fans",
            type: "GET",
            data: {  userId: _this.data.queryString.id,pageIndex: 1, pageSize: 10 },
            success: function (resultDto) {
                var data = resultDto.data.fansList;
             
                if (!data) {
                    return;
                }
                var fansHtml = "";

            

                for (var index in data) {
                   
                    var fans = data[index];
                 
                    var avatarCropPath = "";
                    if(fans.kuserAvatarUrl.indexOf("http") >= 0 ) { 
                        avatarCropPath=fans.kuserAvatarUrl;
                    }
                    else
                    {
                        var avatarArr = fans.kuserAvatarUrl.split('.');
                        avatarCropPath=basejs.cdnDomain + "/" + avatarArr[0] + "_80x80." + avatarArr[1];
                    }
                    
                    fansHtml += _this.template.essayFans.format({
                        kuserId:fans.kuserId,
                        avatarCropPath:  avatarCropPath,
                        defaultAvatarPath:basejs.defaults.avatarPath,
                        kuserNickName:fans.kuserNickName
                    });
                            
                }
                if(httpPars.data.pageIndex==1){
                   $("#my-content", _this.data.scope).empty();
                }
                $("#my-content", _this.data.scope).append(fansHtml);
               
                successFunc&&successFunc(resultDto);

               
            },
            error: function () {
                errorFunc&&errorFunc();
               
            }
        };

        return httpPars;
    }




};

$(function () {
    //菜单
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    usercenterjs.init();

});
