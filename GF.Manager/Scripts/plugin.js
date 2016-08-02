var enums = null;
(function (enums) {

    var gender = {};
    gender[gender['Male'] = 0] = '男';
    gender[gender['Female'] = 1] = '女';
    gender[gender['DeclineToState'] = 2] = '拒绝透漏';

    enums.gender = gender;
})(enums || (enums = {}));
var plugins = [
    {
        displayName: "德州扑克管理",
        name: "texaspoker",
        description: "texaspoker manager",
        collections: [
            {
                name: "player-manager",
                displayName: "玩家管理",
                items: [
                    {
                        name: "player-search",
                        displayName: "玩家查询",
                        view: "player-search.html",
                        type: 'search'
                    },
                    {
                        name: "bot-search",
                        displayName: "机器人查询",
                        view: "bot-search.html",
                        type: 'search'
                    }
                ]
            }
        ]
    }
];
var app = angular.module("pluginApp", ['ui.bootstrap', 'chart.js', 'ngRoute'])
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
    }).config(['$routeProvider', function ($routeProvider) {
        if (plugins) {
            plugins.forEach(function (p) {
                $routeProvider.when('/' + p.name, {
                    templateUrl: '/plugins/_templates/views/plugin.html',
                    controller: 'pluginController'
                });

                if (p.collections) {
                    p.collections.forEach(function (c) {
                        $routeProvider.when('/' + p.name + '/' + c.name, {
                            templateUrl: '/plugins/_templates/views/collection.html',
                            controller: 'collectionController'
                        });

                        if (c.items) {
                            c.items.forEach(function (i) {
                                $routeProvider.when('/' + p.name + '/' + c.name + '/' + i.name, {
                                    templateUrl: '/plugins/_templates/views/' + i.type + '.html',
                                    controller: i.type + 'Controller'
                                });
                            })
                        }
                    });

                    if (p.items) {
                        p.items.forEach(function (i) {
                            $routeProvider.when('/' + p.name + '/' + i.name, {
                                templateUrl: '/plugins/_templates/views/' + i.type + '.html',
                                controller: i.type + 'Controller'
                            });
                        })
                    }
                }
            });
            $routeProvider.otherwise({
                templateUrl: '/plugins/_templates/views/error.html'
            })
        }
    }]).service('pluginService', ['$http', function ($http) {
        this.plugins = plugins;
        this.plugin = plugins[0];
    }]).controller('navController', ['$scope', '$http', '$templateCache', 'pluginService', function ($scope, $http, $templateCache, pluginService) {
        $scope.plugins = pluginService.plugins;
        $scope.plugin = pluginService.plugin;
        $scope.setCollection = function (c) {
            pluginService.collection = c;
        };

        $scope.setItem = function (i) {
            pluginService.item = i;
        }
    }]).controller('pluginController', ['$scope', '$http', '$templateCache', 'pluginService', function ($scope, $http, $templateCache, pluginService) {
        $scope.plugin = pluginService.plugin;
    }]).controller('collectionController', ['$scope', '$http', '$templateCache', 'pluginService', function ($scope, $http, $templateCache, pluginService) {
        $scope.collection = pluginService.collection;
    }]).controller('searchController', ['$scope', '$http', '$templateCache', 'pluginService', function ($scope, $http, $templateCache, pluginService) {
        $scope.item = pluginService.item;
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
