if (typeof String.prototype.format != 'function') {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{([^{}]*)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : ''
            ;
        });
    };
}

if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (str) {
        return this.slice(0, str.length) == str;
    };
}

if (typeof String.prototype.endsWith != 'function') {
    String.prototype.endsWith = function (str) {
        return this.slice(-str.length) == str;
    };
}

if (typeof String.linkify != 'function') {
    String.prototype.linkify = function () {        
        var urlPattern = /\b(?:https?|ftp):\/\/[a-z0-9-+&@#\/%?=~_|!:,.;]*[a-z0-9-+&@#\/%=~_|]/gim, // http://, https://, ftp://
            pseudoUrlPattern = /(^|[^\/])(www\.[\S]+(\b|$))/gim, // www. sans http:// or https://        
            emailAddressPattern = /[\w.]+@[a-zA-Z_-]+?(?:\.[a-zA-Z]{2,6})+/gim, // Email addresses
            imgPattern = /\.(?:jpe?g|gifv?|png)$/i;

        return this
            .replace(urlPattern, function (match) {
                if (imgPattern.test(match)) { // it's a link to an image
                    return '<a href="{0}" target="_blank"><img src="{0}" /></a>'.format(match);
                } else { // it's a normal link
                    return '<a href="{0}" target="_blank">{0}</a>'.format(match);
                }
            })
            .replace(pseudoUrlPattern, function (match) {
                if (imgPattern.test(match)) { // it's a link to an image
                    return '<a href="http://{0}" target="_blank"><img src="{0}" /></a>'.format(match);
                } else { // it's a normal link
                    return '<a href="http://{0}" target="_blank">{0}</a>'.format(match);
                }
            })
            .replace(emailAddressPattern, '<a href="mailto:$&" target="_blank">$&</a>');
    };
}

if (typeof Date.convertToLocalDate != 'function') {
    Date.prototype.convertToLocalDate = function () {
        // this assumes time is in UTC, will not work for developing locally
        var date = new Date(this.getTime() + this.getTimezoneOffset() * 60 * 1000),
            offset = this.getTimezoneOffset() / 60,
            hours = this.getHours();

        date.setHours(hours - offset);
        return date;
    }
}