$(document).ready(function () {
    var jwtToken = window.localStorage.getItem('token');
    $.ajax({
        url: "/api/projects/list",
        dataType: 'json',
        success: function (data, status) {
            for (var index in data.projects) {
                var project = data.projects[index];

                var date = new Date(project.lastEdit * 1000);

                $('#tbl-projects-body').append('<tr><td>' + project.name + '</td><td>' + timeSince(date) + '</td><td>' + project.owner + '</td><td>'
                    + '<a class="btn btn-primary btn-xs" href="javascript:void(0)" data-toggle="modal" data-target="#VisitorDelete" title="Rename"><i class="glyphicon glyphicon-pencil"></i></a> '
                    + '<a class="btn btn-danger btn-xs" href="javascript:void(0)" data-toggle="modal" data-target="#VisitorDelete" title="Remove"><i class="glyphicon glyphicon-trash"></i></a>'
                    +'</td></tr>');
            }
        },
        beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + jwtToken); } //set tokenString before send
    });
});

function timeSince(date) {

    var seconds = Math.floor((new Date() - date) / 1000);

    var interval = Math.floor(seconds / 31536000);

    if (interval > 1) {
        var iso = date.toISOString();
        return iso.substring(0, 10) + ' ' + iso.substring(11, 16);
    }
    interval = Math.floor(seconds / 2592000);
    if (interval > 1) {
        var iso = date.toISOString();
        return iso.substring(0, 10) + ' ' + iso.substring(11, 16);
    }
    interval = Math.floor(seconds / 86400);
    if (interval > 1) {
        return interval + " days ago";
    }
    interval = Math.floor(seconds / 3600);
    if (interval > 1) {
        return interval + " hours ago";
    }
    interval = Math.floor(seconds / 60);
    if (interval > 1) {
        return interval + " minutes ago";
    }
    return Math.floor(seconds) + " seconds ago";
}