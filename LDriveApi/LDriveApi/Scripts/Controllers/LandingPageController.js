var webApp = angular.module('webApp', []);

webApp.m = false;
webApp.key = Date();

webApp.Load = function Load($scope, $http) {
    webApp.m = false;
    $.connection.hub.start().done(function () {
        $http({
            method: 'POST',
            url: '/api/values/',
            data: { Value: $scope.Path, Key: webApp.key }
        }).success(function (data, status, headers, config) {
            $.connection.hub.stop();
            $scope.Data = data;
            webApp.m = true;
            alert("Ready");
        });
    });

    var simpleHub = $.connection.simpleHub;
    simpleHub.client.getResults = function (value, key) {
        if (key == webApp.key)
        {         
                $scope.Data = value;
                $scope.$apply();            
        }
        
    }    
}

webApp.controller('LandingPageController', function ($scope, $http) {    

    $.connection.hub.stop();

    webApp.Load($scope, $http);

    $scope.changePath = function (path) {
        if (webApp.m) {
            $scope.Path = path;
            webApp.Load($scope, $http);
        }
        else {
            alert('Wait...')
        }
    };
});

