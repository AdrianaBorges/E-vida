<%@ Page Title="e-Vida Beneficiários" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="eVidaBeneficiarios.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
    	function ConfigControlsCustom() {
            //$('#<%= txtLogin.ClientID %>').ForceNumericOnly();
        }

    	function esqueciSenha() {
    		//window.open('<%= eVidaGeneralLib.Util.ParametroUtil.UrlGalenus %>HcTissFlex/ht119nl.isa', 'novaSenha', 'scrollbars=0,status=1,resizable=1,menubar=0,toolbar=0,width=550,height=250');
    		window.location = '<%= eVidaGeneralLib.Util.ParametroUtil.UrlGalenus %>ajuda/senha.html';
        	return false;
        }

	</script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<div class="lista">
		<div style="width: 350px">
		<fieldset>
			<legend>Autenticação</legend>
			<table width="100%">
                <tr>
                    <td colspan="2"><asp:Label ID="lblLoginExpirado" runat="server" Text="Sua sessão expirou!" /></td>
                </tr>
				<tr>
					<td>Usuário:</td>
					<td><asp:TextBox ID="txtLogin" runat="server" TextMode="SingleLine" Width="200px" ToolTip="Digite sua matrícula" /></td>
				</tr>
				<tr>
					<td>Senha:</td>
					<td><asp:TextBox ID="txtSenha" runat="server" TextMode="Password" Width="200px" ToolTip="Digite sua senha de acesso ao reembolso." /></td>
				</tr>
				<tr>
					<td colspan="2" style="text-align:center;">
                        <asp:Button ID="btnLogin" runat="server" Text="Entrar" onclick="btnLogin_Click" CssClass="button" />
                        <%--
                        <asp:Button ID="btnEsqueciSenha" runat="server" Text="Esqueci minha senha" CssClass="button" OnClick="btnEsqueciSenha_Click"  UseSubmitBehavior="False" Visible="true" OnClientClick="return esqueciSenha()" />
                        --%>                   
					</td>
				</tr>

			</table>
		</fieldset>
		</div>
	</div>
</asp:Content>