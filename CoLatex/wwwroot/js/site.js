$(document).ready(function () {
    $('#modal-signup-submit').click(function () {
        $('#modal-signup-username').parent().removeClass('has-error');
        $('#modal-signup-username-message').text('');

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
    });
});

function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
}