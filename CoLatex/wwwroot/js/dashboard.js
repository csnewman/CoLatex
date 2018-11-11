$(document).ready(function () {
    alert("HI");
    $.getJSON('/api/projects/list', function (data) {
    });
});