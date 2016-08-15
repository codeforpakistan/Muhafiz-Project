// Ionic Starter App

// angular.module is a global place for creating, registering and retrieving Angular modules
// 'starter' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
// 'starter.services' is found in services.js
// 'starter.controllers' is found in controllers.js
angular.module('Starting_Point', ['ionic','ngCordova', 'Starting_Point.controllers', 'Starting_Point.services','Starting_Point.directive','Starting_Point.Constants'])

.run(function($ionicPlatform,ConnectivityMonitor) {
  $ionicPlatform.ready(function() {
    // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
    // for form inputs)
    if (window.cordova && window.cordova.plugins && window.cordova.plugins.Keyboard) {
      cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
      cordova.plugins.Keyboard.disableScroll(true);

    }
    if (window.StatusBar) {
      // org.apache.cordova.statusbar required
      StatusBar.styleDefault();
    }
      ConnectivityMonitor.startWatching();


      document.addEventListener("online", onOnline, false);
      document.addEventListener("offline", onOffline, false);

      function onOnline() {
          $rootScope.$apply(function(){
             // console.log("just got online event");
              $rootScope.noNetwork = false;
          });
      }

      function onOffline() {
          $rootScope.$apply(function(){
              //.log("just got offline event");
              $rootScope.noNetwork = true;
          });
      }
  });
})

   
    .value('urlprefix', "http://muhafizapp.azurewebsites.net/Api/UserScripts/")
	  //.value('urlprefix', "http://10.11.202.171/Api/UserScripts/")
	  //.value('urlprefix', "http://192.168.1.6/Api/UserScripts/")

.config(function($stateProvider, $urlRouterProvider,$ionicConfigProvider) {

	$ionicConfigProvider.views.maxCache(0);
  // Ionic uses AngularUI Router which uses the concept of states
  // Learn more here: https://github.com/angular-ui/ui-router
  // Set up the various states which the app can be in.
  // Each state's controller can be found in controllers.js
  $stateProvider

  // setup an abstract state for the tabs directive


  // Each tab has its own nav history stack:
      .state('menu', {
          url: '/menu',
          abstract:true,
          templateUrl: 'templates/menu.html',
          controller: 'menuCtrl'
      })

      .state('Login', {
        url: '/Login',
        templateUrl: 'templates/Login.html',
        controller: 'LoginCtrl'
      })

      /*.state('index', {
        url: '/index',
        templateUrl: 'index.html',
        controller: 'indexCtrl'
      })*/
      .state('Registration', {
        url: '/Registration',
        templateUrl: 'templates/Registration.html',
        controller: 'RegisterCtrl'

      })
      .state('pendingRegistration', {
          url: '/pendingRegistration',
          templateUrl: 'templates/pendingRegistration.html',
          controller: 'pendingRegistrationCtrl'

      })
    
      .state('menu.UserHome', {
          url: '/UserHome',
          views:{
              'menuContent':{
                  templateUrl: 'templates/UserHome.html',
                  controller: 'UserHomeCtrl'
              }
          }


      })
      .state('start', {
          url: '/start',
          templateUrl: 'templates/start.html',
          controller: 'startCtrl'

      })
      .state('ProfileSetting', {
          url: '/ProfileSetting',

                  templateUrl: 'templates/ProfileSetting.html',
                  controller: 'ProfileSettingCtrl'



      })
      .state('ThreatReport', {
        url: '/ThreatReport',
        templateUrl: 'templates/ThreatReport.html',
        controller: 'ThreatReportCtrl'

      });


  // if none of the above states are matched, use this as the fallback
  $urlRouterProvider.otherwise('start');

});
