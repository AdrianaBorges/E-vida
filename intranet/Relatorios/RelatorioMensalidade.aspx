<%@ Page Title="Mensalidades" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioMensalidade.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioMensalidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
            
        }

    </script>
    <style type="text/css">
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    <table width="1050px" cellspacing="10px">
	<tr>
		<td colspan="2"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:100px; text-align: right">Mês Ref.:</td>
		<td align="left" colspan="3"><asp:DropDownList ID="dpdMes" runat="server" Width="120px" /> <asp:DropDownList ID="dpdAno" runat="server" Width="90px" /></td>
	</tr>
    <tr>
		<td style="width:100px; text-align: right">Planos:</td>        
        <td>
            <asp:CheckBoxList ID="chkListPlano" runat="server" DataValueField="Codigo" DataTextField="Descricao" RepeatColumns="3" BorderStyle="Solid"/>
        </td>
    </tr>
    <tr>
        <td style="width:100px; text-align: right">Categorias:</td>
        <td>
            <asp:CheckBoxList ID="chkListCat" runat="server" BorderStyle="Solid"/>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		    </td>        
	</tr>
    </table>


    <table>
	<tr style="height:300px">
		<td>
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="1300px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="Mes/Ano Ref" DataField="dt_ano_mes_ref"/>
					<asp:BoundField HeaderText="PLANO" DataField="cd_grupo_lancto"/>
					<asp:BoundField HeaderText="CATEGORIA" DataField="cd_categoria"/>
                    <asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="EMPRESA" DataField="cd_empresa" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="TIPO" DataField="tp_beneficiario"/>
                    <asp:BoundField HeaderText="NOME BENEFICIARIO" DataField="nm_beneficiario"/>
                    <asp:BoundField HeaderText="CARTEIRA" DataField="cd_alternativo"/>
                    <asp:BoundField HeaderText="SALÁRIO" DataField="vl_salario" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right"  />
                    <asp:BoundField HeaderText="TOTAL" DataField="vl_lancto" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="BENEFICIARIO" DataField="vl_beneficiario" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="EMPRESA" DataField="vl_empresa" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="IDADE" DataField="idade" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="MATRICULA RESP. FAMILIA" DataField="cd_mat_resp_familia" DataFormatString="{0:0}"/>
                    <asp:BoundField HeaderText="MATRICULA RESP. FINANCEIRO" DataField="cd_mat_resp_financeiro" DataFormatString="{0:0}" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
