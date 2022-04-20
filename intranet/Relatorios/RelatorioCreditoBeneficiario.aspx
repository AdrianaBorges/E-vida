<%@ Page Title="Créditos do Beneficiário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioCreditoBeneficiario.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioCreditoBeneficiario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:140px; text-align: right">Período de solicitação:</td>
		<td align="left">de <asp:TextBox ID="txtInicio" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  /> até 
                <asp:TextBox ID="txtFim" runat="server" Width="80px" MaxLength="10" CssClass="calendario" /></td>
        <td style="width:160px; text-align: right">Nº da Carteirinha:</td>
        <td><asp:TextBox ID="txtCartao" runat="server" Width="100%" MaxLength="50"/></td>
	</tr>
    <tr>
		<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		</td>
	</tr>
    </table>

    
    <table>
	<tr style="height:300px">
		<td>
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="1200px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="CARTEIRINHA" DataField="cd_alternativo" ItemStyle-Wrap="false" />
                    <asp:BoundField HeaderText="NOME BENEFICIARIO" DataField="nm_beneficiario" />
					<asp:BoundField HeaderText="CÓD. PLANO" DataField="cd_plano" HeaderStyle-Width="40px" />
					<asp:BoundField HeaderText="PLANO" DataField="ds_plano" />
					<asp:BoundField HeaderText="CÓD. CATEGORIA" DataField="cd_categoria" HeaderStyle-Width="70px" />
					<asp:BoundField HeaderText="CATEGORIA" DataField="ds_categoria" />
					<asp:BoundField HeaderText="DATA SOLICITAÇÃO" DataField="dt_solic_pagamento" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-Width="70px" />
					<asp:BoundField HeaderText="DATA PAGAMENTO" DataField="dt_pagamento" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-Width="70px" />
                    <asp:BoundField HeaderText="VALOR TOTAL" DataField="vl_pagar" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
