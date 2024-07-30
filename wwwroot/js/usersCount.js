
// create connectioin
var connectionUserCount = new signalR.HubConnectionBuilder().withUrl("hubs/userCount").build();

connectionUserCount.on("updateTotalViews", (value) => {
    document.getElementById("totalViewsCounter").innerText = value.toString();
})

connectionUserCount.on("updateTotalUsers", (value) => {
    document.getElementById("totalUsersCounter").innerText = value.toString();
})
function newWindowLoaderOnClient() {
    connectionUserCount.send("newWindowLoaded");
}

function fulfilled() {
    console.log("connection with signalR successful");
    newWindowLoaderOnClient();
}

function rejected() {
    console.log("log the rejected issue");
}

connectionUserCount.start().then(fulfilled, rejected);