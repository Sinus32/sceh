$(function () {

    var prices = {};

    var updatePrice = function () {
        var myPrice = 0;
        var myPriceIsComplete = true;
        var otherPrice = 0;
        var otherPriceIsComplete = true;

        $('.allSteamApps .myCards .selected').each(function () {
            var obj = $(this);
            var marketHashName = obj.data('marketHashName');
            var result = prices[marketHashName];
            if (result === undefined) {
                myPriceIsComplete = false;
            } else {
                myPrice += result;
            }
        });

        $('.allSteamApps .otherCards .selected').each(function () {
            var obj = $(this);
            var marketHashName = obj.data('marketHashName');
            var result = prices[marketHashName];
            if (result === undefined) {
                otherPriceIsComplete = false;
            } else {
                otherPrice += result;
            }
        });

        myPrice = Math.round(myPrice * 100) / 100;
        otherPrice = Math.round(otherPrice * 100) / 100;

        $('#priceOfMine').text(myPriceIsComplete ? myPrice : myPrice + '...');
        $('#priceOfOther').text(otherPriceIsComplete ? otherPrice : otherPrice + '...');
    };

    var downloadValue = function (obj) {
        var marketHashName = obj.data('marketHashName');
        if (!marketHashName)
            return;

        var result = prices[marketHashName];
        if (result === undefined) {
            var url = $('#cardsPrice').data('cardPriceService');
            $.get(url, { 'marketHashName': marketHashName }, function (data, stats, xhr) {
                var lowest_price = parseFloat(data.lowest_price.replace(',', '.'));
                prices[marketHashName] = lowest_price;
                result = lowest_price;
                updatePrice();
            }, 'json');
        }
        updatePrice();
    };

    $(document).on('click', '.steamCard', function () {
        var obj = $(this);
        obj.toggleClass('selected');
        downloadValue(obj);
    });

    window.unselectAll = function () {
        $('.allSteamApps .selected').removeClass('selected');
        updatePrice();
    };
});