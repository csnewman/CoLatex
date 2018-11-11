$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const projectId = urlParams.get('project');

    var project;
    
    var jwtToken = window.localStorage.getItem('token');

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/api/projects/hub", {
            accessTokenFactory: () => jwtToken
        })
        .build();

    loadProject();

    function loadProject() {
        $.ajax({
            url: '/api/projects/info',
            type: "POST",
            data: '{"projectId":"' + projectId + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                project = data.project;
                $('#editor-header').text(project.name);
                $('#modal-project-name').val(project.name);
                $('#modal-project-owner').val(project.owner);

                for (var index in project.collaborators) {
                    $('#modal-project-collaborators').append('<option>' + project.collaborators[index] + '</option>');
                }
            },
            error: function (data) {
                alert('Could not retrieve project');
                console.log(data);
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + jwtToken); } //set tokenString before send
        });
    }

    function loadFiles() {

    }

    $('#modal-project').on('show.bs.modal', function (event) {
        $('#modal-project-message').text('');
        $('#modal-project-collaborator').val('');
    });

    $('#modal-project-collaborators-add-submit').click(function (e) {
        $('#modal-project-message').text('');

        if ($('#modal-project-collaborator').val() === '') {
            $('#modal-project-message').text('Please enter a username');
            return;
        }

        $.ajax({
            url: '/api/projects/add-collaborator',
            type: "POST",
            data: JSON.stringify($('#form-project').serializeFormJSON()),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                loadProject();
            },
            error: function (data) {
                $('#modal-project-message').text('Today is not your lucky day, pal');
                console.log(data);
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + jwtToken); } //set tokenString before send
        });

        e.stopPropagation();
    });

    $('#modal-project-collaborators-remove-submit').click(function (e) {
        $('#modal-project-message').text('');

        if ($('#modal-project-collaborator').val() === '') {
            $('#modal-project-message').text('Please enter a username');
            return;
        }

        $.ajax({
            url: '/api/projects/remove-collaborator',
            type: "POST",
            data: JSON.stringify($('#form-project').serializeFormJSON()),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                loadProject();
            },
            error: function (data) {
                $('#modal-project-message').text('Today is not your lucky day, pal');
                console.log(data);
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + jwtToken); } //set tokenString before send
        });

        e.stopPropagation();
    });
});