function getProtocolo(dados, controlId) {
    
    var actionurl = "http://j4call.e-vida.org.br:8085/j4call/rest/services/issue/save";
    console.log(actionurl);
    console.log(dados);
    $.ajax({
        url: actionurl,
        type: 'post',
        dataType: 'application/json',
        data: dados,
        complete: function (data) {

            var protocolo = JSON.parse(data.responseText).protocol;

            if (protocolo !== '') {
                var ctrl = document.getElementById(controlId);
                ctrl.value = protocolo;
            }

            //var inputHidden = document.getElementById('inputHidden');
            //$("#" + inputHidden.value).val(protocolo);

     
            console.log(protocolo);

            alert(protocolo);

            //hdnProtocolo.value = protocolo;
            //hdnExibir.value - protocolo;
        },
        error: function (request, status, error) {
            var val = request.responseText;
            console.log(val);
        }
    });

}



