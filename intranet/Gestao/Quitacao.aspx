<%@ Page Title="QUITAÇÃO SAP X ISA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Quitacao.aspx.cs" Inherits="eVidaIntranet.Gestao.Quitacao" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    
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

            $('#fileUpload').fileupload('option', 'messages.acceptFileTypes', "Tipo de arquivo inválido!");
            $('#fileUpload').fileupload('option', 'acceptFileTypes', /(\.|\/)(csv)$/i);
            $('#fileUpload').fileupload('option', 'autoUpload', true);
            <% } %>
        }

    	function goBack() {
    		window.location = './BuscaQuitacao.aspx';
    		return false;
    	}


    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="placeHolder" runat="server">
    
    <asp:HiddenField ID="hidRnd" runat="server" />
    <table width="650px" cellspacing="10px" style="vertical-align:top;">
        <tr>
		    <td colspan="4"><h2 class="componentheading">Arquivo SAP</h2></td>
	    </tr>
        <tr>
            <td>Arquivo (.csv):</td>
            <td><div style="width:200px" id="divUpload" runat="server">
                    <!-- The fileinput-button span is used to style the file input field as button -->
                    <span class="botao fileinput-button" style="display: inline-block">
                        <span>Adicionar/Alterar arquivo</span>
                        <input id="fileUpload" type="file" name="files[]" />
                    </span>
                </div>
                <!-- The container for the uploaded files -->
                <asp:Label ID="file" runat="server" CssClass="files" />
                <input id="fileName" runat="server" type="hidden" />
                <input type="hidden" id="ofileName" runat="server" /></td>
        </tr>
		<tr>
			<td>Tipo Arquivo:</td>
			<td colspan="3"><asp:DropDownList ID="dpdTipoArquivo" runat="server" Width="200px" /></td>
		</tr>
	    <tr>
            <td >Mês Folha:</td>
            <td align="left" colspan="3"><asp:DropDownList ID="dpdMes" runat="server" Width="120px" /> <asp:DropDownList ID="dpdAno" runat="server" Width="90px" /></td>
        </tr>
        <tr>
            <td >Data Recebimento:</td>
            <td align="left" colspan="3"><asp:TextBox ID="txtRecebimento" runat="server" Width="120px" MaxLength="10" CssClass="calendario" /></td>
        </tr>
        <tr>
            <td colspan="4" align="center">
				<asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClientClick="return goBack()" />
				<asp:Button ID="btnImportar" runat="server" Text="Importar" OnClick="btnImportar_Click" />
                <asp:Button ID="btnQuitar" runat="server" Text="Quitar" OnClick="btnQuitar_Click" Visible="false" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar Arquivo" OnClick="btnCancelar_Click" Visible="false" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar Inconsistências" OnClick="btnExportar_Click" Visible="false" />
            </td>
        </tr>
        <tr>
		    <td colspan="4"><h2 class="componentheading">INCONSISTÊNCIAS</h2></td>
	    </tr>
        <tr style="height:300px">
		    <td colspan="4">
                <asp:Label ID="lblCount" runat="server" />
			    <asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				    AllowSorting="false" CssClass="tabela" Width="800px"
                    AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" 
                    onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                    <Columns>
                        <asp:BoundField HeaderText="EMPRESA" DataField="cd_empresa" DataFormatString="{0:0}" />
					    <asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" DataFormatString="{0:0}" />
                        <asp:BoundField HeaderText="VALOR SAP" DataField="vl_sap" DataFormatString="{0:C}" />
                        <asp:BoundField HeaderText="VALOR ISA" DataField="vl_isa" DataFormatString="{0:C}" />
                        <asp:BoundField HeaderText="DIFERENÇA (SAP-ISA)" DataField="vl_diff" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="VERBA" DataField="cd_verba" DataFormatString="{0:0}" />
						<asp:BoundField HeaderText="VALOR VERBA" DataField="vl_quitacao" DataFormatString="{0:C}" />
				    </Columns>
			    </asp:GridView>
		    </td>
	    </tr>
    </table>
</asp:Content>
