<%@ Page Title="Autorizações Simples" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaAutorizacao.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaAutorizacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		var POP_CANCELAR = 0;
		var POP_APROVAR = 1;
		var POP_NEGAR = 2;
		var POP_SERVICO = 3;
		var POP_DOC = 4;
		var POP_REVALIDAR = 5;

		function ConfigControlsCustom() {
		    $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
		    $('#<%= txtProtocoloANS.ClientID %>').ForceNumericOnly();
			$('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

			createLocator(650, 550, dlgOpen, dlgClose, dlgCallback);
		}

	    function dlgCallback(handler, response) {
	        executeLocatorHandlerPost(handler, response);
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
				case POP_CANCELAR: src = 'PopAutorizacaoChanger.aspx?TIPO=CANCEL'; break;
				case POP_APROVAR: src = 'PopAutorizacaoAprov.aspx?'; break;
				case POP_NEGAR: src = 'PopAutorizacaoChanger.aspx?TIPO=NEGAR'; break;
				case POP_SERVICO: src = '../GenPops/PopServico.aspx?SHOWT=true'; break;
			    case POP_DOC: src = 'PopAutorizacaoChanger.aspx?TIPO=SOL_DOC'; break;
			    case POP_REVALIDAR: src = '../FormsPop/PopAutorizacaoRevalidar.aspx?'; break;
			}
			src += '&ID=' + handler.id;
			setLocatorUrl(src);
		}

		function dlgClose(ev, ui) {
			
		}

        function openCancelar(obj, id, row) {
        	openPop(POP_CANCELAR, obj, id, row);
        	return false;
        }
        function openNegar(obj, id, row) {
        	openPop(POP_NEGAR, obj, id, row);
        	return false;
        }
        function openAprovar(obj, id, row) {
        	openPop(POP_APROVAR, obj, id, row);
        	return false;
        }
        function openServico(btn) {
        	openPop(POP_SERVICO, btn, '<%: hidCodServico.ClientID%>');
        	return false;
        }
        function openSolDoc(obj, id, row) {
        	openPop(POP_DOC, obj, id, row);
        	return false;
        }

        function openRevalidar(obj, id, row) {
            openPop(POP_REVALIDAR, obj, id, row);
            return false;
        }
        function openPop(tipo, obj, id, row) {
        	var handler = new LocatorHandler(tipo, id, row, obj);
        	var titulo = "";
        	switch (tipo) {
        		case POP_APROVAR: titulo = "Aprovar Autorização"; break;
        		case POP_NEGAR: titulo = "Negar Autorização"; break;
        		case POP_CANCELAR: titulo = "Cancelar Autorização"; break;
        		case POP_SERVICO: titulo = "Serviços"; break;
        		case POP_DOC: titulo = "Solicitar documentos adicionais"; break;
        	}

        	openLocator(titulo, handler);
        	return false;
        }

		function openPdf(id, fName) {
			openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.AUTORIZACAO %>', "ID=" + id + ";" + fName);    		
			return false;
		}
        function openEdit(btn,id) {
        	window.location = "FormAutorizacao.aspx?id=" + id;
        	return false;
        }

        function goNova(id) {
        	window.location = 'FormAutorizacao.aspx';
        	return false;
        }

        function limpar() {
        	window.location = './BuscaAutorizacao.aspx';
        	return false;
        }
        function openAuditores() {
            open('../docs/PERICIA_REGIONAL.pdf?DT=' + (new Date()).getDate(), 'auditores');
        }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	
    
	<table width="1100px" cellpadding="10px" class="tabelaForm">
		<tr>
			<td colspan="6"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:120px" class="tdLblForm">Protocolo:</td>
			<td style="width:135px"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
			<td style="width:140px" class="tdLblForm">Protocolo ANS:</td>
			<td style="width:220px"><asp:TextBox ID="txtProtocoloANS" runat="server" Width="220px" MaxLength="20"  /></td>
			<td style="width:60px" class="tdLblForm">Controle:</td>
			<td><asp:DropDownList ID="dpdPrazo" runat="server" Width="200px" /></td>
		</tr>
		<tr>
			<td class="tdLblForm">Situação: </td>
			<td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="220px" /></td>
			<td class="tdLblForm">Origem: </td>
			<td align="left"><asp:DropDownList ID="dpdOrigem" runat="server" Width="220px" /></td> 
			<td class="tdLblForm">Tipo:</td>
			<td><asp:DropDownList ID="dpdTipo" runat="server" Width="200px" /></td>       
		</tr>
		<tr>
			<td style="width:120px" class="tdLblForm">Nº TISS:</td>
			<td style="width:135px"><asp:TextBox ID="txtNroTiss" runat="server" Width="120px" MaxLength="20"  /></td>
			<td class="tdLblForm">Gestor Responsável:</td>
			<td align="left" colspan="3"><asp:DropDownList ID="dpdGestorAjuste" runat="server" Width="450px" /></td>
		</tr>
		<tr>
			<td colspan="6"><hr /><h3>Beneficiário</h3>
				<table width="100%">
					<tr>
						<td style="width:120px"  class="tdLblForm">Matrícula:</td>
						<td style="width:135px"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20" /></td>
						<td style="width:140px" class="tdLblForm">Nº do Cartão:</td>
						<td style="width:135px"><asp:TextBox ID="txtNumCartao" runat="server" Width="120px" MaxLength="50"  /></td>
						<td style="width:100px" class="tdLblForm">Nome:</td>
						<td><asp:TextBox ID="txtNomeBenef" runat="server" Width="320" MaxLength="255" /></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="6"><hr /><h3>Credenciado</h3>
				<table width="100%">
					<tr>
						<td style="width:120px" class="tdLblForm">CPF/CNPJ:</td>
						<td style="width:135px"><asp:TextBox ID="txtCpfCnpj" runat="server" Width="120px" MaxLength="15"  /></td>
						<td style="width:140px" class="tdLblForm">Nome/Razão social:</td>
						<td><asp:TextBox ID="txtNomeCred" runat="server" Width="320" MaxLength="255" /></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="6"><hr /><h3>Profissional</h3>
				<table width="100%">
					<tr>
						<td style="width:130px" class="tdLblForm">Nº do Conselho:</td>
						<td style="width:135px"><asp:TextBox ID="txtNroConselho" runat="server" Width="120px" MaxLength="15"  /></td>
						<td style="width:130px" class="tdLblForm">Tipo do Conselho:</td>
						<td style="width:115px"><asp:DropDownList ID="dpdConselho" runat="server" Width="120px" DataValueField="Key" DataTextField="Key" /></td>			
						<td style="width:60px" class="tdLblForm">UF:</td>
						<td style="width:125px"><asp:DropDownList ID="dpdUfConselho" runat="server" Width="120px" DataValueField="Sigla" DataTextField="Nome" /></td>			
						<td style="width:100px" class="tdLblForm">Nome:</td>
						<td ><asp:TextBox ID="txtNomeProfissional" runat="server" Width="230" MaxLength="255" /></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="6"><hr /><h3>Dados</h3>
				<table width="100%">
					<tr>
						<td style="width:120px" class="tdLblForm">Caráter:</td>
						<td style="width:135px"><asp:DropDownList ID="dpdCarater" runat="server" Width="120px" /></td>
						<td style="width:120px" class="tdLblForm">Código Doença:</td>
						<td style="width:135px"><asp:TextBox ID="txtCodDoenca" runat="server" Width="80px" /></td>
						<td style="width:100px" class="tdLblForm">Nome Doença:</td>
						<td><asp:TextBox ID="txtNomeDoenca" runat="server" Width="300px" /></td>
					</tr>
					<tr>
						<td class="tdLblForm">Internação:</td>
						<td ><asp:DropDownList ID="dpdInternacao" runat="server" Width="120px" >
							<asp:ListItem Text="TODOS" Value="" />
							<asp:ListItem Text="SIM" Value="S" />
							<asp:ListItem Text="NÃO" Value="N" />
						  </asp:DropDownList></td>
						<td class="tdLblForm">Data Internação:</td>
						<td ><asp:TextBox ID="txtDataInternacao" runat="server" Width="80px" CssClass="calendario" /></td>
						<td class="tdLblForm">Hospital:</td>
						<td ><asp:TextBox ID="txtHospital" runat="server" Width="300px" /></td>
					</tr>
					<tr>
						<td class="tdLblForm">Indicação:</td>
						<td colspan="5"><asp:TextBox ID="txtIndicacao" runat="server" Width="400px" /></td>
					</tr>
                    <tr>
						<td class="tdLblForm">TFD:</td>
						<td ><asp:DropDownList ID="dpdTfd" runat="server" Width="120px" >
							<asp:ListItem Text="TODOS" Value="" />
							<asp:ListItem Text="SIM" Value="S" />
							<asp:ListItem Text="NÃO" Value="N" />
						  </asp:DropDownList></td>
                    </tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="6"><hr /><h3>Procedimentos</h3>
				<asp:UpdatePanel ID="upPnlServico" runat="server" UpdateMode="Conditional">
					<ContentTemplate>

						<table width="100%">
							<tr>
								<td style="width:120px" class="tdLblForm">Serviço:</td>
								<td style="width:435px">
                                    <asp:HiddenField ID="hidCodServico" runat="server" />
									<asp:ImageButton ID="btnLocServico" runat="server" ImageUrl="~/img/lupa.gif" OnClick="btnLocServico_Click" OnClientClick="return openServico(this)" />
									<asp:ImageButton ID="btnClrServico" runat="server" ImageUrl="~/img/remove.png" OnClick="btnClrServico_Click" /><br />
									<asp:Label ID="lblServico" runat="server" Width="100%" />									
								</td>
								<td style="width:100px" class="tdLblForm">OPME:</td>
								<td style="width:120px"><asp:DropDownList ID="dpdOpme" runat="server" Width="80px">
									<asp:ListItem Text="TODOS" Value="" />
									<asp:ListItem Text="SIM" Value="S" />
									<asp:ListItem Text="NÃO" Value="N" />
								  </asp:DropDownList>
								</td>
								<td></td>
							</tr>
						</table>
						
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
		<tr>
			<td colspan="6" style="text-align:center">
				<hr /><br />
				<asp:Button ID="btnLimpar" runat="server" Text="Limpar Filtros" OnClientClick="return limpar();"/>
				<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
				<asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClientClick="return goNova();" />
				<asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			</td>
		</tr>
    </table>
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="cd_autorizacao"
				AllowSorting="false" CssClass="tabela" Width="2000px" 
					OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
					PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                <Columns>
					<asp:TemplateField HeaderText="Controle">
						<ItemTemplate>
							<asp:Image ID="imgControle" runat="server" />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField HeaderText="Protocolo" DataField="cd_autorizacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Beneficiário" DataField="benef_NM_NOMUSR" />
					<asp:BoundField HeaderText="Nº TISS" />
					<asp:BoundField HeaderText="Plano" DataField="plano_DESCRI" />
					<asp:BoundField HeaderText="Solicitante" DataField="TP_ORIGEM" />					
					<asp:BoundField HeaderText="Tipo" DataField="TP_AUTORIZACAO" />
					<asp:BoundField HeaderText="Caráter" DataField="TP_URGENCIA" />
					<asp:BoundField HeaderText="Status" DataField="ST_AUTORIZACAO" />
                    <asp:BoundField HeaderText="Data Solicitação" DataField="dt_solicitacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />					
                    <asp:BoundField HeaderText="Data Sol. Revalidação" DataField="dt_sol_revalidacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
					<asp:BoundField HeaderText="Data Status" DataField="DT_STATUS" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
					<asp:BoundField HeaderText="Responsável" DataField="responsavel_NM_USUARIO" />
                    <asp:BoundField HeaderText="TFD" DataField="FL_TFD" />
					<asp:TemplateField HeaderText="Último ajuste">
						<ItemTemplate>
							<asp:Label ID="lblUltimoAjuste" runat="server" />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Ações">
						<ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <asp:LinkButton  ID="btnRevalidar" runat="server" CommandName="CmdRevalidar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'  OnClientClick='<%# CreateJsFunctionGrid(Container, "openRevalidar") %>' >
                                <asp:Image ID="imgRevalidar" runat="server" ImageUrl="~/img/refresh.png" Height="25px" AlternateText="Revalidar Solicitação" ToolTip="Aprovar Solicitação" />
                            </asp:LinkButton>
							<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEdit") %>' />							
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdAprovar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' >
                                <asp:Image ID="imgAprovar" runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Aprovar Solicitação" ToolTip="Aprovar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnNegar" runat="server" CommandName="CmdNegar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openNegar") %>'>
                                <asp:Image ID="imgNegar" runat="server" ImageUrl="~/img/cancel.png" Height="25px" AlternateText="Negar Solicitação" ToolTip="Negar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnCancelar" runat="server" CommandName="CmdCancelar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgCancelar" runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Cancelar Solicitação" ToolTip="Cancelar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnSolDoc" runat="server" CommandName="CmdDocAdicional" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openSolDoc") %>'>
                                <asp:Image ID="imgSolDoc" runat="server" ImageUrl="~/img/newDoc.png" Height="25px" AlternateText="Solicitação Adicional" ToolTip="Solicitação Adicional" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnPericia" runat="server" CommandName="CmdPericia" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>
                                <asp:Image ID="imgPericia" runat="server" ImageUrl="~/img/pericia.jpg" Height="25px" AlternateText="Encaminhar para perícia" ToolTip="Encaminhar para perícia" />
                            </asp:LinkButton>
							<asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_autorizacao") + ");" %>' Visible="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>
