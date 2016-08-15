angular.module('Starting_Point.controllers', [])

        /*===========Start Controller============= */
    .controller('startCtrl', function ($scope, $ionicPopup, $location, $state, $localstorage) {
        /*=====Checking if the user is login======= */
       if (typeof($localstorage.get('SESSION_USER')) === 'string')
           $state.go('menu.UserHome');


        /*===definition of function wriiten behind the buttons====*/
        $scope.RedirecttoLogin = function () {
            $state.go('Login');
        }

        $scope.RedirecttoRegister = function () {
            $state.go('Registration');


        }

    })

    /*==============App controller====================*/
    .controller('menuCtrl', function($scope, $ionicSideMenuDelegate,$state,$ionicHistory) {

        /*======defination of function called when button is pressed=====*/
        $scope.ProfileSettings =
            function () {
             $state.go('ProfileSetting');

            }

        $scope.ThreatReport=
            function () {
                $state.go('ThreatReport');

            }

        $scope.LogOut = function () {

                window.localStorage.clear();
                $ionicHistory.clearCache();
                $ionicHistory.clearCache().then(function(){
                    $state.go('start');
                });

                //$state.go('start');

        }


    })

    /*==========================LOGIN CONTROLLER========================*/
    .controller('LoginCtrl', function ($scope, $http, Ajaxfact, $ionicPopup, $state, user, $localstorage, $ionicLoading) {

        $scope.data = {emailid:""};
        //Function called when back button is pressed

        $scope.redirecttohome = function () {
            $state.go('start');
        }
        //getting control of all labels value from constantsfile

        $scope.data.labels = user;

        //Function called when Login button is pressed
        $scope.login = function () {

            $ionicLoading.show({
                content: 'checking credentials',
                animation: 'fade-in',
                showBackdrop: true,
                maxWidth: 200,
                showDelay: 0
            });

            /*=========Request send when the user enter email and password======*/
            Ajaxfact.Request('GET', 'LoginandRegistrationscripts.php?names=' + $scope.data.emailid + '&password=' + $scope.data.password, "").success(function (data) {


                $ionicLoading.hide();
                if (data == "0") {
                    var alertPopup = $ionicPopup.alert({
                        title: 'Invalid Username Or Password'
                    });

                }

                else {
                    $scope.status=data.RegistrationStatus;
                        if($scope.status=="Approved") {
                            $localstorage.set('SESSION_USER', JSON.stringify(data));
                            $state.go('menu.UserHome');

                        }
                        else{
                            var alertPopup = $ionicPopup.alert({
                                title: 'Your resgistration is not approved by admin'
                            });

                        }
                }
            }).error(function (data) {
                $ionicLoading.hide();
                var alertPopup = $ionicPopup.alert({
                    title: 'Error!',
                    template: 'An error occured while processing your request kindly check your internet connection!' //+ data
                });
            });
        }
    })


    /*==========================REGISTRATION CONTROLLER========================*/
    .controller('RegisterCtrl', function ($scope, $http, Ajaxfact, $ionicPopup, $state, user, $ionicLoading) {

        $scope.data = {
            radioValue: ''
        };
        //variable for getting the selected value form dropdown

        var selectedvalue = -1;
        var status = "Pending";
        $scope.data.labels = user;
        //getting control of all labels value from constantsfile

        //Function called when value from dropdown is selected

        $scope.showSelectValue = function (mySelect) {
            selectedvalue = mySelect;

        }
        //Function called when back button is pressed

        $scope.redirecttohome =
            function () {
                $state.go('start');

            }


        $scope.Register = function () {


            $ionicLoading.show({
                content: 'Loading',
                animation: 'fade-in',
                showBackdrop: true,
                maxWidth: 200,
                showDelay: 0
            });


            var date = new Date();
            var regdate = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);

            if ($scope.data.radioValue == 'Freelance') {
                $scope.data.Organization = "Freelancer";
            }


            var data = {
                FNAME: $scope.data.FName,
                EMAILID: $scope.data.emailid,
                RADIOVALUE: $scope.data.radioValue,
                ORGANIZATION: $scope.data.Organization,
                PHONENO: $scope.data.phoneno,
                STATIONEDAT: $scope.data.StationedAt,
                SELECTEDVALUE: selectedvalue,
                PASSWORD: $scope.data.password,
                REGSTATUS: "Pending",
                REGDATE: regdate
            }

            /*====Sending data to store in database=====*/

            Ajaxfact.Request('POST', 'LoginandRegistrationscripts.php', data).success(function (data) {
                $ionicLoading.hide();

                var resdata = data;
                if (resdata.match("Mobileno")) {
                    var alertPopup = $ionicPopup.alert({
                      //  title: 'Success',
                        template:'Account with this mobile already exist'
                    });

                }
                else if (resdata.match("Email")) {
                    var alertPopup = $ionicPopup.alert({
                       // title: 'Success',
                        template:'Account with this email already exist'
                    });

                }
                else if (resdata.match("inserted successfully")) {
                    var alertPopup = $ionicPopup.alert({
                        title: 'Success',
                        template:'Thank You! Your information is collected Successfully'
                    });
                    $state.go('pendingRegistration');

                }
                else {

                    var alertPopup = $ionicPopup.alert({
                        title: 'Invalid Credentials',
                        template: 'could not received your data successfully!' + data
                    });
                }

            }).error(function (data) {
                $ionicLoading.hide();
                var alertPopup = $ionicPopup.alert({
                     title: 'Error!',
                    template: 'An error occured while processing your request kindly check your internet connection!' //+ data
                });
            });


        }


    })
    /*==========================Pending Registration CONTROLLER========================*/

    .controller('pendingRegistrationCtrl', function ($scope, $state, user) {
        $scope.data = {};
        $scope.data.labels = user;
        /*====redirect to start page=====*/
        $scope.redirecttohome =
            function () {
                $state.go('start');

            }


    })
    /*==========================UserHome CONTROLLER========================*/

    .controller('UserHomeCtrl', function ($scope, $http, Ajaxfact,ConnectivityMonitor, $ionicPopup, $ionicLoading,$cordovaNetwork,$stateParams, $state,$rootScope, user, $localstorage, $ionicHistory) {

        $scope.data = {};
        $scope.data.display = true;
        var ooflinelocalstorage=[];

        /*Retrieving data from session object*/
        var User = JSON.parse($localstorage.get('SESSION_USER'));
        $scope.data.Name = User.Name;
        $scope.data.Mobileno = User.Mobileno;
        $scope.data.workat = User.OrganizationName;
        $scope.data.reg_id = User.Registration_Id;
        $scope.data.Email = User.Email;
        $scope.data.Address = User.Address;
        $scope.data.Role = User.Role;
        $scope.data.Password = User.Password;

        /*====getting control over the labels names from constant file*/
        $scope.data.labels = user;

        /*=========Definition of function written behind the buttons=====*/

        $scope.LogOut =
            function () {


                window.localStorage.clear();
                $ionicHistory.clearCache();
				$ionicHistory.clearCache().then(function(){
					$state.go('start');
				});

                //$state.go('start');

            }

        $scope.PanicAttack=
            function() {
                var date = new Date();
                var regdate = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);

                var data = {
                    REGID: $scope.data.reg_id,
                    REGDATE: regdate,
                    NAME: $scope.data.Name,
                    MOBILENO: $scope.data.Mobileno,
                    ROLE:$scope.data.Role,
                    ORGANIZATIONNAME:$scope.data.workat
                }
				
				/*===============================*/
		
        var number = "03425063376"
        var message = "hi i m hell";
        

        //CONFIGURATION
        var options = {
            replaceLineBreaks: false, // true to replace \n by a new line, false by default
            android: {
                intent: 'INTENT'  // send SMS with the native android SMS messaging
                //intent: '' // send SMS without open any other app
            }
        };

        var success = function () { alert('Message sent successfully'); };
        var error = function (e) { alert('Message Failed:' + e); };
        sms.send(number, message, options, success, error);
		
		
    
				
				
				
				
				
				/*================================*/
				
				
				
				
                $ionicLoading.show({
                    content: 'Loading',
                    animation: 'fade-in',
                    showBackdrop: true,
                    maxWidth: 200,
                    showDelay: 0
                });
				
				
				

                /*====Check if the device is ofline=======*/
                if (ConnectivityMonitor.isOffline()) {

                    $ionicLoading.hide();
                    /*checking if the user login for the first time*/
                    if(typeof($localstorage.get('OFFLINE_REQUESTS')) != 'string') {

                        ooflinelocalstorage.push(data);
                        $localstorage.set("OFFLINE_REQUESTS", JSON.stringify(ooflinelocalstorage));
                        ooflinelocalstorage.pop();
                    }
                    else{


                        var offline_data  = JSON.parse($localstorage.get("OFFLINE_REQUESTS"));
                        console.log(offline_data.length)
                        offline_data.push(data);
                        $localstorage.set("OFFLINE_REQUESTS", JSON.stringify(offline_data));

                    }
                    var alertPopup = $ionicPopup.alert({
                        title: 'msg',
                        template: 'u r offline your request is being saved and sms is being sent to muhafiz team'
                    });


                }
                /*----Checking f the deice is online-----*/
                if (ConnectivityMonitor.isOnline()) {


                    Ajaxfact.Request('POST', 'PanicAttackScripts.php', data).success(function (data) {
                        $ionicLoading.hide();
                        var alertPopup = $ionicPopup.alert({
                            title: 'success',
                            template: 'panic attack send'+data
                        });

                    }).error(function (data) {
                        $ionicLoading.hide();
                        var alertPopup = $ionicPopup.alert({
                            title: 'error',
                            template: 'errrrrrrr'
                        });
                    });


                }
            }


         /*to show previous reports in table*/
        $scope.PreviousReports =
            function () {

                $scope.ThreatReports = [];
                $scope.data.display = false;

                $ionicLoading.show({
                    content: 'Loading',
                    animation: 'fade-in',
                    showBackdrop: true,
                    maxWidth: 200,
                    showDelay: 0
                });
                
                Ajaxfact.Request('GET', 'ThreatReportScripts.php?REG_ID=' + $scope.data.reg_id, "").success(function (data) {
                    $ionicLoading.hide();
                    if (data == "0") {
                        var alertPopup = $ionicPopup.alert({
                            title: "status",
                            template: 'No Threat Reports to show'


                        });

                    }

                    else {


                        $scope.ThreatReports = data;


                    }
                }).error(function (data) {
                    $ionicLoading.hide();
                    var alertPopup = $ionicPopup.alert({
                        title: 'Error!',
                    template: 'An error occured while processing your request kindly check your internet connection!' //+ data
                    });
                });
            }


    })


    /*==========================ProfileSetting CONTROLLER========================*/

    .controller('ProfileSettingCtrl', function ($scope, $http, Ajaxfact, $ionicPopup, $stateParams, $state, user, $localstorage, $ionicLoading) {

        $scope.data = {};
        $scope.data.labels = user;
    /*======retriving data from storage=========*/
        var User = JSON.parse($localstorage.get('SESSION_USER'));
        $scope.data.FName = User.Name;
        $scope.data.phoneno = User.Mobileno;
        $scope.data.Organization = User.OrganizationName;
        $scope.data.reg_id = User.Registration_Id;
        $scope.data.emailid = User.Email;
        $scope.data.Role = User.Role;
        $scope.data.StationedAt = User.StationedAt;


        /*==Function definations==*/


        $scope.Home = function () {

            $state.go('menu.UserHome');
        }
        /*===Sending the request to update profile*/
        $scope.SaveInfo = function () {
			


            var date = new Date();
            var regdate = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);

            var data={
                REGID: $scope.data.reg_id,
                STATUS:"pending",
                REGDATE: regdate,
				EMAIL:$scope.data.emailid
            }
			 
            $ionicLoading.show({
                content: 'Loading',
                animation: 'fade-in',
                showBackdrop: true,
                maxWidth: 200,
                showDelay: 0
            });

            Ajaxfact.Request('POST', 'ProfileSettingsScripts.php', data).success(function (data) {
                $ionicLoading.hide();
                var alertPopup = $ionicPopup.alert({
                    title: 'Success !',
                    template: 'Your request is  send successfully wait for call' //+data
                });

            }).error(function (data) {
                $ionicLoading.hide();
                var alertPopup = $ionicPopup.alert({
                    title: 'Error!',
                    template: 'An error occured while processing your request kindly check your internet connection!' //+ data
                });
            });


        }


    })
    /*==========================ThreatReportCONTROLLER========================*/
    .controller('ThreatReportCtrl', function ($scope, $state, $ionicPopup, Ajaxfact, $stateParams, user, $localstorage, $ionicLoading) {
        //Function called when back button is pressed

        $scope.data = {};
        //getting control of all labels value from constantsfile
        $scope.data.labels = user;
        /*===retrieving data from session===*/
        var User = JSON.parse($localstorage.get('SESSION_USER'));
        $scope.data.Name = User.Name;
        $scope.data.Mobileno = User.Mobileno;
        $scope.data.workat = User.OrganizationName;
        $scope.data.reg_id = User.Registration_Id;
        $scope.data.Email = User.Email;
        $scope.data.Role = User.Role;


        //Function called when back button is pressed
        $scope.Home = function () {

            $state.go('menu.UserHome');
        }

/*========Request to send threat if any=====*/
        $scope.SubmitThreat =
            function () {

                $ionicLoading.show({
                    content: 'processing',
                    animation: 'fade-in',
                    showBackdrop: true,
                    maxWidth: 200,
                    showDelay: 0
                });

                var data = {
                    REGID: $scope.data.reg_id,
                    LEVELOFTHREAT: $scope.data.Threatlevel,
                    THREATSTATUS: $scope.data.Threatstatus,
                    NAME: $scope.data.Name,
                    MOBILENO: $scope.data.Mobileno,
                    ROLE:$scope.data.Role,
                    ORGANIZATIONNAME:$scope.data.workat
                }


                Ajaxfact.Request('POST', 'ThreatReportScripts.php', data)
                    .success(function (data) {
                        $ionicLoading.hide();

                        var alertPopup = $ionicPopup.alert({
                            title: 'Success', //+ data
                            template:'Report submitted successfully!!'+data
                        });



                    }).error(function (data) {
                    $ionicLoading.hide();
                    var alertPopup = $ionicPopup.alert({
                        title: 'Error!',
                    template: 'An error occured while processing your request kindly check your internet connection!' //+ data
                    });

                });



            }


    });

