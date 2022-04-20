<%@ Page Title="SOLICITAÇÃO DE EXCLUSÃO DE BENEFICIÁRIOS" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaExclusao.aspx.cs" Inherits="eVidaBeneficiarios.Forms.BuscaExclusao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openView(id) {
            window.location = './Exclusao.aspx?ID=' + id;
            return false;
        }
        function openPdf(id) {
        	openReport(RELATORIO_EXCLUSAO, "ID=" + id);
        	return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    <table style="text-align:center">
	<tr style="height:300px">
		<td colspan="4">
            <asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClick="btnNovo_Click" /><br /><br />
            <asp:Label ID="lblCount" runat="server" /><br /><br />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="cd_solicitacao"
				AllowSorting="false" CssClass="tabela" Width="650px" OnRowDataBound="gdvRelatorio_RowDataBound">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="cd_solicitacao" SortExpression="cd_solicitacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Beneficiários" DataField="ds_beneficiario" SortExpression="ds_beneficiario" />
					<asp:BoundField HeaderText="Data Criação" DataField="dt_criacao" SortExpression="dt_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />					
					<asp:BoundField HeaderText="Data Alteracao" DataField="dt_alteracao" SortExpression="dt_alteracao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText="Situação" DataField="cd_status" SortExpression="cd_status" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>

