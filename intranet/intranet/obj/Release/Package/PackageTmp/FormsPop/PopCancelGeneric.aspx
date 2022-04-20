<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopCancelGeneric.aspx.cs" Inherits="eVidaIntranet.FormsPop.PopCancelGeneric" %>
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
            <td><h2><asp:Literal ID="litTitle" runat="server" /></h2><br />
                <asp:Literal ID="litQuestion" runat="server" /> ?<br /></td>
        </tr>
        <tr>
            <td><asp:Literal ID="litPrompt" runat="server" /></td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtCancelamento" runat="server" Width="450px" TextMode="SingleLine" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Cancelar solicitação" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>
