$(document).ready(function () {
    mascarasCampos();

    $('.carterinha').on("input", function (e) {
        $(this).val($(this).val().replace(/[^0-9-.]+$/, ""));
    });

    $('.tipo-solicitante').on('change', function () {
        limpaGeral();
        $("#div-form").show();
        var carterinha = $("#div-carterinha");
        if (this.value === "1") {
            carterinha.show();
            $("[name='code']").attr("required", true);
        }
        if (this.value === "2") {
            carterinha.hide();
            $("[name='code']").attr("required", false);
            //$('.carterinha').removeAttr('required');​​​​​
        }
    });
});

function mascarasCampos() {
    $(".data").mask("99/99/9999");
    $(".cpf").mask('999.999.999-99');
    $(".telefone").mask("(99) 9999-9999");
    $(".rg").mask("9.999.999");
    $(".cep").mask("99.999-999");
    $(".hora").mask("99:99");
    $('.celular').mask("(99) 9999-9999?9").focusout(function (event) {
        var target, phone, element;
        target = (event.currentTarget) ? event.currentTarget : event.srcElement;
        phone = target.value.replace(/\D/g, '');
        element = $(target);
        element.unmask();
        if (phone.length > 10) {
            element.mask("(99) 99999-999?9");
        } else {
            element.mask("(99) 9999-9999?9");
        }
    });

    $(".cpfcnpj").mask("999.999.999-99?99999").focusout(function (event) {
        var target, pfpj, element;
        target = (event.currentTarget) ? event.currentTarget : event.srcElement;
        pfpj = target.value.replace(/\D/g, '');
        element = $(target);
        element.unmask();
        if (pfpj.length > 11) {
            element.mask("99.999.999/9999-99");
        } else {
            element.mask("999.999.999-99?99999");
        }
    });

    $(".btn-enviar").on("click", function () {

        var categoria = $('.categoria').val();
        var tipo = $('.tipo').val();
        if (categoria === null) {
            $('.categoria').focus();
            return false;
        }
        if (tipo === null) {
            $('.tipo').focus();
            return false;
        }
        return true;
    });

}

function alerta(titulo, msg) {
    BootstrapDialog.show({
        title: titulo,
        message: msg,
        type: BootstrapDialog.TYPE_SUCCESS
    });
}

function limpaGeral() {
    $('#idFormIssue').each(function () {
        this.reset();
    });
}

function DateNow() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    today = dd + '/' + mm + '/' + yyyy;
    return today;
}

function alertaJQuery(titulo, msg, isRefresh) {
    $('#dialogMsg').html(msg);
    $("#dialog").dialog({
        modal: true,
        title: titulo,
        close: function (event, ui) {
            if (isRefresh === true) {
                location.reload();
            }
        }
    });
}

function recaptchaCallback() {
    var response = grecaptcha.getResponse(),
        $button = $("#contacts-submit");
    if (response !== '') {
        $button.attr("disabled", false);
    }
    else {
        $button.attr("disabled", "disabled");
    }
}

function recaptchaExpired() {
    var response = grecaptcha.getResponse(),
        $button = $("#contacts-submit");
    if (response !== '') {
        $button.attr("disabled", false);
    }
    else {
        $button.attr("disabled", "disabled");
    }
}

function post() {
    var response = grecaptcha.getResponse();
    if (response !== '') {
        $("#idFormIssue").submit(function (e) {
            e.preventDefault();
            var actionurl = "http://j4call.e-vida.org.br:8085/j4call/rest/services/issue/save";
            console.log(actionurl);
            $.ajax({
                url: actionurl,
                type: 'post',
                dataType: 'application/json',
                data: $("#idFormIssue").serialize(),
                complete: function (data) {
                    var titulo = 'Solicita\u00e7\u00e3o criada com sucesso! \n\n';
                    var msg01 = '<b>Protocolo:</b> '
                    var msg02 = JSON.parse(data.responseText).protocol;
                    var msg03 = '<br><br><b>Data de abertura:</b> ';
                    var msg04 = DateNow();
                    var msg05 = '<br><br>Prazo estimado 5 dias!!! A E-VIDA agradece o seu contato.'
                    var msg = msg01 + msg02 + msg03 + msg04 + msg05;
                    limpaGeral();
                    alertaJQuery(titulo, msg, true);
                    return true;
                },
                error: function (request, status, error) {
                    var val = request.responseText;
                    limpaGeral();
                }
            });
        });
    }
    else {
        alertaJQuery("Captcha!", "O captcha não foi checado, favor verificar!", false);
        return false;
    }
}


