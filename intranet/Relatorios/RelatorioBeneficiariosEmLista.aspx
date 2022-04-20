<%@ Page Title="BENEFICIÁRIOS EM LISTA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioBeneficiariosEmLista.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioBeneficiariosEmLista" %>
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
				AllowSorting="false" CssClass="tabela" Width="1100px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
                    <asp:BoundField HeaderText="NOME BENEFICIARIO" DataField="BA1_NOMUSR" ItemStyle-Width="50%" />
                    <asp:BoundField HeaderText="EMAIL BENEFICIARIO" DataField="BA1_EMAIL" ItemStyle-Width="50%" />
				</Columns>
			</asp:GridView>
					</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>

</table>
</asp:Content>
