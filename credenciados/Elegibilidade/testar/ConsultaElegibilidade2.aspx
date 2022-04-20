<%@ Page Title="Consultar Elegibilidade" Language="C#" MasterPageFile="~/Internal/ExternalPages.Master" AutoEventWireup="true" CodeBehind="ConsultaElegibilidade.aspx.cs" Inherits="eVidaCredenciados.Elegibilidade.ConsultaElegibilidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openAutPrevia() {
            window.open('http://e-vida.org.br/images/credenciamento/CR005_autorizacao_previa%20.pdf', 'AUT_PREVIA');
            return false;
        }
        function openInfoCred() {
            window.open('http://www.e-vida.org.br/index.php/credenciado/informativos-para-rede', 'INFO_CRED');
            return false;
        }
    </script>
    <style type="text/css">
        @media screen {
            #placeHolder_tbResult {

            }
        }
        @media print {
            /*#placeHolder_tbResult {
                display: none;
                visibility:hidden;
            }*/

            .rodape_reciprocidade {
                display: none;
                visibility:hidden;            
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="header">
	    <div class="logo">
		    <asp:HyperLink ID="lnkInicial" runat="server" NavigateUrl="~/Elegibilidade/ConsultaElegibilidade.aspx">
			    <asp:Image ID="imgLogo" runat="server" ToolTip="Página inicial" AlternateText="Logo" ImageUrl="~/img/logo.png" Height="195" Width="211" />				
		    </asp:HyperLink>
	    </div>
	    <div id="divMenu" class="menu">
		    <div id="divHeaderInfo2" runat="server" style="height:110px;">
			    <br />
		    </div>
		    <div style="height:25px;"><br /></div>
            <div id="divTitle" style="padding: 5px; text-align:center; color:#FFF;" ><%= this.Title  %></div>
	    </div>
			
        <br />
            
	    <div class="clear"></div>
    </div>
    <table width="50%" style="margin-left:200px">
        <tr>
            <td colspan="2"><asp:Label ID="lblLoginExpirado" runat="server" Text="Sua sessão expirou!" Visible="false" /></td>
        </tr>
		<tr>
			<td style="width:150px">Número da Carteira:</td>
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
			<td colspan="2" align="center"><asp:Button ID="btnLogin" runat="server" Text="Enviar" onclick="btnLogin_Click" /></td>
		</tr>
    </table>
    <table id="tbResult" runat="server" cellpadding="10" cellspacing="10" style="width: 80%; margin-left:200px" visible="false" >
        <tr>

            <td colspan="2"><h2>Resultado</h2></td>
        </tr>
        <tr>
            <td style="width:150px"><b>Situação:</b></td>
            <td><asp:Label ID="lblSituacao" runat="server" Font-Bold="true" /></td>
        </tr>
        <tr>
            <td><b>Nome do Beneficiário:</b></td>
            <td><asp:Literal ID="litNomeBeneficiario" runat="server" /></td>
        </tr>
        <tr>
            <td><b>Data de Nascimento:</b></td>
            <td><asp:Literal ID="litDataNascimento" runat="server" /></td>
        </tr>
        <tr>
            <td><b>Número do Cartão:</b></td>
            <td><asp:Literal ID="litCartao" runat="server" /></td>
        </tr>
        <tr>
            <td><b>Operadora:</b></td>
            <td>E-VIDA - 41873-4</td>
        </tr>
        <tr>
            <td><b>Plano</b></td>
            <td><asp:Literal ID="litPlano" runat="server" /></td>
        </tr>
        <tr>
            <td><b>Início Cobertura:</b></td>
            <td><asp:Literal ID="litCobertura" runat="server" /></td>
        </tr>
        <tr>
            <td><b>Carência:</b></td>
            <td><asp:Label ID="lblCarencia" runat="server" Font-Bold="true" /></td>
        </tr>
        <tr>
            <td><b>Observação:</b></td>
            <td><asp:Literal ID="litObservacao" runat="server" /></td>
        </tr>
        <tr class="rodape_reciprocidade">
            <td colspan="2" align="center">
                <br /><br />
                <asp:Button ID="btnInformativo" runat="server" Text="Informativos para a rede credenciada" OnClientClick="return openInfoCred();" />
            </td>
        </tr>
        <tr class="rodape_reciprocidade">
            <td colspan="2">
                <p style="font-size:large"> Atendimentos de Urgência/Emergência devem ser realizados independente do período de carência</p>
            </td>
        </tr>
    </table>
</asp:Content>
