<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopFinalizarCanalGestante.aspx.cs" Inherits="eVidaIntranet.Forms.PopFinalizarCanalGestante" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">
            function setCallback(id) {
            	parent.locatorCallback(id);
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="500px">
        <tr>
            <td>Deseja realmente finalizar a solicitação <asp:Literal ID="litProtocolo" runat="server" /> ?<br /></td>
        </tr>
        <tr>
            <td>Informe os procedimentos realizados para atender à solicitação:</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtDescricao" runat="server" Width="450px" TextMode="MultiLine" Height="100px" Rows="6" MaxLength="500" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Finalizar" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>