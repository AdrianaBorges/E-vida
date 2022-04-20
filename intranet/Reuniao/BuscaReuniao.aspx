<%@ Page Title="Reunião / Documentações" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaReuniao.aspx.cs" Inherits="eVidaIntranet.Reuniao.BuscaReuniao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		var POP_EMAIL = 2;

		function ConfigControlsCustom() {
			createLocator(650, 550, dlgOpen, null, dlgCallback);
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
				case POP_EMAIL: src = './PopEmail.aspx?'; break;
			}
			src += '&ID=' + handler.id;
			setLocatorUrl(src);
		}

		function dlgCallback(handler, response) {
			switch (handler.tipo) {
				case POP_EMAIL: break;
			}
		}

		function openPop(tipo, obj, id, row) {
			var handler = new LocatorHandler(tipo, id, row, obj);
			var titulo = "";
			switch (tipo) {
				case POP_EMAIL: titulo = "Enviar e-mail"; break;
			}

			openLocator(titulo, handler);
			return false;
		}

		function openEmail(obj, id, row) {
			openPop(POP_EMAIL, obj, id, row);
			return false;
		}
		function goNova() {
			window.location = './AreaReuniao.aspx';
			return false;
		}

		function openEdit(id) {
			window.location = './AreaReuniao.aspx?ID=' + id;
			return false;
		}

		function confirmExcluir(id) {
			return confirm("Deseja realmente excluir a reunião?");
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

	<table width="100%" cellspacing="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:100px; text-align: right">Órgão:</td>
			<td align="left"><asp:DropDownList ID="dpdConselho" runat="server" Width="300px" DataValueField="Codigo" DataTextField="Nome" /></td>
		</tr>
		<tr>
			<td style="width:200px; text-align: right">Título:</td>
			<td align="left"><asp:TextBox ID="txtTitulo" runat="server" Width="300px" /></td>
		</tr>  
		<tr>
			<td style="width:200px; text-align: right">Descrição:</td>
			<td align="left"><asp:TextBox ID="txtDescricao" runat="server" Width="300px" /></td>
		</tr>  
		<tr>
			<td style="width:200px; text-align: right">Período:</td>
			<td align="left">entre <asp:TextBox ID="txtInicio" runat="server" Width="100px" CssClass="calendario" /> e
				<asp:TextBox ID="txtFim" runat="server" Width="100px" CssClass="calendario" />
			</td>
		</tr>  
		<tr>
			<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
				<asp:Button ID="btnNovo" runat="server" Text="Nova Reunião" OnClientClick="return goNova();" />
			</td>
		</tr>
    </table>
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_REUNIAO"
					AllowSorting="false" CssClass="gridView" Width="100%" 
					OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand" >
					<Columns>
						<asp:BoundField HeaderText="Código" DataField="CD_REUNIAO" DataFormatString="{0:000000000}" />
						<asp:BoundField HeaderText="Órgão" DataField="nm_conselho" />
						<asp:BoundField HeaderText="Título" DataField="nm_reuniao" />
						<asp:BoundField HeaderText="Descrição" DataField="ds_reuniao" />
						<asp:BoundField HeaderText="Data da Reunião" DataField="dt_reuniao" DataFormatString="{0:dd/MM/yyyy}" />
						<asp:TemplateField HeaderText="Acesso">
							<ItemStyle Width="95px" />
							<ItemTemplate>
								<asp:ImageButton ID="btnAcessar" runat="server" ImageUrl="~/img/reuniao.png" Height="30px" OnClientClick='<%# "return openEdit("+ Eval("CD_REUNIAO") + ");" %>' AlternateText="Entrar" ToolTip="Entrar" />
								<asp:LinkButton  ID="btnExcluir" runat="server" CommandName="CmdExcluir" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "confirmExcluir") %>'>
									<asp:Image ID="imgExcluir" runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Excluir Reunião" ToolTip="Excluir Reunião" />
								</asp:LinkButton>
								<asp:ImageButton ID="btnEmail" runat="server" ImageUrl="~/img/email.png" Height="25px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEmail") %>' AlternateText="Enviar email" ToolTip="Enviar email" />
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>
