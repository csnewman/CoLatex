$(document).ready(function () {
    var jwtToken = window.localStorage.getItem('token');

    loadProjects();

    $('#modal-rename').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);

        $('#modal-rename-name').val('');
        $('#modal-rename-message').text('');
        $('#modal-rename-name').parent().removeClass('has-error');
        $('#modal-rename-name-message').text('');

        $('#modal-rename-id').val(button.data('project'));
    });

    $('#modal-delete').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);

        $('#modal-delete-message').text('');

        $('#modal-delete-id').val(button.data('project'));
    });

    $('#modal-rename-submit').click(function () {
        $('#modal-rename-message').text('');

        $('#modal-rename-name').parent().removeClass('has-error');
        $('#modal-rename-name-message').text('');

        if ($('#modal-rename-name').val() === '') {
            $('#modal-rename-name').parent().addClass('has-error');
            $('#modal-rename-name-message').text('New name cannot be empty');
            return;
        }

        $.ajax({
            url: '/api/projects/rename',
            type: "POST",
            data: JSON.stringify($('#form-rename').serializeFormJSON()),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    $('#tbl-projects-body').empty();
                    loadProjects();
                } else {
                    switch (data.error) {
                        case 'internalError':
                            $('#modal-rename-message').text('Internal error occurred, please try again later.');

                            break;
                        default:
                            $('#modal-rename-message').text('Unspecified error occurred');

                            break;
                    }
                }
            },
            error: function (data) {
                $('#modal-rename-message').text('Today is not your lucky day, pal');
            }
        });
    });

    $('#modal-delete-submit').click(function () {
        $('#modal-delete-message').text('');

        $.ajax({
            url: '/api/projects/delete',
            type: "POST",
            data: JSON.stringify($('#form-delete').serializeFormJSON()),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    $('#tbl-projects-body').empty();
                    loadProjects();
                } else {
                    switch (data.error) {
                        case 'internalError':
                            $('#modal-delete-message').text('Internal error occurred, please try again later.');

                            break;
                        default:
                            $('#modal-delete-message').text('Unspecified error occurred');

                            break;
                    }
                }
            },
            error: function (data) {
                $('#modal-delete-message').text('Today is not your lucky day, pal');
            }
        });
    });

    function loadProjects() {
        $.ajax({
            url: "/api/projects/list",
            dataType: 'json',
            success: function (data, status) {
                for (var index in data.projects) {
                    var project = data.projects[index];

                    var date = new Date(project.lastEdit * 1000);

                    $('#tbl-projects-body').append('<tr><td>' + project.name + '</td><td>' + timeSince(date) + '</td><td>' + project.owner + '</td><td>'
                        + '<a class="btn btn-primary btn-xs" data-toggle="modal" data-target="#modal-rename" data-project="' + project.id + '" title="Rename"><i class="glyphicon glyphicon-pencil"></i></a> '
                        + '<a class="btn btn-danger btn-xs" data-toggle="modal" data-target="#modal-delete" data-project="' + project.id + '" title="Delete"><i class="glyphicon glyphicon-trash"></i></a>'
                        + '</td></tr>');
                }
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + jwtToken); } //set tokenString before send
        });
    }

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