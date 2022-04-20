<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopViewFormNegativa.aspx.cs" Inherits="eVidaIntranet.Forms.PopViewFormNegativa" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <style type="text/css">
        .frmTabela {
            border: 1px solid black;            
	        border-collapse: collapse;
            
        }
        .frmTabela td {
            border: 1px solid black;            
	        border-collapse: collapse;
            padding: 5px;
            vertical-align: top;
        }

        .noborder td
        {
            border:none;
        }

        .checklabel
        {
            padding:2px;
            text-align:center;
            border: 1px solid;
            width:15px;
            height:15px;
            display:inline-block;
        }
    </style>
    <script type="text/javascript">
        function ConfigControlsCustom() {
            window.print();
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
<div id="conteudo" style="margin-left:15px; text-align: justify; width:830px" runat="server" clientidmode="Static">
    <table width="100%">
        <tr>
            <td width="33%"></td>
            <td width="33%" align="center"><img src="../img/logo-sized.jpg" alt="Logo" /></td>
            <td align="right">PROTOCOLO <asp:Literal ID="litProtocolo" runat="server" /><br />
                PROTOCOLO ANS <asp:Literal ID="litProtocoloAns" runat="server" /></td>
        </tr>
    </table>
    <div style="text-align:center">
        <b style="font-size:15pt">COMPROVANTE DE NEGATIVA CONTRATUAL/ ASSISTENCIAL</b><br />
        (RESOLUÇÃO NORMATIVA - RN Nº 395, DE 14 DE JANEIRO DE 2016)
    </div>
    <div>
        <br />Prezado(a) beneficiário(a)<br /><br />
        A <b>CAIXA DE ASSISTÊNCIA DO SETOR ELÉTRICO – E-VIDA</b>, pessoa jurídica de direito privado, associação sem fins lucrativos, 
        inscrita no CNPJ/MF sob o nº 11.828.089/0001-03 com registro de operadora de planos privados de assistência à saúde sob o nº <b>41.837-4</b>, 
        com sede no 704/705 Norte Bloco C Loja 48 CEP: 70.730-630, comunica o indeferimento de concessão de senha de autorização, 
        em virtude da(s) seguinte(s) justificativa(s):
    </div>

    <h2>1. INFORMAÇÕES DE ACORDO COM DISPOSITIVO LEGAL</h2>
    <asp:Label ID="lblCoberturaContratual" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text=" " CssClass="checklabel" />SEM COBERTURA CONTRATUAL<br />
    <asp:Label ID="lblIndicacao" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text=" " CssClass="checklabel" />SEM INDICAÇÃO TÉCNICA (MÉDICA E/OU ODONTOLÓGICA)<br />
    
    <h2>2.	DADOS DO BENEFICIÁRIO</h2>
    <table width="100%" class="frmTabela" cellpadding="10px">
        <tr>
            <td colspan="2">Nome do beneficiário:<br /><asp:Label ID="lblBeneficiario" runat="server" Text="-" Font-Bold="true" /></td>
            <td>Data de Nascimento:<br /><asp:Label ID="lblNascimento" runat="server" Text="-" Font-Bold="true" /></td>
        </tr>
        <tr>
            <td>Nº da carteira do beneficiário:<br /><asp:Label ID="lblCartao" runat="server" Text="-" Font-Bold="true"/></td>
            <td>CPF:<br /><asp:Label ID="lblCpf" runat="server" Text="-" Font-Bold="true"/></td>
            <td>Data de assinatura do contrato:<br /><asp:Label ID="lblDataAdesao" runat="server" Text="-" Font-Bold="true"/></td>
        </tr>
        <tr>
            <td colspan="3">Número do Contrato do beneficiário:<br /><asp:Label ID="lblContrato" runat="server" Text="-" Font-Bold="true" /></td>
        </tr>
    </table>

    <h2>3.	DADOS DO PRODUTO</h2>
    <table width="100%" class="frmTabela" cellpadding="10px">
        <tr>
            <td colspan="2">Nº de identificação/Nome do plano do beneficiário na ANS: <asp:Label ID="lblNroRegistroProduto" runat="server" Font-Bold="true" /> - <asp:Label ID="lblNomePlano" runat="server" /></td>
        </tr>
        <tr>
            <td>Área Geográfica de Abrangência<br />
                <asp:DataList ID="dtlAreaGeografica" runat="server" RepeatColumns="2" CssClass="noborder" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <asp:HiddenField ID="hid" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "CD_ITEM_LISTA") %>' />
                        <asp:Label ID="lblCheck" runat="server" CssClass="checklabel" Text="&nbsp;" />
                        <asp:Literal ID="lbl" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DS_ITEM_LISTA") %>'  />
                    </ItemTemplate>
                </asp:DataList>                
            </td>
            <td>Padrão de Acomodação<br />
                <asp:Label ID="chkEnfermaria" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text="&nbsp;" CssClass="checklabel" />ENFERMARIA<br />
                <asp:Label ID="chkApartamento" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text="&nbsp;" CssClass="checklabel" />APARTAMENTO<br />
            </td>
        </tr>
    </table>

    <h2>4.	DADOS DA SOLICITAÇÃO</h2>

	<table width="100%" class="frmTabela" cellpadding="10px">
        <tr>
            <td><asp:Label ID="chkRedeCred" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text=" " CssClass="checklabel" />REDE CREDENCIADA<br />
				<asp:Label ID="chkRedeLivre" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text=" " CssClass="checklabel" />LIVRE ESCOLHA<br />               
            </td>
            <td>Código do contratado solicitante:<br />
                <asp:Label ID="lblCpfCnpjPrestador" runat="server" Font-Bold="true" />
            </td>
            <td>
                Nome do contratado solicitante:<br />
                <asp:Label ID="txtNomePrestador" runat="server" Font-Bold="true" /><br />                
            </td>
        </tr>
        <tr>
            <td colspan="3">Data de Solicitacao:<br />
                <asp:Label ID="txtDataSolicitacao" runat="server" Font-Bold="true" /><br />
            </td>
        </tr>
    </table>
    <br />
    <table width="100%" class="tabelaFormBorder" cellpadding="10px">
        <tr>
            <td colspan="3"><b>DADOS DO PROFISSIONAL SOLICITANTE</b></td>
        </tr>
        <tr>
            <td>
				<asp:Label ID="lblNroConselho" runat="server" Text="Nº do Conselho" Font-Bold="false" /><br />
				<asp:Label ID="txtNroConselho" runat="server" Font-Bold="true"/>
            </td>
			<td>
                <asp:Label ID="lblConselho" runat="server" Text="Tipo Conselho/UF" Font-Bold="false" /><br />
                <asp:Label ID="txtConselho" runat="server" Font-Bold="true" />
            </td>
            <td>
				<asp:Label ID="lblNomeProfissional" runat="server" Text="Nome do Profissional" Font-Bold="false" /><br />
				<asp:Label ID="txtNomeProfissional" runat="server" Font-Bold="true" />
            </td>
        </tr>
    </table>
    <br />
    <b>Procedimentos ou itens assistenciais solicitados</b><br />
    <asp:DataList ID="dtlSolicitacoes" runat="server" OnItemDataBound="dtlSolicitacoes_ItemDataBound">
        <ItemTemplate>
            <br />
            <table cellspacing="10" cellpadding="10px" width="830px" class="frmTabela" >
                <tr>
                    <td>Código:<br /><asp:Label ID="txtCodigo" runat="server"  Width="100px"/></td>
                    <td>Descrição do Procedimento ou Material:<br /><asp:Label ID="txtDescricao" runat="server" /></td>
                    <td>Quantidade:<br /><asp:Label ID="txtQuantidade" runat="server" Width="60px" /></td>
                </tr>
                <tr><td colspan="3">
                    Observação:<br />
                    <asp:Label ID="txtObservacao" runat="server" Width="90%" />
                </td></tr>
            </table>
            
        </ItemTemplate>
    </asp:DataList>
    <br />
    <div style="text-align:left">
    Solicitação médica e/ou odontológica:<br />
    <asp:Label ID="txtSolicitacao" runat="server" Font-Bold="true" /><br />
    </div>
    <br />
    <table width="100%" class="tabelaFormBorder">
        <tr>
            <td colspan="2">Motivo de Negativa:<br />                
                <asp:Label ID="lblMotivo" runat="server" Font-Bold="true" />
            </td>
        </tr>
    </table>
    <h2>5. JUSTIFICATIVA CONTRATUAL</h2>
    <table width="100%" cellpadding="15px" cellspacing="15px">
        <tr><td style="width:25px"><asp:Label ID="chkJC1" runat="server" /></td>
            <td>Procedimento e/ou solicitação não coberto(a) pelo Rol de Procedimentos e Eventos em Saúde (RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJC2" runat="server" /></td>
            <td>Beneficiário(a) em cumprimento de carência até <asp:Label ID="txtDataJC2" runat="server" Width="100px" /> (art. 12, inciso V, da Lei nº 9.656/98).</td>
        </tr>
        <tr><td><asp:Label ID="chkJC3" runat="server" /></td>
            <td>Atendimento fora da área de abrangência contratual (art. 12, inciso I, letra “a" c/c Anexo I, letra "f", da IN/DIPRO nº 23, da ANS). </td>
        </tr>
        <tr><td><asp:Label ID="chkJC4" runat="server" /></td>
            <td>Procedimento não coberto em contrato de plano de saúde não regulamentado (plano antigo), com vigência anterior à Lei nº 9656/98.</td>
        </tr>
        <tr><td><asp:Label ID="chkJC5" runat="server" /></td>
            <td>Beneficiário(a) submetido(a) à Cobertura Parcial Temporária - CPT para Doença ou Lesão Preexistente - DLP até 
                <asp:Label ID="txtDataJC5" runat="server" Width="100px" />  (art. 2º, inciso II, RN ANS 162/2007). </td>
        </tr>
        <tr><td><asp:Label ID="chkJC6" runat="server" /></td>
            <td>Inadimplência superior a 60 dias (art. 13, parágrafo único, inciso 11, da Lei nº 9.656/98). </td>
        </tr>
        <tr><td><asp:Label ID="chkJC7" runat="server" /></td>
            <td>Beneficiário excluído em 
                <asp:Label ID="txtDataJC7" runat="server" Width="100px" />  (art. 13, parágrafo único, inciso 11, da Lei nº 9.656/98). </td>
        </tr>
    </table>
        
    <h2>6.	JUSTIFICATIVA ASSISTENCIAL</h2>
    <table width="100%" cellpadding="25px" cellspacing="15px">
        <tr><td style="width:25px"><asp:Label ID="chkJA1" runat="server" /></td>
            <td>Não atendimento das Diretrizes de Utilização (DUT) previstas no Rol de Procedimentos e Eventos em Saúde (Anexo I da RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA2" runat="server" /></td>
            <td>Não atendimento das Diretrizes Clínicas previstas no Rol de Procedimentos e Eventos em Saúde 
			(Anexo II da RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA3" runat="server" /></td>
            <td>Procedimentos clínicos ou cirúrgicos com finalidade estética, bem como órteses e próteses para o mesmo fim, ou seja, aqueles que não visam restauração parcial ou total da  função de órgão ou parte do corpo humano lesionada, seja por enfermidade, traumatismo ou anomalia congênita (art. 20, inciso II, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA4" runat="server" /></td>
            <td>Fornecimento de medicamentos para tratamento domiciliar, isto é, aqueles prescritos pelo médico assistente para a administração em ambiente externo ao de unidade de saúde, com exceção dos medicamentos previstos nos incisos X e XI do art. 21 desta RN e, ressalvando o disposto no artigo 14 desta resolução normativa; (Redação dada pela Retificação publicada no DOU em 04 de Dezembro de 2015, Seção 1, página 41) (art. 20, inciso VI, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017)</td>
        </tr>
        <tr><td><asp:Label ID="chkJA5" runat="server" /></td>
            <td>Fornecimento de medicamentos e produtos para a saúde importados não nacionalizados, isto é, aqueles produzidos fora do território nacional e sem registro vigente na ANVISA (art. 20, inciso V, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA6" runat="server" /></td>
            <td>Tratamento de rejuvenescimento ou de emagrecimento com finalidade estética, assim como em spas, clínicas de repouso e estâncias hidrominerais (art. 20, inciso IV, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA7" runat="server" /></td>
            <td>Próteses, órteses e seus acessórios não ligados ao ato cirúrgico (art. 20, inciso VII, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017). </td>
        </tr>
        <tr><td><asp:Label ID="chkJA8" runat="server" /></td>
            <td>Divergência médica e/ou científica, baseada em decisão de Junta Médica (inciso V, art. 4º da Resolução CONSU nº 08/98; art. 16, parágrafo 4Q, RN ANS 338/2013 Art. 21 § 1º; e na Resolução do CFM nº 1956/2010).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA9" runat="server" /></td>
            <td>Tratamento clínico ou cirúrgico experimental, isto é, aquele que: a) emprega medicamentos, produtos para a saúde ou técnicas não registrados/não regularizados no país; b) é considerado experimental pelo Conselho Federal de Medicina - CFM ou pelo Conselho Federal de Odontologia- CFO; ou c) não possui as indicações descritas na bula/manual registrado na ANVISA, uso off-label (art. 20, inciso I, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA10" runat="server" /></td>
            <td>Inseminação artificial, entendida como técnica de reprodução assistida que inclui a manipulação de oócitos e esperma para alcançar a fertilização, por meio de injeções de esperma intracitoplasmáticas, transferência intrafalopiana de gameta, doação de oócitos, indução da ovulação, concepção póstuma, recuperação espermática ou transferência intratubária do zigoto, entre outras técnicas 
			(art. 20, inciso III, RN ANS 338/2013 c/c art. 10, inciso III, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA11" runat="server" /></td>
            <td>Tratamentos ilícitos ou antiéticos, assim definidos sob o aspecto médico, ou não reconhecidos pelas autoridades competentes 
			(art. 20, inciso VIII, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA12" runat="server" /></td>
            <td>Estabelecimentos para acolhimento de idosos e internações que não necessitem de cuidados médicos em ambiente hospitalar 
			(art. 20, inciso X, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA13" runat="server" /></td>
            <td>Casos de cataclismos, guerras e comoções internas, quando declarados pela autoridade competente 
				(art. 20, inciso IX, RESOLUÇÃO NORMATIVA - RN Nº 428, DE 7 DE NOVEMBRO DE 2017).</td>
        </tr>
        <tr><td><asp:Label ID="chkJA99" runat="server" /></td>
            <td>Outros: <asp:Label ID="txtJA99" runat="server" Width="550px" /></td>
        </tr>

    </table>
    Previsão da cláusula contratual, se aplicável ao caso:<br />
    <asp:Label ID="txtPrevContratual" runat="server" Width="100%" BorderWidth="1px" />
    <br /><br />
    Observação: Em caso de pleito para revisão da negativa, encaminhar solicitação de revisão através do canal ouvidoria@e-vida.org.br.
    <br /><br />
    A Lei nº 9.656/98 e as normas citadas acima podem ser consultadas no site da Agência Nacional de Saúde Suplementar (www.ans.gov.br) e no site do Conselho Federal de Medicina - CFM (www.portal.cfm.org.br).
    <br /><br />
    O presente documento constituído de ______ páginas informa as razões e motivos da negativa de cobertura que poderá ocorrer por razões contratuais e/ou assistenciais.
    <br /> <br />
    Data da informação ao beneficiário:<br />
    Brasília, <asp:Label ID="txtDataFormulario" runat="server"/> .
    <br /><br />
    <table width="100%" class="frmTabela" cellpadding="10px">
        <tr><td width="150px">Elaborado por:</td><td><asp:Label ID="lblUsuario" runat="server" /></td></tr>
        <tr><td>Cargo/Função:</td><td><asp:Label ID="lblCargo" runat="server" /></td></tr>
    </table>

    <div style="text-align:center">
        <br /><br /><br /><br />      
        ____________________________________<br />
<asp:Label ID="lblAprovador" runat="server" /><br />
<asp:Label ID="lblCargoAprovador" runat="server" /><br />
        <br />
    </div>
    
    <table width="100%">
        <tr>
            <td align="center" style="font-size:8pt"><b style="text-decoration:underline">E-VIDA</b> - Caixa de Assistência do Setor Elétrico.<br />
                <b>Mais Energia na Gestão de sua Saúde</b><br />
 <img src="../img/ans.png" width="100px" alt="ANS 41.837-4" />
</td>
            <td style="font-size:8pt">704/705 Norte Bloco C Loja 48 CEP: 70.730-630.<br />
            Tel.: 0800-607-8300. www.e-vida.org.br
            </td>
        </tr>
    </table>
    </div>
</asp:Content>
