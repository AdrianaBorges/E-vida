<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopCancelSegViaCarteira.aspx.cs" Inherits="eVidaIntranet.Forms.PopCancelSegViaCarteira" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">
            function setCancelamento(id) {
                parent.locatorCallback(id);
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="500px">
        <tr>
            <td>Deseja realmente cancelar a solicitação de protocolo <asp:Literal ID="litProtocolo" runat="server" /> ?<br /></td>
        </tr>
        <tr>
            <td>Informe o motivo de cancelamento abaixo:</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtCancelamento" runat="server" Width="450px" TextMode="SingleLine" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Cancelar solicitação" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>
