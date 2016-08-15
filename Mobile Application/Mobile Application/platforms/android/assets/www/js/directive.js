/**
 * Created by maheen on 6/14/2016.
 * this direcytive is used to compare the password and confirm password on run time
 */
angular.module('Starting_Point.directive', [])
    .directive('validPasswordC', function () {
        return {
            require: 'ngModel',
            scope: {

                reference: '=validPasswordC'

            },
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    var noMatch = viewValue != scope.reference
                    ctrl.$setValidity('noMatch', !noMatch);
                    return (noMatch) ? noMatch : !noMatch;
                });

                scope.$watch("reference", function (value) {
                    ;
                    ctrl.$setValidity('noMatch', value === ctrl.$viewValue);

                });
            }
        }
    });


