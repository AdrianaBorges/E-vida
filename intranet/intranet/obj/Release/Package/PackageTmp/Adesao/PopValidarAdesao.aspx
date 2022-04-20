<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopValidarAdesao.aspx.cs" Inherits="eVidaIntranet.Adesao.PopValidarAdesao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setDone(id) {
            parent.locatorCallback(id);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="500px">
        <tr>
            <td>Deseja validar/invalidar a proposta de adesão <asp:Literal ID="litProtocolo" runat="server" /> ?<br /></td>
        </tr>
        <tr>
            <td><asp:DropDownList ID="dpdValidacao" runat="server">
                <asp:ListItem Text="SELECIONE" Value="" />
                <asp:ListItem Text="VALIDAR" Value="S" />
                <asp:ListItem Text="INVALIDAR" Value="N" />
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td>Informe uma observação referente à validação:</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtMotivo" runat="server" Width="450px" TextMode="SingleLine" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Validar proposta" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>