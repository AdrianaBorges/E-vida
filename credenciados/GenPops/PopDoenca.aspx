<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopDoenca.aspx.cs" Inherits="eVidaCredenciados.GenPops.PopDoenca" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setDoenca(id) {
        	parent.locatorCallback(id);
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
                <asp:GridView ID="gdv" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="BA9_CODDOE" AutoGenerateColumns="false"
                    OnRowCommand="gdv_RowCommand">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Selecionar" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="BA9_CODDOE" HeaderText="CÓDIGO" />
                        <asp:BoundField DataField="BA9_DOENCA" HeaderText="DESCRIÇÃO" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
