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

    var downloadQueue = [];

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
                if (downloadQueue.length > 0) {
                    var next = downloadQueue.shift();
                    setTimeout(function () { downloadValue(next); }, 1200);
                }
            }, 'json').error(function () {
                downloadQueue.push(obj);
                var next = downloadQueue.shift();
                setTimeout(function () { downloadValue(next); }, 10000);
            });
        } else {
            if (downloadQueue.length > 0) {
                var next = downloadQueue.shift();
                setTimeout(function () { downloadValue(next); }, 10);
            }
        }
        updatePrice();
    };

    $(document).on('click', '.steamCard', function () {
        var obj = $(this);
        obj.toggleClass('selected');
        var steamApp = obj.closest('.steamApp');
        if (obj.hasClass('selected')) {
            steamApp.addClass('hasSelected');
            if (obj.is('.myCards .steamCard')) {
                steamApp.addClass('hasSelectedMine');
            } else {
                steamApp.addClass('hasSelectedOther');
            }
        } else {
            var a = steamApp.find('.myCards .steamCard.selected:first').length;
            var b = steamApp.find('.otherCards .steamCard.selected:first').length;
            if (a) {
                if (!b) {
                    steamApp.removeClass('hasSelectedOther');
                }
            } else {
                if (b) {
                    steamApp.removeClass('hasSelectedMine');
                } else {
                    steamApp.removeClass('hasSelected hasSelectedMine hasSelectedOther');
                }
            }
        }
        downloadValue(obj);
    });

    $('.allSteamApps .steamCard img').lazyload({
        skip_invisible: true
    });

    window.unselectAll = function () {
        $('.allSteamApps .selected').removeClass('selected');
        $('.steamApp.hasSelected').removeClass('hasSelected hasSelectedMine hasSelectedOther');
        updatePrice();
    };

    window.filterSelected = function (mode) {
        switch (mode) {
            case 1: $('.allSteamApps').removeClass('filterSelectedApp filterSelected').addClass('showMyApps'); break;
            case 2: $('.allSteamApps').removeClass('showMyApps filterSelectedApp filterSelected'); break;
            case 3: $('.allSteamApps').addClass('filterSelectedApp').removeClass('showMyApps filterSelected'); break;
            case 4: $('.allSteamApps').addClass('filterSelectedApp filterSelected').removeClass('showMyApps'); break;
        }
    };

    window.selectFromHtml = function () {
        var dialog = $('<div>');
        var msg = $('<div>');
        msg.appendTo(dialog);
        var tb = $('<textarea>', { style: 'width: 500px; height: 250px;' });
        tb.appendTo(dialog);
        dialog.appendTo(document.body);
        dialog.dialog({
            autoOpen: true,
            width: 550,
            title: 'Place html from a trade offer below',
            close: function () { $(this).dialog('destroy').remove(); },
            buttons: {
                'execute': function () {
                    var val = tb.val();
                    if (!val) {
                        msg.text('please, provide html');
                        return;
                    }
                    var data = $(val);
                    if (!data || !data.length) {
                        msg.text('invalid html');
                        return;
                    }
                    var tradeItems = data.find('div.trade_item[data-economy-item]');
                    if (!tradeItems.length) {
                        msg.text('no trade_items found');
                        return;
                    }
                    var missing = 0;
                    var mine = 0;
                    var others = 0;
                    tradeItems.each(function () {
                        var economyItem = $(this).data('economy-item');
                        var parts = economyItem.split('/');
                        if (parts.length == 4) {
                            var q = '.allSteamApps .steamCard[data-id=' + parts[2] + ']:first';
                            var obj = $(q);
                            if (obj.length) {
                                var steamApp = obj.closest('.steamApp');
                                if (obj.is('.myCards .steamCard')) {
                                    mine += 1;
                                    steamApp.addClass('hasSelectedMine');
                                } else {
                                    others += 1;
                                    steamApp.addClass('hasSelectedOther');
                                }
                                obj.addClass('selected');
                                steamApp.addClass('hasSelected');
                                downloadQueue.push(obj);
                            } else {
                                missing += 1;
                            }
                        }
                    });
                    msg.text('Done. My cards: ' + mine + ', other: ' + others + ', not found:' + missing);
                    setTimeout(updatePrice, 10);
                    if (downloadQueue.length > 0) {
                        var obj = downloadQueue.shift();
                        downloadValue(obj);
                    }
                },
                'close': function () { $(this).dialog('close').remove(); }
            }
        });
    };
});