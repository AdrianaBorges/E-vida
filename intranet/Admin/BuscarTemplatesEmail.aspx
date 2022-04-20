<%@ Page Title="Templates de Email" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscarTemplatesEmail.aspx.cs" Inherits="eVidaIntranet.Admin.BuscarTemplatesEmail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        
        function openEdit(id) {
            window.location = "TemplateEmail.aspx?id=" + id;
            return false;
        }
        function goNova(id) {
            window.location = 'TemplateEmail.aspx';
            return false;
        }
        function confirmExclusao() {
            return confirm("Deseja realmente remover o template?");
        }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="100%" cellspacing="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:100px; text-align: right">Nome:</td>
			<td align="left"><asp:TextBox ID="txtNome" runat="server" Width="320px" /></td>
			<td style="width:200px; text-align: right">Tipo:</td>
			<td align="left">
                <asp:DropDownList ID="dpdTipo" runat="server" Width="220px"/></td>
		</tr>
		<tr>
			<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
				<asp:Button ID="btnNovo" runat="server" Text="Novo Template" OnClientClick="return goNova();" />
			</td>
		</tr>
    </table>
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
					AllowSorting="false" CssClass="gridView" Width="100%" 
					OnRowDataBound="gdvRelatorio_RowDataBound" >
					<Columns>
						<asp:BoundField HeaderText="Id" DataField="Id" />
						<asp:BoundField HeaderText="Tipo" DataField="Tipo" />
						<asp:BoundField HeaderText="Nome" DataField="Nome" />
						<asp:BoundField HeaderText="Data de Criação" DataField="DataCriacao" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:BoundField HeaderText="Data de Alteração" DataField="DataAlteracao" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
						<asp:TemplateField>
							<ItemStyle Width="95px" />
							<ItemTemplate>
								<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="25px" OnClientClick='<%# "return openEdit("+ Eval("Id") + ");" %>' AlternateText="Editar Template" ToolTip="Editar Template" />
                                <asp:ImageButton ID="bntExcluir" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
											OnClick="bntExcluir_Click" OnClientClick='<%# "return confirmExclusao();" %>' />
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>

