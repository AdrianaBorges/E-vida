<%@ Page Title="Formulários de Adesão" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaAdesoes.aspx.cs" Inherits="eVidaIntranet.Adesao.BuscaAdesoes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_VALIDAR = 1;

        function ConfigControlsCustom() {
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

            createLocator(650, 550, dlgOpen, null, defaultDlgCallback);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_VALIDAR: src = 'PopValidarAdesao.aspx?'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function validarProposta(obj, id, row) {
            defaultOpenPop(POP_VALIDAR, obj, id, row, "Validar Proposta");
            return false;

        }

        function integrarProposta(obj, id, row) {
            window.location = './IntegrarAdesao.aspx?ID=' + id;
            return false;

        }
        function confirmReceber(nuProposta) {
            return confirm("Deseja realmente marcar esta proposta " + nuProposta + " como recebida?");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">  
    
    <h2 class="componentheading">Resumo</h2>
	<table id="tbResumo" width="100%" cellspacing="10px">
        <tr>
            <td align="center"><b>Empresa</b>: <asp:DropDownList ID="dpdEmpresaResumo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dpdEmpresaResumo_SelectedIndexChanged" /></td>
        </tr>
        <tr>
           	<td align="center">
				<asp:GridView ID="gdvResumo" runat="server" AutoGenerateColumns="false" 
					AllowSorting="false" CssClass="tabela" Width="800px" OnRowDataBound="gdvResumo_RowDataBound" >
					<Columns>
						<asp:BoundField HeaderText="Empresa" DataField="EMPRESA" />
						<asp:BoundField HeaderText="Produto" DataField="PRODUTO" />
						<asp:BoundField HeaderText="Total" DataField="QTD_TOTAL" />
						<asp:BoundField HeaderText="Recebidas" DataField="QTD_VERIFICADO" />
						<asp:BoundField HeaderText="Validadas" DataField="QTD_VALIDADO" />
						<asp:BoundField HeaderText="Invalidadas" DataField="QTD_INVALIDADO" />
						<asp:BoundField HeaderText="Integradas" DataField="QTD_INTEGRADO" />
					</Columns>
				</asp:GridView>
            </td>
        </tr>
	</table>

    <table width="100%" cellspacing="10px">
	    <tr>
		    <td colspan="6"><h2 class="componentheading">Filtros</h2></td>
	    </tr>
        <tr>
            <td style="width:100px; text-align: right">Empresa:</td>
            <td><asp:DropDownList ID="dpdEmpresa" runat="server" /></td>
            <td style="width:200px; text-align: right">Situação:</td>
            <td><asp:DropDownList ID="dpdStatus" runat="server" /></td>
        </tr>
        <tr>
            <td style="width:100px; text-align: right">Nº da proposta:</td>
		    <td align="left"><asp:TextBox ID="txtNumProposta" runat="server" Width="120px" MaxLength="10"  /></td>
            <td style="width:200px; text-align: right">Matrícula:</td>
            <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
        </tr>
	    <tr>
		    <td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
		    </td>
	    </tr>
    </table>
    <table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblObsReceber" runat="server" Text="Para marcar como recebida ou validar uma proposta, realize a busca pelo protocolo ou matrícula!" Font-Bold="true" ForeColor="Red" Visible="false" />
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="NU_DECLARACAO"
				AllowSorting="false" CssClass="gridView" Width="1300px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand">
                <EmptyDataTemplate>
						Não foram encontrados registros com os filtros informados
				</EmptyDataTemplate>
                <Columns>
					<asp:BoundField HeaderText="Empresa" DataField="ID_EMPRESA" />
					<asp:BoundField HeaderText="Produto" DataField="PRODUTO" />
					<asp:BoundField HeaderText="Nº da proposta" DataField="NU_DECLARACAO" DataFormatString="{0:000000000}" />
					<asp:BoundField HeaderText="Matrícula" DataField="ID_MATRICULA" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Titular" DataField="NOME_TITULAR" />
					<asp:BoundField HeaderText="Data de Criação" DataField="DT_CRIACAO" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
					<asp:TemplateField HeaderText="Data Recebida">
                        <ItemTemplate>
                            <asp:Button ID="btnReceber" runat="server" Text="Marcar recebida" CommandArgument='<%# Eval("NU_DECLARACAO") %>' OnClick="btnReceber_Click" Visible="false" OnClientClick='<%# "return confirmReceber(" + Eval("NU_DECLARACAO") + ");" %>' />
                            <asp:Literal ID="litRecebida" runat="server" Text='<%# Eval("DT_RECEBIDA", "{0:dd/MM/yyyy HH:mm}") %>' Visible='<%# Eval("DT_RECEBIDA") != DBNull.Value%>' />
                        </ItemTemplate>
					</asp:TemplateField>
                    <asp:TemplateField HeaderText="Data Validação">
                        <ItemTemplate>
                            <asp:Button ID="btnValidar" runat="server" Text="Marcar validada" CommandArgument='<%# Eval("NU_DECLARACAO") %>' OnClick="btnValidar_Click" OnClientClick='<%# CreateJsFunctionGrid(Container, "validarProposta") %>' Visible="false" />
                            <asp:Literal ID="litValidada" runat="server" Text='<%# Eval("DT_VALIDADA", "{0:dd/MM/yyyy HH:mm}") %>' Visible='<%# Eval("DT_VALIDADA") != DBNull.Value%>' />
                            <asp:Literal ID="litValidada2" runat="server" Visible='<%# Eval("DT_VALIDADA") != DBNull.Value%>' />
                        </ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField HeaderText="Data Integração" DataField="DT_INTEGRACAO" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
					<asp:HyperLinkField HeaderText="Visualizar" DataNavigateUrlFields="NU_DECLARACAO, ID_PRODUTO" DataNavigateUrlFormatString="../View/View.aspx?id={0:N}&tp={1}" Target="_declaracao"
						Text="Visualizar" />
                    <asp:TemplateField>
                        <ItemStyle Width="125px" />
                        <ItemTemplate>
                            <asp:LinkButton  ID="btnIntegrar" runat="server" CommandName="CmdEncerrar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "integrarProposta") %>' Visible="false">
                                <asp:Image ID="imgIntegrar"  runat="server" ImageUrl="~/img/process-info.png" Height="25px" AlternateText="Integrar proposta" ToolTip="Integrar proposta" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
