<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopAutorizacaoProvChanger.aspx.cs" Inherits="eVidaIntranet.Forms.PopAutorizacaoProvChanger" %>
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
            <td>Deseja realmente cancelar o formulário de protocolo <asp:Literal ID="litProtocolo" runat="server" /> ?<br /></td>
        </tr>
        <tr>
            <td>Informe o motivo de cancelamento abaixo:</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtCancelamento" runat="server" Width="450px" TextMode="SingleLine" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Cancelar formulário" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>

