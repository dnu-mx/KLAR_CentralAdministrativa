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
