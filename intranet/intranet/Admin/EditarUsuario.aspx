<%@ Page Title="Cadastrar/Alterar Usuário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="EditarUsuario.aspx.cs" Inherits="eVidaIntranet.Admin.EditarUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

	    var POP_ASSINATURA = 1;
	    var POP_PERMISSAO = 2;
		function ConfigControlsCustom() {
			createLocator(650, 550, dlgOpen, null, dlgCallback);
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
			    case POP_ASSINATURA: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.ASSINATURA.Value%>'; break;
			    case POP_PERMISSAO: src = './PopResumoPermUsuario.aspx?LOGIN=<%= eVidaGeneralLib.Util.FormatUtil.ToBase64String(Convert.ToString(Request["id"])) %>'; break;
			}
			setLocatorUrl(src);
		}

		function dlgCallback(handler, response) {
			switch (handler.tipo) {
			    case POP_ASSINATURA: break;
			    case POP_PERMISSAO: break;
			}
		}

		function onAfterUpload(url, originalName) {
			$("#<%:hidArqFisico.ClientID%>").val(url);
    		$("#<%:hidArqOrigem.ClientID%>").val(originalName);
    		$('#popLocator').dialog('close');
			<%= ClientScript.GetPostBackEventReference(btnAssinatura, "") %>
    	}

		function openAssinatura() {
			var handler = new LocatorHandler(POP_ASSINATURA);
			openLocator("Assinatura", handler);
			return false;
		}

		function openPermissoes() {
		    var handler = new LocatorHandler(POP_PERMISSAO);
		    openLocator("Permissões", handler);
		    return false;
		}

		function openViewAss() {
			var fName = $("#<%: hidArqOrigem.ClientID %>").val();
			openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.ASSINATURA %>', "ID=<%= eVidaGeneralLib.Util.FormatUtil.ToBase64String(Convert.ToString(Request["id"])) %>");
			return false;
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
	    <tr>
            <td style="width:200px; text-align: right">ID:</td>
            <td><asp:TextBox ID="lblId" runat="server" Width="75px" Enabled="false" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Login:</td>
		    <td align="left"><asp:TextBox ID="txtLogin" runat="server" Width="220px" Enabled="false" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Nome:</td>
		    <td align="left"><asp:TextBox ID="txtNome" runat="server" Width="320px" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">E-mail:</td>
		    <td align="left"><asp:TextBox ID="txtEmail" runat="server" Width="320px" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Cargo:</td>
		    <td align="left"><asp:TextBox ID="txtCargo" runat="server" Width="320px" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Matrícula (E-VIDA):</td>
		    <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="320px" OnTextChanged="txtMatricula_TextChanged" AutoPostBack="true" />
                <asp:Label ID="lblEmpregado" runat="server" />
		    </td>
	    </tr>
        <tr>
		    <td style="text-align: right">Regional:</td>
		    <td align="left"><asp:DropDownList ID="dpdRegional" runat="server" Width="320px" DataValueField="Key" DataTextField="Value" /></td>
	    </tr>
		<tr id="trAss" runat="server" visible="false">
			<td style="text-align: right">Assinatura eletrônica:</td>
			<td>
				<asp:HiddenField ID="hidArqFisico" runat="server" />
				<asp:HiddenField ID="hidArqOrigem" runat="server" />
				<a href="javascript:void(0)" onclick="return openViewAss();">Visualizar</a>
				<asp:Button ID="btnAssinatura" runat="server" Text="Incluir/alterar" OnClientClick="return openAssinatura()" OnClick="btnAssinatura_Click" />
			</td>
		</tr>
        <tr>
			<td style="text-align: right"><b>Perfis:</b></td>
			<td><asp:CheckBoxList ID="chkPerfil" runat="server" DataValueField="Key" DataTextField="Value" 
				RepeatColumns="3" RepeatDirection="Horizontal" Width="700px" /></td>
		</tr>
        <tr>
            <td><br /></td>
	    </tr>
        <tr>
		<td colspan="2" style="text-align:center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
            <asp:Button ID="btnRemover" runat="server" Text="Remover Usuário da Intranet" OnClick="btnRemover_Click" />
            <asp:Button ID="btnPermissao" runat="server" Text="Visualizar Permissões" OnClientClick="return openPermissoes()" />
		</td>
	</tr>
    </table>
</asp:Content>
