/* eslint-disable */


jQuery(document).ready(function ($) {
    // jQuery code is in here

    //1-
    var fileinput = $("#inputGroupFile02");
    fileinput.on("change", function () {
        var files = fileinput[0].files;
        $.each(files, function (index, file) {
            if (!file.type.startsWith("image/")) {
                fileinput.val("");
                alert("please select image file");
                return false;
            } else {
                //console.log("File  index: " + (index ) + " file name : " + file.name + " file  type: " + file.type );
            }
        });
    });
    //var preventScrollElement = $('.prevent-scroll');
    //preventScrollElement.on("click", function (event) {
    
    //    event.preventDefault(); // Prevent the default behavior
    //    /*var href = $(this).attr("href");*/
    //    // Handle the navigation or other logic as needed
    //    // ...
    //});


});

//2-
function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = "deleteSpan_" + uniqueId;
    var confirmDeleteSpan = "confirmDeleteSpan_" + uniqueId;

    if (isDeleteClicked) {
        $("#" + deleteSpan).hide();
        $("#" + confirmDeleteSpan).show();
    } else {
        $("#" + deleteSpan).show();
        $("#" + confirmDeleteSpan).hide();
    }
}

//3-

function updateTimer() {
    //console.log("updateTimer start")

    var csharp_TotalMillisecondsToExpireWithoutaddCurrentMilieseconds = parseInt(
        document.getElementById(
            "csharp_TotalMillisecondsToExpireWithoutaddCurrentMilieseconds"
        ).innerHTML
    );
    var csharp_TotalMillisecondToExpireAfteraddCurrentMilieseconds = parseInt(
        document.getElementById(
            "csharp_TotalMillisecondToExpireAfteraddCurrentMilieseconds"
        ).innerHTML
    );

    var CSharpCurrentMilieseconds = parseInt(
        document.getElementById("CSharpCurrentMilieseconds").innerHTML
    );
    var JSCurrentMilliseconds = new Date().getTime();

    //console.log("CSharpCurrentMilieseconds"+CSharpCurrentMilieseconds)
    //console.log("JSCurrentMilliseconds"+JSCurrentMilliseconds)
    //console.log("csharp_TotalMillisecondsToExpireWithoutaddCurrentMilieseconds"+csharp_TotalMillisecondsToExpireWithoutaddCurrentMilieseconds)
    //console.log("csharp_TotalMillisecondToExpireAfteraddCurrentMilieseconds"+csharp_TotalMillisecondToExpireAfteraddCurrentMilieseconds)

    now = Date.parse(new Date());

    if (cookieExpirationDate != null) {
        diff = cookieExpirationDate - now;
    } else {
        diff = csharp_TotalMillisecondToExpireAfteraddCurrentMilieseconds - now;
    }

    days = Math.floor(diff / (1000 * 60 * 60 * 24));
    hours = Math.floor(diff / (1000 * 60 * 60));
    mins = Math.floor(diff / (1000 * 60));
    secs = Math.floor(diff / 1000);

    d = days;
    h = hours - days * 24;
    m = mins - hours * 60;
    s = secs - mins * 60;

    if (diff <= 900) {
        d, h, m, (s = 0);
        clearInterval(interval);

        // call this function when you want to show the alert
        showAlert();
    }

    document.getElementById("timer").innerHTML =
        "<div>" +
        d +
        "<span>Days</span></div>" +
        "<div>" +
        h +
        "<span>Hours</span></div>" +
        "<div>" +
        m +
        "<span>Minutes</span></div>" +
        "<div>" +
        s +
        "<span>Seconds</span></div>";

    //console.log(diff);
    //console.log(diff <= 0);
}

function showAlert() {
    console.log("showAlert start");

    // Show the alert
    $("#Logout_alert").fadeIn();
    setTimeout(function () {
        window.location.href = "/account/login";
    }, 5000);
}

var cookieValue;
var cookieExpirationDate;
var checkCookieExistenceVariable;

async function checkCookieExistence() {
    //console.log("checkCookieExistence start")

    await cookieStore.get(".AspNetCore.Identity.Application").then((cookies) => {
        cookieValue = cookies.name;
        cookieExpirationDate = cookies.expires;
    });

    //checkCookieExistenceVariable = !!cookieValue && !!cookieExpirationDate;
    checkCookieExistenceVariable = !!cookieValue;
    //console.log(cookieValue);
    //console.log(cookieExpirationDate);
    //console.log(checkCookieExistenceVariable);

    return "Done";
}

async function checkCondition() {
    //console.log("checkCondition start")

    await checkCookieExistence();

    if (checkCookieExistenceVariable) {
        interval = setInterval("updateTimer()", 1000);

        // cookie exists
        if (cookieExpirationDate != null) {
            //after claim the name and expire, we can call the updateTimer now
            //    interval = setInterval('updateTimer()', 1000);
        }
    }
}

checkCondition();
