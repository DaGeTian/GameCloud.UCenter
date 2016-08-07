var $enums = null;
(function (enums) {

    var gender = {};
    gender[gender['Male'] = 0] = '男';
    gender[gender['Female'] = 1] = '女';
    gender[gender['DeclineToState'] = 2] = '拒绝透漏';

    enums.gender = gender;
})($enums || ($enums = {}));

$groupCache = {};
// var $plugins = null;
var $pluginApp = angular.module("pluginApp", ['ui.bootstrap', 'chart.js', 'ngRoute'])
    .filter('enums', function () {
        return function (input, enumName) {
            var items = $enums[enumName];
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
    }).filter('group', function () {
        return function (input, length) {
            if (!input) {
                return null;
            }

            var key = [];
            angular.forEach(input, function (v) {
                key.push(v.name);
            });
            key = key.join('-') + "-" + length;
            if ($groupCache[key]) {
                return $groupCache[key];
            }

            var rows = [];
            var row = [];
            if (input.length < length) {
                rows = [input];
            } else {
                for (var i = 0; i < input.length; i++) {
                    if (i % length == 0) {
                        row = [];
                        rows.push(row);
                    }

                    row.push(input[i]);
                }
            }

            $groupCache[key] = rows;
            return rows;
        };
    }).service('pluginService', ['$http', '$location', function ($http, $location) {
        var find = function (items, name) {
            for (var i = items.length - 1; i >= 0; i--) {
                if (items[i].name == name) {
                    return items[i];
                }
            }

            return items[0];
        }

        var observerCallbacks = [];

        //register an observer
        this.registerObserverCallback = function (callback) {
            observerCallbacks.push(callback);
        };

        //call this when you know 'foo' has been changed
        var notifyObservers = function () {
            angular.forEach(observerCallbacks, function (callback) {
                callback();
            });
        };

        this.setCurrent = function (path) {
            var paths = path.split('/');
            var plugin = null;
            var category = null;
            var item = null;
            for (var i = 0; i < paths.length; i++) {
                var p = paths[i];
                if (p) {
                    var kv = p.split(':');
                    var k = kv[0];
                    var v = kv[1];
                    switch (k) {
                        case 'p':
                            plugin = find(this.plugins, v);
                            break;
                        case 'c':
                            if (plugin) {
                                category = find(plugin.categories, v);
                            }
                            break;
                        case 'i':
                            if (category) {
                                item = find(category.items, v);
                            } else if (plugin) {
                                item = find(plugin.items, v);
                            }
                            break;
                    }
                }
            }
            this.plugin = plugin;
            this.category = category;
            this.item = item;
        };

        var _this = this;
        this.init = function (route, data) {
            this.plugins = data || this.plugins;
            if (!this.plugins) {
                $http.get('/api/plugins').then(function (response) {
                    _this.plugins = response.data;
                    _this.init(route);
                    initRouteProvider(_this.plugins);
                    $location.url($location.url());
                }, function (response) {
                    console.log('get plugin list error:', response);
                });
            } else {
                if (this.plugins && route) {
                    this.setCurrent(route);
                }

                notifyObservers();
            }
        };
    }])
    .controller('baseController', ['$scope', '$http', '$location', '$templateCache', 'pluginService', function ($scope, $http, $location, $templateCache, pluginService) {

        var group = function (items, length) {
            if (!items) {
                return null;
            }

            if (items.length <= length) {
                return [items];
            }
            var groups = [];
            var group = [];
            for (var i = 0; i < items.length; i++) {
                if (i % length == 0) {
                    group = [];
                    groups.push(group);
                }

                group.push(items[i]);
            }
            console.log(length, groups);
            return groups;
        }

        function updateScope() {
            $scope.plugins = pluginService.plugins;
            $scope.plugin = pluginService.plugin;
            $scope.category = pluginService.category;
            $scope.item = pluginService.item;
        }

        updateScope();

        pluginService.registerObserverCallback(updateScope);
    }]).controller('navController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });
        $scope.plugins = pluginService.plugins;
        $scope.plugin = pluginService.plugin;
        $scope.setCategory = function (c) {
            pluginService.category = c;
        };

        $scope.setItem = function (item) {
            pluginService.item = item;
        };

    }]).controller('pluginController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });
    }]).controller('categoryController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });
    }]).controller('apiController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });
        $scope.params = {};
        $scope.beforeFetch = function () { };

        var post = function (method) {
            $scope.code = null;
            $scope.response = null;
            var params = [];
            for (var n in $scope.params) {
                if ($scope.params[n]) {
                    params.push(n + "=" + $scope.params[n]);
                }
            }

            $http.post(
                '/pluginrequest',
                {
                    pluginName: $scope.plugin.name,
                    category: $scope.category ? $scope.category.name : null,
                    item: $scope.item.name,
                    method: method,
                    content: $scope.params
                },
                {
                    responseType: 'json',
                    cache: $templateCache
                }).
                then(function (response) {
                    $scope.status = response.status;
                    $scope.data = response.data;
                }, function (response) {
                    $scope.data = response.data || "Request failed";
                    $scope.status = response.status;
                });
        }

        $scope.fetch = function () {
            post('Read');
        };

        $scope.update = function () {
            post('Update');
        };
    }]).controller('pluginsController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });
    }]).controller('listController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });
        $scope.maxShowPages = 7;
        $scope.params.pageSize = 10;
        $scope.pageSizeList = [5, 10, 20, 50];
        $scope.beforeFetch = function () { };

        $scope.onPageChange = function () {
            $scope.fetch();
        }

        $scope.onPageSizeChange = function (size) {
            $scope.params.pageSize = size;
            $scope.fetch();
        }

        $scope.updateModel = function (method, url) {
            $scope.method = method;
            $scope.url = url;
        };

        $scope.fetch();
    }]).controller('updateController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });

        $scope.updateData = function () {
            $scope.params.data = JSON.stringify($scope.data);
            $scope.update();
        }

        $scope.fetch();
    }]).controller('reportController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });
        $scope.fetch();
    }])
    .config(['$routeProvider', function ($routeProvider) {
        if ($plugins) {
            $plugins.forEach(function (p) {
                $routeProvider.when('/p:' + p.name, {
                    templateUrl: '/plugins/_templates/views/plugin.html',
                    controller: 'pluginController'
                });

                if (p.categories) {
                    p.categories.forEach(function (c) {
                        $routeProvider.when('/p:' + p.name + '/c:' + c.name, {
                            templateUrl: '/plugins/_templates/views/category.html',
                            controller: 'categoryController'
                        });

                        if (c.items) {
                            c.items.forEach(function (i) {
                                $routeProvider.when('/p:' + p.name + '/c:' + c.name + '/i:' + i.name, {
                                    templateUrl: '/plugins/_templates/views/' + i.type + '.html',
                                    controller: i.controller
                                });
                            })
                        }
                    });

                    if (p.items) {
                        p.items.forEach(function (i) {
                            $routeProvider.when('/p:' + p.name + '/i:' + i.name, {
                                templateUrl: '/plugins/_templates/views/' + i.type + '.html',
                                controller: i.controller
                            });
                        })
                    }
                }
            });
            $routeProvider.otherwise({
                templateUrl: '/plugins/_templates/views/error.html'
            });
        }
    }])
    .run(['$rootScope', '$location', '$http', 'pluginService', function ($rootScope, $location, $http, pluginService) {
        $rootScope.$on('$routeChangeStart', function (next, current) {
            pluginService.init($location.path());
        });

        pluginService.init($location.path(), $plugins);
    }]);
