<%@ Page Title="Acesso Negado" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Negado.aspx.cs" Inherits="eVidaIntranet.Internal.Negado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	Você não possui permissão de acesso ao módulo: <h2><asp:Literal ID="ltModulo" runat="server" /></h2>
</asp:Content>
