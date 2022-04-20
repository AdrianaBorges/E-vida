<%@ Page Title="Princípios ativos" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopPrincipioAtivo.aspx.cs" Inherits="eVidaIntranet.GenPops.PopPrincipioAtivo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setLocatorValue(id) {
        	parent.locatorCallback(id);
        }

        function openExcluir(obj, id, row) {
            return confirm("Deseja realmente excluir este princípio?");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="90%">
        <tr>
            <td><asp:Label ID="lblCodigo" runat="server" Text="Código:" /></td>
            <td><asp:TextBox ID="txtCodigo" runat="server" Width="250px" /></td>
            <td rowspan="2"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblDescricao" runat="server" Text="Descrição:" /></td>
            <td><asp:TextBox ID="txtDescricao" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td colspan="3">
                <hr />
                <asp:GridView ID="gdv" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="cd_principio_ativo" AutoGenerateColumns="false"
                    OnRowCommand="gdv_RowCommand" OnRowDataBound="gdv_RowDataBound">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Selecionar" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="cd_principio_ativo" HeaderText="CÓDIGO" />
                        <asp:BoundField DataField="ds_principio_ativo" HeaderText="DESCRIÇÃO" />
                        <asp:TemplateField>
                            <ItemTemplate>
                            <asp:LinkButton  ID="btnExcluir" runat="server" CommandName="CmdExcluir" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openExcluir") %>'>
                                <asp:Image ID="imgExcluir"  runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Excluir princípio" ToolTip="Excluir princípio" />
                            </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <table id="tbCadastro" runat="server" width="100%" visible="false">
                    <tr>
                        <td colspan="2"><h2>Criar Novo</h2></td>
                    </tr>
                    <tr>
                        <td><b>Descrição</b></td>
                        <td><asp:TextBox ID="txtDescricaoCadastro" runat="server" /></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnSalvar" runat="server" OnClick="btnSalvar_Click" Text="Criar novo principio" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>