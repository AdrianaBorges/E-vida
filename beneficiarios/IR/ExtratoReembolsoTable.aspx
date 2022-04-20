<%@ Page Title="Extrato de Reembolsos" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ExtratoReembolsoTable.aspx.cs" Inherits="eVidaBeneficiarios.IR.ExtratoReembolsoTable" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function ConfigControlsCustom() {

		}

		function openPdf() {
			var id = $('#<%= dpdAno.ClientID%>').val();
			openReport(RELATORIO_REEMBOLSO_IR, "ANO=" + id);
			return false;
		}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="850px" cellspacing="10px" id="tbFitro">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:140px; text-align: right">Ano Ref.:</td>
			<td align="left" colspan="3"><asp:DropDownList ID="dpdAno" runat="server" Width="120px" /> </td>
		</tr>
		<tr>
			<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
                <asp:ImageButton ID="btnExportar" runat="server" Text="Imprimir" OnClientClick="return openPdf()" 
					Visible="false" ImageUrl="~/img/PDF.png" Height="35px" ImageAlign="AbsMiddle" />
		    </td>        
		</tr>
	</table>
	
    <table>
		<tr style="height:300px">
			<td>
                <p style="text-indent: 30px;">
                    O VALOR DA DEDUÇÃO CORRESPONDE  A DIFERENÇA ENTRE O QUE FOI PAGO POR V.SA. E O REEMBOLSADO PELA E-VIDA,  SENDO QUE O "VALOR REAL"  E O "VALOR REEMB"  ESTÃO APRESENTADOS EM REAIS NO MÊS DO PAGAMENTO EFETUADO POR V.SA. E NO MÊS DE REEMBOLSO  EFETUADO PELA OPERADORA, DESDE QUE OS VALORES SEJAM REEMBOLSADOS DENTRO DO MESMO ANO CALENDÁRIO.
                </p>
                <p style="text-indent: 30px;">
                    PARA OS CASOS DE RECIBOS APRESENTADOS NO ANO CALENDÁRIO E REEMBOLSADOS NO ANO CALENDÁRIO POSTERIOR, CONSIDERA-SE COMO DESPESA DEDUTÍVEL O VALOR INTEGRAL DO PAGAMENTO REALIZADO AO PRESTADOR. O VALOR REEMBOLSADO NO ANO CALENDÁRIO POSTERIOR SERÁ INFORMADO COMO RENDIMENTO TRIBUTÁVEL RECEBIDO DE PESSOA JURÍDICA, PASSÍVEL DE TRIBUTAÇÃO, INFORMANDO OS DADOS DA E-VIDA, COMO FONTE PAGADORA, EM CUMPRIMENTO A IN/RFB 1500/2014.
                </p>
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
					AllowSorting="false" CssClass="tabela" Width="1050px" AllowPaging="false" >
					<Columns>
						<asp:BoundField HeaderText="Mês Referência" DataField="mes_ano" />
						<asp:BoundField HeaderText="Nome" DataField="nm_beneficiario" />
						<asp:BoundField HeaderText="CPF" DataField="nr_cpf" />
						<asp:BoundField HeaderText="Prestador do Serviço" DataField="nm_razao_social" />
						<asp:BoundField HeaderText="CPF/CNPJ do PRESTADOR" DataField="nr_cnpj_cpf" />
						<asp:BoundField HeaderText="Mês Atendimento" DataField="dt_atendimento_item" />
						<asp:BoundField HeaderText="Valor Apresentado" DataField="vl_apresentado" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Valor Reembolsado" DataField="vl_reembolso" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Diferença a ser deduzida no IR" DataField="vl_diff" DataFormatString="{0:C}" />
					</Columns>
				</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>
