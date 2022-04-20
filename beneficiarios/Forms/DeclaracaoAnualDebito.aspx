<%@ Page Title="Declaração Anual de Débitos" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="DeclaracaoAnualDebito.aspx.cs" Inherits="eVidaBeneficiarios.Forms.DeclaracaoAnualDebito" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

<table style="text-align:center">
    <tr>
        <td colspan="2"><br />
			<asp:Button ID="btnGerar" runat="server" Text="Enviar" OnClick="btnGerar_Click" />
            <br /><br /></td>
    </tr>
</table>
</asp:Content>
