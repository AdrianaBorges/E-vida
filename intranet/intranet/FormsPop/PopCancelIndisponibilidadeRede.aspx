<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopCancelIndisponibilidadeRede.aspx.cs" Inherits="eVidaIntranet.Forms.PopCancelIndisponibilidadeRede" %>
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
            <td>Deseja realmente encerrar o formulário de protocolo <asp:Literal ID="litProtocolo" runat="server" /> ?<br /></td>
        </tr>
        <tr>
            <td>Informe o motivo de encerramento abaixo:</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtCancelamento" runat="server" Width="450px" TextMode="SingleLine" /></td>
        </tr>
        <tr>
            <td>Informe a procedência:</td>
        </tr>
        <tr>
		    <td>
                <asp:DropDownList ID="dpdProcedencia" AutoPostBack="true" OnSelectedIndexChanged="dpdProcedencia_change" runat="server" Width="200px">
                <asp:ListItem Value="" Text="" />
                <asp:ListItem Value="P" Text="PROCEDENTE" />
			    <asp:ListItem Value="I" Text="IMPROCEDENTE" />
                </asp:DropDownList>
		    </td>
        </tr>
        <tr>
            <td><asp:Label id="lblDataAtendimento" Visible="false" runat="server">Informe a Data de Atendimento:</asp:Label></td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtDataAtendimento" Visible="false" runat="server" Width="190px" CssClass="calendario" AutoPostBack="true" OnTextChanged="data_TextChanged" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnSalvar" runat="server" Text="Encerrar formulário" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>
