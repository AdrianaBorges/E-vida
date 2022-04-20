<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="eVidaBeneficiarios.controls.Header" %>
		<div id="header">
			<div class="logo">
				<asp:HyperLink ID="lnkInicial" runat="server" NavigateUrl="~/inicial.aspx">
					<asp:Image ID="imgLogo" runat="server" ToolTip="Página inicial" AlternateText="Logo" ImageUrl="~/img/logo.png" Height="195" Width="211" />				
				</asp:HyperLink>
			</div>
			<div id="divMenu" class="menu">
				<div id="divHeaderInfo" runat="server" >
					Bem vindo: <asp:Literal ID="ltNome" runat="server" /><br />
                    Empresa: <asp:Literal ID="ltEmpresa" runat="server" /><br />
					Plano: <asp:Literal ID="ltPlano" runat="server" /><br />
					Vigência do Plano: <asp:Literal ID="ltVigenciaPlano" runat="server" /><br />
					Categoria: <asp:Literal ID="ltCategoria" runat="server" /><br />
					Vigência da Categoria: <asp:Literal ID="ltVigenciaCategoria" runat="server" /><br />
				</div>
				<div id="divHeaderInfo2" runat="server" style="height:130px">
					<br />
				</div>
				<div style="height:10px"></div>                
				<div id="divTitle" style="padding: 10px; text-align:center; color:#FFF"><%= this.Page.Title  %></div>
			</div>
			
            <br />
            
			<div class="clear"></div>
		</div>