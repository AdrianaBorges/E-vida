<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopUpload.aspx.cs" Inherits="eVidaBeneficiarios.GenPops.PopUpload" %>
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

		function ConfigControlsCustom() {
			<% if (this.hidRnd.Visible) { %>
			var url = "./Upload.evida?prefix=<%= this.hidRnd.Value %>";

			configureInputFile(url, 'fileUpload', '<%= this.file.ClientID %>', '<%= this.fileName.ClientID %>', '<%= this.ofileName.ClientID %>');

			var fileMaxSize = <%: GetUploadSize() * 1024 %>

			$('#fileUpload').fileupload('option', 'messages.acceptFileTypes', "Tipo de arquivo inválido!");
			$('#fileUpload').fileupload('option', 'messages.maxFileSize', "Arquivo muito grande!");
			$('#fileUpload').fileupload('option', 'acceptFileTypes', fileTypes);
			$('#fileUpload').fileupload('option', 'maxFileSize', fileMaxSize);
			$('#fileUpload').fileupload('option', 'autoUpload', true);
			<% } %>
		}

		function onAfterUpload(url, original) {
			parent.onAfterUpload(url, original);
		}



    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<asp:HiddenField ID="hidRnd" runat="server" />
    <table width="95%" cellspacing="10px" style="vertical-align:top;">
        <tr>
		    <td colspan="4"><h2 class="componentheading">Envio de Arquivo</h2></td>
	    </tr>
        <tr>
            <td>Arquivo:</td>
            <td><div style="width:200px" id="divUpload" runat="server">
                    <!-- The fileinput-button span is used to style the file input field as button -->
                    <span class="botao fileinput-button" style="display: inline-block">
                        <span>Selecionar arquivo</span>
                        <input id="fileUpload" type="file" name="files[]" />
                    </span>
                </div>
                <!-- The container for the uploaded files -->
                <asp:Label ID="file" runat="server" CssClass="files" />
                <input id="fileName" runat="server" type="hidden" />
                <input type="hidden" id="ofileName" runat="server" /></td>
        </tr>
		<tr>
			<td colspan="2">
				<b>Tipos suportados:</b> <%: GetUploadFilter() %><br />				
				<b>Tamanho máximo:</b> <%: GetUploadSize()/1024 %>MB
			</td>
		</tr>
		<tr>
			<td colspan="2" style="text-align:center">
				Caso tenha problemas para envio de arquivo, tente <asp:HyperLink ID="lnkClassic" runat="server" Text="Utilizar método clássico para envio de arquivo" /></td>
		</tr>
	</table>
</asp:Content>

