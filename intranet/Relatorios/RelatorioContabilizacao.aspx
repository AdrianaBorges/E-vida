<%@ Page Title="CONTABILIZACAO E-VIDA/ELN" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioContabilizacao.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioContabilizacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<table width="1100px" cellspacing="10px">
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
		<td style="width:100px; text-align: right">Sistema de Atendimento:</td>
		<td align="left"><asp:DropDownList ID="dpdSistema" runat="server" Width="120px">
            <asp:ListItem Value="" Text="Todos" />
            <asp:ListItem Value="CRED" Text="Credenciamento" />
            <asp:ListItem Value="REEMB" Text="Reembolso" />
            </asp:DropDownList></td>
        <td style="width:100px; text-align: right">Situação Pagamento:</td>
        <td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="120px">
            <asp:ListItem Value="" Text="Todos" />
            <asp:ListItem Value="R" Text="Realizado Pagamento" />
            <asp:ListItem Value="V" Text="Valorado" />
            <asp:ListItem Value="C" Text="Consistido" />
            <asp:ListItem Value="A" Text="Aberto" />
            </asp:DropDownList></td>
	</tr>
    <tr>
        <td>Categorias:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkCategoria" runat="server" DataValueField="cd_categoria" DataTextField="Description" RepeatColumns="3" Width="1000px" BorderStyle="Solid" />
        </td>
    </tr>
    <tr>
        <td>Empresas:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkEmpresa" runat="server" DataValueField="cd_empresa" DataTextField="Description" RepeatColumns="3" Width="1000px" BorderStyle="Solid" />
        </td>
    </tr>    
    <tr>
        <td>Planos:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkPlano" runat="server" DataValueField="CdPlano" DataTextField="DsPlano" RepeatColumns="3" Width="1000px" BorderStyle="Solid"  />
        </td>
    </tr>
	<tr>
		<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		</td>
	</tr>
    </table>
    <table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblCount" runat="server" Visible="false" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="2600px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="MÊS" DataField="dt_ano_mes_ref" SortExpression="dt_ano_mes_ref" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="CPF/CNPJ" DataField="nr_cnpj_cpf" SortExpression="nr_cnpj_cpf" />
					<asp:BoundField HeaderText="Razão Social" DataField="nm_razao_social" SortExpression="nm_razao_social" />
                    <asp:BoundField HeaderText="Empresa" DataField="cd_empresa" SortExpression="cd_empresa" DataFormatString="{0:0}"/>
                    <asp:BoundField HeaderText="Plano" DataField="ds_plano" SortExpression="ds_plano" />
                    <asp:BoundField HeaderText="Categoria" DataField="ds_categoria" SortExpression="ds_categoria" />
                    <asp:BoundField HeaderText="PID" DataField="PID" SortExpression="PID" />
                    
                    <asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" SortExpression="cd_funcionario" DataFormatString="{0:0}"/>
					<asp:BoundField HeaderText="BENEFICIÁRIO" DataField="nm_beneficiario" />
                    <asp:BoundField HeaderText="LOTACAO" DataField="cd_lotacao" SortExpression="cd_lotacao" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="CENTRO CUSTO" DataField="cd_centro_custo" SortExpression="cd_centro_custo" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="Valor Doc Fiscal" DataField="vl_docto" SortExpression="vl_docto" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Despesa Saúde" DataField="vl_despesa_saude" SortExpression="vl_despesa_saude" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="GLOSA" DataField="vl_glosa" SortExpression="vl_glosa" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="PARTE EMPRESA" DataField="vl_particip_empresa" SortExpression="vl_particip_empresa" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="PARTE EMPREGADO" DataField="vl_participacao" SortExpression="vl_participacao" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="SISTEMA DE ATENDIMENTO" DataField="tp_sistema_atend" SortExpression="tp_sistema_atend" />
                    <asp:BoundField HeaderText="TIPO DOC" DataField="tp_docto" SortExpression="tp_docto" />
                    <asp:BoundField HeaderText="NR DOCUMENTO FISCAL" DataField="nr_docto" SortExpression="nr_docto" />
                    <asp:BoundField HeaderText="PROTOCOLO ISA" DataField="nr_protocolo" SortExpression="nr_protocolo" />
                    <asp:BoundField HeaderText="SITUAÇÃO PGTO" DataField="st_atendimento" SortExpression="st_atendimento" />
                    <asp:BoundField HeaderText="DATA RECEBIMENTO" DataField="dt_recto_docto" SortExpression="dt_recto_docto" DataFormatString="{0:dd/MM/yyyy}" />                    
                    <asp:BoundField HeaderText="Data Vencimento" DataField="dt_vencto_docto" SortExpression="dt_vencto_docto" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Data Valoração" DataField="dt_valoracao" SortExpression="dt_valoracao" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Data Pagamento" DataField="dt_pagamento" SortExpression="dt_pagamento" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="MED. TRABALHO" DataField="med_trabalho" SortExpression="med_trabalho" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
