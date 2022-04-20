<%@ Page Title="Formulário de Negativa" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormNegativa.aspx.cs" Inherits="eVidaIntranet.Forms.FormNegativa" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .noborder td
        {
            border:none;
        }
    </style>

    <script type="text/javascript">
        (function ($) {
            $.fn.extend({
                limiter: function (limit, elem) {
                    $(this).on("keyup focus", function () {
                        setCount(this, elem);
                    });
                    function setCount(src, elem) {
                        var chars = src.value.length;
                        if (chars > limit) {
                            src.value = src.value.substr(0, limit);
                            chars = limit;
                        }
                        elem.html(formatCounter(limit - chars));
                    }
                    setCount($(this)[0], elem);
                }
            });
        })(jQuery);


        function formatCounter(qtd) {
            return "Restam " + qtd + " caracteres";
        }

        var POP_SERVICO = 1;
        var POP_BENEFICIARIO = 2;
        var POP_CREDENCIADO = 3;
        var POP_PROFISSIONAL = 4;
        var POP_MOTIVO = 5;

        function ConfigControlsCustom() {
            configureCounter('<%=txtPrevContratual.ClientID%>', '<%=lblCounterPrevContratual.ClientID%>', '<%=txtPrevContratual.MaxLength%>'); 

            createLocator(650, 650, dlgOpen, dlgClose, defaultDlgCallback);

            $('.inteiro').each(function () {
                $('#' + this.id).ForceNumericOnly();
            });
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_SERVICO: src = '../GenPops/PopServico.aspx?'; break;
                case POP_BENEFICIARIO: src = '../GenPops/PopBeneficiario.aspx?'; break;
                case POP_CREDENCIADO: src = '../GenPops/PopCredenciado.aspx?'; break;
                case POP_PROFISSIONAL: src = '../GenPops/PopProfissional.aspx?'; break;
                case POP_MOTIVO: src = '../GenPops/PopMotivoGlosa.aspx?'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function dlgClose(ev, ui) {

        }
        function openPop(tipo, obj, id, row) {
            var handler = new LocatorHandler(tipo, id, row, obj);
            var titulo = "";
            switch (tipo) {
                case POP_SERVICO: titulo = "Localizar Serviço"; break;
                case POP_BENEFICIARIO: titulo = "Localizar Beneficiário"; break;
                case POP_CREDENCIADO: titulo = "Localizar Credenciado"; break;
                case POP_PROFISSIONAL: titulo = "Localizar Profissional"; break;
                case POP_MOTIVO: titulo = "Localizar Motivo de Glosa"; break;
            }
            openLocator(titulo, handler);
            return false;
        }


        function configureCounter(item, lbl, size) {
            $("#" + item).limiter(size, $("#"+lbl));
        }

        function openPopServico(id, btn, row) {
            openPop(POP_SERVICO, btn, id, row);
            return false;
        }

        function openPopBeneficiario(btn) {
            openPop(POP_BENEFICIARIO, btn, '<%: hidCodBeneficiario.ClientID %>');
            return false;
        }
        function openPopCred(btnLoc) {
            var handler = new LocatorHandler(POP_CREDENCIADO, '<%: hidCredenciado.ClientID %>', -1, btnLoc);
            openLocator("Credenciado", handler);
            return false;
        }
        function openPopProf(btnLoc) {
            var handler = new LocatorHandler(POP_PROFISSIONAL, '<%: hidProfissional.ClientID %>', -1, btnLoc);
            openLocator("Profissional", handler);
            return false;
        }
        function openPopMotivo(btnLoc) {
            var handler = new LocatorHandler(POP_MOTIVO, '<%: hidMotivo.ClientID %>', -1, btnLoc);
            openLocator("Motivos de Glosa", handler);
            return false;
        }

    	function openPdf(id) {
    		openReport(RELATORIO_NEGATIVA, "ID=" + id, true);
            return false;
        }
        function openView(id) {
            window.open('../FormsPop/PopViewFormNegativa.aspx?ID=' + id, "_negativa", "width=880px, height=700px,scrollbars=yes", true);
            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<div id="conteudo" style="margin-left:15px; text-align: justify" runat="server" clientidmode="Static">
    <div style="text-align:center">
        <asp:ImageButton ID="btnPrint" runat="server" ImageUrl="~/img/print.png" Visible="false" Width="50px" />
        <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Visible="false" Width="50px" />
    </div>
    <div style="text-align:center">
        <b style="font-size:15pt">PROTOCOLO:</b> <asp:Literal ID="litProtocolo" runat="server" /><br />
        <b style="font-size:15pt">PROTOCOLO ANS:</b> <asp:Literal ID="litProtocoloAns" runat="server" /><br />
        <b style="font-size:15pt">COMPROVANTE DE NEGATIVA CONTRATUAL/ ASSISTENCIAL</b><br />
        (Resolução Normativa — RN nº 319, da ANS, de 5 de março de 2013)
    </div>
    <div>
        <br />Prezado(a) beneficiário(a)<br /><br />
        A CAIXA DE ASSISTÊNCIA DO SETOR ELÉTRICO – E-VIDA, pessoa jurídica de direito privado, associação sem fins lucrativos, 
        inscrita no CNPJ/MF sob o nº 11.828.089/0001-03 com registro de operadora de planos privados de assistência à saúde sob o nº 41.837-4, 
        com sede no 704/705 Norte Bloco C Loja 48 CEP: 70.730-630, comunica o indeferimento de concessão de senha de autorização, 
        em virtude da(s) seguinte(s) justificativa(s):
    </div>

    <h2>1. INFORMAÇÕES DE ACORDO COM DISPOSITIVO LEGAL</h2>

    <asp:CheckBox ID="chkCoberturaContratual" runat="server" Text="SEM COBERTURA CONTRATUAL" /><br />
    <asp:CheckBox ID="chkIndicacao" runat="server" Text="SEM INDICAÇÃO TÉCNICA (MÉDICA E/OU ODONTOLÓGICA)" />

    <h2>2.	DADOS DO BENEFICIÁRIO</h2>
    <table width="100%" class="tabelaFormBorder">
        <tr>
            <td colspan="2">Nome do beneficiário:
                <asp:ImageButton ID="btnBuscarBeneficiario" runat="server" ImageUrl="~/img/lupa.gif" 
                    OnClientClick="return openPopBeneficiario(this)" OnClick="btnBuscarBeneficiario_Click" /><br />
                <asp:HiddenField ID="hidCodBeneficiario" runat="server" />
                <asp:Label ID="lblBeneficiario" runat="server" Text="-" Font-Bold="true" /></td>
            <td>Data de Nascimento:<br /><asp:Label ID="lblNascimento" runat="server" Text="-" Font-Bold="true" /></td>
        </tr>
        <tr>
            <td>Nº da carteira do beneficiário:<br /><asp:Label ID="lblCartao" runat="server" Text="-" Font-Bold="true"/></td>
            <td>CPF:<br /><asp:Label ID="lblCpf" runat="server" Text="-" Font-Bold="true"/></td>
            <td>Data de assinatura do contrato:<br /><asp:Label ID="lblDataAdesao" runat="server" Text="-" Font-Bold="true"/></td>
        </tr>
        <tr>
            <td colspan="3">Número do Contrato do beneficiário:<br />
                <asp:TextBox ID="txtContrato" runat="server" Width="250px" /></td>
        </tr>
    </table>

    <h2>3.	DADOS DO PRODUTO</h2>
    <table width="100%" class="tabelaFormBorder" cellpadding="10px">
        <tr>
            <td colspan="2">Nº de identificação/Nome do plano do beneficiário na ANS: <asp:Label ID="lblNroRegistroProduto" runat="server" Font-Bold="true" /> - <asp:Literal ID="ltNomePlano" runat="server" /></td>
        </tr>
        <tr>
            <td>Área Geográfica de Abrangência<br />
                <asp:RadioButtonList ID="rblAreaGeografica" runat="server" CssClass="noborder" RepeatColumns="2" DataValueField="CD_ITEM_LISTA" DataTextField="DS_ITEM_LISTA" />
            </td>
            <td>Padrão de Acomodação<br />
                <asp:RadioButtonList ID="rblAcomodacao" runat="server" CssClass="noborder" >
                    <asp:ListItem Value="ENF" Text="ENFERMARIA" />
                    <asp:ListItem Value="APT" Text="APARTAMENTO" />
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>

    <h2>4.	DADOS DA SOLICITAÇÃO</h2>
    <table width="100%" class="tabelaFormBorder"  cellpadding="10px">
        <tr>
            <td>
				<asp:RadioButtonList ID="rblTipoRede" runat="server" CssClass="noborder" OnSelectedIndexChanged="rblTipoRede_SelectedIndexChanged" AutoPostBack="true">
					<asp:ListItem Value="CRED" Text="REDE CREDENCIADA" />
					<asp:ListItem Value="LIVR" Text="LIVRE ESCOLHA" />
				</asp:RadioButtonList>
            </td>
            <td>
                Código do contratado solicitante:<asp:ImageButton ID="btnLocPrestador" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopCred(this)" OnClick="btnLocPrestador_Click" Visible="false" />
                <asp:HiddenField ID="hidCredenciado" runat="server" />
                <br />
                <asp:TextBox ID="txtCpfCnpjPrestador" runat="server" MaxLength="20" Width="200px" />
            </td>
            <td>Nome do contratado solicitante:<br />
                <asp:TextBox ID="txtNomePrestador" runat="server" MaxLength="255" Width="500px" />
            </td>
        </tr>
        <tr>
            <td colspan="3">Data da solicitação:<br />
                <asp:TextBox ID="txtDataSolicitacao" runat="server" CssClass="calendario" MaxLength="12" Width="120px" />
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
                <asp:HiddenField ID="hidProfissional" runat="server" />
				<asp:Label ID="lblNroConselho" runat="server" Text="Nº do Conselho" Font-Bold="false" /><br />
				<asp:TextBox ID="txtNroConselho" runat="server" Width="150px" AutoPostBack="true" OnTextChanged="txtNroConselho_TextChanged"/>
				<asp:ImageButton ID="btnLocProf" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopProf(this)" OnClick="btnLocProf_Click" />
				<asp:ImageButton ID="btnClrProf" runat="server" ImageUrl="~/img/remove.png" OnClick="btnClrProf_Click" />
            </td>
			<td>
                <asp:Label ID="lblTipoConselho" runat="server" Text="Tipo Conselho/UF" Font-Bold="false" /><br />
				<asp:DropDownList ID="dpdTipoConselho" runat="server" Width="150px" DataValueField="Key" DataTextField="Key" AutoPostBack="true" OnSelectedIndexChanged="dpdTipoConselho_SelectedIndexChanged"/>
				<asp:DropDownList ID="dpdUfConselho" runat="server" Width="60px" DataValueField="Sigla" DataTextField="Sigla" AutoPostBack="true" OnSelectedIndexChanged="dpdUfConselho_SelectedIndexChanged"/>
            </td>
            <td>
				<asp:Label ID="lblNomeProfissional" runat="server" Text="Nome do Profissional" Font-Bold="false" /><br />
				<asp:TextBox ID="txtNomeProfissional" runat="server" Width="400px" />
            </td>
        </tr>
    </table>
	
    <br /><br />
    <b>Procedimentos ou itens assistenciais solicitados</b><br />
    <asp:DataList ID="dtlSolicitacoes" runat="server" OnItemDataBound="dtlSolicitacoes_ItemDataBound">
        <ItemStyle BorderWidth="1px" />
        <ItemTemplate>
            <table cellspacing="10" cellpadding="10px" width="950px">
                <tr><td rowspan="2">
                     <asp:ImageButton ID="btnRemover" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemover_Click" /></td>
                    <td>Código:<br />
                        <asp:Panel ID="pnlSolicitacao" runat="server" DefaultButton="btnBuscarCodigo">
                            <asp:HiddenField ID="hidCodServico" runat="server" />
                            <asp:TextBox ID="txtCodigo" runat="server"  Width="100px"/>
                            <asp:ImageButton ID="btnBuscarCodigo" runat="server" ImageUrl="~/img/seta_direita.png" Width="25px" OnClick="btnBuscarCodigo_Click" />
                        </asp:Panel> 
                    </td>
                    <td>Descrição do Procedimento ou Material:<br /><asp:TextBox ID="txtDescricao" runat="server" Width="600px" />
                        <asp:ImageButton ID="btnBuscarDescricao" runat="server" ImageUrl="~/img/lupa.gif" OnClick="btnBuscarDescricao_Click" />
                    </td>
                    <td>Quantidade:<br /><asp:TextBox ID="txtQuantidade" runat="server" Width="60px" MaxLength="3" CssClass="inteiro" /></td>
                </tr>
                <tr><td colspan="3">
                    Observação:<br />
                    <asp:TextBox ID="txtObservacao" runat="server" TextMode="MultiLine" MaxLength="1400" Rows="4" Width="95%" />
                    <br />
                    <asp:Label ID="lblCounter" runat="server" Text="" />    
                </td></tr>
            </table>
        </ItemTemplate>
    </asp:DataList>
    <asp:Button ID="btnAdicionarItem" runat="server" Text="Adicionar Item" OnClick="btnAdicionarItem_Click" />
    <br /><br />
    
    Solicitação médica e/ou odontológica:<br />
    <asp:TextBox ID="txtSolicitacao" runat="server" MaxLength="1000" Width="900px" TextMode="MultiLine" Rows="4" /><br /><br />
    <table width="100%" class="tabelaFormBorder">
        <tr>
            <td colspan="2">Motivo de Negativa:<asp:ImageButton ID="btnLocMotivo" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopMotivo(this)" OnClick="btnLocMotivo_Click" /><br />
                <asp:HiddenField ID="hidMotivo" runat="server" />
                <asp:TextBox ID="txtCodigoMotivo" runat="server" AutoPostBack="true" Width="100px" CssClass="inteiro" OnTextChanged="txtCodigoMotivo_TextChanged" />                
                <asp:Label ID="lblDescricaoMotivo" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <h2>5. JUSTIFICATIVA CONTRATUAL</h2>
    <table width="100%" cellpadding="25px" cellspacing="15px">
        <tr><td><asp:CheckBox ID="chkJC1" runat="server" /></td>
            <td>Procedimento e/ou solicitação não coberto(a) pelo Rol de Procedimentos e Eventos em Saúde (RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJC2" runat="server" /></td>
            <td>Beneficiário(a) em cumprimento de carência até <asp:TextBox ID="txtDataJC2" runat="server" CssClass="calendario" Width="100px" /> (art. 12, inciso V, da Lei nº 9.656/98).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJC3" runat="server" /></td>
            <td>Atendimento fora da área de abrangência contratual (art. 12, inciso I, letra “a" c/c Anexo I, letra "f", da IN/DIPRO nº 23, da ANS). </td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJC4" runat="server" /></td>
            <td>Procedimento não coberto em contrato de plano de saúde não regulamentado (plano antigo), com vigência anterior à Lei nº 9656/98.</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJC5" runat="server" /></td>
            <td>Beneficiário(a) submetido(a) à Cobertura Parcial Temporária - CPT para Doença ou Lesão Preexistente - DLP até 
                <asp:TextBox ID="txtDataJC5" runat="server" CssClass="calendario" Width="100px" />  (art. 2º, inciso II, RN ANS 162/2007). </td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJC6" runat="server" /></td>
            <td>Inadimplência superior a 60 dias (art. 13, parágrafo único, inciso 11, da Lei nº 9.656/98). </td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJC7" runat="server" /></td>
            <td>Beneficiário excluído em 
                <asp:TextBox ID="txtDataJC7" runat="server" CssClass="calendario" Width="100px" />  (art. 13, parágrafo único, inciso 11, da Lei nº 9.656/98). </td>
        </tr>
    </table>
        
    <h2>6.	JUSTIFICATIVA ASSISTENCIAL</h2>
    <table width="100%" cellpadding="25px" cellspacing="15px">
        <tr><td><asp:CheckBox ID="chkJA1" runat="server" /></td>
            <td>Não atendimento das Diretrizes de Utilização (DUT) previstas no Rol de Procedimentos e Eventos em Saúde (Anexo I da RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA2" runat="server" /></td>
            <td>Não atendimento das Diretrizes Clínicas previstas no Rol de Procedimentos e Eventos em Saúde 
			(Anexo II da RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA3" runat="server" /></td>
            <td>Procedimentos clínicos ou cirúrgicos com finalidade estética, bem como órteses e próteses para o mesmo fim, ou seja, aqueles que não visam restauração parcial ou total da  função de órgão ou parte do corpo humano lesionada, seja por enfermidade, traumatismo ou anomalia congênita (art. 20, inciso II, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA4" runat="server" /></td>
            <td>Fornecimento de medicamentos para tratamento domiciliar, isto é, aqueles prescritos pelo médico assistente para a administração em ambiente externo ao de unidade de saúde, com exceção dos medicamentos previstos nos incisos X e XI do art. 21 desta RN e, ressalvando o disposto no artigo 14 desta resolução normativa; (Redação dada pela Retificação publicada no DOU em 04 de Dezembro de 2015, Seção 1, página 41) (art. 20, inciso VI, RN ANS 387, 28 de outubro de 2015)</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA5" runat="server" /></td>
            <td>Fornecimento de medicamentos e produtos para a saúde importados não nacionalizados, isto é, aqueles produzidos fora do território nacional e sem registro vigente na ANVISA (art. 20, inciso V, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA6" runat="server" /></td>
            <td>Tratamento de rejuvenescimento ou de emagrecimento com finalidade estética, assim como em spas, clínicas de repouso e estâncias hidrominerais (art. 20, inciso IV, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA7" runat="server" /></td>
            <td>Próteses, órteses e seus acessórios não ligados ao ato cirúrgico (art. 20, inciso VII, RN ANS 387, 28 de outubro de 2015). </td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA8" runat="server" /></td>
            <td>Divergência médica e/ou científica, baseada em decisão de Junta Médica (inciso V, art. 4º da Resolução CONSU nº 08/98; art. 16, parágrafo 4Q, RN ANS 338/2013 Art. 21 § 1º; e na Resolução do CFM nº 1956/2010).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA9" runat="server" /></td>
            <td>Tratamento clínico ou cirúrgico experimental, isto é, aquele que: a) emprega medicamentos, produtos para a saúde ou técnicas não registrados/não regularizados no país; b) é considerado experimental pelo Conselho Federal de Medicina - CFM ou pelo Conselho Federal de Odontologia- CFO; ou c) não possui as indicações descritas na bula/manual registrado na ANVISA, uso off-label (art. 20, inciso I, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA10" runat="server" /></td>
            <td>Inseminação artificial, entendida como técnica de reprodução assistida que inclui a manipulação de oócitos e esperma para alcançar a fertilização, por meio de injeções de esperma intracitoplasmáticas, transferência intrafalopiana de gameta, doação de oócitos, indução da ovulação, concepção póstuma, recuperação espermática ou transferência intratubária do zigoto, entre outras técnicas 
			(art. 20, inciso III, RN ANS 338/2013 c/c art. 10, inciso III, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA11" runat="server" /></td>
            <td>Tratamentos ilícitos ou antiéticos, assim definidos sob o aspecto médico, ou não reconhecidos pelas autoridades competentes 
			(art. 20, inciso VIII, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA12" runat="server" /></td>
            <td>Estabelecimentos para acolhimento de idosos e internações que não necessitem de cuidados médicos em ambiente hospitalar 
			(art. 20, inciso X, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA13" runat="server" /></td>
            <td>Casos de cataclismos, guerras e comoções internas, quando declarados pela autoridade competente 
				(art. 20, inciso IX, RN ANS 387, 28 de outubro de 2015).</td>
        </tr>
        <tr><td><asp:CheckBox ID="chkJA99" runat="server" /></td>
            <td>Outros: <br />
				<asp:TextBox ID="txtJA99" runat="server" TextMode="MultiLine" Width="800px" Rows="5" MaxLength="1400" Height="200px"/></td>
        </tr>

    </table>
    Previsão da cláusula contratual, se aplicável ao caso:<br />
    <asp:TextBox ID="txtPrevContratual" runat="server" TextMode="MultiLine" Width="800px" Rows="5" MaxLength="1400" Height="200px" /><br />
    <asp:Label ID="lblCounterPrevContratual" runat="server" />
    <br /><br />
    Observação: Em caso de pleito para revisão da negativa, encaminhar solicitação de revisão através do canal ouvidoria@e-vida.org.br.
    <br /><br />
    A Lei nº 9.656/98 e as normas citadas acima podem ser consultadas no site da Agência Nacional de Saúde Suplementar (www.ans.gov.br) e no site do Conselho Federal de Medicina - CFM (www.portal.cfm.org.br).
    <br /><br />
    O presente documento constituído de ______ páginas informa as razões e motivos da negativa de cobertura que poderá ocorrer por razões contratuais e/ou assistenciais.
    <br /> <br />
    Data da informação ao beneficiário:<br />
    Brasília, <asp:TextBox ID="txtDataFormulario" runat="server" CssClass="calendario" Width="120px" /> .
    <br /><br />
    <table width="100%" class="tabelaFormBorder" cellpadding="10px">
        <tr><td width="150px">Elaborado por:</td><td><asp:Label ID="lblUsuario" runat="server" /></td></tr>
        <tr><td>Cargo/Função:</td><td><asp:Label ID="lblCargo" runat="server" /></td></tr>
    </table>

    <div style="text-align:center">
        <br /><br /><br /><br />
        ____________________________________<br />
[APROVADOR]<br />
[CARGO APROVADOR]<br />
        <br />
        <asp:Button ID="btnSalvar" runat="server" Text="Salvar Formulário" OnClick="btnSalvar_Click" />
        <asp:Button ID="btnAprovar" runat="server" Text="Aprovar Negativa" OnClick="btnAprovar_Click" OnClientClick="return confirm('Deseja realmente aprovar esta solicitação?');" Visible="false" />
    </div>
    
    </div>
</asp:Content>
