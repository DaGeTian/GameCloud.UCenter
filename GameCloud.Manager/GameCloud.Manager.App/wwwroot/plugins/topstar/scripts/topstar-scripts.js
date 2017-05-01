(function ($enums) {

    var canShowRanking = [{
        name: 'true',
        displayName: '显示',
        value: true
    }, {
        name: 'false',
        displayName: '不显示',
        value: false
    }];

    $enums.items.push({ name: 'canShowRanking', items: canShowRanking });

    var canShowSendGolds = [{
        name: 'true',
        displayName: '可赠送',
        value: true
    }, {
        name: 'false',
        displayName: '不可赠送',
        value: false
    }];

    $enums.items.push({ name: 'canShowSendGolds', items: canShowSendGolds });

    var commonTrueFalse = [{
        name: 'true',
        displayName: '是',
        value: true
    }, {
        name: 'false',
        displayName: '不是',
        value: false
    }];

    $enums.items.push({ name: 'commonTrueFalse', items: commonTrueFalse });

    var sysNoticeType = [{
        name: 'Sys',
        displayName: '系统',
        value: 1
    }, {
        name: 'Player',
        displayName: '玩家',
        value: 2
    }];

    $enums.items.push({ name: 'sysNoticeType', items: sysNoticeType });

    var sysNoticeLevel = [{
        name: 'Normal',
        displayName: '普通',
        value: 1
    }, {
        name: 'Important',
        displayName: '重要',
        value: 2
    }];

    $enums.items.push({ name: 'sysNoticeLevel', items: sysNoticeLevel });

    var produceGoldType = [{
        name: 'BuyInGame',
        displayName: '游戏内购买',
        value: 1
    }, {
        name: 'BuyFromGM',
        displayName: '通过后台购买',
        value: 2
    }, {
        name: 'BuyFromGMBank',
        displayName: '通过后台购买存入银行',
        value: 3
    }, {
        name: 'DailySendBySys',
        displayName: '每日赠送',
        value: 4
    }, {
        name: 'LostAllThenSendBySys',
        displayName: '输光后赠送',
        value: 5
    }, {
        name: 'SellItem',
        displayName: '卖属性',
        value: 6
    }, {
        name: 'BotSysSeedMoney',
        displayName: '机器人系统初始资金',
        value: 7
    }];

    $enums.items.push({ name: 'produceGoldType', items: produceGoldType });

    var produceDiamondType = [{
        name: 'BuyInGame',
        displayName: '游戏内购买',
        value: 1
    }, {
        name: 'BuyFromGM',
        displayName: '通过后台购买',
        value: 2
    }, {
        name: 'SellItem',
        displayName: '卖属性',
        value: 3
    }];

    $enums.items.push({ name: 'produceDiamondType', items: produceDiamondType });

    var recoverGoldType = [{
        name: 'BuyGiftTmp',
        displayName: '购买临时礼物',
        value: 1
    }, {
        name: 'BuyItem',
        displayName: '购买属性',
        value: 2
    }, {
        name: 'SeatFee',
        displayName: '台费',
        value: 3
    }, {
        name: 'SysPumping',
        displayName: '抽水',
        value: 4
    }, {
        name: 'RecoverByGM',
        displayName: '通过后台回收',
        value: 5
    }, {
        name: 'RecoverByGMBank',
        displayName: '通过后台回收银行筹码',
        value: 6
    }];

    $enums.items.push({ name: 'recoverGoldType', items: recoverGoldType });

    var recoverDiamondType = [{
        name: 'BuyGiftTmp',
        displayName: '购买临时礼物',
        value: 1
    }, {
        name: 'BuyItem',
        displayName: '购买属性',
        value: 2
    }, {
        name: 'RecoverByGM',
        displayName: '通过后台回收',
        value: 3
    }];

    var botNeedWinOrLoseState = [{
        name: 'NeedWin',
        displayName: '要赢',
        value: 0
    }, {
        name: 'NeedLose',
        displayName: '要输',
        value: 1
    }];
    $enums.items.push({ name: 'recoverDiamondType', items: recoverDiamondType });

    $enums.items.push({ name: 'botNeedWinOrLoseState', items: botNeedWinOrLoseState });    

    var isClassic = [{
        name: 'false',
        displayName: '必下',
        value: false
    }, {
        name: 'true',
        displayName: '经典',
        value: true
    }];

    $enums.items.push({ name: 'isClassic', items: isClassic });

    var cardSuit = [{
        name: 'club',
        displayName: '♣',
        value: 0
    }, {
        name: 'diamond',
        displayName: '♦',
        value: 1
    }, {
        name: 'heart',
        displayName: '♥',
        value: 2
    }, {
        name: 'spade',
        displayName: '♠',
        value: 3
    }];

    $enums.items.push({ name: 'cardSuit', items: cardSuit });

    var cardType = [{
        name: 'jack',
        displayName: 'J',
        value: 11
    }, {
        name: 'queen',
        displayName: 'Q',
        value: 12
    }, {
        name: 'king',
        displayName: 'K',
        value: 13
    }, {
        name: 'ace',
        displayName: 'A',
        value: 14
    }];

    $enums.items.push({ name: 'cardType', items: cardType });

    var handRankType = [{
        name: 'highCard',
        displayName: '高牌',
        value: 1000
    }, {
        name: 'pair',
        displayName: '一对',
        value: 2000
    }, {
        name: 'twoPairs',
        displayName: '两对',
        value: 3000
    }, {
        name: 'threeOfAKind',
        displayName: '三条',
        value: 4000
    }, {
        name: 'straight',
        displayName: '顺子',
        value: 5000
    }, {
        name: 'flush',
        displayName: '同花',
        value: 6000
    }, {
        name: 'fullHouse',
        displayName: '葫芦',
        value: 7000
    }, {
        name: 'fourOfAKind',
        displayName: '四条',
        value: 8000
    }, {
        name: 'straightFlush',
        displayName: '同花顺',
        value: 9000
    }];

    $enums.items.push({ name: 'handRankType', items: handRankType });

    var weekDisplay = [{
        name: '1',
        displayName: '周一',
        value: '1'
    }, {
        name: '2',
        displayName: '周二',
        value: '2'
    }, {
        name: '3',
        displayName: '周三',
        value: '3'
    }, {
        name: '4',
        displayName: '周四',
        value: '4'
    }, {
        name: '5',
        displayName: '周五',
        value: '5'
    }, {
        name: '6',
        displayName: '周六',
        value: '6'
    }, {
        name: '0',
        displayName: '周日',
        value: '0'
    }];

    $enums.items.push({ name: 'weekDisplay', items: weekDisplay });
})($enums || ($enums = {}));

