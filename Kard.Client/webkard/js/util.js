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