<%@ Page Title="E-mail" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopEmail.aspx.cs" Inherits="eVidaIntranet.Reuniao.PopEmail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<h2>Enviar e-mail para os membros do órgão <asp:Literal ID="ltConselho" runat="server" /></h2>
	<b>Assunto:</b><br />
	<asp:TextBox ID="txtAssunto" runat="server" Width="80%" /><br />
	<b>Texto:</b><br />
	<asp:TextBox ID="txtEmail" runat="server" Width="80%" Rows="5" TextMode="MultiLine" />
	<br />
	<asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnEnviar_Click" />
</asp:Content>
