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


    },
    template: {
        searchResultRow: ("<div class='search-result-warp'>" +
            "<div class='result-score'><div class='essay-score'>#{score}</div></div>" +

            "<div class='result-entity'>" +

            "<div class='result-info'>" +
   
            "<div class='result-header'><a href='#{essayDetailPage}' class='essay-title'>#{title}</a></div>" +
            "<div class='result-content'><a href='#{essayDetailPage}' class='essay-content'>#{content}</a></div>" +
            //"<div class='picture-body'><div class='picture-body-tag'>#{tagSpan}</div><div class='picture-body-num'><span class='essay-like-num'>#{ likeNum}</span><span class='essay-share-num'>#{shareNum}</span><span class='essay-browse-num'>#{browseNum}</span></div></div>" +//media.creatorNickName).substring(0, 6)
            //"<div class='picture-footer'><div class='picture-footer-author '><span class='essay-avatar'><img class='lazy' src='#{defaultAvatarPath}' data-original='#{ avatarCropPath }'   /> </span><span>#{creatorNickName} </span></div> <div><span class='essay-city'>#{location}</span><span>#{creationTime}</span></div></div>" +
            "<div class='result-footer'><span class='essay-nickname'><a>#{creatorNickName}</a></span> <span class='essay-creationtime'>#{creationTime}</span><span>#{browseNum}阅读</span>#{tagSpan}#{categorySpan}</div>" +
        
            "</div>" +

            "<div class='result-img'>" +
            "<img class='lazy' src='#{defaultPicturePath}' data-original='#{pictureCropPath}'    />" +
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

        var category = null;
        if (_this.data.queryString && _this.data.queryString.category && _this.data.queryString.category.length > 0) {
            category = decodeURI(_this.data.queryString.category); 
            $("#searchBox", _this.data.scope).val(category);
        }
        var httpPars = {
            url: basejs.requestDomain + "/home/essays",
            type: "GET",
            data: { category: (category||"精选"), pageIndex: 1, pageSize: 15 },
            success: function (resultDto) {
                //设置essays加载更多
                if (!resultDto.result) {
                    return;
                }
                if (resultDto.data.hasNextPage) {
                    _this.data.loadMorePars.offOn = true;
                    _this.data.loadMorePars.page++;
                    $loadMore.text("加载更多");
                }
                else {
                    _this.data.loadMorePars.offOn = false;
                    $loadMore.text("已经是底部");
                }
                _this.bindResultInfo(resultDto.data.essayList);
                //图片懒加载
                $imageLazy = $(".search-result-warp img.lazy", _this.data.scope);
                basejs.lazyInof($imageLazy);
                $imageLazy.removeClass("lazy");
            },
            error: function () {
                _this.data.loadMorePars.offOn = true;

                $(".search-result-left", _this.data.scope).empty();
            }
        };

        var essaysHttpHelper = new httpHelper(httpPars);
        essaysHttpHelper.send();

        $(".section-style-title-big>span", _this.data.scope).click(function () {

            _this.data.loadMorePars.offOn = false;
            _this.data.loadMorePars.page = 1;
            httpPars.data.category = $("#searchBox", _this.data.scope).val();
            httpPars.data.pageIndex = _this.data.loadMorePars.page;
            $loadMore.text("加载中...");
            essaysHttpHelper = new httpHelper(httpPars);
            essaysHttpHelper.send();
        });


        $loadMore.loadMore(50, function () {
            //这里用 [ off_on ] 来控制是否加载 （这样就解决了 当上页的条件满足时，一下子加载多次的问题啦）
            if (_this.data.loadMorePars.offOn) {
                _this.data.loadMorePars.offOn = false;

                httpPars.data.category = $("#searchBox", _this.data.scope).val();
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
                var current = parseInt(index) + 1;
                var topMediaDto = data[index];
                var essayDetailPage = "/essay-detail.html?id=" + topMediaDto.id;
                var defaultPicturePath = "/image/default-picture_100x100.jpg";
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
                    content: topMediaDto.content,
                
                    score: topMediaDto.score,
                    creatorNickName: topMediaDto.creatorNickName,

                    browseNum: basejs.getNumberDiff(topMediaDto.browseNum),
                    tagSpan: tagSpan,
                    categorySpan:categorySpan,

                    creationTime: basejs.getDateDiff(basejs.getDateTimeStamp(topMediaDto.creationTime))

                });



                resultHtml += resultRowHtml;
                resultRowHtml = "";

            }

            $(".search-result-left", _this.data.scope).append(resultHtml);
        }


    }
};

$(function () {
    //菜单
    topMenu.bindMenu();
    topMenu.logout();
    topMenu.authTest();

    searchjs.init();
});