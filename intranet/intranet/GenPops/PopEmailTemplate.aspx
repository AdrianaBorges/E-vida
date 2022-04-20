<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopEmailTemplate.aspx.cs" Inherits="eVidaIntranet.GenPops.PopEmailTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function updVisualizacao(id, str) {
            $("#" + id).contents().find("html").html(str);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="100%">
        <tr>
            <td> Selecione o template:
                <asp:DropDownList ID="dpdTemplate" runat="server" DataValueField="Id" DataTextField="Nome" /></td>
        </tr>
        <tr>
            <td>
                 <asp:MultiView ID="mtvTemplate" runat="server">
                    <asp:View ID="vwGenerico" runat="server">
                        <b>Mensagem:</b><br />
                        <asp:TextBox ID="txtMsgGenerico" runat="server" TextMode="MultiLine" Width="100%" Height="300px" />
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr>
            <td align="center">
                    <asp:Button ID="btnVisualizar" runat="server" Text="Visualizar" OnClick="btnVisualizar_Click" />
    <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnEnviar_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlVisualizacao" runat="server" Visible ="false">
                    Visualização:
                    <iframe id="frmVisualizacao" runat="server" width="100%" height="400px" />
                </asp:Panel>
            </td>
        </tr>
    </table>
   
    <br />
   

    


</asp:Content>
