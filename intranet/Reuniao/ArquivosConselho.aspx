<%@ Page Title="Arquivos do Órgão" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ArquivosConselho.aspx.cs" Inherits="eVidaIntranet.Reuniao.ArquivosConselho" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		var POP_ARQUIVO = 2;

		function ConfigControlsCustom() {

		}
		function openDownload(idReuniao, fId) {
			openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.CONSELHO %>', "ID=" + idReuniao + ";" + fId);    		
			return false;
		}

	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="100%" border="0">
        <tr>
            <td style="width:200px; text-align: right"><b>Órgão:</b></td>
			<td align="left"><asp:DropDownList ID="dpdConselho" runat="server" Width="300px" DataValueField="Codigo" DataTextField="Nome" /></td>
		</tr>
        <tr>
			<td style="width:200px; text-align: right">Nome:</td>
			<td align="left"><asp:TextBox ID="txtTitulo" runat="server" Width="300px" /></td>
		</tr>
        <tr>
			<td style="width:200px; text-align: right">Descrição:</td>
			<td align="left"><asp:TextBox ID="txtDescricao" runat="server" Width="300px" /></td>
		</tr>
		<tr>
			<td colspan="2" align="center"><br />
				<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnAtualizar_Click" />
			</td>
		</tr>
	    <tr style="min-height:300px; vertical-align: top">
		    <td align="center" colspan="2">
				<asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:ListView ID="ltvArquivo" runat="server" ItemPlaceholderID="row"
							ItemType="eVidaGeneralLib.VO.ArquivoTelaVO" DataKeyNames="Id, NomeTela">
							<EmptyDataTemplate><p style="text-align:center"><b>Não existem arquivos cadastrados.</b></p></EmptyDataTemplate>
							<LayoutTemplate>
								<table class="tabela" style="width:800px">
									<tr>
                                        <th id="colConselho" runat="server">Órgão</th>
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
                                    <td runat="server" visible='<%# string.IsNullOrEmpty(CurrentConselho) %>'><%# Item.Parameters["NOME_CONSELHO"] %></td>
									<td><b><%# Item.Id %></b></td>
									<td><%# Item.NomeTela %></td>
									<td><%# Item.Descricao %></td>
									<td>
										<a href="javascript:void(0)" onclick="return openDownload('<%# Item.Parameters["CONSELHO"] %>','<%# Item.Id %>');">								
											<img src="../img/download24.png" alt="Baixar" title="Baixar" />
										</a>
									</td>
								</tr>
							</ItemTemplate>
						</asp:ListView>
					</ContentTemplate>
				</asp:UpdatePanel>
		    </td>
	    </tr>
    </table>
</asp:Content>
