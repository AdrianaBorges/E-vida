<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopAutorizacaoRevalidar.aspx.cs" Inherits="eVidaBeneficiarios.Forms.PopAutorizacaoRevalidar" %>
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
            <td>Deseja realmente solicitar revalidação da solicitação <asp:Literal ID="litProtocolo" runat="server" /> ?<br /></td>
        </tr>
        <tr>
            <td>
                <table id="tbDataProc" runat="server" visible="false">
                    <tr>
                        <td colspan="2">Esta autorização é associada a um procedimento de internação. <br />
                            Para a revalidação é necessário informar a nova data de internação.<br /><br /></td>
                    </tr>
                    <tr>
                        <td><b>Data de Internação:</b></td>
                        <td><asp:TextBox ID="txtDataInternacao" runat="server" CssClass="calendario" MaxLength="10" Width="120px" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Revalidar" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>