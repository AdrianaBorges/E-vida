<%@ Page Title="Endereço de Cobrança" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormEnderecoCobranca.aspx.cs" Inherits="eVidaIntranet.Admin.FormEnderecoCobranca" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<table width="850px" cellspacing="10px">
    <tr>
        <td style="width:200px; text-align: left">CPF:</td>
        <td colspan="3" style="width:200px; text-align: left">Nome:</td>
    </tr>
    <tr>
        <td><asp:Literal ID="litCpf" runat="server"/></td>
        <td colspan="3"><asp:Literal ID="litNome" runat="server"/></td>
    </tr>
    <tr>
        <td style="width:200px; text-align: left">Endereço:</td>
        <td style="width:200px; text-align: left">Número:</td>
        <td style="width:200px; text-align: left">Complemento:</td>
        <td style="width:200px; text-align: left">Bairro:</td>
    </tr>
    <tr>
        <td><asp:Literal ID="litEndereco" runat="server"/></td>
        <td><asp:Literal ID="litNumero" runat="server"/></td>
        <td><asp:Literal ID="litComplemento" runat="server"/></td>
        <td><asp:Literal ID="litBairro" runat="server"/></td>
    </tr>
    <tr>
	    <td style="width:200px; text-align: left">Endereço de Cobrança:</td>
    </tr>
    <tr>
	    <td colspan="3" align="left"><asp:TextBox ID="txtEndCob" runat="server" Width="640px" /></td>
	    <td style="text-align:left"><asp:Button ID="btnGerar" runat="server" Text="Gerar" OnClick="btnGerar_Click" /></td>
    </tr>
    <tr>
	    <td colspan="8" style="text-align:center">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
        </td>
    </tr>
</table>
</asp:Content>
