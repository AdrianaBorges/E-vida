<%@ Page Title="Reciprocidades Solicitadas" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaReciprocidade.aspx.cs" Inherits="eVidaBeneficiarios.Forms.BuscaReciprocidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openView(id) {
            window.location = './Reciprocidade.aspx?ID=' + id;
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
				AllowSorting="false" CssClass="tabela" Width="750px" OnRowDataBound="gdvRelatorio_RowDataBound">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="cd_solicitacao" SortExpression="cd_solicitacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Data Criação" DataField="dt_criacao" SortExpression="dt_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
					
                    <asp:BoundField HeaderText="Município" DataField="ds_cidade" SortExpression="ds_cidade" />
                    <asp:BoundField HeaderText="Situação" DataField="cd_status" SortExpression="cd_status" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/lupa.gif" Height="30px" OnClientClick='<%# "return openView("+ Eval("cd_solicitacao") + ");" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>

