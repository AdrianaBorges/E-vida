<%@ Page Title="Formulário de Reanálise de Negativa" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormNegativaReanalise.aspx.cs" Inherits="eVidaIntranet.Forms.FormNegativaReanalise" %>
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
        
        function ConfigControlsCustom() {
            configureCounter('<%=txtJustReanalise.ClientID%>', '<%=lblCounterJustReanalise.ClientID%>', '<%=txtJustReanalise.MaxLength%>'); 

            createLocator(650, 650, dlgOpen, dlgClose, defaultDlgCallback);

            $('.inteiro').each(function () {
                $('#' + this.id).ForceNumericOnly();
            });
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                
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
               
            }
            openLocator(titulo, handler);
            return false;
        }


        function configureCounter(item, lbl, size) {
            $("#" + item).limiter(size, $("#"+lbl));
        }


    	function openPdf(id) {
    		openReport(RELATORIO_NEGATIVA, "ID=" + id + "&RN=1", true);
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<div id="conteudo" style="margin-left:15px; text-align: justify" runat="server" clientidmode="Static">
    <div style="text-align:center">
        <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Visible="false" Width="50px" />
    </div>
    <div style="text-align:center">
        <b style="font-size:15pt">PROTOCOLO:</b> <asp:Literal ID="litProtocolo" runat="server" /><br />
        <b style="font-size:15pt">PROTOCOLO ANS:</b> <asp:Literal ID="litProtocoloAns" runat="server" /><br />
        <b style="font-size:15pt">COMPROVANTE DE REANÁLISE DE NEGATIVA CONTRATUAL/ ASSISTENCIAL</b><br />
    </div>
    <div>
        <br />Prezado(a) beneficiário(a)<br /><br />
        A CAIXA DE ASSISTÊNCIA DO SETOR ELÉTRICO – E-VIDA, pessoa jurídica de direito privado, associação sem fins lucrativos, inscrita no 
        CNPJ/MF sob o nº 11.828.089/0001-03 com registro de operadora de planos privados de assistência à saúde sob o nº 41.837-4, 
        com sede no 704/705 Norte Bloco C Loja 48 CEP: 70.730-630, comunica o indeferimento de concessão de senha de autorização, 
        em virtude da (s) seguinte(s) justificativa(s):
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
                <asp:Label ID="chkEnfermaria" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text="&nbsp;" CssClass="checklabel" /> ENFERMARIA<br />
                <asp:Label ID="chkApartamento" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text="&nbsp;" CssClass="checklabel" /> APARTAMENTO<br />
            </td>
        </tr>
    </table>

    <h2>4.	DADOS DA SOLICITAÇÃO</h2>

	<table width="100%" class="frmTabela" cellpadding="10px">
        <tr>
            <td><asp:Label ID="chkRedeCred" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text=" " CssClass="checklabel" /> REDE CREDENCIADA<br />
				<asp:Label ID="chkRedeLivre" runat="server" BorderWidth="1px" Width="15px" Height="15px" Text=" " CssClass="checklabel" /> LIVRE ESCOLHA<br />               
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
    <br />
    <h2>5. NÚMERO DO PROTOCOLO DA NEGATIVA DE COBERTURA</h2>
    <table width="100%" cellpadding="25px" cellspacing="15px" class="tabelaBorda">
        <tr>
            <td><asp:Label ID="lblIdProtocolo" runat="server" Font-Bold="true" /><br /></td>
        </tr>
    </table>
        
    <h2>6.	JUSTIFICATIVA DA NEGATIVA DE COBERTURA</h2>
    <table width="100%" cellpadding="25px" cellspacing="15px">
        <tr>
            <td><asp:TextBox ID="txtJustReanalise" runat="server" TextMode="MultiLine" Rows="4" Width="90%" MaxLength="500" /><br />
                <asp:Label ID="lblCounterJustReanalise" runat="server" />
            </td>
        </tr>
    </table>
    
    <h2>7.	POSICIONAMENTO DA REANÁLISE</h2>
    <table width="100%" cellpadding="25px" cellspacing="15px">
        <tr>
            <td>                
                <asp:RadioButtonList ID="rblPosicionamento" runat="server" CssClass="noborder" >
                    <asp:ListItem Value="N" Text="NEGADO" />
                    <asp:ListItem Value="A" Text="AUTORIZADO" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtObsPosicionamento" runat="server" TextMode="MultiLine" Rows="4" Width="90%" MaxLength="500" /></td>
        </tr>
    </table>

    Data da informação ao beneficiário:<br />
    Brasília, <asp:TextBox ID="txtDataReanalise" runat="server" CssClass="calendario" Width="120px" /> .
    <br /><br />
    <table width="100%" class="tabelaFormBorder" cellpadding="10px">
        <tr><td width="150px">Elaborado por:</td><td><asp:Label ID="lblUsuario" runat="server" /></td></tr>
        <tr><td>Cargo/Função:</td><td><asp:Label ID="lblCargo" runat="server" /></td></tr>
    </table>

    <div style="text-align:center">
        <br /><br /><br /><br />
        ____________________________________<br />
<asp:Label ID="lblAprovador" runat="server" /><br />
<asp:Label ID="lblCargoAprovador" runat="server" /><br />
        <br />
        <asp:Button ID="btnSalvar" runat="server" Text="Criar Reanálise" OnClick="btnSalvar_Click" />
        <asp:Button ID="btnAprovar" runat="server" Text="Aprovar Reanálise" OnClick="btnAprovar_Click" OnClientClick="return confirm('Deseja realmente aprovar esta reanálise?');" Visible="false" />
        <br /><br />
    </div>
    <table width="100%" cellpadding="10" id="tbDevolver" runat="server" visible="false">
        <tr>
            <td><b>Devolução caso haja incosistência</b></td>
        </tr>
        <tr>
                <td style="text-align:center">
            <asp:TextBox ID="txtDevolucao" runat="server" TextMode="MultiLine" MaxLength="300" Width="90%" />
            <asp:Button ID="btnDevolver" runat="server" Text="Devolver ao elaborador" OnClick="btnDevolver_Click" />

            </td>
        </tr>
    </table>
    
    </div>
</asp:Content>
