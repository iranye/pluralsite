
var minId = 1;
var currId = 1;
var maxId = 99;
document.addEventListener('keydown', function (event) {
    if (event.ctrlKey && event.key === 'ArrowDown') {
        updateUrl(getNextImgId());
    }

    if (event.ctrlKey && event.key === 'ArrowUp') {
        updateUrl(getPrevImgId());
    }

    if (event.ctrlKey && event.key === 'b') {
        setCurrInd();
    }
});

function setMaxId(id) {
    maxId = id;
}

function updateUrl(id) {
    var idStr = pad(id, 3);
    var newHashtagLink = `#card${idStr}`;
    console.log(newHashtagLink);
    var currentHref = window.location.href;
    var hashTagInd = currentHref.lastIndexOf('#');
    if (hashTagInd > -1) {
        currentHref = currentHref.substr(0, hashTagInd);
    }
    currentHref += newHashtagLink;
    window.location.href = currentHref;
}

function getNextImgId() {
    currId++;
    if (currId > maxId) {
        currId = minId;
    }
    return currId;
}

function getPrevImgId() {
    currId--;
    if (currId < minId) {
        currId = maxId;
    }
    return currId;
}

function pad(num, size) {
    num = num.toString();
    while (num.length < size) num = "0" + num;
    return num;
}

function setCurrInd(ind) {
	if (!ind) {
		var str = prompt("Enter a Card Number to Switch to", "1");
		var ind = parseInt(str, 10);
		if (isNaN(ind)) {
			console.log("Invalid input!");
		}
	}
    currId = ind;
    updateUrl(currId);
}
