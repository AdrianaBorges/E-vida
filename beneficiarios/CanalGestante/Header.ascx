<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="eVidaBeneficiarios.CanalGestante.Header" %>
		<div id="header">
			<div class="logo">
				<asp:HyperLink ID="lnkInicial" runat="server" NavigateUrl="~/CanalGestante/CanalGestante.aspx">
					<asp:Image ID="imgLogo" runat="server" ToolTip="Página inicial" AlternateText="Logo" ImageUrl="~/img/logo.png" Height="195" Width="211" />				
				</asp:HyperLink>
			</div>
			<div id="divMenu" class="menu">
				<div id="divHeaderInfo" runat="server" >
					Bem vindo: <asp:Literal ID="ltNome" runat="server" /><br />
                    Nº do Cartão: <asp:Literal ID="ltCartao" runat="server" /><br />
                    Plano: <asp:Literal ID="ltPlano" runat="server" /><br />
                    <br />
                    <br />
				</div>
				<div id="divHeaderInfo2" runat="server" style="height:120px">
					<br />
				</div>
				<div style="height:25px"><br /></div>
                <div id="divTitle" style="padding: 5px; text-align:center; color:#FFF"><%= this.Page.Title  %></div>
			</div>
			
            <br />
            
			<div class="clear"></div>
		</div>