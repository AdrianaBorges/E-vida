<%@ Page Title="Empresas Conveniadas para Reciprocidade" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="GerenciarEmpresaReciprocidade.aspx.cs" Inherits="eVidaIntranet.Admin.GerenciarEmpresaReciprocidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function openEdit(id) {
            window.location = "EditarEmpresaReciprocidade.aspx?id=" + id;
        }

        function goNova() {
            window.location = "EditarEmpresaReciprocidade.aspx";
            return false;
        }

        function confirmExclusao(nome) {
            return confirm("Deseja realmente excluir a empresa " + nome + "?");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="100%">
        <tr>
            <td align="center"><asp:Button ID="btnNova" runat="server" OnClientClick="return goNova()" Text="Incluir" /></td>
        </tr>
	    <tr style="height:300px; vertical-align: top">
		    <td>
			    <asp:GridView ID="gdvEmpresas" runat="server" AutoGenerateColumns="false" DataKeyNames="Codigo"
				    AllowSorting="false" CssClass="tabela" Width="100%" EmptyDataText="Nenhum registro encontrado" HorizontalAlign="Center"
                    OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
					ItemType="eVidaGeneralLib.VO.EmpresaReciprocidadeVO">
				    <Columns>
					    <asp:BoundField HeaderText="Código" DataField="Codigo" />
					    <asp:BoundField HeaderText="Empresa" DataField="Nome" />
					    <asp:BoundField HeaderText="Área de Atuação" />
                        <asp:TemplateField>
                            <ItemTemplate>                                                               
                                <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# "return openEdit("+ Item.Codigo + ");" %>' AlternateText="Editar" ToolTip="Editar" />
                                &nbsp;
                                <asp:ImageButton ID="bntExcluir" runat="server" ImageUrl="~/img/remove.png" Height="20px" CommandName="Excluir" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"  AlternateText="Excluir" ToolTip="Excluir"
                                    OnClientClick='<%# "return confirmExclusao(" + (char)39 + HttpUtility.JavaScriptStringEncode(Item.Nome) + (char)39 +");" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
				    </Columns>
			    </asp:GridView>
		    </td>
	    </tr>
    </table>
</asp:Content>
