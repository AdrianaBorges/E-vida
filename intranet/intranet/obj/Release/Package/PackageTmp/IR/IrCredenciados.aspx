<%@ Page Title="IR Credenciados" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="IrCredenciados.aspx.cs" Inherits="eVidaIntranet.IR.IrCredenciados" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function openIr(tipo) {
			window.open('IrCredenciados.aspx?DW_FILE=' + tipo);
			return false;
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>				
		<tr>
			<td style="width:140px; text-align: right">Ano Base:</td>
			<td align="left" colspan="3"><asp:DropDownList ID="dpdAnoRef" runat="server" Width="80px" /> </td>
		</tr>
		<tr>
			<td style="width:140px; text-align: right">Razão Social:</td>
			<td align="left" colspan="3"><asp:TextBox ID="txtRazaoSocial" runat="server" Width="320px" /> </td>
		</tr>
		<tr>
			<td style="width:140px; text-align: right">CPF/CNPJ:</td>
			<td align="left" colspan="3"><asp:TextBox ID="txtCnpj" runat="server" Width="180px" /> </td>
		</tr>
		<tr>
			<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
		    </td>        
		</tr>
	</table>
	<table>
		<tr style="height:300px">
			<td>
				<asp:Label ID="lblCount" runat="server" Visible="false" />
				<asp:GridView ID="gdvResultado" runat="server" AutoGenerateColumns="false"
					AllowSorting="false" CssClass="tabela" Width="850px" AllowPaging="false"
					OnRowDataBound="gdvResultado_RowDataBound" >
					<Columns>
						<asp:BoundField HeaderText="CPF/CNPJ do PRESTADOR" DataField="nr_cnpj_cpf" ItemStyle-HorizontalAlign="Right" />
						<asp:BoundField HeaderText="Prestador do Serviço" DataField="nm_razao_social" />
						<asp:BoundField HeaderText="Tipo pessoa" DataField="tp_pessoa" />
						<asp:HyperLinkField HeaderText="Comprovante Rendimentos e Retenções"  ItemStyle-HorizontalAlign="Center"
							DataNavigateUrlFields="CD_CREDENCIADO" DataNavigateUrlFormatString="IrCredenciados.aspx?id={0}&DW_FILE=1078"
							Text="Baixar" Target="ir_credenciado" />
					</Columns>
				</asp:GridView>
			</td>
		</tr>
	</table>
	
</asp:Content>
