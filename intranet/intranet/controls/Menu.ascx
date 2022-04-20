<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="eVidaIntranet.controls.Menu" %>

<%@ Register TagPrefix="evida" Assembly="eVidaWebLib" Namespace="eVida.Web.Controls" %>
<div id="divMenu">
<ul id="menu" class="menu">
    <li><asp:HyperLink ID="btnInicio" runat="server" CausesValidation="false" Text="Inicial" NavigateUrl="~/Internal/Inicial.aspx"/></li>
    
	<evida:MenuItem ID="menuAdesao" runat="server" Label="Adesões">
		<Items>
			<evida:MenuItem ID="mnAdesao01" runat="server" Modulo="ADESAO" Url="~/Adesao/BuscaAdesoes.aspx" Label="Adesões" />
		</Items>
	</evida:MenuItem>
    <evida:MenuItem ID="menuAdmin" runat="server" Label="Administração">
        <Items>
			<evida:MenuItem ID="mnAdmin08" runat="server" Modulo="ADMINISTRACAO_CANAL_GESTANTE" Url="~/Admin/CanalGestante.aspx" Label="Cadastro - Canal Gestante" />
            <%--
            <evida:MenuItem ID="mnAdmin03" runat="server" Modulo="ADMINISTRACAO_EMP_RECIPROCIDADE" Url="~/Admin/GerenciarEmpresaReciprocidade.aspx" Label="Empresas de Reciprocidade" />
			--%>
            <evida:MenuItem ID="mnAdmin07" runat="server" Modulo="ADMINISTRACAO_MOTIVO_PENDENCIA" Url="~/Admin/MotivosPendencia.aspx" Label="Motivos de Pendência" />
			<evida:MenuItem ID="mnAdmin05" runat="server" Modulo="ADMINISTRACAO_CONSELHO" Url="~/Admin/Conselhos.aspx" Label="Órgãos" />
            <evida:MenuItem ID="mnAdmin02" runat="server" Modulo="ADMINISTRACAO_PERFIL_MODULO" Url="~/Admin/PerfilModulo.aspx" Label="Perfil x Módulo" />
			<evida:MenuItem ID="mnAdmin04" runat="server" Modulo="ADMINISTRACAO_PLANTAO_SOCIAL" Url="~/Admin/PlantaoSocial.aspx" Label="Plantões Sociais" />
            <evida:MenuItem ID="mnAdmin09" runat="server" Modulo="ADMINISTRACAO_SETOR_USUARIO" Url="~/Admin/SetoresUsuario.aspx" Label="Setores de Usuários" />
			<evida:MenuItem ID="mnAdmin06" runat="server" Modulo="ADMINISTRACAO_TEMPLATE_EMAIL" Url="~/Admin/BuscarTemplatesEmail.aspx" Label="Templates de Email" />
			<evida:MenuItem ID="mnAdmin01" runat="server" Modulo="ADMINISTRACAO_USUARIO" Url="~/Admin/GerenciarUsuario.aspx" Label="Usuários" />
            <%--
            <evida:MenuItem ID="mnAdmin10" runat="server" Modulo="ADMINISTRACAO_USUARIO_SCL" Url="~/Admin/BuscarUsuariosScl.aspx" Label="Usuários SCL" />
            --%>
            
            <evida:MenuItem ID="mnAdmin11" runat="server" Modulo="ADMINISTRACAO_VIAGEM_PARAMETRO" Url="~/Admin/ParametroViagem.aspx" Label="Viagem - Parâmetros" />
            <evida:MenuItem ID="mnAdmin12" runat="server" Modulo="ADMINISTRACAO_ENDERECO_COBRANCA" Url="~/Admin/BuscaEnderecoCobranca.aspx" Label="Endereços de Cobrança" />                   
        </Items>
    </evida:MenuItem>
    
	<evida:MenuItem ID="menuIR" runat="server" Label="Imposto de Renda">
		<Items>
			<evida:MenuItem ID="menuIR0" runat="server" Modulo="IR_CONFIGURACAO" Url="~/IR/ConfiguracaoIr.aspx" Label="Configuração" />
			<evida:MenuItem ID="menuIR1" runat="server" Modulo="IR_BENEFICIARIO" Url="~/IR/IrBeneficiariosTable.aspx" Label="Beneficiários" />
			<evida:MenuItem ID="menuIR2" runat="server" Modulo="IR_CREDENCIADO" Url="~/IR/IrCredenciados.aspx" Label="Credenciados" />
		</Items>
	</evida:MenuItem>
    
    <evida:MenuItem ID="menuGestao" runat="server" Label="Gestão">
        <Items>
            <%--                 
			<evida:MenuItem ID="mnGestao3" runat="server" Modulo="GESTAO_ALT_AUTORIZACAO_ISA" Url="~/Gestao/AjusteAutorizacao.aspx" Label="Ajuste autorização ISA" />
            --%>
            <evida:MenuItem ID="mnGestao04" runat="server" Modulo="GESTAO_DEBITO_ANUAL" Url="~/Gestao/BuscaDebitoAnual.aspx" Label="Declaracao de Débito Anual" />
            <evida:MenuItem ID="mnGestao05" runat="server" Modulo="GESTAO_MEDICAMENTO_REEMBOLSAVEL" Url="~/Gestao/BuscaMedicamentoReembolsavel.aspx" Label="Medicamento Reembolsável" />
            <%--
            <evida:MenuItem ID="mnGestao2" runat="server" Modulo="GESTAO_QUITACAO" Url="~/Gestao/BuscaQuitacao.aspx" Label="Quitação SAP x ISA" />
            <evida:MenuItem ID="mnGestao1" runat="server" Modulo="GESTAO_RESPONSAVEL" Url="~/Gestao/BuscaBenefResponsavel.aspx" Label="Responsáveis Família" />
            --%>
        </Items>
    </evida:MenuItem>
	<evida:MenuItem ID="menuReuniao" runat="server" Label="Reunião/Documentação">
        <Items>            
            <evida:MenuItem ID="mnReuniao1" runat="server" Modulo="VISUALIZAR_REUNIAO" Url="~/Reuniao/BuscaReuniao.aspx" Label="Reunião" />
            <evida:MenuItem ID="mnReuniao2" runat="server" Modulo="VISUALIZAR_REUNIAO" Url="~/Reuniao/Calendario.aspx" Label="Calendário" />
            <evida:MenuItem ID="mnReuniao3" runat="server" Modulo="VISUALIZAR_REUNIAO" Url="~/Reuniao/ArquivosConselho.aspx" Label="Arquivos do Órgão" />
        </Items>
    </evida:MenuItem>
    <evida:MenuItem ID="menuForms" runat="server" Label="Formulários">
        <Items>
			<evida:MenuItem ID="mnForm06" runat="server" Modulo="ATESTADO_COMPARECIMENTO" Url="~/Forms/BuscaAtestadoComparecimento.aspx" Label="Atestado de Comparecimento" />
			<evida:MenuItem ID="mnForm07" runat="server" Modulo="AUTORIZACAO" Url="~/Forms/BuscaAutorizacao.aspx" Label="Autorizações" />
			<evida:MenuItem ID="mnForm08" runat="server" Modulo="AUTORIZACAO_PROVISORIA" Url="~/Forms/BuscaAutorizacaoProvisoria.aspx" Label="Autorizações Provisórias" />
			<evida:MenuItem ID="mnForm12" runat="server" Modulo="CANAL_GESTANTE" Url="~/Forms/BuscaCanalGestante.aspx" Label="Canal Gestante" />
			<evida:MenuItem ID="mnForm14" runat="server" Modulo="CARTA_POSITIVA_CRA" Url="~/FormsSearch/BuscaCartaPosCra.aspx" Label="Carta Positiva CRA" />
			<evida:MenuItem ID="mnForm05" runat="server" Modulo="UNIVERSITARIO" Url="~/Forms/BuscaUniversitario.aspx" Label="Declaração Universitário" />
			<evida:MenuItem ID="mnForm04" runat="server" Modulo="EXCLUSAO" Url="~/Forms/BuscaExclusao.aspx" Label="Exclusão" />
            <evida:MenuItem ID="mnForm09" runat="server" Modulo="INDISPONIBILIDADE_REDE" Url="~/Forms/BuscaIndisponibilidadeRede.aspx" Label="Indisponibilidade de Rede" />
            <evida:MenuItem ID="mnForm02" runat="server" Modulo="NEGATIVA" Url="~/Forms/BuscaFormNegativa.aspx" Label="Negativa" />            
            <evida:MenuItem ID="mnForm10" runat="server" Modulo="PROTOCOLO_FATURA" Url="~/Forms/BuscaProtocoloFatura.aspx" Label="Protocolos de Fatura" />
            <evida:MenuItem ID="mnForm03" runat="server" Modulo="RECIPROCIDADE_VIEW" Url="~/Forms/BuscaReciprocidade.aspx" Label="Reciprocidades" />
            <evida:MenuItem ID="mnForm11" runat="server" Modulo="VIAGEM" Url="~/Forms/BuscaViagem.aspx" Label="Viagem" />
            <evida:MenuItem ID="mnForm01" runat="server" Modulo="GESTAO_SEG_VIA" Url="~/Forms/SegViaCarteira.aspx" Label="2ª Via de Carteira" />
        </Items>
    </evida:MenuItem>
    
    <evida:MenuItem ID="menuRelatorios" runat="server" Label="Relatórios">
        <Items>
            <%--
            <evida:MenuItem ID="mnRelatorio04" runat="server" Modulo="RELATORIO_AUTORIZACAO" Url="~/Relatorios/RelatorioAutorizacao.aspx" Label="AUTORIZAÇÕES" />			
            --%>
            <evida:MenuItem ID="mnRelatorio07" runat="server" Modulo="RELATORIO_RECIPROCIDADE" Url="~/Relatorios/RelatorioReciprocidade.aspx" Label="AUTORIZAÇÕES DE RECIPROCIDADES" />
            <%--
            <evida:MenuItem ID="mnRelatorio18" runat="server" Modulo="RELATORIO_BENEFICIARIOS_CCO" Url="~/Relatorios/RelatorioBeneficiariosCco.aspx" Label="BENEFICIÁRIOS POR CCO" />
			<evida:MenuItem ID="mnRelatorio11" runat="server" Modulo="RELATORIO_BENEFICIARIOS_LOCAL" Url="~/Relatorios/RelatorioBeneficiariosPorLocal.aspx" Label="BENEFICIÁRIOS POR LOCAL" />
            --%>
            <evida:MenuItem ID="mnRelatorio21" runat="server" Modulo="RELATORIO_BENEFICIARIOS_LISTA" Url="~/Relatorios/RelatorioBeneficiariosEmLista.aspx" Label="BENEFICIÁRIOS EM LISTA" />
            <evida:MenuItem ID="mnRelatorio22" runat="server" Modulo="RELATORIO_CREDENCIADOS_LISTA" Url="~/Relatorios/RelatorioCredenciadosEmLista.aspx" Label="CREDENCIADOS EM LISTA" />
            <%--
            <evida:MenuItem ID="mnRelatorio09" runat="server" Modulo="RELATORIO_BOLETO_VENCIMENTO" Url="~/Relatorios/RelatorioBoletosVencimento.aspx" Label="BOLETOS POR VENCIMENTO" />
			<evida:MenuItem ID="mnRelatorio10" runat="server" Modulo="RELATORIO_BOLETO_NAO_QUITADOS" Url="~/Relatorios/RelatorioBoletosNaoQuitados.aspx" Label="BOLETOS NÃO QUITADOS" />
            <evida:MenuItem ID="mnRelatorio03" runat="server" Modulo="RELATORIO_CONTABILIZACAO_EVIDA_ELN" Url="~/Relatorios/RelatorioContabilizacao.aspx" Label="CONTABILIZACAO E-VIDA/ELN" />
            <evida:MenuItem ID="mnRelatorio08" runat="server" Modulo="RELATORIO_COPARTICIPACAO" Url="~/Relatorios/RelatorioCoparticipacao.aspx" Label="COPARTICIPAÇÃO" />
            <evida:MenuItem ID="mnRelatorio16" runat="server" Modulo="RELATORIO_COPARTICIPACAO" Url="~/Relatorios/RelatorioCoparticipacaoParcela.aspx" Label="COPARTIC. - PARCELA" />
            <evida:MenuItem ID="mnRelatorio20" runat="server" Modulo="RELATORIO_CREDITO_BENEFICIARIO" Url="~/Relatorios/RelatorioCreditoBeneficiario.aspx" Label="CRÉDITOS DO BENEFICIÁRIO" />
			<evida:MenuItem ID="mnRelatorio14" runat="server" Modulo="RELATORIO_CUSTO_INTERNACAO" Url="~/Relatorios/RelatorioCustoInternacao.aspx" Label="CUSTO INTERNAÇÃO" />
            <evida:MenuItem ID="mnRelatorio19" runat="server" Modulo="RELATORIO_DEBITO_CONGELADO" Url="~/Relatorios/RelatorioDebitoCongelado.aspx" Label="DÉBITOS CONGELADOS" />
            <evida:MenuItem ID="mnRelatorio05" runat="server" Modulo="RELATORIO_FATURAMENTO_USUARIO" Url="~/Relatorios/RelatorioFaturamentoUsuario.aspx" Label="FATURAMENTO POR USUÁRIO" />
			<evida:MenuItem ID="mnRelatorio13" runat="server" Modulo="RELATORIO_FRANQUIAS_GERADAS" Url="~/Relatorios/RelatorioFranquiasGeradas.aspx" Label="FRANQUIAS GERADAS" />
            <evida:MenuItem ID="mnRelatorio06" runat="server" Modulo="RELATORIO_MENSALIDADE" Url="~/Relatorios/RelatorioMensalidade.aspx" Label="MENSALIDADE" />
            <evida:MenuItem ID="mnRelatorio02" runat="server" Modulo="RELATORIO_PAGAMENTO" Url="~/Relatorios/RelatorioPagamento.aspx" Label="PAGAMENTO" />
			<evida:MenuItem ID="mnRelatorio15" runat="server" Modulo="RELATORIO_PARCELAMENTOS" Url="~/Relatorios/RelatorioParcelamento.aspx" Label="PARCELAMENTOS" />
            <evida:MenuItem ID="mnRelatorio01" runat="server" Modulo="RELATORIO_PROVISAO" Url="~/Relatorios/RelatorioProvisao.aspx" Label="PROVISAO" />			
			<evida:MenuItem ID="mnRelatorio12" runat="server" Modulo="RELATORIO_SERVICO_PRESTADOR" Url="~/Relatorios/RelatorioServicoPrestador.aspx" Label="SERVIÇOS POR PRESTADOR" />
            <evida:MenuItem ID="mnRelatorio17" runat="server" Modulo="RELATORIO_TRAVAMENTO_ISA" Url="~/Relatorios/RelatorioTravamentoISA.aspx" Label="TRAVAMENTO ISA" />
            --%>
            <evida:MenuItem ID="mnRelatorio23" runat="server" Modulo="RELATORIO_DESPESAS_CEA" Url="~/Relatorios/RelatorioDespesasCEA.aspx" Label="DESPESAS CEA" />
            
        </Items>
    </evida:MenuItem>
    <%--
    <evida:MenuItem ID="menuRotinas" runat="server" Label="Rotinas">
        <Items>            
            <evida:MenuItem ID="mnRotina1" runat="server" Modulo="EXECUTAR_ROTINAS" Url="~/Rotinas/ExecutarRotina.aspx" Label="Executar Rotinas" />            
        </Items>
    </evida:MenuItem>
    <evida:MenuItem ID="menuTelefonia" runat="server" Label="Telefonia">
        <Items>            
            <evida:MenuItem ID="mnTelefonia01" runat="server" Modulo="TELEFONIA_RAMAL" Url="~/Telefonia/Ramais.aspx" Label="Ramais" />    
            <evida:MenuItem ID="mnTelefonia02" runat="server" Modulo="TELEFONIA_RELATORIO_GERENCIAL" Url="~/Telefonia/RelatorioGerencial.aspx" Label="Relatório Gerencial" />
        </Items>
    </evida:MenuItem>
    --%>
    <li><asp:LinkButton ID="btnSair" runat="server" CausesValidation="false" Text="Sair" onclick="btnSair_Click" /></li>
</ul>
</div>