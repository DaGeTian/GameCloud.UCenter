(function ($enums) {

    var accountStatusEnum = [{
        name: 'active',
        displayName: '可用',
        value: 0
    }, {
        name: 'disabled',
        displayName: '禁用',
        value: 1
    }];

    $enums.items.push({ name: 'accountStatusEnum', items: accountStatusEnum });

    var accountTypeEnum = [{
        name: 'normalAccount',
        displayName: '普通用户',
        value: 0
    }, {
        name: 'guest',
        displayName: '访客',
        value: 1
    }];

    $enums.items.push({ name: 'accountTypeEnum', items: accountTypeEnum });
})($enums || ($enums = {}));

$pluginApp.controller('ucenterUserStatisticsController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('chartController', { $scope: $scope });
    var lastMonth = Date.today();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    $scope.params.startDate = lastMonth;
    $scope.params.endDate = Date.today();
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
    $scope.options = {
        legend: { display: true },
        scales: {
            yAxes: [
              {
                  id: 'y-axis-1',
                  type: 'linear',
                  display: true,
                  position: 'left'
              },
              {
                  id: 'y-axis-2',
                  type: 'linear',
                  display: true,
                  position: 'right'
              }
            ]
        }
    };

    $scope.createOverride = function () {
        $scope.override = [{
            label: $scope.type + "率",
            type: "line",
            yAxisID: "y-axis-1"
        }, {
            label: $scope.type + "总数",
            type: "bar",
            yAxisID: "y-axis-2"
        }];
    }

    $scope._sync();
}])
.controller('ucenterUserStayStatisticsController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('ucenterUserStatisticsController', { $scope: $scope });
    $scope.params.isStay = true;
    $scope.title = "留存统计";
    $scope.type = "留存";
    $scope.createOverride();
}])
.controller('ucenterUserLostStatisticsController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('ucenterUserStatisticsController', { $scope: $scope });
    $scope.params.isStay = false;
    $scope.title = "流失统计";
    $scope.type = "流失";
    $scope.createOverride();
}])
.controller('ucenterNewUsersController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
    $controller('chartController', { $scope: $scope });

    $scope.params.startDate = Date.yesterday();
    $scope.params.endDate = Date.today();
    $scope.charts = [[{
        title: '小时新增帐户',
        description: '统计所选时期内，每日玩家激活游戏后，进行了自动或者手动注册有ID信息或者账户信息的玩家账户数量。',
        key: 'hourlyNewUsers'
    }
    //, {
    //    title: '小时新增设备',
    //    description: '统计所选时期内，每日玩家激活游戏后，进行了自动或者手动注册有ID信息或者账户信息的玩家设备数量，每台设备只计算一次。',
    //    key: 'hourlyNewUsers'
    //}],
    //[{
    //    title: '小时设备激活',
    //    description: '"统计所选时期内，每日新增的玩家安装游戏客户端，并运行游戏的可连接网络设备的数量，每台设备只计算一次。',
    //    key: 'hourlyNewUsers'
    //}, {
    //    title: '小时玩家转化率',
    //    description: '统计所选时期内，每日玩家激活游戏后，进行了自动或者手动注册有ID信息或者账户信息的玩家设备数量，单设备中多个帐号只计算一次成功转化。',
    //    key: 'hourlyNewUsers'
    //}],
    //[{
    //    title: '小时首次游戏时长',
    //    description: '统计所选时期内，新增玩家首次进行游戏的游戏时间区间分布。',
    //    key: 'hourlyNewUsers'
    //}, {
    //    title: '小时新增玩家地区/国家',
    //    description: '统计所选时期内，新增玩家注册信息中地区和国家分布情况。',
    //    key: 'hourlyNewUsers'
    //}],
    //[{
    //    title: '小时新增玩家性别',
    //    description: '统计所选时期内，新增玩家注册信息中性别分布情况。',
    //    key: 'hourlyNewUsers'
    //}, {
    //    title: '小时新增玩家年龄',
    //    description: '统计所选时期内，新增玩家注册信息中年龄分布情况。',
    //    key: 'hourlyNewUsers'
    //}
    ]];

    $scope.fetch = function () {
        $scope._sync(function (isSuccess, response) {
            if (isSuccess) {
                $scope.charts.forEach(function (row) {
                    row.forEach(function (chart) {
                        chart.data = response.data[chart.key];
                    });
                });
            }
        });
    };

    $scope.fetch();
}])
.controller('ucenterStatisticsController', ['$scope', '$http', '$templateCache', '$controller', '$cacheFactory', 'pluginService', function ($scope, $http, $templateCache, $controller, $cacheFactory, pluginService) {
    $controller('chartController', { $scope: $scope });
    var $cache = $cacheFactory.get("ucenter-statistics");
    if (!$cache) {
        $cache = $cacheFactory("ucenter-statistics");
    }

    var saveDateRange = function () {
        $cache.put("startDate", $scope.params.startDate);
        $cache.put("endDate", $scope.params.endDate);
    };

    var getDateRange = function () {
        var start = $cache.get("startDate") || Date.today();
        var end = $cache.get("endDate") || Date.today();
        return [start, end];
    };

    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
    $scope.options = {
        legend: { display: true },
        scaleStartValue: 0,
        elements: { line: { fill: false } }
    };

    var range = getDateRange();
    $scope.params.startDate = range[0];
    $scope.params.endDate = range[1];

    $scope._setDates = function (startDate, endDate) {
        $scope.params.startDate = startDate;
        $scope.params.endDate = endDate;
        $scope.sync();
    };

    $scope.today = function () {
        var start = Date.today();
        var end = Date.today();
        $scope._setDates(start, end);
    };

    $scope.yesterday = function () {
        var start = Date.yesterday();
        var end = Date.yesterday();
        $scope._setDates(start, end);
    };

    $scope.lastNDays = function (n) {
        var start = Date.today().add(n, 'days');
        var end = Date.today();
        $scope._setDates(start, end);
    };

    $scope.sync = function () {
        saveDateRange();
        $scope._sync();
    };

    $scope.sync();
}]);