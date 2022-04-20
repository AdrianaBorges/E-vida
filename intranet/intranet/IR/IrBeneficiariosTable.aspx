<%@ Page Title="IR Beneficiários" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="IrBeneficiariosTable.aspx.cs" Inherits="eVidaIntranet.IR.IrBeneficiariosTable" %>
<%@ Import Namespace="eVida.Web.Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function ConfigControlsCustom() {

		}

		function openPdf() {
			var tipo = $('#<%= dpdTipo.ClientID%>').val();
			if (tipo == "M")
				return openPdfMensalidade();
			else return openPdfReembolso();
		}

		function openPdfMensalidade() {
		    var id = $('#<%= hidMatricula.ClientID%>').val();
		    var emp = $('#<%= hidEmpresa.ClientID%>').val();
		    var cartao = $('#<%= txtCartao.ClientID%>').val();
			var ano = $('#<%= dpdAnoRef.ClientID%>').val();
		    openReport('<%= ReportHandler.EnumRelatorio.MENSALIDADE_IR %>', "ANO=" + ano + "&CARTAO_TITULAR=" + cartao + "&MATRICULA=" + id + "&EMPRESA=" + emp);
			return false;
		}
		function openPdfReembolso() {
		    var cartao = $('#<%= txtCartao.ClientID%>').val();
			var ano = $('#<%= dpdAnoRef.ClientID%>').val();
		    openReport('<%= ReportHandler.EnumRelatorio.REEMBOLSO_IR %>', "ANO=" + ano + "&CARTAO_TITULAR=" + cartao);
			return false;
		}

	    function openPdfFile() {
	        var tipo = $('#<%= dpdTipo.ClientID%>').val();
	        if (tipo == "M")
	            return openPdfFileMensalidade();
	    }
	    function openPdfFileMensalidade() {
	        var cartao = $('#<%= txtCartao.ClientID%>').val();
	        var ano = $('#<%= dpdAnoRef.ClientID%>').val();
	        openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.MENSALIDADE_IR %>', "ID=" + ano + ";" + cartao);
	        return false;
	    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<asp:HiddenField ID="hidMatricula" runat="server" />
	<asp:HiddenField ID="hidEmpresa" runat="server" />
	<table width="850px" cellspacing="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:140px; text-align: right">Ano Base:</td>
			<td align="left" colspan="3"><asp:DropDownList ID="dpdAnoRef" runat="server" Width="80px" /> </td>
		</tr>
		<tr>
			<td style="width:140px; text-align: right">Cartão Titular:</td>
			<td align="left" colspan="3"><asp:TextBox ID="txtCartao" runat="server" Width="180px" /> </td>
		</tr>
		<tr>
			<td style="width:140px; text-align: right">Tipo Comprovante:</td>
			<td align="left" colspan="3"><asp:DropDownList ID="dpdTipo" runat="server" Width="280px">
				<asp:ListItem Text="MENSALIDADE / COPARTICIPAÇÕES" Value="M" />
				<asp:ListItem Text="REEMBOLSO" Value="R" />
			</asp:DropDownList></td>
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
				<asp:GridView ID="gdvReembolso" runat="server" AutoGenerateColumns="false"
					AllowSorting="false" CssClass="tabela" Width="1250px" AllowPaging="false" >
					<Columns>
						<asp:BoundField HeaderText="Mês Referência" DataField="mes_ano" />
						<asp:BoundField HeaderText="Nome" DataField="nm_beneficiario" />
						<asp:BoundField HeaderText="CPF" DataField="nr_cpf" />
						<asp:BoundField HeaderText="Prestador do Serviço" DataField="nm_razao_social" />
						<asp:BoundField HeaderText="CPF/CNPJ do PRESTADOR" DataField="nr_cnpj_cpf" />
						<asp:BoundField HeaderText="Mês atendimento" DataField="dt_atendimento_item" />
						<asp:BoundField HeaderText="Valor Apresentado" DataField="vl_apresentado" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Valor Reembolsado" DataField="vl_reembolso" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Diferença a ser deduzida no IR" DataField="vl_diff" DataFormatString="{0:C}" />
					</Columns>
				</asp:GridView>
				<% if (false) { %><h3>Total</h3><%} %>

				<asp:GridView ID="gdvTotalMensalidade" runat="server" AutoGenerateColumns="false"
					AllowSorting="false" CssClass="tabela" Width="800px" AllowPaging="false" Visible="false" >
					<Columns>
						<asp:BoundField HeaderText="Nome" DataField="nm_beneficiario" SortExpression="nm_beneficiario" />
						<asp:BoundField HeaderText="Co-Participação" DataField="vl_despesa_copart" SortExpression="vl_despesa_copart" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Mensalidade" DataField="vl_despesa_mens" SortExpression="vl_despesa_mens" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="CPF" DataField="nr_cpf" SortExpression="nr_cpf" />
					</Columns>
				</asp:GridView>
			</td>
		</tr>
		<tr>
			<td>
				<% if (false) { %><h3>Detalhes</h3><%} %>
				<asp:GridView ID="gdvMensalidade" runat="server" AutoGenerateColumns="false"
					AllowSorting="false" CssClass="tabela" Width="800px" AllowPaging="false" Visible="false" >
					<Columns>
						<asp:BoundField HeaderText="Mês" DataField="mes_ano" SortExpression="mes_ano" />
						<asp:BoundField HeaderText="Nome" DataField="nm_beneficiario" SortExpression="nm_beneficiario" />
						<asp:BoundField HeaderText="Co-Participação" DataField="vl_despesa_copart" SortExpression="vl_despesa_copart" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Mensalidade" DataField="vl_despesa_mens" SortExpression="vl_despesa_mens" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="CPF" DataField="nr_cpf" SortExpression="nr_cpf" />
					</Columns>
				</asp:GridView>
			</td>
		</tr>
	</table>
</asp:Content>
