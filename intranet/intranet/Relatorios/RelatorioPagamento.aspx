<%@ Page Title="PAGAMENTO" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioPagamento.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioPagamento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="1000px" cellspacing="10px">
	    <tr>
		    <td colspan="5"><h2 class="componentheading">Filtros</h2></td>
	    </tr>
	    <tr>
		    <td style="width:150px; text-align: right">Início:</td>
		    <td align="left"><asp:TextBox ID="txtInicio" runat="server" Width="120px" MaxLength="10" CssClass="calendario"  /></td>
            <td style="width:100px; text-align: right">Fim:</td>
            <td align="left"><asp:TextBox ID="txtFim" runat="server" Width="120px" MaxLength="10" CssClass="calendario" /></td>
            <td><asp:RadioButtonList ID="rblTipoRelatorio" runat="server">
                <asp:ListItem Value="A" Text="Agrupado" Selected="True" />
                <asp:ListItem Value="D" Text="Detalhado" />
                </asp:RadioButtonList></td>
	    </tr>
		<tr>
			<td style="width:190px; text-align: right">Tipo Pessoa Credenciado:</td>
			<td><asp:DropDownList ID="dpdTipoPessoa" runat="server">
				<asp:ListItem Value="" Text="TODOS" />
				<asp:ListItem Value="F" Text="FÍSICA" />
				<asp:ListItem Value="J" Text="JURÍDICA" />
			    </asp:DropDownList></td>
		</tr>
		<tr>
			<td style="width:150px; text-align: right">Regional Credenciado:</td>
			<td colspan="4"><asp:CheckBoxList ID="chkRegional" runat="server" DataValueField="Codigo" DataTextField="Descricao" RepeatColumns="3" Width="800px" BorderStyle="Solid" /></td>
		</tr>
	    <tr>
		    <td colspan="5" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		    </td>
	    </tr>
    </table>
    <table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblCount" runat="server" Visible="false" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" 
				AllowSorting="false" CssClass="tabela" Width="3500px" OnSorting="gdvRelatorio_Sorting"
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="CODIGO EMPRESA" DataField="cd_empresa" SortExpression="cd_empresa" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="EMPRESA" DataField="ds_empresa" SortExpression="ds_empresa" />
					<asp:BoundField HeaderText="CPF/CNPJ" DataField="nr_cnpj_cpf" SortExpression="nr_cnpj_cpf" />
					<asp:BoundField HeaderText="Razão Social Credenciado" DataField="nm_razao_social" SortExpression="nm_razao_social" />
					<asp:BoundField HeaderText="Tipo Pessoa Cred." DataField="tp_pessoa" SortExpression="tp_pessoa" />
                    <asp:BoundField HeaderText="Tipo Plano" DataField="tp_plano" SortExpression="tp_plano" />
					<asp:BoundField HeaderText="Plano" DataField="ds_plano" SortExpression="ds_plano" />
					<asp:BoundField HeaderText="Categoria" DataField="ds_categoria" SortExpression="ds_categoria" />

                    <asp:BoundField HeaderText="Tipo DOC" DataField="tp_docto" SortExpression="tp_docto" />
                    <asp:BoundField HeaderText="NR Documento Fiscal" DataField="nr_docto" SortExpression="nr_docto" />
                    <asp:BoundField HeaderText="Data Recebimento" DataField="dt_recto_docto" SortExpression="dt_recto_docto" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Data Vencimento" DataField="dt_vencto_docto" SortExpression="dt_vencto_docto" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Data Pagamento" DataField="dt_pagamento" SortExpression="dt_pagamento" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Valor Doc Fiscal" DataField="vl_docto" SortExpression="vl_docto" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Documento Complementar" DataField="nr_docto_complementar" SortExpression="nr_docto_complementar" />
                    <asp:BoundField HeaderText="Valor Doc Complementar" DataField="vl_docto_complementar" SortExpression="vl_docto_complementar" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Despesa Saúde" DataField="vl_despesa_saude" SortExpression="vl_despesa_saude" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="GLOSA" DataField="vl_glosa" SortExpression="vl_glosa" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Valor Bruto" DataField="vl_bruto" SortExpression="vl_bruto" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="NATUREZA" DataField="ds_natureza" SortExpression="ds_natureza" />
                    <asp:BoundField HeaderText="INSS EMP" DataField="vl_inss_emp" SortExpression="vl_inss_emp" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="INSS CRED" DataField="vl_inss_cred" SortExpression="vl_inss_cred" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="IRRF" DataField="vl_irrf" SortExpression="vl_irrf" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="PIS" DataField="vl_pis" SortExpression="vl_pis" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="COFINS" DataField="vl_cofins" SortExpression="vl_cofins" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="CSLL" DataField="vl_csll" SortExpression="vl_csll" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="ISS" DataField="vl_iss" SortExpression="vl_iss" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Valor Liq Pago" DataField="vl_credito" SortExpression="vl_credito" DataFormatString="{0:C}" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
