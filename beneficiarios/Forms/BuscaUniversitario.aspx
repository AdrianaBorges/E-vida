<%@ Page Title="Declaração de Universitário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaUniversitario.aspx.cs" Inherits="eVidaBeneficiarios.Forms.BuscaUniversitario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openPdf(id) {
        	openReport(RELATORIO_UNIVERSITARIO,	"ID=" + id);
        	return false;
        }

        function openDownload(id) {
        	openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.DECLARACAO_UNIVERSITARIO %>', "ID=" + id);
        	return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

<table style="text-align:center">
    <tr>
        <td>Situação:</td>
        <td><asp:DropDownList ID="dpdFiltroSituacao" runat="server" Width="200px">
                <asp:ListItem Value="" Text="Todos" />
                <asp:ListItem Value="1" Text="Pendente" />
                <asp:ListItem Value="2" Text="Processado" />
                <asp:ListItem Value="3" Text="Recusado" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td colspan="2"><br />
			<asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" OnClick="btnPesquisar_Click" />
            <asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClick="btnNovo_Click" />
            <br /><br /></td>
    </tr>
</table>
<table>
	<tr>
		<td colspan="2">            
            <asp:Label ID="lblCount" runat="server" /><br /><br />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="cd_solicitacao"
				AllowSorting="false" CssClass="tabela" Width="650px" OnRowDataBound="gdvRelatorio_RowDataBound">
                <Columns>
					<asp:BoundField HeaderText="Declaracao" DataField="cd_solicitacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Dependente" DataField="BA1_NOMUSR" SortExpression="BA1_NOMUSR" />
					<asp:BoundField HeaderText="Situação" DataField="CD_STATUS" SortExpression="CD_STATUS" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' ToolTip="Formulário" />
							<asp:ImageButton ID="btnArquivo" runat="server" ImageUrl="~/img/download24.png" Height="24px" OnClientClick='<%# "return openDownload("+ Eval("cd_solicitacao") + ");" %>' ToolTip="Arquivo anexado" />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
