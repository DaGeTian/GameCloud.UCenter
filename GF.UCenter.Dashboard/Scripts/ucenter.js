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

var app = angular.module("ucenter", ['ui.bootstrap'])
    .filter('enums', function () {
        return function (input, enumName) {
            var items = enums[enumName];
            if (typeof (input) == 'number') {
                return items[input];
            } else {
                return items[items[input]];
            }
        }
    })
    .filter('yesNo', function () {
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
    ]);
