<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/ExternalPopUp.Master" AutoEventWireup="true" CodeBehind="PopSolEsclarecimento.aspx.cs" Inherits="eVidaBeneficiarios.CanalGestante.PopSolEsclarecimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var isMsgSalvaOK = false;

        function ClosePopUpCustom(ev, ui, dlg) {
            if (isMsgSalvaOK) {
                setCallback('');
            }
        }
        function setCallback(id) {
            parent.locatorCallback(id);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="90%">
        <tr>
            <td><b>Protocolo:  <asp:Literal ID="litProtocolo" runat="server" /></b><br /><br /></td>
        </tr>
        <tr>
            <td><b>O arquivo de solicitação foi gerado com sucesso! Envie o formulário caso deseje mais esclarecimentos.</b><br /><br /></td>
        </tr>
        <tr>
            <td><b>Informe o canal de resposta desejado</b><br />
                <asp:DropDownList ID="dpdCanalResposta" runat="server" Width="300px" /></td>
        </tr>
        <tr>
            <td><b>Informe sua dúvida:</b><br />
                <asp:TextBox ID="txtDuvida" runat="server" Width="500px" Rows="5" MaxLength="200" TextMode="MultiLine" /></td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnEnviar_Click" Visible="false" />
            </td>
        </tr>
    </table>
</asp:Content>
