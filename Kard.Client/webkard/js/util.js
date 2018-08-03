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