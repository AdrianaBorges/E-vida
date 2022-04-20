<%@ Page Title="BENEFICIÁRIOS POR LOCAL" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioBeneficiariosPorLocal.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioBeneficiariosPorLocal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<table width="1200px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td><b>Tipo:</b></td>           
		<td><asp:RadioButtonList ID="rblTipoRelatorio" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Value="A" Text="Analítico" Selected="True" />
            <asp:ListItem Value="S" Text="Sintético" />
           </asp:RadioButtonList></td>
	</tr>
    <tr>
        <td>UF:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkUf" runat="server" DataValueField="sigla" DataTextField="nome" RepeatColumns="3" Width="1000px" BorderStyle="Solid" />
        </td>
    </tr>
    <tr>
        <td>Regionais:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkRegional" runat="server" DataValueField="Codigo" DataTextField="Descricao" RepeatColumns="3" Width="1000px" BorderStyle="Solid" />
        </td>
    </tr>    
    <tr>
        <td>Planos:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkPlano" runat="server" RepeatColumns="3" Width="1000px" BorderStyle="Solid" DataValueField="Codigo" DataTextField="Descricao"/>
        </td>
    </tr>
	<tr>
        <td>Grau de Parentesco:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkParentesco" runat="server" DataValueField="Key" DataTextField="Value" RepeatColumns="3" Width="1000px" BorderStyle="Solid" />
        </td>
    </tr>
	<tr>
        <td>Deficiente Físico:</td>
        <td colspan="3">
            <asp:DropDownList ID="dpdDeficienteFisico" runat="server">
				<asp:ListItem Value="" Text="TODOS" />
				<asp:ListItem Value="S" Text="SIM" />
				<asp:ListItem Value="N" Text="NÃO" />
            </asp:DropDownList>
        </td>
    </tr>
	<tr>
        <td>Estudante:</td>
        <td colspan="3">
            <asp:DropDownList ID="dpdEstudante" runat="server">
				<asp:ListItem Value="" Text="TODOS" />
				<asp:ListItem Value="S" Text="SIM" />
				<asp:ListItem Value="N" Text="NÃO" />
            </asp:DropDownList>
        </td>
    </tr>
	<tr>		
		<td colspan="4" style="text-align:center">
			<asp:UpdatePanel ID="pnlBotao" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
					<asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
    </table>
    <table>
	<tr style="height:300px">
		<td colspan="4">
			<asp:UpdatePanel ID="pnlGrid" runat="server" UpdateMode="Conditional">
				<ContentTemplate>

            <asp:Label ID="lblCount" runat="server" Visible="false" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="1600px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
                    <asp:BoundField HeaderText="CARTEIRINHA" DataField="BA1_MATANT" />
                    <asp:BoundField HeaderText="NOME TITULAR" DataField="nm_titular" />
                    <asp:BoundField HeaderText="TIPO" DataField="BA1_TIPUSU" />
                    <asp:BoundField HeaderText="NOME BENEFICIARIO" DataField="nm_beneficiario" />
                    <asp:BoundField HeaderText="SEXO" DataField="BA1_SEXO" />
                    <asp:BoundField HeaderText="DATA NASCIMENTO" DataField="BA1_DATNAS" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="IDADE" DataField="idade" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="FAIXA ETÁRIA" DataField="faixa_etaria" />
                    <asp:BoundField HeaderText="DATA INICIO VIGÊNCIA" DataField="dt_inicio_vigencia" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="DATA FIM VIGÊNCIA" DataField="dt_termino_vigencia" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="MUNICÍPIO" DataField="BA1_MUNICI" />
                    <asp:BoundField HeaderText="UF" DataField="BA1_ESTADO" />
                    <asp:BoundField HeaderText="REGIONAL" DataField="BIB_DESCRI" />
                    <asp:BoundField HeaderText="PLANO" DataField="BI3_DESCRI" />
                    <asp:BoundField HeaderText="PARENTESCO" DataField="ds_parentesco" />
                    <asp:BoundField HeaderText="ESTUDANTE" DataField="fl_estudante" />
                    <asp:BoundField HeaderText="DEFICIENTE FÍSICO" DataField="fl_deficiente_fisico" />
                    <asp:BoundField HeaderText="0 A 18 anos" DataField="qtd_0_18" Visible="false" />
                    <asp:BoundField HeaderText="19 A 23 anos" DataField="qtd_19_23" Visible="false" />
                    <asp:BoundField HeaderText="24 A 28 anos" DataField="qtd_24_28" Visible="false" />
                    <asp:BoundField HeaderText="29 A 33 anos" DataField="qtd_29_33" Visible="false" />
                    <asp:BoundField HeaderText="34 A 38 anos" DataField="qtd_34_38" Visible="false" />
                    <asp:BoundField HeaderText="39 A 43 anos" DataField="qtd_39_43" Visible="false" />
                    <asp:BoundField HeaderText="44 A 48 anos" DataField="qtd_44_48" Visible="false" />
                    <asp:BoundField HeaderText="49 A 53 anos" DataField="qtd_49_53" Visible="false" />
                    <asp:BoundField HeaderText="54 A 58 anos" DataField="qtd_54_58" Visible="false" />
                    <asp:BoundField HeaderText="59+" DataField="qtd_59" Visible="false" />
                    <asp:BoundField HeaderText="TOTAL MASC" DataField="qtd_masc" Visible="false" />
                    <asp:BoundField HeaderText="TOTAL FEM" DataField="qtd_fem" Visible="false" />
                    <asp:BoundField HeaderText="TOTAL" DataField="qtd_total" Visible="false" />
				</Columns>
			</asp:GridView>
					</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>

</table>
</asp:Content>