$pluginApp.controller('tpStatisticsBotGroupCurrentChipsController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('chartController', { $scope: $scope });
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
    $scope.options = {
        legend: { display: true },
        elements: { line: { fill: false } }
    };
}])
.controller('tpStatisticsProduceChipsHistoryController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('chartController', { $scope: $scope });
    var lastMonth = Date.today();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    $scope.params.startDate = lastMonth;
    $scope.params.endDate = Date.today();
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
    $scope.options = {
        legend: { display: true },
        elements: { line: { fill: false } }
    };
}])
.controller('tpStatisticsProduceGoldsHistoryController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('chartController', { $scope: $scope });
    var lastMonth = Date.today();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    $scope.params.startDate = lastMonth;
    $scope.params.endDate = Date.today();
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
    $scope.options = {
        legend: { display: true },
        elements: { line: { fill: false } }
    };
}])
.controller('tpStatisticsRecoverChipsHistoryController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('chartController', { $scope: $scope });
    var lastMonth = Date.today();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    $scope.params.startDate = lastMonth;
    $scope.params.endDate = Date.today();
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
    $scope.options = {
        legend: { display: true },
        elements: { line: { fill: false } }
    };
}])
.controller('tpStatisticsRecoverGoldsHistoryController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('chartController', { $scope: $scope });
    var lastMonth = Date.today();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    $scope.params.startDate = lastMonth;
    $scope.params.endDate = Date.today();
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
    $scope.options = {
        legend: { display: true },
        elements: { line: { fill: false } }
    };
}]);