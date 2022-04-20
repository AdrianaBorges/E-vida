<%@ Page Title="Atestados de Comparecimento" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaAtestadoComparecimento.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaAtestadoComparecimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function ConfigControlsCustom() {
            $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

        }
    	function openPdf(id) {
    		openReport(RELATORIO_ATESTADO_COMPARECIMENTO, 'ID=' + id);
		    return false;
		}

        function openEdit(id) {
        	window.location = "FormAtestadoComparecimento.aspx?id=" + id;
        	return false;
        }

        function goView(id) {
        	window.open('ViewAtestadoComparecimento.aspx?ID='+id, 'comparecimento',
				'width=800px, height=800px');
    		return false;
    	}
        function goNova(id) {
        	window.location = 'FormAtestadoComparecimento.aspx';
        	return false;
        }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
	<table width="100%" cellspacing="10px">
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
			<td style="width:150px; text-align: right">Situação: </td>
			<td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="220px" /></td>        
		</tr>    
		<tr>
			<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
				<asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
				<asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClientClick="return goNova();" />
			</td>
		</tr>
    </table>
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
					AllowSorting="false" CssClass="gridView" Width="100%" 
					OnRowDataBound="gdvRelatorio_RowDataBound" >
					<Columns>
						<asp:BoundField HeaderText="Requerimento" DataField="CD_SOLICITACAO" DataFormatString="{0:000000000}" />
						<asp:BoundField HeaderText="Empresa" DataField="BA1_CODEMP" DataFormatString="{0:0}" />
						<asp:BoundField HeaderText="Matrícula" DataField="BA1_MATEMP" DataFormatString="{0:0}" />
						<asp:BoundField HeaderText="Nome Titular" DataField="BA1_NOMUSR" />
						<asp:BoundField HeaderText="Dependente" DataField="nm_beneficiario" />
						<asp:BoundField HeaderText="Situação" DataField="CD_STATUS" />
						<asp:BoundField HeaderText="Data da Solicitação" DataField="dt_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
						<asp:TemplateField>
							<ItemStyle Width="95px" />
							<ItemTemplate>
								<asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Gerar PDF" ToolTip="Gerar PDF" />
								<asp:ImageButton ID="bntView" runat="server" ImageUrl="~/img/lupa.gif" Height="30px" OnClientClick='<%# "return goView("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Visualizar/Imprimir" ToolTip="Visualizar/Imprimir" />
								<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="25px" OnClientClick='<%# "return openEdit("+ Eval("cd_solicitacao") + ");" %>' AlternateText="Editar Solicitação" ToolTip="Editar Solicitação" />
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>
