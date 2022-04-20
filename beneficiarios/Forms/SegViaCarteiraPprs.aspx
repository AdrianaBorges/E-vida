<%@ Page Title="2ª Via de Carteira" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="SegViaCarteiraPprs.aspx.cs" Inherits="eVidaBeneficiarios.Forms.SegViaCarteiraPprs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        //---------------------------- ARQUIVOS ---------------------------------
        var POP_ARQUIVO = 1;

        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.BOLETIM_OCORRENCIA.Value %>';
                    break;
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
            openLocator("Arquivo", handler);
            return false;
        }

        function openDownload(idSegViaCarteira, fId, isNew) {
            if (isNew) {
                alert('Arquivos Novos não podem ser visualizados. O formulário deve ser salvo para disponibilizar a visualização!');
                return false;
            }
            openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.BOLETIM_OCORRENCIA %>', "ID=" + idSegViaCarteira + ";" + fId);
            return false;
        }

        function confirmExclusaoArquivo() {
            return confirm("Deseja realmente retirar este arquivo?");
        }

        //------------------------------------------------------------------------------

        function ConfirmQuebraPerda() {
            if (confirm("Ao confirmar esta solicitação os cartões serão emitidos e a cobrança será realizada automaticamente. Só confirme caso tenha certeza. Não há necessidade de entregar o formulário na E-VIDA.")) {
                $('#<%= hidSalvar.ClientID %>').val(new Date());
                __doPostBack('<%= hidSalvar.ClientID  %>', '');
            }
        }
        function openPdf(id) {
            openReport(RELATORIO_SEGUNDA_VIA, "ID=" + id);
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo">
        <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true" />
		<asp:HiddenField ID="hidArqFisico" runat="server" />
		<asp:HiddenField ID="hidArqOrigem" runat="server" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
                <td rowspan="2" valign="top">
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:Label ID="txtCartao" runat="server" Width="150px"/>
                </td>
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtEmailTitular" runat="server" Width="400px" MaxLength="150" />
                </td>
            </tr>
        </table>
        <br />
        <asp:GridView ID="gdvDependentes" runat="server" Width="100%" AutoGenerateColumns="false" OnRowDataBound="gdvDependentes_RowDataBound"
            DataKeyNames="cd_beneficiario">
            <AlternatingRowStyle CssClass="tbDependenteAlt" />
            <RowStyle CssClass="tbDependente" BorderWidth="1px" />
            
            <Columns>
                <asp:TemplateField HeaderText="Seq.">
                    <ItemTemplate>
                        <asp:Label ID="lblRowNum" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Nome Beneficiário" DataField="nm_beneficiario" />
                <asp:BoundField HeaderText="Parentesco" DataField="BRP_DESCRI" />
                <asp:BoundField HeaderText="Plano Vinculado" DataField="BI3_DESCRI" />
                
                <asp:TemplateField HeaderText="Motivo para emissão da segunda via">
                    <ItemTemplate>                     
                        <asp:DropDownList ID="dpdMotivo" runat="server" AutoPostBack="true" Width="170px" OnSelectedIndexChanged="dpdMotivo_SelectedIndexChanged" >
                            <asp:ListItem Value="" Text="NÃO SOLICITAR" />
                            <asp:ListItem Value="Q" Text="Quebra de Cartão" />
                            <asp:ListItem Value="P" Text="Perda" />
                            <asp:ListItem Value="R" Text="Roubo/Furto" />
						</asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Selecionar">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:CheckBox ID="chk" runat="server" AutoPostBack="true" OnCheckedChanged="chk_CheckedChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
				
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlAnexo" runat="server" DefaultButton="btnIncluirArquivo" Visible="false">
            <h2>Boletim de Ocorrência do Roubo/Furto</h2>
            <table id="tbAnexo" border="1" class="tabelaForm" style="width:100%">
                <tr>
                    <td style="padding-left:50px" align="center" >
                        <asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
			                <ContentTemplate>
				                <div id="dvArquivos" runat="server">
	                                <asp:ListView ID="ltvArquivo" runat="server" ItemPlaceholderID="contentID" ItemType="eVidaGeneralLib.VO.ArquivoTelaVO">
			                            <LayoutTemplate>
				                            <table class="tabela" >
					                            <tr>
						                            <th></th>
						                            <th>Nome</th>						
					                            </tr>
					                            <tr id="contentID" runat="server"/>
				                            </table>
			                            </LayoutTemplate>
			                            <ItemTemplate>
				                            <tr>
					                            <td style="width:50px">
                                                    <asp:ImageButton ID="btnRemoverArquivo" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemoverArquivo_Click" CommandArgument='<%# Item.NomeFisico %>' Visible='<%# Item.IsNew || btnIncluirArquivo.Visible %>'/>
					                            </td>
					                            <td>
                                                    <a href="javascript:void(0)" onclick="return openDownload('<%# Id %>','<%# Item.Id %>', <%# Item.IsNew.ToString().ToLower() %>);">
						                            <%# Item.NomeTela %><%# Item.IsNew ? " - Novo " : "" %>
						                            </a>
					                            </td>
				                            </tr>
			                            </ItemTemplate>
		                            </asp:ListView>
                    
					                <asp:Button ID="btnIncluirArquivo" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArquivo_Click"  UseSubmitBehavior="false"/>
                                </div>
			                </ContentTemplate>
		                </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <h2>2 - INFORMAÇÕES</h2>
	    <div class="observacao">
		    <p>Em casos de Roubo ou Furto, o titular deve anexar a esta solicitação a cópia do Boletim de Ocorrência Policial. Nestes casos, não haverá cobrança para emissão da segunda via.</p>
	    </div>

        <h2>3 - DECLARACAO</h2>
	    <div class="observacao">
		    <p>Declaro estar ciente que pagarei pelo valor integral do custo para emissão da 2ª via de cada carteira solicitada, nos casos de Quebra ou Perda do Cartão, conforme estabelecido nos normativos dos planos da E-VIDA.</p>
            <p>Em caso de uso indevido, estou ciente que participarei integralmente nas despesas referentes à utilização dos planos a que estou vinculado.</p>
	    </div>
        
        <div>
            <br />
	        (Local e Data) <asp:TextBox ID="txtLocal" runat="server" Width="150px" CssClass="inputInline" />, <%= String.Format("{0:dd \\de MMMM \\de yyyy}", DateTime.Now) %> 
	        <br />            
	    </div>

        <div>
            <table width="50%">
                <tr>
                    <td align="center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" /><asp:HiddenField ID="hidSalvar" runat="server" OnValueChanged="hidSalvar_ValueChanged" /></td>
                    <td align="center"><asp:ImageButton ID="btnPdf" ImageUrl="../img/print.png" runat="server" ToolTip="Gerar PDF" Width="50px" CssClass="printer" Visible="false" /></td>
                    <td align="center"><asp:Button ID="btnNova" runat="server" Text="Nova Solicitação" OnClick="btnNova_Click" Visible="false" /></td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
