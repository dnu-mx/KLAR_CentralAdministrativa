// Session Timeout client side. Master and Detail detection. 
/** 
* Detecting in Master and Detail, Mouse and Keyboard events. 
* Using postMessage (HTML5) 
* @autor Camilo Martinez [â‚¬quiman], http://gplus.to/equiman 
* @created 2012-04-11 
* @updated 2012-05-02 
* @link http://www.inaltec.com.co/ 
* @licence CC BY-SA http://creativecommons.org/licenses/by-sa/3.0/ 
*/
// URL Redirection 
var timeOutSession = function () {
    top.location.replace("LogOut.aspx");
};
// Add this into Master Page (TabPanelContain): 
// <DocumentReady Handler="masterTimeOut();" /> 
var masterTimeOut = function () {
    //Convert minutes value indicated in TimeOut del Web.Config to seconds 
    var min = 0;
    var sec = 0;
    var timer = null;
    var vis = false;
    doTimer = function () {
        // At least 1 min, show the countdown window 
        if ((min - sec) < 60) {
            updateTime();
            if (frmTimeOut.hidden) {
                vis = true;
                frmTimeOut.setVisible(true);
            }
        }
        else {
            if (!frmTimeOut.hidden) {
                vis = false;
                frmTimeOut.setVisible(false);
            }
        }
        // Check the countdown counter 
        if ((min - sec) > 0) {
            sec++;
            // Repeat the process each minute 
            timer = setTimeout('doTimer()', 1000);
        }
        else {
            // When countdown is finish, redirect to Login page 
            timeOutSession();
        }
    };
    stopCount = function () {
        if (vis === true) {
            //2 seconds Idle (no detect events, because showing alert window is detected as mousemove) 
            if ((min - sec) < 58) {
                vis = false;
            }
        }
        else {
            // Convert minutes to second, minus 10 to be sure Client TimeOut occurs first than Server Timeout 
            min = (document.getElementById('txtMinTimeOut').value * 60) - 10;
            sec = 0;
            clearTimeout(timer);
            doTimer();
        }
    };
    updateTime = function () {
        var message = 'Inactivity was detected. Your session will expire in';
        var time = (min - sec);
        var unity = 'seconds.';
        Ext.getCmp('lblText').setText(message + ' ' + time + ' ' + unity);
    };
    // Start counter on Load 
    document.onload = function () {
        stopCount();
        //return false; 
    };
    // Star counter when mouse move 
    document.onmousemove = function () {
        stopCount();
        //return false; 
    };
    // Star counter when key is pressed 
    document.onkeypress = function () {
        stopCount();
        //return false; 
    };
    // Read and event when is send from an iFrame 
    function displayMessage(e) {
        if ((e.origin.split(":", 2)[0] + ":" + e.origin.split(":", 2)[1]) === (GetNewPath("/").split(":", 2)[0] + ":" + GetNewPath("/").split(":", 2)[1])) {
            // If the iFrame send any of this events start counter 
            switch (e.data) {
                case "onload":
                case "onmousemove":
                case "onkeypress":
                case "simulated":
                    stopCount();
            }
        }
    };
    if (window.addEventListener) {
        // For standards-compliant web browsers 
        window.addEventListener("message", displayMessage, false);
    }
    else {
        window.attachEvent("onmessage", displayMessage);
    };
};
// Add this into any iFrame Page: 
// <DocumentReady Handler="detailTimeOut();" /> 
var detailTimeOut = function () {
    // Start counter on Load 
    document.onload = function () {
        top.postMessage("onload", GetNewPath("/"));
        return false;
    };
    // Star counter when mouse move 
    document.onmousemove = function () {
        top.postMessage("onmousemove", GetNewPath("/"));
        //return false; 
    };
    // Star counter when key is pressed 
    document.onkeypress = function () {
        top.postMessage("onkeypress", GetNewPath("/"));
        //return false; 
    };
    // Simulated Event 
    simulatedEvent = function () {
        top.postMessage("simulated", GetNewPath("/"));
        //return false; 
    };
};
// Get the Path when use VirtualPath in .Net or IIS 
var GetNewPath = function (relative_path) {
    var url = window.location.href;
    if (url.substring(url.length - 1, url.length) == '/') {
        url = url.substring(0, url.length - 1);
    }
    var url_parts = url.split('/');
    if (relative_path.substring(0, 1) != '/') {
        url_parts[url_parts.length - 2] = relative_path;
        url_parts[url_parts.length - 1] = '';
    }
    else {
        url_parts[url_parts.length - 2] = relative_path.substring(1);
        url_parts[url_parts.length - 1] = '';
    }
    var new_page_absolute_path = url_parts.join('/');
    if (new_page_absolute_path.substring(new_page_absolute_path.length - 1, new_page_absolute_path.length) == '/') {
        new_page_absolute_path = new_page_absolute_path.substring(0, new_page_absolute_path.length - 1);
    }
    return new_page_absolute_path;
};
