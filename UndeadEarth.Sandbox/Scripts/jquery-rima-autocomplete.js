jQuery.fn.autocomplete = function(url, accepted, noresults) {
    this.each(function() {
        var element = this;
        new jQuery.autocomplete(this, url, accepted, noresults);
    });

    // Don't break the chain
    return this;
}

jQuery.autocomplete = function(element, url, accepted, noresults) {
    var activeIndex = -1;
    var results = document.createElement("div");
    var $results = $(results);
    var currentId = '';

    // Add to body element
    $results.addClass('ui-corner-all');
    $results.css('position', 'absolute');
    $results.css({
        width: "0px",
        top: "0px",
        left: "0px"
    });

    $("body").append(results);

    $(element).keydown(
        function(e) {
            switch (e.keyCode) {
                case 38: // up
                    moveUp();
                    e.preventDefault();
                    break;
                case 40: // down
                    moveDown();
                    e.preventDefault();
                    break;
                case 13: // return
                    e.preventDefault();
                    if (accepted != null) {
                        accepted(currentId);
                    }
                    $results.html('');
                    break;
            }
        });

    $(element).keyup(
        function(e) {
            switch (e.keyCode) {
                case 38: // up
                case 40: // down
                case 9:  // tab
                case 13: // return
                    break;
                default:
                    performSearch();
                    break;
            }
        });

    function moveUp() {
        if (activeIndex > 0) {
            activeIndex -= 1;
        }

        highlightValue();
    }

    function moveDown() {
        var count = $results.find("tr").length;

        if (activeIndex < 0) {
            activeIndex = 0;
        }
        else {
            activeIndex += 1;
        }
        if (activeIndex == count) {
            activeIndex = count - 1;
        }

        highlightValue();
    }

    function highlightValue() {
        $results.find("tr").each(
            function(index) {
                if (index == activeIndex) {
                    currentId = $(this).attr('id');
                    $(this).addClass('ui-widget-header');
                }
                else {
                    $(this).removeClass('ui-widget-header');
                }
            });
    }

    function hi() {
        alert('hi');
    }

    function performSearch() {
        var searchStringFromElement = $(element).val();
        if (searchStringFromElement) {
            $.getJSON(url, { searchString: searchStringFromElement }, populateSearchResults);
        }
        else {
            $results.html('');
            $results.css({
                width: "0px",
                top: "0px",
                left: "0px"
            });
        }
    }

    function populateSearchResults(data) {
        activeIndex = -1;
        $results.html('');
        var finalHtml = '<table class="data">';
        var currentValue = $(element).val().toLowerCase();
        for (index = 0; index < data.length; index++) {
            var formattedGameName = data[index].Value;
            var indexOf = formattedGameName.toLowerCase().indexOf(currentValue);
            if (indexOf != -1 && currentValue.length > 1) {
                formattedGameName = data[index].Value.substr(0, indexOf)
                                    + '<strong>' + data[index].Value.substr(indexOf, currentValue.length)
                                    + '</strong>' + data[index].Value.substr(indexOf + currentValue.length);
            }

            finalHtml += '<tr id="' + data[index].Key + '" style="cursor: pointer;" ><td>' + formattedGameName + '</td></tr>';
        }

        if (data.length == 0) {
            finalHtml += '<tr><td>' + noresults + '</td></tr>';
        }

        finalHtml += '</table>';
        $results.append(finalHtml);
        $results.find('tr').each(
            function() {
                $(this).hover(
                function() {
                    whenHover(this);
                });

                $(this).click(
                function() {
                    currentId = $(this).attr('id');
                    accepted(currentId);
                    $results.html('');
                });
            });

        // get the position of the input field right now (in case the DOM is shifted)
        var pos = findPos(element);
        // either use the specified width, or autocalculate based on form element
        var iWidth = $(element).width();
        // reposition
        $results.css({
            width: parseInt(iWidth) + "px",
            top: (pos.y + element.offsetHeight + 3) + "px",
            left: pos.x + "px"
        });
    }

    function findPos(obj) {
        var curleft = obj.offsetLeft || 0;
        var curtop = obj.offsetTop || 0;
        while (obj = obj.offsetParent) {
            curleft += obj.offsetLeft
            curtop += obj.offsetTop
        }
        return { x: curleft, y: curtop };
    }

    function whenHover(element) {
        $results.find("tr").each(
            function(index) {
                $(this).removeClass('ui-widget-header');
            });

        $(element).addClass('ui-widget-header');
        currentId = $(element).attr('id');
    }
}