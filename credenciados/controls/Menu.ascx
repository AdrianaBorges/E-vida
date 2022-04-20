<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="eVidaCredenciados.controls.Menu" %>
<%@ Register TagPrefix="evida" Assembly="eVidaWebLib" Namespace="eVida.Web.Controls" %>
<div id="divMenu">
<ul id="menu" class="menu">
    <li><asp:HyperLink ID="btnInicio" runat="server" CausesValidation="false" Text="Inicial" NavigateUrl="~/Internal/Inicial.aspx"/></li>
	<evida:MenuItem ID="menuFrm" runat="server" Label="Formulários" Enabled="true" >
        <Items>
            <evida:MenuItem ID="mnFrmAutorizacao" runat="server" Url="~/Forms/BuscaAutorizacao.aspx" Label="Autorizações" Visible="false" />
        </Items>
    </evida:MenuItem>
    <evida:MenuItem ID="menuIr" runat="server" Label="Imposto de Renda" Enabled="true">
        <Items>
            <evida:MenuItem ID="menuIr1" runat="server" Url="~/IR/Comprovantes.aspx" Label="Comprovantes" />
        </Items>
    </evida:MenuItem>
    <li><asp:LinkButton ID="btnSair" runat="server" CausesValidation="false" Text="Sair" onclick="btnSair_Click" /></li>
</ul>
</div>
