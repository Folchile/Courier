function borrar_contenido() {

    $('.field-validation-error')
                .removeClass('field-validation-error')
                .addClass('field-validation-valid');

    $('.input-validation-error')
                .removeClass('input-validation-error')
                .addClass('valid');
}