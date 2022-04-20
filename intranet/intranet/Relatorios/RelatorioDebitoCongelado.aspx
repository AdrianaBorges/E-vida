<%@ Page Title="Débitos Congelados" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioDebitoCongelado.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioDebitoCongelado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<table width="850px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:140px; text-align: right">Mês de Referência:</td>
		<td align="left"><asp:TextBox ID="txtMesReferencia" runat="server" CssClass="calendario" MaxLength="10" Width="100px" /></td>
	</tr>
    <tr>
        <td style="text-align: right">Plano:</td>
        <td><asp:DropDownList ID="dpdPlano" runat="server" Width="200px" DataValueField="CdPlano" DataTextField="DsPlano" /></td>
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
				AllowSorting="false" CssClass="tabela" Width="1500px" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
                    <asp:BoundField HeaderText="MES/ANO REF" DataField="dt_ano_mes_ref" DataFormatString="{0:MM/yyyy}" />
                    <asp:BoundField HeaderText="EMPRESA" DataField="cd_empresa" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="NOME" DataField="nm_beneficiario"/>
					<asp:BoundField HeaderText="COD PLANO" DataField="cd_plano" />
					<asp:BoundField HeaderText="PLANO" DataField="DS_PLANO" />
					<asp:BoundField HeaderText="CÓD. CATEGORIA" DataField="cd_categoria" DataFormatString="{0:0}"  />
					<asp:BoundField HeaderText="CATEGORIA" DataField="ds_categoria" />
					<asp:BoundField HeaderText="GRUPO DE LANÇAMENTO" DataField="cd_grupo_lancto" />
                    <asp:BoundField HeaderText="TOTAL_31052016" DataField="total31052016" DataFormatString="{0:C}"/>
                    <asp:BoundField HeaderText="RECEBIMENTO ACUMULADO" DataField="recebimento_acumulado" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="RECEBIMENTO MES ANTERIOR" DataField="recebimento_mes_anterior" DataFormatString="{0:C}" />
					<asp:BoundField HeaderText="RECEBIMENTO MÊS" DataField="recebimento_mes" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="SALDO DEVEDOR" DataField="saldo_devedor" DataFormatString="{0:C}" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>

