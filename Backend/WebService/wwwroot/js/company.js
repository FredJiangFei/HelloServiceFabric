function getCompany() {
    var http = new XMLHttpRequest();
    http.onreadystatechange = function () {
        if (http.readyState === 4) {
            end = new Date().getTime();
            if (http.status < 400) {
                returnData = JSON.parse(http.responseText);
                if (returnData) {
                    countDisplay.innerHTML = returnData.count;
                    updateFooter(http, (end - start));
                }
            } else {
                updateFooter(http, (end - start));
            }
        }
    };
    start = new Date().getTime();
    http.open("GET", "/api/ActorBackendService/?c=" + start);
    http.send();
}

function newActor() {
    var http = new XMLHttpRequest();
    http.onreadystatechange = function () {
        if (http.readyState === 4) {
            end = new Date().getTime();
            if (http.status < 400) {
                returnData = JSON.parse(http.responseText);
                if (returnData) {
                    updateFooter(http, (end - start));
                }
            } else {
                updateFooter(http, (end - start));
            }
        }
    };
    start = new Date().getTime();
    http.open("POST", "/api/ActorBackendService/?c=" + start);
    http.send();
}

