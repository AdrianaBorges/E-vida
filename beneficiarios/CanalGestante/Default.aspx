<%@ Page Title="Canal Gestante" Language="C#" MasterPageFile="~/Internal/ExternalPages.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="eVidaBeneficiarios.CanalGestante.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div style="height:300px">&nbsp;</div>
    <div class="lista">
		<div style="width: 450px">
		<fieldset>
			<legend>Para acessar o Canal Gestante, informe os seguintes dados:</legend>
			<table width="100%">
                <tr>
                    <td colspan="2"><asp:Label ID="lblLoginExpirado" runat="server" Text="Sua sessão expirou!" Visible="false" /></td>
                </tr>
				<tr>
					<td>Cartão:</td>
					<td><asp:TextBox ID="txtCartao" runat="server" TextMode="SingleLine" Width="200px" ToolTip="Informe o número de sua carteirinha, com os hífens. Ex: 60-89891-9090" /></td>
				</tr>
				<tr>
					<td>Data de Nascimento:</td>
					<td><asp:TextBox ID="txtNascimento" runat="server" Width="200px" CssClass="calendario"/></td>
				</tr>
                <tr>
					<td>Plano:</td>
					<td><asp:DropDownList ID="dpdPlano" runat="server" Width="200px" DataTextField="DisplayField" DataValueField="Codigo" /></td>
				</tr>
				<tr>
					<td colspan="2" align="center"><asp:Button ID="btnLogin" runat="server" Text="Entrar" 
							onclick="btnLogin_Click" /></td>
				</tr>

			</table>
		</fieldset>
		</div>
	</div>
</asp:Content>
