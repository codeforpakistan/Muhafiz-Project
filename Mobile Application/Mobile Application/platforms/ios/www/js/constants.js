/**
 * Created by maheen on 6/14/2016.
 * asssignin valuse to varibles and these varibles are used in differrnt pages
 */

angular.module('Starting_Point.Constants', [])
    .value('user',{
        //label used in pendingregistration
        'label_Message':"Thank you for signing up. You will be notified when your registration request has been approved",
        //labels used in login and registration page profile seetin and threat report
        'label_Name':"Name:",
        'label_WorkedAt':"Work place:",
        'label_EmailAddress':"Email Address:",
        'label_password':"Password:",
        'label_confirmpassword':"Confirm Password:",
        'label_Stationed_At':"Stationed at:(city)",
        'label_Mobile_No':"Mobile no:",
        'label_Role':"Role:",
        'label_passwordchange':"You want to change Your Password",
         'label_Threatlevel':"Threat level:",
        'label_Detail_of_Threat':"Is this related to a previouly reported threat?",




    });