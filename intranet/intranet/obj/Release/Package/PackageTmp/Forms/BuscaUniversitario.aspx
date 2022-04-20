<%@ Page Title="Declaração de Universitário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaUniversitario.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaUniversitario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	var POP_CANCELAR = 0;
    	var POP_APROVAR = 1;

    	function ConfigControlsCustom() {
    	    $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
        	$('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

    	    createLocator(550, 450, dlgOpen, null, defaultDlgCallback);
    	}
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CANCELAR: src = 'PopCancelUniversitario.aspx?'; break;
                case POP_APROVAR: src = 'A.aspx?ID='; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function openPop(tipo, obj, id, row) {
            var handler = new LocatorHandler(tipo, id, row, obj);

            var titulo = tipo == POP_APROVAR ? "Aprovar Declaração" : "Recusar Declaração";

            openLocator(titulo, handler);
            return false;
        }

    	function openCancelar(obj, id, row) {
    	    openPop(POP_CANCELAR, obj, id, row);
    		return false;
    	}
    	function openAprovar(obj, id, row) {
    	    openPop(POP_APROVAR, obj, id, row);
    		return false;
    	}
        
    	function goView(id) {
    		window.location = 'ViewUniversitario.aspx?ID=' + id;
    		return false;
    	}

        function openPdf(id) {
        	openReport(RELATORIO_UNIVERSITARIO,	"ID=" + id);
        	return false;
        }

        function openDownload(id) {
        	openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.DECLARACAO_UNIVERSITARIO %>', "ID=" + id);
        	return false;
        }
    	         
    	function confirmAprovar() {
    		return confirm("Deseja realmente aprovar esta solicitação?");
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

<table width="100%" cellpadding="10" cellspacing="10" class="tabelaForm">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">Protocolo:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
        <td style="width:200px; text-align: right">Matrícula:</td>
        <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
    </tr>
	<tr>
        <td style="text-align: right">Situação:</td>
        <td colspan="3"><asp:DropDownList ID="dpdFiltroSituacao" runat="server" Width="200px"/></td>
    </tr>
    <tr>
		<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnPesquisar_Click"/>
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		</td>
	</tr>
</table>
<table width="100%">
	<tr style="min-height: 500px; vertical-align:top">
		<td colspan="2">            
            <asp:Label ID="lblCount" runat="server" /><br /><br />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
				AllowSorting="false" CssClass="tabela" Width="100%" 
				OnRowDataBound="gdvRelatorio_RowDataBound" 
				OnRowCommand="gdvRelatorio_RowCommand">
                <Columns>
					<asp:BoundField HeaderText="Declaracao" DataField="cd_solicitacao" SortExpression="cd_solicitacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Dependente" DataField="BA1_NOMUSR" SortExpression="BA1_NOMUSR" />
					<asp:BoundField HeaderText="Data da Solicitação" DataField="dt_criacao" SortExpression="dt_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"  ItemStyle-HorizontalAlign="Center" />
					<asp:BoundField HeaderText="Data Alteracao" DataField="dt_alteracao" SortExpression="dt_alteracao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
					<asp:BoundField HeaderText="Situação" DataField="CD_STATUS" SortExpression="CD_STATUS" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' ToolTip="Formulário" />
							<asp:ImageButton ID="btnArquivo" runat="server" ImageUrl="~/img/download24.png" Height="24px" OnClientClick='<%# "return openDownload("+ Eval("cd_solicitacao") + ");" %>' ToolTip="Arquivo anexado" />
							<asp:ImageButton ID="bntView" runat="server" ImageUrl="~/img/lupa.gif" Height="30px" OnClientClick='<%# "return goView("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Visualizar" ToolTip="Visualizar" />
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdAprovar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick="return confirmAprovar()" >
                                <asp:Image ID="imgAprovar"  runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Aprovar Solicitação" ToolTip="Aprovar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnNegar" runat="server" CommandName="CmdCancelar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgNegar"  runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Negar Solicitação" ToolTip="Negar Solicitação" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
