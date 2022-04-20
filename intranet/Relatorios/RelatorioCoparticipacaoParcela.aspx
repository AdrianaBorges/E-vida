<%@ Page Title="Relatório de Coparticipação - Parcela" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioCoparticipacaoParcela.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioCoparticipacaoParcela" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    <table width="850px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:100px; text-align: right">Início:</td>
		<td align="left"><asp:TextBox ID="txtInicio" runat="server" Width="120px" MaxLength="10" CssClass="calendario"  /></td>
        <td style="width:100px; text-align: right">Fim:</td>
        <td align="left"><asp:TextBox ID="txtFim" runat="server" Width="120px" MaxLength="10" CssClass="calendario" /></td>
	</tr>
    <tr>
		<td style="width:140px; text-align: right">Situação:</td>
        <td align="left">
            <asp:CheckBoxList ID="chkListSituacao" runat="server">
                <asp:ListItem Value="P" Text="PENDENTE" />
                <asp:ListItem Value="R" Text="RECEBIDO" />
            </asp:CheckBoxList>
        </td>
        <td style="width:100px; text-align: right">Cartão Titular:</td>
        <td align="left"><asp:TextBox ID="txtCartaoTitular" runat="server" Width="220px" MaxLength="50" /></td>
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
				AllowSorting="false" CssClass="tabela" Width="1500px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="Mes/Ano Ref" DataField="dt_ano_mes_ref" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Mes/Ano Ref Cob" DataField="dt_ano_mes_ref_cob" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="PARCELA" DataField="nr_parcela" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="EMPRESA" DataField="cd_empresa" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="NOME BENEFICIARIO" DataField="nm_beneficiario" />
                    <asp:BoundField HeaderText="ENDERECO" DataField="endereco" />
                    <asp:BoundField HeaderText="BAIRRO" DataField="ds_bairro" />
                    <asp:BoundField HeaderText="CIDADE" DataField="ds_municipio" />
                    <asp:BoundField HeaderText="UF" DataField="DS_UF" />
                    <asp:BoundField HeaderText="CEP" DataField="nr_cep" />
					<asp:BoundField HeaderText="GRUPO DE LANCAMENTO" DataField="cd_grupo_lancto" />
					<asp:BoundField HeaderText="CÓD. PLANO" DataField="cd_plano" />
					<asp:BoundField HeaderText="PLANO" DataField="ds_plano" />
					<asp:BoundField HeaderText="CÓD. CATEGORIA" DataField="cd_categoria" />
					<asp:BoundField HeaderText="CATEGORIA" DataField="ds_categoria" />
                    <asp:BoundField HeaderText="VALOR PARCELA" DataField="vl_parcela" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="VALOR RECEBIDO" DataField="vl_recebido" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="DATA PAGAMENTO" DataField="dt_recebimento" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="SITUAÇÃO" DataField="situacao" />
                    <asp:BoundField HeaderText="TIPO COBRANÇA" DataField="tp_cobranca" />
                    <asp:BoundField HeaderText="TIPO QUITAÇÃO" DataField="tp_quitacao" />
                    <asp:BoundField HeaderText="MATRICULA RESP. FAMILIA" DataField="cd_mat_resp_familia" DataFormatString="{0:0}"/>
                    <asp:BoundField HeaderText="MATRICULA RESP. FINANCEIRO" DataField="cd_mat_resp_financeiro" DataFormatString="{0:0}" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
