<%@ Page Title="Indisponibilidade de Rede" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="IndisponibilidadeRede.aspx.cs" Inherits="eVidaIntranet.Forms.IndisponibilidadeRede" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var POP_ARQUIVO = 2;
        var POP_COMPROVANTE = 7;

        var currentUpload = 0;

        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);

            setMaskCelular('<%= txtTelContato.ClientID %>');

            var formatPreco = {
                prefix: '',
                centsSeparator: ',',
                thousandsSeparator: '.',
                centsLimit: 2,
                clearPrefix: true,
                allowNegative: false
            };

            $('#<%= txtValor.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtValorFinanceiro.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtValorExecucao.ClientID %>').priceFormat(formatPreco);


            $('#<%= txtOrcValor1.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtOrcValor2.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtOrcValor3.ClientID %>').priceFormat(formatPreco);


        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.INDISPONIBILIDADE_REDE.Value %>';
                    currentUpload = POP_ARQUIVO;
                    break;
                case POP_COMPROVANTE: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.INDISPONIBILIDADE_REDE.Value %>';
                    currentUpload = POP_COMPROVANTE;
                    break;
            }
            setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
            switch (handler.tipo) {
                case POP_ARQUIVO: break;
                case POP_COMPROVANTE: break;
                case POP_CREDENCIADO:
                    $("#" + handler.id).val(response);
                    closeLocator();
                    __doPostBack(handler.btn.name, '');
                    break;
            }
        }

        function onAfterUpload(url, originalName) {
            $("#<%:hidArqFisico.ClientID%>").val(url);
            $("#<%:hidArqOrigem.ClientID%>").val(originalName);
            closeLocator();
            if (currentUpload == POP_ARQUIVO) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirArquivo, "") %>
            } else if (currentUpload == POP_COMPROVANTE) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirComprovante, "") %>
            } else {
                alert('Upload inválido');
            }
        }


        function openArquivo() {
            var handler = new LocatorHandler(POP_ARQUIVO);
            openLocator("Arquivos", handler);
            return false;
        }

        function openComprovante() {
            var handler = new LocatorHandler(POP_COMPROVANTE);
            openLocator("Comprovante", handler);
            return false;
        }
        function openDownload(idIndisponibilidade, fId, isNew) {
            if (isNew) {
                alert('Arquivos Novos não podem ser visualizados. O formulário deve ser salvo para disponibilizar a visualização!');
                return false;
            }
            openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.INDISPONIBILIDADE_REDE %>', "ID=" + idIndisponibilidade + ";" + fId);
            return false;
        }
        
        function confirmExclusaoArquivo() {
            return confirm("Deseja realmente retirar este arquivo?");
        }
    </script>
    <style type="text/css">
        input, textarea {
            text-transform: uppercase;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo">        
		<asp:HiddenField ID="hidArqFisico" runat="server" />
		<asp:HiddenField ID="hidArqOrigem" runat="server" />
        <h2>1 - Informações Gerais</h2>
	    <table id="tbInfoGeral" border="1" class="tabelaForm" style="width:950px">
            <tr>
                <td>
				    <b>Protocolo</b><br />
				    <asp:Label ID="lblProtocolo" runat="server"/>
                </td>
                <td>
				    <b>Protocolo ANS</b><br />
				    <asp:Label ID="lblProtocoloANS" runat="server"/>
                </td>
                <td>
                    <b>Data de Solicitação</b><br />
				    <asp:Label ID="lblDataSolicitacao" runat="server"/>
                </td>
                <td>
                    <b>Prazo Estimado</b><br />
                    <asp:Label ID="lblPrazo" runat="server" />
                    <asp:Label ID="lblAtraso" runat="server" ForeColor="Red" Font-Bold="true" Visible="false" Text=" - EM ATRASO" />
                </td>
            </tr>
            <tr>
                <td>
				    <b>Situação</b><br />
				    <asp:Label ID="lblSituacao" runat="server"/>
                </td>
                <td>
				    <b>Setor Atual/Encaminhado</b><br />
				    <asp:Label ID="lblSetor" runat="server"/>
                </td>
                <td colspan="2">
				    <b>Responsável Atual</b><br />
				    <asp:Label ID="lblResponsavel" runat="server"/>
                    <asp:Button ID="btnAssumir" runat="server" Text="Assumir Solicitação" Visible="false" OnClick="btnAssumir_Click" />
                    <asp:Button ID="btnEncaminhar" runat="server" Text="Encaminhar Solicitação" Visible="false" OnClick="btnEncaminhar_Click" />
                    <asp:DropDownList ID="dpdSetor" runat="server" Visible="false" />
                </td>
            </tr>
            <tr>
                <td style="text-align:center" colspan="4">
                    <b>Histórico de Situação</b><br />
                    <asp:GridView ID="gdvHistorico" runat="server" AutoGenerateColumns="false" CssClass="tabela" Width="600px" OnRowDataBound="gdvHistorico_RowDataBound" HorizontalAlign="Center">
                        <Columns>
                            <asp:BoundField HeaderText="Data/Hora" DataField="DataHistorico" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                            <asp:BoundField HeaderText="Situação" DataField="StatusDestino" />
                            <asp:BoundField HeaderText="Ação" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <br />
        <asp:Panel ID="pnlSolicitacao" runat="server" DefaultButton="btnSalvarSolicitacao">
            <h2>2 - Dados da Solicitação</h2>
	        <table id="tbBeneficiario" border="1" class="tabelaForm" style="width:950px">
                <tr>
                    <td>
				        <asp:Label ID="lblFuncionario" runat="server" Text="Cartão Titular *" Font-Bold="true" /><br />
				        <asp:TextBox ID="txtCartao" runat="server" OnTextChanged="txtCartao_TextChanged" AutoPostBack="true" Width="160px"/>
                    </td>
                    <td>
				        <asp:Label ID="lblDependente" runat="server" Text="Nome do Beneficiário *" Font-Bold="true" /><br />
				        <asp:DropDownList ID="dpdBeneficiario" runat="server" DataValueField="Cdusuario" DataTextField="Nomusr" />
                    </td>
                    <td>
                        <b>E-mail *</b><br />
                        <asp:TextBox ID="txtEmail" runat="server" Width="300px" MaxLength="200" />
                    </td>
                    
                </tr>
                <tr>
                    <td>
                        <b>Telefone de Contato *</b><br />
                        <asp:TextBox ID="txtTelContato" runat="server" Width="180px" MaxLength="20" />
                    </td>
                    <td><b>UF *</b> <br />
                        <asp:DropDownList ID="dpdUf" runat="server" DataValueField="Sigla" DataTextField="Nome" AutoPostBack="true" OnSelectedIndexChanged="dpdUf_SelectedIndexChanged" />
                    </td>
                    <td><b>Município *</b><br />
                        <asp:DropDownList ID="dpdMunicipio" runat="server" DataValueField="BID_CODMUN" DataTextField="BID_DESCRI" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3"><h2>DADOS DO ESTABELECIMENTO E PROFISSIONAL / PRESTADOR</h2>
                    </td>
                </tr>
                <tr>                    
                    <td colspan="2">
                        <b>Especialidade *</b><br />
                        <asp:DropDownList ID="dpdEspecialidade" runat="server" DataValueField="Id" DataTextField="Nome" OnSelectedIndexChanged="dpdEspecialidade_SelectedIndexChanged" AutoPostBack="true" />
                    </td>
                    <td>
                        <b>Serviço *</b><br />
                        <asp:DropDownList ID="dpdPrioridade" runat="server" Width="300px" />
                    </td>
                    
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblCpfCnpj" runat="server" Text="CPF/CNPJ" Font-Bold="true" /><br />
				        <asp:TextBox ID="txtCpfCnpj" runat="server" Width="170px"/>
                    </td>
			        <td>
				        <asp:Label ID="lblRazaoSocial" runat="server" Text="Nome/Razão Social" Font-Bold="true" /><br />
				        <asp:TextBox ID="txtRazaoSocial" runat="server" Width="350px"/>
                    </td>
                    <td>
                        <b>Valor:<br /></b>
                        <asp:TextBox ID="txtValor" runat="server" Width="200px" />
                    </td>
			    </tr>
                <tr>
                    <td colspan="2"><b>Endereço do Prestador:<br /></b>
                        <asp:TextBox ID="txtEnderecoPrestador" runat="server" Width="550px" MaxLength="250" />
                    </td>
                    <td><b>Telefone do Prestador:<br /></b>
                        <asp:TextBox ID="txtTelefonePrestador" runat="server" MaxLength="15" Width="150px" />
                    </td>
                </tr>
                <tr id="trObsNovo" runat="server">
                    <td colspan="3">
                        <asp:TextBox ID="txtObsNovo" runat="server" Width="700px" TextMode="MultiLine"/>
                    </td>
                </tr>
                <tr id="trTratativa" runat="server" visible="false">
                    <td colspan="3">
                        <h2>TRATATIVA</h2>
                        <table class="tabelaEdit" width="900px">
                            <tr>
                                <th>REFERÊNCIA</th>
                                <th>OBS</th>
                            </tr>
                            <tr>
                                <td class="colgroup"><b>GUIA MEDICO E-VIDA</b></td>
                                <td><asp:TextBox ID="txtTratativaGuia" runat="server" Width="650px" /></td>
                            </tr>
                            <tr>
                                <td class="colgroup"><b>RECIPROCIDADE</b></td>
                                <td><asp:TextBox ID="txtTratativaReciprocidade" runat="server" Width="650px" /></td>
                            </tr>
                        </table>
                        <h2>ORÇAMENTO PARTICULAR</h2>
                        <table class="tabelaEdit tabelaBorda"  width="900px">
                            <tr>
                                <th></th>
                                <th><b>CPF/CNPJ</b></th>
                                <th><b>NOME DO PRESTADOR</b></th>
                                <th><b>TELEFONE</b></th>
                                <th><b>E-MAIL</b></th>
                                <th><b>VALOR</b></th>
                            </tr>
                            <tr>
                                <td>1</td>
                                <td><asp:TextBox ID="txtOrcCnpj1" runat="server" Width="150px" /></td>
                                <td><asp:TextBox ID="txtOrcNome1" runat="server" Width="200px" /></td>
                                <td><asp:TextBox ID="txtOrcTel1" runat="server" Width="120px" /></td>
                                <td><asp:TextBox ID="txtOrcEmail1" runat="server" Width="150px" /></td>
                                <td><asp:TextBox ID="txtOrcValor1" runat="server" Width="100px" /></td>
                            </tr>
                            <tr>
                                <td>2</td>
                                <td><asp:TextBox ID="txtOrcCnpj2" runat="server" Width="150px" /></td>
                                <td><asp:TextBox ID="txtOrcNome2" runat="server" Width="200px" /></td>
                                <td><asp:TextBox ID="txtOrcTel2" runat="server" Width="120px" /></td>
                                <td><asp:TextBox ID="txtOrcEmail2" runat="server" Width="150px" /></td>
                                <td><asp:TextBox ID="txtOrcValor2" runat="server" Width="100px" /></td>
                            </tr>
                            <tr>
                                <td>3</td>
                                <td><asp:TextBox ID="txtOrcCnpj3" runat="server" Width="150px" /></td>
                                <td><asp:TextBox ID="txtOrcNome3" runat="server" Width="200px" /></td>
                                <td><asp:TextBox ID="txtOrcTel3" runat="server" Width="120px" /></td>
                                <td><asp:TextBox ID="txtOrcEmail3" runat="server" Width="150px" /></td>
                                <td><asp:TextBox ID="txtOrcValor3" runat="server" Width="100px" /></td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3"><asp:Button ID="btnSalvarSolicitacao" runat="server" Text="Salvar dados da solicitação" OnClick="btnSalvarSolicitacao_Click" /></td>
                </tr>   
                <tr id="trObsEdit" runat="server" visible="false">
                    <td colspan="3" align="center">
                        <hr />
                        <b>Observação</b> (ao beneficiário/solicitante)<br />
                        <asp:GridView ID="gdvObs" runat="server" AutoGenerateColumns="false" Width="700px" Visible="false" CssClass="tabela">
                            <Columns>
                                <asp:BoundField HeaderText="Data" DataField="DataRegistro" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Width="150px"/>
                                <asp:BoundField HeaderText="Observação" DataField="Observacao" />
                            </Columns>
                        </asp:GridView><br />
                        <asp:TextBox ID="txtObsEdit" runat="server" Width="700px" TextMode="MultiLine"/><br />
                        <asp:Button ID="btnSalvarObs" runat="server" Text="Incluir Observação" OnClick="btnSalvarObs_Click" Visible="false" />
                    </td>
                </tr>        
	        </table>
        </asp:Panel>
        <br />
        
        <asp:Panel ID="pnlAnexos" runat="server" DefaultButton="btnIncluirArquivo">
            <h2>3 - Anexos</h2>
            <table id="tbAnexo" border="1" class="tabelaForm" style="width:950px">
                <tr>
                    <td style="padding-left:50px" align="center" >
                        <asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
			                <ContentTemplate>
				                <div id="dvArquivos" runat="server">
	                                <asp:ListView ID="ltvArquivo" runat="server" ItemPlaceholderID="contentID" ItemType="eVidaGeneralLib.VO.ArquivoTelaVO">
			                            <LayoutTemplate>
				                            <table class="tabela" >
					                            <tr>
						                            <th></th>
						                            <th>Nome</th>						
					                            </tr>
					                            <tr id="contentID" runat="server"/>
				                            </table>
			                            </LayoutTemplate>
			                            <ItemTemplate>
				                            <tr>
					                            <td style="width:50px"><asp:ImageButton ID="btnRemoverArquivo" runat="server" 
						                            ImageUrl="~/img/remove.png" OnClick="bntRemoverArquivo_Click" 
						                            CommandArgument='<%# Item.NomeFisico %>'
						                            Visible='<%# Item.IsNew || btnIncluirArquivo.Visible %>'/></td>
					                            <td>
                                                    <a href="javascript:void(0)" onclick="return openDownload('<%# Id %>','<%# Item.Id %>', <%# Item.IsNew.ToString().ToLower() %>);">								
						                            <%# Item.NomeTela %><%# Item.IsNew ? " - Novo " : "" %>
						                            </a></td>
				                            </tr>
			                            </ItemTemplate>
		                            </asp:ListView>
                    
					                <asp:Button ID="btnIncluirArquivo" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArquivo_Click"  UseSubmitBehavior="false"/>
                                </div>
			                </ContentTemplate>
		                </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />      
        
        <asp:Panel ID="pnlCredenciamento" runat="server" DefaultButton="btnSalvarFinanceiro" Visible="false">
            <h2>4 - Tratativas do Credenciamento</h2>
            
	        <table id="tbCredenciamento" border="1" class="tabelaForm" style="width:950px">
                <tr>
                    <td>
                        <b>Banco</b><br />
                        <asp:DropDownList ID="dpdBanco" runat="server" Width="300px" DataValueField="Id" DataTextField="Nome" />
                    </td>
                    <td>
                        <b>Agência</b><br />
                        <asp:TextBox ID="txtAgencia" runat="server" Width="150px" />
                    </td>
                    <td>
                        <b>Conta</b><br />
                        <asp:TextBox ID="txtConta" runat="server" Width="150px" />
                    </td>
                    <td>
                        <b>Tipo</b><br />
                        <asp:DropDownList ID="dpdAvalFaturamento" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <b>Favorecido</b><br />
                        <asp:TextBox ID="txtFavorecido" runat="server" Width="400px" />
                    </td>
                    <td>
                        <b>Valor</b><br />
                        <asp:TextBox ID="txtValorFinanceiro" runat="server" Width="200px" />
                    </td>
                    <td><b>Código Serviço<br /></b>
                        <asp:TextBox ID="txtCodigoServico" runat="server" Width="200px" MaxLength="50" />
                    </td>
                </tr>    
                <tr>
                    <td align="center"  colspan="4">
                        <asp:Button ID="btnSalvarDadosBancarios" runat="server" Text="Salvar Dados Bancários" OnClick="btnSalvarDadosBancarios_Click" Visible="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="center">
                        <hr />
                        <b>Pendências</b> (interno)<br />
                        <asp:GridView ID="gdvPendencia" runat="server" AutoGenerateColumns="false" Width="700px" Visible="false" CssClass="tabela">
                            <Columns>
                                <asp:BoundField HeaderText="Data" DataField="DataRegistro" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Width="150px"/>
                                <asp:BoundField HeaderText="Obs" DataField="Observacao" />
                                <asp:BoundField HeaderText="Pendência" DataField="Pendencia" />
                            </Columns>
                        </asp:GridView><br />
                        <table>
                            <tr valign="top">
                                <td><b>Tipo de Pendência</b><br />
                                    <asp:DropDownList ID="dpdPendencia" runat="server"/>
                                </td>
                                <td><b>Texto</b><br />
                                    <asp:TextBox ID="txtPendenciaEdit" runat="server" Width="700px" TextMode="MultiLine"/> 
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:Button ID="btnSalvarPendencia" runat="server" Text="Incluir Pendência/Obs" OnClick="btnSalvarObs_Click" Visible="false" />
                    </td>
                </tr>   
            </table>
        </asp:Panel>
        <br />
        <asp:Panel ID="pnlAutorizacao" runat="server" DefaultButton="btnSalvarAutorizacao" Visible="false">
            <h2>5 - Tratativas de Autorização</h2>

	        <table id="tbAutorizacao" border="1" class="tabelaForm" style="width:950px">
                <tr>
                    <td>
                        <b>Aval</b><br />
                        <asp:DropDownList ID="dpdAvalAutorizacao" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblComplementoAutorizacao" runat="server" Text="Complemento" Font-Bold="true" /><br />
                        <asp:TextBox ID="txtComplementoAutorizacao" runat="server" Width="800px" TextMode="MultiLine" Rows="3" />
                    </td>
                </tr>
                <tr>                
                    <td style="text-align:center">
                        <asp:Button ID="btnSalvarAutorizacao" runat="server" Text="Salvar Aval Autorização" OnClick="btnSalvarAutorizacao_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <asp:Panel ID="pnlDiretoria" runat="server" DefaultButton="btnSalvarDiretoria" Visible="false">
            <h2>6 - Aval da Diretoria</h2>

	        <table id="tbDiretoria" border="1" class="tabelaForm" style="width:950px">
                <tr>
                    <td>
                        <b>Aval</b><br />
                        <asp:DropDownList ID="dpdAvalDiretoria" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblComplDiretoria" runat="server" Text="Complemento" Font-Bold="true" /><br />
                        <asp:TextBox ID="txtComplementoDiretoria" runat="server" Width="800px" TextMode="MultiLine" Rows="3" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center">
                        <asp:Button ID="btnSalvarDiretoria" runat="server" Text="Salvar Aval Diretoria" OnClick="btnSalvarDiretoria_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        

        <asp:Panel ID="pnlFinanceiro" runat="server" DefaultButton="btnSalvarFinanceiro" Visible="false">
            <h2>7 - Tratativas do Financeiro</h2>

	        <table id="tbFinanceiro" border="1" class="tabelaForm" style="width:950px">
                <tr>
                    <td colspan="4"><b>Status</b><br />
                        <asp:Label ID="lblStatusFinanceiro" runat="server" />&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <b>Complemento de Pagamento</b><br />
                        <asp:TextBox ID="txtObsFinanceiro" runat="server" TextMode="MultiLine" Rows="3" Width="400px"/>
                    </td>
                    <td colspan="2">
                        <b>Complemento de Baixa</b><br />
                        <asp:TextBox ID="txtObsFinanceiro2" runat="server" TextMode="MultiLine" Rows="3" Width="400px"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="text-align:center">
                        <asp:Button ID="btnSalvarFinanceiro" runat="server" Text="Salvar dados Financeiro" OnClick="btnSalvarFinanceiro_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="text-align:center">
                        <b>Comprovante de Pagamento</b><br />
                        <table id="tbComprovante" runat="server" visible="false" style="width:400px;" >
                            <tr>
					            <td style="width:50px"><asp:ImageButton ID="btnRemoverComprovante" runat="server" 
						            ImageUrl="~/img/remove.png" OnClick="btnRemoverComprovante_Click" 						            
						            Visible='<%# btnIncluirComprovante.Visible %>'/></td>
					            <td>
                                    <asp:HiddenField ID="hidComprovante" runat="server" />
                                    <a href="javascript:void(0)" onclick="return openDownload('<%= Id %>','<%= hidComprovante.Value %>', false);">								
						                <asp:Literal ID="ltComprovante" runat="server" />
						            </a>
					            </td>
				            </tr>
                        </table>
                        <asp:Button ID="btnIncluirComprovante" runat="server" Text="Incluir/Alterar comprovante" OnClientClick="return openComprovante()" UseSubmitBehavior="false" OnClick="btnIncluirComprovante_Click"/>
                    </td>
                </tr>
                <tr>
                    <td><b>Data de Execução</b><br />
                        <asp:TextBox ID="txtDataExecucao" runat="server" CssClass="calendario" Width="120px" MaxLength="10" Enabled="false" />
                    </td>
                    <td><b>Valor de Execução</b><br />
                        <asp:TextBox ID="txtValorExecucao" runat="server" MaxLength="15" Width="120px" Enabled="false" />
                    </td>
                    <td colspan="2"><asp:Button ID="btnExecutarCobranca" runat="server" Text="Execução efetuada" Visible="false" OnClick="btnExecutarCobranca_Click" /></td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlFaturamento" runat="server" DefaultButton="btnSalvarFaturamento" Visible="false">
            <h2>8 - Tratativas do Faturamento/Reembolso</h2>

	        <table id="tbFaturamento" border="1" class="tabelaForm" style="width:950px">
                <tr>
                    <td>
                        <b>Responsável</b><br />
                        <asp:Literal ID="litResponsavelFaturamento" runat="server" />&nbsp;
                    </td>
                    <td>
                        <b>Protocolo Gerado:</b><br />
                        <asp:TextBox ID="txtProtocoloFaturamento" runat="server" Width="200px" />
                    </td>
                    <td style="text-align:center">
                        <asp:Button ID="btnSalvarFaturamento" runat="server" Text="Salvar Faturamento/Reembolso" OnClick="btnSalvarFaturamento_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br /><br />
    </div>
</asp:Content>
