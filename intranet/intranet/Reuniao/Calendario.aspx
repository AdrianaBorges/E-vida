<%@ Page Title="Calendário de Reuniões" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Calendario.aspx.cs" Inherits="eVidaIntranet.Reuniao.Calendario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function openReuniao(id) {
			window.location = './AreaReuniao.aspx?ID=' + id;
			return false;
		}

	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table>
		<tr>
			<td><b>Filtro de órgão:</b></td>
            <td><asp:CheckBoxList ID="chkLstOrgao" runat="server" DataValueField="Codigo" DataTextField="Nome" RepeatDirection="Horizontal" RepeatColumns="5" /></td>
            <td><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
		<tr>
			<td colspan="3">
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
					AllowSorting="false" CssClass="gridView" Width="100%"  >
					<Columns>
						<asp:BoundField HeaderText="Código" DataField="Id" DataFormatString="{0:000000000}" />
						<asp:BoundField HeaderText="Órgão" DataField="CodConselho" />
						<asp:BoundField HeaderText="Título" DataField="Titulo" />
						<asp:BoundField HeaderText="Descrição" DataField="Descricao" />
						<asp:BoundField HeaderText="Data da Reunião" DataField="Data" DataFormatString="{0:dd/MM/yyyy}" />
						<asp:TemplateField HeaderText="Acesso">
							<ItemStyle Width="95px" />
							<ItemTemplate>
								<asp:ImageButton ID="btnAcessar" runat="server" ImageUrl="~/img/reuniao.png" Height="30px" OnClientClick='<%# "return openReuniao("+ Eval("Id") + ");" %>' AlternateText="Entrar" ToolTip="Entrar" />								
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</td>
		</tr>
		<tr>
			<td colspan="3" style="text-align:center">
				<asp:Repeater ID="rptAno" runat="server" ItemType="Int32">
					<ItemTemplate>
						<asp:LinkButton ID="btnAno" runat="server" Text='<%# Item %>' Font-Bold='<%# Item == CurrentAno %>'
							OnClick="btnAno_Click" />
					</ItemTemplate>
				</asp:Repeater>
			</td>
		</tr>
		<tr>
			<td colspan="3">
				<asp:DataList ID="dlCalendar" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" CellSpacing="10"
					OnItemDataBound="dlCalendar_ItemDataBound">
					<ItemTemplate>
						<asp:Calendar ID="calendario" runat="server" SelectionMode="Day" ShowNextPrevMonth="false" OnDayRender="cal_DayRender"
							Width="300px" OnSelectionChanged="calendario_SelectionChanged" />&nbsp;
					</ItemTemplate>
				</asp:DataList>
			</td>
		</tr>
	</table>
</asp:Content>
