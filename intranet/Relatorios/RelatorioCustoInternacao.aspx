<%@ Page Title="Custo Internação Total" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioCustoInternacao.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioCustoInternacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	var POP_BENEFICIARIO = 1;
    	function ConfigControlsCustom() {
    		createLocator(600, 600, dlgOpen, null, dlgCallback);
    	}

    	function dlgOpen(handler, ev, ui) {
    		var src = "";
    		switch (handler.tipo) {
    			case POP_BENEFICIARIO: src = '../GenPops/PopBeneficiario.aspx'; break;
    		}
    		setLocatorUrl(src);
    	}

    	function dlgCallback(handler, response) {
    		switch (handler.tipo) {
    			case POP_BENEFICIARIO:
    				$("#" + handler.id).val(response);
    				closeLocator();
    				__doPostBack(handler.btn.name, '');
    				break;
    		}
    	}
    	function openPopBeneficiario(btnLoc) {
    		var handler = new LocatorHandler(POP_BENEFICIARIO, '<%: hidCodBenef.ClientID %>', -1, btnLoc);
			openLocator("Beneficiário", handler);
			return false;
		}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

<table width="1100px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td>Referência:</td>
        <td colspan="3">
            <asp:TextBox ID="txtAnoMesRef" runat="server" Width="80px" MaxLength="12" CssClass="calendario" />
        </td>
    </tr>
	<tr>
        <td>Número do Cartão:</td>
        <td><asp:HiddenField ID="hidCodBenef" runat="server" />
			<asp:TextBox ID="txtNumCartao" runat="server" Width="200px" MaxLength="100" />
			<asp:ImageButton ID="btnBuscarBeneficiario" runat="server" ImageUrl="~/img/lupa.gif" 
                    OnClientClick="return openPopBeneficiario(this)" OnClick="btnBuscarBeneficiario_Click" />
        </td>
		<td>Beneficiário:</td>
        <td><asp:TextBox ID="txtNomeBenef" runat="server" Width="300px" MaxLength="100" /></td>
    </tr>
	<tr>
        <td>Número da Autorização:</td>
        <td>
            <asp:TextBox ID="txtNumAutorizacao" runat="server" Width="200px" />
        </td>
	</tr>
	<tr>
		<td>Plano:</td>
        <td colspan="3">
            <asp:CheckBoxList ID="chkPlano" runat="server" Width="100%" DataValueField="CdPlano" DataTextField="DsPlano" RepeatColumns="3"  BorderStyle="Solid" />
        </td>
    </tr>
	<tr>		
		<td colspan="4" style="text-align:center">
			<asp:UpdatePanel ID="pnlBotao" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
					<asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
    </table>
    <table>
	<tr style="height:300px">
		<td colspan="4">
			<asp:UpdatePanel ID="pnlGrid" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<asp:Label ID="lblCount" runat="server" Visible="false" />
					<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
						AllowSorting="false" CssClass="tabela" Width="1500px" OnSorting="gdvRelatorio_Sorting" 
						AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
						<Columns>
							<asp:BoundField HeaderText="REFERÊNCIA" DataField="dt_ano_mes_ref" DataFormatString="{0:dd/MM/yyyy}"/>
							<asp:BoundField HeaderText="NÚMERO CARTÃO" DataField="cd_alternativo" />
							<asp:BoundField HeaderText="BENEFICIÁRIO" DataField="nm_beneficiario" />
							<asp:BoundField HeaderText="NÚMERO AUTORIZAÇÃO" DataField="nr_autorizacao" />
							<asp:BoundField HeaderText="PLANO" DataField="cd_plano_cob" />
							<asp:BoundField HeaderText="VALOR TOTAL INTERNACAO" DataField="vl_item" DataFormatString="{0:C}" />
							<asp:BoundField HeaderText="INICIO INTERNACAO" DataField="dt_inicio_autorizacao" DataFormatString="{0:dd/MM/yyyy}" />
							<asp:BoundField HeaderText="TERMINO INTERNACAO" DataField="dt_termino_autorizacao" DataFormatString="{0:dd/MM/yyyy}" />
						</Columns>
					</asp:GridView>
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>

</table>
</asp:Content>
