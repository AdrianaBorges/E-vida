<%@ Page Title="Protocolos de Fatura" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaProtocoloFatura.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaProtocoloFatura" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_CANCELAR = 1;
        var POP_EMAIL = 2;
        var POP_CREDENCIADO = 4;
        var POP_ANALISTA = 5;
        var POP_ANALISE_CONTA = 6

        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);
            
            $('#<%= txtValor.ClientID %>').priceFormat(formatPreco);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CANCELAR: src = 'PopCancelProtocoloFatura.aspx?'; break;
                case POP_EMAIL: src = '../GenPops/PopEmailTemplate.aspx?TIPO=<%= (int)eVidaGeneralLib.VO.TipoTemplateEmail.PROTOCOLO_FATURA %>'; break;
                case POP_CREDENCIADO: src = '../GenPops/PopCredenciado.aspx?enableEmpty=false'; break;
                case POP_ANALISTA: src = '../GenPops/PopUsuario.aspx?'; break;
                case POP_ANALISE_CONTA: src = './PopDemoAnaliseConta.aspx?'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }
        function dlgCallback(handler, response) {
            executeLocatorHandlerPost(handler, response);
        }

        function openEdit(btn, id) {
            window.location = "FormProtocoloFatura.aspx?id=" + id;
            return false;
        }
        function goNova(id) {
            window.location = 'FormProtocoloFaturaNova.aspx';
            return false;
        }
        function openCancelar(obj, id, row) {
            defaultOpenPop(POP_CANCELAR, obj, id, row, "Cancelar Protocolo");
            return false;
        }
        function openPopCred(btnLoc) {
            defaultOpenPop(POP_CREDENCIADO, btnLoc, '<%: hidCredenciado.ClientID %>', -1, "Credenciado");
            return false;
        }
        function openPopAnalista(btnLoc) {
            defaultOpenPop(POP_ANALISTA, btnLoc, '<%: hidCodAnalista.ClientID %>', 0, "Analista");
            return false;
        }
        
        function openEmail(obj, id, row) {
            try {
                defaultOpenPop(POP_EMAIL, obj, id, row, "Enviar email");
            } catch (e) {
                alert(e.message);
            }
            return false;
        }

        function openDemoAnalise(obj, id, row) {
            defaultOpenPop(POP_ANALISE_CONTA, obj, id, row, "Demonstrativo de Análise de Conta");
        }

        function openPdf(id) {
            try {
                openReport(RELATORIO_PROTOCOLO_FATURA_CAPA, 'ID=' + id);
            } catch (e) {
                alert(e.message);
            }
            return false;
        }
        function openPrint(id) {
            try {
                window.open('PopImprimirProtocoloFatura.aspx?ID=' + id);
            } catch (e) {
                alert(e.message);
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
	<table width="1200px" cellpadding="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td>Protocolo:</td>
			<td><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="15"  /></td>
		</tr>
        <tr>
			<td>CPF/CNPJ:</td>
			<td>
				<asp:HiddenField ID="hidCredenciado" runat="server" />
                <asp:TextBox ID="txtCpfCnpj" runat="server" Width="200px" MaxLength="20"  />
                <asp:ImageButton ID="btnLocCred" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopCred(this)" OnClick="btnLocCred_Click" />
				<asp:ImageButton ID="btnClrCred" runat="server" ImageUrl="~/img/remove.png" OnClick="btnClrCred_Click" />
			</td>
            <td>Razão Social:</td>
            <td><asp:TextBox ID="txtRazaoSocial" runat="server" Width="400px" /></td>
        </tr>
        <tr>
            <td><br /><br />Analista Responsável:</td>
            <td colspan="3"><br /><br />
                <asp:HiddenField ID="hidCodAnalista" runat="server" />
                <asp:GridView ID="gdvUsuarios" runat="server" Width="500px" CssClass="tabela"
                    AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"
                    OnRowCommand="gdvUsuarios_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnRemover" runat="server" ImageUrl="~/img/remove.png" CommandName="RemoverUsuario" CommandArgument='<%# Container.DataItemIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="cd_usuario" HeaderText="USUÁRIO" />
                        <asp:BoundField DataField="nm_usuario" HeaderText="NOME" />
                    </Columns>
                </asp:GridView>
                <asp:Button ID="btnAdicionarUsuario" runat="server" OnClientClick="return openPopAnalista(this)" Text="Adicionar Usuário" OnClick="btnAdicionarUsuario_Click" />
                <asp:Button ID="btnLimparUsuario" runat="server" Text="Limpar Usuários" OnClick="btnLimparUsuario_Click" Visible="false" />
                <br /><br />
            </td>
        </tr>
        <tr>
			<td>Doc. Fiscal:</td>
			<td><asp:TextBox ID="txtDocFiscal" runat="server" Width="120px" MaxLength="100"  /></td>
			<td>Data de Emissão:</td>
			<td><asp:TextBox ID="txtDataEmissao" runat="server" Width="80px" CssClass="calendario"  /></td>
		</tr>
        <tr>
			<td>Valor Apresentado:</td>
			<td><asp:TextBox ID="txtValor" runat="server" Width="120px" MaxLength="100"  /></td>
			<td>Data de Entrada:</td>
			<td>de <asp:TextBox ID="txtDataEntradaInicio" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
                até  <asp:TextBox ID="txtDataEntradaFim" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
			</td>
		</tr>
		<tr>
			<td>Situação: </td>
			<td><asp:DropDownList ID="dpdSituacao" runat="server" Width="220px" /></td>    
            <td>Vencimento:</td>
			<td>de <asp:TextBox ID="txtVencimentoInicio" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
                até  <asp:TextBox ID="txtVencimentoFim" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
			</td>
		</tr>
        <tr>
			<td>Natureza:</td>
			<td><asp:DropDownList ID="dpdNatureza" runat="server" Width="320px" DataValueField="Codigo" DataTextField="Descri" /></td>
            <td>Finalização:</td>
			<td>de <asp:TextBox ID="txtFinalizacaoInicio" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
                até  <asp:TextBox ID="txtFinalizacaoFim" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
			</td>
		</tr>
        <tr>
			<td>Unidade:</td>
			<td><asp:DropDownList ID="dpdRegional" runat="server" Width="320px" DataValueField="Key" DataTextField="Value" /></td>
            <td>Expedição:</td>
			<td>de <asp:TextBox ID="txtExpedicaoInicio" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
                até  <asp:TextBox ID="txtExpedicaoFim" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
			</td>
            
        </tr>
        <tr>
            <td>Controle:</td>
            <td><asp:DropDownList ID="dpdControle" runat="server"/></td>
        </tr>
		<tr>
			<td colspan="6" style="text-align:center"><br /><br />
				<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
				<asp:Button ID="btnNovo" runat="server" Text="Nova Entrada" OnClientClick="return goNova();" />
				<asp:Button ID="btnLimpar" runat="server" Text="Limpar Filtros" OnClick="btnLimpar_Click" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			</td>
		</tr>
    </table>
    
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
                <asp:Label ID="lblDuplicados" runat="server" ForeColor="Red" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_PROTOCOLO_FATURA"
					AllowSorting="false" CssClass="tabela" Width="2000px"
					OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
					PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                <Columns>
                    <asp:TemplateField HeaderText="Controle">
                        <ItemTemplate>
                            <asp:Image ID="imgControle" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
					<asp:BoundField HeaderText="Protocolo" DataField="NR_protocolo" ItemStyle-Wrap="false" />
                    <asp:BoundField HeaderText="CPF/CNPJ" DataField="BAU_CPFCGC" />
					<asp:BoundField HeaderText="Razão Social" DataField="BAU_NOME" />
					<asp:BoundField HeaderText="Natureza" DataField="BAG_DESCRI" />
                    <asp:BoundField HeaderText="Doc. Fiscal" DataField="DS_DOC_FISCAL" />
					<asp:BoundField HeaderText="Data de Emissão" DataField="DT_EMISSAO" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Data de Entrada" DataField="DT_ENTRADA" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Vencimento" DataField="DT_VENCIMENTO" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Finalizado em" DataField="DT_FINALIZACAO" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Data Expedição" DataField="DT_EXPEDICAO" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Valor Apresentado" DataField="VL_APRESENTADO" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Valor Processado" DataField="VL_PROCESSADO" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Glosa" DataField="VL_GLOSA" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Valor Original" DataField="E2_VALOR" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Valor Pago" DataField="E2_VALLIQ" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Data Baixa" DataField="E2_BAIXA" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="ISS" DataField="E2_ISS" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="IRRF" DataField="E2_IRRF" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="Desconto" DataField="E2_DESCONT" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="COFINS" DataField="E2_COFINS" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="PIS" DataField="E2_PIS" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="CSLL" DataField="E2_CSLL" DataFormatString="{0:C}" />
                    <asp:BoundField HeaderText="USUÁRIO PROTOCOLO" DataField="criador_CD_USUARIO" />
                    <asp:BoundField HeaderText="ANALISTA RESP." DataField="analista_CD_USUARIO" />
                    <asp:TemplateField HeaderText="Situação">
                        <ItemTemplate>
                            <asp:Literal ID="litSituacao" runat="server" />
                        </ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Unidade">
                        <ItemTemplate>
                            <asp:Literal ID="litUnidade" runat="server" />
                        </ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Pendência">
                        <ItemTemplate>
                            <asp:Literal ID="litPendencia" runat="server" />
                        </ItemTemplate>
					</asp:TemplateField>
                    <asp:TemplateField HeaderText="Ações">
						<ItemStyle Wrap="false" />
                        <ItemTemplate>
							<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEdit") %>' />
                            <asp:LinkButton  ID="btnNegar" runat="server" CommandName="CmdNegar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgNegar" runat="server" ImageUrl="~/img/cancel.png" Height="25px" AlternateText="Negar/Cancelar Solicitação" ToolTip="Cancelar Protocolo" />
                            </asp:LinkButton>
                            <asp:ImageButton ID="btnPrint" runat="server" ImageUrl="~/img/print.png" ToolTip="Entrada" Height="30px" OnClientClick='<%# "return openPrint("+ Eval("cd_protocolo_fatura") + ");" %>' />
							<asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" ToolTip="Capa de Lote" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_protocolo_fatura") + ");" %>' />
                            <asp:ImageButton ID="btnEmail" runat="server" ImageUrl="~/img/email.png" ToolTip="Enviar Email" Height="30px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEmail") %>' />
                            <asp:ImageButton ID="btnAnalise" runat="server" ImageUrl="~/img/analise.png" ToolTip="Demonstrativo de Análise" Height="30px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openDemoAnalise") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>
