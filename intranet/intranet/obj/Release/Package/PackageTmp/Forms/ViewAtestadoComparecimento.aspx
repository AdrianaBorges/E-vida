<%@ Page Title="Atestado de Comparecimento" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="ViewAtestadoComparecimento.aspx.cs" Inherits="eVidaIntranet.Forms.ViewAtestadoComparecimento" %>
<%@ Import Namespace="eVidaGeneralLib.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function imprimir() {
    		window.print();
    		return false;
    	}

    	$(document).ready(function () {
    		imprimir();
    	});
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo" style="margin-left: 25px; width:100%">
		<table id="headerPop" style="border-width: thin; border-style: solid;">
			<tr>
				<td>
					<img class="logo1" src="../img/logo.png" alt="E-Vida" width="80px" />
				</td>
				<td style="width:500px; text-align:center"><h1 style="width:100%">ATESTADO DE COMPARECIMENTO</h1>				
					<br />					
					Procolo: <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true" />
				</td>
				<td align="right">				
					<img src="../img/logo-eletrobras.png" alt="Eletrobras"/>
				</td>
			</tr>
    
		</table>
        <br /><br />
	    <table class="tbTitular">
            <tr>
				<td>
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão" /><br />
				    <asp:Label ID="txtCartao" runat="server" Width="150px" Font-Bold="true" />
                </td>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Titular" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="600px" Font-Bold="true"/>					
                </td>                
            </tr>
			<tr>
				<td></td>
			</tr>
            <tr>
                <td>
				    <asp:Label ID="lblLotacao" runat="server" Text="Lotação" /><br />
				    <asp:Label ID="txtLotacao" runat="server" Width="250px" Font-Bold="true" />
                </td>
				<td>
				    <asp:Label ID="lblBeneficiario" runat="server" Text="Beneficiário" /><br />
				    <asp:Label ID="txtBeneficiario" runat="server" Width="600px" Font-Bold="true" />
                </td>
            </tr>
        </table>
        <br />
        
		<table id="tbPeriodo">
            <tr>
                <td> Atesto que o(a) beneficiário(a) acima identificado(a) esteve nas dependências da E-VIDA,
					no dia <asp:Label ID="txtData" runat="server" Font-Bold="true" />,
					no período de <asp:Label ID="txtHoraInicial" runat="server" Font-Bold="true" />
					às <asp:Label ID="txtHoraFinal" runat="server" Font-Bold="true" />
					para atendimento conforme abaixo:
                </td>
            </tr>
	    </table>


		<br /><br /><br />
        <table id="tbAtendimento">
            <tr>
				<td style="width:50%; text-align:center">
					<asp:CheckBoxList ID="chkTipoAtendimento" runat="server" Enabled="false" RepeatColumns="2">
						<asp:ListItem Value="1" Text="Perícia Odontológica" />
						<asp:ListItem Value="2" Text="Perícia Psicológica" />
						<asp:ListItem Value="4" Text="Perícia Médica" />
						<asp:ListItem Value="8" Text="Setor de Cadastro" />
						<asp:ListItem Value="16" Text="Setor de Credenciamento" />
						<asp:ListItem Value="32" Text="Setor de Reembolso" />
						<asp:ListItem Value="64" Text="Setor de Suporte TI" />
						<asp:ListItem Value="128" Text="Setor Financeiro" />
					</asp:CheckBoxList>
				</td>
				<td>					
					<div style="text-align:center">
						<br />
						Brasília, <asp:Literal ID="ltData" runat="server" /> 
						<br />  
						<br />
						<br />
						_____________________________________________________
						Carimbo e Assinatura          
					</div>
				</td>
			</tr>
		</table>
    </div>
</asp:Content>
