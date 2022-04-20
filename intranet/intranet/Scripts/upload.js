
function clearUpload() {
    for (var i = 1; i <= 4; ++i) {
        $('#fileUpload' + i).parent().parent().empty();
    }
}

function setUploadLabel(lblId, hidId) {
    var val = $('#' + hidId).val();
    if (val != '') {
        val += " - Enviado";
    }
    $('#' + lblId).text(val);
}


function configureInputFile(url, id, divId, hidId, ohidId) {
    if (typeof uploadButton == 'undefined') {

        uploadButton = $('<button/>')
        .addClass('botao')
        .prop('disabled', true)
        .text('Processing...')
        .on('click', function () {
            var $this = $(this),
                data = $this.data();
            $this
                .off('click')
                .text('Abort')
                .on('click', function () {
                    $this.remove();
                    data.abort();
                });
            $('#ctl00_updProgress').css('display', 'block');
            data.submit().always(function () {
                $this.remove();
                $('#ctl00_updProgress').css('display', 'none');
            });
            return false;
        });

    }


    $('#' + id).fileupload({
        url: url,
        dataType: 'json',
        autoUpload: false,
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png|docx|doc|pdf|txt)$/i,
        maxFileSize: 4000000, // 4 MB
        divId: '#' + divId,
        campoHidFile: '#' + hidId,
        campoOHidFile: ohidId
    }).on('fileuploadadd', function (e, data) {
        addUpload(e, data, uploadButton, this);
    }).on('fileuploadprocessalways', function (e, data) {
        processUpload(e, data, this);
    }).on('fileuploadprogressall', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('#progress .progress-bar').css(
            'width',
            progress + '%'
        );
        if (progress > 0 && progress < 100 ) {
            $('#progress').css('height', '20px');
        } else {
            $('#progress').css('height', '0px');
        }
    }).on('fileuploaddone', function (e, data) {
        doneUpload(e, data, this);
    }).on('fileuploadfail', function (e, data) {
        failUpload(e, data, this);
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
}

function failUpload(e, data, obj) {
    alert('Erro ao enviar o arquivo. - ' + data.errorThrown);
    $.each(data.files, function (index, file) {
    	var error = $('<span class="text-danger"/>').text('Erro ao enviar arquivo. ' + data.errorThrown);
        $(data.context.children()[index])
            .append('<br>')
            .append(error);
    });
    if ($(obj).fileupload('option').autoUpload) $('#ctl00_updProgress').css('display', 'none');
}

function addUpload(e, data, uploadButton, obj) {
    var label = $(obj).fileupload('option').divId;
    $(label).empty();
    data.context = $('<div/>').appendTo(label);
    $.each(data.files, function (index, file) {
        var node = $('<span/>').text(file.name);
        node.css("margin-right", "5px");
        if (!index) {
            if (!$(obj).fileupload('option').autoUpload) {
                node = $('<span/>').append(node);
                node.append($('<span/>').append(uploadButton.clone(true).data(data)));
            }
        }
        node.appendTo(data.context);
    }
    );

    if ($(obj).fileupload('option').autoUpload)  $('#ctl00_updProgress').css('display', 'block');
}

function processUpload(e, data, obj) {
    var index = data.index,
            file = data.files[index],
            node = $(data.context.children()[index]);
    if (file.preview) {
        node.prepend('<br>').prepend(file.preview);
    }
    if (file.error) {
        node.append('<br>')
            .append($('<span class="text-danger"/>').text(file.error));

        if ($(obj).fileupload('option').autoUpload) $('#ctl00_updProgress').css('display', 'none');
    }
    if (index + 1 === data.files.length) {
        
        if (!$(obj).fileupload('option').autoUpload) {
            data.context.find('button').text('Enviar').prop('disabled', !!data.files.error);
        }
    }
}

function doneUpload(e, data, obj) {

    var hid = $(obj).fileupload('option').campoHidFile;
    var label = $(obj).fileupload('option').divId;
    var ohid = $(obj).fileupload('option').campoOHidFile;

    $(label).empty();
    $.each(data.result.files, function (index, file) {
        if (file.url) {
            $(label).text(file.url + " - Enviado");
            $(hid).val(file.url);
            if (ohid) {
                $('#' + ohid).val(file.originalName);
            }
            if (typeof (onAfterUpload) === 'function')
            	onAfterUpload(file.url, file.originalName);
        } else if (file.error) {
        	$(label).text(file.name + " - Erro: " + file.error);
            //var error = $('<span class="text-danger"/>').text(file.error);
            /*$(data.context.children()[index])
                .append('<br>')
                .append(error);*/
        }
    });
	
    if ($(obj).fileupload('option').autoUpload)  $('#ctl00_updProgress').css('display', 'none');
}