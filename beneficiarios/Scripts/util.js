/*var RELATORIO_EXCLUSAO = 'EXCLUSAO';
var RELATORIO_SEG_VIA = 'SEGUNDA_VIA';
var RELATORIO_MENSALIDADE_IR = 'MENSALIDADE_IR';
var RELATORIO_REEMBOLSO_IR = 'REEMBOLSO_IR';
*/
function openReport(tipo, params, inline) {
	var url = '../relatorio.evida?REPORT=' + tipo;
	url += '&' + params;
	var sizes = 'width=300px,height=300px';
	if (inline) {
		url += "&INLINE=true";
		sizes = '';
	}
	window.open(url, 'relatorio', sizes);
}

function openFile(tipo, params) {
	var url = '../download.evida?TIPO=' + tipo;
	url += '&' + params;
	window.open(url, 'file', 'width=600px,height=300px');
}