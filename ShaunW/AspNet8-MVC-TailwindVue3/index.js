console.log("Hello World");

const theForm = document.getElementById("theForm");
theForm.hidden = true;

const showButton = document.getElementById("showButton");
showButton.addEventListener("click", () => {
    if (theForm.hidden) {
        showButton.innerText = "Hide Form";
        theForm.hidden = false;
    } else {
        showButton.innerText = "Show Form";
        theForm.hidden = true;
    }
});