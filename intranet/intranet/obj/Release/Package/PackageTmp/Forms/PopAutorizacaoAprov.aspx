<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopAutorizacaoAprov.aspx.cs" Inherits="eVidaIntranet.Forms.PopAutorizacaoAprov" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <link rel="stylesheet" href="../css/eVida/jquery.fileupload.css" />
    <link rel="stylesheet" href="../css/eVida/jquery.fileupload-ui.css" />
        
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script type="text/javascript" src="../Scripts/upload/jquery.iframe-transport.js"></script>
    <!-- The basic File Upload plugin -->
    <script type="text/javascript" src="../Scripts/upload/jquery.fileupload.js"></script>
    <!-- The File Upload processing plugin -->
    <script type="text/javascript" src="../Scripts/upload/jquery.fileupload-process.js"></script>
    <!-- The File Upload validation plugin -->
    <script type="text/javascript" src="../Scripts/upload/jquery.fileupload-validate.js"></script>
    
    <script type="text/javascript" src="../Scripts/upload.js"></script>
    <script type="text/javascript">

    	var urlUpl = "./Upload.evida?prefix=<%= this.hidRnd.Value %>";

    	var validadorArquivo = {
    		test: function (nomeArquivo) {
    			alert(nomeArquivo);
    			return true;
    		}
    	}

    	function ConfigControlsCustom() {
        	var i = 0;
        	for (i = 0; i < upls.length; ++i) {
        		configInternalUpl(upls[i]);
        	}
        }

    	function configInternalUpl(upl) {
    		inputFile = upl.inputFile;
    		lblFile = upl.lblFile;
    		lblFileName = upl.lblFileName;
    		$urlUpl = "./Upload.evida?prefix=" + upl.prefix;
    		configureInputFile($urlUpl, inputFile, lblFile, lblFileName);

    		$('#' + inputFile).fileupload('option', 'messages.acceptFileTypes', "Tipo de arquivo inválido!");
    		$('#' + inputFile).fileupload('option', 'acceptFileTypes', /(\.|\/)(zip|pdf|doc|docx|gif|jpe?g|png)$/i);
    		$('#' + inputFile).fileupload('option', 'autoUpload', true);
    	}

    	function setSolicitacao(id) {
    		parent.locatorCallback(id);
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<asp:HiddenField ID="hidRnd" runat="server" />
	<p style="text-align:center">Informe a autorização TISS e anexo o PDF gerado no ISA</p>
	<asp:ListView ID="ltvAutorizacao" runat="server" ItemPlaceholderID="content">
		<LayoutTemplate>
			<table width="500px">
				<tr>
					<th></th>
					<th>Nº Autorização TISS</th>
					<th>Anexo</th>
				</tr>
				<tr id="content" runat="server" />
				<tr>
					<td colspan="3" style="text-align:center">
						<br /><br />
						<asp:Button ID="btnIncluir" runat="server" Text="Incluir outro arquivo" OnClick="btnIncluir_Click" />
					</td>
				</tr>
				<tr>
					<td colspan="3">
						<br />
						<b>OBSERVAÇÕES NO ATO DA APROVAÇÃO:</b>
						<asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine" Rows="4" Width="100%" />
					</td>
				</tr>
				<tr>
					<td colspan="3" style="text-align:center">
						<br /><br />
						<asp:Button ID="btnSalvar" runat="server" Text="Aprovar solicitação" OnClick="btnSalvar_Click" />
					</td>
				</tr>
			</table>
		</LayoutTemplate>
		<ItemTemplate>
			<tr>
				<td><asp:ImageButton ID="btnRemover" runat="server" ImageUrl="~/img/remove.png" Visible='<%# Container.DataItemIndex != 0 %>'
					CommandArgument='<%# Container.DataItemIndex%>' OnClick="btnRemover_Click" /></td>
				<td style="text-align:center"><asp:TextBox ID="txtNroAutorizacao" runat="server" Width="150px" /></td>
				<td>
					<div style="width:200px">
						<!-- The fileinput-button span is used to style the file input field as button -->
						<span class="botao fileinput-button" style="display: inline-block">
							<span>Adicionar/Alterar arquivo</span>
							<input id="fileUpload_<%# Container.DataItemIndex %>" type="file" name="files[]" />
						</span>
					</div>
					<!-- The container for the uploaded files -->
					<asp:Label ID="file" runat="server" CssClass="files" />
					<input id="fileName" runat="server" type="hidden" />
					<input id="ofileName" runat="server" type="hidden"  />
				</td>
			</tr>
		</ItemTemplate>
	</asp:ListView>
</asp:Content>
