﻿////function getFitbitToken() {
////    var accessToken = localStorage.getItem('fitbit_access_token')
////    if (accessToken !== null) {
////        // check if token is still valid
////        var expires_at = localStorage.getItem('fitbit_expires_at')
////        var expires = new Date(decodeHtml(expires_at));
////        var currentDate = new Date(Date.now())
////        if (expires < currentDate) {
////            // no longer valid, continue
////            localStorage.clear();
////        }
////        else {
////            // still valid, just return token
////            return accessToken;
////        }
////    }

////    // open new window to login to spotify
////    popup = window.open(
////        '/fitbit/auth',
////        'Login with Fitbit',
////        'width=800,height=600'
////    )

////    var timer = setInterval(function () {
////        // try getting access token from storage
////        var accessToken = localStorage.getItem('fitbit_access_token')
////        if (accessToken !== null) {
////            clearInterval(timer);
////            return accessToken;
////        }
////    }, 500);
////}

////function decodeHtml(html) {
////    var txt = document.createElement("textarea");
////    txt.innerHTML = html;
////    return txt.value;
////}