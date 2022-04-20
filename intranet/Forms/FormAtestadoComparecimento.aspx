<%@ Page Title="Atestado de Comparecimento" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormAtestadoComparecimento.aspx.cs" Inherits="eVidaIntranet.Forms.FormAtestadoComparecimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function ConfigControlsCustom() {
        }
        function goNova() {
        	window.location = "FormAtestadoComparecimento.aspx";
        	return false;
        }
        function confirmSave() {
        	var msg = "Ao confirmar esta solicitação, o setor de cadastro providenciará a inclusão na reciprocidade e o informará através de e-mail as orientações cabíveis quando da providência. Só confirme caso tenha certeza. Não há necessidade de entregar o formulário na E-VIDA";
        	//return confirm(msg);
        	return true;
        }

        function openPdf() {
        	openReport(RELATORIO_ATESTADO_COMPARECIMENTO, 'ID=' + $('#<%= litProtocolo.ClientID %>').val());
        	return false;
        }

    	function goView() {
    		window.open('ViewAtestadoComparecimento.aspx?ID=' + $('#<%= litProtocolo.ClientID %>').val(), 'comparecimento',
				'width=800px, height=800px');
    		return false;
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo">
        <asp:HiddenField ID="litProtocolo" runat="server" />
        <h2>1 - Dados do Beneficiário</h2>
	    <table class="tbTitular" cellspacing="10px">
            <tr>
				<td>
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão Titular" /><br />
				    <asp:TextBox ID="txtCartao" runat="server" Width="150px" OnTextChanged="txtCartao_TextChanged" AutoPostBack="true"/>
                </td>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Titular" /><br />
				    <asp:TextBox ID="txtNomeTitular" runat="server" Width="600px"/>					
                </td>                
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblLotacao" runat="server" Text="Lotação" /><br />
				    <asp:TextBox ID="txtLotacao" runat="server" Width="250px" MaxLength="5" ToolTip="Digite no máximo 5 caracteres" />
                </td>
				<td>
					<asp:Label ID="lblBeneficiario" runat="server" Text="Beneficiário" /><br />
				    <asp:DropDownList ID="dpdBeneficiario" runat="server" Width="600px" DataValueField="Cdusuario" DataTextField="Nomusr" />
				</td>
            </tr>
        </table>
        <br />
        
        <h2>2 - Datas e Horários</h2>
	    <table id="tbPeriodo">
            <tr>
                <td> Atesto que o(a) beneficiário(a) acima identificado(a) esteve nas dependências da E-VIDA,
					no dia <asp:TextBox ID="txtData" runat="server" CssClass="calendario" MaxLength="10" Width="100px" />,
					no período 
                    de <asp:TextBox ID="txtHoraInicial" runat="server" MaxLength="2" Width="30px" />:<asp:TextBox ID="txtMinInicial" runat="server" MaxLength="2" Width="30px" />
					às <asp:TextBox ID="txtHoraFinal" runat="server" MaxLength="2" Width="30px" />:<asp:TextBox ID="txtMinFinal" runat="server" MaxLength="2" Width="30px" />
					para atendimento conforme abaixo:
                </td>
            </tr>
	    </table>
        
        <h2>3 - Tipo de Atendimento</h2>
	    <table id="tbAtendimento">
            <tr>
				<td style="width:50%; text-align:center">
					<asp:CheckBoxList ID="chkTipoAtendimento" runat="server" RepeatColumns="2">
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
						_____________________________________________________<br />
						Carimbo e Assinatura          
					</div>
				</td>
			</tr>
		</table>
        

        <div>
            <table width="100%">
                <tr>
                    <td align="center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" OnClientClick="return confirmSave()" />
						<asp:Button ID="btnImprimir" runat="server" Text="Imprimir" OnClientClick="return goView()" Visible="false" />
						<asp:Button ID="btnPdf" runat="server" Text="Gerar PDF" OnClientClick="return openPdf()" Visible="false" />
						<asp:Button ID="btnFinalizar" runat="server" Text="Finalizar" OnClick="btnFinalizar_Click" Visible="false" />
						
                    </td>
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>
