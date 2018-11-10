$(document).ready(function () {
    $('#modal-signup-submit').click(function () {
        $('#modal-signup-password-confirm-message').text('');
        if ($('#modal-signup-password-confirm').val() !== $('#modal-singup-password').val()) {
            $('#modal-signup-password-confirm').parent().addClass('has-error');
            $('#modal-signup-password-confirm-message').text('Password and confirm password do not match');
            return;
        }
    });
});