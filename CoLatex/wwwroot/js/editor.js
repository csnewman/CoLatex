$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const projectId = urlParams.get('project');

    var project;
    
    var jwtToken = window.localStorage.getItem('token');
    
    $.ajax({
        url: '/api/projects/info',
        type: "POST",
        data: '{"id":"' + projectId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            project = data;
            $('#editor-header').text(project.name);
        },
        error: function (data) {
            alert('Could not retrieve project');
            console.log(data);
        },
        beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + jwtToken); } //set tokenString before send
    });
});