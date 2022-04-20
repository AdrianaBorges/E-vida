<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopUsuario.aspx.cs" Inherits="eVidaIntranet.GenPops.PopUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
        }
        function setUsuario(id) {
            if (parent.$locatorConfig) {
                parent.locatorCallback(id);
            } else {
                alert('erro de config do locator!');
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="450px">
        <tr>
            <td><asp:Label ID="lblLogin" runat="server" Text="Login:" /></td>
            <td><asp:TextBox ID="txtLogin" runat="server" Width="250px" /></td>
            <td><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <asp:GridView ID="gdvUsuario" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="ID_USUARIO" AutoGenerateColumns="false"
                    OnRowCommand="gdvUsuario_RowCommand">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Selecionar" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="cd_usuario" HeaderText="USUÁRIO" />
                        <asp:BoundField DataField="nm_usuario" HeaderText="NOME" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
