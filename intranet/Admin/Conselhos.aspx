<%@ Page Title="Órgãos" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Conselhos.aspx.cs" Inherits="eVidaIntranet.Admin.Conselhos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		function confirmExclusao() {
			return confirm("Deseja realmente remover o órgão?");
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

		function onAfterUpload(url, originalName) {
		    $("#<%:hidArqFisico.ClientID%>").val(url);
		    $("#<%:hidArqOrigem.ClientID%>").val(originalName);
		    closeLocator();
		    <%= ClientScript.GetPostBackEventReference(btnIncluirArquivo, "") %>
		}


		function openArquivo() {
		    var handler = new LocatorHandler(POP_ARQUIVO);
		    openLocator("Arquivos", handler);
		    return false;
		}

		function openDownload(idConselho, fId) {
		    openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.CONSELHO %>', "ID=" + idConselho + ";" + fId);
		    return false;
		}

		function confirmExclusaoArquivo() {
		    return confirm("Deseja realmente retirar este arquivo?");
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
								<th colspan="6"><h2>Incluir/Alterar Órgão</h2><br /></th>
							</tr>
							<tr>
								<td><b>Código:</b></td>
								<td><asp:TextBox ID="txtCodigo" runat="server" MaxLength="5" Width="80px"/></td>
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
				<asp:UpdatePanel runat="server" ID="updConselho" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:ListView ID="ltvConselho" runat="server" ItemPlaceholderID="row"
							ItemType="eVidaGeneralLib.VO.ConselhoVO" DataKeyNames="Codigo">
							<EmptyDataTemplate><p style="text-align:center"><b>Não existem órgãos cadastrados.</b></p></EmptyDataTemplate>
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
									<td><b><%# Item.Codigo %></b></td>
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
									<b>Usuários sem associação</b><br />
                                    <asp:DropDownList ID="dpdPerfil" runat="server" DataValueField="Key" DataTextField="Value" AutoPostBack="true" OnSelectedIndexChanged="dpdPerfil_SelectedIndexChanged" /><br />
									<asp:ListBox ID="ltbUsuario" runat="server" Width="300px" Rows="10" DataValueField="Id" DataTextField="Nome" SelectionMode="Multiple"></asp:ListBox>
								</td>
								<td style="width:100px; text-align:center">
									<asp:Button ID="btnAddUsuario" runat="server" Text=">>" OnClick="btnAddUsuario_Click"  UseSubmitBehavior="false"/><br />
									<asp:Button ID="btnDelUsuario" runat="server" Text="<<" OnClick="btnDelUsuario_Click"  UseSubmitBehavior="false"/>
								</td>
								<td>
									<b>Usuários associados ao órgão</b><br />
									<asp:ListBox ID="ltbUsuarioConselho" runat="server" Width="300px" Rows="10" DataValueField="Id" DataTextField="Nome" SelectionMode="Multiple"></asp:ListBox>
								</td>
							</tr>
						</table>
						
						
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
        <tr>
            <td align="center">
                <asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
					<ContentTemplate>
						<div id="dvArquivos" runat="server" visible="false">
						<h2>Arquivos do Órgão</h2>		
							<asp:HiddenField ID="hidArqFisico" runat="server" />
							<asp:HiddenField ID="hidArqOrigem" runat="server" />
							<asp:Button ID="btnIncluirArquivo" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArquivo_Click"  UseSubmitBehavior="false"/>


						<asp:ListView ID="ltvArquivo" runat="server" ItemPlaceholderID="row"
							ItemType="eVidaGeneralLib.VO.ArquivoTelaVO" DataKeyNames="Id, NomeTela">
							<EmptyDataTemplate><p style="text-align:center"><b>Não existem arquivos cadastrados.</b></p></EmptyDataTemplate>
							<LayoutTemplate>
								<table class="tabela" style="width:800px">
									<tr>
										<th style="width:50px">Sequencial</th>
										<th>Nome</th>
										<th>Descrição</th>
										<th style="width:85px"></th>	
									</tr>
									<tr id="row" runat="server"></tr>
								</table>
							</LayoutTemplate>
							<ItemTemplate>
								<tr>
									<td><b><%# Item.Id %></b></td>
									<td><%# Item.NomeTela %></td>
									<td><%# Item.Descricao %>
                                        <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" 
											OnClick="bntEditarArquivo_Click" Visible='<%# ltvArquivo.EditIndex == -1 %>' />
									</td>
									<td>
										<a href="javascript:void(0)" onclick="return openDownload('<%# EditId %>','<%# Item.Id %>');">								
											<img src="../img/download24.png" alt="Baixar" title="Baixar" />
										</a>
										<asp:ImageButton ID="bntRemoverArquivo" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
											OnClick="bntRemoverArquivo_Click" OnClientClick='<%# "return confirmExclusaoArquivo();" %>' 
											Visible='<%# ltvArquivo.EditIndex == -1 %>' />
									</td>
								</tr>
							</ItemTemplate>
							<EditItemTemplate>
								<tr>
									<td></td>
									<td><%# Item.NomeTela %></td>
									<td><asp:TextBox ID="txtDescricao" runat="server" Text='<%# Item.Descricao %>' onkeypress="return disableEnterKey(event)" /></td>
									<td style="width:100px">
										<asp:Button ID="btnSalvarArquivo" runat="server" Text="Salvar"
											OnClick="btnSalvarArquivo_Click" CommandArgument='<%# Item.Id %>'/>
										<asp:ImageButton ID="bntRemoverArquivo" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
											OnClick="bntRemoverArquivo_Click" CommandArgument='<%# Item.NomeTela %>' 
											OnClientClick='<%# "return confirmExclusaoArquivo();" %>'
                                            Visible='<%# string.IsNullOrEmpty(Item.Id) %>'
											/>
									</td>
								</tr>
							</EditItemTemplate>
						</asp:ListView>
						</div>
					</ContentTemplate>
				</asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

