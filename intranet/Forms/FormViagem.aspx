<%@ Page Title="Viagem" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormViagem.aspx.cs" Inherits="eVidaIntranet.Forms.FormViagem" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
	<link href="../css/tabs.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        var POP_ARQUIVO = 2;
        var POP_COMPROVANTE_FINANCEIRO = 3;
        var POP_RELATORIO = 4;
        var POP_COMP_TRASLADO = 5;
        var POP_COMP_HOTEL = 6;
        var POP_CURSO = 7;
        var POP_COMP_REEMBOLSO = 8;
        var POP_CANCELAR = 10;

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

            $('#<%= txtValorPago.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtPcHospedagem.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtPcPassagem.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtHospValor.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtVooValor.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtValorAdiantamento.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtDespDetValor.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtValorCurso.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtVooHoraIda.ClientID %>').mask("99:99");
            $('#<%= txtVooHoraChegada.ClientID %>').mask("99:99");

            $('.inteiro').each(function () {
                $('#' + this.id).ForceNumericOnly();
            });
        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.SOLICITACAO_VIAGEM.Value %>';
                    currentUpload = POP_ARQUIVO;
                    break;
                case POP_COMPROVANTE_FINANCEIRO:
                case POP_COMP_TRASLADO:
                case POP_COMP_HOTEL:
                case POP_CURSO:
                    src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.SOLICITACAO_VIAGEM.Value %>';
                    currentUpload = handler.tipo;
                    break;
                case POP_RELATORIO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.SOLICITACAO_VIAGEM.Value %>';
                    currentUpload = POP_RELATORIO;
                    break;
                case POP_CANCELAR: src = '../GenPops/PopGenericCancel.aspx?tipo=VIAGEM&ID=' + handler.id;
                    break;
                case POP_COMP_REEMBOLSO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.SOLICITACAO_VIAGEM.Value %>';
                    currentUpload = POP_COMP_REEMBOLSO;
                    break;
            }
            setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
            switch (handler.tipo) {
                case POP_ARQUIVO: break;
                case POP_COMP_TRASLADO:
                case POP_CURSO:
                case POP_COMP_HOTEL: return defaultDlgCallback(handler, response);
                case POP_COMPROVANTE_FINANCEIRO: break;
                case POP_RELATORIO: break;
                case POP_CANCELAR: return defaultDlgCallback(handler, response);
                case POP_COMP_REEMBOLSO: break;
            }
        }

        function onAfterUpload(url, originalName) {
            $("#<%:hidArqFisico.ClientID%>").val(url);
            $("#<%:hidArqOrigem.ClientID%>").val(originalName);
            closeLocator();

            if (currentUpload == POP_ARQUIVO) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirArq, "") %>
            } else if (currentUpload == POP_COMPROVANTE_FINANCEIRO) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirComprovante, "") %>
            } else if (currentUpload == POP_COMP_TRASLADO || currentUpload == POP_COMP_HOTEL) {
                locatorCallback(originalName);
            } else if (currentUpload == POP_RELATORIO) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirRelatorioViagem, "") %>
            } else if (currentUpload == POP_CURSO) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirCurso, "") %>
            } else if (currentUpload == POP_COMP_REEMBOLSO) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirCompPcFinReemb, "") %>
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
    var handler = new LocatorHandler(POP_COMPROVANTE_FINANCEIRO);
    openLocator("Comprovante", handler);
    return false;
}
function openCurso() {
    var handler = new LocatorHandler(POP_CURSO);
    openLocator("Documento Curso", handler);
    return false;
}
function openComprovanteTraslado(btn, id, row) {
    var handler = new LocatorHandler(POP_COMP_TRASLADO, id, row, btn);
    openLocator("Comprovante Traslado", handler);
    return false;
}
function openComprovanteHotel(btn, id, row) {
    var handler = new LocatorHandler(POP_COMP_HOTEL, id, row, btn);
    openLocator("Comprovante Hotel", handler);
    return false;
}
function openRelatorio() {
    var handler = new LocatorHandler(POP_RELATORIO);
    openLocator("Relatório", handler);
    return false;
}
function openCompFinReemb() {
    var handler = new LocatorHandler(POP_COMP_REEMBOLSO);
    openLocator("Comprovante reembolso/devolução", handler);
    return false;
}
function openDownload(idViagem, idTipo, fId, isNew) {
    if (isNew) {
        alert('Arquivos Novos não podem ser visualizados. O formulário deve ser salvo para disponibilizar a visualização!');
        return false;
    }
    openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.SOLICITACAO_VIAGEM %>', "ID=" + idViagem + ";" + idTipo + ";" + fId);
            return false;
        }

        function confirmExclusaoArquivo() {
            return confirm("Deseja realmente retirar este arquivo?");
        }

        function confirmPrestFinanceiro() {
            return confirm("Deseja realmente confirmar a validação desta despesa?");
        }

        function goBusca() {
            window.location = './BuscaViagem.aspx';
            return false;
        }

        function openCancelar(btn, id) {
            var handler = new LocatorHandler(POP_CANCELAR, id, 0, btn);
            openLocator("Cancelar", handler);
            return false;
        }

        function openRelViagem(idViagem) {
            openReport(RELATORIO_VIAGEM, "ID=" + idViagem, false);
            return false;
        }

        function reloadPage() {
            window.location.reload(true);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo">        
		<asp:HiddenField ID="hidArqComplemento" runat="server" />
		<asp:HiddenField ID="hidArqFisico" runat="server" />
		<asp:HiddenField ID="hidArqOrigem" runat="server" />

        <h2>Informações Gerais</h2>
	    <table id="tbInfoGeral" border="1" class="tabelaForm" style="width:950px">
            <tr>
                <td rowspan="2">
				    <b>Protocolo</b><br />
				    <asp:Label ID="lblProtocolo" runat="server"/>
                </td>
                <td>
                    <b>Data de Solicitação</b><br />
				    <asp:Label ID="lblDataSolicitacao" runat="server"/>
                </td>
                <td>
                    <b>Solicitado por</b><br />
                    <asp:Label ID="lblSolicitadoPor" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Data da Situação</b><br />
				    <asp:Label ID="lblDataSituacao" runat="server"/>
                </td>
                 <td>
				    <b>Situação</b><br />
				    <asp:Label ID="lblSituacao" runat="server"/>
                </td> 
            </tr>
        </table>
        <br />

        <table id="tabDetail" runat="server" class="tabelaTab" >
			<tr class="tabHeader">
				<td>
					<asp:Button Text="Solicitação" BorderStyle="None" ID="tabSolicitacao" CssClass="tabSelected" runat="server" OnClick="Tab_Click" />
                    <asp:Button Text="Info Compra / Pagamento" BorderStyle="None" ID="tabCompra" CssClass="tabNoSelected" runat="server" OnClick="Tab_Click" Enabled="false" />
                    <asp:Button Text="Prestação de Contas" BorderStyle="None" ID="tabPrestacaoConta" CssClass="tabNoSelected" runat="server" OnClick="Tab_Click" Enabled="false" />
				</td>
			</tr>
			<tr>
				<td class="tabContent">
                    <asp:MultiView ID="mtvViagem" runat="server">

                        <asp:View ID="vwSolicitacao" runat="server">
                            <asp:Panel ID="pnlSolicitacao" runat="server" DefaultButton="btnSalvarSolicitacao">
                                <h2>1 - Dados da Solicitação</h2>
	                            <table id="tbFuncionario" border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td><b>Tipo de solicitação:</b><br />
                                            <asp:DropDownList ID="dpdTipoSolicitacao" runat="server" OnSelectedIndexChanged="dpdTipoSolicitacao_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="0" Text="INTERNA" />
                                            <asp:ListItem Value="1" Text="EXTERNA" />
                                            </asp:DropDownList></td>
                                        <td colspan="2"><b>Tipo de Viagem:</b><br />
                                            <asp:DropDownList ID="dpdTipoViagem" runat="server">
                                                <asp:ListItem Value="" Text="SELECIONE" />
                                                <asp:ListItem Value="1" Text="CURSO" />
                                                <asp:ListItem Value="2" Text="VISITAS" />
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chkUsaDiaria" runat="server" Text="Diárias com valor?" Checked="true" Enabled="false" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
				                            <asp:Label ID="lblFuncionario" runat="server" Text="Matrícula" Font-Bold="true" CssClass="inteiro" /><br />
				                            <asp:TextBox ID="txtMatricula" runat="server" OnTextChanged="txtMatricula_TextChanged" AutoPostBack="true" Width="100px"/>
                                        </td>
                                        <td colspan="3">
                                            <b>Nome</b><br />
                                            <asp:TextBox ID="txtNomeFuncionario" runat="server" Width="600px" MaxLength="200" Enabled="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>CPF</b><br />
                                            <asp:TextBox ID="txtCpf" runat="server" Width="120px" MaxLength="11" Enabled="false" />
                                        </td>
                                        <td>
                                            <b>RG</b><br />
                                            <asp:TextBox ID="txtRg" runat="server" Width="180px" MaxLength="20" Enabled="false" />
                                        </td>
                                        <td colspan="2">
                                            <b>Data de Nascimento</b><br />
                                            <asp:TextBox ID="txtDataNascimento" runat="server" Width="120px" MaxLength="11" CssClass="calendario" Enabled="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Telefone de Contato</b><br />
                                            <asp:TextBox ID="txtTelContato" runat="server" Width="150px" MaxLength="20" />
                                        </td>
                                        <td>
                                            <b>Ramal</b><br />
                                            <asp:TextBox ID="txtRamal" runat="server" Width="120px" MaxLength="20" />
                                        </td>
                                        <td colspan="2">
                                            <b>Cargo</b><br />
                                            <asp:TextBox ID="txtCargo" runat="server" Width="250px" MaxLength="50" Enabled="false" />
                                        </td>
                    
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>Objetivo da Viagem</b><br />
                                            <asp:TextBox ID="txtObjetivoViagem" runat="server" Width="850px" TextMode="MultiLine" Rows="3" />
                                        </td>
                                    </tr>
                                    <tr id="trCurso" runat="server" visible="true">
                                        <td colspan="4">
                                            <b>Solicitação/Aprovação para curso</b><br />
                                            <table id="tbCurso" runat="server" visible="false" >
                                                <tr>
					                                <td style="width:50px"><asp:ImageButton ID="btnRemoverCurso" runat="server" 
						                                ImageUrl="~/img/remove.png" OnClick="btnRemoverCurso_Click" Visible="false"/></td>
					                                <td>
                                                        <asp:HiddenField ID="hidCurso" runat="server" />
                                                        <a href="javascript:void(0)" onclick="return openDownload('<%= Id %>', '<%= ((int)eVidaGeneralLib.VO.TipoArquivoViagem.CURSO) %>', '<%= hidCurso.Value %>', <%= (Id == null).ToString().ToLower() %>);">								
						                                    <asp:Literal ID="ltCurso" runat="server" />
						                                </a>
					                                </td>
				                                </tr>
                                            </table>
                                            <asp:Button ID="btnIncluirCurso" runat="server" Text="Alterar documento curso" OnClientClick="return openCurso()" UseSubmitBehavior="false" OnClick="btnIncluirCurso_Click" Visible="false" />
                                        </td>
                                    </tr>
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
                                            <b>Adiantamento</b><br />
                                            <asp:TextBox ID="txtValorAdiantamento" runat="server" Width="150px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>Justificativa para o adiantamento</b><br />
                                            <asp:TextBox ID="txtJusAdiantamento" runat="server" TextMode="MultiLine" Width="850px" Rows="2" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>Meios de Transporte desejados</b><br />
                                            <asp:CheckBoxList ID="chkMeioTransporte" runat="server" Width="750px" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="1" Text="AÉREO" />
                                                <asp:ListItem Value="2" Text="TERRESTRE" />
                                                <asp:ListItem Value="99" Text="OUTROS" />
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>Itinerários</b><br />
                                            <table id="tbFormItinerarios" runat="server">
                                                <tr>
                                                    <td><b>De:</b></td>
                                                    <td><asp:TextBox ID="txtSolDe" runat="server" Width="400px" /></td>
                                                    <td><b>Data Ida:</b></td>
                                                    <td><asp:TextBox ID="txtSolDataIda" runat="server" Width="100px" CssClass="calendario" /></td>
                                                    <td rowspan="2">
                                                        <asp:Button ID="btnAddItinerario" runat="server" Text="Incluir" OnClick="btnAddItinerario_Click" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td><b>Para:</b></td>
                                                    <td><asp:TextBox ID="txtSolPara" runat="server" Width="400px" /></td>
                                                    <td><b>Data Volta:</b></td>
                                                    <td><asp:TextBox ID="txtSolDataVolta" runat="server" Width="100px" CssClass="calendario" /></td>
                                                </tr>
                                            </table><br />
                                            <asp:GridView ID="gdvItinerario" runat="server" ItemType="eVidaGeneralLib.VO.SolicitacaoViagemItinerarioVO"
                                                AutoGenerateColumns="false" CssClass="tabela">
                                                <EmptyDataTemplate>
                                                    <b>Sem itinerários cadastrados no momento</b>
                                                </EmptyDataTemplate>
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hidDe" runat="server" Value='<%# Item.Origem %>' />
                                                            <asp:HiddenField ID="hidPara" runat="server" Value='<%# Item.Destino %>'  />
                                                            <asp:HiddenField ID="hidDataIda" runat="server" Value='<%# Item.DataPartida %>'  />
                                                            <asp:HiddenField ID="hidDataVolta" runat="server" Value='<%# Item.DataRetorno %>'  />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="DE" DataField="Origem" />
                                                    <asp:BoundField HeaderText="PARA" DataField="Destino" />
                                                    <asp:BoundField HeaderText="DATA IDA" DataField="DataPartida" DataFormatString="{0:dd/MM/yyyy}" />
                                                    <asp:BoundField HeaderText="DATA VOLTA" DataField="DataRetorno" DataFormatString="{0:dd/MM/yyyy}" />
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btnRemoverItinerario" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemoverItinerario_Click" Visible='<%# btnSalvarSolicitacao.Visible %>'  />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="4">
                                            <asp:Button ID="btnCancelar" runat="server" Text="CANCELAR SOLICITAÇÃO" OnClick="btnCancelar_Click" Visible="false" />
                                            <asp:Button ID="btnSalvarSolicitacao" runat="server" Text="ENVIAR SOLICITAÇÃO" OnClick="btnSalvarSolicitacao_Click" Visible="false" />
                                        </td>
                                    </tr>          
	                            </table>
                            </asp:Panel>
                            <br />
                            <asp:Panel ID="pnlAvalSolCoordenador" runat="server" DefaultButton="btnSalvarSolCoordenador" Enabled="false">
                                <h2>2 - Aval do Superior Imediato</h2>
	                            <table id="tbSolCoordenador" border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td><b>Superior imediato:</b><br />
                                            <asp:Label ID="lblSuperiorImediato" runat="server" />
                                        </td> 
                                        <td>
                                            <b>Aval</b><br />
                                            <asp:DropDownList ID="dpdAvalSolCoordenador" runat="server">
                                                <asp:ListItem Text="SELECIONE" Value="" />
                                                <asp:ListItem Text="APROVADO" Value="S" />
                                                <asp:ListItem Text="REPROVADO" Value="N" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <b>Justificativa</b><br />
                                            <asp:TextBox ID="txtJusSolCoordenador" runat="server" Width="800px" TextMode="MultiLine" Rows="3" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center">
                                            <asp:Button ID="btnSalvarSolCoordenador" runat="server" Text="Salvar Aval Coordenador" Visible="false" OnClick="btnSalvarSolCoordenador_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <br />
                            <asp:Panel ID="pnlAvalSolDiretoria" runat="server" DefaultButton="btnSalvarSolDiretoria" Enabled="false">
                                <h2>3 - Aval da Diretoria</h2>
	                            <table id="tbSolDiretoria" border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td><b>Diretor responsável:</b><br />
                                            <asp:Label ID="lblDiretorResponsavelSol" runat="server" />
                                        </td>      
                                        <td>
                                            <b>Aval</b><br />
                                            <asp:DropDownList ID="dpdAvalSolDiretoria" runat="server">
                                                <asp:ListItem Text="SELECIONE" Value="" />
                                                <asp:ListItem Text="APROVADO" Value="S" />
                                                <asp:ListItem Text="REPROVADO" Value="N" />
                                            </asp:DropDownList>
                                        </td>                              
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <b>Justificativa</b><br />
                                            <asp:TextBox ID="txtJusSolDiretoria" runat="server" Width="800px" TextMode="MultiLine" Rows="3" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center">
                                            <asp:Button ID="btnSalvarSolDiretoria" runat="server" Text="Salvar Aval Diretoria" OnClick="btnSalvarSolDiretoria_Click" Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </asp:View>
                    
                        <asp:View ID="vwCompra" runat="server">

                            <asp:Panel ID="pnlComplementar" runat="server" DefaultButton="btnSalvarComplementar" Enabled="false">
                                <h2>1 - Informações Complementares</h2>
	                            <table id="tbInfoComplementar" border="1" class="tabelaForm" style="width:950px" cellpadding="5px">
                                    <tr>
                                        <td>
                                            <b>Dados dos Traslados</b><br /><br />
                                            <table>
                                                <tr>
                                                    <td style="width:150px; text-align:right"><b>Meio de Transporte:</b></td>
                                                    <td><asp:DropDownList ID="dpdMeioTransporte" runat="server" OnSelectedIndexChanged="dpdMeioTransporte_SelectedIndexChanged" AutoPostBack="true"/></td>
                                                    <td style="width:170px; text-align:right"><b><asp:Literal ID="litValorTraslado" runat="server" Text="Valor do traslado" />:</b></td>
                                                    <td><asp:TextBox ID="txtVooValor" runat="server" Width="100px"/></td>
                                                    <td rowspan="3">
                                                        <asp:Button ID="btnIncluirVoo" runat="server" Text="Incluir" Visible="false" OnClick="btnAddVoo_Click" />
                                                    </td> 
                                                </tr>
                                                <tr>
                                                    <td style="text-align:right"><b>Origem:</b></td>
                                                    <td><asp:TextBox ID="txtVooOrigem" runat="server" Width="250px" /></td>
                                                    <td style="text-align:right"><b>Data/Hora de Partida:</b></td>
                                                    <td style="width:350px"><asp:TextBox ID="txtVooDataPartida" runat="server" CssClass="calendario" Width="100px" />  <asp:TextBox ID="txtVooHoraIda" runat="server" Width="100px" /></td>
                                                </tr>
                                                <tr>            
                                                    <td style="text-align:right"><b>Destino:</b></td>
                                                    <td><asp:TextBox ID="txtVooDestino" runat="server" Width="250px" /></td>                                      
                                                    <td style="text-align:right"><b>Data/Hora de Chegada:</b></td>
                                                    <td><asp:TextBox ID="txtVooDataChegada" runat="server" CssClass="calendario" Width="100px" />  <asp:TextBox ID="txtVooHoraChegada" runat="server" Width="100px" /></td>

                                                </tr>
                                                <tr>
                                                    <td colspan="5">
                                                        <asp:GridView ID="gdvVoo" runat="server" ItemType="eVidaGeneralLib.VO.SolicitacaoViagemItinerarioVO"
                                                            AutoGenerateColumns="false" CssClass="tabela" OnRowDataBound="gdvVoo_RowDataBound" Width="950px">
                                                            <EmptyDataTemplate>
                                                                <b>Sem traslados cadastrados no momento</b>
                                                            </EmptyDataTemplate>
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidId" runat="server" Value='<%# Item.IdItinerario %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="MEIO DE TRANSPORTE">
                                                                    <ItemTemplate>
                                                                        <asp:Literal ID="litMeioTransporte" runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="ORIGEM" ItemStyle-Width="150px">
                                                                    <ItemTemplate>
                                                                        <%# Item.Origem %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="DESTINO" ItemStyle-Width="150px">
                                                                    <ItemTemplate>
                                                                        <%# Item.Destino %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="DATA DE PARTIDA">
                                                                    <ItemTemplate>
                                                                        <%# Item.DataPartida.ToString("dd/MM/yyyy HH:mm") %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="DATA DE CHEGADA">
                                                                    <ItemTemplate>
                                                                        <%# Item.DataRetorno.ToString("dd/MM/yyyy HH:mm") %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:C}" />
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="btnRemoverVoo" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemoverVoo_Click"  />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>                                
                                    <tr>
                                        <td>
                                            <b>Dados das Hospedagens</b><br /><br />
                                            <table>
                                                <tr>
                                                    <td><b>Hotel:</b></td>
                                                    <td colspan="5"><asp:TextBox ID="txtHospHotel" runat="server" /></td>
                                                    <td rowspan="2"><asp:Button ID="btnIncluirHotel" runat="server" Text="Incluir" Visible="false" OnClick="btnAddHotel_Click" /></td>
                                                </tr>
                                                <tr>
                                                    <td><b>Data de Check IN:</b></td>
                                                    <td><asp:TextBox ID="txtHospCheckIn" runat="server" CssClass="calendario"  Width="100px"/></td>
                                                    <td><b>Data de Check OUT:</b></td>
                                                    <td><asp:TextBox ID="txtHospCheckOut" runat="server" CssClass="calendario"  Width="100px"/></td>     
                                                    <td><b>Valor de hospedagem:</b></td>
                                                    <td><asp:TextBox ID="txtHospValor" runat="server" Width="100px"/></td>                                                
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <br />
                                                        <asp:GridView ID="gdvHospedagem" runat="server" ItemType="eVidaGeneralLib.VO.SolicitacaoViagemItinerarioVO"
                                                            AutoGenerateColumns="false" CssClass="tabela" OnRowDataBound="gdvHospedagem_RowDataBound">
                                                            <EmptyDataTemplate>
                                                                <b>Sem hospedagens cadastradas no momento</b>
                                                            </EmptyDataTemplate>
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidId" runat="server" Value='<%# Item.IdItinerario %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField HeaderText="Hotel" DataField="Origem" />
                                                                <asp:BoundField HeaderText="CheckIn" DataField="DataPartida" DataFormatString="{0:dd/MM/yyyy}" />
                                                                <asp:BoundField HeaderText="CheckOut" DataField="DataRetorno" DataFormatString="{0:dd/MM/yyyy}" />
                                                                <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:C}" />
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="btnRemoverHotel" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemoverHotel_Click" Visible='<%# pnlComplementar.Enabled %>'  />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Dados das Diárias</b><br /><br />
                                            <table>
                                                <tr>
                                                    <td>
                                                        <b>Total de diárias: </b><asp:TextBox ID="txtDiarias" runat="server" MaxLength="3" Width="50px" CssClass="inteiro" Enabled="false" />
                                                    </td>
                                                    <td>
                                                        <b>Valor da Diária</b> <asp:Label ID="lblValorDiaria" runat="server" />
                                                    </td>
                                                    <td>
                                                        <b>Valor Total:</b> <asp:TextBox ID="txtValorTotalDiarias" runat="server" Width="120px" Enabled="false" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                
                                    <tr>
                                        <td>
                                            <b>Dados do Curso</b><br /><br />
                                            <table>
                                                <tr>
                                                    <td>
                                                        <b>Valor do Curso: </b><asp:TextBox ID="txtValorCurso" runat="server" MaxLength="10" Width="150px" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" style="text-align:center">
                                            <asp:Button ID="btnSalvarComplementar" runat="server" Text="Salvar e Encaminhar para Financeiro" OnClick="btnSalvarComplementar_Click" Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                
                            <asp:Panel ID="pnlFinanceiro" runat="server" DefaultButton="btnSalvarFinanceiro" Enabled="false">
                                <h2>2 - Dados do Financeiro</h2>
	                            <table id="tbFinanceiro" border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <b>Adiantamento e Diárias:</b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td><b>Valor Total do Comprovante</b><br />
                                                        <asp:TextBox ID="txtValorPago" runat="server" Width="120px" /><br />
                                                        <asp:Label ID="lblNovoValorPago" runat="server" Font-Bold="true" ForeColor="Red" />
                                                    </td>
                                                    <td>
                                                        <b>Comprovante</b><br />
                                                        <table id="tbComprovante" runat="server" visible="false">
                                                            <tr>
					                                            <td style="width:50px"><asp:ImageButton ID="btnRemoverComprovante" runat="server" 
						                                            ImageUrl="~/img/remove.png" OnClick="btnRemoverComprovante_Click" Visible="false"/></td>
					                                            <td>
                                                                    <asp:HiddenField ID="hidComprovante" runat="server" />
                                                                    <a href="javascript:void(0)" onclick="return openDownload('<%= Id %>', '<%= ((int)eVidaGeneralLib.VO.TipoArquivoViagem.COMPROVANTE_PAGAMENTO_DIARIA) %>', '<%= hidComprovante.Value %>', false);">								
						                                                <asp:Literal ID="litComprovante" runat="server" />
						                                            </a>
					                                            </td>
				                                            </tr>
                                                        </table>
                                                        <asp:Button ID="btnIncluirComprovante" runat="server" Text="Incluir/Alterar comprovante" OnClientClick="return openComprovante()" UseSubmitBehavior="false" OnClick="btnIncluirComprovante_Click" Visible="false"/>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Traslados:</b>
                                            <asp:GridView ID="gdvCompFinVoo" runat="server" ItemType="eVidaGeneralLib.VO.SolicitacaoViagemItinerarioVO"
                                                AutoGenerateColumns="false" CssClass="tabela" Width="950px" DataKeyNames="IdItinerario" OnRowDataBound="gdvCompFinVoo_RowDataBound">
                                                <EmptyDataTemplate>
                                                    <b>Sem traslados cadastrados no momento</b>
                                                </EmptyDataTemplate>
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hidId" runat="server" Value='<%# Item.IdItinerario %>' />
                                                            <asp:Label ID="lblNew" runat="server" Text="NOVO" Visible="false" Font-Bold="true" ForeColor="Red" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="MEIO DE TRANSPORTE">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litMeioTransporte" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ORIGEM" ItemStyle-Width="150px">
                                                        <ItemTemplate>
                                                            <%# Item.Origem %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DESTINO" ItemStyle-Width="150px">
                                                        <ItemTemplate>
                                                            <%# Item.Destino %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DATA DE PARTIDA">
                                                        <ItemTemplate>
                                                            <%# Item.DataPartida.ToString("dd/MM/yyyy HH:mm") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DATA DE CHEGADA">
                                                        <ItemTemplate>
                                                            <%# Item.DataRetorno.ToString("dd/MM/yyyy HH:mm") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:C}" />
                                                    <asp:TemplateField HeaderText="Comprovante">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkCompVoo" runat="server" Text="Comprovante" Visible="false" />
                                                            <asp:Button ID="btnAddCompVoo" runat="server" Text="Incluir" OnClick="btnAddCompVoo_Click" UseSubmitBehavior="false" OnClientClick='<%# CreateJsFunctionGrid(Container, "openComprovanteTraslado") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr><tr>
                                        <td>
                                            <b>Hotéis:</b>
                                            <asp:GridView ID="gdvCompFinHotel" runat="server" ItemType="eVidaGeneralLib.VO.SolicitacaoViagemItinerarioVO"
                                                AutoGenerateColumns="false" CssClass="tabela" Width="950px" DataKeyNames="IdItinerario" OnRowDataBound="gdvCompFinHotel_RowDataBound">
                                                <EmptyDataTemplate>
                                                    <b>Sem hotéis cadastrados no momento</b>
                                                </EmptyDataTemplate>
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hidId" runat="server" Value='<%# Item.IdItinerario %>' />
                                                            <asp:Label ID="lblNew" runat="server" Text="NOVO" Visible="false" Font-Bold="true" ForeColor="Red" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Hotel" DataField="Origem" />
                                                    <asp:BoundField HeaderText="CheckIn" DataField="DataPartida" DataFormatString="{0:dd/MM/yyyy}" />
                                                    <asp:BoundField HeaderText="CheckOut" DataField="DataRetorno" DataFormatString="{0:dd/MM/yyyy}" />
                                                    <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:C}" />
                                                    <asp:TemplateField HeaderText="Comprovante">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkCompHotel" runat="server" Text="Comprovante" Visible="false" />
                                                            <asp:Button ID="btnAddCompHotel" runat="server" Text="Incluir/Alterar Comprovante" OnClick="btnAddCompHotel_Click" UseSubmitBehavior="false" OnClientClick='<%# CreateJsFunctionGrid(Container, "openComprovanteHotel") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center">
                                            <asp:Button ID="btnSalvarFinanceiro" runat="server" Text="Salvar dados Financeiro" OnClick="btnSalvarFinanceiro_Click" Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                            <asp:Panel ID="pnlConfirmarPagamento" runat="server" Enabled="false">
                                <h2>3 - Confirmação de Pagamento/Recebimento</h2>
                                <table border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td style="text-align:center">
                                            <asp:Label ID="lblPagamentoRecebido2" runat="server" Text="Financeiro ainda não enviou os comprovantes." />
                                            <asp:Button ID="btnPagamentoRecebido" runat="server" Text="Confirmar pagamento do valor recebido" OnClick="btnPagamentoRecebido_Click" Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <br />
                            <br />
                        </asp:View>

                        <asp:View ID="vwPrestacaoConta" runat="server">
                
                            <asp:Panel ID="pnlPrestacaoConta" runat="server" DefaultButton="btnAddDespDet" Enabled="false">
                                <h2>1 - Prestação de Conta</h2>
                                <table id="tbPrestacaoConta" border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td><b>Valor das Passagens</b><br />
                                            <asp:TextBox ID="txtPcPassagem" runat="server" Width="100px" Enabled="false" />
                                        </td>
                                        <td><b>Despesa Hospedagem</b><br />
                                            <asp:TextBox ID="txtPcHospedagem" runat="server" Width="100px" Enabled="false" />
                                        </td>
                                        <td><b>Valor de Adiantamento</b><br />
                                            <asp:TextBox ID="txtPcAdiantamento" runat="server" Width="100px" Enabled="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>Despesas Detalhadas</b><br />
                                            <table>
                                                <tr>
                                                    <td><b>Data:</b></td>
                                                    <td><asp:TextBox ID="txtDespDetData" runat="server" CssClass="calendario" Width="90px" /></td>
                                                    <td><b>Descrição:</b></td>
                                                    <td colspan="3"><asp:TextBox ID="txtDespDet" runat="server" Width="300px" /></td>
                                                    <td rowspan="2"><asp:Button ID="btnAddDespDet" runat="server" Text="Incluir" Visible="false" OnClick="btnAddDespDet_Click" /></td>
                                                </tr>
                                                <tr>
                                                    <td><b>Tipo de Despesa:</b></td>
                                                    <td>
                                                        <asp:DropDownList runat="server" ID="dpdDespDetGrupo" AutoPostBack="true" OnSelectedIndexChanged="dpdDespDetGrupo_SelectedIndexChanged" Width="200px" /></td>
                                                    <td><b>SubTipo:</b></td>
                                                    <td>
                                                        <asp:DropDownList runat="server" ID="dpdDespDetTipo" AutoPostBack="true" OnSelectedIndexChanged="dpdDespDetTipo_SelectedIndexChanged" Width="200px" /></td>
                                                    <td>
                                                        <asp:TextBox id="txtDespDetOutros" runat="server" Enabled="false" Width="200px" />
                                                    </td>
                                                </tr>
                                                <tr>                                                
                                                    <td><b>Identificador</b></td>
                                                    <td><asp:TextBox ID="txtDespDetIdent" runat="server" Width="120px" /></td>
                                                    <td><b>Valor:</b></td>
                                                    <td><asp:TextBox ID="txtDespDetValor" runat="server" Width="100px"/></td>                                                
                                                </tr>
                                                <tr>
                                                    <td colspan="7">
                                                        <br />
                                                        <asp:GridView ID="gdvDespDet" runat="server" ItemType="eVidaGeneralLib.VO.SolicitacaoViagemDespesaDetalhadaVO" ShowFooter="false"
                                                            AutoGenerateColumns="false" CssClass="tabela" OnRowDataBound="gdvDespDet_RowDataBound" Width="100%" DataKeyNames="IdDespesa"
                                                            AllowPaging="true" PageSize="10" PagerSettings-PageButtonCount="20" PagerSettings-Position="Bottom" OnPageIndexChanging="gdvDespDet_PageIndexChanging" >
                                                            <EmptyDataTemplate>
                                                                <b>Sem despesas cadastradas no momento</b>
                                                            </EmptyDataTemplate>
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidId" runat="server" Value='<%# Item.IdDespesa %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField HeaderText="Data" DataField="Data" DataFormatString="{0:dd/MM/yyyy}" />
                                                                <asp:BoundField HeaderText="Tipo" DataField="GrupoDespesa"/>                                                            
                                                                <asp:BoundField HeaderText="SubTipo" DataField="TipoDespesa"/>
                                                                <asp:BoundField HeaderText="Descricao" DataField="Descricao" />
                                                                <asp:BoundField HeaderText="Identificador" DataField="Identificador"/>
                                                                <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:C}" />                                                   
                                                                <asp:BoundField HeaderText="Data Aprov Financeiro" DataField="DataConferido" DataFormatString="{0:dd/MM/yyyy}" />
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="btnRemoverDespDet" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemoverDespDet_Click" Visible='<%# pnlPrestacaoConta.Enabled %>'  />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right">
                                                        <b>Valor Total:</b> <asp:Literal ID="litDespDetValorTotal" runat="server" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" style="padding-left:50px; text-align:center">
                                            <b>Relatório da Viagem</b><br />
                                            <table id="tbRelatorioViagem" runat="server" visible="false" style="width:90%">
                                                <tr>
					                                <td style="width:50px"><asp:ImageButton ID="btnRemoverRelatorioViagem" runat="server" 
						                                ImageUrl="~/img/remove.png" Visible="false"/></td>
					                                <td style="text-align:center">
                                                        <asp:HiddenField ID="hidRelatorioViagem" runat="server" />
                                                        <a href="javascript:void(0)" onclick="return openDownload('<%= Id %>', '<%= ((int)eVidaGeneralLib.VO.TipoArquivoViagem.RELATORIO_VIAGEM) %>','<%= hidRelatorioViagem.Value %>', false);">								
						                                    <asp:Literal ID="ltRelatorioViagem" runat="server" />
						                                </a>
					                                </td>
				                                </tr>
                                            </table>
                                            <asp:Button ID="btnIncluirRelatorioViagem" runat="server" Text="Incluir/Alterar relatório" OnClientClick="return openRelatorio()" OnClick="btnIncluirRelatorioViagem_Click" UseSubmitBehavior="false" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trUpdArq">
                                        <td colspan="4" style="padding-left:25px; text-align:center">
                                            <b>Comprovantes</b><br />
                                            <asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
			                                    <ContentTemplate>
				                                    <div id="dvArquivos" runat="server">
                                                        <asp:ListView ID="ltvArquivo" runat="server" GroupPlaceholderID="groupPlaceHolder" ItemPlaceholderID="contentID" ItemType="eVidaGeneralLib.VO.ArquivoTelaVO" GroupItemCount="2">
			                                                <LayoutTemplate>
				                                                <table class="tabela" style="width:850px" >					                                            
					                                                <tr id="groupPlaceHolder" runat="server"/>
				                                                </table>
			                                                </LayoutTemplate>
			                                                <ItemTemplate>
				                                                <td style="width:50px"><asp:ImageButton ID="btnRemoverArquivo" runat="server" 
						                                                ImageUrl="~/img/remove.png" OnClick="btnRemoverArquivo_Click" 
						                                                CommandArgument='<%# Item.NomeFisico %>'
						                                                Visible='<%# Item.IsNew || CanEditFile %>'/></td>
					                                                <td>
                                                                        <a href="javascript:void(0)" onclick="return openDownload('<%# Id %>', '<%= ((int)eVidaGeneralLib.VO.TipoArquivoViagem.COMPROVANTE_DESPESA) %>','<%# Item.Id %>', <%# Item.IsNew.ToString().ToLower() %>);">								
						                                                <%# Item.NomeTela %><%# Item.IsNew ? " - Novo " : "" %>
						                                                </a></td>
			                                                </ItemTemplate>
                                                            <GroupTemplate>
                                                                <tr runat="server" id="rows" style="background-color: #FFFFFF">
                                                                    <td runat="server" id="contentID" />
                                                                </tr>
                                                            </GroupTemplate>
                                                            <ItemSeparatorTemplate>
                                                            
                                                            </ItemSeparatorTemplate>
		                                                </asp:ListView>
                                                        <asp:Button ID="btnIncluirArq" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArquivo_Click"  UseSubmitBehavior="false" Visible="false"/>
	                                                
                                                    </div>
			                                    </ContentTemplate>
		                                    </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>Resumo da Viagem</b><br />
                                            <asp:TextBox ID="txtResumoViagem" runat="server" TextMode="MultiLine" Rows="4" Width="90%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" style="text-align:center">
                                            <asp:Button ID="btnSalvarPrestacaoConta" runat="server" Text="ENVIAR PRESTAÇÃO DE CONTAS" OnClick="btnSalvarPrestacaoConta_Click" Visible="false" />                        
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <br />

                            <asp:Panel ID="pnlAvalPcFinanceiro" runat="server" DefaultButton="btnAvalPcFinanceiro" Enabled="false">
                                <h2>2 - Conferência do Financeiro</h2>
	                            <table id="tbAvalPcFinanceiro" border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td colspan="2"><b>Despesas do Viajante a conferir:</b>
                                            <br />
                                            <asp:GridView ID="gdvDespDetFin" runat="server" ItemType="eVidaGeneralLib.VO.SolicitacaoViagemDespesaDetalhadaVO" ShowFooter="false"
                                                AutoGenerateColumns="false" CssClass="tabela" OnRowDataBound="gdvDespDetFin_RowDataBound" Width="100%" DataKeyNames="IdDespesa"
                                                AllowPaging="true" PageSize="10" PagerSettings-PageButtonCount="20" PagerSettings-Position="Bottom" OnPageIndexChanging="gdvDespDetFin_PageIndexChanging" >
                                                <EmptyDataTemplate>
                                                    <b>Sem despesas cadastradas no momento</b>
                                                </EmptyDataTemplate>
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hidId" runat="server" Value='<%# Item.IdDespesa %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Data" DataField="Data" DataFormatString="{0:dd/MM/yyyy}" />
                                                    <asp:BoundField HeaderText="Tipo" DataField="GrupoDespesa"/>                                                            
                                                    <asp:BoundField HeaderText="SubTipo" DataField="TipoDespesa"/>
                                                    <asp:BoundField HeaderText="Descricao" DataField="Descricao" />
                                                    <asp:BoundField HeaderText="Identificador" DataField="Identificador"/>
                                                    <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:C}" />                                                            
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDataConferido" runat="server" />
                                                            <asp:ImageButton ID="btnDespDetConferido" runat="server" ImageUrl="~/img/ok.jpg" OnClick="btnDespDetConferido_Click" Visible='<%# pnlAvalPcFinanceiro.Enabled %>' ToolTip="Marcar como conferido" OnClientClick="return confirmPrestFinanceiro()"  />
                                                            <asp:ImageButton ID="btnDespDetReverter" runat="server" ImageUrl="~/img/remove.png" OnClick="btnDespDetReverter_Click" ToolTip="Reverter conferido" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <b>Comprovante de Reembolso/Devolução</b><br />
                                            <table>
                                                <tr>
                                                    <td>                                                    
                                                        <asp:Label ID="lblParecerValorReemb" runat="server" Font-Bold="true" ForeColor="Red" /><br />
                                                        <asp:TextBox ID="txtValorReemb" runat="server" Width="120px" Enabled="false" />
                                                    </td>
                                                    <td style="text-align:center">
                                                        <table id="tbPcFinReemb" runat="server" visible="false" style="width:70%; padding-left:50px; text-align:center">
                                                            <tr>
					                                            <td style="width:50px"><asp:ImageButton ID="btnRemoverCompPcFinReemb" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemoverCompPcFinReemb_Click" Visible="false"/></td>
					                                            <td style="text-align:center">
                                                                    <asp:HiddenField ID="hidCompPcFinReemb" runat="server" />
                                                                    <a href="javascript:void(0)" onclick="return openDownload('<%= Id %>', '<%= ((int)eVidaGeneralLib.VO.TipoArquivoViagem.COMPROVANTE_REEMBOLSO) %>','<%= hidCompPcFinReemb.Value %>', false);">								
						                                                <asp:Literal ID="ltCompPcFinReemb" runat="server" />
						                                            </a>
					                                            </td>
				                                            </tr>
                                                        </table>
                                                        <asp:Button ID="btnIncluirCompPcFinReemb" runat="server" Text="Incluir/Alterar" OnClientClick="return openCompFinReemb()" OnClick="btnIncluirCompPcFinReemb_Click" UseSubmitBehavior="false" />
                                                    </td>
                                                </tr>
                                            </table>
                                        
                                        

                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Financeiro responsável</b><br />
                                            <asp:Label ID="lblFinanceiroPc" runat="server" />
                                        </td>
                                        <td>
                                            <b>Aval</b><br />
                                            <asp:DropDownList ID="dpdAvalPcFinanceiro" runat="server">
                                                <asp:ListItem Text="SELECIONE" Value="" />
                                                <asp:ListItem Text="APROVADO" Value="S" />
                                                <asp:ListItem Text="REPROVADO" Value="N" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <b>Justificativa</b><br />
                                            <asp:TextBox ID="txtJusPcFinanceiro" runat="server" Width="800px" TextMode="MultiLine" Rows="3" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center">
                                            <asp:Button ID="btnAvalPcFinanceiro" runat="server" Text="Salvar Aval Financeiro" Visible="false" OnClick="btnAvalPcFinanceiro_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <br />

                            <asp:Panel ID="pnlAvalPcDiretoria" runat="server" DefaultButton="btnAvalPcDiretoria" Enabled="false">
                                <h2>3 - Aval da Diretoria</h2>
	                            <table id="tbAvalPcDiretoria" border="1" class="tabelaForm" style="width:950px">
                                    <tr>
                                        <td><b>Diretor responsável:</b><br />
                                            <asp:Label ID="lblDiretorResponsavelPc" runat="server" />
                                        </td>
                                        <td>
                                            <b>Aval</b><br />
                                            <asp:DropDownList ID="dpdAvalPcDiretoria" runat="server">
                                                <asp:ListItem Text="SELECIONE" Value="" />
                                                <asp:ListItem Text="APROVADO" Value="S" />
                                                <asp:ListItem Text="REPROVADO" Value="N" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <b>Justificativa</b><br />
                                            <asp:TextBox ID="txtJusPcDiretoria" runat="server" Width="800px" TextMode="MultiLine" Rows="3" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center"><asp:Button ID="btnRelatorio" runat="server" Text="Relatório de Viagem" />
                                            <asp:Button ID="btnAvalPcDiretoria" runat="server" Text="Salvar Aval Diretoria" Visible="false" OnClick="btnAvalPcDiretoria_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <br />
                        </asp:View>

                    </asp:MultiView>
                </td>
            </tr>
        </table>

    </div>
</asp:Content>

