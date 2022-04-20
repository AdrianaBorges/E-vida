<%@ Page Title="Medicamento/Material Reembolsável" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="MedicamentoReembolsavel.aspx.cs" Inherits="eVidaIntranet.Gestao.MedicamentoReembolsavel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_ARQUIVO = 2;
        var POP_PRINCIPIO = 3;

        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);
            
            var formatPreco = {
                prefix: '',
                centsSeparator: ',',
                thousandsSeparator: '.',
                centsLimit: 2,
                clearPrefix: true,
                allowNegative: false
            };


            $('.inteiro').each(function () {
                $('#' + this.id).ForceNumericOnly();
            });
        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.MEDICAMENTO_REEMBOLSAVEL.Value %>';
                    currentUpload = POP_ARQUIVO;
                    break;
                case POP_PRINCIPIO: src = '../GenPops/PopPrincipioAtivo.aspx';
                    break;
            }
            setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
            switch (handler.tipo) {
                case POP_ARQUIVO: break;
                case POP_PRINCIPIO: defaultDlgCallback(handler, response);
                    break;
            }
        }

        function onAfterUpload(url, originalName) {
            $("#<%:hidArqFisico.ClientID%>").val(url);
            $("#<%:hidArqOrigem.ClientID%>").val(originalName);
            closeLocator();

            if (currentUpload == POP_ARQUIVO) {
                <%= ClientScript.GetPostBackEventReference(btnIncluirArq, "") %>
            } else {
                alert('Upload inválido');
            }
        }


        function openArquivo() {
            var handler = new LocatorHandler(POP_ARQUIVO);
            openLocator("Arquivos", handler);
            return false;
        }

        function openDownload(idMed, fId, isNew) {
            if (isNew) {
                alert('Arquivos Novos não podem ser visualizados. O formulário deve ser salvo para disponibilizar a visualização!');
                return false;
            }
            openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.MEDICAMENTO_REEMBOLSAVEL %>', "ID=" + idMed + ";" + fId);
            return false;
        }

        function confirmeDelete() {
            return confirm("Deseja realmente remover este anexo?");
        }
        function confirmOutroMed() {
            var resp = confirm("Deseja replicar as configurações para os demais medicamentos de mesmo princípio ativo?");
            $("#<%:hidReplica.ClientID%>").val(resp);
            return true;
        }
        function openPopPrincipio(btnLoc) {
            var handler = new LocatorHandler(POP_PRINCIPIO, '<%: hidPrincipio.ClientID %>', -1, btnLoc);
            openLocator("Princípio Ativo", handler);
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
		<asp:HiddenField ID="hidArqComplemento" runat="server" />
		<asp:HiddenField ID="hidArqFisico" runat="server" />
		<asp:HiddenField ID="hidArqOrigem" runat="server" />
        <asp:HiddenField ID="hidReplica" runat="server" />
    <table width="1000px" cellspacing="10px">
        <tr>
            <td colspan="2"><b>Criado por:</b><br />
                <asp:Literal ID="ltCriadoPor" runat="server" Text="-" />
            </td>
            <td colspan="2"><b>Alterado por:</b><br />
                <asp:Literal ID="ltAlteradoPor" runat="server" Text="-"  />
            </td>
        </tr>
	    <tr>
            <td style="width:150px;"><b>Máscara:</b><br />
                <asp:Literal ID="ltMascara" runat="server"/></td>
            <td colspan="2"><b>Descrição:</b><br />
                <asp:Literal ID="ltDescricao" runat="server"/></td>
            <td style="width:350px;" rowspan="2">Planos:<br />
                <asp:CheckBoxList ID="chkLstPlano" runat="server" RepeatColumns="2" Width="350px"
                    DataValueField="Codigo" DataTextField="NomeReduzido" /></td>
	    </tr>
        <tr>
            <td>Reembolsável:<br />
		        <asp:DropDownList ID="dpdReembolsavel" runat="server">
                    <asp:ListItem Value="N" Text="NÃO" />
                    <asp:ListItem Value="S" Text="SIM" />
		        </asp:DropDownList></td>
            <td>Pode ser uso contínuo:<br />
		        <asp:DropDownList ID="dpdUsoContinuo" runat="server">
                    <asp:ListItem Value="N" Text="NÃO" />
                    <asp:ListItem Value="S" Text="SIM" />
		        </asp:DropDownList></td>
        </tr>
        <tr>
		    <td colspan="4"><b>Princípio Ativo:<asp:ImageButton ID="btnLocPrincipio" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopPrincipio(this)" OnClick="btnLocPrincipio_Click" /></b><br />
					<asp:HiddenField ID="hidPrincipio" runat="server" />
                    <asp:Label ID="lblPrincipio" runat="server" Width="220px" />                
					
		    </td>            		    
	    </tr>
        <tr>
		    <td colspan="4"><br /><br />Observações:<br />
                <asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine" Width="80%" /></td>
	    </tr>
        <tr>
		    <td colspan="4" style="text-align:center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" OnClientClick="return confirmOutroMed()" Visible="false" />
                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
		    </td>
	    </tr>
        <tr>
            <td colspan="4">
                <b>Arquivos</b><br />
                <asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
			        <ContentTemplate>
				        <div id="dvArquivos" runat="server">
                            <asp:ListView ID="ltvArquivo" runat="server" GroupPlaceholderID="groupPlaceHolder" ItemPlaceholderID="contentID" ItemType="eVidaGeneralLib.VO.ArquivoTelaVO" GroupItemCount="2">
			                    <LayoutTemplate>
				                    <table class="tabela" style="width:850px" >					                                            
					                    <tr id="groupPlaceHolder" runat="server"/>
				                    </table>
			                    </LayoutTemplate>
			                    <ItemTemplate>
				                    <td style="width:50px"><asp:ImageButton ID="btnRemoverArquivo" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemoverArquivo_Click" 
						                    CommandArgument='<%# Item.NomeFisico %>' Visible='<%# CanEditFile %>' OnClientClick="return confirmeDelete();"/></td>
					                    <td>
                                            <a href="javascript:void(0)" onclick="return openDownload('<%# Id %>', '<%# Item.Id %>', <%# Item.IsNew.ToString().ToLower() %>);">
						                        <%# Item.NomeTela %><%# Item.IsNew ? " - Novo " : "" %>
						                    </a></td>
			                    </ItemTemplate>
                                <GroupTemplate>
                                    <tr runat="server" id="rows" style="background-color: #FFFFFF">
                                        <td runat="server" id="contentID" />
                                    </tr>
                                </GroupTemplate>
                                <ItemSeparatorTemplate>
                                                            
                                </ItemSeparatorTemplate>
                                <EmptyDataTemplate>
                                    <b>Sem arquivos vinculados</b><br />
                                </EmptyDataTemplate>
		                    </asp:ListView>
                            <asp:Button ID="btnIncluirArq" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArq_Click"  UseSubmitBehavior="false" Visible="false"/>
	                                                
                        </div>
			        </ContentTemplate>
		        </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>
