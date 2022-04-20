<%@ Page Title="Relatório de Autorizações de Reciprocidade" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioReciprocidade.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioReciprocidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function ConfigControlsCustom() {
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

        }

        function openPdf(id) {
            try {
                openReport(RELATORIO_ENVIO_RECIPROCIDADE, 'ID=' + id);
            } catch (e) {
                alert(e.message);
            }
            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
<table width="880px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:120px; text-align: right">Período:</td>
		<td align="left">de <asp:TextBox ID="txtInicio" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  /> até 
                <asp:TextBox ID="txtFim" runat="server" Width="80px" MaxLength="10" CssClass="calendario" /></td>
        <td style="width:160px; text-align: right">Empresa Conveniada:</td>
        <td><asp:DropDownList ID="dpdEmpresa" runat="server" Width="100%" DataTextField="Nomint" DataValueField="Codint" /></td>
	</tr>
    <tr>
        <td style="text-align: right">Matrícula:</td>
        <td><asp:TextBox ID="txtMatricula" runat="server" Width="150px" MaxLength="10"/></td>
        <td style="width:120px; text-align: right">Nome Titular:</td>
        <td><asp:TextBox ID="txtTitular" runat="server" Width="300px" MaxLength="255"/></td>
    </tr>
    <tr>
		<td style="text-align: right">Dependente:</td>
        <td><asp:TextBox ID="txtDependente" runat="server" Width="300px" MaxLength="255"/></td>
		<td style="text-align: right">Situação:</td>
        <td><asp:DropDownList ID="dpdSituacao" runat="server">
            <asp:ListItem Value="" Text="Todos" />
            <asp:ListItem Value="1" Text="ENVIADO" />
            <asp:ListItem Value="2" Text="APROVADO" />
            <asp:ListItem Value="3" Text="NEGADO" />
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td colspan="4" style="text-align:center">
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />                
		</td>        
	</tr>
    </table>


    <table>
	<tr style="height:300px">
		<td>
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="1400px" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" 
                onpageindexchanging="gdvRelatorio_PageIndexChanging" OnSorting="gdvRelatorio_Sorting" OnRowDataBound="gdvRelatorio_RowDataBound" >
                <Columns>
					<asp:BoundField HeaderText="Nº Autorização" DataField="cd_solicitacao" SortExpression="cd_solicitacao" DataFormatString="{0:000000000}" />
					<asp:BoundField HeaderText="Matrícula" DataField="BA3_MATEMP" SortExpression="BA3_MATEMP" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Nome Titular" DataField="BA1_NOMUSR" SortExpression="BA1_NOMUSR" />
					<asp:BoundField HeaderText="Empresa Conveniada" DataField="BA0_NOMINT" SortExpression="BA0_NOMINT" />
                    <asp:BoundField HeaderText="Início" DataField="dt_inicio" SortExpression="dt_inicio" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Término" DataField="dt_fim" SortExpression="dt_fim" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Usuário Situação" DataField="nm_usuario_situacao" SortExpression="nm_usuario_situacao" />
                    <asp:BoundField HeaderText="Situação" DataField="cd_status" SortExpression="cd_status" />
                    <asp:BoundField HeaderText="Data Criação" DataField="dt_criacao" SortExpression="dt_criacao" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Usuário Envio" DataField="nm_usuario_envio" SortExpression="nm_usuario_envio" />
                    <asp:BoundField HeaderText="Data Envio" DataField="dt_envio" SortExpression="dt_envio" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Usuário Aprovação" DataField="nm_usuario_aprovacao" SortExpression="nm_usuario_aprovacao" />
                    <asp:BoundField HeaderText="Data Aprovação" DataField="dt_aprovacao" SortExpression="dt_aprovacao" DataFormatString="{0:dd/MM/yyyy}" />
				    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Gerar PDF" ToolTip="Gerar PDF" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
