<%@ Page Title="CREDENCIADOS EM LISTA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioCredenciadosEmLista.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioCredenciadosEmLista" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="950px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td>UF:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkUf" runat="server" DataValueField="sigla" DataTextField="nome" RepeatColumns="3" Width="600px" BorderStyle="Solid" />
        </td>
    </tr>
    <!-- Estes filtros foram retirados porque não foram encontrados no Protheus -->
    <!--tr>
        <td>Natureza:</td>
        <td colspan="3">
            <asp:DropDownList ID="dpdNatureza" runat="server" DataValueField="CdNatureza" DataTextField="DsNatureza" />
        </td>
    </tr-->
	<!--tr>
        <td>Sistema de Atendimento:</td>
        <td colspan="3">
            <asp:DropDownList ID="dpdSistema" runat="server">
                <asp:ListItem Text="Todos" Value="" />
                <asp:ListItem Value="CRED" Text="CREDENCIAMENTO" />
                <asp:ListItem Value="REEMB" Text="REEMBOLSO" />
            </asp:DropDownList>
        </td>
    </tr-->
	<tr>
        <td>Situação:</td>
        <td colspan="3">
            <asp:DropDownList ID="dpdStatus" runat="server">
                <asp:ListItem Text="Todas" Value="" />
                <asp:ListItem Value="A" Text="ATIVO" />
                <asp:ListItem Value="I" Text="INATIVO" />
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

    <h2>IMPORTANTE</h2>
	<div class="observacao" style="background-color:red; color:white">
		<p>Para exportar o arquivo, desabilite o bloqueador de pop-ups para esta página.</p>
	</div>

    <table>
	<tr style="height:300px">
		<td colspan="4">
			<asp:UpdatePanel ID="pnlGrid" runat="server" UpdateMode="Conditional">
				<ContentTemplate>

            <asp:Label ID="lblCount" runat="server" Visible="false" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" OnRowDataBound="gdvRelatorio_RowDataBound"
				AllowSorting="false" CssClass="tabela" Width="975px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
                    <asp:BoundField HeaderText="RAZÃO SOCIAL" DataField="BAU_NOME" ItemStyle-Width="50%" />
                    <asp:BoundField HeaderText="EMAIL DO CREDENCIADO" DataField="BAU_EMAIL" ItemStyle-Width="50%" />
				</Columns>
			</asp:GridView>
					</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>

</table>
</asp:Content>
