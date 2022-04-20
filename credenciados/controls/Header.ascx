<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="eVidaCredenciados.controls.Header" %>
<div id="header">
	<div class="logo">
		<asp:HyperLink ID="lnkInicial" runat="server" NavigateUrl="~/inicial.aspx">
			<asp:Image ID="imgLogo" runat="server" ToolTip="Página inicial" AlternateText="Logo" ImageUrl="~/img/logo.png" Height="195" Width="211" />				
		</asp:HyperLink>
	</div>
	<div id="divMenu" class="menu">
		<div id="divHeaderInfo" runat="server" >
			Bem vindo: <asp:Literal ID="ltNome" runat="server" /><br />
		</div>
		<div id="divHeaderInfo2" runat="server" style="height:20px">
			<br />
		</div>
		<div style="height:110px"><br /></div>
		<div id="divTitle" style="padding: 10px; text-align:center; color:#FFF"><%= this.Page.Title  %></div>
	</div>
    <br />
            
	<div class="clear"></div>
</div>