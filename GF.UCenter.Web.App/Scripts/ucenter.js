var enums = null;
(function (enums) {
    var orderState = {};
    orderState[orderState['Created'] = 0] = '已创建';
    orderState[orderState['Success'] = 1] = '成功';
    orderState[orderState['Failed'] = 2] = '失败';
    orderState[orderState['Expired'] = 3] = '已过期';

    enums.orderState = orderState;

    var sex = {};
    sex[sex['Male'] = 0] = '男';
    sex[sex['Female'] = 1] = '女';
    sex[sex['Unknown'] = 2] = '未知';

    enums.sex = sex;
})(enums || (enums = {}));

var app = angular.module("ucenter", ['ui.bootstrap', 'chart.js'])
    .filter('enums', function () {
        return function (input, enumName) {
            var items = enums[enumName];
            if (typeof (input) === 'number') {
                return items[input];
            } else {
                return items[items[input]];
            }
        }
    }).filter('yesNo', function () {
        return function (input) {
            return input ? '是' : '否';
        }
    }).controller('listController', ['$scope', '$http', '$templateCache',
        function ($scope, $http, $templateCache) {

            $scope.maxShowPages = 7;
            $scope.page = 1;
            $scope.pageSize = 10;
            $scope.pageSizeList = [5, 10, 20, 50];
            $scope.params = {};
            $scope.beforeFetch = function () { };
            $scope.fetch = function () {
                $scope.code = null;
                $scope.response = null;
                var params = { page: $scope.page, count: $scope.pageSize };
                for (p in $scope.params) {
                    if ($scope.params[p]) {
                        params[p] = $scope.params[p];
                    }
                }

                $http.get(
                    $scope.url,
                    {
                        params: params,
                        responseType: 'json',
                        cache: $templateCache
                    }).
                    then(function (response) {
                        $scope.status = response.status;
                        $scope.data = response.data;
                        $scope.raws = response.data.Raws;
                    }, function (response) {
                        $scope.data = response.data || "Request failed";
                        $scope.status = response.status;
                    });
            };

            $scope.onPageChange = function () {
                $scope.fetch();
            }

            $scope.onPageSizeChange = function (size) {
                $scope.pageSize = size;
                $scope.fetch();
            }

            $scope.updateModel = function (method, url) {
                $scope.method = method;
                $scope.url = url;
            };
        }
    ]).controller('usersController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {
            $controller('listController', { $scope: $scope });
            $scope.url = "/api/users";
            $scope.fetch();
        }
    ]).controller('ordersController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {
            $controller('listController', { $scope: $scope });
            $scope.url = "/api/orders";
        }
    ]).controller('orderController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {
            $controller('listController', { $scope: $scope });
            $scope.url = "/api/orders";
            $scope.fetch = function () {
                $http.get(
                    '/api/orders?id=' + $scope.id,
                    {
                        responseType: 'json'
                    }).
                    then(function (response) {
                        $scope.status = response.status;
                        $scope.data = response.data;
                    }, function (response) {
                        $scope.data = response.data || "Request failed";
                        $scope.status = response.status;
                    });
            };
        }
    ]).controller('activeUsersController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {
            $scope.hourDAULabels = common.getNArray(30);
            $scope.hourDAUSeries = ['小时DAU'];
            $scope.hourDAUData = common.getNRandomArray(30);

            $scope.hourWAULabels = common.getNArray(7);
            $scope.hourWAUSeries = ['小时WAU'];
            $scope.hourWAUData = common.getNRandomArray(7);

            $scope.hourMAULabels = common.getNArray(30);
            $scope.hourMAUSeries = ['小时MAU'];
            $scope.hourMAUData = common.getNRandomArray(30);

            $scope.activeUserSexPieLabels = ["男", "女"];
            $scope.activeUserSexPieData = [85, 15];

            $scope.activeUserAgePieLabels = ["20-30", "30-40", "40-50", "50-60"];
            $scope.activeUserAgePieData = [55, 5, 15, 15, 5];


        }
    ])
    .controller('newUsersController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {

            $scope.firstPlayTimePieLabels = ["6PM", "7PM", "8PM", "9PM", "10PM", "11PM", "12PM"];
            $scope.firstPlayTimePieData = [10, 20, 10, 20, 10, 20, 10];

            $scope.newUserSexPieLabels = ["男", "女"];
            $scope.newUserSexPieData = [85, 15];

            $scope.newUserAgePieLabels = ["20-30", "30-40", "40-50", "50-60"];
            $scope.newUserAgePieData = [55, 5, 15, 15, 5];

            $scope.hourActiveDeviceLabels = common.getNArray(24);
            $scope.hourActiveDeviceSeries = ['小时设备激活'];
            $scope.hourActiveDeviceData = common.getNRandomArray(24);

            $scope.hourNewUserLabels = common.getNArray(7);
            $scope.hourNewUserSeries = ['小时新增帐户'];
            $scope.hourNewUserData = common.getNRandomArray(7);

            $scope.hourNewDeviceLabels = common.getNArray(7);
            $scope.hourNewDeviceSeries = ['小时新增设备'];
            $scope.hourNewDeviceData = common.getNRandomArray(7);
        }
    ]).controller('onlineAnalyticsController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {
            $scope.activeUserLabels = common.getNArray(24);
            $scope.activeUserSeries = ['今日', '昨日', '上周同日'];
            $scope.activeUserData = [
             common.getNRandomArray(24),
             common.getNRandomArray(24),
             common.getNRandomArray(24)
            ];
            $scope.activeUserOnClick = function (points, evt) {
                console.log(points, evt);
            };
            $scope.activeUserDatasetOverride = [{ yAxisID: 'y-axis-1' }, { yAxisID: 'y-axis-2' }, { yAxisID: 'y-axis-3' }];
            $scope.activeUserOptions = {
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
                          position: 'left'
                      },
                        {
                            id: 'y-axis-3',
                            type: 'linear',
                            display: true,
                            position: 'left'
                        }
                    ]
                }
            };

            $scope.hourACULabels = common.getNArray(15);
            $scope.hourACUSeries = ['今日'];
            $scope.hourACUData = common.getNRandomArray(15);
            $scope.hourACUOnClick = function (points, evt) {
                console.log(points, evt);
            };
            $scope.hourACUDatasetOverride = [{ yAxisID: 'y-axis-1' }];
            $scope.hourACUOptions = {
                scales: {
                    yAxes: [
                      {
                          id: 'y-axis-1',
                          type: 'linear',
                          display: true,
                          position: 'left'
                      }
                    ]
                }
            };

            $scope.hourAveACULabels = common.getNArray(15);
            $scope.hourAveACUSeries = ['今日'];
            $scope.hourAveACUData = common.getNRandomArray(15);
            $scope.hourAveACUOnClick = function (points, evt) {
                console.log(points, evt);
            };
            $scope.hourAveACUDatasetOverride = [{ yAxisID: 'y-axis-1' }];
            $scope.hourAveACUOptions = {
                scales: {
                    yAxes: [
                      {
                          id: 'y-axis-1',
                          type: 'linear',
                          display: true,
                          position: 'left'
                      }
                    ]
                }
            };

            $scope.hourMaxACULabels = common.getNArray(15);
            $scope.hourMaxACUSeries = ['今日'];
            $scope.hourMaxACUData = common.getNRandomArray(15);
            $scope.hourMaxACUOnClick = function (points, evt) {
                console.log(points, evt);
            };
            $scope.hourMaxACUDatasetOverride = [{ yAxisID: 'y-axis-1' }];
            $scope.hourMaxACUOptions = {
                scales: {
                    yAxes: [
                      {
                          id: 'y-axis-1',
                          type: 'linear',
                          display: true,
                          position: 'left'
                      }
                    ]
                }
            };

            $scope.hourPCULabels = common.getNArray(15);
            $scope.hourPCUSeries = ['今日'];
            $scope.hourPCUData = common.getNRandomArray(15);
            $scope.hourPCUOnClick = function (points, evt) {
                console.log(points, evt);
            };
            $scope.hourPCUDatasetOverride = [{ yAxisID: 'y-axis-1' }];
            $scope.hourPCUOptions = {
                scales: {
                    yAxes: [
                      {
                          id: 'y-axis-1',
                          type: 'linear',
                          display: true,
                          position: 'left'
                      }
                    ]
                }
            };

            $scope.hourACUPCULabels = common.getNArray(15);
            $scope.hourACUPCUSeries = ['今日'];
            $scope.hourACUPCUData = common.getNRandomArray(15);
            $scope.hourACUPCUOnClick = function (points, evt) {
                console.log(points, evt);
            };
            $scope.hourACUPCUDatasetOverride = [{ yAxisID: 'y-axis-1' }];
            $scope.hourACUPCUOptions = {
                scales: {
                    yAxes: [
                      {
                          id: 'y-axis-1',
                          type: 'linear',
                          display: true,
                          position: 'left'
                      }
                    ]
                }
            };
        }
    ]).controller('onlineBehaviourController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {

        }
    ]);
