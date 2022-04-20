var tipoPopUp = -1;
var $locatorConfig;

// id = identificador
// rowNum = row index
// btn = btn submit
function LocatorHandler(tipo, id, rowNum, btn) {
	this.tipo = tipo;
	this.id = id;
	this.rowId = rowNum;
	this.btn = btn;
}

function LocatorConfig($width, $height, $open, $close, $callback) {
	this.width = $width;
	this.height = $height;
	this.open = $open;
	this.close = $close;
	this.handler = null;
	this.callback = $callback;
	this.exists = false;
	this.url = "../img/progress.gif";
}

LocatorConfig.prototype.createInternal = function () {
	var $this = this;
	$('#popLocator').dialog({
		autoOpen: false,
		width: $locatorConfig.width,
		height: $locatorConfig.height,
		draggable: false,
		modal: true,
		resizable: false,
		open: function (ev, ui) {
			$("#dFrmLocator").append("<iframe id=\"frmLocator\" src=\"../img/progress.gif\" width=\"1600\" height=\"1480px\" frameborder=\"0\"></iframe>");

			$this.open($this.handler, ev, ui);
			$('#frmLocator').attr('src', $this.url);
			$("#frmLocator").width($this.width - 50);
			$("#frmLocator").height($this.height - 65);

		},
		close: function (ev, ui) {
			if ($this.close)
				$this.close($this.handler, ev, ui);
			$("#dFrmLocator").empty();
			$this.url = "../img/progress.gif";
			$this.handler = null;
		},

		show: { effect: 'fade', duration: 500 },
		hide: { effect: 'fade', duration: 500 }
	});
	$this.exists = true;
}


function createLocator($width, $height, $open, $close, $callback) {
	if (!$locatorConfig)
		$locatorConfig = new LocatorConfig($width, $height, $open, $close, $callback);
	$locatorConfig.createInternal();
}


function setLocatorUrl(url) {
	$locatorConfig.url = url;
}

function openLocator(titulo, handler) {
	if (!$locatorConfig.exists)
		$locatorConfig.createInternal();
	$locatorConfig.handler = handler;
	$('#popLocator').dialog("option", "title", titulo);
	$("#popLocator").dialog('open');
}

function closeLocator() {
	$('#popLocator').dialog('close');
}

function locatorCallback(response) {
	try {
		if ($locatorConfig.handler) {
			$handler = $locatorConfig.handler;
			if ($locatorConfig.callback) {
				$locatorConfig.callback($handler, response);
			}
		}
	} catch (e) {
		alert('Erro interno em locatorCallback: ' + e.message);
	}
}

function defaultDlgCallback(handler, response) {
	executeLocatorHandlerPost(handler, response);
}

// id = identificador
// row = row index
// obj = btn submit
function defaultOpenPop(tipo, obj, id, row, titulo) {
	var handler = new LocatorHandler(tipo, id, row, obj);
	openLocator(titulo, handler);
	return false;
}

function executeLocatorHandlerPost(handler, response) {
    if (handler.id)
        $("#" + handler.id).val(response);
	closeLocator();
	if (handler.btn.href)
		eval(handler.btn.href);
	else if ($(handler.btn).attr('href'))
		eval($(handler.btn).attr('href'));
	else
		__doPostBack(handler.btn.name, '');
}
