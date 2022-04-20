<%@ Page Title="Novo Usuário SCL" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="CriarUsuarioScl.aspx.cs" Inherits="eVidaIntranet.Admin.CriarUsuarioScl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var currLogin = '';
        function goToEdit() {
            if (currLogin == '') return false;
            window.location = './AlterarSenhaUsuarioScl.aspx?login=' + currLogin;
            return false;
        }

        function ClosePopUpCustom(ev, ui, dlg) {
            return goToEdit();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
        <tr>
		    <td style="text-align: right; width: 200px">Login:</td>
		    <td align="left"><asp:TextBox ID="txtLogin" runat="server" Width="220px" MaxLength="20"/></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Nome:</td>
		    <td align="left"><asp:TextBox ID="txtNome" runat="server" Width="320px" MaxLength="80" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Perfil:</td>
		    <td align="left"><asp:DropDownList ID="dpdPerfil" runat="server" Width="320px" OnSelectedIndexChanged="dpdPerfil_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Text="SELECIONE" Value="" />
                <asp:ListItem Text="BENEFICIÁRIO" Value="1" />
                <asp:ListItem Text="CREDENCIADOS" Value="2" />
                <asp:ListItem Text="CREDENCIADOS ODONTOLÓGICOS" Value="4" />
		     </asp:DropDownList></td>
	    </tr>
        <tr>
		    <td style="text-align: right"><asp:Label ID="lblDominio" runat="server" Text="DOMÍNIO" />:</td>
		    <td align="left"><asp:TextBox ID="txtDominio" runat="server" Width="320px" MaxLength="100" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Senha:</td>
		    <td align="left"><asp:TextBox ID="txtSenha" runat="server" Width="220px" TextMode="Password" /></td>
        </tr>
        <tr>
		    <td style="text-align: right">Confirmação de Senha:</td>
		    <td align="left"><asp:TextBox ID="txtConfSenha" runat="server" Width="220px" TextMode="Password" />
                <asp:CompareValidator ID="cmpValConfSenha" runat="server" ErrorMessage="As senhas estão diferentes!" CssClass="ui-state-error"
                    Type="String" ControlToCompare="txtSenha" ControlToValidate="txtConfSenha" Operator="Equal"></asp:CompareValidator>
		    </td>
	    </tr>
        <tr>
            <td><br /></td>
	    </tr>
        <tr>
		<td colspan="2" style="text-align:center"><asp:Button ID="btnSalvar" runat="server" Text="Criar Usuário" OnClick="btnSalvar_Click" />
		</td>
	</tr>
    </table>
</asp:Content>
