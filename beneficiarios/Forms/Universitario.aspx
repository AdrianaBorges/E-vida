<%@ Page Title="Declaração de Universitário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Universitario.aspx.cs" Inherits="eVidaBeneficiarios.Forms.Universitario" %>
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
    		$("#<%= tbSelecao.ClientID %> ").css({ "width": "100%" });
    		$("#<%= tbSelecao.ClientID %> td").css({ "padding": "2px" });
    		$("#<%= tbArquivo.ClientID %> ").css({ "width": "100%" });

    		var url = "./Upload.evida?prefix=<%= this.hidRnd.Value %>";

    		configureInputFile(url, 'fileUpload', '<%= this.file.ClientID %>', '<%= this.fileName.ClientID %>');

    		$('#fileUpload').fileupload('option', 'messages.acceptFileTypes', "Tipo de arquivo inválido!");
    		$('#fileUpload').fileupload('option', 'acceptFileTypes', /(\.|\/)(zip|pdf|doc|docx|gif|jpe?g|png|rar)$/i);
    		$('#fileUpload').fileupload('option', 'autoUpload', true);
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <asp:HiddenField ID="hidRnd" runat="server" />
    <div id="conteudo">
        <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
			    <td rowspan="2" valign="top">
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:Label ID="txtCartao" runat="server" Width="150px"/>
                </td>
                <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
                
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtEmailTitular" runat="server" Width="400px" MaxLength="150" />
                </td>
            </tr>
        </table>
        <br />
        <h2>2 - Dados do Dependente</h2>
        <table id="tbSelecao" runat="server" border="1" class="tabelaForm">
            <tr>
                <td colspan="3"><asp:DropDownList ID="dpdDependente" runat="server" DataValueField="Cdusuario" DataTextField="Nomusr" OnSelectedIndexChanged="dpdDependente_SelectedIndexChanged" AutoPostBack="true" /></td>
            </tr>
            <tr style="height:50px">
                <td><b>PARENTESCO</b><br />
                    <asp:Label ID="txtParentesco" runat="server" Width="200px"/>
                </td>
                <td><b>PLANO</b><br />
                    <asp:Label ID="txtPlano" runat="server" Width="200px" />
                </td>
                <td><b>IDADE</b><br />
                    <asp:Label ID="txtIdade" runat="server" Width="80px" />
                </td>
            </tr>            
	    </table>
		<h2>3 - Anexar Declaração de Universitário</h2>
        <table id="tbArquivo" runat="server" border="1" class="tabelaForm">
            <tr>
                <td style="width:250px">					
                    <div style="width:200px">
						<!-- The fileinput-button span is used to style the file input field as button -->
						<span class="botao fileinput-button" style="display: inline-block">
							<span>Adicionar/Alterar arquivo</span>
							<input id="fileUpload" type="file" name="files[]" />
						</span>
						(zip,pdf,doc,docx,gif,jpg,png,rar)
					</div>
					<!-- The container for the uploaded files -->
					<input id="fileName" runat="server" type="hidden" />
					<input type="hidden" id="ofileName" runat="server" />
                </td>
				<td>
					<asp:Label ID="file" runat="server" CssClass="files" />
				</td>
            </tr>
        </table>
        <div>
            <table width="50%">
                <tr>
                    <td align="center"><asp:Button ID="btnSalvar" runat="server" Text="Enviar" OnClick="btnSalvar_Click" /></td>
                    <td align="center"><asp:ImageButton ID="btnPdf" ImageUrl="../img/print.png" runat="server" ToolTip="Gerar PDF" Width="50px" CssClass="printer" Visible="false" /></td>
                    <td align="center"><asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" /></td>
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>
