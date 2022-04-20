<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="eVidaIntranet.controls.Header" %>
		<div id="header">
			<div class="logo">
				<asp:HyperLink ID="lnkInicial" runat="server" NavigateUrl="~/inicial.aspx">
					<asp:Image ID="imgLogo" runat="server" ToolTip="Página inicial" AlternateText="Logo" ImageUrl="~/img/logo-evida2.jpg" />				
				</asp:HyperLink>
			</div>
			<div id="divMenu" runat="server" class="menu">
				<asp:Literal ID="ltNome" runat="server" />
				Último acesso em: <asp:Literal ID="ltUltAcesso" runat="server" /><br />
			</div>
            <div id="divHelp" runat="server" style="text-align:right">
                <asp:Image ID="imgHelp" runat="server" ImageUrl="~/img/remove.png" onclick="return showHelp();" />
            </div>
            <br />
            
			<div class="clear"></div>
		</div>