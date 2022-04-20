/*var RELATORIO_EXCLUSAO = 'EXCLUSAO';
var RELATORIO_SEG_VIA = 'SEGUNDA_VIA';
var RELATORIO_RECIPROCIDADE = 'ENVIO_RECIPROCIDADE';
var RELATORIO_SOL_RECIPROCIDADE = 'SOL_RECIPROCIDADE';
var RELATORIO_NEGATIVA = 'NEGATIVA';
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

function openFile(tipo, params, inline) {
	var url = '../download.evida?TIPO=' + tipo;
	url += '&' + params;
	var sizes = 'width=600px,height=300px';
	if (inline) {
	    url += "&INLINE=true";
	    sizes = '';
	}
	window.open(url, 'file', sizes);
}
