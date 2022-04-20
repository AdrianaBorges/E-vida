<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopMotivoGlosa.aspx.cs" Inherits="eVidaIntranet.GenPops.PopMotivoGlosa" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setReturn(id) {
            parent.locatorCallback(id);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="94%">
        <tr>
            <td><asp:Label ID="lblCodigo" runat="server" Text="Código:" /></td>
            <td><asp:TextBox ID="txtCodigo" runat="server" Width="250px" /></td>
            <td rowspan="3"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td>Grupo:</td>
            <td><asp:DropDownList ID="dpdGrupo" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblDescricao" runat="server" Text="Descrição:" /></td>
            <td><asp:TextBox ID="txtDescricao" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:GridView ID="gdv" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="Id" AutoGenerateColumns="false"
                    OnRowCommand="gdv_RowCommand">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Sel." CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="Id" HeaderText="CÓDIGO" />
                        <asp:BoundField DataField="Grupo" HeaderText="GRUPO" />
                        <asp:BoundField DataField="Descricao" HeaderText="DESCRIÇÃO" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
