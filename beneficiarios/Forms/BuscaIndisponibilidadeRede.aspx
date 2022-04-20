<%@ Page Title="Indisponibilidade de Rede" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaIndisponibilidadeRede.aspx.cs" Inherits="eVidaBeneficiarios.Forms.BuscaIndisponibilidadeRede" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openEdit(id) {
            window.location = './IndisponibilidadeRede.aspx?ID=' + id;
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
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_INDISPONIBILIDADE"
				AllowSorting="false" CssClass="tabela" Width="1150px" OnRowDataBound="gdvRelatorio_RowDataBound">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="CD_INDISPONIBILIDADE" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Beneficiário" DataField="BA1_NOMUSR" />
                    <asp:BoundField HeaderText="Especialidade" DataField="NM_ESPECIALIDADE" />
                    <asp:BoundField HeaderText="Prioridade" DataField="NR_PRIORIDADE" />
					<asp:BoundField HeaderText="Data Criação" DataField="DT_SOLICITACAO" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />		
                    <asp:BoundField HeaderText="Situação" DataField="ID_SITUACAO" />
                    <asp:TemplateField>
						<ItemStyle Width="60px" />
                        <ItemTemplate>
							<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# "return openEdit("+ Eval("CD_INDISPONIBILIDADE") + ");" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>           
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
