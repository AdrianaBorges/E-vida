<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopAprovReciprocidade.aspx.cs" Inherits="eVidaIntranet.Forms.PopAprovReciprocidade" %>
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

        var validadorArquivo = {
            test: function (nomeArquivo) {
                alert(nomeArquivo);
                return true;
            }
        }

        function ConfigControlsCustom() {
            var url = "./Upload.evida?prefix=<%= this.hidRnd.Value %>";

            configureInputFile(url, 'fileUpload', '<%= this.file.ClientID %>', '<%= this.fileName.ClientID %>');

            $('#fileUpload').fileupload('option', 'messages.acceptFileTypes', "Tipo de arquivo inválido!");
            $('#fileUpload').fileupload('option', 'acceptFileTypes', /(\.|\/)(zip|pdf|doc|docx|gif|jpe?g|png)$/i);
            $('#fileUpload').fileupload('option', 'autoUpload', true);
        }


        function setAprovacao(id) {
        	//parent.setAprovacao(id);
        	parent.locatorCallback(id);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <asp:HiddenField ID="hidRnd" runat="server" />
    <table width="500px">
        <tr>
            <td>Informe alguma observação para ser enviada no e-mail do beneficiário<br /></td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtObservacao" runat="server" Width="450px" TextMode="SingleLine" /></td>
        </tr>
        <tr>
            <td>Anexo:</td>
        </tr>
        <tr>
            <td>
                <div style="width:200px">
                    <!-- The fileinput-button span is used to style the file input field as button -->
                    <span class="botao fileinput-button" style="display: inline-block">
                        <span>Adicionar/Alterar arquivo</span>
                        <input id="fileUpload" type="file" name="files[]" />
                    </span>
                </div>
                <!-- The container for the uploaded files -->
                <asp:Label ID="file" runat="server" CssClass="files" />
                <input id="fileName" runat="server" type="hidden" />
                <input type="hidden" id="ofileName" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Aprovar formulário" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>
