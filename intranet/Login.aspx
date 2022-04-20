<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="eVidaIntranet.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
		$(document).ready(function () {
			
		});
		
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
					<td>Login:</td>
					<td><asp:TextBox ID="txtLogin" runat="server" TextMode="SingleLine" Width="200px" /></td>
				</tr>
				<tr>
					<td>Senha:</td>
					<td><asp:TextBox ID="txtSenha" runat="server" TextMode="Password" Width="200px" /></td>
				</tr>
				<tr>
					<td colspan="2"><asp:Button ID="btnLogin" runat="server" Text="Entrar" 
							onclick="btnLogin_Click" CssClass="button" />
							<asp:Button ID="btnEsqueciSenha" runat="server" Text="Esqueci minha senha" 
							CssClass="button" OnClick="btnEsqueciSenha_Click"  UseSubmitBehavior="False" Visible="false" /></td>
				</tr>

			</table>
		</fieldset>
		</div>
	</div>
</asp:Content>