var enums = null;
(function (enums) {

    var gender = {};
    gender[gender['Male'] = 0] = '男';
    gender[gender['Female'] = 1] = '女';
    gender[gender['DeclineToState'] = 2] = '拒绝透漏';

    enums.gender = gender;
})(enums || (enums = {}));

var app = angular.module("managerApp", ['ui.bootstrap', 'chart.js'])
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
    ]).controller('playerController', ['$scope', '$http', '$templateCache', '$controller',
        function ($scope, $http, $templateCache, $controller) {
            $controller('listController', { $scope: $scope });
            $scope.url = "/api/players";
            $scope.fetch();
        }
    ]);
