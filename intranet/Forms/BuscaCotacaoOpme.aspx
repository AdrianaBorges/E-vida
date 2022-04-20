<%@ Page Title="Cotações OPME" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaCotacaoOpme.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaCotacaoOpme" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		var POP_CANCELAR = 0;
		var POP_APROVAR = 1;

		function ConfigControlsCustom() {
			$('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();

			createLocator(650, 550, dlgOpen, dlgClose, dlgCallback);
		}

		function dlgCallback(handler, response) {
			closeLocator();
			if (handler.btn.href)
				eval(handler.btn.href);
			else if ($(handler.btn).attr('href'))
				eval($(handler.btn).attr('href'));
			else
				__doPostBack(handler.btn.name, '');
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
				case POP_CANCELAR: src = 'A.aspx?TIPO=CANCEL'; break;
			}
			src += '&ID=' + handler.id;
			setLocatorUrl(src);
		}

		function dlgClose(ev, ui) {

		}

    	function openCancelar(obj, id, row) {
    		openPop(CANCELAR, obj, id, row);
    		return false;
    	}
    	function openAprovar(obj, id, row) {
    		openPop(APROVAR, obj, id, row);
    		return false;
    	}

    	function openPop(tipo, obj, id, row) {
    		var handler = new LocatorHandler(tipo, id, row, obj);
    		var titulo = "";
    		switch (tipo) {
    			case POP_APROVAR: titulo = "Aprovar Cotação"; break;
    			case POP_CANCELAR: titulo = "Cancelar Cotação"; break;
    		}

    		openLocator(titulo, handler);
    		return false;
    	}

        function confirmAprovar() {
        	return confirm("Deseja realmente aprovar esta solicitação?");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	
<table width="100%" cellpadding="10" cellspacing="10">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">Protocolo:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
	</tr>
	<tr>
        <td style="width:200px; text-align: right">Nome do beneficiário:</td>
        <td align="left"><asp:TextBox ID="txtNomeBeneficiario" runat="server" Width="320px" MaxLength="200"  /></td>
    </tr>
	<tr>
        <td style="width:200px; text-align: right">Fornecedor:</td>
        <td align="left"><asp:TextBox ID="txtNomeFornecedor" runat="server" Width="320px" MaxLength="200"  /></td>
    </tr>
	<tr>
        <td style="text-align: right">Situação:</td>
        <td colspan="3"><asp:DropDownList ID="dpdFiltroSituacao" runat="server" Width="200px"/></td>
    </tr>
    <tr>
		<td colspan="6" style="text-align:center">
			<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
		</td>
	</tr>
</table>
<table width="100%">
	<tr style="min-height: 500px; vertical-align:top">
		<td colspan="2">            
            <asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="cd_autorizacao"
				AllowSorting="false" CssClass="tabela" Width="1000px" OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="cd_autorizacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Beneficiário" DataField="benef_NM_BENEFICIARIO" />
					<asp:BoundField HeaderText="Fornecedor" DataField="fornec_NM_CREDENCIADO" />
					<asp:BoundField HeaderText="Status" DataField="ST_COTACAO" />
                    <asp:BoundField HeaderText="Data Solicitação" DataField="dt_solicitacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />					
					<asp:TemplateField HeaderText="Ações">
						<ItemStyle Wrap="false" />
                        <ItemTemplate>
							<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEdit") %>' />							
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdAprovar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' >
                                <asp:Image ID="imgAprovar" runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Aprovar Solicitação" ToolTip="Aprovar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnCancelar" runat="server" CommandName="CmdCancelar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openCancelar") %>'>
                                <asp:Image ID="imgCancelar" runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Cancelar Solicitação" ToolTip="Cancelar Solicitação" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
