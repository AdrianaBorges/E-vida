<%@ Page Title="Setores de Usuários" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="SetoresUsuario.aspx.cs" Inherits="eVidaIntranet.Admin.SetoresUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		function confirmExclusao() {
			return confirm("Deseja realmente remover o setor?");
		}

		var POP_ARQUIVO = 2;

		function ConfigControlsCustom() {
		    createLocator(650, 550, dlgOpen, null, dlgCallback);
		}

		function dlgOpen(handler, ev, ui) {
		    var src = "";
		    switch (handler.tipo) {
		        case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.CONSELHO.Value %>'; break;
		    }
		    setLocatorUrl(src);
		}

		function dlgCallback(handler, response) {
		    switch (handler.tipo) {
		        case POP_ARQUIVO: break;
		    }
		}
        
		function openArquivo() {
		    var handler = new LocatorHandler(POP_ARQUIVO);
		    openLocator("Arquivos", handler);
		    return false;
		}

	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="100%">
        <tr>
            <td align="center">
				<asp:UpdatePanel runat="server" ID="updCadastro" UpdateMode="Conditional">
					<ContentTemplate>
						<table class="tabelaForm" width="500px">
							<tr>
								<th colspan="6"><h2>Incluir/Alterar Setor</h2><br /></th>
							</tr>
							<tr>
								<td><b>Código:</b></td>
								<td><asp:Label ID="lblCodigo" runat="server" Width="80px"/></td>
								<td><b>Nome:</b></td>
								<td><asp:TextBox ID="txtNome" runat="server" Width="250px" /></td>
							</tr>
							<tr>
								<td colspan="6" align="center"><br />
									<asp:Button ID="btnNovo" runat="server" Text="Limpar/Novo" OnClick="btnNovo_Click" Visible="false" UseSubmitBehavior="false" />
									<asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
								</td>
							</tr>
						</table>
					</ContentTemplate>
				</asp:UpdatePanel>
            </td>
        </tr>
	    <tr style="min-height:200px; vertical-align: top">
		    <td align="center">
				<asp:UpdatePanel runat="server" ID="updSetor" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:ListView ID="ltvSetor" runat="server" ItemPlaceholderID="row"
							ItemType="eVidaGeneralLib.VO.SetorUsuarioVO" DataKeyNames="Id">
							<EmptyDataTemplate><p style="text-align:center"><b>Não existem setores cadastrados.</b></p></EmptyDataTemplate>
							<LayoutTemplate>
								<table class="tabela" style="width:800px">
									<tr>
										<th style="width:50px"></th>	
										<th style="width:50px">Código</th>
										<th>Nome</th>
									</tr>
									<tr id="row" runat="server"></tr>
								</table>
							</LayoutTemplate>
							<ItemTemplate>
								<tr>
									<td><asp:ImageButton ID="bntExcluir" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
											OnClick="bntExcluir_Click" OnClientClick='<%# "return confirmExclusao();" %>' />
										<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px"
											OnClick="btnEditar_Click"/>
									</td>
									<td><b><%# Item.Id %></b></td>
									<td><%# Item.Nome %></td>
								</tr>
							</ItemTemplate>
						</asp:ListView>
						</ContentTemplate>
				</asp:UpdatePanel>
		    </td>
	    </tr>
		<tr>
		    <td align="center">
				<asp:UpdatePanel runat="server" ID="updUsuarios" UpdateMode="Conditional">
					<ContentTemplate>
						<table id="tbUsuarios" runat="server" visible="false">
							<tr>
								<th colspan="3"><h2>Usuários</h2><br /></th>
							</tr>
							<tr>
								<td>
									<b>Usuários</b><br />
                                    <asp:DropDownList ID="dpdPerfil" runat="server" DataValueField="Key" DataTextField="Value" AutoPostBack="true" OnSelectedIndexChanged="dpdPerfil_SelectedIndexChanged" /><br />
									<asp:ListBox ID="ltbUsuario" runat="server" Width="300px" Rows="10" DataValueField="Id" DataTextField="Nome" SelectionMode="Multiple"></asp:ListBox>
								</td>
								<td style="width:100px; text-align:center">
									<asp:Button ID="btnAddUsuario" runat="server" Text=">>" OnClick="btnAddUsuario_Click"  UseSubmitBehavior="false"/><br />
									<asp:Button ID="btnDelUsuario" runat="server" Text="<<" OnClick="btnDelUsuario_Click"  UseSubmitBehavior="false"/>
								</td>
								<td>
									<b>Usuários associados ao setor</b><br />
									<asp:ListBox ID="ltbUsuarioSetor" runat="server" Width="300px" Rows="10" DataValueField="Id" DataTextField="Nome" SelectionMode="Multiple"></asp:ListBox>
								</td>
							</tr>
						</table>						
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
    </table>
</asp:Content>

