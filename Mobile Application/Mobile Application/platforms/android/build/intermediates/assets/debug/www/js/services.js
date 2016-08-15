angular.module('Starting_Point.services', [])
/*=====Local storage for storing user Session======*/
    .factory('$localstorage', ['$window', function ($window) {
        return {
            set: function (key, value) {
                $window.localStorage[key] = value;
            },
            get: function (key, defaultValue) {
                return $window.localStorage[key] || defaultValue;
            },
            setObject: function (key, value) {
                $window.localStorage[key] = JSON.stringify(value);
            },
            getObject: function (key) {
                return JSON.parse($window.localStorage[key] || '{}');
            },
            removeItem:function (key) {
                $window.localStorage.removeItem(key);
            },
            Clear: function () {
                $window.localStorage.clear();
            }
            
        }
    }])
    /*=========Factory for  checking the online and offline status of device======== */
    .factory('ConnectivityMonitor', function ($rootScope, $cordovaNetwork,$localstorage,Ajaxfact,$ionicPopup) {

        return {
            isOnline: function () {
                if (ionic.Platform.isWebView()) {
                    return $cordovaNetwork.isOnline();
                } else {
                    return navigator.onLine;
                }
            },
            isOffline: function () {
                if (ionic.Platform.isWebView()) {
                    return !$cordovaNetwork.isOnline();
                } else {
                    return !navigator.onLine;
                }
            },
            startWatching: function () {
                if (ionic.Platform.isWebView()) {

                    $rootScope.$on('$cordovaNetwork:online', function (event, networkState) {
                        console.log("went online");
                        
                        
                        //var offline_data  = JSON.parse($localstorage.get("OFFLINE_REQUESTS"));
                        //console.log(offline_data);
                        //console.log(offline_data.length);
                        
                        //var totallength=offline_data.length;
                        
                       // for(var i=0;i<totallength;i++){
                            
                           // var data=offline_data.pop();

                            /* Ajaxfact.Request('POST', 'PanicAttackScripts.php', data).success(function (data) {

                                $localstorage.set("OFFLINE_REQUESTS", JSON.stringify(offline_data));
                                var alertPopup = $ionicPopup.alert({
                                    title: 'success',
                                    template: 'panic attack send' //+data
                                });

                            }).error(function (data) {
                                //$ionicLoading.hide();
                                var alertPopup = $ionicPopup.alert({
                                    title: 'error',
                                    template: 'error while processing panic request'//+data
                                });
                            }); */

                            /* if(i==totallength-1){
                                $localstorage.removeItem("OFFLINE_REQUESTS");
                            }
 */

                       // }


                    });

                    $rootScope.$on('$cordovaNetwork:offline', function (event, networkState) {
                        console.log("went offline");
                    });

                }
                else {

                    window.addEventListener("online", function (e) {
                        console.log("went online");
                    }, false);

                    window.addEventListener("offline", function (e) {
                        console.log("went offline");
                    }, false);
                }
            }
        }
    })
    /*======Factory for sending post,get ,put requests to the server========*/
    .factory('Ajaxfact', function ($http, urlprefix) {
        return {
            Request: function (method, url, data) {
                return $http(
                    {
                        method: method,
                        url: urlprefix + url,
                        headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                        data: data
                    });
            }
        }
    });

