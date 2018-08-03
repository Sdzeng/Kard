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
        // �ж�������ڹ�������С��0�����ذ�ť
        if ($(window).scrollTop() < 500) {
            $(_this).hide();
        }

        // ���ڹ���ʱ���жϵ�ǰ���ڹ�������
        $(window).scroll(function () {
            if ($(this).scrollTop() >= 500) {
                $(_this).fadeIn();
            } else {
                $(_this).fadeOut();
            }
        });

        // �������ť��һ��click�¼�
        _this.bind('click', function () {
            $('html ,body').animate({ scrollTop: 0 }, 500);
            return false;
        });
    };
}