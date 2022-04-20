<%@ Page Title="Área de Trabalho da Reunião" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="AreaReuniao.aspx.cs" Inherits="eVidaIntranet.Reuniao.AreaReuniao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		var POP_ARQUIVO = 2;

		function ConfigControlsCustom() {
			createLocator(650, 550, dlgOpen, null, dlgCallback);
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
				case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.REUNIAO.Value %>'; break;
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

		function openDownload(idReuniao, fId) {
			openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.REUNIAO %>', "ID=" + idReuniao + ";" + fId);    		
			return false;
		}

		function confirmExclusaoArquivo() {
			return confirm("Deseja realmente retirar este arquivo?");
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="100%" border="0">
		<tr>			
			<td colspan="3"><h2>Dados da Reunião</h2></td>
		</tr>
        <tr>
			<td rowspan="4" style="width:50px">&nbsp;</td>
            <td style="width:100px"><b>Título</b></td>
			<td><asp:TextBox ID="txtTitulo" runat="server" Width="250px"/></td>
		</tr>
		<tr>
			<td><b>Conselho:</b></td>
			<td><asp:DropDownList ID="dpdConselho" runat="server" DataTextField="Nome" DataValueField="Codigo"/></td>
		</tr>
		<tr>
			<td><b>Descrição:</b></td>
			<td><asp:TextBox ID="txtDescricao" runat="server" Width="500px" /></td>
		</tr>
		<tr>
			<td><b>Data:</b></td>
			<td><asp:TextBox ID="txtData" runat="server" Width="100px" CssClass="calendario" /></td>
		</tr>
		<tr>
			<td colspan="3" align="center"><br />
				<asp:Button ID="btnSalvar" runat="server" Text="Criar reunião" OnClick="btnSalvar_Click" />
			</td>
		</tr>
	    <tr style="min-height:200px; vertical-align: top">
		    <td align="center" colspan="3">
				<asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
					<ContentTemplate>
						<div id="dvArquivos" runat="server" visible="false">
						<h2>Arquivos da Reunião</h2>		
							<asp:HiddenField ID="hidArqFisico" runat="server" />
							<asp:HiddenField ID="hidArqOrigem" runat="server" />
							<asp:Button ID="btnIncluirArquivo" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArquivo_Click" />
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
											OnClick="bntEditarArquivo_Click" Visible='<%# HasGerencia() && ltvArquivo.EditIndex == -1 %>' />
									</td>
									<td>
										<a href="javascript:void(0)" onclick="return openDownload('<%# Request["ID"] %>','<%# Item.Id %>');">								
											<img src="../img/download24.png" alt="Baixar" title="Baixar" />
										</a>
										<asp:ImageButton ID="bntRemoverArquivo" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
											OnClick="bntRemoverArquivo_Click" OnClientClick='<%# "return confirmExclusaoArquivo();" %>' 
											Visible='<%# HasGerencia() && ltvArquivo.EditIndex == -1 %>' />
									</td>
								</tr>
							</ItemTemplate>
							<EditItemTemplate>
								<tr>
									<td></td>
									<td><%# Item.NomeTela %></td>
									<td><asp:TextBox ID="txtDescricao" runat="server" Text='<%# Item.Descricao %>' onkeypress="return disableEnterKey(event)"  /></td>
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
