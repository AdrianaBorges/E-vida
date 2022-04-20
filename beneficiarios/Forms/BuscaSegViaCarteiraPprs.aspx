<%@ Page Title="Solicitações de 2ª Via" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaSegViaCarteiraPprs.aspx.cs" Inherits="eVidaBeneficiarios.Forms.BuscaSegViaCarteiraPprs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function openPdf(id) {
            openReport(RELATORIO_SEGUNDA_VIA, "ID=" + id);
            return false;
        }

        function openDownload(idSegViaCarteira, fId, isNew) {

            if (isNew) {
                alert('Arquivos Novos não podem ser visualizados. O formulário deve ser salvo para disponibilizar a visualização!');
                return false;
            }
            openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.BOLETIM_OCORRENCIA %>', "ID=" + idSegViaCarteira + ";" + fId);

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
					    <asp:BoundField HeaderText="Número da Solicitação" DataField="cd_solicitacao" SortExpression="cd_solicitacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" ItemStyle-HorizontalAlign="Right" />
					    <asp:BoundField HeaderText="Data Criação" DataField="dt_criacao" SortExpression="dt_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
					    <asp:BoundField HeaderText="Situação" DataField="TP_STATUS" SortExpression="TP_STATUS" />
                        <asp:BoundField HeaderText="Data Alteração" DataField="DT_ALTERACAO" SortExpression="DT_ALTERACAO" DataFormatString="{0:dd/MM/yyyy}"/>
                        <asp:TemplateField HeaderText="Boletim de Ocorrência" ItemStyle-HorizontalAlign="Center" >
                            <ItemTemplate>
                                <asp:ImageButton ID="btnArquivo" runat="server" ImageUrl="~/img/anexo.png" Height="30px" />
                            </ItemTemplate>
                        </asp:TemplateField>
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
