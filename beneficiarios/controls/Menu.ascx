<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="eVidaBeneficiarios.controls.Menu" %>

<%@ Register TagPrefix="evida" Assembly="eVidaWebLib" Namespace="eVida.Web.Controls" %>
<div id="divMenu">
<ul id="menu" class="menu">
    <li><asp:HyperLink ID="btnInicio" runat="server" CausesValidation="false" Text="Inicial" NavigateUrl="~/Internal/Inicial.aspx"/></li>
    <evida:MenuItem ID="menuPessoal" runat="server" Label="Pessoal">
        <Items>
            <evida:MenuItem ID="mnPessoal1" runat="server" Url="~/Forms/DadosPessoais.aspx" Label="Dados Pessoais" />
        </Items>
    </evida:MenuItem>
    <evida:MenuItem ID="menuIr" runat="server" Label="Imposto de Renda">
        <Items>
            <evida:MenuItem ID="menuIr1" runat="server" Url="~/IR/ExtratoMensalidadeTable.aspx" Label="Mensalidades e Co-participações" />
			<evida:MenuItem ID="menuIr2" runat="server" Url="~/IR/ExtratoReembolsoTable.aspx" Label="Extrato de Reembolsos" />
        </Items>
    </evida:MenuItem>
    <evida:MenuItem ID="menuForms" runat="server" Label="Formulários">
        <Items>
            <evida:MenuItem ID="mnForms1" runat="server" Url="~/Forms/BuscaSegViaCarteiraPprs.aspx" Label="2ª Via de Carteira" />
			<evida:MenuItem ID="mnForms5" runat="server" Url="~/Forms/BuscaAutorizacao.aspx" Label="Autorizações" />
			<evida:MenuItem ID="mnForms4" runat="server" Url="~/Forms/BuscaUniversitario.aspx" Label="Declaração de Universitários" />
			<evida:MenuItem ID="mnForms2" runat="server" Url="~/Forms/BuscaExclusao.aspx" Label="Exclusão de Beneficiários" />
            <evida:MenuItem ID="mnForms3" runat="server" Url="~/Forms/BuscaReciprocidade.aspx" Label="Inclusão na rede de reciprocidade " />
            <evida:MenuItem ID="MenuItem1" runat="server" Url="~/Forms/BuscaIndisponibilidadeRede.aspx" Label="Indisponibilidade de rede" />
        </Items>
    </evida:MenuItem>
    <%--
    <evida:MenuItem ID="menuExtratoReembolos" runat="server" Label="Reembolso e Extrato">
        <Items>
            <evida:MenuItem ID="mnGalenus" runat="server" Label="Galenus" />
            <evida:MenuItem ID="mnDecAnual" runat="server" Url="~/Forms/DeclaracaoAnualDebito.aspx" Label="Declaração Anual de Débitos" />
        </Items>
    </evida:MenuItem>
    --%>
    <evida:MenuItem ID="menuRelatorios" runat="server" Label="Relatórios" Visible="false">
        <Items>
            <evida:MenuItem ID="mnRelatorio1" runat="server" Url="~/Relatorios/RelatorioProvisao.aspx" Label="PROVISAO" />
            <evida:MenuItem ID="mnRelatorio2" runat="server" Url="~/Relatorios/RelatorioPagamento.aspx" Label="PAGAMENTO" />
            <evida:MenuItem ID="mnRelatorio3" runat="server" Url="~/Relatorios/RelatorioContabilizacao.aspx" Label="CONTABILIZACAO E-VIDA/ELN" />
        </Items>
   </evida:MenuItem>
    <li><asp:LinkButton ID="btnSair" runat="server" CausesValidation="false" Text="Sair" onclick="btnSair_Click" /></li>
</ul>
</div>