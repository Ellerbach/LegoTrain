function btnclicked(boxMSG, cmdSend) {
    var xhr = new XMLHttpRequest();
    boxMSG.innerHTML = "Waiting";
    xhr.open('GET', cmdSend + '&_=' + Math.random());
    xhr.send(null); xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200)
                boxMSG.innerHTML = xhr.responseText;
            else
                boxMSG.innerHTML = "Error";
        }
    };
}
function findTop(iobj) { ttop = 0; do { ttop += iobj.offsetTop || 0; iobj = iobj.offsetParent; } while (iobj); return ttop; }
function findLeft(iobj) { tleft = 0; do { tleft += iobj.offsetLeft || 0; iobj = iobj.offsetParent; } while (iobj); return tleft; }
//request switch change
function swclicked(boxMSG, cmdSend) {
    var xhr = new XMLHttpRequest();
    var mycmd = cmdSend + '&md=';
    if (boxMSG.src.indexOf('trn.png') >= 0)
        mycmd += 'false';
    else
        mycmd += 'true';
    xhr.open('GET', 'api/switch?' + mycmd + '&_=' + Math.random());
    xhr.send(null); xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200)
                if (boxMSG.src.indexOf('trn') >= 0)
                    boxMSG.src = 'config/switch-' + boxMSG.getAttribute("data-type") + '-str.png';
                else
                    boxMSG.src = 'config/switch-' + boxMSG.getAttribute("data-type") + '-trn.png';
        }
    };
}
//request signal change
function siclicked(boxMSG, cmdSend) {
    var xhr = new XMLHttpRequest(); var mycmd = cmdSend + '&md=';
    if (boxMSG.src.indexOf('signal-green.png') >= 0) mycmd += '1'; else mycmd += '2';
    xhr.open('GET', 'api/signal?' + mycmd + '&_=' + Math.random());
    xhr.send(null); xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200)
                if (boxMSG.src.indexOf('red') >= 0) boxMSG.src = 'config/signal-green.png'; else boxMSG.src = 'config/signal-red.png';
        }
    };
}
//create the initial value for the switches
function buildSwitch(boxMSG, num) {
    var obj = document.getElementById(boxMSG);
    if (obj != null)
        if (num == 0) obj.src = 'config/switch-' + obj.getAttribute("data-type") + '-str.png'; else obj.src = 'config/switch-' + obj.getAttribute("data-type") + '-trn.png';
}
function getswitches() {
    var xhr = new XMLHttpRequest(); xhr.open('GET', 'api/switchstatus?_=' + Math.random()); xhr.send(null); xhr.onreadystatechange = function () {
        if (xhr.readyState == 4)
            if (xhr.status == 200) {
                let responseText = xhr.responseText; // "0=1;2=2;5=1"
                let pairs = responseText.split(';'); // ["0=1", "2=2", "5=1"]
                pairs.forEach(pair => {
                    let [key, value] = pair.split('=');
                    buildSwitch('switch' + key, value);
                });
            }
    };
}
//signals
function buildSignal(boxMSG, num) {
    var obj = document.getElementById(boxMSG);
    if (obj != null)
        if (num == 2)
            obj.src = 'config/signal-green.png';
        else if (num == 1)
            obj.src = 'config/signal-red.png';
        else obj.src = 'config/signal-black.png';
}
function getsignals() {
    var xhr = new XMLHttpRequest(); xhr.open('GET', 'api/signalstatus?_=' + Math.random()); xhr.send(null); xhr.onreadystatechange = function () {
        if (xhr.readyState == 4)
            if (xhr.status == 200) {
                let responseText = xhr.responseText; // "0=1;2=2;5=1"
                let pairs = responseText.split(';'); // ["0=1", "2=2", "5=1"]
                pairs.forEach(pair => {
                    let [key, value] = pair.split('=');
                    buildSignal('signal' + key, value);
                });
            }
    };
}
//initialization
function InitAll() { getsignals(); getswitches(); }
document.onreadystatechange = function () {
    if (document.readyState == "complete") {
        InitAll();
    }
}