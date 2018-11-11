$(document).ready(function () {
    var jwtToken = "hi";
    $.ajax({
        url: "/api/projects/list",
        dataType: 'json',
        success: function (data, status) {
            console.log(data);
        },
        beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + jwtToken); } //set tokenString before send
    });
});