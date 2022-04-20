var msg = "";
var isErro = false;
var ctrFocus = "";
var urlHelp = "";

var formatPreco = {
    prefix: '',
    centsSeparator: ',',
    thousandsSeparator: '.',
    centsLimit: 2,
    clearPrefix: true,
    allowNegative: false
};

function endRequestHandler(sender, arg) {
	if (arg.get_error() !== null) {
		//alert user of problem and continue
		msg = "";
		ctrFocus = "";
		showError(arg.get_error().message);
		arg.set_errorHandled(true);
		setTimeout(function () { openDialog(); }, 1000);
	} else {
		openDialog();
	}
	onAfterRequestCompleted();
}
function beginRequestHandler(sender, args) {
	msg = "";
	isErro = false;
	ctrFocus = "";
}

$(document).ready(function () {
	$('#dialog').dialog({
		autoOpen: false,
		width: 550,
		height: 350,
		draggable: false,
		modal: true,
		resizable: false,
		buttons: {
			"Ok": function (ev, ui) {

				$(this).dialog("close");
				showFocus();
			}
		},
		close: function (ev, ui) {
			onClosePopUp(ev, ui, this);
		}
	});
	setTimeout(function () { openDialog(); }, 1000);
	Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
	Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) {
		ConfigControls();
	});
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
});

function onClosePopUp(ev, ui, dlg) {
	if (typeof (ClosePopUpCustom) === 'function')
		ClosePopUpCustom(ev, ui, dlg);
}

function openDialog() {
	if (msg != "") {
		$('#msgErro').html(msg);
		if (isErro)
			$('#dialog').dialog("option", "title", "Erro");
		else
			$('#dialog').dialog("option", "title", "Mensagem");
		$('#dialog').dialog('open');

		if (!isErro) {
			//setTimeout(function () { $('#dialog').dialog('close'); }, 1200);
		}

		msg = "";
		isErro = false;
	} else {
		showFocus();
	}
}

function showFocus() {
	if (ctrFocus != "") {
		$('#' + ctrFocus).focus();
		ctrFocus = "";
	}
}

function showError(msg) {
	showMessage(msg, true);
}

function showMessage(mmsg, isError) {
	msg += mmsg;
	isErro = (isError || isErro);
}

function clearStartupMessage() {
	//$(document).ready(function () {
	msg = "";
	ctrFocus = "";
	isErro = false;
	//});
}

function showStartupMessage(msg, isErro) {
	//$(document).ready(function () {
	showMessage(msg, isErro);
	//});
}

function showStartupError(msg) {
	showStartupMessage(msg, true);
}

jQuery.fn.ForceNumericOnly =
	function () {
		return this.each(function () {
		    $(this).keydown(function (e) {
		        var key = e.charCode || e.keyCode || 0;
		        if (e.ctrlKey && key == 86)
		            return true; // allow ctrl v
				// allow backspace, tab, delete, arrows, numbers and keypad numbers ONLY
				return (
					key == 8 ||
					key == 9 ||
					key == 46 ||
					(key >= 37 && key <= 40) ||
					(key >= 48 && key <= 57) ||
					(key >= 96 && key <= 105));
			});
		});
	};


function setMaskCelular(elId) {
	$('#' + elId).mask("(99) 9999-9999?9")
	.live('focusout', function (event) {
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
}
function setAllCalendars() {
	$('.calendario').each(function () {
		$('#' + this.id).datepicker();
		$('#' + this.id).mask("99/99/9999", { placeholder: " " });
	});
}

function ConfigControls() {

	$('.sexo label').each(function () {
		this.style.color = '#555';
	});
	setAllCalendars();
	$('.cpf').each(function () {
		$('#' + this.id).mask("999.999.999-99", { placeholder: " " });
	});

	if (typeof (ConfigControlsCustom) === 'function')
		ConfigControlsCustom();

}

function onAfterRequestCompleted() {
	if (typeof (RequestCompletedCustom) === 'function')
		RequestCompletedCustom();
}

function disableAll() {
	var els = document.getElementsByTagName("input");
	for (var i = 0; i < els.length; i++) {
		els[i].disabled = "true";
	}
	els = document.getElementsByTagName("select");
	for (var j = 0; j < els.length; j++) {
		els[j].disabled = "true";
	}
}
function disableEnterKey(e) {
    var key;
    if (window.event) key = window.event.keyCode; // Internet Explorer
    else key = e.which;

    return (key != 13);
}

function showHelp() {
    if (urlHelp == "") {
        alert('Ajuda não configurada!');
        return false;
    }
    window.open(urlHelp, 'help');
    return false;
}