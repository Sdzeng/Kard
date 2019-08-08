// String
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments,
            json = args[0];
        return this.replace(/\#{([^{}]+)\}/gm, function (m, n) {
            n = n.trim();
            var index = Number(n),
                result;

            if (index >= 0) {
                result = typeof args[index] === 'function' ? args[index]() : args[index];
            }
            else {
                result = typeof json[n] === 'function' ? json[n]() : json[n];
            }
            return result !== undefined ? result : '';
        });
    }
}

 

// jQuery
if (!jQuery.fn.goToTop) {
    jQuery.fn.goToTop = function () {
        var _this = this;
        // 判断如果窗口滚动距离小于0，隐藏按钮
        if ($(window).scrollTop() < 500) {
            $(_this).hide();
        }

        // 窗口滚动时，判断当前窗口滚动距离
        $(window).scroll(function () {
            if ($(this).scrollTop() >= 500) {
                $(_this).fadeIn();
            } else {
                $(_this).fadeOut();
            }
        });

        // 给这个按钮绑定一个click事件
        _this.bind('click', function () {
            $('html ,body').animate({ scrollTop: 0 }, 500);
            return false;
        });
    };
}

if (!jQuery.fn.loadMore) {
    jQuery.fn.loadMore = function (bottom,loadingDataFn) {
        var _this = this;
        $(window).scroll(function () {
            //当时滚动条离底部350px时开始加载下一页的内容
            //$(window).scrollTop()滚动偏移量
            // $(window).height() 代表了当前可见区域的大小，
            // $(document).height() 代表了整个html文档的高度
            if ($(this).scrollTop() + $(this).height() > $(document).height() - bottom ) {
                loadingDataFn && loadingDataFn();
            }
        });
    };
}

//if (!String.prototype.htmlEncode) {
//    /*1.用浏览器内部转换器实现html转码*/
//    String.prototype.htmlEncode = function () {
//        //1.首先动态创建一个容器标签元素，如DIV
//        var temp = document.createElement("div");
//        //2.然后将要转换的字符串设置为这个元素的innerText(ie支持)或者textContent(火狐，google支持)
//        (temp.textContent != undefined) ? (temp.textContent = this) : (temp.innerText = this);
//        //3.最后返回这个元素的innerHTML，即得到经过HTML编码转换的字符串了
//        var output = temp.innerHTML;
//        temp = null;
//        return output;
//    };
//}

//if (!String.prototype.htmlDecode) {
//    /*2.用浏览器内部转换器实现html解码*/
//    String.prototype.htmlDecode = function () {
//        //1.首先动态创建一个容器标签元素，如DIV
//        var temp = document.createElement("div");
//        //2.然后将要转换的字符串设置为这个元素的innerHTML(ie，火狐，google都支持)
//        temp.innerHTML = this;
//        //3.最后返回这个元素的innerText(ie支持)或者textContent(火狐，google支持)，即得到经过HTML解码的字符串了。
//        var output = temp.innerText || temp.textContent;
//        temp = null;
//        return output;
//    };
//}