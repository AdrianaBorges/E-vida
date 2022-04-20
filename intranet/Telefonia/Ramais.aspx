<%@ Page Title="Ramais" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Ramais.aspx.cs" Inherits="eVidaIntranet.Telefonia.Ramais" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		function confirmExclusao() {
			return confirm("Deseja realmente remover o ramal?");
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
								<th colspan="6"><h2>Incluir/Alterar Ramal</h2><br /></th>
							</tr>
							<tr>
								<td><b>Número:</b></td>
								<td><asp:TextBox ID="txtRamal" runat="server" Width="80px" MaxLength="4"/></td>
                                <td><b>Alias:</b></td>
                                <td><asp:TextBox ID="txtAlias" runat="server" Width="120px" MaxLength="50"/></td>
								<td><b>Tipo:</b></td>
								<td><asp:DropDownList ID="dpdTipo" runat="server" Width="250px" OnSelectedIndexChanged="dpdTipo_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="SETOR" Value="SETOR" />
                                    <asp:ListItem Text="USUÁRIO" Value="USUARIO" />
								    </asp:DropDownList></td>
							</tr>
                            <tr>
                                <td colspan="6">
                                    <asp:MultiView ID="mtvTipoRamal" runat="server">
                                        <asp:View ID="vwSetor" runat="server">
                                            <b>Setor:</b>
                                            <asp:DropDownList ID="dpdSetor" runat="server" Width="350px" DataValueField="Id" DataTextField="Nome" />
                                        </asp:View>
                                        <asp:View ID="vwUsuario" runat="server">
                                            <asp:UpdatePanel ID="updUsuarios" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <table id="tbUsuarios" runat="server" visible="false">
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
			                                                    <b>Usuários associados ao ramal</b><br />
			                                                    <asp:ListBox ID="ltbUsuarioRamal" runat="server" Width="300px" Rows="10" DataValueField="Id" DataTextField="Nome" SelectionMode="Multiple"></asp:ListBox>
		                                                    </td>
	                                                    </tr>
                                                    </table>                                                    
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </asp:View>
                                    </asp:MultiView>
                                </td>
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
				<asp:UpdatePanel runat="server" ID="updRamal" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:ListView ID="ltvRamal" runat="server" ItemPlaceholderID="row"
							ItemType="eVidaGeneralLib.VO.RamalVO" DataKeyNames="NrRamal" OnItemDataBound="ltvRamal_ItemDataBound">
							<EmptyDataTemplate><p style="text-align:center"><b>Não existem ramais cadastrados.</b></p></EmptyDataTemplate>
							<LayoutTemplate>
								<table class="tabela" style="width:800px">
									<tr>
										<th style="width:50px"></th>	
										<th style="width:50px">Número</th>
										<th style="width:120px">Alias</th>
										<th>Tipo</th>
                                        <th>Associado à</th>
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
									<td><b><%# Item.NrRamal %></b></td>
									<td><b><%# Item.Alias %></b></td>
									<td><%# Item.Tipo %></td>
                                    <td><asp:Literal ID="ltAssociacao" runat="server" /></td>
								</tr>
							</ItemTemplate>
						</asp:ListView>
						</ContentTemplate>
				</asp:UpdatePanel>
		    </td>
	    </tr>
    </table>
</asp:Content>