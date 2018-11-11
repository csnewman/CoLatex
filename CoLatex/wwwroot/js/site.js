$(document).ready(function () {
    var jwtToken = window.localStorage.getItem('token');

    if (jwtToken == null) {
        $('#nav-login').css('display', 'block');
        $('#nav-signup').css('display', 'block');
        $('#jumb-nouser').css('display', 'block');
    } else {
        $('#nav-user-name').text(window.localStorage.getItem('username'));
        $('#nav-user').css('display', 'block');
        $('#jumb-loggedin').css('display', 'block');
    }

    $('#modal-signup-submit').click(function () {
        $('#modal-signup-message').text('');

        $('#modal-signup-username').parent().removeClass('has-error');
        $('#modal-signup-username-message').text('');

        $('#modal-signup-name').parent().removeClass('has-error');
        $('#modal-signup-name-message').text('');

        $('#modal-signup-password').parent().removeClass('has-error');
        $('#modal-signup-password-message').text('');

        $('#modal-signup-password-confirm').parent().removeClass('has-error');
        $('#modal-signup-password-confirm-message').text('');

        $('#modal-signup-email').parent().removeClass('has-error');
        $('#modal-signup-email-message').text('');

        if ($('#modal-signup-username').val() === '') {
            $('#modal-signup-username').parent().addClass('has-error');
            $('#modal-signup-username-message').text('Username cannot be empty');
            return;
        }

        if ($('#modal-signup-name').val() === '') {
            $('#modal-signup-name').parent().addClass('has-error');
            $('#modal-signup-name-message').text('Name cannot be empty');
            return;
        }

        if (!isValidEmailAddress($('#modal-signup-email').val())) {
            $('#modal-signup-email').parent().addClass('has-error');
            $('#modal-signup-email-message').text('Please enter a valid email');
            return;
        }

        if ($('#modal-signup-password').val() === '') {
            $('#modal-signup-password').parent().addClass('has-error');
            $('#modal-signup-password-message').text('Password cannot be empty');
            return;
        }

        if ($('#modal-signup-password-confirm').val() !== $('#modal-signup-password').val()) {
            $('#modal-signup-password-confirm').parent().addClass('has-error');
            $('#modal-signup-password-confirm-message').text('Password and confirm password do not match');
            return;
        }

        $.ajax({
            url: '/api/auth/register',
            type: "POST",
            data: JSON.stringify($('#form-signup').serializeFormJSON()),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert('yeet');
                    //window.location = '/dashboard';
                } else {
                    switch (data.error) {
                        case 'internalError':
                            $('#modal-signup-message').text('Internal error occurred, please try again later.');

                            break;
                        case 'usernameInUse':
                            $('#modal-signup-username').parent().addClass('has-error');
                            $('#modal-signup-username-message').text('Username already in use');

                            break;
                        case 'emailInUse':
                            $('#modal-signup-email').parent().addClass('has-error');
                            $('#modal-signup-email-message').text('Email address already in use');

                            break;
                        case 'invalidUsername':
                            $('#modal-signup-username').parent().addClass('has-error');
                            $('#modal-signup-username-message').text('Invalid username');

                            break;
                        case 'invalidPassword':
                            $('#modal-signup-password').parent().addClass('has-error');
                            $('#modal-signup-password-message').text('Invalid password');

                            break;
                        case 'invalidName':
                            $('#modal-signup-name').parent().addClass('has-error');
                            $('#modal-signup-name-message').text('Invalid name');

                            break;
                        case 'invalidEmail':
                            $('#modal-signup-email').parent().addClass('has-error');
                            $('#modal-signup-name-email').text('Invalid email');

                            break;
                        default:
                            $('#modal-signup-message').text('Unspecified error occurred');

                            break;
                    }
                }
            },
            error: function (data) {
                $('#modal-signup-message').text('Today is not your lucky day, pal');
                console.log(data);
            }
        });
    });

    $('#modal-login-submit').click(function () {
        $('#modal-login-message').text('');

        $('#modal-login-username').parent().removeClass('has-error');
        $('#modal-login-username-message').text('');

        $('#modal-login-password').parent().removeClass('has-error');
        $('#modal-login-password-message').text('');

        if ($('#modal-login-username').val() === '') {
            $('#modal-login-username').parent().addClass('has-error');
            $('#modal-login-username-message').text('Username cannot be empty');
            return;
        }

        if ($('#modal-login-password').val() === '') {
            $('#modal-login-password').parent().addClass('has-error');
            $('#modal-login-password-message').text('Password cannot be empty');
            return;
        }

        $.ajax({
            url: '/api/auth/login',
            type: "POST",
            data: JSON.stringify($('#form-login').serializeFormJSON()),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    window.localStorage.setItem('username', $('#modal-login-username').val());
                    window.localStorage.setItem('token', data.token);
                    window.location = '/dashboard';
                } else {
                    $('#modal-login-message').text('Username or password incorrect');
                }
            },
            error: function (data) {
                $('#modal-login-message').text('Today is not your lucky day, pal');
                console.log(data);
            }
        });
    });


    $('#modal-login').on('show.bs.modal', function (event) {
        $('#modal-login-message').text('');

        $('#modal-login-username').val('');
        $('#modal-login-username').parent().removeClass('has-error');
        $('#modal-login-username-message').text('');

        $('#modal-login-password').val('');
        $('#modal-login-password').parent().removeClass('has-error');
        $('#modal-login-password-message').text('');
    });

    $('#modal-signup').on('show.bs.modal', function (event) {
        $('#modal-signup-message').text('');

        $('#modal-signup-username').val('');
        $('#modal-signup-username').parent().removeClass('has-error');
        $('#modal-signup-username-message').text('');

        $('#modal-signup-name').val('');
        $('#modal-signup-name').parent().removeClass('has-error');
        $('#modal-signup-name-message').text('');

        $('#modal-signup-password').val('');
        $('#modal-signup-password').parent().removeClass('has-error');
        $('#modal-signup-password-message').text('');

        $('#modal-signup-password-confirm').val('');
        $('#modal-signup-password-confirm').parent().removeClass('has-error');
        $('#modal-signup-password-confirm-message').text('');

        $('#modal-signup-email').val('');
        $('#modal-signup-email').parent().removeClass('has-error');
        $('#modal-signup-email-message').text('');
    });

    $('#btn-signout').click(function (e) {
        window.localStorage.clear();
        window.location = '/';

        e.stopPropagation();
        e.preventDefault();
    });
});

function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
}

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

(function ($) {
    $.fn.serializeFormJSON = function () {

        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
})(jQuery);