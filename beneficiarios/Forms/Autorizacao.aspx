<%@ Page Title="SOLICITAÇÃO DE AUTORIZAÇÃO" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Autorizacao.aspx.cs" Inherits="eVidaBeneficiarios.Forms.Autorizacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

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
        var POP_ARQUIVO = 2;
        var POP_DOENCA = 3;
        var POP_CREDENCIADO = 4;
        var POP_PROFISSIONAL = 5;
        var POP_HOSPITAL = 6;

        var isMsgSalvaOK = false;
        function ClosePopUpCustom() {
        	if (isMsgSalvaOK) {
        		window.location = './BuscaAutorizacao.aspx';
        		return false;
        	}
        }
        function ConfigControlsCustom() {
        	createLocator(650, 550, dlgOpen, null, dlgCallback);

        	$('.inteiro').each(function () {
        		$('#' + this.id).ForceNumericOnly();
        	});

        	if (typeof (configAllCounters) === 'function')
        		setTimeout(function () { configAllCounters(); }, 1000);
        }

        function configureCounter(item, lbl, size) {
        	$("#" + item).limiter(size, $("#" + lbl));
        }

        function dlgOpen(handler, ev, ui) {
        	var src = "";
        	switch (handler.tipo) {
        		case POP_SERVICO: src = '../GenPops/PopServico.aspx?SHOWT=true'; break;
        		case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.AUTORIZACAO.Value %>'; break;
        		case POP_DOENCA: src = '../GenPops/PopDoenca.aspx'; break;
        		case POP_CREDENCIADO: src = '../GenPops/PopCredenciado.aspx?enableEmpty=true'; break;
        		case POP_PROFISSIONAL: src = '../GenPops/PopProfissional.aspx?enableEmpty=true'; break;
        		case POP_HOSPITAL: src = '../GenPops/PopCredenciado.aspx?hospital=true&enableEmpty=true'; break;
        	}
        	setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
        	switch (handler.tipo) {
        		case POP_ARQUIVO: break;
        		case POP_SERVICO:
        		case POP_CREDENCIADO:
        		case POP_PROFISSIONAL:
				case POP_HOSPITAL:
        			$("#" + handler.id).val(response);
        			closeLocator();
        			__doPostBack(handler.btn.name, '');
        			break;
        		case POP_DOENCA:
        			$("#<%: txtDoenca.ClientID %>").val(response);
    				$("#<%: hidDoenca.ClientID %>").val(response);
    				closeLocator();
    				break;
			}
		}

    	function openPopServico($idCod, $btnLoc) {
    		var handler = new LocatorHandler(POP_SERVICO, $idCod, -1, $btnLoc);
    		openLocator("Serviços", handler);
    		return false;
    	}

    	function openArquivo() {
    		var handler = new LocatorHandler(POP_ARQUIVO);
    		openLocator("Arquivos", handler);
    		return false;
    	}
    	function openPopDoenca() {
    		var handler = new LocatorHandler(POP_DOENCA);
    		openLocator("Doença", handler);
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
    	function openPopHosp(btnLoc) {
    		var handler = new LocatorHandler(POP_HOSPITAL, '<%: hidHospital.ClientID %>', -1, btnLoc);
    		openLocator("Hospital", handler);
    		return false;
    	}

    	function openPdf(id) {
    		openReport(RELATORIO_NEGATIVA, "ID=" + id, true);
    		return false;
    	}
    	function openView(id) {
    		window.open('PopViewFormNegativa.aspx?ID=' + id, "_negativa", "width=880px, height=700px,scrollbars=yes", true);
    		return false;
    	}

    	function onAfterUpload(url, originalName) {
    		$("#<%:hidArqFisico.ClientID%>").val(url);
    		$("#<%:hidArqOrigem.ClientID%>").val(originalName);
    		$('#popLocator').dialog('close');
			<%= ClientScript.GetPostBackEventReference(btnIncluirArquivo, "") %>
    	}

    	function goNova() {
    		window.location = './Autorizacao.aspx';
    		return false;
    	}

    	function confirmReenvio() {
    		return confirm("Deseja realmente reencaminhar o formulário? Após reenvio não será mais possível anexar documentos!");
    	}
    	function openDownload(fName, isNew) {
    		if (!isNew) {
    			openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.AUTORIZACAO %>', "ID=" + $("#<%: litProtocolo.ClientID %>").val() + ";" + fName);
    		} else {
    			alert('Apenas arquivos salvos podem ser abertos!');
    		}
			return false;
    	}
    	function openNegPdf(id) {
    		openReport(RELATORIO_NEGATIVA, 'ID=' + id, false);
    		return false;
    	}

    	function confirmSave() {
    		return true;
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<div id="divAtendimento" runat="server" style="width:50%; margin: auto">
		<h2>Atendimento</h2>
	    <table border="1" class="tbBorda">
            <tr>
				<td>
                    A Autorização de Atendimento será:
                </td>
				<td>
                    <asp:RadioButtonList ID="rblAtendimento" runat="server" OnSelectedIndexChanged="rblAtendimento_SelectedIndexChanged" AutoPostBack="true">
						<asp:ListItem Text="Rede Credenciada" Value="1" />
						<asp:ListItem Text="Reembolso - Livre Escolha" Value="2" />
                    </asp:RadioButtonList>
                </td>
			</tr>
		</table>
		<asp:Panel ID="pnlMsgReembolso" runat="server" Visible="false" HorizontalAlign="Justify">
			<b>Atenção, não é necessária a solicitação de autorização prévia nesta opção,
			a análise será realizada quando da entrega da solicitação de reembolso.<br />
			Em caso de dúvidas acione o setor de reembolso<br />
			E-mail: reembolso@e-vida.org.br<br />
			Telefone: (61) 2099-8399
			selecionar a opção de falar no reembolso)</b>
		</asp:Panel>
		</div>
    <div id="conteudo" runat="server" clientidmode="Static" visible="false">
        <asp:HiddenField ID="litProtocolo" runat="server" />
		<table id="tbCabecalho" class="tbBorda" border="1">
            <tr>
				<td>
                    <asp:Label ID="lblProtocolo" runat="server" Text="Protocolo" Font-Bold="true" /><br />
				    <asp:Label ID="txtProtocolo" runat="server" Width="150px"/>
                </td>
				<td>
                    <b>Protocolo ANS</b><br />
				    <asp:Label ID="txtProtocoloANS" runat="server" Width="150px"/>
                </td>
				<td>
                    <asp:Label ID="lblNroAutorizacao" runat="server" Text="Nº Autorização TISS" Font-Bold="true" /><br />
					<asp:Repeater ID="rptAutorizacaoTiss" runat="server" Visible="false">
						<ItemTemplate><asp:LinkButton ID="lnkAutorizacaoTiss" runat="server"/></ItemTemplate>
						<SeparatorTemplate>, </SeparatorTemplate>
				    </asp:Repeater>
                </td>
			</tr>
			<tr>
			    <td colspan="2">
				    <asp:Label ID="lblDataSolicitacao" runat="server" Text="Data de Solicitação" Font-Bold="true" /><br />
				    <asp:Label ID="txtDataSolicitacao" runat="server" Width="150px"/>
                </td>
                <td>
                    <asp:Label ID="lblDataAutorizacao" runat="server" Text="Data de Autorização" Font-Bold="true" /><br />
				    <asp:Label ID="txtDataAutorizacao" runat="server" Width="150px"/>					
                </td>
            </tr>
			<tr>
			    <td colspan="2">
				    <asp:Label ID="lblDataSolRevalidacao" runat="server" Text="Data de Sol. da Revalidação" Font-Bold="true" /><br />
				    <asp:Label ID="txtDataSolRevalidacao" runat="server" Width="150px"/>
                </td>
                <td>
                    <asp:Label ID="lblDataAprovRevalidacao" runat="server" Text="Data de Autorização Revalidação" Font-Bold="true" /><br />
				    <asp:Label ID="txtDataAprovRevalidacao" runat="server" Width="150px"/>
                </td>
            </tr>
            <tr>
                <td colspan="3">
				    <asp:Label ID="lblStatus" runat="server" Text="Status" Font-Bold="true" /><br />
				    <asp:Label ID="txtStatus" runat="server" /> 
					<asp:LinkButton ID="lnkNeg" runat="server" Visible="false" Text="Negativa" />
					<asp:Label ID="lblMotivo" runat="server" Visible="false" />
                </td>
            </tr>
        </table>

        <h2>1 - Dados do Beneficiário</h2>
	    <table id="tbTitular" border="1">
            <tr>
                <td>
				    <asp:Label ID="lblDependente" runat="server" Text="Nome do Beneficiário *" Font-Bold="true" /><br />
				    <asp:DropDownList ID="dpdDependente" runat="server" DataValueField="Cdusuario" DataTextField="Nomusr" OnSelectedIndexChanged="dpdDependente_SelectedIndexChanged" AutoPostBack="true" />
                </td>
				<td>
                    <asp:Label ID="lblNumCartao" runat="server" Text="Número do Cartão" Font-Bold="true" /><br />
				    <asp:Label ID="txtNumCartao" runat="server" Width="150px"/>
                </td>
			</tr>
			<tr>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Titular" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
                <td>
                    <asp:Label ID="lblPlano" runat="server" Text="Plano Vinculado" Font-Bold="true" /><br />
				    <asp:Label ID="txtPlano" runat="server" Width="150px"/>
                </td>
            </tr>
        </table>
        <br />
        
        <h2>2 - DADOS DO ESTABELECIMENTO OU PROFISSIONAL SOLICITANTE</h2>
	    <table id="tbCredenciado" class="tbBorda" border="1">
            <tr>
				<td>
					<asp:HiddenField ID="hidCredenciado" runat="server" />
                    <asp:Label ID="lblCpfCnpj" runat="server" Text="CPF/CNPJ" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtCpfCnpj" runat="server" Width="190px" Enabled="false"/>
					<asp:ImageButton ID="btnLocCred" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopCred(this)" OnClick="btnLocCred_Click" />
					<asp:ImageButton ID="btnClrCred" runat="server" ImageUrl="~/img/remove.png" OnClick="btnClrCred_Click" />
                </td>
			    <td colspan="2">
				    <asp:Label ID="lblRazaoSocial" runat="server" Text="Nome/Razão Social" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtRazaoSocial" runat="server" Width="450px" Enabled="false"/>
                </td>
			</tr>
            <tr>
                <td>
                    <asp:HiddenField ID="hidProfissional" runat="server" />
					<asp:Label ID="lblConselho" runat="server" Text="Nº do Conselho *" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtNroConselho" runat="server" Width="150px" Enabled="false"/>
					<asp:ImageButton ID="btnLocProf" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopProf(this)" OnClick="btnLocProf_Click" />
					<asp:ImageButton ID="btnClrProf" runat="server" ImageUrl="~/img/remove.png" OnClick="btnClrProf_Click" />
                </td>
				<td>
                    <asp:Label ID="lblTipoConselho" runat="server" Text="Tipo Conselho/UF *" Font-Bold="true" Enabled="false"/><br />
				    <asp:DropDownList ID="dpdTipoConselho" runat="server" Width="120px" DataValueField="Key" DataTextField="Key" Enabled="false"/>
					<asp:DropDownList ID="dpdUfConselho" runat="server" Width="60px" DataValueField="Sigla" DataTextField="Sigla" Enabled="false"/>
                </td>
                <td>
				    <asp:Label ID="lblNomeProfissional" runat="server" Text="Nome do Profissional *" Font-Bold="true"/><br />
				    <asp:TextBox ID="txtNomeProfissional" runat="server" Width="350px" Enabled="false"/>
                </td>
            </tr>
        </table>
		
        <h2>3 - DADOS DA SOLICITACAO</h2>
	    <table id="tbSolicitacao" class="tbBorda" border="1">
            <tr>
				<td>
                    <asp:Label ID="lblTipo" runat="server" Text="Tipo da Autorização *" Font-Bold="true" /><br />
				    <asp:DropDownList ID="dpdTipo" runat="server">
						<asp:ListItem Value="" Text="-" />
						<asp:ListItem Value="MEDICA" Text="MÉDICA" />
						<asp:ListItem Value="ODONTO" Text="ODONTOLÓGICA" />
				    </asp:DropDownList>
                </td>
				<td>
                    <asp:Label ID="lblCarater" runat="server" Text="Caráter da Solicitação" Font-Bold="true" /><br />
				    <asp:DropDownList ID="dpdCarater" runat="server">
						<asp:ListItem Value="" Text="-" />
						<asp:ListItem Value="ELETIVA" Text="ELETIVA" />
						<asp:ListItem Value="URGENCIA" Text="URGENCIA" />
				    </asp:DropDownList>
                </td>
			    <td>
				    <asp:Label ID="lblInternacao" runat="server" Text="Internacao *" Font-Bold="true" /><br />
				    <asp:DropDownList ID="dpdInternacao" runat="server">
						<asp:ListItem Value="" Text="-" />
						<asp:ListItem Value="S" Text="SIM" />
						<asp:ListItem Value="N" Text="NÃO" />
				    </asp:DropDownList>
                </td>
			</tr>
            <tr>
                <td>
                    <asp:Label ID="lblDoenca" runat="server" Text="CID 10" Font-Bold="true" /><br />
					<asp:HiddenField ID="hidDoenca" runat="server" />
				    <asp:TextBox ID="txtDoenca" runat="server" Width="150px" Enabled="false"/>
					<asp:ImageButton ID="btnLocDoenca" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopDoenca(this)" />
					<asp:ImageButton ID="btnClrDoenca" runat="server" ImageUrl="~/img/remove.png" OnClick="btnClrDoenca_Click" />
                </td>
				<td>
                    <asp:Label ID="lblDataInternacao" runat="server" Text="Data Provável de Internação" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtDataInternacao" runat="server" Width="80px" CssClass="calendario" />
                </td>
                <td>
					<asp:HiddenField ID="hidHospital" runat="server" />
				    <asp:Label ID="lblHospital" runat="server" Text="Hospital/Clínica *" Font-Bold="true" />
					<asp:ImageButton ID="btnLocHospital" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopHosp(this)" OnClick="btnLocHospital_Click" />
					<asp:ImageButton ID="btClrHospital" runat="server" ImageUrl="~/img/remove.png" OnClick="btClrHospital_Click" /><br />
				    <asp:TextBox ID="txtHospital" runat="server" Width="200px" Enabled="false" />
                </td>
            </tr>
			<tr>
				<td colspan="3">
					<asp:Label ID="lblIndicacao" runat="server" Text="Indicação Clínica" Font-Bold="true"/><br />
					<asp:TextBox ID="txtIndicacao" runat="server" Width="90%" TextMode="MultiLine" Rows="3" />
				</td>
			</tr>
	        <tr>
		        <td>
			        <asp:Label ID="lblTfd" runat="server" Text="TFD" Font-Bold="true" />*<br />
			        <asp:DropDownList ID="dpdTfd" runat="server">
				        <asp:ListItem Value="" Text="-" />
				        <asp:ListItem Value="S" Text="SIM" />
				        <asp:ListItem Value="N" Text="NÃO" />
			        </asp:DropDownList>
		        </td>
		        <td>
			        <asp:Label ID="lblDataInicioTfd" runat="server" Text="Data do Início do TFD" Font-Bold="true" /><br />
			        <asp:TextBox ID="txtDataInicioTfd" runat="server" Width="80px" CssClass="calendario" />
		        </td>
		        <td>
			        <asp:Label ID="lblDataTerminoTfd" runat="server" Text="Data do Término do TFD" Font-Bold="true" /><br />
			        <asp:TextBox ID="txtDataTerminoTfd" runat="server" Width="80px" CssClass="calendario" />
		        </td>                                                
	        </tr>
        </table>
		<br /><br />
		<h2>4 - Solicitações médicas *</h2>
		<table width="100%" class="tbBorda" border="1">
			<tr>
				<td><b>Anexar as solicitações médicas - Pedido médico, Guias TISS, Laudos, etc.</b></td>
				<td rowspan="3">Formatos aceitos: <asp:Literal id="ltFormatoUpload" runat="server" />
					<br />
					Tamanho máximo de cada arquivo: <asp:Literal id="ltTamUpload" runat="server" />MB
				</td>
			</tr>
			<tr>
				<td>					
				<asp:ListView ID="ltvArquivos" runat="server" ItemPlaceholderID="contentID" ItemType="eVidaBeneficiarios.Forms.Autorizacao+ArquivoTela">
					<LayoutTemplate>
						<table class="tbArquivo">
							<tr>
								<th></th>
								<th>Nome</th>						
							</tr>
							<tr id="contentID" runat="server"/>
						</table>
					</LayoutTemplate>
					<ItemTemplate>
						<tr>
							<td><asp:ImageButton ID="btnRemoverArquivo" runat="server" 
								ImageUrl="~/img/remove.png" OnClick="btnRemoverArquivo_Click" 
								CommandArgument='<%# Item.NomeSalvar %>'
								Visible='<%# CanEdit || Item.New %>'/></td>
							<td><a href="javascript:void(0)" onclick="return openDownload('<%# Item.NomeSalvar %>', <%# Item.New.ToString().ToLower() %>);">								
								<%# Item.NomeSalvar %><%# Item.New ? " - Novo " : "" %>
								</a></td>
						</tr>
					</ItemTemplate>
				</asp:ListView>
				</td>
			</tr>
			<tr>
				<td>
					<asp:HiddenField ID="hidArqFisico" runat="server" />
					<asp:HiddenField ID="hidArqOrigem" runat="server" />
					<asp:Button ID="btnIncluirArquivo" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArquivo_Click" />
					<asp:Button ID="btnEnviarDoc" runat="server" Text="Reenviar formulário" OnClientClick="return confirmReenvio()" OnClick="btnEnviarDoc_Click" />
				</td>
			</tr>
			<tr id="trMensagemSolDoc" runat="server" visible="false">
				<td colspan="2">
					<p><b>Atenção ao solicitado:</b> <asp:Literal ID="txtMensagemSolDoc" runat="server" /></p>
					<b>Últimas solicitações:</b><br />
					<asp:GridView ID="gdvSolDoc" runat="server" AutoGenerateColumns="false" 
						Width="100%" CssClass="tabela"
						ItemType="eVidaGeneralLib.VO.AutorizacaoSolDocVO">
						<Columns>
							<asp:BoundField HeaderText="Data/Hora" DataField="Data" />
							<asp:BoundField HeaderText="Usuário" DataField="NomUsuario" />
							<asp:BoundField HeaderText="Mensagem" DataField="MensagemSolDoc" ItemStyle-Width="70%" />
						</Columns>
					</asp:GridView>
					<br />
				</td>
			</tr>
		</table>
		<br /><br />
		<table class="tbBorda" border="1">
			<tr>
				<td><b>Observação</b><br />
					<asp:TextBox ID="txtObservacao" runat="server" TextMode="MultiLine" MaxLength="1400" Rows="4" Width="90%" />
				</td>
			</tr>
		</table>
		<br /><br />
        <div>
            <table width="50%">
                <tr>					
                    <td align="center">
						* Campos obrigatórios<br />
						<asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" OnClientClick="return confirmSave()" /></td>
                </tr>
            </table>
        </div>
		<br />

	</div>
</asp:Content>
