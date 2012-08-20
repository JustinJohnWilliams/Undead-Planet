jQuery.extend({
    isDate: function(aspNetDate) {
        if (aspNetDate) {
            return aspNetDate.toString().indexOf("Date(", 0) != -1;
        }
        else {
            return false;
        }
    },

    toDate: function(aspNetDate) {
        return new Date(parseInt(aspNetDate.replace("/Date(", "").replace(")/", ""), 10));
    },

    newDate: function(year, month, day, hour, minutes, amOrPm) {
        return month + "/" + day + "/" + year + " " + hour + ":" + minutes + " " + amOrPm; //"11/30/2009 12:31 AM";
    },

    formatDate: function(date) {
        return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear();
    },

    formatTime: function(date) {
        var hour = date.getHours();
        var minute = date.getMinutes();
        var ap = "AM";
        if (hour > 11) { ap = "PM"; }
        if (hour > 12) { hour = hour - 12; }
        if (hour == 0) { hour = 12; }
        if (hour < 10) { hour = "0" + hour; }
        if (minute < 10) { minute = "0" + minute; }
        var timeString = hour +
                    ':' +
                    minute +
                    " " +
                    ap;
        return timeString;
    }
});

jQuery.fn.clickOnEnterPressed = function(elementToClick) {
    $(this).keypress(function(e) { if (e.which == 13) { $(elementToClick).click(); } });
}