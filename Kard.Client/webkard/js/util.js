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

if (!jQuery.fn.loadMore) {
    jQuery.fn.loadMore = function (bottom,loadingDataFn) {
        var _this = this;
        $(window).scroll(function () {
            //��ʱ��������ײ�350pxʱ��ʼ������һҳ������
            //$(window).scrollTop()����ƫ����
            // $(window).height() �����˵�ǰ�ɼ�����Ĵ�С��
            // $(document).height() ����������html�ĵ��ĸ߶�
            if ($(this).scrollTop() + $(this).height() > $(document).height() - bottom ) {
                loadingDataFn && loadingDataFn();
            }
        });
    };
}

//if (!String.prototype.htmlEncode) {
//    /*1.��������ڲ�ת����ʵ��htmlת��*/
//    String.prototype.htmlEncode = function () {
//        //1.���ȶ�̬����һ��������ǩԪ�أ���DIV
//        var temp = document.createElement("div");
//        //2.Ȼ��Ҫת�����ַ�������Ϊ���Ԫ�ص�innerText(ie֧��)����textContent(�����google֧��)
//        (temp.textContent != undefined) ? (temp.textContent = this) : (temp.innerText = this);
//        //3.��󷵻����Ԫ�ص�innerHTML�����õ�����HTML����ת�����ַ�����
//        var output = temp.innerHTML;
//        temp = null;
//        return output;
//    };
//}

//if (!String.prototype.htmlDecode) {
//    /*2.��������ڲ�ת����ʵ��html����*/
//    String.prototype.htmlDecode = function () {
//        //1.���ȶ�̬����һ��������ǩԪ�أ���DIV
//        var temp = document.createElement("div");
//        //2.Ȼ��Ҫת�����ַ�������Ϊ���Ԫ�ص�innerHTML(ie�������google��֧��)
//        temp.innerHTML = this;
//        //3.��󷵻����Ԫ�ص�innerText(ie֧��)����textContent(�����google֧��)�����õ�����HTML������ַ����ˡ�
//        var output = temp.innerText || temp.textContent;
//        temp = null;
//        return output;
//    };
//}