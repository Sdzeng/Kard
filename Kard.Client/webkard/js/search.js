var searchjs = {
    data: {
        scope: $("#searchPage"),
        queryString: basejs.getQueryString(),
        loadMorePars: {
            //设置essays加载更多
            offOn: false,
            page: 1

        }
    },
    init: function () {
        var _this = this;
        _this.bindCover();
        _this.bindSearchResult();
        _this.bindRecommendList();

        $('.go-to-top', _this.data.scope).goToTop();
    },
    template: {
        searchResultRow: ("<div class='search-result-warp'>" +
            "<div class='result-score'><div class='essay-score'>#{score}</div><div class='essay-score-head-count'>#{likeNum}</div></div>" +

            "<div class='result-entity'>" +

            "<div class='result-info'>" +

            "<div class='result-header'><a href='#{essayDetailPage}' class='essay-title'>#{title}</a></div>" +
            "<div class='result-content'><a href='#{essayDetailPage}' class='essay-content'>#{subContent}</a></div>" +
            //"<div class='picture-body'><div class='picture-body-tag'>#{tagSpan}</div><div class='picture-body-num'><span class='essay-like-num'>#{ likeNum}</span><span class='essay-share-num'>#{shareNum}</span><span class='essay-browse-num'>#{browseNum}</span></div></div>" +//media.creatorNickName).substring(0, 6)
            //"<div class='picture-footer'><div class='picture-footer-author '><span class='essay-avatar'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{ avatarCropPath }'   /> </span><span>#{creatorNickName} </span></div> <div><span class='essay-city'>#{location}</span><span>#{creationTime}</span></div></div>" +
            "<div class='result-footer'><span class='essay-nickname'><a>#{creatorNickName}</a></span> <span class='essay-creationtime'>#{creationTime}</span><span>#{browseNum}阅读</span>#{tagSpan}#{categorySpan}</div>" +

            "</div>" +

            "<div class='result-img'>" +
            "<a href='#{essayDetailPage}'><img class='lazy' src='#{defaultPicturePath}' data-original='#{pictureCropPath}'    /></a>" +
            "</div>" +

            "</div >" +
            "</div >")
    },
    bindCover: function () {
        var _this = this;

        //设置首页封面
        topCover.getHomeCover(function (resultDto) {
            var data = resultDto.data;
            //data = JSON.parse(data);
            if (!data) {
                return;
            }
            $(".bg-default", _this.data.scope).css("background-image", "linear-gradient(to bottom, rgba(0, 0, 0, 0.2) 0%, rgba(0, 0, 0, 0.2) 100%),url(" + basejs.cdnDomain + "/" + data.essayCoverPath + (data.essayCoverMediaType == "picture" ? "." + data.essayCoverExtension : ".jpg") + ")");
        });
    },
    bindSearchResult: function () {
        var _this = this;

        var $loadMore = $(".load-more>span", _this.data.scope);
        $loadMore.text("加载中...");

        var keyword = null;
        if (_this.data.queryString && _this.data.queryString.keyword && _this.data.queryString.keyword.length > 0) {
            keyword = decodeURI(_this.data.queryString.keyword);
            $("#searchBox", _this.data.scope).val(keyword);
        }
        var httpPars = {
            url: basejs.requestDomain + "/home/essays",
            type: "GET",
            data: { keyword: keyword, pageIndex: 1, pageSize: 10, orderBy: "" },
            success: function (resultDto) {
                //设置essays加载更多
                if (!resultDto.result) {
                    return;
                }

                _this.bindResultInfo(resultDto.data.essayList);
                //图片懒加载
                $imageLazy = $(".search-result-warp img.lazy", _this.data.scope);
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
            },
            error: function () {
                _this.data.loadMorePars.offOn = true;

                $(".search-result-left", _this.data.scope).empty();
            }
        };

        var essaysHttpHelper = new httpHelper(httpPars);
        essaysHttpHelper.send();

        $(".btn-search", _this.data.scope).click(function () {

            _this.data.loadMorePars.offOn = false;
            _this.data.loadMorePars.page = 1;
            httpPars.data.keyword = $("#searchBox", _this.data.scope).val();
            httpPars.data.pageIndex = _this.data.loadMorePars.page;

            $loadMore.text("加载中...");
            essaysHttpHelper = new httpHelper(httpPars);
            essaysHttpHelper.send();
        });

        $("body").keydown(function () {
            if (event.keyCode == "13") {//keyCode=13是回车键；数字不同代表监听的按键不同
                $(".btn-search", _this.data.scope).click();
            }
        });


        $loadMore.loadMore(50, function () {
            //这里用 [ off_on ] 来控制是否加载 （这样就解决了 当上页的条件满足时，一下子加载多次的问题啦）
            if (_this.data.loadMorePars.offOn) {
                _this.data.loadMorePars.offOn = false;

                httpPars.data.keyword = $("#searchBox", _this.data.scope).val();
                httpPars.data.pageIndex = _this.data.loadMorePars.page;
                $loadMore.text("加载中...");
                essaysHttpHelper = new httpHelper(httpPars);
                essaysHttpHelper.send();
            }
        });

    },
    bindResultInfo: function (data) {

        var _this = this;
        var titleTagArr = [];
        var resultHtml = "";

        if (data) {
            var resultRowHtml = "";

            for (var index in data) {
                var topMediaDto = data[index];
                var essayDetailPage = "/" + topMediaDto.pageUrl;
                var defaultPicturePath = basejs.cdnDomain +"/image/default-picture_100x100.jpg";
                var pictureCropPath = "";
                switch (topMediaDto.coverMediaType) {
                    case "picture": pictureCropPath = basejs.cdnDomain + "/" + topMediaDto.coverPath + "_100x100." + topMediaDto.coverExtension; break;
                    case "video": pictureCropPath = basejs.cdnDomain + "/" + topMediaDto.coverPath + "_100x100.jpg"; break;
                }


                var tagSpan = "";
                if (topMediaDto.tagList && topMediaDto.tagList.length > 0) {
                    tagSpan += "<span class='essay-tag' title='" + topMediaDto.tagList[0].tagName + "'>" + topMediaDto.tagList[0].tagName + "</span>";//(topMediaDto.tagList[0].tagName.length > 4 ? topMediaDto.tagList[0].tagName.substr(0, 3) + "..." : topMediaDto.tagList[0].tagName);
                    titleTagArr.push(topMediaDto.tagList[0].tagName);
                }
                var categorySpan = "<span class='essay-category' title='" + topMediaDto.category + "'>" + topMediaDto.category + "</span>";

                resultRowHtml += _this.template.searchResultRow.format({
                    essayDetailPage: essayDetailPage,
                    defaultPicturePath: defaultPicturePath,
                    pictureCropPath: pictureCropPath,

                    title: topMediaDto.title,
                    subContent: topMediaDto.subContent,

                    score: topMediaDto.score,
                    likeNum:(topMediaDto.likeNum>0?topMediaDto.likeNum + "喜欢":""),
                    creatorNickName: topMediaDto.creatorNickName,

                    browseNum: basejs.getNumberDiff(topMediaDto.browseNum),
                    tagSpan: tagSpan,
                    categorySpan: categorySpan,

                    creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(topMediaDto.creationTime))

                });



                resultHtml += resultRowHtml;
                resultRowHtml = "";

            }

            if (_this.data.loadMorePars.page == 1) {
                $(".search-result-left", _this.data.scope).html(resultHtml);
            } else {
                $(".search-result-left", _this.data.scope).append(resultHtml);
            }
        }


    },
    bindRecommendList: function () {
        var _this=this;
        var httpPars = {
            url: basejs.requestDomain + "/home/essays",
            type: "GET",
            data: { keyword: "", pageIndex: 1, pageSize: 10, orderBy: "choiceness" },
            success: function (resultDto) {

                if (!resultDto.result) {
                    return;
                }
                if ((!resultDto.data)||(!resultDto.data.essayList)||resultDto.data.essayList.length<=0) {
                    return;
                }
                var data=resultDto.data.essayList;
                var essayRecommendAObj = $(".essay-recommend-a", _this.data.scope);

                for (var index in data) {
                    
                    var topMediaDto = data[index];
                     
                    var essayDetailPage = "/" + topMediaDto.pageUrl;
                    essayRecommendAObj.append("<a href='" + essayDetailPage + "' title='"+topMediaDto.title+"'><span class='recommend-list-number'>" + (parseInt(index)+1)+"</span>"+topMediaDto.title + "</a>");
                }
            },
            error: function () {
                _this.data.loadMorePars.offOn = true;

                $(".search-result-left", _this.data.scope).empty();
            }
        };

        var essaysHttpHelper = new httpHelper(httpPars);
        essaysHttpHelper.send();
    }
};

$(function () {
    //菜单
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    searchjs.init();
});